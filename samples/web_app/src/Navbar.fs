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
            attribute "href" "https://twitter.com/FableCompiler"
            attribute "target" "_blank"
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
            attribute "href" "https://github.com/fable-compiler/fable-arch"
            attribute "target" "_blank"
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
//        div
//          [ classy "nav-right nav-menu" ]
//          [ navItem (NavLink.Create("Home", Route.Index)) model.CurrentPage
//            navItem (NavLink.Create("About", Route.About)) model.CurrentPage
//          ]
        navButton
      ]
