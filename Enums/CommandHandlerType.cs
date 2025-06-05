// -----------------------------------------------------------------------
// <copyright file="CommandHandlerType.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Enums;

using System;
using System.ComponentModel;

/// <summary>
/// Used to indicate what handlers commands should be registered to.
/// </summary>
[Flags]
public enum CommandHandlerType : byte
{
    /// <summary>
    /// Indicates that the client console [`] command handler should be used.
    /// </summary>
    [Description("Indicates that the client console [`] command handler should be used.")]
    ClientConsole = 1,

    /// <summary>
    /// Indicates that the remote admin [M] command handler should be used.
    /// </summary>
    [Description("Indicates that the remote admin [M] command handler should be used.")]
    RemoteAdmin = 2,

    /// <summary>
    /// Indicates that the server console command handler should be used.
    /// </summary>
    [Description("Indicates that the server console command handler should be used.")]
    GameConsole = 4,
}