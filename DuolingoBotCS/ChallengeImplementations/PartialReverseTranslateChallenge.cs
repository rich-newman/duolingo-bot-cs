using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class PartialReverseTranslateChallenge : Challenge
    {
        internal override void Run()
        {
            try
            {
                IWebElement editableSpanElement = driver.FindElement(
                    By.XPath("//span[@contenteditable=\"true\"]"));
                // Not sure how reliable this will be!  But it's too tempting not to try.
                IWebElement solutionElement = driver.FindElement(
                    By.XPath("//span[@class=\"_31xxw _2eX9t _1vqO5\"]"));
                string textToEnter = solutionElement.GetAttribute("innerHTML");
                editableSpanElement.SendKeys(textToEnter);
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
