using OpenQA.Selenium;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class TypeCompleteTableChallenge : Challenge
    {
        internal override void Run()
        {
            try
            {
                // Not sure how reliable this will be!  But it's too tempting not to try.
                // Can also do this by finding the first sibling of the input referred to in EnterText, which may be better
                IWebElement solutionElement = driver.FindElement(
                    By.XPath("//div[@class=\"caPDQ\"]"));
                string textToEnter = solutionElement.GetAttribute("innerHTML").Trim('_');
                EnterText(textToEnter);
                ClickCheckButton();
                ClickContinueOrShowTipButton();
            }
            catch (Exception e)
            {
                Program.Log(e);
            }
        }

        private static void EnterText(string text)
        {
            IWebElement inputElement = driver.FindElement(
                By.XPath("//input[@type=\"text\"]"));
            inputElement.SendKeys(text);
        }
    }
}
