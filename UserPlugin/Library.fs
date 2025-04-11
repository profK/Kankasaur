namespace Kankasaur

open System.Text.Json
open Avalonia.FuncUI
open Kanka.NET.Kanka
open Kankasaur.PluginInterface
open ManagerRegistry

    

module User =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type UserState = {
        id:  int32
        name: string
        avatarURL:string
        avatar_thumbURL: string
        locale: string
        timezone: string
        date_format: string
        default_pagination: int32
        last_campaign_id: int32
        is_subscriber: bool
        rate_limit: int } interface IPluginState
    
    let init =
        GetProfile()
        |>fun (jel: JsonElement) ->
                       let data = jel.GetProperty("data")
                       {
                            id = data.GetProperty("id").GetInt32()
                            name = data.GetProperty("name").GetString()
                            avatarURL = data.GetProperty("avatar").GetString()
                            avatar_thumbURL = data.GetProperty("avatar_thumb").GetString()
                            locale = data.GetProperty("locale").GetString()
                            timezone = data.GetProperty("timezone").GetString()
                            date_format = data.GetProperty("date_format").GetString()
                            default_pagination = data.GetProperty("default_pagination").GetInt32()
                            last_campaign_id = data.GetProperty("last_campaign_id").GetInt32()
                            is_subscriber = data.GetProperty("is_subscriber").GetBoolean()
                            rate_limit = data.GetProperty("rate_limit").GetInt32()
                       }, Map.empty<string,obj>


    type UserMsg = Increment | Decrement | Reset  interface IPluginMsg


    let update (msg: UserMsg) (state: UserState)
        (values:Map<string,obj>):UserState * Map<string, obj> =
        state, Map.empty<string,obj>
    
    let view (state: UserState) (dispatch: IPluginMsg -> unit   ) : Types.IView=
        TextBox.create [
            TextBox.text (state.ToString())
            
        ]
        
    [<ManagerRegistry.Manager("User",
           supportedSystems.Linux|||supportedSystems.Windows|||supportedSystems.Mac)>]
    type UserPlugin() =
        interface IPlugin with
            member this.Init() =
                init
                |> fun (state, map) ->
                    state :> IPluginState, map
            member this.Update(msg:IPluginMsg) (state:IPluginState)
                    (values:Map<string, obj>)=
                let msg = msg :?> UserMsg
                update msg (state :?> UserState) values
                match state, values with
                | :? UserState as state, _ ->
                    (state :> IPluginState, Map.empty)
          
            member this.View(state:IPluginState) (dispatch:(IPluginMsg -> unit)) =
                view (state :?> UserState) dispatch :> Types.IView
                    