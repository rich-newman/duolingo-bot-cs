namespace DuolingoBotCS
{
    internal static class BaseLanguage
    {
        // Should all be upper-case: string comparisons are case-insensitive in general though
        // Text on buttons during challenges
        internal static string Check = "CHECK";
        internal static string Continue = "CONTINUE";
        internal static string Skip = "SKIP";
        internal static string ShowTip = "SHOW TIP";
        // Text on button at end of story when asking for writing exercise
        internal static string SkipExercise = "SKIP EXERCISE";
        // Text when click next lesson in tree
        internal static string Start = "START";
        // Text on button when asking if you want a recap lesson on main tree when come back after a period away
        internal static string NoThanks = "NO THANKS";
        // Text when click a chest in the tree when it's the next item in the tree
        internal static string OpenChest = "OPEN CHEST";
        // Text when skip/guess a question in a challenge and correct answer is displayed
        internal static string CorrectSolution = "CORRECT SOLUTION:"; // Need the : here so we don't find solution if it's solutionS
        // Edge case: NameChallenge can give more than one solution, and different text identifying it 
        internal static string CorrectSolutions = "CORRECT SOLUTIONS";
        // Used to identify stories on the main tree
        internal static string Story = "STORY";

        // If you are broken in debug in DoTree you can call, e.g., BaseLanguage.Set("english") from the Immediate Window to change
        internal static void Set(string language)
        {
            if (language.ToLower() == "english")
            {
                Check = "CHECK";
                Continue = "CONTINUE";
                Skip = "SKIP";
                SkipExercise = "SKIP EXERCISE";
                Start = "START";
                NoThanks = "NO THANKS";
                OpenChest = "OPEN CHEST";
                CorrectSolution = "CORRECT SOLUTION:";
                CorrectSolutions = "CORRECT SOLUTIONS";
                Story = "STORY";
            }
            else if (language.ToLower() == "francais" || language.ToLower() == "français")
            {
                Check = "VALIDER";
                Continue = "CONTINUER";
                Skip = "PASSER";
                SkipExercise = "PASSER";
                Start = "COMMENCER";
                NoThanks = "NON MERCI";
                OpenChest = "OUVRIR LE COFFRE";
                CorrectSolution = "LA BONNE RÉPONSE EST";
                CorrectSolutions = "SOLUTIONS EXACTES";
                Story = "HISTOIRE";
            }
            else if (language.ToLower() == "espanol" || language.ToLower() == "español")
            {
                Check = "COMPROBAR";
                Continue = "CONTINUAR";
                Skip = "SALTAR";
                SkipExercise = "SALTAR";
                Start = "EMPEZAR";
                NoThanks = "NO GRACIAS";
                OpenChest = "ABRE EL COFRE";
                CorrectSolution = "SOLUCIÓN CORRECTA";
                CorrectSolutions = "SOLUCIONES CORRECTAS";
                Story = "CUENTO";
            }
        }
    }
}
