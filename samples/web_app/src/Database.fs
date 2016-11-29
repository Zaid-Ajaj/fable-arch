namespace WebApp

open Fable.Core
open Fable.Import.Browser
open Fable.Core.JsInterop
open System
open Fable.Helpers.IndexedStorage

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

  type Storage() =
    interface DBImplementation with
      member self.Version = 1
      member self.Upgrade (db: DBUpgrade) =
        let userStore = db.createStore<User>(AutoIncrement)

        // Populate some data
        Setup.patients
        |> List.iter(fun x ->
          userStore.add x
          |> ignore
        )

  let openDb () =
    let db = new IndexedDB<Storage>()

    let user =
      { Firstname = "Maxime"
        Surname = "Mangel"
        Email = "mangel.maxime@outlook.com"
        Age = 24
        Gender = Female
      }

    Async.StartWithContinuations(
      db.``use store read write 1``<User, obj>(fun store ->
        store.addAsync(user)
      ),
      (fun result ->
        console.log result
      ),
      (fun exn ->
        console.error exn.Message
      ),
      (fun _ -> ())
    )




//
//  let mutable db : IDBDatabase = null
//
//  let simpleLog (req: IDBRequest) =
//    req.onerror <- (fun ev ->
//      console.error (sprintf "Error: %A" ev.target?error)
//      null
//    )
//    req.onsuccess <- (fun ev ->
//      console.log ("Success")
//      null
//    )
//
//  let openDb (cb: unit -> unit) () =
//    console.log "openDb..."
//
//    let test =
//      async {
//        let! test = DBFactory.OpenAsync("FableArch")
//        return "maxime"
//      }
//
//    console.log test

//    let req = indexedDB.``open``("FableArch")
//    req.onsuccess <- (fun ev ->
//      db <- unbox ev.target?result
//      cb ()
//      null
//    )
//    req.onerror <- (fun ev ->
//      console.error (sprintf "openDb: %A" ev.target?errorCode)
//      null
//    )

//    req.onupgradeneeded <- (fun ev ->
//      if ev.newVersion = 1. then
//          let db = extractDb ev
//          let store = db.createObjectStore<User>(AutoIncrement)
//
//          store.createIndex("Email_Unique", U2.Case1 "Email", true)
//          |> ignore
//
//          Setup.patients
//          |> List.iter(fun x ->
//            console.log x.Gender
//            store.add(x)
//            |> simpleLog
//          )
//      null
//    )

//  let selectUserById id =
//    let transaction = db.transaction(U2.Case1("User"), "readonly")
//    let store = transaction.objectStore("User")
//
//    let ob = store.get(id)
//    ob.onsuccess <- (fun ev ->
//      let user = unbox<User> (unbox<IDBOpenDBRequest> ev.target).result
//      console.log user.Gender
//      console.log user.Age
//      null
//    )

