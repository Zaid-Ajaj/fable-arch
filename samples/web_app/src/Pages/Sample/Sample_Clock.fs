namespace WebApp.Pages.Sample

open Fable.Core
open Fable.Import

open Fable.Arch
open Fable.Arch.Html
open Fable.PowerPack
open Fable.PowerPack.Fetch

open WebApp

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
      [ classy "content" ]
      [ h1
          [ classy "is-marginless" ]
          [ text (sprintf "%s %s" model.Date model.Time )]
      ]

  let mutable docs = ""

  fetch (DocGen.createSampleURL __SOURCE_FILE__) []
  |> Promise.bind(fun res ->
    res.text()
  )
  |> Promise.map(fun text ->
    docs <-
      DocGen.generateDocumentation text
      |> Marked.Globals.marked.parse
  )
  |> ignore


  /// Our application view
  let view model =

    div
      [ classy "section" ]
      [ div
          [ classy "content" ]
          [ h1
              []
              [ text "Clock sample" ]
          ]
        div
          [ classy "columns" ]
          [ div
              [ classy "column is-half is-offset-one-quarter has-text-centered" ]
              [ sampleDemo model ]
          ]
        div
          [ classy "content"
            property "innerHTML" docs
          ]
          []
      ]

  (*
  [BeginDocs]

  # ClockSample

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


  ## Producer sample
  ```fsharp
  let update2 model action =
    let model, action =
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
    "test"
  ```



  ## Producer sample
  ```fsharp
  let tickProducer push =
    window.setInterval((fun _ ->
        push(SampleDispatcherAction (Pages.Sample.Dispatcher.ClockActions (Pages.Sample.Clock.Tick DateTime.Now)))
        null
    ),
        1000) |> ignore
    // Force the first to push to have immediate effect
    // If we don't do that there is one second before the first push
    // and the view is rendered with the Model.init values
    push(SampleDispatcherAction (Pages.Sample.Dispatcher.ClockActions (Pages.Sample.Clock.Tick DateTime.Now)))
  let update model action =
    let model, action =
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
    "test"
  ```

  [EndDocs]
  *)
