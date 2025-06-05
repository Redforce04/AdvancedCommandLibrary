// -----------------------------------------------------------------------
// <copyright file="RequirePermissionsAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;

using Contexts;

/// <summary>
/// Used to indicate that a command requires some form of permissions to execute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class RequirePermissionsAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequirePermissionsAttribute"/> class.
    /// </summary>
    protected RequirePermissionsAttribute()
    {
    }

    /// <summary>
    /// Called to check the permissions of the player executing.
    /// </summary>
    /// <param name="args">The arguments passed along to check the permission context.</param>
    public abstract void CheckPermissions(ProcessPermissionsArgs args);
}