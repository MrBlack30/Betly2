using OpenQA.Selenium;

namespace Betly.Tests.Selenium.Pages
{
    public class DashboardPage : BasePage
    {
        public DashboardPage(IWebDriver driver) : base(driver) { }

        public void Navigate()
        {
            NavigateTo("Account/Dashboard");
        }

        public decimal GetBalance()
        {
            var element = WaitAndFindElement(By.CssSelector(".card-title.text-success"));
            var text = element.Text.Replace("Balance:", "").Replace("$", "").Replace("₪", "").Trim();
            // Handle potentially different currency symbols or formatting
             // Remove any non-numeric characters except decimal point
            var numericText = new string(text.Where(c => char.IsDigit(c) || c == '.').ToArray());
            return decimal.Parse(numericText);
        }

        public void ClickAddCredit()
        {
            Click(By.CssSelector("a[href*='AddCredit']"));
        }

        public void ClickCreateEvent()
        {
             Click(By.CssSelector("a[href*='Events/Create']"));
        }

        public void ClickTransactionHistory()
        {
            Click(By.CssSelector("a[href*='Account/History']"));
        }

        public void LogOut()
        {
            NavigateTo("Account/Logout");
            Wait.Until(d => d.Url.Contains("Account/Login", StringComparison.OrdinalIgnoreCase));
        }

        public bool IsBetPresentInTable(string eventTitle, string outcome)
        {
             // Simple check if text exists in table
             // In a real scenario, we might iterate rows
             return IsElementPresent(By.XPath($"//td[contains(text(), '{eventTitle}')]"));
        }

         public string? GetBetStatus(string eventTitle)
        {
             // Simplified XPath to find the status badge in the same row as the event title
             // Assuming structure: tr -> td(Title) ... td(Status > span)
             try
             {
                var element = Driver.FindElement(By.XPath($"//td[contains(text(), '{eventTitle}')]/..//span[contains(@class, 'badge')]"));
                return element.Text;
             }
             catch (NoSuchElementException)
             {
                 return null;
             }
        }
    }
}
