using DuolingoBotCS.ChallengeBaseClasses;

namespace DuolingoBotCS.ChallengeImplementations
{
    internal class AssistChallenge : ChoiceChallenge
    {
        protected override string GetQuestion()=> GetQuestionFromChallengeHeader();
    }
}
