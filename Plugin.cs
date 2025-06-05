// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary;

using System;

using LabApi.Features;
using LabApi.Loader.Features.Plugins.Enums;
using MEC;

/// <summary>
/// The main plugin class.
/// </summary>
public class Plugin : LabApi.Loader.Features.Plugins.Plugin<Config>
{
    /// <inheritdoc/>
    public override string Name => "AdvancedCommandLibrary";

    /// <inheritdoc/>
    public override string Description => "Provides an advanced yet easy system to create and load commands.";

    /// <inheritdoc/>
    public override string Author => "Redforce04";

    /// <inheritdoc/>
    public override Version Version => new (1, 0, 0);

    /// <inheritdoc/>
    public override LoadPriority Priority => LoadPriority.Lowest;

    /// <inheritdoc/>
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

    /// <inheritdoc/>
    public override void Enable()
    {
        if(Config is not null)
        {
            CommandManager.DebugMode = Config.LoggingMode;
        }

        Timing.CallDelayed(1f, CommandManager.LoadCommandManager);
    }

    /// <inheritdoc/>
    public override void Disable()
    {
        CommandManager.UnloadCommandManager();
    }
}