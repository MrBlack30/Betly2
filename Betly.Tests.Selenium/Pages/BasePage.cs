using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Betly.Tests.Selenium.Pages
{
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;
        protected virtual string BaseUrl => "http://localhost:5000"; // Adjust port if necessary

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        protected void NavigateTo(string path)
        {
            var url = $"{BaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
            Driver.Navigate().GoToUrl(url);
            // Wait for URL to at least contain the path or not be about:blank
            Wait.Until(d => !d.Url.Contains("about:blank"));
        }

        protected void WaitForUrl(string part, bool exact = false)
        {
            if (exact)
                Wait.Until(d => d.Url.TrimEnd('/').Equals($"{BaseUrl.TrimEnd('/')}/{part.TrimStart('/')}".TrimEnd('/'), StringComparison.OrdinalIgnoreCase));
            else
                Wait.Until(d => d.Url.Contains(part, StringComparison.OrdinalIgnoreCase));
        }

        protected void WaitUntilUrlChanges(string oldUrlPart)
        {
            Wait.Until(d => !d.Url.Contains(oldUrlPart, StringComparison.OrdinalIgnoreCase));
        }

        protected void WaitForSuccess()
        {
            Wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert-success")));
        }

        protected IWebElement WaitAndFindElement(By by)
        {
            try 
            {
                return Wait.Until(ExpectedConditions.ElementIsVisible(by));
            }
            catch (WebDriverTimeoutException ex)
            {
                var currentUrl = Driver.Url;
                throw new WebDriverTimeoutException($"Timed out waiting for element {by} on page: {currentUrl}", ex);
            }
        }
        
        protected void SendKeys(By by, string text)
        {
            var element = WaitAndFindElement(by);
            element.Clear();
            element.SendKeys(text);
        }

        protected void Click(By by)
        {
            var element = WaitAndFindElement(by);
            ClickElement(element);
        }

        protected void ClickElement(IWebElement element)
        {
            Wait.Until(ExpectedConditions.ElementToBeClickable(element));
            try 
            {
                element.Click();
            } 
            catch (ElementClickInterceptedException) 
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);
            }
        }

         protected bool IsElementPresent(By by)
        {
            try
            {
                Driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
