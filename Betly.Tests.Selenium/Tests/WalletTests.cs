using Betly.Tests.Selenium.Pages;
using NUnit.Framework;

namespace Betly.Tests.Selenium.Tests
{
    [TestFixture]
    public class WalletTests : BaseTest
    {
        [Test]
        public void DepositAndVerifyBalance_WalletFlow()
        {
            // 1. Login with existing (newly created) user
            var email = GenerateUniqueEmail();
            var password = "Password123!";
            RegisterAndLogin(email, password);

            var dashboard = new DashboardPage(Driver);
            var initialBalance = dashboard.GetBalance();

            // 2. Go to Add Credit
            dashboard.ClickAddCredit();

            // 3. Enter Amount (100)
            var addCreditPage = new AddCreditPage(Driver);
            decimal depositAmount = 100m;
            addCreditPage.AddCredit(depositAmount);

            // 4. Navigate to Transaction History (Implicitly returned to Dashboard, but requirements say Navigate to History)
            // Let's verify dashboard balance first
            var newBalance = dashboard.GetBalance();
            Assert.That(newBalance, Is.EqualTo(initialBalance + depositAmount), "Balance should increase by 100");

            dashboard.ClickTransactionHistory();

            // 5. Expected Result: Transaction appears
            var historyPage = new HistoryPage(Driver);
            bool exists = historyPage.IsTransactionPresent("Deposit", depositAmount);
            Assert.That(exists, Is.True, "Deposit transaction should appear in history");
        }
    }
}
