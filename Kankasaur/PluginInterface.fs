module Kankasaur.PluginInterface

open System
open Avalonia.FuncUI
open ManagerRegistry

type IPluginMsg= interface end
type IPluginState= interface end
type IPlugin = 
    abstract member Init : unit -> IPluginState
    abstract member Update : IPluginMsg -> IPluginState -> IPluginState
    abstract member View : IPluginState -> (IPluginMsg -> unit) -> Types.IView

