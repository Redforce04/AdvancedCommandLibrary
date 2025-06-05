// -----------------------------------------------------------------------
// <copyright file="AdvancedPermissionCommand.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.GameCommandModules;

using System;
using System.Diagnostics.CodeAnalysis;

using CommandSystem;
using LabApi.Features.Permissions;

/// <summary>
/// A better implementation of the base game command with a permission requirement node.
/// </summary>
public abstract class AdvancedPermissionCommand : ICommand, IUsageProvider, ICommandPermission
{
    /// <summary>
    /// Gets the permission node to check for.
    /// </summary>
    public abstract string PermissionNode { get; }

    /// <summary>
    /// Gets the command's name.
    /// </summary>
    public abstract string Command { get; }

    /// <summary>
    /// Gets the command's aliases.
    /// </summary>
    public abstract string[] Aliases { get; }

    /// <summary>
    /// Gets the command's description.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Gets the command's usages.
    /// </summary>
    public abstract string[] Usage { get; }

    /// <summary>
    /// Called when the command is checking permissions.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="missingPermissionString">A response indicating whether the command is approved or why it was denied.</param>
    /// <returns>True if the command can run, otherwise false.</returns>
    public bool CheckPermission(ICommandSender sender, out string missingPermissionString)
    {
        if(sender.HasPermissions(this.PermissionNode))
        {
            missingPermissionString = $"Permission Approved [Permission Branch: {this.PermissionNode}]";
            return true;
        }

        missingPermissionString = $"You do not have permission to use this command. [Permission Branch: {this.PermissionNode}]";
        return false;
    }

    /// <summary>
    /// Called when the command is executed.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="response">The response out.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public virtual bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        bool result = this.CheckPermission(sender, out string permissionString);
        response = permissionString;
        return result;
    }
}