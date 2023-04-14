using OpenQA.Selenium;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class CompleteReverseTranslationChallenge: Challenge
    {
        internal override void Run()
        {
            try
            {
                // Not sure how reliable this will be!  But it's too tempting not to try.
                IWebElement solutionElement = driver.FindElement(
                    By.XPath("//div[@class=\"caPDQ\"]"));
                string textToEnter = solutionElement.GetAttribute("innerHTML").Trim('_');
                EnterTextIntoChallengeTextInput(textToEnter);
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
