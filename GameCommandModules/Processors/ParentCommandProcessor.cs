// -----------------------------------------------------------------------
// <copyright file="ParentCommandProcessor.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.GameCommandModules.Processors;

using System;
using System.Linq;
using System.Reflection;

using CommandSystem;
using Contexts;
using Contexts.Helpers;
using Enums;
using LabApi.Features.Console;
using Trackers;

/// <summary>
/// The game implementation for processing parent commands.
/// </summary>
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ConvertToAutoProperty
internal sealed class ParentCommandProcessor : ParentCommand, IUsageProvider
{
    // ReSharper disable InconsistentNaming
    private string command;
    private string[] aliases;
    private string description;
    private string[] usage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParentCommandProcessor"/> class.
    /// </summary>
    /// <param name="command">The parent command tracker that represents this command.</param>
    internal ParentCommandProcessor(ParentCommandTracker command)
    {
        this.Tracker = command;
        this.command = command.Name;
        this.aliases = command.Aliases;
        this.description = command.Description;
        this.usage = command.Usages;
        try
        {
            this.SubscribeMethodToEvent(ref command);
            if(command.LoadGeneratedCommandsExecutor is not null)
            {
                this.SubscribeToLoadGeneratedCommandsMethod(command.LoadGeneratedCommandsExecutor);
            }
        }
        catch (Exception ex)
        {
            Logger.Warn($"An error occured while trying to subscribe command method to event.");
            Logger.Debug($"Exception: \n{ex}", CommandManager.DebugMode >= LoggingMode.Debug);
        }
    }

    /// <summary>
    /// The delegate used to call <see cref="ParentCommandProcessor.ExecuteCommand"/>.
    /// </summary>
    /// <param name="context">The context to use to process the command.</param>
    public delegate void ExecuteCommandDelegateType(ParentCommandContext context);

    /// <summary>
    /// The delegate used to call <see cref="ParentCommandProcessor.ExecuteLoadGeneratedCommands"/>.
    /// </summary>
    /// <param name="context">The context to use to process the loaded commands.</param>
    public delegate void ExecuteLoadGeneratedCommandsDelegateType(LoadGeneratedCommandsContext context);

    /// <summary>
    /// The event called when the command is executing.
    /// </summary>
    public event ExecuteCommandDelegateType? ExecuteCommand;

    /// <summary>
    /// The event called when the command is loading generated commands.
    /// </summary>
    public event ExecuteLoadGeneratedCommandsDelegateType? ExecuteLoadGeneratedCommands;

    /// <summary>
    /// Gets the name of this command.
    /// </summary>
    public override string Command => this.command;

    /// <summary>
    /// Gets the aliases for this command.
    /// </summary>
    public override string[] Aliases => this.aliases;

    /// <summary>
    /// Gets the description for this command.
    /// </summary>
    public override string Description => this.description;

    /// <summary>
    /// Gets the usages of this command.
    /// </summary>
    public string[] Usage => this.usage;

    /// <summary>
    /// Gets the tracker instance representing this command.
    /// </summary>
    internal ParentCommandTracker Tracker { get; }

    /// <summary>
    /// Called when loading generated commands.
    /// </summary>
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    public override void LoadGeneratedCommands()
    {
        Logger.Debug($"[Parent Command] Command \"{this.Command}\" Registering {this.Tracker.ChildCommands.Count} child commands", CommandManager.DebugMode >= LoggingMode.Ludicrous);
        foreach (CommandTracker? cmd in this.Tracker.ChildCommands)
        {
            if (cmd?.GameCommandInstance != null)
            {
                if (this.Commands.Keys.Any(x => string.Equals(x, cmd.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    Logger.Warn($"Parent command {cmd.Name} cannot register subcommand {cmd.Name} because a subcommand with that name has already been registered.");
                    continue;
                }

                if (this.CommandAliases.Values.Any(x => cmd.Aliases.Any(y => string.Equals(x, y, StringComparison.CurrentCultureIgnoreCase))))
                {
                    Logger.Warn($"Parent command {cmd.Name} cannot register subcommand {cmd.Name} because an alias of the command has already been registered.");
                    continue;
                }

                try
                {
                    RegisterCommand(cmd.GameCommandInstance);
                }
                catch (Exception ex)
                {
                    Logger.Warn($"An error has occured while registering children for command {cmd.Name}.");
                    Logger.Debug($"Exception: \n{ex}", CommandManager.DebugMode >= LoggingMode.Insanity);
                }
            }
            else
            {
                Logger.Debug($"Warning{(cmd is null ? " command is null" : string.Empty)}{(cmd?.GameCommandInstance is null ? " command game instance is null" : string.Empty)}.", CommandManager.DebugMode >= LoggingMode.Insanity);
            }
        }

        if (this.ExecuteLoadGeneratedCommands is null)
        {
            // Logger.Warn($"Cannot execute load generated commands for command {this.Command} because it is likely not yet implemented.");
            // Logger.Debug($"Executor for load generated commands is not implemented yet.", CommandManager.DebugMode >= LoggingMode.Debug);
            return;
        }

        LoadGeneratedCommandsContext ev = new(this, this.Tracker.ParentTrackerInstance?.GameCommandInstance as ParentCommand);
        try
        {
            this.ExecuteLoadGeneratedCommands.Invoke(ev);
        }
        catch (Exception e)
        {
            Logger.Error($"An error occured trying to process command {this.Command}.");
            Logger.Debug($"Error: \n{e}");
        }
    }

    /// <summary>
    /// Executed when the command is called without a subcommand.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="response">The response out.</param>
    /// <returns>True if successful, false otherwise.</returns>
    protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
    {
        response = "An error occured while executing the command.";
        if (this.ExecuteCommand is null)
        {
            Logger.Warn($"Cannot call command {this.Command} due to an error.");
            Logger.Debug($"Command event handler is null when it shouldn't be", CommandManager.DebugMode >= LoggingMode.Debug);
            return false;
        }

        ParentCommandContext ev = new(args, sender, response, this, this.Tracker.ParentTrackerInstance?.GameCommandInstance as ParentCommand, this.Tracker.PermissionsRequirementTracker);
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

    private void SubscribeMethodToEvent(ref ParentCommandTracker cmd)
    {
        EventInfo eventInfo = typeof(ParentCommandProcessor).GetEvent(nameof(this.ExecuteCommand), BindingFlags.Public | BindingFlags.Instance)!;

        // Create the delegate on the command target class because that's where the
        // method is. This corresponds with `new EventHandler(command.Type.Method)`.
        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, cmd.Method);

        // Assign the event handler. This corresponds with `this.OnExecute += command.Type.Method`.
        eventInfo.AddEventHandler(this, handler);
    }

    private void SubscribeToLoadGeneratedCommandsMethod(MethodInfo executor)
    {
        EventInfo eventInfo = typeof(ParentCommandProcessor).GetEvent(nameof(this.ExecuteLoadGeneratedCommands), BindingFlags.Public | BindingFlags.Instance)!;

        // Create the delegate on the command target class because that's where the
        // method is. This corresponds with `new EventHandler(command.Type.Method)`.
        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, executor);

        // Assign the event handler. This corresponds with `this.OnExecute += command.Type.Method`.
        eventInfo.AddEventHandler(this, handler);
    }
}