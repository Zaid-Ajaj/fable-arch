namespace WebApp.Pages.Sample

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common

module Dispatcher =

  type Model =
    { Clock: Clock.Model option
    }

    static member Generate (?index) =
      { Clock = index
      }

    static member Initial (currentPage: SampleApi.Route) =
      match currentPage with
      | SampleApi.Clock -> Model.Generate (index = Clock.Model.Initial)

  type NavbarLink =
    { Text: string
      Route: SampleApi.Route
    }

    static member Create (text, route) =
      { Text = text
        Route = route
      }

  type Actions
    = ClockActions of Clock.Actions
    | NavigateTo of SampleApi.Route

  let update model action =
    match action with
    | ClockActions act ->
        let (res, action) = Clock.update model.Clock.Value act
        let action' = mapActions ClockActions action
        { model with Clock = Some res}, action'
    | NavigateTo route ->
        let message =
          [ fun h ->
              let url = resolveRoutesToUrl (Sample route)
              match url with
              | Some u -> location.hash <- u
              | None -> failwith "Cannot be reached. Route should always be resolve"
          ]
        model, message


  let navItem item currentPage =
    a
      [ classList
          [ "is-active", item.Route = currentPage
            "nav-item is-tab", true
          ]
        voidLinkAction<Actions>
        onMouseClick(fun _ ->
          NavigateTo item.Route
        )
      ]
      [ text item.Text ]


  let navbar items currentPage =
    div
      [ classy "nav-left" ]
      (items |> List.map(fun item -> navItem item currentPage))


  let view model subRoute =
    let htmlContent =
      match subRoute with
      | SampleApi.Clock ->
          Html.map ClockActions (Clock.view model.Clock.Value)

    div
      []
      [ nav
          [ classy "nav has-shadow" ]
          [ div
              [ classy "container" ]
              [ navbar
                  [ NavbarLink.Create("Clock", SampleApi.Clock)
                    NavbarLink.Create("Counter", SampleApi.Counter)
                  ]
                  subRoute
              ]
          ]
        htmlContent
      ]
