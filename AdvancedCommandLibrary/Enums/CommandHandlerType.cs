// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         CommandHandlerTypes.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 16:40
//    Created Date:     05/19/2025 16:05
// -----------------------------------------

namespace AdvancedCommandLibrary;

using System;
using CommandSystem;

[Flags]
public enum CommandHandlerType : byte
{
    ClientConsole = 1,
    RemoteAdmin = 2,
    GameConsole = 4,
}