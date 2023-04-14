using OpenQA.Selenium;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class TypeClozeChallenge: Challenge
    {
        internal override void Run()
        {
            try
            {
                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> solutionElement = driver.FindElements(
                    By.XPath("//span[input[not(@type='hidden')]]/div"));
                // There are two divs, we want the first one
                string textToEnter = solutionElement[0].GetAttribute("innerHTML").Trim('_');
                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> inputElement = driver.FindElements(
                    By.XPath("//span[div]/input[not(@type='hidden')]"));
                // Should only be one input meeting the criteria
                inputElement[0].SendKeys(textToEnter);
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
