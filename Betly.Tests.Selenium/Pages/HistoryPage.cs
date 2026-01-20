using OpenQA.Selenium;
using System.Linq;

namespace Betly.Tests.Selenium.Pages
{
    public class HistoryPage : BasePage
    {
        public HistoryPage(IWebDriver driver) : base(driver) { }

        public bool IsTransactionPresent(string type, decimal amount)
        {
            // Simple check: Look for a row containing both the type and the amount text
            // In a robust implementation, we'd parse the table rows.
            // XPath: //tr[contains(., 'Deposit') and contains(., '$100.00')] (Currency symbol might vary)
            
            // Removing currency symbol from check to be safer, or checking part of expected string
            var amountStr = amount.ToString("N2"); // e.g. 100.00
            
            // Constructing a flexible XPath
            var xpath = $"//tr[.//td[contains(., '{type}')] and .//td[contains(., '{amountStr}')]]";
            return IsElementPresent(By.XPath(xpath));
        }
    }
}
