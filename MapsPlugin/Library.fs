namespace MapsPlugin

open System
open System.Text.Json
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Media
open Avalonia.Media.Imaging
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
        Maps: MapRec list
        CurrentMapImg : Bitmap option} interface IPluginState
     
    type MapsMsg =
        | LoadImage
        interface IPluginMsg
    
    let init (appState:ShellState)  : IPluginState * ShellState=
        let state = {
            Maps = List.empty
            CurrentMapImg = None
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
            

            
    let update (msg: ShellMsg) (pstate:ShellState) (state: IPluginState) :ShellState * IPluginState=
        let shellState = pstate 
        let state = state :?> MapsState
        match   msg  with
            | MapSelected index ->
                 printfn $"Num Maps (update) {state.Maps |> Seq.length}"
                 {shellState with
                    mapID =
                        state.Maps
                        |> Seq.toArray
                        |> fun map -> Some map.[index].id
                  } :> ShellState, state
            | CampaignSelected idx ->
                let cid = pstate.campaignID
                let maps =
                    match cid with
                    |Some id ->GetMapsList (GetMaps (id.ToString())) |> Seq.toList
                    |None ->
                        printfn $"No campaign selected"
                        List.empty
                {shellState with
                    mapID = None
                } :> ShellState, { state with Maps = maps }
            
            
                
        
    let view (pState:ShellState) (state: MapsState) (dispatch   ) : Types.IView=
           //printfn $"Maps Count view {state.Maps |> Seq.length}"
       let names =
                (state.Maps)
                 |> Seq.map (
                         fun c ->
                             c.name)                      
                 |> Seq.toList
       Grid.create [
        Grid.children [
            // ScrollViewer that contains the image
            ScrollViewer.create [
                ScrollViewer.content (
                    Image.create [
                        match state.CurrentMapImg with
                        | Some img -> Image.source img
                        | None -> ()
                        Image.stretch Avalonia.Media.Stretch.Uniform
                    ]
                )
            ]

            // ComboBox overlaid at the top left
            ComboBox.create [
                ComboBox.dataItems names
                
                ComboBox.onSelectedIndexChanged(fun args ->
                       printfn $"Maps Count (view) {state.Maps |> Seq.length}"
                       let index = args 
                       match index with
                       | index when index >= 0 ->
                            //printfn $"Selected map {names.[index]}"
                            //let mid = state.Maps |> Seq.toList |> List.item index
                            dispatch (LoadImage)
                       | _ -> printfn "Invalid index value")
                Grid.row 0
                Grid.column 0
                // Use Margin to place it a little bit away from top/left
                ComboBox.margin (10.0, 10.0, 0.0, 0.0)
                // Floating in the grid
                Panel.zIndex 1
                // Optional: Small width
                ComboBox.width 150.0
            ]
        ]
    ] 
 (*ComboBox.create [
                   ComboBox.dataItems  names
                   ComboBox.onSelectedIndexChanged (fun args ->
                       printfn $"Maps Count (view) {state.Maps |> Seq.length}"
                       let index = args
                       match index with
                       | index when index >= 0 ->
                            //printfn $"Selected map {names.[index]}"
                            //let mid = state.Maps |> Seq.toList |> List.item index
                            dispatch (LoadImage)
                       | _ -> printfn "Invalid index value"
                   )
               ]*)               
                
                   
    [<OrderAttribute(3)>]
    [<AutoOpen>]
    [<ManagerRegistry.Manager("Maps",
           supportedSystems.Linux|||supportedSystems.Windows|||supportedSystems.Mac,
            [||] , 0 )>]
    type MapsPlugin() =
        interface IPlugin with
            member this.Init (appState:ShellState  )  : ShellState * IPluginState    =
                init (appState)
                |> fun tpl ->
                        let state =( fst tpl) :> IPluginState
                        let appState =snd tpl :> ShellState
                        appState, state
                    
                
            member this.Update(msg:ShellMsg) (aState:ShellState) (pState:IPluginState) =
                 update msg aState pState
            member this.View (appState: ShellState) (state:IPluginState) (dispatch:(obj -> unit)) =
                view appState (state :?> MapsState) dispatch :> Types.IView
                    