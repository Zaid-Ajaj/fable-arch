namespace WebApp.Pages.Sample

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common
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

  /// A really simple type to Store our ModelChanged
  type Model =
      { Time: string      // Time: HH:mm:ss
        Date: string }    // Date: YYYY/MM/DD

      /// Static member giving back an init Model
      static member Initial =
          { Time = "00:00:00"
            Date = "1970/01/01" }

  /// Handle all the update of our Application
  let update model action =
      let model', action' =
          match action with
          /// Tick are push by the producer
          | Tick datetime ->
              // Normalize the day and month to ensure a 2 digit representation
              let day = datetime.Day |> normalizeNumber
              let month = datetime.Month |> normalizeNumber
              // Create our date string
              let date = sprintf "%i/%s/%s" datetime.Year month day
              { model with
                  Time = String.Format("{0:HH:mm:ss}", datetime)
                  Date = date }, []
      model', action'

  /// Our application view
  let view model =
      div
          []
          [ text model.Date
            br []
            text model.Time]
