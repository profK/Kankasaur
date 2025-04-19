namespace MapsPlugin

open System.Text.Json
open Avalonia.FuncUI
open Kanka.NET.Kanka
open KankasaurPluginSupport.SharedTypes
open ManagerRegistry
open MapsPlugin.Data

    

module Maps =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type MapsState = {
        Maps: MapRec seq } interface IPluginState
    
    let init (appState:ShellState)  : MapsState * ShellState=
        match appState.campaignID with
        |  id when id <=0-> { Maps = Seq.empty }, appState
        | _ ->
            GetMaps (appState.campaignID.ToString())
            |> fun (jel: JsonElement) ->
                    let data = jel.GetProperty("data")
                    printfn $" { data.ToString()}"
                    data.EnumerateArray()
                    |> Seq.cast<JsonElement>
                    |>Seq.map (fun map ->
                         map
                         |> MakeMapRec)
                    |> fun maps ->
                            {  Maps = maps},  appState
       

    type MapsMsg =
        | MapSelected of int
        interface IPluginMsg


    let update (msg: MapsMsg) (pstate:IAppState) (state: MapsState) :IAppState * MapsState =
        pstate, state
    
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
                match msg with
                | :? MapsMsg as msg ->
                    match msg with
                    | MapSelected index ->
                        match index with
                        | i when i < 0 -> aState, pState
                        | i when i >= 0 ->
                            let map = (pState :?> MapsState).Maps |> Seq.toList |> List.item index
                            {(aState :?> ShellState ) with mapID = map.id}  , pState
                            let newA, newP = update msg aState (pState :?> MapsState) 
                            newA, newP :> IPluginState
                    | _ -> aState, pState
                | _ -> aState, pState
            member this.View (appState: IAppState) (state:IPluginState) (dispatch:(IPluginMsg -> unit)) =
                view appState (state :?> MapsState) dispatch :> Types.IView
                    