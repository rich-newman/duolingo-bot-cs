namespace DuolingoBotCS.ChallengeImplementations
{
    internal class TranslateSolver
    {
        // Data inputs to the method object
        private readonly List<string> choiceTexts;
        private readonly string[] solutionWords;

        // Output
        private readonly List<string> results = new();

        internal TranslateSolver(string solution, IEnumerable<string> choiceTexts)
        {
            this.choiceTexts = choiceTexts.ToList();
            this.solutionWords = Challenge.RemovePunctuation(solution).Split(" ");
        }

        internal List<string> GetChoiceTextsToClick()
        {
            foreach (string word in solutionWords)
            {
                if (word.Trim().Length == 0) continue;
                bool clicked = MatchWordToChoices(word);
                if (!clicked && word.Contains('\'') && word.Contains('-'))
                    SplitDashedWordWithCharToMatchTokens('\'', word);
                else if (!clicked && word.Contains('\''))
                    SplitWordToMatchChoices('\'', word);
                else if (!clicked && word.Contains('-'))
                    SplitDashedWordToMatchChoices(word);
            }
            return results;
        }

        protected bool MatchWordToChoices(string word)
        {
            string foundChoice = FindWordInList(word, choiceTexts);
            if (foundChoice != null) AddToResults(foundChoice);
            return foundChoice != null;
        }

        private static string FindWordInList(string word, List<string> list)
        {
            string foundChoice = null;
            foreach (string listWord in list)
            {

                if (listWord.Equals(word, StringComparison.OrdinalIgnoreCase)
                    || Challenge.RemovePunctuation(listWord).Equals(word, StringComparison.OrdinalIgnoreCase))
                {
                    foundChoice = listWord;
                    break;
                }
            }
            return foundChoice;
        }

        private void AddToResults(string choiceText)
        {
            results.Add(choiceText);
            choiceTexts.Remove(choiceText);
        }

        private void SplitDashedWordWithCharToMatchTokens(char splitChar, string word)
        {
            string[] wordsToTry = word.Split('-');
            foreach (string wordToTry in wordsToTry)
            {
                if (wordToTry.Contains(splitChar))
                    SplitWordToMatchChoices(splitChar, wordToTry);
                else
                    MatchWordToChoices(wordToTry);
            }
        }

        private void SplitDashedWordToMatchChoices(string word)
        {
            string[] wordsToTry = word.Split('-');
            foreach (string wordToTry in wordsToTry)
                MatchWordToChoices(wordToTry);
        }

        private void SplitWordToMatchChoices(char splitChar, string word)
        {
            // If we have a word including an apostrophe (e.g. L'hôpital or pilot's) then Duo can split it onto
            // two 'word' tokens with the apostrophe in the first word (e.g. L' and hôpital) or the second 
            // (e.g. pilot and 's).  Then there's didn't which gets split did and n't.
            int index = word.IndexOf(splitChar);
            if (!SplitWordByIndexToMatchChoices(index, word))  // Split at index
            {
                if (!SplitWordByIndexToMatchChoices(index + 1, word))  // Split at index + 1
                {
                    // We've tried matching the entire word to a token, then splitting it in two, firstly with everything up to the split
                    // char but not including it, and then the rest, then including the split char as part of the first word and the rest.
                    // That hasn't worked, so iterate through the word splitting at each viable point
                    for (int counter = 1; counter < word.Length; counter++)
                    {
                        if (counter == index || counter == index + 1) continue;
                        if (SplitWordByIndexToMatchChoices(counter, word)) break;
                    }
                }
            }
        }

        private bool SplitWordByIndexToMatchChoices(int index, string word)
        {
            string apostropheWord = word[..index];
            string apostropheWordInChoices = FindWordInList(apostropheWord, choiceTexts);
            if (apostropheWordInChoices == null) return false;
            List<string> toSearch = new(choiceTexts);
            toSearch.Remove(apostropheWordInChoices);
            string restOfWord = word[index..];
            string restOfWordInChoices = FindWordInList(restOfWord, toSearch);
            if (restOfWordInChoices == null) return false;
            AddToResults(apostropheWordInChoices);
            AddToResults(restOfWordInChoices);
            return true;
        }
    }
}
