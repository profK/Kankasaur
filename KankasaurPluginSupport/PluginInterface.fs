module Kankasaur.PluginInterface

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout

type IPluginMsg= interface end
type IPluginState= interface end
type IPlugin = 
    abstract member Init : unit -> IPluginState
    abstract member Update : IPluginMsg -> IPluginState -> IPluginState
    abstract member View : IPluginState -> (IPluginMsg -> unit) -> Types.IView

