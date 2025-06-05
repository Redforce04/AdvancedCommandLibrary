// -----------------------------------------------------------------------
// <copyright file="LoadGeneratedCommandsContext.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Contexts.Helpers;

using System;

using CommandSystem;
using JetBrains.Annotations;

/// <summary>
/// The arguments for when LoadGeneratedCommands is called.
/// </summary>
public class LoadGeneratedCommandsContext : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadGeneratedCommandsContext"/> class.
    /// </summary>
    /// <param name="command">The base command.</param>
    /// <param name="parentCommand">The parent command.</param>
    internal LoadGeneratedCommandsContext(ICommand command, ParentCommand? parentCommand = null)
    {
        this.CommandInstance = command;
        this.ParentCommand = parentCommand;
    }

    /// <summary>
    /// Gets the base command instance.
    /// </summary>
    [PublicAPI]
    public ICommand CommandInstance { get; }

    /// <summary>
    /// Gets the parent command instance if it exists.
    /// </summary>
    [PublicAPI]
    public ParentCommand? ParentCommand { get; }
}