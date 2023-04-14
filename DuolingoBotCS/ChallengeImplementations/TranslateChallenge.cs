using DuolingoBotCS.ChallengeBaseClasses;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class TranslateChallenge: ChoiceChallenge
    {
        protected override ReadOnlyCollection<IWebElement> GetChoices() =>
            driver.FindElements(By.XPath("//span[@data-test=\"challenge-tap-token-text\"]"));
        
        protected override void EnterSolution(string solution, ReadOnlyCollection<IWebElement> choices, string _)
        {
            if (choices.Count > 0)
                EnterClickSolution(solution, choices);
            else
                SendTextToInputArea(solution);
        }

        protected override void MakeGuess(ReadOnlyCollection<IWebElement> choices)
        {
            if (choices.Count > 0)
                choices[0].Click();
            else
                SendTextToInputArea("a");
        }

        private static void SendTextToInputArea(string text)
        {
            IWebElement inputElement = driver.FindElement(
                By.XPath("//textarea[@data-test=\"challenge-translate-input\"]"));
            inputElement.SendKeys(text);
        }

        private static void EnterClickSolution(string solution, ReadOnlyCollection<IWebElement> choices)
        {
            IEnumerable<string> choiceTexts = choices.Select(choice => choice.Text);
            TranslateSolver translateSolver = new(solution, choiceTexts);
            List<string> choiceTextsToClick = translateSolver.GetChoiceTextsToClick();
            ClickChoiceTextsToClick(choiceTextsToClick, choices);
        }
    }
}