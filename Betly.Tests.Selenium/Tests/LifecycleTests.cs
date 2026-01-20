using Betly.Tests.Selenium.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Betly.Tests.Selenium.Tests
{
    [TestFixture]
    public class LifecycleTests : BaseTest
    {
        [Test]
        public void EventClosure_Payout()
        {
            var password = "Password123!";
            var creatorEmail = GenerateUniqueEmail();
            var bettorEmail = GenerateUniqueEmail();
            var loserEmail = GenerateUniqueEmail();

            // 1. Creator creates event
            RegisterAndLogin(creatorEmail, password);
            var dashboard = new DashboardPage(Driver);
            dashboard.ClickCreateEvent();
            string eventTitle = $"LifecycleEvent_{Guid.NewGuid()}";
            var createPage = new CreateEventPage(Driver);
            createPage.FillEventForm(eventTitle, "TeamA", "TeamB", 2.0m, 3.0m, 3.0m, isPublic: true);
            createPage.Submit();
            dashboard.LogOut();

            // 2. Bettor bets on TeamA (Odds 2.0)
            RegisterAndLogin(bettorEmail, password);
            dashboard.ClickAddCredit();
            new AddCreditPage(Driver).AddCredit(100); 
            
            var eventsPage = new EventsPage(Driver);
            eventsPage.Navigate();
            eventsPage.ClickPlaceBet(eventTitle);
            new PlaceBetPage(Driver).PlaceBet("TeamA", 50); 
            dashboard.LogOut();

            // 3. Loser bets on TeamB (To create a pool to split)
            RegisterAndLogin(loserEmail, password);
            dashboard.ClickAddCredit();
            new AddCreditPage(Driver).AddCredit(50);
            eventsPage.Navigate();
            eventsPage.ClickPlaceBet(eventTitle);
            new PlaceBetPage(Driver).PlaceBet("TeamB", 50); 
            dashboard.LogOut();

            // 4. Creator resolves event (TeamA wins)
            LoginOnly(creatorEmail, password);
            eventsPage.Navigate();
            eventsPage.ClickResolveEvent(eventTitle);
            
            var resolvePage = new ResolveEventPage(Driver);
            resolvePage.Resolve("TeamA");
            
            dashboard.LogOut();

            // 5. Bettor verifies Win
            // Started 100. AddCredit 100. Bet 50. Balance 50.
            // Pool: 50 (Bettor) + 50 (Loser) = 100.
            // Bettor is the only winner, so takes the whole pool (100).
            // Final balance: 50 + 100 = 150.
            LoginOnly(bettorEmail, password);
            dashboard.Navigate();
            
            var balanceEnd = dashboard.GetBalance();
            Assert.That(balanceEnd, Is.EqualTo(150m), "Balance should reflect payout (initial 100 - bet 50 + pool winnings 100)");

            var status = dashboard.GetBetStatus(eventTitle);
            Assert.That(status, Is.EqualTo("Won"), "Bet status should be Won");

            dashboard.ClickTransactionHistory();
            var historyPage = new HistoryPage(Driver);
            Assert.That(historyPage.IsTransactionPresent("Win", 100m), Is.True, "Win transaction should appear");
        }
    }
}
