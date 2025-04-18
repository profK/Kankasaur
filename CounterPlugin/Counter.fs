namespace Kankasaur

open Avalonia.FuncUI

open KankasaurPluginSupport.SharedTypes
open ManagerRegistry

    

module  Counter=
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type CounterState = { count : int } interface IPluginState
    let init  IAppState=
        { count = 0 }

    type CounterMsg = Increment | Decrement | Reset  interface IPluginMsg


    let update (msg: CounterMsg) (appState: IAppState) (state: CounterState) :
        IAppState * CounterState =
        match msg with
        | Increment -> (appState, { state with count = state.count + 1 })
        | Decrement -> (appState,{ state with count = state.count - 1 } )
        | Reset -> (appState, init appState)
    
    let view appState (state: CounterState) (dispatch: IPluginMsg -> unit   ) : Types.IView=
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
           supportedSystems.Linux|||supportedSystems.Windows|||supportedSystems.Mac,
           [||] , 0 )>]
    type CounterPlugin() =
        interface IPlugin with
            member this.Init (appState: IAppState)=
                let pluginState = (init appState) :> IPluginState
                appState,  pluginState 
            member this.Update(msg:IPluginMsg) (appState:IAppState)
                (state:IPluginState) =
                let msg = msg :?> CounterMsg
                let uAppState, uPluginState =
                    update msg (appState) (state :?> CounterState)
                (uAppState, uPluginState :> IPluginState)
                
           
            member this.View appState pState (dispatch:(IPluginMsg -> unit)) =
                view appState (pState :?> CounterState) dispatch :> Types.IView
                     