// -----------------------------------------------------------------------
// <copyright file="SoloCommandTesting.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Testing;

using Attributes;
using Contexts;

/// <summary>
/// A solo testing command.
/// </summary>
public class SoloCommandTesting
{
    /// <summary>
    /// A Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [Command("SoloCommand", "An example subcommand.", ["solo"], [])]
    public static void Run(CommandContext context)
    {
        if(!context.CheckPermissions())
        {
            return;
        }

        context.Allow("it worked!");
    }
}