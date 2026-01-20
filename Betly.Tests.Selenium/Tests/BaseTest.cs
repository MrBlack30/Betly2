using Betly.Tests.Selenium.Drivers;
using Betly.Tests.Selenium.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

[assembly: LevelOfParallelism(1)] // Run tests sequentially to avoid state collisions

namespace Betly.Tests.Selenium.Tests
{
    public abstract class BaseTest
    {
        protected IWebDriver Driver;
        protected WebDriverWait Wait;
        
        [SetUp]
        public void Setup()
        {
            Driver = WebDriverFactory.CreateDriver();
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }

        [TearDown]
        public void Teardown()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }

        protected void RegisterAndLogin(string email, string password)
        {
            // Register
            var registerPage = new RegisterPage(Driver);
            registerPage.Navigate();
            registerPage.Register(email, password);
            
            // Wait for redirect to Login
            try 
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.Url.Contains("Login"));
            }
            catch (WebDriverTimeoutException) 
            {
                // Ignore, might stay on page if error, will be caught by next step
            }

            if (Driver.Url.Contains("Login"))
            {
                 var loginPage = new LoginPage(Driver);
                 loginPage.Login(email, password);
            }

            // Final check: User should be on Dashboard or at least not on Login/Register
            Wait.Until(d => d.Url.Contains("Dashboard") || !d.Url.Contains("Account/"));
            Assert.That(Driver.Url, Does.Not.Contain("Login"), $"Login failed, still on Login page. URL: {Driver.Url}");
            Assert.That(Driver.Url, Does.Not.Contain("Register"), $"Registration failed, still on Register page. URL: {Driver.Url}");
        }
        
        protected void LoginOnly(string email, string password)
        {
            var loginPage = new LoginPage(Driver);
            loginPage.Navigate();
            loginPage.Login(email, password);

            // Wait for redirect
            Wait.Until(d => d.Url.Contains("Dashboard") || !d.Url.Contains("Account/"));
            Assert.That(Driver.Url, Does.Not.Contain("Login"), $"Login failed for {email}. URL: {Driver.Url}");
        }

        protected string GenerateUniqueEmail()
        {
            return $"user{Guid.NewGuid().ToString().Substring(0,8)}@test.com";
        }
    }
}
