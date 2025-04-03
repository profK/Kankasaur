﻿module Kankasaur.TabbedShell

open System
open System.Reflection
open Avalonia.Controls
open Avalonia.FuncUI.DSL


type ShellMag =
    |  PluginMsg of Kankasaur.PluginInterface.IPluginMsg
    
type PluginRecord = {
     Name : string
     Instance : Kankasaur.PluginInterface.IPlugin
     State: Kankasaur.PluginInterface.IPluginState
 }
  
    
type ShellState = { plugins : PluginRecord list }
 
let mutable plugins = []
 
let init() =
     AppDomain.CurrentDomain.GetAssemblies()
     |> Array.iter ManagerRegistry.scanAssembly
     
     ManagerRegistry.getAllManagers<PluginInterface.IPlugin>()
     |> List.map (fun Iplugin ->
            Iplugin.GetType().
                    GetCustomAttribute<ManagerRegistry.Manager>()
            |> fun attr -> {
                                        Name = attr.Name
                                        Instance = Iplugin
                                        State = Iplugin.Init()
                                    }
         )
     |> fun pluginRecs -> {plugins = pluginRecs}
     
let update (msg:ShellMag) (state:ShellState) =
     match msg with
     | PluginMsg pluginMsg ->
          let newPlugins =
              plugins
              |> List.map (fun pluginRec ->
                    let newState = pluginRec.Instance.Update pluginMsg pluginRec.State
                    {pluginRec with State = newState}
                )
          {state with plugins = newPlugins}
     | _ -> state
     
let view (state:ShellState) (dispatch: obj -> unit) =
         DockPanel.create [
            DockPanel.children [
                TabControl.create [
                    TabControl.dock Dock.Top
                    TabControl.viewItems [
                        for pluginRec in state.plugins do
                            TabItem.create [
                                TabItem.header pluginRec.Name
                                TabItem.content (
                                    pluginRec.Instance.View pluginRec.State (fun msg ->
                                        // dispatch (PluginMsg msg //TODO
                                        () ))
                            ]
                    ]
                ]
            ]
        ]
    
         

