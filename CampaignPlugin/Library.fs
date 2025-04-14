namespace CampaignPlugin

module Campaign =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Kanka.NET.Kanka
    open Kankasaur.PluginInterface
    open ManagerRegistry

    type CampaignState = {
        Campaign: CampaignState } interface IPluginState

    let init()  =
        GetCampaign()
        |>fun (jel: JsonElement) ->
                      let data = jel.GetProperty("data")
                      {
                         id = data.GetProperty("id").GetInt32()
                         name = data.GetProperty("name").GetString()
                         is_private = data.GetProperty("is_private").GetBoolean()
                         is_collaborative = data.GetProperty("is_collaborative").GetBoolean()
                         is_admin = data.GetProperty("is_admin").GetBoolean()
                         is_premium = data.GetProperty("is_premium").GetBoolean()
                         is_pinned = data.GetProperty("is_pinned").GetBoolean()
                         is_subscribed = data.GetProperty("is_subscribed").GetBoolean()
                         is_community = data.GetProperty("is_community").GetBoolean()
                         is_featured = data.GetProperty("is_featured").GetBoolean()
                      }