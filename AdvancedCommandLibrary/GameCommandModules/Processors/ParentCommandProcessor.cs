namespace AdvancedCommandLibrary.GameCommandModules.Processors;

using System;
using System.Linq;
using System.Reflection;
using Contexts;
using Trackers;
using CommandSystem;
using Contexts.Helpers;
using Enums;
using LabApi.Features.Console;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ConvertToAutoProperty
internal sealed class ParentCommandProcessor : ParentCommand, IUsageProvider
{
    internal ParentCommandProcessor(ParentCommandTracker command)
    {
        this.Tracker = command;
        this._command = command.Name;
        this._aliases = command.Aliases;
        this._description = command.Description;
        this._usage = command.Usages;
        try
        {
            this._subscribeMethodToEvent(ref command);
            if(command.LoadGeneratedCommandsExecutor is not null)
                this._subscribeToLoadGeneratedCommandsMethod(command.LoadGeneratedCommandsExecutor);
        }
        catch (Exception ex)
        {
            Logger.Warn($"An error occured while trying to subscribe command method to event.");
            Logger.Debug($"Exception: \n{ex}", CommandManager.DebugMode >= LoggingMode.Debug);
        }
    }
    
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    public override void LoadGeneratedCommands()
    {
        Logger.Debug($"[Parent Command] Command \"{this.Command}\" Registering {this.Tracker.ChildCommands.Count} child commands", CommandManager.DebugMode >= LoggingMode.Ludicrous);
        foreach (var cmd in this.Tracker.ChildCommands)
        {
            if (cmd is not null && cmd.GameCommandInstance is not null)
            {
                if (this.Commands.Keys.Any(x => String.Equals(x, cmd.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    Logger.Warn($"Parent command {cmd.Name} cannot register subcommand {cmd.Name} because a subcommand with that name has already been registered.");
                    continue;
                }
                if (this.CommandAliases.Values.Any(x => cmd.Aliases.Any(y => String.Equals(x, y, StringComparison.CurrentCultureIgnoreCase))))
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
                Logger.Debug($"Warning{(cmd is null ? " command is null" : "")}{(cmd?.GameCommandInstance is null ? " command game instance is null" : "")}.", CommandManager.DebugMode >= LoggingMode.Insanity);
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

    private void _subscribeMethodToEvent(ref ParentCommandTracker command)
    {
        EventInfo? eventInfo = typeof(ParentCommandProcessor).GetEvent(nameof(this.ExecuteCommand), BindingFlags.Public | BindingFlags.Instance)!;
        
        // Create the delegate on the command target class because that's where the
        // method is. This corresponds with `new EventHandler(command.Type.Method)`.
        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, command.Method);
        
        // Assign the event handler. This corresponds with `this.OnExecute += command.Type.Method`.
        eventInfo.AddEventHandler(this, handler);
    }
    
    private void _subscribeToLoadGeneratedCommandsMethod(MethodInfo executor)
    {
        EventInfo? eventInfo = typeof(ParentCommandProcessor).GetEvent(nameof(this.ExecuteLoadGeneratedCommands), BindingFlags.Public | BindingFlags.Instance)!;
        
        // Create the delegate on the command target class because that's where the
        // method is. This corresponds with `new EventHandler(command.Type.Method)`.
        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, executor);
        
        // Assign the event handler. This corresponds with `this.OnExecute += command.Type.Method`.
        eventInfo.AddEventHandler(this, handler);
    }
    public delegate void ExecuteCommandDelegateType (ParentCommandContext context);
    public delegate void ExecuteLoadGeneratedCommandsDelegateType (LoadGeneratedCommandsContext context);

    public event ExecuteCommandDelegateType? ExecuteCommand;
    public event ExecuteLoadGeneratedCommandsDelegateType? ExecuteLoadGeneratedCommands;
    internal ParentCommandTracker Tracker { get; }
    
    public override string Command => this._command;
    private string _command;

    public override string[] Aliases => this._aliases;
    private string[] _aliases; 

    public override string Description => this._description;
    private string _description;

    public string[] Usage => this._usage;
    private string[] _usage;
}