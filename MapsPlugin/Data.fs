module MapsPlugin.Data

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