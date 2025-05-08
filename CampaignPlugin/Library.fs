namespace CampaignPlugin

open System
open System.Text.Json
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open CampaignPlugin.Data
open KankasaurPluginSupport
open KankasaurPluginSupport.SharedTypes
open ManagerRegistry

module Campaign =
    open Avalonia.Controls
   
    open Kanka.NET.Kanka
   

      type CampaignState = {
            Campaigns: CampaignData seq  } interface IPluginState
     
    //type  CampaignMsg =
    //   | CampaignSelected of int
    //    interface IPluginMsg

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
    let update (msg: ShellMsg) (pstate:ShellState)
                (state: CampaignState) :ShellState * IPluginState * ShellMsg option =
                    match msg with
                    | CampaignSelected idx ->
                        match idx with                      
                        | idx when idx >= 0 ->
                            let campaign = state.Campaigns |> Seq.toList |> List.item idx
                            printf $"Selected campaign {campaign.name}"
                            {pstate  with campaignID = Some campaign.id}  , state , None        
                        | _ -> pstate, state :>IPluginState , None
                    | _ -> pstate, state :>IPluginState,  None
                    
    let view (pState:ShellState) (state: CampaignState)
        (dispatch  ) : Types.IView=
           let names =
                (state.Campaigns)
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
                       dispatch (CampaignSelected idx)
                   | _ -> ())
           ]

    [<Order(2)>]
    [<ManagerRegistry.Manager("Campaigns",
           supportedSystems.Linux|||supportedSystems.Windows|||supportedSystems.Mac,
            [||] , 0 )>]
   
    type CampaignPlugin() =
        interface IPlugin with
            member this.Init appState=
                appState, init() :> IPluginState
                
            member this.Update(msg:ShellMsg) (aState:ShellState) (pState:IPluginState) =
                update msg aState (pState :?> CampaignState)
          
            member this.View (appState: ShellState) (state:IPluginState) (dispatch:(obj -> unit)) =
                view appState (state :?>CampaignState) dispatch :> Types.IView