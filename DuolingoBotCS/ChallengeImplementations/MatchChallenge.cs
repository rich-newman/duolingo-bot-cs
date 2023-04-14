using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class MatchChallenge: Challenge
    {
        internal override void Run()
        {
            try
            {
                ReadOnlyCollection<IWebElement> choices = driver.FindElements(
                    By.XPath("//button[.//span[@data-test=\"challenge-tap-token-text\"]]"));
                RunMatch(choices);
                ClickContinueOrShowTipButton();
            }
            catch (Exception e)
            {
                Program.Log(e);
            }
        }

        internal static void RunMatch(ReadOnlyCollection<IWebElement> choices)
        {
            if (choices.Count == 0) return;
            int numberOfPairs = choices.Count / 2;
            HashSet<int> secondColumnMatched = new();
            for (int firstColumnCounter = 0; firstColumnCounter < numberOfPairs; firstColumnCounter++)
            {
                for (int secondColumnCounter = numberOfPairs; secondColumnCounter < choices.Count; secondColumnCounter++)
                {
                    if (secondColumnMatched.Contains(secondColumnCounter)) continue;
                    choices[firstColumnCounter].Click();
                    choices[secondColumnCounter].Click();
                    Thread.Sleep(1000);  // Wait for the animations and update before testing
                    string disabled = choices[secondColumnCounter].GetAttribute("aria-disabled");
                    if (disabled == "true")
                    {
                        secondColumnMatched.Add(secondColumnCounter);
                        break;
                    }
                }
            }
        }
    }
}
