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
using Contexts;
using LabApi.Features.Permissions;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequirePermissionNodesAttribute : RequirePermissionsAttribute
{
    public RequirePermissionNodesAttribute(bool requireAll = true, params string[] permissions)
    {
        this.RequiredPermissionNodes = permissions;
        this.RequireAll = requireAll;
    }

    public RequirePermissionNodesAttribute(string permissions)
    {
        this.RequiredPermissionNodes = [ permissions ];
        this.RequireAll = true;
    }
    
    public string[] RequiredPermissionNodes { get; }

    public bool RequireAll { get; }

    public override void CheckPermissions(ProcessPermissionsArgs args)
    {
        if (!args.IsAllowed)
        {
            return;
        }

        bool isAllowed = this.RequireAll ? args.Sender.HasPermissions(this.RequiredPermissionNodes) : args.Sender.HasAnyPermission(this.RequiredPermissionNodes);
        args.IsAllowed = isAllowed;
        if (isAllowed)
            return;
        
        args.Response = $"Sender is missing required permission nodes.";
        if(CommandManager.ShowPermissionsBranches)
            args.Response += $"[Required Permission Nodes: {String.Join(", ", this.RequiredPermissionNodes)}]";
    }
}