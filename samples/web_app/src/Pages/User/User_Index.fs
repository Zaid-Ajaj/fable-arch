namespace WebApp.Pages.User

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common
open WebApp

module Index =

  type Model =
    { Users: UserRecord list
    }

    static member Initial =
      { Users = []
      }

  type Actions
    = NoOp
    | Init
    | SetUsers of UserRecord list

  let update model action =
    match action with
    | NoOp -> model, []
    | Init ->
        let message =
          [ fun h ->
              FakeApi.Get<UserRecord list>
                (FakeApi.Ressources.User FakeApi.UserRes.Index)
                (fun data ->
                  h (SetUsers data)
                )
          ]
        model, message
    | SetUsers users ->
      { model with Users = users }, []

  let header : DomNode<Actions> =
    thead
      []
      [ tr
          []
          [ th
              []
              [ text "Gender" ]
            th
              []
              [ text "Firstname" ]
            th
              []
              [ text "Surname" ]
            th
              []
              [ text "Email" ]
            th
              [ attribute "colspan" "2" ]
              []
          ]
      ]

  let row (item: UserRecord) =
    tr
      []
      [ td
          []
          [text (item.Gender.ToString()) ]
        td
          []
          [ text item.Firstname ]
        td
          []
          [ text item.Surname ]
        td
          []
          [ text item.Email ]
        td
          [ classy "is-icon" ]
          [ a
              [ voidLinkAction<Actions> ]
              [ i
                  [ classy "fa fa-pencil" ]
                  []
              ]
          ]
        td
          [ classy "is-icon" ]
          [ a
              [ voidLinkAction<Actions> ]
              [ i
                  [ classy "fa fa-eye" ]
                  []
              ]
          ]
      ]

  let body users =
    tbody
      []
      (users |> List.map row)

  let view model =
    table
      [ classy "table" ]
      [ header
        body model.Users
      ]
