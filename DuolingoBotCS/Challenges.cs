using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DuolingoBotCS
{
    /// <summary>
    /// Caches and runs challenges
    /// Exists for the length of a set of challenges before we return to the tree screen
    /// </summary>
    internal class Challenges
    {
        // Cache for challenge objects: cached answers from previous runs are stored and can be reused
        private readonly Dictionary<string, Challenge> challenges = new();
        protected static IWebDriver driver = Program.driver;

        internal void Run()
        {
            bool challengesCompleted = false;
            while (!challengesCompleted)
            {
                Program.ClickAnyOverlayButton();
                RunChallenge();

                //Program.DebugDumpButtonState("Debug Dump: In Run before wait");
                WaitForNewScreenThenContinue();
                //Program.DebugDumpButtonState("Debug Dump: In Run after wait");

                ClickAnyNextButton();
                ClickAnyNoThanksButton();

                if (Program.IsOnHomePage()) challengesCompleted = true;
            }
        }

        private void RunChallenge(ReadOnlyCollection<IWebElement> elements)
        {
            IWebElement element = elements[0];
            string attributeValue = element.GetAttribute("data-test").Split()[1];
            string className = ConvertToClassName(attributeValue);
            Program.Log($"Running {className}");
            #region Test code to be removed
            if (className != "AssistChallenge"
                //&& className != "CompleteReverseTranslationChallenge"
                && className != "DefinitionChallenge"
                && className != "DialogueChallenge"
                && className != "FormChallenge"
                && className != "GapFillChallenge"
                && className != "MatchChallenge"
                && className != "NameChallenge"
                && className != "PartialReverseTranslateChallenge"
                && className != "ReadComprehensionChallenge"
                && className != "SelectChallenge"
                && className != "TapClozeChallenge"
                && className != "TapCompleteChallenge"
                //&& className != "TapCompleteTableChallenge"
                && className != "TranslateChallenge"
                && className != "TypeClozeChallenge"
                && className != "TypeCompleteTableChallenge"
                && className != "SpeakChallenge"
                && className != "ListenChallenge"
                && className != "ListenCompleteChallenge"
                && className != "ListenMatchChallenge"
                && className != "ListenIsolationChallenge"
                && className != "ListenComprehensionChallenge"
                && className != "ListenTapChallenge"
                && className != "SelectTranscriptionChallenge"
                )
            {
                System.Diagnostics.Debugger.Break();  // Identifying new classes (or ones we want to edit) before we do anything with them
            }
            #endregion
            if (challenges.ContainsKey(className))
            {
                challenges[className].Run();
            }
            else
            {
                Type type = Type.GetType("DuolingoBotCS.ChallengeImplementations." + className);
                if (type != null)
                {
                    Challenge challenge = Activator.CreateInstance(type) as Challenge;
                    challenges[className] = challenge;
                    challenge.Run();
                }
            }
        }

        private static void ClickAnyNextButton()
        {
            try
            {
                string nextButtonXPath = "//button[@data-test=\"player-next\" " +
                    "and @aria-disabled=\"false\" " +
                    "and not(following::div[@id='overlayButtons' and @onclick]//button/span)]";
                ReadOnlyCollection<IWebElement> nextButtons = driver.FindElements(By.XPath(nextButtonXPath));
                if (nextButtons.Count > 0) nextButtons[0].Click();
            }
            catch (Exception e) { Program.Log($"Exception: {e}"); }
        }

        private static void ClickAnyNoThanksButton()
        {
            ReadOnlyCollection<IWebElement> noThanksButtons = driver.FindElements(By.XPath(
                    "//button[@data-test=\"practice-hub-ad-no-thanks-button\" or @data-test=\"plus-no-thanks\"]"));
            if (noThanksButtons.Count > 0) noThanksButtons[0].Click();
        }

        private void RunChallenge()
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.XPath(
                "//div [starts-with (@data-test, 'challenge challenge-')]"));
            if (elements.Count > 0) RunChallenge(elements);
        }

        private static void WaitForNewScreenThenContinue()
        {
            // We have seen occasions where we've ended up stuck on a screen that meets one of the scenarios that indicates we're not
            // on a new screen below.  And in those cases just continuing and hitting the next button did let the system recover.
            // So we try that here.
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                    .Until(e => !WaitForNewScreenIfNoTextOnNextButton());
                new WebDriverWait(driver, TimeSpan.FromSeconds(2))
                    .Until(e => !WaitForNewScreen());
            }
            catch (WebDriverTimeoutException)
            {
#if DEBUG
                // At present we expect these exceptions where we have a test that has no skip button
                Program.Log("TIMEOUT EXCEPTION THROWN waiting for new screen");
#endif
            }
        }

        // If there's no text on the next button we are at the end of a series of challenges, when Duo is superslow
        // So we wait for text on the button (which normally there is) with a longer timeout than with WaitForNewScreen below
        private static bool WaitForNewScreenIfNoTextOnNextButton()
        {
            ReadOnlyCollection<IWebElement> nextButtons = driver.FindElements(By.XPath("//button[@data-test=\"player-next\"]"));
            bool nextButtonExists = nextButtons.Count > 0;
            string nextButtonText = nextButtonExists ? nextButtons[0].Text : null;
            if (nextButtonExists && nextButtonText == "") return true;
            return false;
        }

        private static bool WaitForNewScreen()
        {
            ReadOnlyCollection<IWebElement> challengeElements = driver.FindElements(By.XPath(
                    "//div [starts-with (@data-test, 'challenge challenge-')]"));
            bool challengeExists = challengeElements.Count > 0;
            ReadOnlyCollection<IWebElement> nextButtons = driver.FindElements(By.XPath("//button[@data-test=\"player-next\"]"));
            bool nextButtonExists = nextButtons.Count > 0;
            string nextButtonText = nextButtonExists ? nextButtons[0].Text : null;
            ReadOnlyCollection<IWebElement> skipButtons = driver.FindElements(By.XPath("//button[@data-test=\"player-skip\"]"));
            bool skipButtonExists = skipButtons.Count > 0;
            ReadOnlyCollection<IWebElement> showTipButtons = driver.FindElements(By.XPath("//button[span[text()='Show tip']]"));
            bool showTipButtonExists = showTipButtons.Count > 0;

            // It's NOT a new screen if the next button says CONTINUE and there is a skip button
            // It's also not a new screen if the next button has no text
            // It's also not a new screen if we have a next button saying CHECK but there's no skip button or show tip button,
            // this is the case that's blowing up two consecutive TranslateChallenges because the screen isn't ready.  Unfortunately
            // this can validly happen in sets of challenges where Duo have decided to not let you skip, but it's better to time out
            // in those cases.
            // Finally if we're on a challenge screen with a Continue button it's not a valid new screen until the skip button appears
            // We get no skip or next buttons at startup for a while, but that's also true on the tree screen,
            // so we need a more sophisticated test if we're going to detect the startup case and stop looping
            if (nextButtonExists && nextButtonText == BaseLanguage.Continue && skipButtonExists) return true;
            if (nextButtonExists && nextButtonText == BaseLanguage.Continue && showTipButtonExists) return true;
            if (nextButtonExists && nextButtonText == BaseLanguage.Check && !skipButtonExists && !showTipButtonExists) return true; 
            if (challengeExists && nextButtonExists && nextButtonText == BaseLanguage.Continue && !skipButtonExists && !showTipButtonExists) 
                return true;
            return false;
        }

        // Want to convert e.g. challenge-listenTap to ListenTapChallenge or challenge-translate to TranslateChallenge
        private static string ConvertToClassName(string input)
        {
            string[] ary = input.Split('-');
            return ary.Length == 2 ? CapitalizeFirstLetter(ary[1]) + CapitalizeFirstLetter(ary[0]) : input;
        }

        private static string CapitalizeFirstLetter(string s) => s[0].ToString().ToUpper() + s[1..];
    }
}
