// -----------------------------------------------------------------------
// <copyright file="RequirePlayerPermissionsAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;
using System.Linq;

using Contexts;
using JetBrains.Annotations;

/// <summary>
/// Used to indicate that a command requires one or more player permissions.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePlayerPermissionsAttribute : RequirePermissionsAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequirePlayerPermissionsAttribute"/> class.
    /// </summary>
    /// <param name="requiredPermissions">The player permissions required from the player.</param>
    public RequirePlayerPermissionsAttribute(params PlayerPermissions[] requiredPermissions)
    {
        ulong requiredPermissionsValue = requiredPermissions.Aggregate<PlayerPermissions, ulong>(0, (current, requiredPermission) => current + (ulong)requiredPermission);

        this.RequiredPlayerPermissions = (PlayerPermissions)requiredPermissionsValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequirePlayerPermissionsAttribute"/> class.
    /// </summary>
    /// <param name="requiredPermissions">The player permissions required from the player.</param>
    public RequirePlayerPermissionsAttribute(PlayerPermissions requiredPermissions)
    {
        this.RequiredPlayerPermissions = requiredPermissions;
    }

    /// <summary>
    /// Gets a value containing the required permissions for a player to execute.
    /// </summary>
    [PublicAPI]
    public PlayerPermissions RequiredPlayerPermissions { get; }

    /// <inheritdoc />
    public override void CheckPermissions(ProcessPermissionsArgs args)
    {
        if (!args.IsAllowed)
        {
            return;
        }

        bool isAllowed = args.Sender.CheckPermission(this.RequiredPlayerPermissions);
        args.IsAllowed = isAllowed;
        if (isAllowed)
        {
            return;
        }

        args.Response = $"Sender is missing required player permissions.";
        if(CommandManager.ShowPermissionsBranches)
        {
            args.Response += $"[Required Player Permissions: ";
        }

        foreach (Enum enumValue in Enum.GetValues(typeof(PlayerPermissions)))
        {
            if (this.RequiredPlayerPermissions.HasFlag(enumValue))
            {
                args.Response += $"{enumValue}, ";
            }
        }

        args.Response = args.Response.Substring(0, args.Response.Length - 2) + ']';
    }
}