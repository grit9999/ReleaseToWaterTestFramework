using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;

namespace ReleaseToWaterTestScenarioFrameWork
{
    [TestFixture]
    public class Tests
    {
        IWebDriver webDriver;
        WebDriverWait wait;
        Actions mouseActions;
        const string TestDescriptionOne = "test description one";
        const string TestDescriptionTwo = "test description two";
        readonly string chromeDriverName = "chromedriver";
        readonly string siteUrl = "https://stirling.she-development.net/automation";

        [SetUp]
        public void Setup()
        {
            webDriver = new ChromeDriver();
            webDriver.Manage().Window.Maximize();
            webDriver.Navigate().GoToUrl(siteUrl);
            wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));            
        }

        [Test]
        public void VerifyReleaseToWaterRecordDeleted()
        {            
            IWebElement username = webDriver.FindElement(By.Id("username"));
            username.SendKeys("SukhjivanD");
            wait.Until(driver => username.GetAttribute("value") == "SukhjivanD");
                        
            IWebElement password = webDriver.FindElement(By.Id("password"));
            password.SendKeys("yMtdhOwfP%HZ");
            wait.Until(driver => password.GetAttribute("value") == "yMtdhOwfP%HZ");

            webDriver.FindElement(By.Id("login")).Click();
            wait.Until(driver => driver.FindElements(By.CssSelector("div.headercolumn-center > div:nth-child(2) li.she-has-submenu a")).Any());

            IWebElement webElement = webDriver.FindElement(By.CssSelector("div.headercolumn-center > div:nth-child(2) li.she-has-submenu a"));
            webElement.Click();
            wait.Until(driver => driver.FindElements(By.CssSelector("ul.she-nav-modules")).Any(element => element.Displayed));
            IWebElement element = webDriver.FindElement(By.CssSelector("ul.she-nav-modules"));            
            
            mouseActions = new Actions(webDriver);
            IWebElement environmentMenuOption = webDriver.FindElement(By.CssSelector("li[data-areaname='Environment'] a"));
            mouseActions.MoveToElement(environmentMenuOption).Perform();

            wait.Until(driver => driver.FindElements(By.CssSelector("a[href='/automation/Environment/ReleaseToWater/Page/1']"))).Any(element => element.Displayed);
            IWebElement releaseToWaterMenuOption = webDriver.FindElement(By.CssSelector("a[href='/automation/Environment/ReleaseToWater/Page/1']"));
            releaseToWaterMenuOption.Click();
            wait.Until(driver => driver.FindElements(By.CssSelector("section[role='main'] a[href='/automation/Environment/ReleaseToWater/Create']"))).Any(element => element.Displayed);

            //add a record
            AddAReleaseToWaterRecordAndSave(TestDescriptionOne);
            //Add another record
            AddAReleaseToWaterRecordAndSave(TestDescriptionTwo);

            IWebElement lastSearchResultsButton = webDriver.FindElement(By.XPath("//a[text()='Last']"));
            lastSearchResultsButton.Click();
            wait.Until(driver => driver.FindElement(By.CssSelector("div[class='information'] a[title='test description one']")).Displayed);

            DeleteRecord(TestDescriptionOne);
        }

        private void DeleteRecord(string recordDescriptionToDelete)
        {
            mouseActions = new Actions(webDriver);
            IWebElement sheLogo = webDriver.FindElement(By.Id("small-she-logo"));
            mouseActions.MoveToElement(sheLogo).Perform();

            IList<IWebElement> listOfRecordsDisplayed = webDriver.FindElements(By.CssSelector("div[class^='list_layout']"));
            IWebElement recordToDelete = listOfRecordsDisplayed.Where(element => element.Text.Contains(recordDescriptionToDelete)).FirstOrDefault();

            IWebElement deleteRecordButton = recordToDelete.FindElement(By.CssSelector("div[class='btn-group'] button[id^='manageRecord']"));
            deleteRecordButton.Click();
            wait.Until(driver => driver.FindElements(By.CssSelector("ul[class='dropdown-menu'][style='display: block;']")).Any(element => element.Displayed));

            mouseActions = new Actions(webDriver);
            IWebElement deleteRecordLink = webDriver.FindElement(By.CssSelector("ul[class='dropdown-menu'][style='display: block;'] a[id^='cogDelete']"));
            mouseActions.MoveToElement(deleteRecordLink).Perform();

            deleteRecordLink.Click();

            IWebElement confirmDialogButton = webDriver.FindElement(By.CssSelector("div[class^='ui-dialog-buttonpane'] button:nth-child(1)"));
            confirmDialogButton.Click();

            listOfRecordsDisplayed = webDriver.FindElements(By.CssSelector("div[class^='list_layout']"));
            bool isRecordDeletedFromView = listOfRecordsDisplayed.Where(element => element.Text.Contains(recordDescriptionToDelete)).Count() == 0;
            Assert.IsTrue(isRecordDeletedFromView, "ERROR: Record with description of " + recordDescriptionToDelete + " has NOT been deleted from the page");
        }

        private void AddAReleaseToWaterRecordAndSave(string descriptionToEnter = "")
        {
            IWebElement newRecordButton = webDriver.FindElement(By.CssSelector("section[role='main'] a[href='/automation/Environment/ReleaseToWater/Create']"));
            newRecordButton.Click();
            wait.Until(driver => driver.Url == "https://stirling.she-development.net/automation/Environment/ReleaseToWater/Create#/information");
            wait.Until(driver => !(driver.FindElement(By.Id("spinner")).Displayed));

            IWebElement description = webDriver.FindElement(By.CssSelector("#SheReleaseToWater_Description"));
            description.SendKeys(descriptionToEnter);
            wait.Until(driver => description.GetAttribute("value") == descriptionToEnter);


            IWebElement sampleDateCalendarButton = webDriver.FindElement(By.CssSelector("#tabstripContent fieldset div li:nth-child(7) span button span"));
            string scriptCommandToExecute = "arguments[0].scrollIntoView(true);";
            (webDriver as IJavaScriptExecutor).ExecuteScript(scriptCommandToExecute, sampleDateCalendarButton);
            sampleDateCalendarButton.Click();
            wait.Until(driver => driver.FindElement(By.CssSelector("div[class^='ui-datepicker-buttonpane'] button[data-handler='today']")).Displayed);

            IWebElement sampleDateCalendarButtonToday = webDriver.FindElement(By.CssSelector("div[class^='ui-datepicker-buttonpane'] button[data-handler='today']"));
            sampleDateCalendarButtonToday.Click();
            wait.Until(driver => !(driver.FindElement(By.CssSelector("table[class='ui-datepicker-calendar']")).Displayed));

            IWebElement saveAndCloseButton = webDriver
                                            .FindElement(By.CssSelector("div.buttons_line_container li:nth-child(3) button[class^='btn btn-large'][name='submitButton']"));
            saveAndCloseButton.Click();
            wait.Until(driver => driver.Url == "https://stirling.she-development.net/automation/Environment/ReleaseToWater");
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                DeleteRecord(TestDescriptionTwo);    //Remove remaining record added
                webDriver.Close();
                webDriver.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            KillAnyRemainingBrowserProcesses(chromeDriverName);

        }

        private void KillAnyRemainingBrowserProcesses(string processName)
        {
            try
            {
                List<Process> browserProcessList = Process.GetProcessesByName(processName).ToList();
                foreach (Process processToKill in browserProcessList)
                    processToKill.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}