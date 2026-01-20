using OpenQA.Selenium;

namespace Betly.Tests.Selenium.Pages
{
    public class FriendsPage : BasePage
    {
        public FriendsPage(IWebDriver driver) : base(driver) { }

        public void Navigate()
        {
            NavigateTo("Account/Friends");
        }

        public void AddFriend(string email)
        {
            SendKeys(By.Name("email"), email);
            Click(By.XPath("//button[contains(text(), 'Send Request')]"));
            WaitForSuccess();
        }

        public void AcceptFriendRequest(string fromEmail)
        {
            // Find the list item containing the email, then find the accept form/button
            var xpath = $"//li[contains(., '{fromEmail}')]//button[contains(text(), 'Accept')]";
            Click(By.XPath(xpath));
            WaitForSuccess();
        }

        public bool IsFriendListed(string email)
        {
            return IsElementPresent(By.XPath($"//h6[contains(text(), '{email}')]"));
        }
    }
}
