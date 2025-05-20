// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         Config.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 20:51
//    Created Date:     05/19/2025 20:05
// -----------------------------------------

namespace AdvancedCommandLibrary;

using Enums;

public class Config
{
    public LoggingMode LoggingMode { get; set; } = LoggingMode.Info;

    public bool IsEnabled { get; set; } = true;
}