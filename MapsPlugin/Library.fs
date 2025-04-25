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

  //  type MapsMsg =
 //       | 
 //       interface IPluginMsg
    
    let GetMapsList (jel: JsonElement) =
        jel.GetProperty("data")
        |> fun data ->
            printfn $" { data.ToString()}"
            data.EnumerateArray()
            |> Seq.cast<JsonElement>
            |>Seq.map (fun map ->
                 map
                 |> MakeMapRec)
            

            
    let update (msg: ShellMsg) (pstate:IAppState) (state: IPluginState) :IAppState * IPluginState=
        let shellState = pstate :?> ShellState
        let state = state :?> MapsState
        match   msg  with
            | MapSelected index ->
                 {shellState with
                    mapID =
                        state.Maps
                         |> Seq.toArray
                         |> fun map -> Some map.[index].id
                  } :> IAppState, state
            | CampaignSelected id ->
                let maps = GetMapsList (GetMaps (id.ToString()))
                printfn $" { maps.ToString()}"
                {shellState with
                    mapID = None
                    campaignID = Some id
                } :> IAppState, { state with Maps = maps }
            
            
                
        
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
                   match index with
                   | idx when idx >= 0 ->
                        printfn $"Selected map {names.[idx]}"
                        dispatch (MapSelected idx)
                     | _ -> ())
                  
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
                    