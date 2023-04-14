using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuolingoBotCS.ChallengeBaseClasses
{
    internal class SkipChallenge: Challenge
    {
        internal override void Run()
        {
            // Click Skip and then click Continue/Next to go to next screen
            // This implementation can be used for listen and speak tests as nextButton disables the test type
            try
            {
                ClickSkipButton();
                ClickContinueButton();
            }
            catch (Exception e)
            {
                Program.Log(e);
            }
        }
    }
}
