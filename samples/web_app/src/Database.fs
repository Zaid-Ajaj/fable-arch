namespace WebApp

open Fable.Core
open Fable.Import.Browser
open System

open Herebris.SimpleDatabase
open Fable.Core.JsInterop
open Fable.PowerPack

module Database =
  type Gender =
    | Male
    | Female

  type User =
    { Firstname: string
      Surname: string
      Age: int
      Email: string
      Gender: Gender
    }

  module Setup =

    let patients =
      [
        { Firstname = "John"
          Surname = "Doe"
          Age = 24
          Email = "john.doe@mail.com"
          Gender = Male
        }
        { Firstname = "Jane"
          Surname = "Doe"
          Age = 27
          Email = "jane.doe@mail.com"
          Gender = Female
        }
        { Firstname = "Barry"
          Surname = "Magium"
          Email = "barry.magium@mail.com"
          Age = 24
          Gender = Male
        }
      ]

  let main () =
    let db = new Database()
    db.CreateStore<User>()

    Setup.patients
    |> List.iter( fun x ->
      db.AddItem<User> x
    )

    let test = db.GetItems<User>(fun x ->
      x.Age = 24
    )

    //printf "%A" test

    promise {
      return! db.GetAll<User>()
    }
    |> Promise.map(fun x ->
      console.log x
    )
    |> Promise.catch(fun x ->
      console.log x
    )
    |> ignore

    //console.log (db.Get<User>(1))

    //db.Console()zdz
