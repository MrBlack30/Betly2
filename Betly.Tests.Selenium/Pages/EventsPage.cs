using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Betly.Tests.Selenium.Pages
{
    public class EventsPage : BasePage
    {
        public EventsPage(IWebDriver driver) : base(driver) { }

        public void Navigate()
        {
            NavigateTo("Events/Index");
        }

        public bool IsEventPresent(string title)
        {
             var xpath = $"//h5[contains(@class, 'card-title') and contains(., '{title}')]";
             
             // First attempt
             if (IsElementVisible(By.XPath(xpath), timeoutSeconds: 5)) return true;
             
             // Second attempt with refresh (handles rare timing issues where Index doesn't show new event immediately)
             Driver.Navigate().Refresh();
             return IsElementVisible(By.XPath(xpath), timeoutSeconds: 10);
        }

        private bool IsElementVisible(By by, int timeoutSeconds)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(by));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ClickPlaceBet(string title)
        {
            // Find the card container first
            var xpath = $"//div[contains(@class, 'card')][.//h5[contains(., '{title}')]]";
            var card = WaitAndFindElement(By.XPath(xpath));
            // Find the Place Bet button within that card
            var btn = card.FindElement(By.LinkText("Place Bet"));
            ClickElement(btn);
        }

        public void ClickResolveEvent(string title)
        {
            var xpath = $"//div[contains(@class, 'card')][.//h5[contains(., '{title}')]]";
            var card = WaitAndFindElement(By.XPath(xpath));
            var btn = card.FindElement(By.LinkText("Resolve Event"));
            ClickElement(btn);
        }
    }
}
