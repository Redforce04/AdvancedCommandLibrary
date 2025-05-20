// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PowerupAPI
//    Project:          PowerupAPI
//    FileName:         ICommandPermission.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/18/2025 11:23
//    Created Date:     05/18/2025 11:05
// -----------------------------------------

namespace AdvancedCommandLibrary.GameCommandModules;

using CommandSystem;

public interface ICommandPermission
{
    public string PermissionNode { get; }
    public bool CheckPermission(ICommandSender sender, out string missingPermissionString);
}