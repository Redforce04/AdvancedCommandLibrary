namespace AdvancedCommandLibrary.GameCommandModules.Processors;

using System;
using System.Reflection;
using Contexts;
using Trackers;
using CommandSystem;
using Enums;
using LabApi.Features.Console;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ConvertToAutoProperty
internal sealed class ChildCommandProcessor : ICommand, IUsageProvider
{
    internal ChildCommandProcessor(CommandTracker command)
    {
        this.Tracker = command;
        this._command = command.Name;
        this._aliases = command.Aliases;
        this._description = command.Description;
        this._usage = command.Usages;
        try
        {
            this._subscribeMethodToEvent(ref command);
        }
        catch (Exception ex)
        {
            Logger.Warn($"An error occured while trying to subscribe command method to event.");
            Logger.Debug($"Exception: \n{ex}", CommandManager.DebugMode >= LoggingMode.Debug);
        }
    }
    
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

    private void _subscribeMethodToEvent(ref CommandTracker command)
    {
        EventInfo? eventInfo = typeof(ChildCommandProcessor).GetEvent(nameof(this.ExecuteCommand), BindingFlags.Public | BindingFlags.Instance);
        
        // Create the delegate on the command target class because that's where the
        // method is. This corresponds with `new EventHandler(command.Type.Method)`.
        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, command.Method);

        // Assign the event handler. This corresponds with `this.OnExecute += command.Type.Method`.
        eventInfo.AddEventHandler(this, handler);
    }

    public delegate void ExecuteCommandDelegateType (CommandContext context);
    public event ExecuteCommandDelegateType? ExecuteCommand;
    internal CommandTracker Tracker { get; }
    
    public string Command => this._command;
    private string _command;

    public string[] Aliases => this._aliases;
    private string[] _aliases; 

    public string Description => this._description;
    private string _description;

    public string[] Usage => this._usage;
    private string[] _usage; 
}