using OpenQA.Selenium;

namespace Betly.Tests.Selenium.Pages
{
    public class CreateEventPage : BasePage
    {
        public CreateEventPage(IWebDriver driver) : base(driver) { }

        public void FillEventForm(string title, string teamA, string teamB, decimal oddsA, decimal oddsB, decimal oddsDraw, bool isPublic)
        {
            SendKeys(By.Id("Title"), title);
            SendKeys(By.Id("Description"), "Automated Test Event");
            SendKeys(By.Id("TeamA"), teamA);
            SendKeys(By.Id("TeamB"), teamB);
            
            SendKeys(By.Id("OddsTeamA"), oddsA.ToString());
            SendKeys(By.Id("OddsTeamB"), oddsB.ToString());
            SendKeys(By.Id("OddsDraw"), oddsDraw.ToString());
            
            // Use JavaScript to set the value directly for datetime-local, as SendKeys is often flaky here.
            // Ensure the date is in 2026 as requested (Metadata shows current time is Jan 2026)
            var eventDate = DateTime.Now.AddDays(1);
            if (eventDate.Year != 2026)
            {
                eventDate = new DateTime(2026, eventDate.Month, eventDate.Day, eventDate.Hour, eventDate.Minute, 0);
            }
            var dateStr = eventDate.ToString("yyyy-MM-ddTHH:mm");
            var dateInput = WaitAndFindElement(By.Id("Date"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].value = arguments[1];", dateInput, dateStr);

            var publicCheckbox = WaitAndFindElement(By.Id("isPublicCheckbox"));
            if (isPublic && !publicCheckbox.Selected)
            {
                ClickElement(publicCheckbox);
            }
            else if (!isPublic && publicCheckbox.Selected)
            {
                ClickElement(publicCheckbox);
            }
        }

        public void InviteFriend(string friendEmail)
        {
             // Assumes Private is selected so list is visible
             // Label text is the email
             var label = WaitAndFindElement(By.XPath($"//label[contains(., '{friendEmail}')]"));
             var checkboxId = label.GetAttribute("for");
             if (string.IsNullOrEmpty(checkboxId)) return;
             
             var checkbox = WaitAndFindElement(By.Id(checkboxId));
             if(!checkbox.Selected) ClickElement(checkbox);
        }

        public void Submit()
        {
            Click(By.CssSelector("button[type='submit']"));
            WaitUntilUrlChanges("Events/Create");
        }
    }
}
