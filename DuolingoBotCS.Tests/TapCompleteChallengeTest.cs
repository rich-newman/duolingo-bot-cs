using DuolingoBotCS.ChallengeImplementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DuolingoBotCS.Tests
{
    /// <summary>
    /// Class to test the logic in the tap complete challenge
    /// </summary>
    /// <remarks>
    /// This is a little random, as there aren't any other tests, primarily because you end up testing your mock of Selenium more than
    /// logic in the code.  The difficult thing in this project is minimizing exceptions when the web page is in the wrong state,
    /// and getting waits right, neither of which is easy to unit test.  However, the logic for the tap complete challenge needs to 
    /// deal with a number of edge cases, and TDD seemed a good way to make sure the code caught them.
    /// </remarks>    
    [TestClass]
    public class TapCompleteChallengeTest
    {
        [TestMethod]
        public void OneWordTest()
        {
            string fullSolution = "¡El museo está cerrado!";
            string originalQuestion = "El está cerrado ";
            string[] choiceTexts = new string[] { "dinero", "boleto", "museo", "agua" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[2]);
        }

        [TestMethod]
        public void TwoWordsTest()
        {
            string fullSolution = "¿Dónde está la niña? ¿Está en el hospital?";
            string originalQuestion = "Dónde está niña Está en hospital ";
            string[] choiceTexts = new string[] { "la", "el" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
        }

        [TestMethod]
        public void OneWordAtEndTest()
        {
            string fullSolution = "Yo tengo tu boleto a Madrid.";
            string originalQuestion = "Yo tengo tu boleto a ";
            string[] choiceTexts = new string[] { "teléfono", "Madrid", "es", "pan" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
        }

        [TestMethod]
        public void TwoWordsOneAtEndTest()
        {
            string fullSolution = "¿Dónde está el boleto? ¿Aquí en la maleta?";
            string originalQuestion = "Dónde está el Aquí en la ";
            string[] choiceTexts = new string[] { "maleta", "boleto" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
        }

        [TestMethod]
        public void TwoWordsInChoiceTest()
        {
            string fullSolution = "Hier, tu es allé chez tes amis. Et aujourd'hui, tu vas chez qui ?";
            string originalQuestion = "Hier tu chez tes amis Et aujourd' hui tu chez qui ";
            string[] choiceTexts = new string[] { "vas", "es allé" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
        }

        [TestMethod]
        public void ConsecutiveChoicesInSolutionTest()
        {
            // Not sure if this is possible in Duo, but we should deal with it anyway
            string fullSolution = "Hier, tu es allé chez tes amis. Et aujourd'hui, tu vas chez qui ?";
            string originalQuestion = "Hier tu chez tes amis Et aujourd' hui tu chez qui ";
            string[] choiceTexts = new string[] { "allé", "es", "vas" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 3);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
            Assert.AreEqual(solutions[2], choiceTexts[2]);
        }

        [TestMethod]
        public void ApostropheMatchesTest()
        {
            string fullSolution = "Mon père travaille à l'hôpital.";
            string originalQuestion = "Mon père à l' hôpital ";
            string[] choiceTexts = new string[] { "veux", "travaille", "prend", "s'appelle" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
        }

        [TestMethod]
        public void ApostropheInChoiceTest()
        {
            string fullSolution = "Je vais à la station de métro et il va à l'hôpital.";
            string originalQuestion = "Je vais à station de métro et il va à hôpital ";
            string[] choiceTexts = new string[] { "l'", "la" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
        }

        // Invented because I'm not sure these edge cases exist, but I want the code to deal with them
        [TestMethod]
        public void MatchPartWordWithApostropheTest()
        {
            string fullSolution = "I did the job but I couldn't do it in a day.";
            string originalQuestion = "I the job but I n't do it in a day ";
            string[] choiceTexts = new string[] { "could", "did" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
        }

        [TestMethod]
        public void MatchPartWordWithApostropheWithChoiceImmediatelyBeforeTest()
        {
            string fullSolution = "I did the job but I really couldn't do it in a day.";
            string originalQuestion = "I the job but I n't do it in a day ";
            string[] choiceTexts = new string[] { "could", "did", "really" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 3);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[2]);
            Assert.AreEqual(solutions[2], choiceTexts[0]);
        }

        [TestMethod]
        public void MatchPartWordWithApostropheWithChoiceImmediatelyAfterTest()
        {
            string fullSolution = "I did the job but I couldn't really do it in a day.";
            string originalQuestion = "I the job but I n't do it in a day ";
            string[] choiceTexts = new string[] { "could", "did", "really" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 3);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
            Assert.AreEqual(solutions[2], choiceTexts[2]);
        }

        [TestMethod]
        public void ChoiceIsWordAfterApostropheTest()
        {
            string fullSolution = "Je vais à la station de métro et il va à l'hôpital aujourd'hui.";
            string originalQuestion = "Je vais à station de métro et il va à l' ";
            string[] choiceTexts = new string[] { "hôpital", "la", "aujourd'hui" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 3);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
            Assert.AreEqual(solutions[2], choiceTexts[2]);
        }

        [TestMethod]
        public void ChoiceIsWordAtEndAfterApostropheTest()
        {
            string fullSolution = "Je vais à la station de métro et il va à l'hôpital.";
            string originalQuestion = "Je vais à station de métro et il va à l' ";
            string[] choiceTexts = new string[] { "hôpital", "la" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
        }

        [TestMethod]
        public void ApostropheInChoiceAndMatchesAndTwoWordsInChoiceTest()
        {
            string fullSolution = "Oui, ce sont les pharmacies et c'est l'hôpital.";
            string originalQuestion = "Oui les pharmacies et l' hôpital ";
            string[] choiceTexts = new string[] { "ce sont", "c'est" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
        }

        [TestMethod]
        public void FrenchQuotesInQuestionTest()
        {
            string fullSolution = "Je suis à la station de métro « Concorde ».";
            string originalQuestion = "Je suis à la station de Concorde ";
            string[] choiceTexts = new string[] { "fromage", "métro", "magasin", "restaurant" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
        }

        [TestMethod]
        public void ActualComplexApostropheTest()
        {
            string fullSolution = "Marc est arrivé à l'aéroport à l'heure.";
            string originalQuestion = "Marc est à l' à l' heure ";
            string[] choiceTexts = new string[] { "arrivé", "école", "aimé", "aéroport", "ordinateur", "acheté" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[3]);
        }

        [TestMethod]
        public void ActualApostropheMatchTest()
        {
            string fullSolution = "Hier, Victor est arrivé en retard. D'habitude, il arrive à l'heure.";
            string originalQuestion = "Hier Victor en retard D' habitude il à l' heure ";
            string[] choiceTexts = new string[] { "est arrivé", "arrive" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
        }

        [TestMethod]
        public void BasicDashTest()
        {
            string fullSolution = "Maman, est-ce que c'est le lapin de Pâques ?";
            string originalQuestion = "Maman est ce que c' est le de Pâques ";
            string[] choiceTexts = new string[] { "fruit", "lapin", "cochon", "zoo" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
        }

        [TestMethod]
        public void DashInSolutionTest()
        {
            string fullSolution = "Le quatorze juillet, c'était la fête sur les Champs-Élysées !";
            string originalQuestion = "Le quatorze juillet c' était la fête sur les ";
            string[] choiceTexts = new string[] { "étages", "Pâques", "Champs-Élysées", "souvenirs" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[2]);
        }

        [TestMethod]
        public void DashInMatchAtEndTest()
        {
            string fullSolution = "Nous sommes allés au Louvre. Nous voulons aussi visiter Notre-Dame.";
            string originalQuestion = "Nous sommes allés au Louvre Nous voulons aussi Notre Dame ";
            string[] choiceTexts = new string[] { "comprendre", "porter", "visiter", "appeler" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[2]);
        }

        [TestMethod]
        public void PunctuationTest()
        {
            string fullSolution = "El pájaro le dijo al hombre: \"No bebas café o no dormirás\".";
            string originalQuestion = "El pájaro le dijo al hombre No bebas o no dormirás ";
            string[] choiceTexts = new string[] { "café", "agua", "cuadernos", "antibióticos" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
        }

        [TestMethod]
        public void DashPartialMatchStartTest()
        {
            string fullSolution = "Oui, expliquons-la bien.";
            string originalQuestion = "Oui expliquons bien ";
            string[] choiceTexts = new string[] { "la", "leur", "expliquons" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
        }

        [TestMethod]
        public void DashPartialMatchEndTest()
        {
            string fullSolution = "Oui, expliquons-la bien.";
            string originalQuestion = "Oui la bien ";
            string[] choiceTexts = new string[] { "la", "leur", "expliquons" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 1);
            Assert.AreEqual(solutions[0], choiceTexts[2]);
        }

        [TestMethod]
        public void DashExactMatchTest()
        {
            string fullSolution = "Oui, expliquons-la bien.";
            string originalQuestion = "Oui expliquons la bien ";
            string[] choiceTexts = new string[] { "la", "leur", "expliquons" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 0);
        }

        [TestMethod]
        public void DashedNoMatchTest()
        {
            string fullSolution = "Oui, expliquons-la bien.";
            string originalQuestion = "Oui bien ";
            string[] choiceTexts = new string[] { "la", "leur", "expliquons" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[2]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
        }

        // An actual Duo question
        [TestMethod]
        public void TwoDashesPartialMatchStartTest()
        {
            string fullSolution = "C'est une demande un peu spéciale, expliquons-la-leur bien.";
            string originalQuestion = "C' est une demande un peu spéciale expliquons bien ";
            string[] choiceTexts = new string[] { "la", "leur" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[0]);
            Assert.AreEqual(solutions[1], choiceTexts[1]);
        }

        [TestMethod]
        public void TwoDashesPartialMatchEndTest()
        {
            string fullSolution = "C'est une demande un peu spéciale, expliquons-la-leur bien.";
            string originalQuestion = "C' est une demande un peu spéciale leur bien ";
            string[] choiceTexts = new string[] { "la", "leur", "expliquons" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[2]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
        }

        [TestMethod]
        public void TwoDashesNoMatchTest()
        {
            string fullSolution = "C'est une demande un peu spéciale, expliquons-la-leur bien.";
            string originalQuestion = "C' est une demande un peu spéciale bien ";
            string[] choiceTexts = new string[] { "la", "leur", "expliquons" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 3);
            Assert.AreEqual(solutions[0], choiceTexts[2]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
            Assert.AreEqual(solutions[2], choiceTexts[1]);
        }

        [TestMethod]
        public void TolerateLastWordMissingInQuestionTest()
        {
            string fullSolution = "Lui, il aurait annulé la commande et il serait parti en râlant.";
            string originalQuestion = "Lui il annulé la commande et il parti en ";
            string[] choiceTexts = new string[] { "serait", "aurait" };
            List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
            Assert.AreEqual(solutions.Count, 2);
            Assert.AreEqual(solutions[0], choiceTexts[1]);
            Assert.AreEqual(solutions[1], choiceTexts[0]);
        }

        // Code can't deal with this at present, not sure if it needs to
        //[TestMethod]
        //public void TwoDashesExactMatchTest()
        //{
        //    string fullSolution = "C'est une demande un peu spéciale, expliquons-la-leur bien.";
        //    string originalQuestion = "C' est une demande un peu spéciale expliquons la leur bien ";
        //    string[] choiceTexts = new string[] { "la", "leur", "expliquons" };
        //    List<string> solutions = new TapCompleteSolver(fullSolution, originalQuestion, choiceTexts).GetChoiceTextsToClick();
        //    Assert.AreEqual(solutions.Count, 0);
        //}

    }
}
