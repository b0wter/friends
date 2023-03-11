open System
open Elmish
open Xelmish.Model
open Friends.Xelmish.Screens


type Model =
    | Overworld of OverworldScreen.Model
    
    
type Message =
    | OverworldScreenMessage of OverworldScreen.Message
    
    
let init () =
    Overworld (OverworldScreen.init ()), Cmd.none
    
    
let update message model =
    match model, message with
    | Overworld o, OverworldScreenMessage msg ->
        match msg with
        | OverworldScreen.CameraUp -> (Overworld o, Cmd.none)
        | OverworldScreen.CameraDown -> (Overworld o, Cmd.none)
        | OverworldScreen.CameraLeft -> (Overworld o, Cmd.none)
        | OverworldScreen.CameraRight -> (Overworld o, Cmd.none)
    // Invalid combination
    | _ -> model, Cmd.none
    

let view model dispatch =
    match model with
    | Overworld overworldScreen ->
        OverworldScreen.view overworldScreen (OverworldScreenMessage >> dispatch)
        
        
[<EntryPoint; STAThread>]
let main _ =
    let config = {
        resolution = Windowed (1920, 1080)
        clearColour = Some Colour.Gray
        mouseVisible = true
        assetsToLoad = [
            PipelineFont ("connection", "./Content/Connection")
        ]
    }

    Program.mkProgram init update view
    |> Program.withConsoleTrace
    |> Xelmish.Program.runGameLoop config
    0