// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary;

using System.ComponentModel;

using Enums;

/// <summary>
/// The main plugin configuration.
/// </summary>
public class Config
{
    /// <summary>
    /// Gets or sets the logging mode.
    /// </summary>
    [Description("Indicates the logging mode. Options: Info, Debug, Insanity, Ludicrous.")]
    public LoggingMode LoggingMode { get; set; } = LoggingMode.Info;

    /// <summary>
    /// Gets or sets a value indicating whether the plugin should be enabled.
    /// </summary>
    [Description("Indicates whether the plugin should be enabled.")]
    public bool IsEnabled { get; set; } = true;
}