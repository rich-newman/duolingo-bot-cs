using DuolingoBotCS.ChallengeBaseClasses;

namespace DuolingoBotCS.ChallengeImplementations
{
    // Listen and Speak challenges use the SkipChallenge implementation of Run() that just clicks Skip and then Next
    // I've broken the one class per code file rule, I'd fail code reviews everywhere
    internal class SpeakChallenge : SkipChallenge { }
    internal class ListenChallenge : SkipChallenge { }
    internal class ListenCompleteChallenge : SkipChallenge { }
    internal class ListenMatchChallenge : SkipChallenge { }
    internal class ListenIsolationChallenge : SkipChallenge { }
    internal class ListenComprehensionChallenge : SkipChallenge { }
    internal class ListenTapChallenge : SkipChallenge { }
    internal class SelectTranscriptionChallenge : SkipChallenge { }
}
