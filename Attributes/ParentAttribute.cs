// -----------------------------------------------------------------------
// <copyright file="ParentAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;

/// <summary>
/// Indicates the parent command of a command.
/// </summary>
/// <param name="parentBaseType">The type declaring the parent command.</param>
/// <param name="parentName">The name of the parent commands method.</param>
// Instead of parsing here, I want to parse this in the loader so it is safer.
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ParentAttribute(Type parentBaseType, string parentName) : Attribute
{
    /// <summary>
    /// Gets the type declaring the parent command.
    /// </summary>
    public Type ParentBaseType { get; } = parentBaseType;

    /// <summary>
    /// Gets the name of the method of the parent command.
    /// </summary>
    public string ParentName { get; } = parentName;
}