namespace Herebris

open Fable.Core
open Fable.Import.Browser
open System.Collections.Generic
open Fable.PowerPack

module SimpleDatabase =

  type StoreItem<'T> =
    { Id: int
      Value: 'T
    }

    [<PassGenerics>]
    static member ExtractValue item =
      unbox<'T> item.Value

  type Store<'T> =
    { mutable Rows: StoreItem<'T> list
    }

    static member Create () =
      { Rows = []
      }

  type StoreMeta =
    { Name: string
      mutable Id: int
    }

    static member Create (name) =
      { Name = name
        Id = 0
      }

    member self.NextId =
      let id = self.Id
      self.Id <- self.Id + 1
      id

  type DatabaseRecord =
    { Metadatas: Dictionary<string, StoreMeta>
      Stores: Dictionary<string, Store<obj>>
    }

    static member Create () =
      { Metadatas = new Dictionary<_,_>()
        Stores = new Dictionary<_,_>()
      }

  type Database() =
    let db = DatabaseRecord.Create()

    [<PassGenerics>]
    member self.CreateStore<'S> () =
      let storeName = typeof<'S>.Name
      if not (db.Metadatas.ContainsKey(storeName)) then
        db.Metadatas.Add(storeName, StoreMeta.Create(storeName))
        db.Stores.Add(storeName, Store<'S>.Create())

    [<PassGenerics>]
    member self.AddItem<'S>(item: 'S) =
      let storeName = typeof<'S>.Name
      try
        db.Stores.[storeName].Rows <-
          { Id = db.Metadatas.[storeName].NextId
            Value = item
          } :: db.Stores.[storeName].Rows
      with
        | _ -> failwithf "Store: %s not found" storeName

    [<PassGenerics>]
    member self.Get<'S>(id) =
      let storeName = typeof<'S>.Name
      try
        db.Stores.[storeName].Rows
        |> List.find(fun x -> x.Id = id)
        |> StoreItem<'S>.ExtractValue
      with
        | _ -> failwithf "Store: %s not found" storeName

    [<PassGenerics>]
    member self.GetItems<'S>(action) =
      let storeName = typeof<'S>.Name
      try
        db.Stores.[storeName].Rows
        |> List.filter(fun x -> unbox action)
        |> List.map(fun x -> x.Value)
      with
        | _ -> failwithf "Store: %s not found" storeName

    [<PassGenerics>]
    member self.GetAll<'S>() =
      let storeName = typeof<'S>.Name
      Promise.create(
        fun cont econt ->
          try
            cont(
              db.Stores.[storeName].Rows
              |> List.map(fun x ->
                StoreItem<'S>.ExtractValue x
              )
            )
          with
            | _ -> econt(exn(sprintf "Store: %s not found" storeName))
      )

    member self.Console() =
      printfn "%A" self
      console.log db

