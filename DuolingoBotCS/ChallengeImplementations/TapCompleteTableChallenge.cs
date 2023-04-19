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

        // Cheat, but this should work if there are only two choices
        // We tried it forwards already and failed, try in reverse
        private static void ClickAllChoicesInReverse(IEnumerable<IWebElement> choices)
        {
            foreach (IWebElement choice in choices.Reverse())
                choice.Click();
        }

        // For the cheat, force the test to guess the first time through, whether we can skip or not
        protected override bool SkipButtonExists() => false;
        // We don't use the solution, so no point in getting it, we just need a dictionary entry to
        // let us know we guessed once
        protected override string GetSolutionText() => string.Empty;
    }
}
