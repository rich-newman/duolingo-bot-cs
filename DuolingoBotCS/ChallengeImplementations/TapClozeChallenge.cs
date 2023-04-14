using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class TapClozeChallenge: Challenge
    {
        private readonly Dictionary<string, string> dictionary = new();
        // Slightly different approach to elsewhere because the solution is an entire sentence and we'd have to pull the 
        // bit we need out of it.  Instead we only have two answers to try, so order them, try the first one, and if we
        // fail the second one must be right.
        internal override void Run()
        {
            try
            {
                string question = GetQuestionFromHintTokens();
                ReadOnlyCollection<IWebElement> choices = driver.FindElements(
                    By.XPath("//div[@data-test=\"word-bank\"]//span[@data-test=\"challenge-tap-token-text\"]"));
                var orderedChoices = choices.OrderBy(t => t.Text).ToList();
                question = MakeQuestionUnique(question, choices);
                if (dictionary.ContainsKey(question))
                {
                    orderedChoices[1].Click();
                    ClickCheckButton();
                }
                else
                {
                    orderedChoices[0].Click();
                    ClickCheckButton();
                    dictionary[question] = choices[1].Text;  // If we fail the other solution must be the right one
                }
                ClickContinueOrShowTipButton();
            }
            catch (Exception e)
            {
                Program.Log(e);
            }
        }
    }
}
