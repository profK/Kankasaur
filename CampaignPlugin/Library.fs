namespace CampaignPlugin

open System.Text.Json
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open CampaignPlugin.Data
open KankasaurPluginSupport.SharedTypes
open ManagerRegistry

module Campaign =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Kanka.NET.Kanka

    open ManagerRegistry

    type CampaignState = {
        Campaigns: CampaignData seq } interface IPluginState
    
    type  CampaignMsg =
        | CampaignSelected of int
        interface IPluginMsg

    let init()  =
        
        GetCampaigns()
        |> fun (jel: JsonElement) ->
                let data = jel.GetProperty("data")
                printfn $" { data.ToString()}"
                data.EnumerateArray()
                |> Seq.cast<JsonElement>
                |>Seq.map (fun campaign ->
                     campaign
                     |> parseCampaignData)
                |> fun campaigns ->
                    {
                        Campaigns = campaigns
                    }
    let update (msg: CampaignMsg) (pstate:IAppState)
                (state: CampaignState) :IAppState * CampaignState =
                    pstate, state
                    
    let view (pState:IAppState) (state: CampaignState)
        (dispatch: IPluginMsg -> unit   ) : Types.IView=
           let names =
                (state.Campaigns)
                |> Seq.map (
                        fun c ->
                            c.name)
                         
                 |> Seq.toList
           ComboBox.create [
               ComboBox.dataItems  names
           ]


    [<ManagerRegistry.Manager("Campaigns",
           supportedSystems.Linux|||supportedSystems.Windows|||supportedSystems.Mac,
            [||] , 0 )>]
   
    type CampaignPlugin() =
        interface IPlugin with
            member this.Init()  =
                init() :> IPluginState
                
            member this.Update(msg:IPluginMsg) (aState:IAppState) (pState:IPluginState) =
                let msg = msg :?> CampaignMsg
                let newA, newP = update msg aState (pState :?> CampaignState) 
                newA, newP :> IPluginState
          
            member this.View (appState: IAppState) (state:IPluginState) (dispatch:(IPluginMsg -> unit)) =
                view appState (state :?>CampaignState) dispatch :> Types.IView