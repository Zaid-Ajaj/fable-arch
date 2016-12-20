namespace WebApp

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.App.AppApi
open Fable.Arch.Html
open Fable.Arch.RouteParser.Parsing
open Fable.Arch.RouteParser.RouteParser

open Aether
open Aether.Operators

open WebApp
open WebApp.Common

module Main =

  type SubModels =
    { Navbar: Navbar.Model
      Menu: Menu.Model
      Index: Pages.Index.Model option
      About: Pages.About.Model option
      User: Pages.User.Dispatcher.Model option
    }

    static member Initial =
      { Navbar = Navbar.Model.Initial(Index)
        Menu = Menu.Model.Initial(Index)
        Index = None
        About = None
        User = None
      }

    static member Index_ =
      (fun r -> r.Index), (fun v r -> { r with Index = Some v } )

    static member About_ =
      (fun r -> r.About), (fun v r -> { r with About = Some v } )

    static member User_ =
      (fun r -> r.User), (fun v r -> { r with User = Some v } )

    static member Navbar_ =
      (fun r -> r.Navbar), (fun v r -> { r with Navbar = v } )

    static member Menu_ =
      (fun r -> r.Menu), (fun v r -> { r with Menu = v } )

  type Model =
    { CurrentPage: Route
      SubModels: SubModels
    }

    static member Initial =
      { CurrentPage = Index
        SubModels = SubModels.Initial
      }

    static member SubModels_ =
      (fun r -> r.SubModels), (fun v r -> { r with SubModels = v } )

    static member CurrentPage_ =
      (fun r -> r.CurrentPage), (fun v r -> { r with CurrentPage = v } )


  type Actions
    = NoOp
    | NavigateTo of Route
    | IndexActions of Pages.Index.Actions
    | AboutActions of Pages.About.Actions
    | UserDispatcherAction of Pages.User.Dispatcher.Actions
    | MenuActions of Menu.Actions
    | NavbarActions of Navbar.Actions


  let update model action =
    match action with
    | NavigateTo route ->
      match route with
      | Index ->
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.Index_) Pages.Index.Model.Initial
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Navbar_ >-> Navbar.Model.CurrentPage_) route
            |> Optic.set (Model.CurrentPage_) route
          m', []
      | About ->
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.About_) Pages.About.Model.Initial
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Navbar_ >-> Navbar.Model.CurrentPage_) route
            |> Optic.set (Model.CurrentPage_) route
          m', []
      | User subRoute ->
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.User_) (Pages.User.Dispatcher.Model.Initial(subRoute))
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Navbar_ >-> Navbar.Model.CurrentPage_) route
            |> Optic.set (Model.CurrentPage_) route

          let message =
            match subRoute with
            | UserApi.Route.Index ->
              [ fun h ->
                  h (UserDispatcherAction (Pages.User.Dispatcher.Actions.IndexActions Pages.User.Index.Actions.Init))
              ]
            | _ -> []
          m', message
    | IndexActions act ->
        let (res, action) = Pages.Index.update model.SubModels.Index.Value act
        let action' = mapActions IndexActions action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.Index_) res model
        m', action'
    | AboutActions act ->
        let (res, action) = Pages.About.update model.SubModels.About.Value act
        let action' = mapActions AboutActions action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.About_) res model
        m', action'
    | UserDispatcherAction act ->
        let (res, action) = Pages.User.Dispatcher.update model.SubModels.User.Value act
        let action' = mapActions UserDispatcherAction action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.User_) res model
        m', action'
    | MenuActions act ->
        let (res, action) = Menu.update model.SubModels.Menu act
        let action' = mapActions MenuActions action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.Menu_) res model
        m', action'
    | NavbarActions act ->
        let (res, action) = Navbar.update model.SubModels.Navbar act
        let action' = mapActions NavbarActions action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.Navbar_) res model
        m', action'
    | NoOp -> model, []

  let view model =
    let pageHtml =
      match model.CurrentPage with
      | Index -> Html.map IndexActions (Pages.Index.view model.SubModels.Index.Value)
      | About -> Html.map AboutActions (Pages.About.view model.SubModels.About.Value)
      | User subRoute -> Html.map UserDispatcherAction (Pages.User.Dispatcher.view model.SubModels.User.Value subRoute)

    let navbarHtml =
      Html.map NavbarActions (Navbar.view model.SubModels.Navbar)

    let menuHtml =
      Html.map MenuActions (Menu.view model.SubModels.Menu)

    div
      []
      [ div
          [ classy "container" ]
          [ navbarHtml
            div
              [ classy "columns content" ]
              [ div
                  [ classy "column is-10 is-offset-1" ]
                  [ pageHtml ]
              ]
          ]
        menuHtml
      ]



  let routes =
    [
      runM (NavigateTo Index) (pStaticStr "/" |> (drop >> _end))
      runM (NavigateTo About) (pStaticStr "/about" |> (drop >> _end))
      runM (NavigateTo (User UserApi.Index)) (pStaticStr "/users" |> (drop >> _end))
      runM (NavigateTo (User UserApi.Create)) (pStaticStr "/user/create" |> (drop >> _end))
      runM1 (fun id -> (NavigateTo (User (UserApi.Edit id)))) (pStaticStr "/user" </.> pint <./> pStaticStr "edit")
      runM1 (fun id -> (NavigateTo (User (UserApi.Show id)))) (pStaticStr "/user" </.> pint)
    ]


  let mapToRoute route =
    match route with
    | NavigateTo r ->
        resolveRoutesToUrl r
    | _ -> None


  let router = createRouter routes mapToRoute

  let locationHandler =
    {
      SubscribeToChange =
        (fun h ->
            window.addEventListener_hashchange(fun _->
              h(location.hash.Substring 1)
              null
            )
        )

      PushChange =
        (fun s -> location.hash <- s)
    }


  let routerF m = router.Route m.Message

  // Ensure database creation
  WebApp.Database.init ()

  createApp Model.Initial view update Virtualdom.createRender
  |> withStartNodeSelector "#app"
  |> withProducer (routeProducer locationHandler router)
  |> withSubscriber (routeSubscriber locationHandler routerF)
  |> start
  |> ignore
