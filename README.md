# Advanced Command Library
A command library that makes SCP-SL Commands cleaner, simpler, and easier.

### Usage
Look at the testing directory for examples of how to use.
```csharp
    [ParentCommand("nestedBase", "The Second base command for the plugin", [], [])]
    public static void BaseCommand(ParentCommandContext context)
    {
        if(!context.CheckPermissions())
        {
            return;
        }

        context.RespondWithSubCommands();
    }
```