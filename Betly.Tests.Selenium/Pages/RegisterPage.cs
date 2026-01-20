using OpenQA.Selenium;

namespace Betly.Tests.Selenium.Pages
{
    public class RegisterPage : BasePage
    {
        public RegisterPage(IWebDriver driver) : base(driver) { }

        public void Navigate()
        {
            NavigateTo("Account/Register");
        }

        public void Register(string email, string password)
        {
            SendKeys(By.Id("Email"), email);
            SendKeys(By.Id("Password"), password);
            Click(By.CssSelector("input[type='submit']"));
        }
    }
}
