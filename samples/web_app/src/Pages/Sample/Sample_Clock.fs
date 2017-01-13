﻿namespace WebApp.Pages.Sample

open Fable.Core
open Fable.Import

open Fable.Arch
open Fable.Arch.Html
open Fable.PowerPack
open Fable.PowerPack.Fetch

open WebApp
open WebApp.Common

open System

module Clock =

  /// Make sure that number have a minimal representation of 2 digits
  let normalizeNumber x =
    if x < 10 then
      sprintf "0%i" x
    else
      string x

  type Actions =
    | Tick of DateTime

  /// [BeginBlock:Model]
  /// A really simple type to Store our ModelChanged
  type Model =
    { Time: string      // Time: HH:mm:ss
      Date: string }    // Date: YYYY/MM/DD

    /// Static member giving back an init Model
    static member Initial =
      { Time = "00:00:00"
        Date = "01/01/1970" }
  /// [EndBlock]


  /// A really simple type to Store our ModelChanged
  /// Handle all the update of our Application
  /// [BeginBlock:Update]
  let update model action =
    let model', action' =
      match action with
      /// Tick are push by the producer
      | Tick datetime ->
        // Normalize the day and month to ensure a 2 digit representation
        let day = datetime.Day |> normalizeNumber
        let month = datetime.Month |> normalizeNumber
        // Create our date string
        let date = sprintf "%s/%s/%i" month day datetime.Year
        { model with
            Time = String.Format("{0:HH:mm:ss}", datetime)
            Date = date }, []
    model', action'
  /// [EndBlock]

  let sampleDemo model =
    div
      [ classy "content has-text-centered" ]
      [ h1
          [ classy "is-marginless" ]
          [ text (sprintf "%s %s" model.Date model.Time )]
      ]

  let docs = new DocGen.Documentation(__SOURCE_FILE__)

  /// Our application view
  let view model =
    VDom.Html.sampleView "Clock sample" (sampleDemo model) docs.Html

  (*
  [BeginDocs]

  This sample is a simple sample to show you how to use producers.
  A producer, is used to push a message into your application from the outside world of the application.

  In this sample, the producer is used to push a `Tick` actions every seconds.

  ```fsharp
  /// Comments
  let maxime = "klpo"
  ```

  ## Producer sample
  ```fsharp
  [Block:Update]
  ```

  [EndDocs]
  *)
