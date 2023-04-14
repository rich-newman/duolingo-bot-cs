using DuolingoBotCS.ChallengeBaseClasses;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class SelectChallenge: ChoiceChallenge
    {
        protected override string GetQuestion() => GetQuestionFromChallengeHeader();

        protected override ReadOnlyCollection<IWebElement> GetChoices() =>
            driver.FindElements(By.XPath("//div[@data-test=\"challenge-choice\"]//span[@dir=\"ltr\"]"));
    }
}
