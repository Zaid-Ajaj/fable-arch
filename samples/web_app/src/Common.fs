namespace WebApp

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
