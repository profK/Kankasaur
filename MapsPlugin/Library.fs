namespace MapsPlugin

open System.Net.Http
open System.Threading.Tasks

open System.Text.Json
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
            try
                let! response = httpClient.GetAsync(url)
                if response.IsSuccessStatusCode then
                    use! stream = response.Content.ReadAsStreamAsync()
                    return Some(Bitmap(stream))
                else
                    return None
            with
            | _ -> return None
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
                shellState, newState :> IPluginState, None
        | MapSelected index ->
            match index with
            |idx when (idx>=0) ->               
                let url = state.Maps[index].image_full // Replace with your URL
                
                let cmd =
                    Cmd.OfAsync.perform (fun () ->
                        async {
                            let! bitmapOpt = loadMapImage url |> Async.AwaitTask
                            match bitmapOpt with
                            | Some bitmap ->
                                printfn "Image loaded successfully"
                                return PluginMsg (ImageLoaded bitmap) :> obj
                            | None ->
                                printfn "Image load failed"
                                return PluginMsg (ImageLoadFailed url) :> obj
                        }) ()
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
                    Image.create [
                        match state.CurrentMapImg with
                        | Some img -> Image.source img
                        | None -> ()
                        Image.stretch Stretch.Uniform
                    ]
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
                    