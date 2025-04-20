module Kankasaur.TabbedShell

open System
open System.IO
open System.Reflection
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Elmish
open KankasaurPluginSupport
open KankasaurPluginSupport.SharedTypes


 

 
let init: ShellState*Cmd<obj> =
     // Scan the current assembly for plugins
     AppDomain.CurrentDomain.GetAssemblies()
     |> Array.iter ManagerRegistry.scanAssembly
 
     Directory.GetFiles(".", "*.dll")
     |> Array.iter (fun file ->
         Assembly.LoadFrom(file)
         |> ManagerRegistry.scanAssembly )
 
     let  appState = {plugins = []; campaignID = None; mapID = None}
     ManagerRegistry.getAllManagers<IPlugin>()
     |> List.sortBy (
            fun Iplugin ->
                Iplugin.GetType()
                    .GetCustomAttribute<OrderAttribute>()
                |> Option.ofObj
                |> function
                    |None -> 0
                    | Some v -> v.Order
                
 
         )
     |> List.map (fun Iplugin ->
            Iplugin.GetType().
                    GetCustomAttribute<ManagerRegistry.Manager>()
            |> fun attr ->
                let pluginState = Iplugin.Init appState
                {
                    Name = attr.Name
                    Instance = Iplugin
                    State = snd pluginState
                }
         )
     |> fun pluginRecs ->
         //plugins <- pluginRecs
         {   plugins = pluginRecs
             campaignID = None
             mapID = None}, Cmd.none
     

     
let view (state: ShellState) (dispatch) =
         DockPanel.create [
            DockPanel.children [
                TabControl.create [
                    TabControl.dock Dock.Top
                    TabControl.viewItems [
                        for pluginRec in state.plugins do
                            TabItem.create [
                                TabItem.header pluginRec.Name
                                TabItem.content (
                                    pluginRec.Instance.View state pluginRec.State
                                            (fun (msg:IPluginMsg) ->
                                                    dispatch ((ShellMsg.PluginMsg msg):>obj))
                                         
                                )
                            ] 
                    ]
                ]
            ]
        ]
         
let update (sysmsg: obj) (state: ShellState): ShellState * Cmd<_> =
     sysmsg :?> ShellMsg
     |> function
         | PluginMsg pluginMsg ->
              let newState,newPlugins =
                  state.plugins
                  |> List.fold (fun state pluginRec ->
                        let appState = fst state :> IAppState
                        let pluginRecLst= snd state :> PluginRecord list
                        let appState, newState = pluginRec.Instance.Update pluginMsg appState pluginRec.State
                        let newlist = snd state
                        let newPlugin = {pluginRec with State = newState}
                        appState, newPlugin::pluginRecLst
                        ) (state, [])
                  
              {(newState :?> ShellState) with plugins = newPlugins}, Cmd.none
         
         | _ -> state, Cmd.none
    
         

