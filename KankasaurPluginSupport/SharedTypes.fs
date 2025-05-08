namespace KankasaurPluginSupport.SharedTypes

open Elmish
open Avalonia.FuncUI




type IPluginMsg = interface end
type IPluginState= interface end



type ShellMsg =
    |  PluginMsg of IPluginMsg
    | CampaignSelected of int
    | MapSelected of int
    
type PluginRecord = {
     Name : string
     Instance : IPlugin
     State: IPluginState
 }
    
and ShellState = {
         plugins : PluginRecord list
         campaignID : int option
         mapID : int option}
and  IPlugin = 
        abstract member Init : ShellState-> ShellState* IPluginState
        abstract member Update : ShellMsg -> ShellState -> IPluginState->
            ShellState * IPluginState *ShellMsg option
        abstract member View : ShellState->IPluginState -> (obj -> unit) -> Types.IView

    
   

