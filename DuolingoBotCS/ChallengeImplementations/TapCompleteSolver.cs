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
            solutionWords = Challenge.RemovePunctuation(fullSolution).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            questionWords = Challenge.RemovePunctuation(originalQuestion).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
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
                else
                {
                    bool done = TryAddMismatchesSplitByCharacter(' ');
                    if (!done) done = TryAddMismatchesSplitByCharacter('-') ;
                    if (!done) TryAddMismatchesSplitByCharacter(' ', '-');
                }
                currentMismatches = "";
            }
        }

        private bool TryAddMismatchesSplitByCharacter(char character1, char character2)
        {
            if (!(currentMismatches.Contains(character1) && currentMismatches.Contains(character2))) return false;
            return TryAddMismatchesSplitByCharacter(new char[] { character1, character2 });
        }

        private bool TryAddMismatchesSplitByCharacter(char character)
        {
            if (!currentMismatches.Contains(character)) return false;
            return TryAddMismatchesSplitByCharacter(new char[] { character });
        }

        private bool TryAddMismatchesSplitByCharacter(char[] characters)
        {
            string[] splitWords = currentMismatches.Split(characters, StringSplitOptions.RemoveEmptyEntries);
            bool allWordsFound = true;
            foreach (string splitWord in splitWords)
            {
                if (!choiceTexts.Contains(splitWord))
                {
                    allWordsFound = false;
                    break;
                }
            }
            if (allWordsFound) results.AddRange(splitWords);
            return allWordsFound;
        }

        private void IncrementQuestionWordsPosition(int increment = 1)
        {
            questionWordsPosition += increment;
            if (questionWordsPosition == questionWords.Length) finishedQuestionWords = true;
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
            List<string> choices = FindChoicesForSolutionWord(restOfSolutionWord);
            if (choices != null)
            {
                // We have part of a word matching, and part not and that part is a choice or choices
                // Firstly we may already have identified a word preceding this that is a choice
                AddCurrentMismatchesToResults();
                IncrementQuestionWordsPosition();
                // Add the part of the word that doesn't match to our results
                results.AddRange(choices);
            }
            else
            {
                // There's no partial match after all, add the solution word to our current mismatches
                AddMismatchedSolutionWordToCurrentMismatches(solutionWord);
            }
        }

        private List<string> FindChoicesForSolutionWord(string solutionWord)
        {
            if (choiceTexts.Contains(solutionWord)) return new List<string>() { solutionWord };
            if(solutionWord.Contains('-'))
            {
                string[] splitSolutionWords = solutionWord.Split('-', StringSplitOptions.RemoveEmptyEntries);
                bool allMatch = true;
                foreach(string splitSolutionWord in splitSolutionWords)
                {
                    if (!choiceTexts.Contains(splitSolutionWord)) allMatch = false;
                }
                if (allMatch) return splitSolutionWords.ToList();
            }
            return null;
        }

        private void AddMismatchedSolutionWordToCurrentMismatches(string solutionWord) => currentMismatches += solutionWord + " ";
    }
}
