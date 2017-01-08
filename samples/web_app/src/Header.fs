namespace WebApp

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common

module Header =

  type Model =
    { CurrentPage: Route
    }

    static member Initial (page) =
      { CurrentPage = page
      }

    static member CurrentPage_ =
      (fun r -> r.CurrentPage), (fun v r -> { r with CurrentPage = v })

  type Actions
    = NoOp
    | NavigateTo of Route

  type HeroLink =
    { Text: string
      Route: Route
    }

    static member Create (text, route) =
      { Text = text
        Route = route
      }

  let update model action =
    match action with
    | NoOp ->
      model, []
    | NavigateTo route ->
      let message =
        [ fun h ->
            let url = resolveRoutesToUrl route
            match url with
            | Some u -> location.hash <- u
            | None -> failwith "Cannot be reached. Route should always be resolve"
        ]
      model, message

  let footerLinkItem menuLink currentPage =
    li
      [ classList
          [ "is-active", menuLink.Route = currentPage
          ]
      ]
      [ a
          [ voidLinkAction<Actions>
            onMouseClick (fun _ ->
              NavigateTo menuLink.Route
            )]
          [ text menuLink.Text]
      ]

  let footerLinks items currentPage =
      ul
        []
        (items |> List.map(fun x -> footerLinkItem x currentPage))

  let footer model =
    div
      [ classy "hero-foot" ]
      [ div
          [ classy "container" ]
          [ nav
              [ classy "tabs is-boxed" ]
              [ footerLinks
                  [ HeroLink.Create("Home", Route.Index)
                    HeroLink.Create("Docs", (Route.Docs DocsApi.Index))
                  ]
                  model.CurrentPage
              ]
          ]
      ]

  let view model =
    section
      [ classy "hero is-primary" ]
      [ div
          [ classy "hero-body" ]
          [ div
              [ classy "container" ]
              [ h1
                  [ classy "title" ]
                  [ text "Documentation"]
                h2
                  [ classy "subtitle"
                    property "innerHTML" "Everything you need to create a website using <strong>Fable Arch<strong>"
                  ]
                  []
              ]
          ]
        footer model
      ]
