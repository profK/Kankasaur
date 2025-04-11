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
  
    
type ShellState = { plugins : PluginRecord list
                    sharedValues: Map<string, obj>}
 
//let mutable plugins = []
let combineValueMaps (map1: Map<string, obj>) (map2: Map<string, obj>) =
    // Combine two maps by adding the values of the same keys
    // If a key exists in both maps, the value from map2 will be used
    // If a key exists only in one map, it will be included in the result
    // This is a simple merge function that does not handle conflicts
    // You can modify this logic as per your requirements
    Map.fold (fun acc key value -> Map.add key value acc) map1 map2
let init: ShellState*Cmd<obj> =
     // Scan the current assembly for plugins
     AppDomain.CurrentDomain.GetAssemblies()
     |> Array.iter ManagerRegistry.scanAssembly
     
     Directory.GetFiles(".", "*.dll")
     |> Array.iter (fun file ->
         Assembly.LoadFrom(file)
         |> ManagerRegistry.scanAssembly
     )
     
     
     ManagerRegistry.getAllManagers<PluginInterface.IPlugin>()
     |> List.map (fun Iplugin ->
            Iplugin.GetType().
                    GetCustomAttribute<ManagerRegistry.Manager>()
            |> fun attr ->
                    let initTuple: (IPluginState * Map<string,obj>) = Iplugin.Init()
                    (
                        {
                            Name = attr.Name
                            Instance = Iplugin
                            State = fst initTuple
                        }, snd initTuple
                    )
         )
     |> List.unzip
     |> fun (pluginRecs, initValues) ->
             let valueMap =
                 initValues
                 |> List.fold combineValueMaps Map.empty
             { plugins = pluginRecs
               sharedValues= valueMap}, Cmd.none
     
let update (sysmsg: obj) (state: ShellState): ShellState * Cmd<_> =
     sysmsg :?> ShellMsg
     |> function
         | PluginMsg pluginMsg ->           
              state.plugins
              |> List.map (fun pluginRec ->
                    let stateAndMap = pluginRec.Instance.Update pluginMsg pluginRec.State state.sharedValues
                    {pluginRec with State = fst stateAndMap}, snd stateAndMap
                )
              |> List.unzip
              |> fun (newPluginRecs, newValues) ->
                  let valueMap =
                      newValues
                      |> List.fold combineValueMaps Map.empty
                  { state with plugins = newPluginRecs; sharedValues = valueMap }, Cmd.none
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
                                    pluginRec.Instance.View pluginRec.State
                                            (fun (msg:IPluginMsg) ->
                                                    dispatch ((ShellMsg.PluginMsg msg):>obj))
                                         
                                )
                            ] 
                    ]
                ]
            ]
        ]
    
         

