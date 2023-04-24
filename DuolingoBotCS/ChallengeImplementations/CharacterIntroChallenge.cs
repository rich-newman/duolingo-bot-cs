using DuolingoBotCS.ChallengeBaseClasses;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class CharacterIntroChallenge : ChoiceChallenge
    {
        protected override string GetQuestion()
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(
                By.XPath("//div[@class='_11apW']/span"));
            string text = elements[0].Text;
            return text;
        }
    }
}
