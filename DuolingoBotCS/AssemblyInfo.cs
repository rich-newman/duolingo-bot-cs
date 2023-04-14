using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

// Suppress the message that ContainsKey on a dictionary should be replaced with TryGetValue when you're trying to find out if a
// dictionary contains a key
[assembly: SuppressMessage("Category", "CA1854", Justification = "Introduces an unnecessary variable that often conflicts with " +
    "existing variables representing the same thing and makes code more difficult to read, " +
    "just to save an O(1) lookup")]

[assembly: InternalsVisibleTo("DuolingoBotCS.Tests")]