// -----------------------------------------------------------------------
// <copyright file="ICommandPermission.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.GameCommandModules;

using CommandSystem;

/// <summary>
/// An interface used to implement permissions.
/// </summary>
public interface ICommandPermission
{
    /// <summary>
    /// Gets the permission node to check for.
    /// </summary>
    public string PermissionNode { get; }

    /// <summary>
    /// Called when the command is checking permissions.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="missingPermissionString">A response indicating whether the command is approved or why it was denied.</param>
    /// <returns>True if the command can run, otherwise false.</returns>
    public bool CheckPermission(ICommandSender sender, out string missingPermissionString);
}