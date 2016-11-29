namespace Fable.Helpers

open Fable.Core
open Fable.Import.Browser
open Fable.Core.JsInterop

module IndexedStorage =
  let extractDb (ev: IDBVersionChangeEvent) =
    let currentTarget = unbox<IDBOpenDBRequest> ev.currentTarget
    unbox<IDBDatabase> currentTarget.result

  type DBKeyMethod =
    | KeyPath of string
    | AutoIncrement

  type DBUpgrade =
    [<PassGenerics>]
    member db.deleteStore<'T>() =
      let storeName = typeof<'T>.Name
      (unbox db: IDBDatabase).deleteObjectStore(storeName)

    [<PassGenerics>]
    member db.createStore<'T> (keyMethods: DBKeyMethod) =
      let args = createEmpty<IDBObjectStoreParameters>

      match keyMethods with
      | KeyPath key ->
          args.keyPath <- Some (U2.Case1 key)
      | AutoIncrement ->
          args.autoIncrement <- Some true

      let storeName = typeof<'T>.Name
      (unbox db: IDBDatabase).createObjectStore(storeName, unbox args)

  type DBImplementation =
    abstract member Version: int
    abstract member Upgrade: DBUpgrade -> unit

  type IDBObjectStore with
    member x.createIndex(name: string, keyPath: U2<string, ResizeArray<string>>, ?unique: bool) =
      let args = createEmpty<IDBIndexParameters>
      args.unique <- Some (defaultArg unique false)
      (unbox x: IDBObjectStore).createIndex(name, keyPath, args)

  type DBStore<'T> =
    member internal x.original with get() = unbox<IDBObjectStore> x

    member x.getAsync(key: obj) =
      Async.FromContinuations(fun (cont, econt, _) ->
        let request = x.original.get(key)
        request.onerror <- fun _ -> box(econt(exn request.error.name))
        request.onsuccess <- fun _ -> box(cont(unbox<'T> request.result))
      )

  type DBStoreRW<'T> =
    inherit DBStore<'T>

    member x.Add(item: 'T) =
      ignore(x.original.add(item))

    member x.addAsync(item: 'T) =
      Async.FromContinuations(fun (cont, econt, _) ->
        let request = x.original.add(item)
        request.onerror <- fun _ -> box(econt(exn request.error.name))
        request.onsuccess <- fun _ -> box(cont(request.result))
      )

  type IndexedDB<'T when 'T :> DBImplementation and 'T : (new: unit->'T)>() =
    [<PassGenerics>]
    member private x.useAsync (mkTransaction: IDBDatabase->IDBTransaction) (execTransaction: IDBTransaction->Async<'Result>) =
      Async.FromContinuations(fun (cont, econt, _) ->
        let impl = new 'T() :> DBImplementation
        let name = typeof<'T>.Name
        let request = indexedDB.``open``(name, float impl.Version)
        request.onerror <- fun _ ->
          box(econt(exn request.error.name))
        request.onupgradeneeded <- fun ev ->
          box(
            try
              let db = unbox<IDBDatabase> request.result
              impl.Upgrade(unbox db)
            with
              | e -> econt(e)
          )
        request.onsuccess <- fun _ ->
          Async.StartImmediate(async {
            try
              let db = unbox<IDBDatabase> request.result
              let trans = mkTransaction db
              let! res = execTransaction trans
              trans.oncomplete <- fun _ -> db.close(); box(cont(res))
              trans.onerror <- fun _ -> db.close(); box(econt(exn trans.error.name))
            with
              | e -> econt(e)
          })
          null
      )

    [<PassGenerics>]
    member x.``use store read only 1``<'S1, 'Result>(transaction: DBStore<'S1>->Async<'Result>) =
      let storeName1 = typeof<'S1>.Name
      let mkTransation = fun (db: IDBDatabase) ->
        db.transaction(U2.Case1 storeName1, "readonly")
      let execTransaction = fun (trans: IDBTransaction) ->
        async {
          let store1 = unbox<DBStore<'S1>>(trans.objectStore(storeName1))
          return! transaction store1
        }
      x.useAsync mkTransation execTransaction

    [<PassGenerics>]
    member x.``use store read write 1``<'S1, 'Result>(transaction: DBStoreRW<'S1>->Async<'Result>) =
      let storeName1 = typeof<'S1>.Name
      let mkTransaction = fun (db: IDBDatabase) ->
        db.transaction(U2.Case1 storeName1, "readwrite")
      let execTransaction =
        fun (trans: IDBTransaction) ->
          async {
            let store1 = unbox<DBStoreRW<'S1>>(trans.objectStore(storeName1))
            return! transaction store1
          }
      x.useAsync mkTransaction execTransaction
