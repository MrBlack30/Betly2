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
                       r.Id, r.Email, 
                       a.Id, a.Email
                FROM Friendships f
                JOIN Users r ON f.RequesterId = r.Id
                JOIN Users a ON f.AddresseeId = a.Id
                WHERE (f.RequesterId = @UserId OR f.AddresseeId = @UserId) AND f.Status = 'Accepted'";
            
            var friendships = await connection.QueryAsync<Friendship, User, User, Friendship>(
                sql,
                (friendship, requester, addressee) =>
                {
                    friendship.Requester = requester;
                    friendship.Addressee = addressee;
                    return friendship;
                },
                new { UserId = userId },
                splitOn: "Id,Id"
            );
            
            return friendships.ToList();
        }
        
        public async Task<List<Friendship>> GetPendingRequestsByUserIdAsync(int userId)
        {
            using var connection = CreateConnection();
            // Incoming requests: Requester is someone else, Addressee is ME.
            var sql = @"
                SELECT f.*, 
                       r.Id, r.Email
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
                splitOn: "Id"
            );

            return requests.ToList();
        }

        public async Task<List<Friendship>> GetSentRequestsByUserIdAsync(int userId)
        {
            using var connection = CreateConnection();
            // Outgoing requests: Requester is ME, Addressee is someone else.
            var sql = @"
                SELECT f.*, 
                       a.Id, a.Email
                FROM Friendships f
                JOIN Users a ON f.AddresseeId = a.Id
                WHERE f.RequesterId = @UserId AND f.Status = 'Pending'";

             var requests = await connection.QueryAsync<Friendship, User, Friendship>(
                sql,
                (friendship, addressee) =>
                {
                    friendship.Addressee = addressee;
                    return friendship;
                },
                new { UserId = userId },
                splitOn: "Id"
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
