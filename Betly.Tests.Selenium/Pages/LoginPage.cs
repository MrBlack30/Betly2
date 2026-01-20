using OpenQA.Selenium;

namespace Betly.Tests.Selenium.Pages
{
    public class LoginPage : BasePage
    {
        public LoginPage(IWebDriver driver) : base(driver) { }

        public void Navigate()
        {
            NavigateTo("Account/Login");
        }

        public void Login(string email, string password)
        {
            SendKeys(By.Id("Email"), email);
            SendKeys(By.Id("Password"), password);
            Click(By.CssSelector("button[type='submit']"));
        }
    }
}
