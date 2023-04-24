using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class CharacterSelectChallenge : SelectChallenge
    {
        protected override ReadOnlyCollection<IWebElement> GetChoices() =>
            driver.FindElements(By.XPath("//div[@data-test=\"challenge-choice\"]//span[@dir=\"ltr\"][@style]"));
    }
}
