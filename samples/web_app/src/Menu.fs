namespace WebApp

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common

module Menu =

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

  type MenuLink =
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
        [ fun h ->
            let url = resolveRoutesToUrl route
            match url with
            | Some u -> location.hash <- u
            | None -> failwith "Cannot be reached. Route should always be resolve"
        ]
      model, message

  let menuListItem menuLink currentPage =
    let class' =
      classBaseList
        "pure-menu-item"
        [ "pure-menu-selected", menuLink.Route = currentPage
        ]
    li
      [ class'  ]
      [ a
          [ classy "pure-menu-link"
            voidLinkAction<Actions>
            onMouseClick (fun _ ->
              NavigateTo menuLink.Route
            )
          ]
          [ text menuLink.Text]
      ]

  let menuList menuLink items currentPage =
    let class' =
      classBaseList
        "pure-menu-item pure-menu-has-children pure-menu-allow-hover"
        [ "pure-menu-selected", menuLink.Route = currentPage
        ]
    li
      [ class' ]
      [ a
          [ classy "pure-menu-link"
            voidLinkAction<Actions>
            onMouseClick (fun _ ->
              NavigateTo menuLink.Route
            )
          ]
          [ text menuLink.Text ]
        ul
          [ classy "pure-menu-children" ]
          (items |> List.map(fun x -> menuListItem x currentPage))
      ]

  let view model =
    div
      [ classy "header black-bg" ]
      [ div
          [ classy "pure-menu pure-menu-horizontal" ]
          [ a
              [ classy "pure-menu-heading pure-menu-link"
                voidLinkAction<Actions>
              ]
              [ text "Fable-Arch : WebApp" ]
            ul
              [ classy "pure-menu-list" ]
              [ menuListItem (MenuLink.Create("Home", Route.Index)) model.CurrentPage
                menuList
                  (MenuLink.Create("Users", (Route.User UserApi.Index)))
                  [ MenuLink.Create("Index", (Route.User UserApi.Index))
                    MenuLink.Create("Create", (Route.User UserApi.Create))
                  ]
                  model.CurrentPage
                menuListItem (MenuLink.Create("About", Route.About)) model.CurrentPage
              ]
          ]
      ]
