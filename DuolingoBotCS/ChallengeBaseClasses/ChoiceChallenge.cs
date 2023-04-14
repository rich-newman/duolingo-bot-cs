using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeBaseClasses
{
    /// <summary>
    /// Base class for a series of challenges that all have very similar logic: find the question, find choices for solutions, make a 
    /// guess of some kind or click skip if it exists, record the solution, then next time we see the question enter the recorded solution
    /// Implemented as a Template Method
    /// </summary>
    internal abstract class ChoiceChallenge : Challenge
    {
        protected readonly Dictionary<string, string> dictionary = new();
        internal override void Run()
        {
            try
            {
                string originalQuestion = GetQuestion();
                ReadOnlyCollection<IWebElement> choices = GetChoices();
                string question = MakeQuestionUnique(originalQuestion, choices);
                if (dictionary.ContainsKey(question))
                {
                    EnterSolution(solution: dictionary[question], choices, originalQuestion);
                    ClickCheckButton();
                }
                else
                {
                    if (SkipButtonExists())
                    {
                        ClickSkipButton();
                    }
                    else
                    {
                        MakeGuess(choices);
                        ClickCheckButton();
                    }
                    dictionary[question] = GetSolutionText();
                }
                ClickContinueOrShowTipButton();
            }
            catch (Exception e)
            {
                Program.Log(e);
            }
        }

        protected virtual string GetQuestion() => GetQuestionFromHintTokens();

        protected virtual ReadOnlyCollection<IWebElement> GetChoices() =>
            driver.FindElements(By.XPath("//div[@data-test=\"challenge-judge-text\"]"));

        protected virtual void EnterSolution(string solution, ReadOnlyCollection<IWebElement> choices, string _) => 
            ClickChoice(solution, choices);

        protected virtual void MakeGuess(ReadOnlyCollection<IWebElement> choices) => choices[0].Click();

        protected virtual string GetSolutionText()
        {
            ReadOnlyCollection<IWebElement> solutions = GetSolutionOnScreen();
            return solutions.Count > 0 ? solutions[0].Text : null;
        }

        protected static string GetQuestionFromChallengeHeader()
        {
            ReadOnlyCollection<IWebElement> headers = driver.FindElements(By.XPath("//*[@data-test=\"challenge-header\"]"));
            return headers[0].Text;
        }

        protected static void ClickChoiceTextsToClick(List<string> choiceTextsToClick, ReadOnlyCollection<IWebElement> choices)
        {
            HashSet<IWebElement> clickedElements = new();
            foreach (string textToClick in choiceTextsToClick)
            {
                foreach (IWebElement choice in choices)
                {
                    if (!clickedElements.Contains(choice) && choice.Text == textToClick)
                    {
                        choice.Click();
                        clickedElements.Add(choice);
                        break;
                    }
                }
            }
        }
    }
}
