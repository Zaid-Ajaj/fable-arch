namespace WebApp

open Fable.Arch.Html

module Common =

  [<RequireQualifiedAccess>]
  module UserApi =
    type Route
      = Index
      | Create
      | Edit of int
      | Show of int

  type Route
    = Index
    | User of UserApi.Route
    | About

  let resolveRoutesToUrl r =
    match r with
      | Index -> Some "/"
      | About -> Some "/about"
      | User api ->
        match api with
        | UserApi.Index -> Some "/users"
        | UserApi.Create -> Some "/user/create"
        | UserApi.Edit id -> Some (sprintf "/user/%i/edit" id)
        | UserApi.Show id -> Some (sprintf "/user/%i" id)

  let voidLinkAction<'T> : Attribute<'T> = attribute "href" "javascript:void(0)"

  [<AutoOpen>]
  module Types =
    type Gender =
    | Male
    | Female

    type UserRecord =
      { Firstname: string
        Surname: string
        Age: int
        Email: string
        Gender: Gender
      }
