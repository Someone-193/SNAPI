namespace SNAPI
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using HarmonyLib;
    using RemoteAdmin;
    using SNAPI.EventHandlers;
    using SNAPI.Events.Handlers;
    using EPlayer = Exiled.Events.Handlers.Player;
    using EServer = Exiled.Events.Handlers.Server;
    using WaitingForPlayers = SNAPI.EventHandlers.WaitingForPlayers;
#if RUEI
    using RueI;
#endif

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class Main : Plugin<Config>
  {
    private Harmony harmony = null!;

    /// <summary>
    /// Gets the main plugin instance.
    /// </summary>
    public static Main Instance { get; private set; } = null!;

    /// <summary>
    /// Gets Me!.
    /// </summary>
    public override string Author => "@Someone";

    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public override string Name => "SNAPI";

    /// <summary>
    /// Gets the prefix of the plugin.
    /// </summary>
    public override string Prefix => "SNAPI";

    /// <summary>
    /// Gets the priority of the plugin.
    /// </summary>
    public override PluginPriority Priority => PluginPriority.High;

    /// <summary>
    /// Called when the plugin is enabled.
    /// </summary>
    public override void OnEnabled()
    {
      Instance = this;
      harmony = new Harmony("SNAPI");
      harmony.PatchAll();
      EServer.WaitingForPlayers += WaitingForPlayers.OnWaitingForPlayers;
      EPlayer.ChangingItem += ChangingItem.OnChangingItem;
      EPlayer.ItemRemoved += ItemRemoved.OnItemRemoved;
      if (!Config.APIMode)
      {
        SnakePlayer.StartingNewSnake += StartingNewSnake.OnStartNewSnake;
        SnakePlayer.SnakeMove += SnakeMove.OnSnakeMove;
      }

      // For debugging
      // SnakePlayer.GameOver += _ => Log.Warn("Game Over");
      // SnakePlayer.PausingSnake += _ => Log.Warn("Snake Paused");
      // SnakePlayer.ResumingSnake += _ => Log.Warn("Snake Resumed");
      // SnakePlayer.Score += _ => Log.Warn("Scored");
      // SnakePlayer.SnakeMove += _ => Log.Warn("Snake Moved");
      // SnakePlayer.StartingNewSnake += _ => Log.Warn("Snake Started");
      // SnakePlayer.SwitchAxes += _ => Log.Warn("Switch Axes");
#if RUEI
      RueIMain.EnsureInit();
#endif
      base.OnEnabled();
    }

    /// <summary>
    /// Called when the plugin is disabled.
    /// </summary>
    public override void OnDisabled()
    {
      base.OnDisabled();
      harmony.UnpatchAll("SNAPI");
      EServer.WaitingForPlayers -= WaitingForPlayers.OnWaitingForPlayers;
      EPlayer.ChangingItem -= ChangingItem.OnChangingItem;
      EPlayer.ItemRemoved -= ItemRemoved.OnItemRemoved;
      if (Config.APIMode)
            return;
      SnakePlayer.StartingNewSnake -= StartingNewSnake.OnStartNewSnake;
      SnakePlayer.SnakeMove -= SnakeMove.OnSnakeMove;
    }

    /// <summary>
    /// Called when registering commands.
    /// </summary>
    public override void OnRegisteringCommands()
    {
      Dictionary<Type, List<ICommand>> dictionary = new();
      foreach (Type type1 in Assembly.GetTypes())
      {
        if (type1.GetInterface("ICommand") != typeof(ICommand) || !Attribute.IsDefined(type1, typeof(CommandHandlerAttribute))) 
          continue;
        foreach (CustomAttributeData customAttributeData in type1.GetCustomAttributesData())
        {
          try
          {
            if (customAttributeData.AttributeType != typeof(CommandHandlerAttribute)) 
              continue;
            Type type2 = (Type)customAttributeData.ConstructorArguments[0].Value;
            ICommand command1 = GetCommand(type1) ?? (ICommand)Activator.CreateInstance(type1);
            if (command1.Command == "ForceSnake" && !Config.SoftDcCommandEnabled) 
              continue;
            if (typeof(ParentCommand).IsAssignableFrom(type2))
            {
              if (!(GetCommand(type2) is ParentCommand command2))
              {
                if (!dictionary.TryGetValue(type2, out List<ICommand> commandList))
                  dictionary.Add(type2, [command1]);
                else
                  commandList.Add(command1);
              }
              else
                command2.RegisterCommand(command1);
            }
            else
            {
              try
              {
                if (type2 == typeof(RemoteAdminCommandHandler))
                  CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command1);
                else if (type2 == typeof(GameConsoleCommandHandler))
                  GameCore.Console.ConsoleCommandHandler.RegisterCommand(command1);
                else if (type2 == typeof(ClientCommandHandler))
                  QueryProcessor.DotCommandHandler.RegisterCommand(command1);
              }
              catch (ArgumentException ex)
              {
                if (ex.Message.StartsWith("An"))
                  Log.Error("Command with same name has already registered! Command: " + command1.Command);
                else
                  Log.Error($"An error has occurred while registering a command: {ex}");
              }
              
              Commands[type2][type1] = command1;
            }
          }
          catch (Exception ex)
          {
            Log.Error($"An error has occurred while registering a command: {ex}");
          }
        }
      }
    }
  }
}