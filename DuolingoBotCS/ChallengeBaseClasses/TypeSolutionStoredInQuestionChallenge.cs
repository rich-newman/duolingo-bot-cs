using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeBaseClasses
{
    /// <summary>
    /// Solves challenges that provide an input box for entry of text, with a div as a sibling that contains the solution in the question
    /// These are TypeCompleteTableChallenge and CompleteReverseTranslationChallenge currently
    /// </summary>
    /// <remarks>Currently relies on caPDQ CSS class, which is probably very fragile</remarks>
    internal class TypeSolutionStoredInQuestionChallenge : Challenge
    {
        internal override void Run()
        {
            try
            {
                ReadOnlyCollection<IWebElement> spanElements = driver.FindElements(
                    By.XPath("//span[div[@class='caPDQ']]"));
                for (int counter = 0; counter < spanElements.Count; counter++)
                {
                    IWebElement solutionDiv = spanElements[counter].FindElement(By.XPath("div[@class='caPDQ'][1]"));
                    string textToEnter = solutionDiv.GetAttribute("innerHTML").Trim('_');
                    IWebElement solutionInput = spanElements[counter].FindElement(By.XPath("input[@type='text']"));
                    solutionInput.SendKeys(textToEnter);
                }
                ClickCheckButton();
                ClickContinueOrShowTipButton();
            }
            catch (Exception e)
            {
                Program.Log(e);
            }
        }

    }
}
