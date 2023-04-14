using DuolingoBotCS.ChallengeBaseClasses;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class NameChallenge: ChoiceChallenge
    {
        protected override string GetQuestion() => GetQuestionFromChallengeHeader();

        protected override void EnterSolution(string solution, ReadOnlyCollection<IWebElement> choices, string _)
        {
            if (choices.Count > 0)
            {
                // We have articles (e.g. le la l') on tap buttons, and a text     area to fill 
                string[] answer = solution.Split(new char[] { ' ', '\'' });
                if (solution.Contains('\'')) answer[0] += '\'';
                ClickChoice(answer[0], choices);
                EnterTextIntoChallengeTextInput(answer[1]);
            }
            else
            {
                EnterTextIntoChallengeTextInput(solution);
            }
        }

        protected override void MakeGuess(ReadOnlyCollection<IWebElement> choices)
        {
            if (choices.Count > 0) choices[0].Click();
            EnterTextIntoChallengeTextInput("a");
        }

        protected override string GetSolutionText()
        {
            ReadOnlyCollection<IWebElement> solution = GetSolutionOnScreen(BaseLanguage.CorrectSolution);
            if (solution.Count > 0) return solution[0].Text;
            // Try again with an 's' at the end of 'solution': for NameChallenges Duo can provide two solutions
            // separated by a comma
            ReadOnlyCollection<IWebElement> solutions = GetSolutionOnScreen(BaseLanguage.CorrectSolutions);
            if (solutions.Count > 0) return solutions[0].Text.Split(',')[0];
            throw new Exception("Unable to get solution for Name Challenge");
        }
    }
}
