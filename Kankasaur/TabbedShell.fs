module Kankasaur.TabbedShell

open System
open System.IO
open System.Reflection
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Elmish
open Kankasaur.PluginInterface
open System



type ShellMsg =
    |  PluginMsg of Kankasaur.PluginInterface.IPluginMsg
    
type PluginRecord = {
     Name : string
     Instance : Kankasaur.PluginInterface.IPlugin
     State: Kankasaur.PluginInterface.IPluginState
 }  
type ShellState =
    { plugins : PluginRecord list }
    interface IAppState
 
 // Must Inject Appstate
 
let init: ShellState*Cmd<obj> =
     // Scan the current assembly for plugins
     AppDomain.CurrentDomain.GetAssemblies()
     |> Array.iter ManagerRegistry.scanAssembly
 
     Directory.GetFiles(".", "*.dll")
     |> Array.iter (fun file ->
         Assembly.LoadFrom(file)
         |> ManagerRegistry.scanAssembly )
 
 
     ManagerRegistry.getAllManagers<PluginInterface.IPlugin>()
     |> List.map (fun Iplugin ->
            Iplugin.GetType().
                    GetCustomAttribute<ManagerRegistry.Manager>()
            |> fun attr ->
                let pluginState = Iplugin.Init ()
                {
                    Name = attr.Name
                    Instance = Iplugin
                    State = pluginState
                }
         )
     |> fun pluginRecs ->
         //plugins <- pluginRecs
         {plugins = pluginRecs}, Cmd.none
     
let update (sysmsg: obj) (state: ShellState): ShellState * Cmd<_> =
     sysmsg :?> ShellMsg
     |> function
         | PluginMsg pluginMsg ->
              let newPlugins =
                  state.plugins
                  |> List.map (fun pluginRec ->
                        let appState, newState = pluginRec.Instance.Update pluginMsg state pluginRec.State
                        {pluginRec with State = newState}
                    )
              {state with plugins = newPlugins}, Cmd.none
         | _ -> state, Cmd.none
     
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
    
         

