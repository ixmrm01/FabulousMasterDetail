namespace FabulousMasterDetail

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
// open Xamarin.Essentials

module AboutPage = 
    type Model = {
        AppName: string
        PackageName: string
        Version: string
        Build: string
    }

    type Msg = 
        | GetAppInfo
        | UpdateAppInfo of string * string * string * string

    let init () = 
        {
            AppName = ""
            PackageName = ""
            Version = ""
            Build = ""
        }, Cmd.ofMsg GetAppInfo

    let update msg model =
        match msg with
        | GetAppInfo ->
            // model, Cmd.ofMsg (UpdateAppInfo(AppInfo.Name, AppInfo.PackageName, AppInfo.VersionString, AppInfo.BuildString))
            model, Cmd.ofMsg (UpdateAppInfo("aaa", "ppppppp", "vvvvvvv", "bbbbb"))
        | UpdateAppInfo(a, p, v, b) -> {model with AppName = a; PackageName = p; Version = v; Build = b}, Cmd.none

    let view (model: Model) dispatch =
        View.NavigationPage(
            pages = [
                View.ContentPage(
                    content = View.StackLayout(
                        orientation = StackOrientation.Horizontal,
                        children = [
                            View.Label(text = (sprintf "App: %s" model.AppName ))
                            View.Label(text = (sprintf "Package: %s" model.PackageName ))
                            View.Label(text = (sprintf "Version: %s" model.Version ))
                            View.Label(text = (sprintf "Build: %s" model.Build ))
                        ]
                    )
                ).HasNavigationBar(true)
            ]
        )
