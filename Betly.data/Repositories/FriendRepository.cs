using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Betly.core.Interfaces;
using Betly.core.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Betly.data.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly string _connectionString;

        public FriendRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BetlyDbConnection") 
                                ?? throw new System.Exception("Connection string 'BetlyDbConnection' not found.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task AddFriendshipAsync(Friendship friendship)
        {
            using var connection = CreateConnection();
            var sql = "INSERT INTO Friendships (RequesterId, AddresseeId, Status, CreatedAt) VALUES (@RequesterId, @AddresseeId, @Status, @CreatedAt)";
            await connection.ExecuteAsync(sql, friendship);
        }

        public async Task<Friendship?> GetFriendshipAsync(int userAId, int userBId)
        {
            using var connection = CreateConnection();
            // Check for friendship in either direction
            var sql = "SELECT * FROM Friendships WHERE (RequesterId = @UserA AND AddresseeId = @UserB) OR (RequesterId = @UserB AND AddresseeId = @UserA)";
            return await connection.QueryFirstOrDefaultAsync<Friendship>(sql, new { UserA = userAId, UserB = userBId });
        }

        public async Task<List<Friendship>> GetFriendshipsByUserIdAsync(int userId)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT f.*, 
                       r.Id AS RId, r.Email AS REmail, 
                       a.Id AS AId, a.Email AS AEmail
                FROM Friendships f
                JOIN Users r ON f.RequesterId = r.Id
                JOIN Users a ON f.AddresseeId = a.Id
                WHERE (f.RequesterId = @UserId OR f.AddresseeId = @UserId) AND f.Status = 'Accepted'";

            // Mapping to populate user details if needed, for simplicity let's just return the friendship
            // Ideally we join to get friends details.
            // Let's assume we want to fill the 'Friend' details into one of the User slots?
            // Actually, let's keep it simple and just do a multi-map if possible or just return relations.
            // Given the simple requirement "see all his friends", let's load friend info.
            
            var friendships = await connection.QueryAsync<Friendship, User, User, Friendship>(
                sql,
                (friendship, requester, addressee) =>
                {
                    friendship.Requester = requester;
                    friendship.Addressee = addressee;
                    return friendship;
                },
                new { UserId = userId },
                splitOn: "RId,AId"
            );
            
            return friendships.ToList();
        }
        
        public async Task<List<Friendship>> GetPendingRequestsByUserIdAsync(int userId)
        {
            using var connection = CreateConnection();
            // Incoming requests: Requester is someone else, Addressee is ME.
            var sql = @"
                SELECT f.*, 
                       r.Id AS RId, r.Email AS REmail
                FROM Friendships f
                JOIN Users r ON f.RequesterId = r.Id
                WHERE f.AddresseeId = @UserId AND f.Status = 'Pending'";

             var requests = await connection.QueryAsync<Friendship, User, Friendship>(
                sql,
                (friendship, requester) =>
                {
                    friendship.Requester = requester;
                    return friendship;
                },
                new { UserId = userId },
                splitOn: "RId"
            );

            return requests.ToList();
        }

        public async Task UpdateFriendshipAsync(Friendship friendship)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Friendships SET Status = @Status WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { friendship.Status, friendship.Id });
        }
    }
}
