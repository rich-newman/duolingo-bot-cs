using DuolingoBotCS.ChallengeBaseClasses;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class TapCompleteChallenge : ChoiceChallenge
    {
        protected override ReadOnlyCollection<IWebElement> GetChoices() => 
            driver.FindElements(By.XPath("//div[@data-test=\"word-bank\"]//span[@data-test=\"challenge-tap-token-text\"]"));

        protected override void MakeGuess(ReadOnlyCollection<IWebElement> choices) => ClickAllChoices(choices);

        protected override void EnterSolution(string solution, ReadOnlyCollection<IWebElement> choices, string originalQuestion)
        {
            IEnumerable<string> choiceTexts = choices.Select(choice => choice.Text);
            TapCompleteSolver tapCompleteSolver = new(solution, originalQuestion, choiceTexts);
            List<string> choiceTextsToClick = tapCompleteSolver.GetChoiceTextsToClick();
            ClickChoiceTextsToClick(choiceTextsToClick, choices);
        }
    }
}
