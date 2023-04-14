namespace DuolingoBotCS.ChallengeImplementations
{
    /// <summary>
    /// Method object to get TapCompleteChallenge solutions given question, answer, and choices we can 'tap'
    /// </summary>
    internal class TapCompleteSolver
    {
        // Data inputs to the method object
        private readonly IEnumerable<string> choiceTexts;
        private readonly string[] solutionWords;
        private readonly string[] questionWords;

        // Variables used in method
        private string currentMismatches = "";
        private int questionWordsPosition = 0;
        private bool finishedQuestionWords;

        // Output
        private readonly List<string> results = new();

        internal TapCompleteSolver(string fullSolution, string originalQuestion, IEnumerable<string> choiceTexts)
        {
            this.choiceTexts = choiceTexts;
            solutionWords = Challenge.RemovePunctuation(fullSolution).Split(' ');
            questionWords = Challenge.RemovePunctuation(originalQuestion).Trim().Split(' ');
            finishedQuestionWords = questionWords.Length == 0;  // false unless there's nothing there
        }

        // Returns the choices texts that match the gaps in originalQuestion compared to fullSolution in order they appear
        // We do this by iterating the words in the solution and trying to match with the current position in the question.  If we don't
        // match we add to a variable representing unmatched words (currentMismatches) until we match again.  When we match
        // again we iterate the currentMismatches and try to find valid choices that represent it: we add these to our solution.
        internal List<string> GetChoiceTextsToClick()
        {
            foreach (string solutionWord in solutionWords)
            {
                string questionWord = finishedQuestionWords ? "" : questionWords[questionWordsPosition];
                if (QuestionWordMatchesSolutionWordExactly(questionWord, solutionWord))
                {
                    // We've got a match so we can add any previous mismatches into the results
                    AddCurrentMismatchesToResults();
                    IncrementQuestionWordsPosition();
                }
                else if (ThisAndNextQuestionWordConcatenatedMatchSolutionWord(questionWord, solutionWord))
                {
                    AddCurrentMismatchesToResults();
                    IncrementQuestionWordsPosition(increment: 2);
                }
                else if (QuestionWordMatchesStartOrEndOfSolutionWord(questionWord, solutionWord))
                {
                    TryFindResultWithPartialWordMatch(questionWord, solutionWord);
                }
                else
                {
                    AddMismatchedSolutionWordToCurrentMismatches(solutionWord);
                }
            }
            AddCurrentMismatchesToResults();
            return results;
        }

        private bool QuestionWordMatchesSolutionWordExactly(string questionWord, string solutionWord) =>
            !finishedQuestionWords && solutionWord == questionWord;

        // The currentMismatches string is one or more words separated by spaces.  We assume the entire string can appear as a choice, 
        // including if it's more than one word, or the individual words appear as separate choices.  At present we don't deal with
        // the case where, for example, out of three words one appears on one choice and two together appear on another.  We deal with
        // dashes and apostrophes in the main loop, not here.
        private void AddCurrentMismatchesToResults()
        {
            if (currentMismatches.Length > 0)
            {
                currentMismatches = currentMismatches.Trim();
                if (choiceTexts.Contains(currentMismatches))
                {
                    results.Add(currentMismatches);
                }
                else if (currentMismatches.Contains(' '))
                {
                    string[] splitWords = currentMismatches.Split(' ');
                    foreach (string splitWord in splitWords)
                    {
                        if (choiceTexts.Contains(splitWord))
                            results.Add(splitWord);
                        else
                            throw new Exception($"Unable to find solution string {currentMismatches} in choices");
                    }
                }
                currentMismatches = "";
            }
        }

        private void IncrementQuestionWordsPosition(int increment = 1)
        {
            questionWordsPosition += increment;
            if (questionWordsPosition == questionWords.Length)
            {
                finishedQuestionWords = true;
            }
        }

        private bool ThisAndNextQuestionWordConcatenatedMatchSolutionWord(string questionWord, string solutionWord) =>
            !finishedQuestionWords && (questionWordsPosition + 1) < questionWords.Length &&
            ((questionWord + questionWords[questionWordsPosition + 1]) == solutionWord ||
            (questionWord + "-" + questionWords[questionWordsPosition + 1]) == solutionWord);

        // Deal with edge case where the solution word contains the question word at the start or the end
        // We assume it isn't possible to be in the middle
        // E.g. solution has l'école, question has l' and école is a choice to click.
        private bool QuestionWordMatchesStartOrEndOfSolutionWord(string questionWord, string solutionWord) =>
            !finishedQuestionWords && (solutionWord.StartsWith(questionWord) || solutionWord.EndsWith(questionWord));

        private void TryFindResultWithPartialWordMatch(string questionWord, string solutionWord)
        {
            // Get the part of the solutionWord that is not questionWord (it either starts or ends with questionWord)
            // solutionWord[..^questionWord.Length] is the same as
            // solutionWord.Substring(0, solutionWord.Length - questionWord.Length)
            string restOfSolutionWord = solutionWord.StartsWith(questionWord) ?
                solutionWord[questionWord.Length..] : solutionWord[..^questionWord.Length];
            if (choiceTexts.Contains(restOfSolutionWord))
            {
                // We have part of a word matching, and part not and that part is a choice
                // Firstly we may already have identified a word preceding this that is a choice
                // We assume in these edge cases we're not going to see two words on a choice with one a partial match
                AddCurrentMismatchesToResults();
                IncrementQuestionWordsPosition();
                // Add the part of the word that doesn't match to our results
                results.Add(restOfSolutionWord);
            }
            else
            {
                // There's no partial match after all, add the solution word to our current mismatches
                AddMismatchedSolutionWordToCurrentMismatches(solutionWord);
            }
        }

        private void AddMismatchedSolutionWordToCurrentMismatches(string solutionWord) => currentMismatches += solutionWord + " ";
    }
}
