module MapsPlugin.Data
open System.Text.Json
open Avalonia.FuncUI
open Kanka.NET.Kanka
open KankasaurPluginSupport.SharedTypes
open ManagerRegistry

let GetProperty (data: System.Text.Json.JsonElement) (name: string) =
    printf $"GetProperty {name}\n"
    match data.TryGetProperty(name) with
    | true, value -> value
    | false, _ -> failwith $"Property {name} not found in JSON element"

type LayerRec = {
        created_at: string
        created_by: int
        height: int
        id: int
        is_private: bool
        map_id: int
        name: string
        position: int
        layertype: string
        type_id: bool
        updated_at: string
        visibility_id: int
        width: int}

type MarkerRect = {
    top:int
    left:int
    bottom:int
    right:int
}



type MarkerRec = {
        circle_radius: int
        colour: int
        created_at: string
        created_by: int
        custom_icon: string
        custom_shape: MarkerRect
        entity_id: int
        font_colour: int
        icon: int
        id: int
        is_draggable: bool
        is_private: bool
        is_popupless: bool
        latitude: float
        longitude: float
        map_id: int
        name: string
        opacity: int
        polygon_style: string
        shape_id: int
        size_id: int
        updated_at: string
        visibility_id: int
}

type MapGroupRec = {
        created_at: string
        created_by: int
        id: int
        is_private: bool
        map_id: int
        name: string
        position: int
        updated_at: string

        visibility_id: int
}

type MapRec = {
            id: int
            name: string
            entry: string
            entry_parsed: string
            image: string
            image_full: string
            image_thumb: string
            has_custom_image: bool
            is_private: bool
            is_real: bool
            entity_id: int
            //tags: string list
            created_at:  string
            //created_by: int
            updated_at:  string
            //updated_by: int

            maptype: string
           // height: int
           // width: int

            grid: int
            min_zoom: float32
            max_zoom: float32
            initial_zoom: float32
            
            layers: LayerRec array
            groups: MapGroupRec array
}
let MakeLayerRec (data: System.Text.Json.JsonElement) =
    {
        created_at = (GetProperty data "created_at").GetString()
        created_by = (GetProperty data "created_by").GetInt32()
        height = (GetProperty data "height").GetInt32()
        id = (GetProperty data "id").GetInt32() 
        is_private = (GetProperty data "is_private").GetBoolean()
        map_id = (GetProperty data "map_id").GetInt32()
        name = (GetProperty data "name").GetString()
        position = (GetProperty data "position").GetInt32()
        layertype = (GetProperty data "type").GetString()
        type_id = (GetProperty data "type_id").GetBoolean()
        updated_at = (GetProperty data "updated_at").GetString()

        visibility_id = (GetProperty data "visibility_id").GetInt32()
        width = (GetProperty data "width").GetInt32()
    }

let MakeGroupRec (data: System.Text.Json.JsonElement) =
    {
        created_at = (GetProperty data "created_at").GetString()
        created_by = (GetProperty data "created_by").GetInt32()
        id = (GetProperty data "id").GetInt32()
        is_private = (GetProperty data "is_private").GetBoolean()
        map_id = (GetProperty data "map_id").GetInt32()
        name = (GetProperty data "name").GetString()
        position = (GetProperty data "position").GetInt32()
        updated_at = (GetProperty data "updated_at").GetString()
        visibility_id = (GetProperty data "visibility_id").GetInt32()
    }

let MakeMapRec (data: System.Text.Json.JsonElement) =
    {
        id =  (GetProperty data "id").GetInt32()
        name = (GetProperty data "name").GetString()
        entry = data.GetProperty("entry").GetString()
        entry_parsed = data.GetProperty("entry_parsed").GetString()
        image = data.GetProperty("image").GetString()
        image_full = data.GetProperty("image_full").GetString()
        image_thumb = data.GetProperty("image_thumb").GetString()
        has_custom_image = data.GetProperty("has_custom_image").GetBoolean()
        is_private = data.GetProperty("is_private").GetBoolean()
        is_real = data.GetProperty("is_real").GetBoolean()
        entity_id = data.GetProperty("entity_id").GetInt32()
       // tags =
       //     let tags = data.GetProperty("tags")
        //    tags.EnumerateArray() |> Seq.map (fun t -> t.ToString()) |> Seq.toList
        created_at = (GetProperty data "created_at").GetString()
       // created_by = (GetProperty data "created_by").GetInt32()
        updated_at = (GetProperty data "updated_at").GetString()
       // updated_by = (GetProperty data "updated_by").GetInt32()

        maptype = (GetProperty data "type").GetString()
       // height = (GetProperty data "height").GetInt32()
       // width = (GetProperty data "width").GetInt32()
        grid = (GetProperty data "grid").GetInt32()
        min_zoom = (GetProperty data "min_zoom").GetSingle()
        max_zoom = (GetProperty data "max_zoom").GetSingle()
        initial_zoom = (GetProperty data "initial_zoom").GetSingle()
        
        layers =
            (GetProperty data "layers").EnumerateArray()
            |> Seq.map (fun i -> MakeLayerRec  i)
            |> Seq.toArray
        groups = (GetProperty data "groups").EnumerateArray()
            |> Seq.map (fun g -> MakeGroupRec g)
            |> Seq.toArray
     }
      