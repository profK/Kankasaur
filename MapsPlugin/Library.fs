namespace MapsPlugin

open System.IO
open System.Net.Http

open System.Text.Json
open Avalonia.FuncUI.Types
open Avalonia.Threading
open System.Threading.Tasks
open Elmish
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Media
open Avalonia.Media.Imaging
open Kanka.NET.Kanka
open KankasaurPluginSupport

open KankasaurPluginSupport.SharedTypes
open ManagerRegistry
open MapsPlugin.Data

    

module Maps =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type MapsState = {
        Owner: int option
        Maps: MapRec list
        CurrentMapImg : Bitmap option} interface IPluginState
     
    type MapsMsg =
        | LoadImage of string
        | ImageLoaded of Bitmap
        | ImageLoadFailed of string
        interface IPluginMsg
    
    let init (appState:ShellState)  : IPluginState * ShellState=
        let state = {
            Maps = List.empty
            CurrentMapImg = None
            Owner = Some 0
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
            

    let  loadMapImage(url: string) : Task<Bitmap option> =
        task {
            use httpClient = new HttpClient()
        //    try
            let! response = httpClient.GetAsync(url)
            match response.IsSuccessStatusCode with
            | true ->
                use! stream = response.Content.ReadAsStreamAsync()
                let! bitmap =
                        Dispatcher.UIThread
                            .InvokeAsync(fun () -> Bitmap(stream))
                            .GetTask()
                        |> Async.AwaitTask
                return Some(bitmap)
            | false ->
                return None
        //    with
        //        | _ -> return None
        }
        
        
        
    let update (msg: ShellMsg) (pstate: ShellState) (state: IPluginState) :
        ShellState * IPluginState * Cmd<obj> option =
        let shellState = pstate
        let state = state :?> MapsState
        match msg with
        | PluginMsg pmsg  ->
          match pmsg with
          | :? MapsMsg as mapsMsg ->
            match mapsMsg with           
            | LoadImage url ->               
                shellState, state, None
            | ImageLoaded bitmap ->
                let newState = { state with
                                    CurrentMapImg = Some bitmap
                                    Owner = pstate.campaignID}
                //set the image to the current map
                
                shellState, newState :> IPluginState, None
        | MapSelected index ->
            match index with
            |idx when (idx>=0) ->               
                let url = state.Maps[index].image_full // Replace with your URL
                
                let cmd : Cmd<obj> =
                    Cmd.OfAsync.perform
                            (fun () ->
                                async {
                                    let! bitmapOpt = loadMapImage url |> Async.AwaitTask
                                    match bitmapOpt with
                                    | Some bitmap ->
                                        printfn "Image loaded successfully"
                                        return PluginMsg (ImageLoaded bitmap)  :> obj
                                    | None ->
                                        printfn "Image load failed"
                                        return PluginMsg (ImageLoadFailed url)  :> obj
                                })
                            ()
                            id  // result is already of type obj, so identity function is fine // map to 'msg type
                                // Dispatch the LoadImage message
                shellState, state, Some cmd 
            | _ -> shellState, state, None         
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
            } :> ShellState, { state with Maps = maps },  None   
                
     
    let view (pState:ShellState) (state: MapsState) (dispatch   ) : Types.IView=
           //printfn $"Maps Count view {state.Maps |> Seq.length}"
       let mapTuples =
                (state.Maps)
                 |> Seq.map (
                         fun c ->
                             (c.name,c.image_full))                      
                 |> Seq.toList
       Grid.create [
        Grid.children [
            // ScrollViewer containing the image
            ScrollViewer.create [
         ScrollViewer.content (
            match state.CurrentMapImg with
            | Some img ->
                Image.create [
                    Image.source img
                    Image.stretch Stretch.Uniform
                ] :> IView
            | None ->
                DockPanel.create [
                    DockPanel.lastChildFill true
                    DockPanel.children [
                        TextBlock.create [
                            TextBlock.text "No map loaded"
                            TextBlock.horizontalAlignment HorizontalAlignment.Center
                            TextBlock.verticalAlignment VerticalAlignment.Center
                            TextBlock.textAlignment TextAlignment.Center
                            TextBlock.fontSize 24.0
                        ]
                    ]
                ]:> IView
        )

            ]

            // ComboBox overlaid at the top-left
            ComboBox.create [
                ComboBox.dataItems
                    (mapTuples |> List.map (fun (name, _) -> name))
                ComboBox.onSelectedIndexChanged (fun index ->
                        dispatch (MapSelected index)
                )
                Grid.row 0
                Grid.column 0
                ComboBox.margin (10.0, 10.0, 0.0, 0.0) // Adjust margin as needed
                Panel.zIndex 1 // Ensure ComboBox is overlaid
                ComboBox.width 150.0 // Optional: Set width
           ]
        ]
    ]
       
                   
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
                    