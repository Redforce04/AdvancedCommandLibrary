// -----------------------------------------------------------------------
// <copyright file="ChildCommandProcessor.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.GameCommandModules.Processors;

using System;
using System.Reflection;

using CommandSystem;
using Contexts;
using Enums;
using LabApi.Features.Console;
using Trackers;

/// <summary>
/// The game implementation for processing child commands.
/// </summary>
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ConvertToAutoProperty
internal sealed class ChildCommandProcessor : ICommand, IUsageProvider
{
    // ReSharper disable InconsistentNaming
    private string command;
    private string[] aliases;
    private string description;
    private string[] usage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChildCommandProcessor"/> class.
    /// </summary>
    /// <param name="command">The command tracker representing this command.</param>
    internal ChildCommandProcessor(CommandTracker command)
    {
        this.Tracker = command;
        this.command = command.Name;
        this.aliases = command.Aliases;
        this.description = command.Description;
        this.usage = command.Usages;
        try
        {
            this.SubscribeMethodToEvent(ref command);
        }
        catch (Exception ex)
        {
            Logger.Warn($"An error occured while trying to subscribe command method to event.");
            Logger.Debug($"Exception: \n{ex}", CommandManager.DebugMode >= LoggingMode.Debug);
        }
    }

    /// <summary>
    /// The delegate used to call <see cref="ChildCommandProcessor.ExecuteCommand"/>.
    /// </summary>
    /// <param name="context">The context to use to process the command.</param>
    public delegate void ExecuteCommandDelegateType(CommandContext context);

    /// <summary>
    /// The event called when the command is executing.
    /// </summary>
    public event ExecuteCommandDelegateType? ExecuteCommand;

    /// <summary>
    /// Gets the name of this command.
    /// </summary>
    public string Command => this.command;

    /// <summary>
    /// Gets the aliases for this command.
    /// </summary>
    public string[] Aliases => this.aliases;

    /// <summary>
    /// Gets the description for this command.
    /// </summary>
    public string Description => this.description;

    /// <summary>
    /// Gets the usages of this command.
    /// </summary>
    public string[] Usage => this.usage;

    /// <summary>
    /// Gets the tracker instance representing this command.
    /// </summary>
    internal CommandTracker Tracker { get; }

    /// <summary>
    /// Called when the command is executed.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="response">The response out.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
    {
        response = "An error occured while executing the command.";
        if (this.ExecuteCommand is null)
        {
            Logger.Warn($"Cannot call command {this.Command} due to an error.");
            Logger.Debug($"Command event handler is null when it shouldn't be", CommandManager.DebugMode >= LoggingMode.Debug);
            return false;
        }

        CommandContext ev = new(args, sender, response, this, this.Tracker.ParentTrackerInstance?.GameCommandInstance as ParentCommand, this.Tracker.PermissionsRequirementTracker);
        try
        {
            this.ExecuteCommand.Invoke(ev);
        }
        catch (Exception e)
        {
            Logger.Error($"An error occured trying to process command {this.Command}.");
            Logger.Debug($"Error: \n{e}");
            response = "An error occured while executing the command.";
            return false;
        }

        response = ev.Response;
        return ev.Allowed;
    }

    private void SubscribeMethodToEvent(ref CommandTracker cmd)
    {
        EventInfo? eventInfo = typeof(ChildCommandProcessor).GetEvent(nameof(this.ExecuteCommand), BindingFlags.Public | BindingFlags.Instance);

        if (eventInfo is null)
        {
            Logger.Error($"Event was null.");
            return;
        }

        // Create the delegate on the command target class because that's where the
        // method is. This corresponds with `new EventHandler(command.Type.Method)`.
        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, cmd.Method);

        // Assign the event handler. This corresponds with `this.OnExecute += command.Type.Method`.
        eventInfo.AddEventHandler(this, handler);
    }
}