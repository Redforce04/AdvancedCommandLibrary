// -----------------------------------------------------------------------
// <copyright file="LoggingMode.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Enums;

using System.ComponentModel;

/// <summary>
/// Indicates the logging mode to use.
/// </summary>
public enum LoggingMode
{
    /// <summary>
    /// Only info logs will be shown.
    /// </summary>
    [Description("Only info logs will be shown.")]
    Info = 1,

    /// <summary>
    /// Only info and basic debugging logs will be shown.
    /// </summary>
    [Description("Only info and basic debugging logs will be shown.")]
    Debug = 2,

    /// <summary>
    /// Info, basic debugging, and verbose bugging will be shown.
    /// </summary>
    [Description("Info, basic debugging, and verbose bugging will be shown.")]
    Insanity = 3,

    /// <summary>
    /// Info, basic debugging, verbose bugging, traces, and situational context will be shown.
    /// </summary>
    [Description("Info, basic debugging, verbose bugging, traces, and situational context will be shown.")]
    Ludicrous = 4,
}