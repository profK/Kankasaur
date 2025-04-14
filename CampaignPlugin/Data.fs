module CampaignPlugin.Data

type UserRec = {
    id: int
    name: string
    avatar: string
}

type MemberRecord = {
    id: int
    User: UserRec
}

type CampaignData = {
    id: int
    name: string
    locale: string
    entry: string
    entry_parsed: string
    image: string
    image_full: string
    image_thumb: string
    visibility: string
    visibility_id: int
    created_at: string
    updated_at: string
    members: MemberRecord seq
    setting: obj seq
    ui_settings: obj seq
    default_images: obj seq
    css: string
}

let parseCampaignData(data: System.Text.Json.JsonElement)  =
        {
            id = data.GetProperty("id").GetInt32()
            name = data.GetProperty("name").GetString()
            locale = data.GetProperty("locale").GetString()
            entry = data.GetProperty("entry").GetString()
            entry_parsed = data.GetProperty("entry_parsed").GetString()
            image = data.GetProperty("image").GetString()
            image_full = data.GetProperty("image_full").GetString()
            image_thumb = data.GetProperty("image_thumb").GetString()
            visibility = data.GetProperty("visibility").GetString()
            visibility_id = data.GetProperty("visibility_id").GetInt32()
            created_at = data.GetProperty("created_at").GetString()
            updated_at = data.GetProperty("updated_at").GetString()
            members =
                data.GetProperty("members").EnumerateArray()
                |> Seq.map (
                    fun mbr ->
                        let usr = mbr.GetProperty("user")
                        let id = mbr.GetProperty("id").GetInt32()
                        let usrRec ={
                            id = usr.GetProperty("id").GetInt32()
                            name = usr.GetProperty("name").GetString()
                            avatar = usr.GetProperty("avatar").GetString()
                            }
                        { id=id; User = usrRec }
                    )
                                
            setting = []
            ui_settings = []
            default_images = []
            css = ""
        }
