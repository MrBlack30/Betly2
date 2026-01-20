using Betly.Tests.Selenium.Pages;
using NUnit.Framework;

namespace Betly.Tests.Selenium.Tests
{
    [TestFixture]
    public class BettingTests : BaseTest
    {
        [Test]
        public void PlaceBetOnEvent_BettingFlow()
        {
            var creatorEmail = GenerateUniqueEmail();
            var password = "Password123!";
            RegisterAndLogin(creatorEmail, password);
             
            var createPage = new CreateEventPage(Driver);
            var dashboard = new DashboardPage(Driver);
            
            dashboard.ClickCreateEvent();
            string eventTitle = $"TestEvent_{Guid.NewGuid()}";
            createPage.FillEventForm(eventTitle, "TeamA", "TeamB", 1.5m, 2.0m, 3.0m, isPublic: true);
            createPage.Submit();
            
            dashboard.LogOut();

            var bettorEmail = GenerateUniqueEmail();
            RegisterAndLogin(bettorEmail, password);

            dashboard.ClickAddCredit();
            new AddCreditPage(Driver).AddCredit(100);
            
            var eventsPage = new EventsPage(Driver);
            eventsPage.Navigate();
            
            Assert.That(eventsPage.IsEventPresent(eventTitle), Is.True, "Event should be visible");
            eventsPage.ClickPlaceBet(eventTitle);

            var betPage = new PlaceBetPage(Driver);
            decimal betAmount = 50m;
            betPage.PlaceBet("TeamA", betAmount);

            var balanceAfter = dashboard.GetBalance();
            Assert.That(balanceAfter, Is.EqualTo(50m), "Balance should decrease by bet amount");

            dashboard.Navigate(); 
            bool betExists = dashboard.IsBetPresentInTable(eventTitle, "TeamA");
            Assert.That(betExists, Is.True, "Bet should be present in My Bets");

            string? status = dashboard.GetBetStatus(eventTitle);
            Assert.That(status, Is.EqualTo("Pending") | Is.EqualTo("Active"), "Bet status should be Pending");
        }
    }
}
