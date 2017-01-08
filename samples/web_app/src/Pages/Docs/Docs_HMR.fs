namespace WebApp.Pages.Docs

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common
open WebApp

module HMR =

  type Model =
    { Content: string
    }

    static member Initial =
      { Content = ""
      }

  type Actions =
    | NoOp
    | Init
    | SetContent of string
    | NavigateTo of Route

  let update model action =
    match action with
    | NoOp ->
        model, []
    | Init ->
        let message =
          [ fun h ->
              ()
          ]
        model, message
    | SetContent content ->
        { model with Content = content }, []
    | NavigateTo route ->
      let message =
        [ fun h ->
            let url = resolveRoutesToUrl route
            match url with
            | Some u -> location.hash <- u
            | None -> failwith "Cannot be reached. Route should always be resolve"
        ]
      model, message

  let view model =
    div
      []
      [ text "HMR documentation"
      ]

