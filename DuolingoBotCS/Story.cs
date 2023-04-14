using DuolingoBotCS.ChallengeImplementations;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS
{
    // Container for a StoryChallenge is a Story, container for regular Challenge is a set of Challenges. Both are accessed by clicking
    // a button on the main tree.  So the Story class is equivalent to the Challenges class but for stories, if you see what I mean.
    // Also the individual 'challenges' for a story are much simpler, and are modelled below as methods rather than separate classes,
    // apart from the MatchChallenge where we cheat somewhat by just making the logic static in the MatchChallenge class and calling it.
    internal class Story
    {
        private static readonly IWebDriver driver = Program.driver;

        // There are neater ways of doing this with translate, but this works and it's blindingly obvious what it's doing
        private static readonly string digitSpanXPath = "span[text()='1' or text()='2' or text()='3' or text()='4' or text()='5' " +
            "or text()='6' or text()='7' or text()='8' or text()='9' or text()='0']";
        private readonly string choicesNoDigitsXPath = 
            $"//button[span[@data-test='challenge-tap-token-text'] and not({digitSpanXPath})]";
        private readonly string choicesWithDigitsXPath = $"//button[span[@data-test='challenge-tap-token-text'] and {digitSpanXPath}]";

        internal virtual void Run()
        {
            Program.Log("Running story");
            bool storyCompleted = false;
            while (!storyCompleted)
            {
                try
                {
                    TryClickStoryStartButton();
                    TryClickContinueButton();
                    TryStoriesChoiceChallenge();
                    TryChoiceChallenge();
                    TryMatchChallenge();
                    TryWriteSummaryChallenge();  // We just skip out
                    if (Program.IsOnHomePage()) storyCompleted = true;
                    Thread.Sleep(15);
                }
                catch (Exception e)
                {
                    Program.Log(e);
                }
            }
        }

        private static void TryClickStoryStartButton()
        {
            ReadOnlyCollection<IWebElement> buttons = driver.FindElements(By.XPath("//button[@data-test='story-start']"));
            if (buttons.Count > 0) buttons[0].Click();
        }

        private static bool TryClickContinueButton()
        {
            string enabledNextButtonXPath = "//button[" +
                "(@data-test='stories-player-continue' or @data-test='stories-player-done') " +
                "and not(@disabled) " +
                $"and span[{Challenge.TextMatches(BaseLanguage.Continue)}]]";
            ReadOnlyCollection<IWebElement> continueButtons = driver.FindElements(By.XPath(enabledNextButtonXPath));
            if (continueButtons.Count > 0) continueButtons[0].Click();
            return continueButtons.Count > 0;
        }

        private static void TryWriteSummaryChallenge()
        {
            string enabledNextButtonXPath = "//button[" +
                $"span[{Challenge.TextMatches(BaseLanguage.SkipExercise)}] " +
                "and not(@disabled)]";
            ReadOnlyCollection<IWebElement> skipExerciseButtons = driver.FindElements(By.XPath(enabledNextButtonXPath));
            if (skipExerciseButtons.Count > 0) skipExerciseButtons[0].Click();
        }

        private static void TryStoriesChoiceChallenge()
        {
            ReadOnlyCollection<IWebElement> storiesChoiceButtons = driver.FindElements(
                By.XPath("//button[@data-test=\"stories-choice\"]"));
            if (storiesChoiceButtons.Count > 0)
            {
                foreach (var storiesChoiceButton in storiesChoiceButtons)
                {
                    storiesChoiceButton.Click();
                    Thread.Sleep(500);
                    if (TryClickContinueButton()) break;
                }
            }
        }

        private void TryChoiceChallenge()
        {
            ReadOnlyCollection<IWebElement> choiceTextButtons = driver.FindElements(By.XPath(choicesNoDigitsXPath));
            if (choiceTextButtons.Count > 0)
            {
                var count = choiceTextButtons.Count;
                for (int i = 0; i < count; i++)
                {
                    foreach (var choiceTextButton in choiceTextButtons)
                    {
                            choiceTextButton.Click();
                            Thread.Sleep(500);
                            if (TryClickContinueButton()) return;
                    }
                }
            }
        }

        private void TryMatchChallenge()
        {
            ReadOnlyCollection<IWebElement> choices = driver.FindElements(By.XPath(choicesWithDigitsXPath));
            MatchChallenge.RunMatch(choices);
            TryClickContinueButton();
        }
    }
}
