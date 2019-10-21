// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace FabulousMasterDetail

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module App = 
    type PageModel =
        | LoginModel of LoginPage.Model
        | MasterDetailModel of MasterDetailPage.Model

    type Model = {
        CurrentModel : PageModel
    }

    type Msg = 
        | LoginMsg of LoginPage.Msg
        | MasterDetailMsg of MasterDetailPage.Msg

    let init () = 
        let (lm, lCmd) = LoginPage.init()
        {CurrentModel = LoginModel lm}, Cmd.map LoginMsg lCmd

    let update msg model =
        match msg, model.CurrentModel with
        | LoginMsg msg, LoginModel m ->
            let (lm, lCmd) = LoginPage.update msg m
            match msg with
            | LoginPage.LoggedIn ->
                let (mdm, mdCmd) = MasterDetailPage.init()
                {model with CurrentModel = MasterDetailModel mdm}, Cmd.map MasterDetailMsg mdCmd
            | _ -> {model with CurrentModel = LoginModel lm}, Cmd.map LoginMsg lCmd
        | MasterDetailMsg msg, MasterDetailModel m ->
            let (mdm, mdCmd) = MasterDetailPage.update msg m
            match msg with
            | MasterDetailPage.Msg.LoggedOut -> init()
            | _ -> {model with CurrentModel = MasterDetailModel mdm}, Cmd.map MasterDetailMsg mdCmd
        | _, _ -> failwith "Wrong message, model combination"

    let view (model: Model) dispatch =
        match model.CurrentModel with
        | LoginModel m -> LoginPage.view m (LoginMsg >> dispatch)
        | MasterDetailModel m -> MasterDetailPage.view m (MasterDetailMsg >> dispatch)

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> XamarinFormsProgram.run app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
    //
    //do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
#if APPSAVE
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif


