using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Betly.Tests.Selenium.Pages
{
    public class ResolveEventPage : BasePage
    {
        public ResolveEventPage(IWebDriver driver) : base(driver) { }

        public void Resolve(string winningOutcome)
        {
            var element = WaitAndFindElement(By.Name("WinningOutcome"));
            var winnerSelect = new SelectElement(element);
            winnerSelect.SelectByValue(winningOutcome);
            Click(By.CssSelector("button[type='submit']"));
            WaitUntilUrlChanges("Events/Resolve");
        }
    }
}
