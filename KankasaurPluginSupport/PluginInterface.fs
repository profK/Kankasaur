module Kankasaur.PluginInterface

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout

type IPluginMsg= interface end
type IPluginState= interface end
type IPlugin = 
    abstract member Init : unit -> IPluginState * Map<string,obj>
    abstract member Update : IPluginMsg -> IPluginState -> Map<string, obj> ->IPluginState * Map<string,obj>
    abstract member View : IPluginState -> (IPluginMsg -> unit) -> Types.IView

