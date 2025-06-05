// -----------------------------------------------------------------------
// <copyright file="RequirePermissionNodesAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;

using Contexts;
using JetBrains.Annotations;
using LabApi.Features.Permissions;

/// <summary>
/// Used to indicate that a command requires one or more permission nodes.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionNodesAttribute : RequirePermissionsAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequirePermissionNodesAttribute"/> class.
    /// </summary>
    /// <param name="requireAll">Indicates whether all permission nodes must be present for a player to satisfy the requirements.</param>
    /// <param name="permissions">The permissions the player must have to satisfy the requirements.</param>
    public RequirePermissionNodesAttribute(bool requireAll = true, params string[] permissions)
    {
        this.RequiredPermissionNodes = permissions;
        this.RequireAll = requireAll;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequirePermissionNodesAttribute"/> class.
    /// </summary>
    /// <param name="permissions">The permission the player must have to satisfy the requirements.</param>
    public RequirePermissionNodesAttribute(string permissions)
    {
        this.RequiredPermissionNodes = [permissions];
        this.RequireAll = true;
    }

    /// <summary>
    /// Gets an array of required permission nodes that a player must have in order to satisfy the requirements for executing the command.
    /// </summary>
    [PublicAPI]
    public string[] RequiredPermissionNodes { get; }

    /// <summary>
    /// Gets a value indicating whether all permission nodes must be present for a player to satisfy the requirements.
    /// </summary>
    [PublicAPI]
    public bool RequireAll { get; }

    /// <summary>
    /// Used to check the permissions of the player.
    /// </summary>
    /// <param name="args">The arguments provided to check if a player may execute the command.</param>
    [PublicAPI]
    public override void CheckPermissions(ProcessPermissionsArgs args)
    {
        if (!args.IsAllowed)
        {
            return;
        }

        bool isAllowed = this.RequireAll ? args.Sender.HasPermissions(this.RequiredPermissionNodes) : args.Sender.HasAnyPermission(this.RequiredPermissionNodes);
        args.IsAllowed = isAllowed;
        if (isAllowed)
        {
            return;
        }

        args.Response = $"Sender is missing required permission nodes.";
        if(CommandManager.ShowPermissionsBranches)
        {
            args.Response += $"[Required Permission Nodes: {string.Join(", ", this.RequiredPermissionNodes)}]";
        }
    }
}