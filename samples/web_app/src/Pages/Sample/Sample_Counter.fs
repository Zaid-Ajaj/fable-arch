namespace WebApp.Pages.Sample

open Fable.Core
open Fable.Import
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common
open WebApp

open System

module Counter =

  console.log __SOURCE_DIRECTORY__
  console.log __SOURCE_FILE__


  type Actions
      = Add
      | Sub
      | Reset

  /// A really simple type to Store our ModelChanged
  type Model =
      { Value: int
      }

      /// Static member giving back an init Model
      static member Initial =
        { Value = 0 }

  /// Handle all the update of our Application
  let update model action =
    match action with
    | Add ->
        { model with Value = model.Value + 1 }, []
    | Sub ->
        { model with Value = model.Value - 1 }, []
    | Reset ->
        { model with Value = 0 }, []


  let simpleButton txt action =
    div
      [ classy "column is-narrow" ]
      [ a
          [ classy "button"
            voidLinkAction<Actions>
            onMouseClick(fun _ ->
              action
            )
          ]
          [ text txt ]
      ]

  let sampleDemo model =
    div
      [ classy "columns is-vcentered" ]
      [
        div
          [ classy "column is-narrow"
            Style [ "width", "170px" ]
          ]
          [ text (sprintf "Counter value: %i" model.Value) ]
        simpleButton "+1" Add
        simpleButton "-1" Sub
        simpleButton "Reset" Reset
      ]


  let sampleText =
    "
```fs

```
    "

  /// Our application view
  let view model =
    div
      [ classy "section" ]
      [ div
          [ classy "content" ]
          [ h1
              []
              [ text "Counter sample" ]
          ]
        div
          [ classy "columns" ]
          [ div
              [ classy "column is-half is-offset-one-quarter has-text-centered" ]
              [ div
                  [ classy "columns is-vcentered" ]
                  [ div [ classy "column" ] []
                    sampleDemo model
                    div [ classy "column" ] []
                  ]
              ]
          ]
        div
          [ classy "content"
            property "innerHTML" (Marked.Globals.marked.parse(sampleText))
          ]
          []
      ]
