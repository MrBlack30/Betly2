using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Betly.Tests.Selenium.Drivers
{
    public static class WebDriverFactory
    {
        public static IWebDriver CreateDriver()
        {
            var options = new EdgeOptions();
            // options.AddArgument("--headless"); 
            options.AddArgument("--start-maximized");
            
            return new EdgeDriver(options);
        }
    }
}
