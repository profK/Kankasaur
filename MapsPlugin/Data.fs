module MapsPlugin.Data
open System.Text.Json
open Avalonia.FuncUI
open Kanka.NET.Kanka
open KankasaurPluginSupport.SharedTypes
open ManagerRegistry

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
        updated_by: int
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
        updated_by: int
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
        updated_by: int
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
            tags: string list
            created_at:  string
            created_by: int
            updated_at:  string
            updated_by: int
            location_id: int
            maptype: string
            height: int
            width: int
            map_id: int
            grid: int
            min_zoom: int
            max_zoom: int
            initial_zoom: int
            center_marker_id: int
            center_x: int
            center_y: int
            layers: LayerRec array
            groups: MapGroupRec array
}
let MakeLayerRec (data: System.Text.Json.JsonElement) =
    {
        created_at = data.GetProperty("created_at").GetString()
        created_by = data.GetProperty("created_by").GetInt32()
        height = data.GetProperty("height").GetInt32()
        id = data.GetProperty("id").GetInt32()
        is_private = data.GetProperty("is_private").GetBoolean()
        map_id = data.GetProperty("map_id").GetInt32()
        name = data.GetProperty("name").GetString()
        position = data.GetProperty("position").GetInt32()
        layertype = data.GetProperty("layertype").GetString()
        type_id = data.GetProperty("type_id").GetBoolean()
        updated_at = data.GetProperty("updated_at").GetString()
        updated_by = data.GetProperty("updated_by").GetInt32()
        visibility_id = data.GetProperty("visibility_id").GetInt32()
        width = data.GetProperty("width").GetInt32()
    }

let MakeGroupRec (data: System.Text.Json.JsonElement) =
    {
        created_at = data.GetProperty("created_at").GetString()
        created_by = data.GetProperty("created_by").GetInt32()
        id = data.GetProperty("id").GetInt32()
        is_private = data.GetProperty("is_private").GetBoolean()
        map_id = data.GetProperty("map_id").GetInt32()
        name = data.GetProperty("name").GetString()
        position = data.GetProperty("position").GetInt32()
        updated_at = data.GetProperty("updated_at").GetString()
        updated_by = data.GetProperty("updated_by").GetInt32()
        visibility_id = data.GetProperty("visibility_id").GetInt32()
    }
let MakeMapRec (data: System.Text.Json.JsonElement) =
    {
        id = data.GetProperty("id").GetInt32()
        name = data.GetProperty("name").GetString()
        entry = data.GetProperty("entry").GetString()
        entry_parsed = data.GetProperty("entry_parsed").GetString()
        image = data.GetProperty("image").GetString()
        image_full = data.GetProperty("image_full").GetString()
        image_thumb = data.GetProperty("image_thumb").GetString()
        has_custom_image = data.GetProperty("has_custom_image").GetBoolean()
        is_private = data.GetProperty("is_private").GetBoolean()
        is_real = data.GetProperty("is_real").GetBoolean()
        entity_id = data.GetProperty("entity_id").GetInt32()
        tags =
            let tags = data.GetProperty("tags")
            tags.EnumerateArray() |> Seq.map (fun t -> t.ToString()) |> Seq.toList
        created_at = data.GetProperty("created_at").GetString()
        created_by = data.GetProperty("created_by").GetInt32()
        updated_at = data.GetProperty("updated_at").GetString()
        updated_by = data.GetProperty("updated_by").GetInt32()
        location_id = data.GetProperty("location_id").GetInt32()
        maptype = data.GetProperty("maptype").GetString()
        height = data.GetProperty("height").GetInt32()
        width = data.GetProperty("width").GetInt32()
        map_id = data.GetProperty("map_id").GetInt32()
        grid = data.GetProperty("grid").GetInt32()
        min_zoom = data.GetProperty("min_zoom").GetInt32()
        max_zoom = data.GetProperty("max_zoom").GetInt32()
        initial_zoom = data.GetProperty("initial_zoom").GetInt32()
        center_marker_id = data.GetProperty("center_marker_id").GetInt32()
        center_x = data.GetProperty("center_x").GetInt32()
        center_y = data.GetProperty("center_y").GetInt32()
        layers =
            data.GetProperty("layers").EnumerateArray()
            |> Seq.map (fun i -> MakeLayerRec  i)
            |> Seq.toArray
        groups = data.GetProperty("groups").EnumerateArray()
            |> Seq.map (fun g -> MakeGroupRec g)
            |> Seq.toArray
     }
      