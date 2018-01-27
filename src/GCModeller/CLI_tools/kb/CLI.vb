#Region "Microsoft.VisualBasic::457c1a15ee29239aec07f205716a51e4, ..\GCModeller\CLI_tools\kb\CLI.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.NLP
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Webservices.Bing
Imports Microsoft.VisualBasic.Webservices.Bing.Academic
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base
Imports BingTranslation = Microsoft.VisualBasic.Webservices.Bing.Translation

Module CLI

    <ExportAPI("/kb.build.query")>
    <Usage("/kb.build.query /term <term> [/pages <default=20> /out <out.directory>]")>
    Public Function BingAcademicQuery(args As CommandLine) As Integer
        Dim term$ = args <= "/term"
        Dim out$ = args("/out") Or (App.CurrentDirectory & "/" & term.NormalizePathString)
        Dim pages% = args.GetValue("/pages", 20)

        Call Academic.Build_KB(term, out, pages, flat:=False)

        Return 0
    End Function

    <ExportAPI("/kb.abstract")>
    <Usage("/kb.abstract /in <kb.directory> [/min.weight <default=0.05> /out <out.json>]")>
    Public Function GetKBAbstractInformation(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim minWeight# = args.GetValue("/min.weight", 0.05)
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.textgraph.weights.json"
        Dim kb As IEnumerable(Of ArticleProfile) = (ls - l - r - "*.xml" <= [in]).Select(AddressOf LoadXml(Of ArticleProfile))
        Dim weights = kb.TextGraphWeights
        Dim abstract = weights.AbstractFilter(minWeight:=minWeight)
        Dim abstractText$ = abstract.Keys.JoinBy(ASCII.LF)

        Call (abstractText & ASCII.LF & ASCII.LF & abstract.GetJson(indent:=True)) _
            .SaveTo(out.TrimSuffix & $".abstract(min_weight={minWeight}).txt")

        Return weights _
            .GetJson(indent:=True) _
            .SaveTo(out, TextEncodings.UTF8WithoutBOM) _
            .CLICode
    End Function

    <ExportAPI("/summary")>
    <Usage("/summary /in <directory> [/out <out.csv>]")>
    Public Function Summary(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or ([in].TrimDIR & ".summary.csv")
        Dim articles As Summary() = (ls - l - r - "*.xml" <= [in]) _
            .Select(AddressOf LoadXml(Of ArticleProfile)) _
            .Summary

        Return articles _
            .SaveTo(out, encoding:=Encoding.UTF8) _
            .CLICode
    End Function

    <ExportAPI("/word.translation")>
    <Usage("/word.translation /in <list_words.txt> [/out <translation.csv> /@set sleep=2000]")>
    Public Function WordTranslation(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.translation.csv"
        Dim sleep% = App.GetVariable("sleep") Or "2000".AsDefault
        Dim translation As New List(Of WordTranslation)

        For Each word As String In [in].IterateAllLines
            With BingTranslation.GetTranslation(word)
                If Not .IsNothing Then
                    Call .GetXml.SaveTo(out.ParentPath & $"/{word.NormalizePathString}.Xml")
                    Call translation.Add(.ref)
                Else
                    Call word.Warning
                End If
            End With

            Call Thread.Sleep(sleep)
        Next

        Return translation _
            .SaveTo(out) _
            .CLICode
    End Function
End Module
