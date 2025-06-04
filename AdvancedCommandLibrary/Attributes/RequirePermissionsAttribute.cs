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
using Contexts;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequirePermissionsAttribute : Attribute
{
    protected RequirePermissionsAttribute()
    {
    }

    public virtual void CheckPermissions(ProcessPermissionsArgs args)
    {
    }
}