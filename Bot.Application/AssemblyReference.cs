using System.Reflection;

namespace Bot.Application;

/// <summary>
/// Ссылка на сборку Application.
/// </summary>
public static class AssemblyReference
{
    /// <summary>
    /// Ссылка на сборку Application.
    /// </summary>
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}