namespace WebApp

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common

module Navbar =

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

  type NavLink =
    { Text: string
      Route: Route
    }

    static member Create (text, route) =
      { Text = text
        Route = route
      }

  let update model action =
    match action with
    | NoOp -> model, []
    | NavigateTo route ->
      let message =
        [ fun _ ->
            let url = resolveRoutesToUrl route
            match url with
            | Some u -> location.hash <- u
            | None -> failwith "Cannot be reached. Route should always be resolve"
        ]
      model, message

  let navItem navLink currentPage =
    let class' =
      classBaseList
        "nav-item"
        [ "is-active", navLink.Route = currentPage
        ]
    a
      [ class'
        voidLinkAction<Actions>
        onMouseClick(fun _ ->
          NavigateTo navLink.Route
        )
      ]
      [ text navLink.Text ]

  let navButton =
    span
      [ classy "nav-item"]
      [ a
          [ classy "button"
            property "id" "twitter"
            property "href" "https://twitter.com/FableCompiler"
            property "target" "_blank"
          ]
          [ span
              [ classy "icon" ]
              [ i
                  [ classy "fa fa-twitter" ]
                  []
              ]
            span
              []
              [ text "Twitter" ]
          ]
        a
          [ classy "button"
            property "id" "github"
            property "href" "https://github.com/fable-compiler/fable-arch"
            property "target" "_blank"
          ]
          [ span
              [ classy "icon"]
              [ i
                  [ classy "fa fa-github" ]
                  []
              ]
            span
              []
              [ text "Fork me" ]
          ]
      ]

  let view model =
    nav
      [ classy "nav" ]
      [ div
          [ classy "nav-left" ]
          [ h1
              [ classy "nav-item is-brand title is-4"
                voidLinkAction<Actions>
              ]
              [ text "Fable-Arch"
              ]
          ]
        a
          [ classy "nav-item"
            property "href" "http://fable.io/"
          ]
          [ text "Fable.io" ]
        navButton
      ]
