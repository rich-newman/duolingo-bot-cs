using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace DuolingoBotCS
{
    /// <summary>
    /// Base class for all Challenge classes
    /// </summary>
    internal abstract class Challenge
    {
        protected static IWebDriver driver = Program.driver;
        protected static string Now() => Program.Now();
        // To make handling all the various cases easier Run should complete the current challenge and deposit
        // us on the next screen in all cases
        internal abstract void Run();

        protected static bool SkipButtonExists()
        {
            string enabledSkipButtonXPath = "//button[@data-test='player-skip']";
            ReadOnlyCollection<IWebElement> skipButtons = driver.FindElements(By.XPath(enabledSkipButtonXPath));
            return skipButtons.Count > 0;  // Set to false to test all challenges for absence of skips
        }

        protected static void ClickSkipButton()
        {
            // Duo doesn't enable/disable player-skip, it just removes it when you can't click it
            // It also doesn't mess around changing the text in different circumstances
            string enabledSkipButtonXPath = "//button[@data-test='player-skip']";
                IWebElement skipButton = new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                        .Until(e => e.FindElement(By.XPath(enabledSkipButtonXPath)));
                skipButton.Click();
        }

        // For use when we're not sure if we have a continue button, e.g. at a point where Duo may throw up a message, but may not
        protected static void TryClickNextButton()
        {
            string enabledNextButtonXPath = "//button[@data-test=\"player-next\" " +
                "and (not(@aria-disabled) or @aria-disabled=\"false\")]";
            ReadOnlyCollection<IWebElement> nextButtons = driver.FindElements(By.XPath(enabledNextButtonXPath));
            if (nextButtons.Count > 0) nextButtons[0].Click();
        }

        protected static void ClickCheckButton()
        {
            string enabledNextButtonXPath = "//button[@data-test='player-next' " +
                "and @aria-disabled='false' " +
                $"and span[{TextMatches(BaseLanguage.Check)}]]";
            IWebElement nextButton = new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                    .Until(e => e.FindElement(By.XPath(enabledNextButtonXPath)));
            nextButton.Click();
        }

        protected static bool CheckButtonEnabled()
        {
            string enabledNextButtonXPath = "//button[@data-test='player-next' " +
                "and @aria-disabled='false' " +
                $"and span[{TextMatches(BaseLanguage.Check)}]]";
            ReadOnlyCollection<IWebElement> enabledNextButtons = driver.FindElements(By.XPath(enabledNextButtonXPath));
            return enabledNextButtons.Count > 0;
        }

        // For use when we know there should be a continue button and we definitely want to click it
        protected static void ClickContinueButton()
        {
            string enabledNextButtonXPath = "//button[@data-test='player-next' " +
                "and @aria-disabled='false' " +
                $"and span[{TextMatches(BaseLanguage.Continue)}]]";
            try
            {
                IWebElement nextButton = new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                        .Until(e => e.FindElement(By.XPath(enabledNextButtonXPath)));
                nextButton.Click();
            }
            catch (Exception)
            {
                Program.Log("Time out clicking continue button");
            }
        }

        protected static void ClickContinueOrShowTipButton()
        {
            // When a challenge is complete we get either a Continue button or a Show Tip button
            string enabledNextButtonXPath = "//button[@data-test='player-next' " +
                "and @aria-disabled='false' " +
                $"and (span[{TextMatches(BaseLanguage.Continue)}] " +
                $"or span[{TextMatches(BaseLanguage.ShowTip)}])]";
            try
            {
                IWebElement nextButton = new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                        .Until(e => e.FindElement(By.XPath(enabledNextButtonXPath)));
                string text = nextButton.Text;
                nextButton.Click();
                if(string.Equals(text, BaseLanguage.ShowTip, StringComparison.OrdinalIgnoreCase)) 
                    DismissShowTipScreen();
            }
            catch (Exception e)
            {
                Program.Log("Time out clicking continue button: " + e);
            }
        }

        private static void DismissShowTipScreen()
        {
            Thread.Sleep(2000);
            ReadOnlyCollection<IWebElement> choices = driver.FindElements(
                By.XPath("//div[@data-test='challenge-judge-text']"));
            if (choices.Count > 0)
            {
                Thread.Sleep(500);
                choices[0].Click();
                ClickCheckButton();
                ClickContinueButton();
            }
        }

        protected static string GetQuestionFromHintTokens()
        {
            ReadOnlyCollection<IWebElement> words = driver.FindElements(By.XPath("//div[@data-test='hint-token'] | //span[@data-test='hint-token']"));
            if (words.Count == 0) return null;
            string question = "";
            foreach (IWebElement word in words)
            {
                string text = word.Text;
                question += (text == "" ? " " : text);
            }
            return question;
        }

        protected virtual void ClickAllChoices(IEnumerable<IWebElement> choices)
        {
            foreach (IWebElement choice in choices)
                choice.Click();
        }

        protected static void ClickChoice(string textToClick, ReadOnlyCollection<IWebElement> choices)
        {
            foreach (IWebElement choice in choices)
            {
                if (choice.Text == textToClick)  // Does this ever need to ignore case? choice.Text.Equals(textToClick, StringComparison.OrdinalIgnoreCase)
                {
                    choice.Click();
                    break;
                }
            }
        }

        // It's possible to have two questions the same for the same challenge in the same set of lessons.  These can provide choices for
        // answers and the choices can be different for the two questions.  For example, one question can ask you to choose 'otro' for
        // 'another', and another question in the same lesson can ask you to choose 'otra'.  Because we skip, record the answer and then
        // give it when we're asked the question again this is problematic.
        // Even if we record both answers how do we know which question it is?  If we try our first answer and it's wrong we've now got
        // to wait to be asked the question again and know we need to try the second answer.  Meanwhile the other 'same' question may be
        // asked before we get asked this one again.  It's tricky.
        // Easier, and more efficient, is to record ALL the possible answers as part of the QUESTION key, which is what we do here.
        // Now we know which question and answer set we've cached a given answer for when we see it again.  Note the answers need to be
        // sorted as they are reordered at random when we see the question again.
        protected static string MakeQuestionUnique(string question, ReadOnlyCollection<IWebElement> choices)
        {
            if (choices.Count == 0) return question;
            IOrderedEnumerable<IWebElement> sortedChoices = choices.OrderBy(t => t.Text);
            return question.Trim() + "|" + string.Join("|", sortedChoices.Select(t => t.Text));
        }

        protected static void EnterTextIntoChallengeTextInput(string text)
        {
            IWebElement inputElement = driver.FindElement(
                By.XPath("//input[@data-test=\"challenge-text-input\"]"));
            inputElement.SendKeys(text);
        }

        internal static string RemovePunctuation(string text) =>
            text.Replace(".", "").Replace(",", "").Replace("!", "").Replace("«", "").Replace("»", "").Replace(":", "").Replace("\"", "")
                .Replace("?", "").Replace("¡", "").Replace("¿", "").Replace(";", "").Replace("…", "").Trim();


        internal static string TextMatches(string input)
        {
            string lowerCaseInput = input.ToLower();
            return $"translate(text(), '{input}', '{lowerCaseInput}')='{lowerCaseInput}'";
        }

        internal static string TextContains(string input)
        {
            string lowerCaseInput = input.ToLower();
            string lowerCaseText = $"translate(text(), '{input}', '{lowerCaseInput}')";
            return $"contains({lowerCaseText}, \"{lowerCaseInput}\")";
        }

        protected static ReadOnlyCollection<IWebElement> GetSolutionOnScreen(string solutionText = null)
        {
            solutionText ??= BaseLanguage.CorrectSolution;
            string correctSolutionXPath = $"//*[{TextContains(solutionText)}][1]";
            string actualSolutionXPath = correctSolutionXPath + "/following-sibling::*[1]";
            return driver.FindElements(By.XPath(actualSolutionXPath));
        }
    }
}
