using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Selenium.Utils.Extensions
{
    public static class WebDriverExtensions
    {
        public static void CarregarPagina(this IWebDriver driver, TimeSpan timeOut, string url)
        {
            driver.Manage().Timeouts().PageLoad = timeOut;
            driver.Navigate().GoToUrl(url);
        }

        public static string BuscarHtml(this IWebDriver driver, By by, string atributo)
        {
            IWebElement webElement = driver.FindElement(by);
            return webElement.GetAttribute(atributo);
        }

        public static void IncluirValor(this IWebDriver webDriver, By by, string value)
        {
            IWebElement webElement = webDriver.FindElement(by);
            webElement.SendKeys(value);
        }

        public static void SubmeterBotao(this IWebDriver driver, By by)
        {
            IWebElement webElement = driver.FindElement(by);
            webElement.Submit();
        }

        public static void ClicarBotao(this IWebDriver driver, By by)
        {
            IWebElement webElement = driver.FindElement(by);
            webElement.Click();
        }

        public static ReadOnlyCollection<IWebElement> BuscarElementos(this IWebDriver driver, By by)
        {
            ReadOnlyCollection<IWebElement> webElements = driver.FindElements(by);
            return webElements;
        }
    }
}
