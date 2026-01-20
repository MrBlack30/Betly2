using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Betly.Tests.Selenium.Pages
{
    public class PlaceBetPage : BasePage
    {
        public PlaceBetPage(IWebDriver driver) : base(driver) { }

        public void PlaceBet(string outcome, decimal amount)
        {
            // Select Outcome
            var select = new SelectElement(WaitAndFindElement(By.Id("outcomeSelect")));
            // We can select by value (TeamA name) or Text. The view puts TeamName as value.
            // But if outcome is "Draw", value is "Draw".
            // If outcome matches TeamA name, it works.
            select.SelectByValue(outcome);

            // Enter Amount
            SendKeys(By.Id("wagerAmount"), amount.ToString());

            // Click Review (Show Modal)
            Click(By.XPath("//button[contains(text(), 'Review & Place Bet')]"));

            // Wait for Modal and Click Confirm
            var confirmBtn = Wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), 'Confirm & Place Bet')]")));
            ClickElement(confirmBtn);
            WaitUntilUrlChanges("Events/PlaceBet");
        }
    }
}
