namespace Kankasaur

open Avalonia.FuncUI
open Kankasaur.PluginInterface
open ManagerRegistry

    

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type CounterState = { count : int } interface IPluginState
    let init =
        ({ count = 0 } , Map.empty<string,obj>)

    type CounterMsg = Increment | Decrement | Reset  interface IPluginMsg


    let update (msg: CounterMsg) (state: CounterState)   =
        match msg with
        | Increment -> ({ state with count = state.count + 1 }, Map.empty<string,obj>)
        | Decrement -> ({ state with count = state.count - 1 }, Map.empty)
        | Reset -> init 
    
    let view (state: CounterState) (dispatch: IPluginMsg -> unit   ) : Types.IView=
        DockPanel.create [
            DockPanel.children [
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Reset)
                    Button.content "reset"
                ]                
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Decrement)
                    Button.content "-"
                ]
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Increment)
                    Button.content "+"
                ]
                TextBlock.create [
                    TextBlock.dock Dock.Top
                    TextBlock.fontSize 48.0
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.horizontalAlignment HorizontalAlignment.Center
                    TextBlock.text (string state.count)
                ]
            ]
        ]
        
    [<ManagerRegistry.Manager("Counter",
           supportedSystems.Linux|||supportedSystems.Windows|||supportedSystems.Mac)>]
    type CounterPlugin() =
        interface IPlugin with
            member this.Init() =
                init
                |> fun tuple ->
                    (fst tuple :> IPluginState, snd tuple)
            member this.Update(msg:IPluginMsg) (state:IPluginState) (values:Map<string,obj>)=
                let msg = msg :?> CounterMsg
                update msg (state :?> CounterState)
                |> fun tuple ->
                    (fst tuple :> IPluginState, snd tuple)
          
            member this.View(state:IPluginState) (dispatch:(IPluginMsg -> unit)) =
                view (state :?> CounterState) dispatch :> Types.IView
                    