using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DuolingoBotCS
{
    // This program is, of course, single-threaded
    // Efficiency isn't all that important either: we're waiting for Duolingo a lot
    internal class Program
    {
        private static string chromeDriverPath = "C:\\Program Files(x86)\\chromedriver_win32\\";
        private static readonly bool autoLogin = true;
        private static readonly bool incognito = false;
        private static readonly bool muteAudio = true;
        private static readonly bool maximizeWindow = true;
        private static readonly bool headless = false;
        private static bool practice = false;

        private static string username;
        private static string password;

        public static string Now() => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ff");
        public static void Log(string message) => Console.WriteLine(Now() + " " + message);
        public static void Log(Exception e) => Console.WriteLine(Now() + " " + e);  // Highly sophisticated logging

        internal static IWebDriver driver;

        static void Main()
        {
            Log("Started");
            try
            {
                ReadSettings();
                ChromeOptions options = GetChromeOptions();
                ChromeDriverService service = ChromeDriverService.CreateDefaultService(chromeDriverPath);
                driver = new ChromeDriver(service, options);
                driver.Navigate().GoToUrl("https://duolingo.com");
                IWebElement haveAccount = new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                    .Until(e => e.FindElement(By.XPath("//button [@data-test='have-account']")));
                haveAccount.Click();
                Login();
                if (practice)
                    Practice();
                else
                    DoTree();
                Log("Completed");
            }
            catch (Exception e)
            {
                Log("A fatal error occurred and the program is terminating");
                Log($"Error: {e}");
            }
            driver.Quit();
        }

        private static ChromeOptions GetChromeOptions()
        {
            ChromeOptions options = new();
            options.AddArgument("--log-level=3");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddExcludedArgument("enable-automation");
            if (incognito) options.AddArgument("--incognito");
            if (muteAudio) options.AddArgument("--mute-audio");
            if (maximizeWindow) options.AddArgument("start-maximized");
            if (headless) options.AddArgument("--headless");
            return options;
        }

        private static void Login()
        {
            if (autoLogin && username != null && password != null)
            {
                IWebElement emailInput = new WebDriverWait(driver, TimeSpan.FromSeconds(25))
                    .Until(e => e.FindElement(By.XPath("//input[@data-test=\"email-input\"]")));
                emailInput.SendKeys(username);
                IWebElement passwordInput = driver.FindElement(By.XPath("//input[@data-test=\"password-input\"]"));
                passwordInput.SendKeys(password);
                Thread.Sleep(1000);
                IWebElement loginButton = driver.FindElement(By.XPath("//button[@data-test=\"register-button\"]"));
                loginButton.Click();
            }
            // You get 120 seconds to login if you're logging in manually
            new WebDriverWait(driver, TimeSpan.FromSeconds(120)).Until(e => e.Url == "https://www.duolingo.com/learn");
        }

        private static void ReadSettings()
        {
            // Highly sophisticated settings file
            string[] lines = File.ReadAllLines(@"Settings.txt");
            foreach (string line in lines)
            {
                if (line.StartsWith(@"//") || !line.Contains('=')) continue;
                string[] keyValuePair = line.Split('=');
                if (keyValuePair[0] == "username") username = keyValuePair[1];
                else if (keyValuePair[0] == "password") password = keyValuePair[1];
                else if (keyValuePair[0] == "practice") practice = keyValuePair[1].ToLower() == "true";
                else if (keyValuePair[0] == "chromedriver") chromeDriverPath = keyValuePair[1];
                else if (keyValuePair[0] == "baselanguage") BaseLanguage.Set(keyValuePair[1]);
            }
        }

        private static void Practice()
        {
            bool completed = false;
            while (!completed)
            {
                Log("Trying to start new practice");
                IWebElement practiceButton = new WebDriverWait(driver, TimeSpan.FromSeconds(20))
                        .Until(e => e.FindElement(By.XPath("//a[@data-test=\"global-practice\"]")));
                practiceButton.Click();
                // May throw if the window isn't maximized and the Skip button is thus not showing
                IWebElement skip = new WebDriverWait(driver, TimeSpan.FromSeconds(45))
                    .Until(e => e.FindElement(By.XPath("//button[@data-test=\"player-skip\"]")));
                 new Challenges().Run();
                //  We never exit, we just practice until the world ends
            }
        }

        private static void DoTree()
        {
            bool completed = false;
            while (!completed)
            {
                try
                {
                    Debugger.Break();
                    Log("Trying to start new lesson");
                    WaitForHomePage(secondsToWait: 20);
                    ClickAnyOverlayButton();
                    ClickAnyGoToCurrentUnitButton();
                    RunNextItemInTree();
                    ClickAnyChest();
                }
                catch (Exception e) { Log(e); }
            }
        }

        private static readonly string homePageXpath = "//div[@data-test='home']";
        internal static void WaitForHomePage(int secondsToWait)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToWait)).Until(e => e.FindElement(By.XPath(homePageXpath)));
            }
            catch (WebDriverTimeoutException) { Log("Time out in main loop waiting for home page"); }
        }

        internal static void ClickAnyOverlayButton()
        {
            // Duo have overlaid some message buttons on a valid displayed (and not disabled) player-next button
            // So a naive test for a next button may click the wrong thing and error
            // XPath below is an attempt to identify this and click the button, and exclude it from the next button test
            // When there are multiple buttons on the overlay screen the next/no thanks/continue is usually the last one, now [^1]
            string overlayButtonsXPath = "//div[@id='overlays' and @onclick]//button[span]";
            ReadOnlyCollection<IWebElement> overlayButtons = driver.FindElements(By.XPath(overlayButtonsXPath));
            if (overlayButtons.Count > 0) overlayButtons[^1].Click();
        }

        private static void ClickAnyGoToCurrentUnitButton()
        {
            ReadOnlyCollection<IWebElement> goToCurrentElements = driver.FindElements(By.XPath(
                "//button[@aria-label=\"Go to current unit\"]"));
            if (goToCurrentElements.Count > 0) goToCurrentElements[0].Click();
        }

        private static void RunNextItemInTree()
        {
            string buttonXPath = $@"//div[@role=""button""]//button[@aria-label]
                                            [following-sibling::*[.//div[text()='{BaseLanguage.Start}']]]";
            ReadOnlyCollection<IWebElement> startElements = driver.FindElements(By.XPath(buttonXPath));
            if (startElements.Count > 0)
            {
                string testType = startElements[0].GetAttribute("aria-label");
                ReadOnlyCollection<IWebElement> childImages = driver.FindElements(By.XPath(buttonXPath + "/img"));
                if (childImages.Count > 0) childImages[0].Click();
                Thread.Sleep(2000);
                ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.XPath(
                    "//a[starts-with (@data-test, 'skill-path-state-active')]"));
                if (elements.Count > 0)
                {
                    elements[0].Click();
                    if (testType.ToUpper() == BaseLanguage.Story)
                        new Story().Run();
                    else
                        new Challenges().Run();
                }
            }
        }

        private static void ClickAnyChest()
        {
            string chestXPath = $"//button[{AttributeMatches("@aria-label", BaseLanguage.OpenChest)}]//img";
            ReadOnlyCollection<IWebElement> chestElements = driver.FindElements(By.XPath(chestXPath));
            if (chestElements.Count > 0)
            {
                chestElements[0].Click();
                try
                {
                    new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until
                        (e => e.FindElements(By.XPath(chestXPath)).Count == 0);
                    Thread.Sleep(2000); // Still getting crashes because screen not ready
                }
                catch (WebDriverTimeoutException) { Log("Chest dismissal timed out"); }
            }
        }

        private static string AttributeMatches(string attributeName, string input)
        {
            string lowerCaseInput = input.ToLower();
            return $"translate({attributeName}, '{input}', '{lowerCaseInput}')='{lowerCaseInput}'";
        }

        internal static bool IsOnHomePage()
        {
            ReadOnlyCollection<IWebElement> homePageElements = driver.FindElements(By.XPath(homePageXpath));
            return homePageElements.Count > 0;
        }

        [Conditional("DEBUG")]
        internal static void DebugDumpButtonState(string message = "DebugDumpButtonState:")
        {
            ReadOnlyCollection<IWebElement> challengeElements = driver.FindElements(By.XPath(
                    "//div [starts-with (@data-test, 'challenge challenge-')]"));
            ReadOnlyCollection<IWebElement> nextButtons = driver.FindElements(By.XPath("//button[@data-test=\"player-next\"]"));
            ReadOnlyCollection<IWebElement> skipButtons = driver.FindElements(By.XPath("//button[@data-test=\"player-nextButton\"]"));
            if (nextButtons.Count > 0)
                message += $". Next button exists, text={nextButtons[0].Text}.";
            else
                message += ". Next button does not exist.";
            if (skipButtons.Count > 0)
                message += $" Skip button exists, text={skipButtons[0].Text}.";
            else
                message += " Skip button does not exist.";
            string challengeMessage = (challengeElements.Count > 0) ? "exists" : "doesn't exist";
            message += $" Challenge {challengeMessage}.";
            Log(message);
        }
    }
}