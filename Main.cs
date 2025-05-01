using System;
using System.Collections.Generic;
using System.Reflection;
using CommandSystem;
using Exiled.API.Features;
using HarmonyLib;
using RemoteAdmin;
using SNAPI.EventHandlers;
using SNAPI.Events;
using SNAPI.Events.Handlers;
using EServer = Exiled.Events.Handlers.Server;
using WaitingForPlayers = SNAPI.EventHandlers.WaitingForPlayers;
namespace SNAPI
{
  public class Main : Plugin<Config>
  {
    public override string Author => "@Someone";
    public override string Name => "SNAPI";
    public override string Prefix => "SNAPI";
    public static Main Instance { get; private set; }
    private Harmony Harmony;
    public override void OnEnabled()
    {
      Instance = this;
      Harmony = new Harmony("SNAPI");
      Harmony.PatchAll();
      EServer.WaitingForPlayers += WaitingForPlayers.OnWaitingForPlayers;
      SnakePlayer.GameOver += _ => Log.Warn("GameOver");
      SnakePlayer.PausingSnake += _ => Log.Warn("PausingSnake");
      SnakePlayer.ResumingSnake += _ => Log.Warn("ResumingSnake");
      SnakePlayer.Score += _ => Log.Warn("Score");
      SnakePlayer.SnakeMove += ev => Log.Warn($"SnakeMove: total time: {ev.Context.TotalTimePlaying.TotalSeconds}");
      SnakePlayer.StartingNewSnake += _ => Log.Warn("StartingNewSnake");
      SnakePlayer.SwitchAxes += _ => Log.Warn("SwitchAxes");
      base.OnEnabled();
    }
    public override void OnDisabled()
    {
      base.OnDisabled();
      Harmony.UnpatchAll("SNAPI");
      EServer.WaitingForPlayers -= WaitingForPlayers.OnWaitingForPlayers;
      Instance = null;
    }
    public override void OnRegisteringCommands()
    {
      Dictionary<Type, List<ICommand>> dictionary = new();
      foreach (Type type1 in Assembly.GetTypes())
      {
        if (type1.GetInterface("ICommand") != typeof(ICommand) || !Attribute.IsDefined(type1, typeof(CommandHandlerAttribute))) continue;
        foreach (CustomAttributeData customAttributeData in type1.GetCustomAttributesData())
        {
          try
          {
            if (customAttributeData.AttributeType != typeof(CommandHandlerAttribute)) continue;
            Type type2 = (Type)customAttributeData.ConstructorArguments[0].Value;
            ICommand command1 = GetCommand(type1) ?? (ICommand)Activator.CreateInstance(type1);
            if (command1.Command == "ForceSnake" && Config.NoVSRViolatingCommand) continue;
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
                  GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(command1);
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