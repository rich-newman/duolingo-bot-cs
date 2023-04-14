using DuolingoBotCS.ChallengeImplementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DuolingoBotCS.Tests
{
    /// <summary>
    /// Class to test the logic in the translate challenge
    /// </summary>   
    [TestClass]
    public class TranslateChallengeTest
    {
        [TestMethod]
        public void DashSplitInMiddleOfWordRealTest()
        {
            string solution = "They haven't ever touched a hedgehog.";
            string[] choiceTexts = new string[] 
                { "ever", "n't", "euros", "have", "hedgehog", "smelled", "Emma", "man", "They", "a", "touched"};
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 7);
            Assert.AreEqual(solutions[0], choiceTexts[8]);
            Assert.AreEqual(solutions[1], choiceTexts[3]);
            Assert.AreEqual(solutions[2], choiceTexts[1]);
            Assert.AreEqual(solutions[3], choiceTexts[0]);
            Assert.AreEqual(solutions[4], choiceTexts[10]);
            Assert.AreEqual(solutions[5], choiceTexts[9]);
            Assert.AreEqual(solutions[6], choiceTexts[4]);
        }

        [TestMethod]
        public void DashesSplitAndNotSplitRealTest()
        {
            string solution = "Mon rendez-vous a duré jusqu'à quatre heures de l'après-midi.";
            string[] choiceTexts = new string[]
                { "Mon", "rendez-vous", "a", "duré", "jusqu'à", "quatre", "heures", "de", "l'", 
                    "après", "midi", "ton", "céréales", "matin", "pensé"};
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 11);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
            Assert.AreEqual(solutions[2], choiceTexts[2]);
            Assert.AreEqual(solutions[3], choiceTexts[3]);
            Assert.AreEqual(solutions[4], choiceTexts[4]);
            Assert.AreEqual(solutions[5], choiceTexts[5]);
            Assert.AreEqual(solutions[6], choiceTexts[6]);
            Assert.AreEqual(solutions[7], choiceTexts[7]);
            Assert.AreEqual(solutions[8], choiceTexts[8]);
            Assert.AreEqual(solutions[9], choiceTexts[9]);
            Assert.AreEqual(solutions[10], choiceTexts[10]);
        }

        [TestMethod]
        public void TwoDashesRealTest()
        {
            string solution = "Pourquoi a-t-elle laissé ses cartes de crédit ?";
            string[] choiceTexts = new string[]
                { "Pourquoi", "t", "a", "elle", "laissé", "ses", "cartes", "de", "crédit",
                    "un", "elle", "voleur", "brillant", "police"};
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 9);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[2]);
            Assert.AreEqual(solutions[2], choiceTexts[1]);
            Assert.AreEqual(solutions[3], choiceTexts[3]);
            Assert.AreEqual(solutions[4], choiceTexts[4]);
            Assert.AreEqual(solutions[5], choiceTexts[5]);
            Assert.AreEqual(solutions[6], choiceTexts[6]);
            Assert.AreEqual(solutions[7], choiceTexts[7]);
            Assert.AreEqual(solutions[8], choiceTexts[8]);
        }

        [TestMethod]
        public void RegularDuplicateTest()
        {
            string solution = "foo, foo.";
            string[] choiceTexts = new string[] { "foo", "foo" };
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            // We can't match, only one foo token and need two
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
        }

        [TestMethod]
        public void RegularDashTest()
        {
            string solution = "foo-bar";
            string[] choiceTexts = new string[] { "bar", "foo" };
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            // We can't match, only one foo token and need two
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
        }

        [TestMethod]
        public void RegularDashWithDuplicateTest()
        {
            string solution = "foo-foo.";
            string[] choiceTexts = new string[] { "foo", "foo" };
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            // We can't match, only one foo token and need two
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
        }

        [TestMethod]
        public void DuplicateWithApostropheTest()
        {
            string solution = "foo'foo.";
            string[] choiceTexts = new string[] { "foo" };
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            // We can't match, only one foo token and need two
            Assert.AreEqual(solutions.Count, 0);
        }

        [TestMethod]
        public void DuplicateWithApostropheOnSecondChoiceTest()
        {
            string solution = "foo'foo.";
            string[] choiceTexts = new string[] { "foo", "'foo" };
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
        }

        [TestMethod]
        public void DuplicateWithApostropheOnFirstChoiceTest()
        {
            string solution = "foo'foo.";
            string[] choiceTexts = new string[] { "foo'", "foo" };
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
        }

        [TestMethod]
        public void DuplicateWithNoApostropheOnTokenTest()
        {
            string solution = "foo'foo.";
            string[] choiceTexts = new string[] { "foo", "foo" };
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            // We can't match, we assume the ' appears on a token at present
            Assert.AreEqual(solutions.Count, 0);
        }

        // Not sure if this is possible in a TranslateChallenge at present
        // We will need additional logic if it is, as this doesn't work!
        [TestMethod]
        public void TwoWordsOnChoiceTest()
        {
            string solution = "No one saw him.";
            string[] choiceTexts = new string[] { "him", "No one", "saw" };
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);  // Should be 3
        }

        // This one is possible, and is coded for
        [TestMethod]
        public void ApostropheAndDashTest()
        {
            string solution = "l'après-midi";
            string[] choiceTexts = new string[] { "midi", "après", "l'" };
            List<string> solutions = new TranslateSolver(solution, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 3);
            Assert.AreEqual(solutions[0], choiceTexts[2]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
            Assert.AreEqual(solutions[2], choiceTexts[0]);
        }

    }
}
