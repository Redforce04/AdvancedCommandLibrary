// -----------------------------------------------------------------------
// <copyright file="ParentCommandTracker.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Trackers;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Used to keep track of parent commands.
/// </summary>
internal class ParentCommandTracker : CommandTracker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParentCommandTracker"/> class.
    /// </summary>
    internal ParentCommandTracker()
    {
    }

    /// <summary>
    /// Gets or sets a method that should be called when LoadGeneratedCommands is called.
    /// </summary>
    internal MethodInfo? LoadGeneratedCommandsExecutor { get; set; }

    /// <summary>
    /// Gets a list of Child Commands.
    /// </summary>
    internal List<CommandTracker> ChildCommands => this.ChildCommandsIds.Select(childId => CommandManager.Instance.RegisteredCommands[childId]).ToList();

    /// <summary>
    /// Gets or sets a list of ids that is used to find child commands.
    /// </summary>
    private List<int> ChildCommandsIds { get; set; } = new();

    /// <summary>
    /// Updates the <see cref="ChildCommandsIds"/>.
    /// </summary>
    /// <param name="childIds">The new child ids.</param>
    internal void UpdateChildren(List<CommandTracker> childIds) => this.ChildCommandsIds = childIds.Select(x => x.Id).ToList();

    /// <summary>
    /// Adds a single child id.
    /// </summary>
    /// <param name="child">The child to add.</param>
    internal void AddChild(CommandTracker child) => this.ChildCommandsIds.Add(child.Id);
}