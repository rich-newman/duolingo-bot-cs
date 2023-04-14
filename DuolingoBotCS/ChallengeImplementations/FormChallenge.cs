using DuolingoBotCS.ChallengeBaseClasses;
using OpenQA.Selenium;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class FormChallenge: ChoiceChallenge
    {
        protected override string GetQuestion()
        {
            IWebElement sentenceElement = driver.FindElement(By.XPath("//div[@data-prompt]"));
            return sentenceElement.GetAttribute("data-prompt");
        }
    }
}
