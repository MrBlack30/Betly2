using OpenQA.Selenium;

namespace Betly.Tests.Selenium.Pages
{
    public class AddCreditPage : BasePage
    {
        public AddCreditPage(IWebDriver driver) : base(driver) { }

        public void AddCredit(decimal amount)
        {
            SendKeys(By.Id("Amount"), amount.ToString());
            Click(By.CssSelector("button[type='submit']"));
            WaitForSuccess();
        }
    }
}
