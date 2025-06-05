// -----------------------------------------------------------------------
// <copyright file="AdvancedParentPermissionsCommand.cs" company="Redforce04">
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
/// A better implementation of the base game parent command with a permission context.
/// </summary>
public abstract class AdvancedParentPermissionsCommand : AdvancedParentCommand, ICommandPermission
{
    /// <summary>
    /// Gets the required permission node.
    /// </summary>
    public abstract string PermissionNode { get; }

    /// <summary>
    /// Called when checking permissions.
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

    /// <inheritdoc />
    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!this.CheckPermission(sender, out response))
        {
            return false;
        }

        return base.ExecuteParent(arguments, sender, out response);
    }
}