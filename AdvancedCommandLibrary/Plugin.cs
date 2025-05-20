namespace AdvancedCommandLibrary
{
    using System;
    using LabApi.Features;
    using LabApi.Loader.Features.Plugins;
    using LabApi.Loader.Features.Plugins.Enums;
    using MEC;
    using Trackers;

    public class AdvancedCommandLibraryPlugin : Plugin<Config>
    {
        public override string Name => "AdvancedCommandLibrary";
        public override string Description => "Provides an advanced yet easy system to create and load commands.";
        public override string Author => "Redforce04";
        public override Version Version => new (1, 0, 0);

        public override LoadPriority Priority => LoadPriority.Lowest;

        public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;
        
        public override void Enable()
        {
            if(Config is not null)
                CommandManager.DebugMode = Config.LoggingMode;
            Timing.CallDelayed(1f, () =>
            {
                CommandManager.LoadCommandManager();
            });
        }

        public override void Disable()
        {
            CommandManager.UnloadCommandManager();
        }
    }
}