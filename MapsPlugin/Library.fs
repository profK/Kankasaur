namespace MapsPlugin

open System.Text.Json
open Avalonia.FuncUI
open Kanka.NET.Kanka
open KankasaurPluginSupport
open KankasaurPluginSupport
open KankasaurPluginSupport.SharedTypes
open ManagerRegistry
open MapsPlugin.Data

    

module Maps =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type MapsState = {
        Maps: MapRec seq } interface IPluginState
    
    let init (appState:ShellState)  : IPluginState * ShellState=
        let state = {
            Maps = Seq.empty
        } 
        state:> IPluginState, appState 

    type MapsMsg =
        | MapSelected of int
        interface IPluginMsg
    
    let GetMapsList (jel: JsonElement) =
        jel.GetProperty("data")
        |> fun data ->
            printfn $" { data.ToString()}"
            data.EnumerateArray()
            |> Seq.cast<JsonElement>
            |>Seq.map (fun map ->
                 map
                 |> MakeMapRec)
            
    let update (msg: IPluginMsg) (pstate:IAppState) (state: IPluginState) :IAppState * IPluginState=
        let shellState = pstate :?> ShellState
        let state = state :?> MapsState
        match shellState.campaignID with
        | None-> pstate, state
        | Some cid ->
            let newShellState =
                match msg with
                | :? MapsMsg as mapsMsg  ->
                    //shellState
                    match mapsMsg with
                    | MapSelected index ->
                           {(pstate :?> ShellState ) with mapID = Some index}
                    | _ -> shellState
                | _ -> shellState
                
            let newMapsState=   
                 GetMaps (cid.ToString())
                 |> GetMapsList
                 |> fun maps ->
                    {
                        Maps = maps
                    }
            newShellState,newMapsState :> IPluginState
            
                
        
    let view (pState:IAppState) (state: MapsState) (dispatch: IPluginMsg -> unit   ) : Types.IView=
           let names =
                    (state.Maps)
                     |> Seq.map (
                             fun c ->
                                 c.name)
                              
                     |> Seq.toList
                 
           ComboBox.create [
               ComboBox.dataItems  names
               ComboBox.onSelectedIndexChanged (fun args ->
                   let index = args
                   dispatch (MapSelected index))
               ]
        
    [<OrderAttribute(3)>]
    [<AutoOpen>]
    [<ManagerRegistry.Manager("Maps",
           supportedSystems.Linux|||supportedSystems.Windows|||supportedSystems.Mac,
            [||] , 0 )>]
    type MapsPlugin() =
        interface IPlugin with
            member this.Init (appState:IAppState  )  : IAppState * IPluginState    =
                init (appState :?> ShellState)
                |> fun tpl ->
                        let state =( fst tpl) :> IPluginState
                        let appState =snd tpl :> IAppState
                        appState, state
                    
                
            member this.Update(msg:IPluginMsg) (aState:IAppState) (pState:IPluginState) =
                 update msg aState pState
            member this.View (appState: IAppState) (state:IPluginState) (dispatch:(IPluginMsg -> unit)) =
                view appState (state :?> MapsState) dispatch :> Types.IView
                    