namespace WebApp

open Fable.Core
open Fable.Import.Browser

open System

module DocGen =

  let sampleSourceDirectory = "src/Pages/Sample"

  let createSampleURL file =
    sprintf "/%s/%s" sampleSourceDirectory file


  type ContentGroup =
    { Key: string
      Value: string
    }

    static member Create (key, value)=
      { Key = key
        Value = value
      }

  type CaptureState =
    | Nothing
    | Content
    | Group of string

  type ParserResult =
    { Text: string
      Groups: ContentGroup list
      CaptureState: CaptureState
      Offset: int
    }

    static member Initial =
      { Text = ""
        Groups = []
        CaptureState = Nothing
        Offset = 0
      }

  let (|Contains|_|) (p:string) (s:string) =
    if s.Contains(p) then
      Some s
    else
      None

  let (|Prefix|_|) (p:string) (s:string) =
    if s.StartsWith(p) then
      Some s
    else
      None

  let rec countStart (chars: char list) index =
    match chars with
    | x::xs ->
        if x = ' ' then
          countStart xs (index + 1)
        else
          index
    | [] ->
        index

  let generateDocumentation (text: string) =
    let lines = text.Split('\n') |> Array.toList

    let rec parseSample lines result =
      match lines with
      | line::xs ->
          let newResult =
            match line with
            | Contains "[BeginDocs]" line ->
              { result with
                  CaptureState = Content
                  Offset = countStart (line.ToCharArray() |> Array.toList) 0
              }
            | Contains "[EndDocs]" line ->
              { result with
                  CaptureState = Nothing
                  Offset = 0
              }
            | line ->
                match result.CaptureState with
                | Content ->
                    { result with
                        Text = sprintf "%s\n%s" result.Text (line.Substring(result.Offset))
                    }
                | Group key -> result
                | Nothing -> result
          parseSample xs newResult
      | [] -> result

    let result = parseSample lines ParserResult.Initial
    console.log result.Text
    result.Text
