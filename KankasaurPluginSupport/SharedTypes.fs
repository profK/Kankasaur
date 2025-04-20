namespace KankasaurPluginSupport.SharedTypes

open Avalonia.FuncUI

type IPluginMsg= interface end
type IPluginState= interface end
type IAppState = interface end

type IPlugin = 
    abstract member Init : IAppState-> IAppState * IPluginState
    abstract member Update : IPluginMsg -> IAppState -> IPluginState->
        IAppState * IPluginState
    abstract member View : IAppState->IPluginState -> (IPluginMsg -> unit) -> Types.IView

type ShellMsg =
    |  PluginMsg of IPluginMsg

    
type PluginRecord = {
     Name : string
     Instance : IPlugin
     State: IPluginState
 }  
type ShellState = {
     plugins : PluginRecord list
     campaignID : int
     mapID : int } with interface IAppState
