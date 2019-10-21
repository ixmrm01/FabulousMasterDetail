namespace FabulousMasterDetail

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module LoginPage = 
    type Model = {
        Password : string
    }

    type Msg = 
        | ChangePassword of string
        | Login
        | LoggedIn

    let initModel = {
        Password = ""
    }

    let init () = 
        initModel, Cmd.none

    let update msg model =
        match msg with
        | ChangePassword p -> {model with Password = p}, Cmd.none
        | Login ->
            let p =
                async {
                    let! isAuthenticated = (async {return model.Password = "1234"})
                    if isAuthenticated then 
                        return LoggedIn
                    else 
                        return ChangePassword ""
                }
            model, Cmd.ofAsyncMsg p
        | LoggedIn -> model, Cmd.none

    let view (model: Model) dispatch =
        View.ContentPage(
            title = "Log In",
            content = View.StackLayout(
                padding = 30.0,
                verticalOptions = LayoutOptions.Center,
                children = [
                    View.Entry(
                        text = model.Password,
                        placeholder = "Enter Passcode",
                        textChanged = (
                            fun args ->
                                args.NewTextValue
                                |> ChangePassword
                                |> dispatch
                        ),
                        isPassword = true
                    )
                    View.Button(
                        text = " Log in ",
                        backgroundColor = Color.CadetBlue,
                        textColor = Color.White,
                        horizontalOptions = LayoutOptions.Center,
                        command = (fun () -> dispatch Login)
                    )
                ]
            )
        )
