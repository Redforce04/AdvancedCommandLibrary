// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         VirtualizedCommand.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 16:37
//    Created Date:     05/19/2025 16:05
// -----------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionsAttribute : Attribute
{
    public RequirePermissionsAttribute(params string[] permissionNodes)
    {
        this.RequiredPermissionNodes = permissionNodes;
    }

    public RequirePermissionsAttribute(PlayerPermissions permission)
    {
        this.RequiredPlayerPermissions = permission;
    }
    
    public RequirePermissionsAttribute(PlayerPermissions permission, params string[] permissionNodes)
    {
        this.RequiredPlayerPermissions = permission;
        this.RequiredPermissionNodes = permissionNodes;
    }
    
    public string[]? RequiredPermissionNodes { get; }
    public PlayerPermissions? RequiredPlayerPermissions { get; }
}