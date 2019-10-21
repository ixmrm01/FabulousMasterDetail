namespace FabulousMasterDetail

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module MasterDetailPage = 
    type PagePage =
        | AboutPage
        | HelloWorldPage

    type PageModel =
        | AboutModel of AboutPage.Model
        | HelloWorldModel of HelloWorldPage.Model

    type Model = {
        CurrentPage: PagePage
        CurrentModel: PageModel
        IsMasterPresented : bool
    }

    type Msg = 
        | AboutMsg of AboutPage.Msg
        | HelloWorldMsg of HelloWorldPage.Msg
        | PageChanged of PagePage
        | IsMasterPresentedChanged of bool
        | LoggedOut

    let initializePage (page: PagePage) =
        match page with
        | AboutPage ->
            let (am, aCmd) = AboutPage.init()
            {
                CurrentPage = AboutPage
                CurrentModel = AboutModel am
                IsMasterPresented = false
            }, Cmd.map AboutMsg aCmd
        | HelloWorldPage ->
            let (hwm, hwCmd) = HelloWorldPage.init()
            {
                CurrentPage = HelloWorldPage
                CurrentModel = HelloWorldModel hwm
                IsMasterPresented = false
            }, Cmd.map HelloWorldMsg hwCmd

    let init () = 
        initializePage HelloWorldPage

    let update msg model =
        match msg, model.CurrentModel with
        | AboutMsg msg, AboutModel m ->
            let (am, aCmd) = AboutPage.update msg m
            {model with CurrentModel = AboutModel am}, Cmd.map AboutMsg aCmd
        | HelloWorldMsg msg, HelloWorldModel m ->
            let (hwm, hwCmd) = HelloWorldPage.update msg m
            {model with CurrentModel = HelloWorldModel hwm}, Cmd.map HelloWorldMsg hwCmd
        | PageChanged p, _ -> initializePage p
        | IsMasterPresentedChanged b, _ -> {model with IsMasterPresented = b}, Cmd.none
        | LoggedOut, _ -> model, Cmd.none
        | _, _ -> failwith "Wrong message, model combination"

    let masterPage model dispatch =
        View.ContentPage(
            useSafeArea = true,
            title = "Fabulous Master Detail",
            content = View.StackLayout(
                children = [
                    View.Button(
                        text = "About",
                        textColor = Color.Black,
                        command = (fun _ -> PageChanged AboutPage |> dispatch)
                    )
                    View.Button(
                        text = "Hello World",
                        textColor = Color.Black,
                        command = (fun _ -> PageChanged HelloWorldPage |> dispatch)
                    )
                    View.Button(
                        text = "Logout",
                        textColor = Color.Black,
                        command = (fun _ -> LoggedOut |> dispatch)
                    )
                ]
            ), icon = (if Device.RuntimePlatform = Device.iOS then "menu.png" else null)
        )

    let detailPage model dispatch =
        match model.CurrentPage, model.CurrentModel with
        | AboutPage _, AboutModel m -> AboutPage.view m (AboutMsg >> dispatch)
        | HelloWorldPage _, HelloWorldModel m -> HelloWorldPage.view m (HelloWorldMsg >> dispatch)
        | _, _ -> failwithf "Wrong page model combination at view level"

    let view (model: Model) dispatch =
        View.MasterDetailPage(
            masterBehavior = MasterBehavior.Popover,
            isPresented = model.IsMasterPresented,
            isPresentedChanged = (fun b -> (IsMasterPresentedChanged b) |> dispatch),
            master = masterPage model dispatch,
            detail = detailPage model dispatch
        )
