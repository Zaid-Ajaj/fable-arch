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
    { Menu: Menu.Model
      Index: Pages.Index.Model option
      About: Pages.About.Model option
      User: Pages.User.Dispatcher.Model option
    }

    static member Initial =
      { Menu = Menu.Model.Initial(Index)
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


  type Actions
    = NoOp
    | NavigateTo of Route
    | IndexActions of Pages.Index.Actions
    | AboutActions of Pages.About.Actions
    | UserDispatcherAction of Pages.User.Dispatcher.Actions
    | MenuActions of Menu.Actions


  let update model action =
    match action with
    | NavigateTo route ->
      match route with
      | Index ->
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.Index_) Pages.Index.Model.Initial
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
          m', []
      | About ->
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.About_) Pages.About.Model.Initial
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
          m', []
      | User subRoute ->
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.User_) (Pages.User.Dispatcher.Model.Initial(subRoute))
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
          m', []
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
    | NoOp -> model, []

  let view model =
    let pageHtml =
      match model.CurrentPage with
      | Index -> Html.map IndexActions (Pages.Index.view model.SubModels.Index.Value)
      | About -> Html.map AboutActions (Pages.About.view model.SubModels.About.Value)
      | User subRoute -> Html.map UserDispatcherAction (Pages.User.Dispatcher.view model.SubModels.User.Value subRoute)

    let menuHtml =
      Html.map MenuActions (Menu.view model.SubModels.Menu)

    div
      []
      [ menuHtml
        pageHtml
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

  createApp Model.Initial view update Virtualdom.createRender
  |> withStartNodeSelector "#app"
  |> withProducer (routeProducer locationHandler router)
  |> withSubscriber (routeSubscriber locationHandler routerF)
  |> start
  |> ignore
