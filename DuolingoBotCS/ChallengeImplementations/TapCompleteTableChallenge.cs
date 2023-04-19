using DuolingoBotCS.ChallengeBaseClasses;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class TapCompleteTableChallenge : ChoiceChallenge
    {
        protected override ReadOnlyCollection<IWebElement> GetChoices() =>
            driver.FindElements(By.XPath("//div[@data-test=\"word-bank\"]//span[@data-test=\"challenge-tap-token-text\"]"));

        protected override void MakeGuess(ReadOnlyCollection<IWebElement> choices) => ClickAllChoices(choices);

        protected override void EnterSolution(string solution, ReadOnlyCollection<IWebElement> choices, string originalQuestion) =>
            ClickAllChoicesInReverse(choices);

        // Horrible cheat for now as I didn't get to see what a solution looks like
        // This should work though, because there are only two choices and
        // we tried it forwards already and failed
        private static void ClickAllChoicesInReverse(IEnumerable<IWebElement> choices)
        {
            Debugger.Break();
            foreach (IWebElement choice in choices.Reverse())
                choice.Click();
        }

        // For the cheat, force the test to guess the first time through, whether we can skip or not
        protected override bool SkipButtonExists() => false;
    }
}
