module Kankasaur.TabbedShell

open System
open System.Reflection

type ShellMag =
    |  PluginMsg of Kankasaur.PluginInterface.IPluginMsg
    
type PluginRecord = {
     Name : string
     Instance : Kankasaur.PluginInterface.IPlugin
 }   
    
type ShellState = { plugins : Kankasaur.PluginInterface.IPluginState list }
 
let mutable plugins = []
 
let init() =
     AppDomain.CurrentDomain().GetAssemblies()
     |> Array.iter ManagerRegistry.scanAssembly
     
     plugins <-
         (ManagerRegistry.getAllManagers<PluginInterface.IPlugin>()
         |> List.map (fun Iplugin ->
            Iplugin.GetType().
                    GetCustomAttribute<ManagerRegistry.Manager>()
            |> fun attr -> {Name = attr.Name; Instance = Iplugin}))       
         

