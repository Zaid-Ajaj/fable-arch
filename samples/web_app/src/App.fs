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
    { Index: Pages.Index.Model option
      About: Pages.About.Model option
      User: Pages.User.Dispatcher.Model option
    }

    static member Generate (?index, ?about, ?user) =
      { Index = index
        About = about
        User = user
      }

    static member Index_ =
      (fun r -> r.Index), (fun v r -> { r with Index = Some v } )

    static member About_ =
      (fun r -> r.About), (fun v r -> { r with About = Some v } )

    static member User_ =
      (fun r -> r.User), (fun v r -> { r with User = Some v } )

  type Model =
    { CurrentPage: Route
      SubModels: SubModels
    }

    static member Initial =
      { CurrentPage = Index
        SubModels = SubModels.Generate()
      }

    static member SubModels_ =
      (fun r -> r.SubModels), (fun v r -> { r with SubModels = v } )


  type Actions
    = NoOp
    | NavigateTo of Route
    | IndexActions of Pages.Index.Actions
    | AboutActions of Pages.About.Actions
    | UserDispatcherAction of Pages.User.Dispatcher.Actions


  let update model action =
    match action with
    | NavigateTo route ->
      match route with
      | Index ->
          let m' =
            Optic.set (Model.SubModels_ >-> SubModels.Index_) Pages.Index.Model.Initial model
          m', []
      | About ->
          let m' =
            Optic.set (Model.SubModels_ >-> SubModels.About_) Pages.About.Model.Initial model
          m', []
      | User subRoute ->
          let m' =
            Optic.set (Model.SubModels_ >-> SubModels.User_) (Pages.User.Dispatcher.Model.Initial(subRoute)) model
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
    | NoOp -> model, []



  let view model =
    let pageHtml =
      match model.CurrentPage with
      | Index -> Html.map IndexActions (Pages.Index.view model.SubModels.Index.Value)
      | About -> Html.map AboutActions (Pages.About.view model.SubModels.About.Value)
      | User subRoute -> Html.map UserDispatcherAction (Pages.User.Dispatcher.view model.SubModels.User.Value subRoute)

    div
      []
      [ text "coucou"
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
      match r with
      | Index -> Some "/"
      | About -> Some "/about"
      | User api ->
        match api with
        | UserApi.Index -> Some "/users"
        | UserApi.Create -> Some "/user/create"
        | UserApi.Edit id -> Some (sprintf "/user/%i/edit" id)
        | UserApi.Show id -> Some (sprintf "/user/%i" id)
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

  createApp Model.Initial view update Virtualdom.renderer
  |> withStartNodeSelector "#app"
  |> withProducer (routeProducer locationHandler router)
  |> withSubscriber (routeSubscriber locationHandler routerF)
  |> start
  |> ignore
