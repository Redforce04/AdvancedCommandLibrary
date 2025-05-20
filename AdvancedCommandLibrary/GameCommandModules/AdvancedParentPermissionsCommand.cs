// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PowerupAPI
//    Project:          PowerupAPI
//    FileName:         ParentCommand.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/18/2025 11:17
//    Created Date:     05/18/2025 11:05
// -----------------------------------------

namespace AdvancedCommandLibrary.GameCommandModules;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using LabApi.Features.Permissions;

public abstract class AdvancedParentPermissionsCommand : AdvancedParentCommand, ICommandPermission
{
    public abstract string PermissionNode { get; }
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

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!this.CheckPermission(sender, out response))
            return false;
        return base.ExecuteParent(arguments, sender, out response);
    }
}