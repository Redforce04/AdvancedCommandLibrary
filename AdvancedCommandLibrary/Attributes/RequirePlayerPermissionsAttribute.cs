// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         RequirePlayerPermissionsAttribute.cs
//    Author:           Redforce04#4091
//    Revision Date:    06/04/2025 14:31
//    Created Date:     06/04/2025 14:06
// -----------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;
using System.Linq;
using Contexts;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequirePlayerPermissionsAttribute : RequirePermissionsAttribute
{
    public RequirePlayerPermissionsAttribute(params PlayerPermissions[] requiredPermissions)
    {
        ulong requiredPermissionsValue = requiredPermissions.Aggregate<PlayerPermissions, ulong>(0, (current, requiredPermission) => current + (ulong)requiredPermission);

        this.RequiredPlayerPermissions = (PlayerPermissions)requiredPermissionsValue;
    }

    public RequirePlayerPermissionsAttribute(PlayerPermissions requiredPermissions)
    {
        this.RequiredPlayerPermissions = requiredPermissions;
    }

    public PlayerPermissions RequiredPlayerPermissions { get; }
    public override void CheckPermissions(ProcessPermissionsArgs args)
    {
        if (!args.IsAllowed)
        {
            return;
        }

        bool isAllowed = args.Sender.CheckPermission(this.RequiredPlayerPermissions);
        args.IsAllowed = isAllowed;
        if (isAllowed)
            return;
        
        args.Response = $"Sender is missing required player permissions.";
        if(CommandManager.ShowPermissionsBranches)
            args.Response += $"[Required Player Permissions: ";
        foreach (Enum enumValue in Enum.GetValues(typeof(PlayerPermissions)))
        {
            if (this.RequiredPlayerPermissions.HasFlag(enumValue))
                args.Response += $"{enumValue}, ";
        }
        args.Response = args.Response.Substring(0, args.Response.Length - 2) + ']';
    }
    
}