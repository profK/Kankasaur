module Kankasaur.TabbedShell

open System
open System.IO
open System.Reflection
open Elmish
open KankasaurPluginSupport
open KankasaurPluginSupport.SharedTypes
open Avalonia.FuncUI.DSL
open Avalonia.Controls



 

 
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
            TabControl.viewItems (
                state.plugins
                |> List.map (fun pluginRec ->
                    TabItem.create [
                        TabItem.header pluginRec.Name
                        TabItem.content (
                            pluginRec.Instance.View state pluginRec.State
                                (fun (msg: obj) ->
                                    dispatch (msg :> obj))
                        )
                    ]
                )
            )
        ]
    ]
]
         
let update (msg: obj) (state: ShellState): ShellState * Cmd<_> =

    match msg with
    | :? ShellMsg as shellMsg ->
        // Handle plugin messages
        let newState, newPlugins, msgList =
            state.plugins
            |> List.fold (fun state pluginRec ->
                let appState,  pluginRecList, msgList = state
                let appState, newState, msgOpt = pluginRec.Instance.Update shellMsg appState pluginRec.State

                let newPlugin = {pluginRec with State = newState}
                let msgList = 
                    match msgOpt with
                    | Some msg -> msg :: msgList
                    | None -> msgList
                appState, newPlugin::pluginRecList,  msgList
                ) (state, [], [ ])
        //printfn "Plugins after update %A" newPlugins
        let cmdList =
            msgList
            |> List.map(fun msg ->
                                    Cmd.ofMsg msg
                                    |> Cmd.map (fun cmd ->cmd:>obj) )
        {newState with plugins = (newPlugins |> List.rev)}, Cmd.batch cmdList
    | _ ->      
        // Handle other messages
        printfn "Unhandled message: %A" msg
        state, Cmd.none
         
        
    
         

