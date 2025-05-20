using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using LabApi.Features.Permissions;

namespace AdvancedCommandLibrary.GameCommandModules;

public abstract class AdvancedPermissionCommand : ICommand, IUsageProvider, ICommandPermission
{

    public bool CheckPermission(ICommandSender sender, out string missingPermissionString)
    {
        if(sender.HasPermissions(this.PermissionNode))
        {
            missingPermissionString = $"Permission Approved [Permission Branch: {this.PermissionNode}]";
            return true;
        }
        missingPermissionString = $"You do not have permission to use this command. [Permission Branch: {this.PermissionNode}]";
        return false;
    }


    public virtual bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        bool result = this.CheckPermission(sender, out string permissionString);
        response = permissionString;
        return result;
    }
    
    public abstract string PermissionNode { get; }

    public abstract string Command { get; }

    public abstract string[] Aliases { get; }

    public abstract string Description { get; }

    public abstract string[] Usage { get; }
}