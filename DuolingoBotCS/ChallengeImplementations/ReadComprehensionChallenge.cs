using DuolingoBotCS.ChallengeBaseClasses;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class ReadComprehensionChallenge: ChoiceChallenge
    {
        internal override void Run()
        {
            try
            {
                // NOTE we click next FIRST here - we may have a Next button before we start
                TryClickNextButton();  
                base.Run();
            }
            catch (Exception e)
            {
                Program.Log(e);
            }
        }
    }
}
