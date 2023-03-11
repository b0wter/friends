namespace Friends.Xelmish.Screens

module OverworldScreen =
    open Xelmish.Viewables
    open Xelmish.Model
    
    type Model = {
        Counter: int
    }
    
    let init () =
        {
            Counter = 0
        }
        
    type Message =
        | CameraUp
        | CameraDown
        | CameraLeft
        | CameraRight

    let view model dispatch =
        let text size = text "connection" size Colour.White (-0.5, 0.)
        [
            yield text 100.0 "TETRIS!" (400, 40)
        ]
