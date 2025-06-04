// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         ProcessPermissionsArgs.cs
//    Author:           Redforce04#4091
//    Revision Date:    06/04/2025 14:25
//    Created Date:     06/04/2025 14:06
// -----------------------------------------

namespace AdvancedCommandLibrary.Contexts;

using System;
using CommandSystem;

public class ProcessPermissionsArgs(ICommandSender sender)
{
    public ICommandSender Sender { get; } = sender;

    public bool IsAllowed { get; set; } = true;
    
    public string Response { get; set; } = string.Empty;
}