// -----------------------------------------------------------------------
// <copyright file="ParentCommandContext.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Contexts;

using System;
using System.Linq;
using System.Text;

using CommandSystem;
using Enums;
using GameCommandModules.Processors;
using Helpers;
using LabApi.Features.Console;
using Trackers;

/// <summary>
/// The primary context to respond to a parent command.
/// </summary>
public class ParentCommandContext : CommandContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParentCommandContext"/> class.
    /// </summary>
    /// <param name="args">The arguments provided by the sender.</param>
    /// <param name="sender">The CommandSender who sent the message.</param>
    /// <param name="response">The default response to show.</param>
    /// <param name="command">The command instance that is being executed.</param>
    /// <param name="parentCommand">The parent command of the command instance (if applicable).</param>
    /// <param name="permissionsTracker">A permission tracker if the command has a permission tracker.</param>
    internal ParentCommandContext(ArraySegment<string> args, ICommandSender sender, string response, ParentCommand command, ParentCommand? parentCommand = null, PermissionsTracker? permissionsTracker = null)
        : base(args, sender, response, command, parentCommand, permissionsTracker)
    {
    }

    /// <summary>
    /// Responds to the player with a list of all allowed sub-commands. A description and arguments will be shown.
    /// </summary>
    public void RespondWithSubCommands()
    {
        if (this.CommandInstance is not ParentCommandProcessor cmd)
        {
            return;
        }

        Logger.Debug($"Command \"{cmd.Command}\" children: {cmd.AllCommands.Count()}", CommandManager.DebugMode >= LoggingMode.Ludicrous);
        StringBuilder builder = new ();
        builder.AppendLine($"Command Usage:");
        CommandTrackerTreeSearcher.SearchResult searchResult = CommandTrackerTreeSearcher.RecursivelySearchCommand(cmd.Tracker, 0);
        CommandTrackerTreeSearcher.RecursivelyBuildString(ref builder, searchResult, 0);
        this.Response = builder.ToString();
    }
}