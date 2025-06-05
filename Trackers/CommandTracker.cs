// -----------------------------------------------------------------------
// <copyright file="CommandTracker.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Trackers;

using System.Reflection;

using CommandSystem;
using Enums;

/// <summary>
/// Used to keep track of child commands.
/// </summary>
internal class CommandTracker
{
    // ReSharper disable once InconsistentNaming
    private int parentTrackerId = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandTracker"/> class.
    /// </summary>
    /// <param name="id">The id of the command.</param>
    internal CommandTracker(int id = -1)
    {
        if (id == -1)
        {
            this.Id = LastId;
            LastId += 1;
        }
        else
        {
            this.Id = id;
        }
    }

    /// <summary>
    /// Gets or sets the id used to keep track of this command tracker.
    /// </summary>
    internal int Id { get; set; }

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    internal required string Name { get; init; }

    /// <summary>
    /// Gets the description for the command.
    /// </summary>
    internal required string Description { get; init; }

    /// <summary>
    /// Gets the aliases for the command.
    /// </summary>
    internal required string[] Aliases { get; init; }

    /// <summary>
    /// Gets the usages for the command.
    /// </summary>
    internal required string[] Usages { get; init; }

    /// <summary>
    /// Gets the <see cref="CommandHandlerType"/> for processing the command.
    /// </summary>
    internal required CommandHandlerType HandlerType { get; init; }

    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> instance for the registered game command.
    /// </summary>
    internal ICommand? GameCommandInstance { get; set; }

    /// <summary>
    /// Gets the method to be called when this command is executed.
    /// </summary>
    internal required MethodInfo Method { get; init; }

    /// <summary>
    /// Gets the assembly that the <see cref="Method"/> is located in.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    internal required Assembly Assembly { get; init; }

    /// <summary>
    /// Gets the <see cref="ParentCommandTracker"/> if this command has a parent.
    /// </summary>
    internal ParentCommandTracker? ParentTrackerInstance => parentTrackerId is -1 ? null : CommandManager.Instance.RegisteredCommands[this.parentTrackerId] as ParentCommandTracker;

    /// <summary>
    /// Gets the <see cref="PermissionsTracker"/> if this command has a permission requirement.
    /// </summary>
    internal PermissionsTracker? PermissionsRequirementTracker { get; init; }

    /// <summary>
    /// Gets or sets the last id used to generate commands. This can be used when auto-generating ids.
    /// </summary>
    private static int LastId { get; set; }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return this.Id;
    }

    /// <summary>
    /// Used to update the <see cref="ParentTrackerInstance"/> for this command.
    /// </summary>
    /// <param name="tracker">The new tracker to use.</param>
    internal void UpdateParentTracker(ParentCommandTracker? tracker) => this.parentTrackerId = tracker?.Id ?? -1;

    /// <summary>
    /// Used to indicate whether this command is the same as another command.
    /// </summary>
    /// <param name="other">The other command to check.</param>
    /// <returns>True if the command is the same, otherwise False.</returns>
    protected bool Equals(CommandTracker other)
    {
        return this.Id == other.Id;
    }
}