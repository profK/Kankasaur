module Kankasaur.PluginInterface

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout

type IPluginMsg= interface end
type IPluginState= interface end
type IAppState = interface end
type IPlugin = 
    abstract member Init: unit-> IPluginState
    abstract member Update : IPluginMsg -> IAppState -> IPluginState->
        IAppState * IPluginState
    abstract member View : IAppState->IPluginState -> (IPluginMsg -> unit) -> Types.IView

