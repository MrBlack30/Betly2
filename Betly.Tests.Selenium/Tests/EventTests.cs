using Betly.Tests.Selenium.Pages;
using NUnit.Framework;

namespace Betly.Tests.Selenium.Tests
{
    [TestFixture]
    public class EventTests : BaseTest
    {
        [Test]
        public void CreatePublicEvent_VisibleToOthers()
        {
            var creatorEmail = GenerateUniqueEmail();
            var password = "Password123!";
            RegisterAndLogin(creatorEmail, password);

            var dashboard = new DashboardPage(Driver);
            dashboard.ClickCreateEvent();
            string eventTitle = $"PublicEvent_{Guid.NewGuid()}";
            var createPage = new CreateEventPage(Driver);
            createPage.FillEventForm(eventTitle, "TeamA", "TeamB", 1.5m, 1.5m, 1.5m, isPublic: true);
            createPage.Submit();
            
            dashboard.LogOut();

            var otherEmail = GenerateUniqueEmail();
            RegisterAndLogin(otherEmail, password);

            var eventsPage = new EventsPage(Driver);
            eventsPage.Navigate();
            Assert.That(eventsPage.IsEventPresent(eventTitle), Is.True, "Public event should be visible to others");
        }

        [Test]
        public void CreatePrivateEvent_OnlyInvitedFriendsCanSee()
        {
            var password = "Password123!";
            var userA = GenerateUniqueEmail(); // Creator
            var userB = GenerateUniqueEmail(); // Friend & Invited
            var userC = GenerateUniqueEmail(); // Stranger

            RegisterAndLogin(userB, password);
             new DashboardPage(Driver).LogOut();

            RegisterAndLogin(userA, password);
            var friendsPage = new FriendsPage(Driver);
            friendsPage.Navigate();
            friendsPage.AddFriend(userB);
            new DashboardPage(Driver).LogOut();

            // Login B, Accept Request
            LoginOnly(userB, password);
            friendsPage.Navigate();
            friendsPage.AcceptFriendRequest(userA);
            new DashboardPage(Driver).LogOut();
 
            // Login A, Create Private Event
            LoginOnly(userA, password);
            var dashboard = new DashboardPage(Driver);
            dashboard.ClickCreateEvent();
            string eventTitle = $"PrivateEvent_{Guid.NewGuid()}";
            var createPage = new CreateEventPage(Driver);
            
            // Fill form as PRIVATE
            createPage.FillEventForm(eventTitle, "TeamA", "TeamB", 2.0m, 2.0m, 3.0m, isPublic: false);
            // Invite B
            createPage.InviteFriend(userB);
            createPage.Submit();
            dashboard.LogOut();
 
            // Login B -> Should See
            LoginOnly(userB, password);
            var eventsPage = new EventsPage(Driver);
            eventsPage.Navigate();
            Assert.That(eventsPage.IsEventPresent(eventTitle), Is.True, "Invited friend should see private event");
            dashboard.LogOut();

            // Login C -> Should NOT See
            RegisterAndLogin(userC, password);
            eventsPage.Navigate();
            Assert.That(eventsPage.IsEventPresent(eventTitle), Is.False, "Uninvited user should NOT see private event");
        }
    }
}
