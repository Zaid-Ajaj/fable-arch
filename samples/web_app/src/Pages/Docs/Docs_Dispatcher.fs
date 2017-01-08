namespace WebApp.Pages.Docs

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common

module Dispatcher =

  type Model =
    { Index: Index.Model option
      HMR: HMR.Model option
    }

    static member Generate (?index, ?hmr) =
      { Index = index
        HMR = hmr
      }

    static member Initial (currentPage: DocsApi.Route) =
      match currentPage with
      | DocsApi.Index -> Model.Generate (index = Index.Model.Initial)
      | DocsApi.HMR -> Model.Generate (hmr = HMR.Model.Initial)


  type Actions
    = IndexActions of Index.Actions
    | HMRActions of HMR.Actions

  let update model action =
    match action with
    | IndexActions act ->
        let (res, action) = Index.update model.Index.Value act
        let action' = mapActions IndexActions action
        { model with Index = Some res}, action'
    | HMRActions act ->
        let (res, action) = HMR.update model.HMR.Value act
        let action' = mapActions HMRActions action
        { model with HMR = Some res}, action'


  let view model subRoute =
    match subRoute with
    | DocsApi.Index ->
        Html.map IndexActions (Index.view model.Index.Value)
    | DocsApi.HMR ->
        Html.map HMRActions (HMR.view model.HMR.Value)
