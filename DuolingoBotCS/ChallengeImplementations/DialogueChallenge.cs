using DuolingoBotCS.ChallengeBaseClasses;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class DialogueChallenge : ChoiceChallenge
    {
        internal override void Run()
        {
            try
            {
                TryClickNextButton();
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
