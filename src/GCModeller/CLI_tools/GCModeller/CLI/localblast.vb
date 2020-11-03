#Region "Microsoft.VisualBasic::0ddb4c90624334d339addf73d5926d1f, CLI_tools\GCModeller\CLI\localblast.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    ' Module CLI
    ' 
    '     Function: mapFileData, MapHits, MapHitsTaxonomy
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping

Partial Module CLI

    <ExportAPI("/Map.Hits",
               Usage:="/Map.Hits /in <query.csv> /mapping <blastnMapping.csv> [/split.Samples /sample.Name <filedName,default:=track> /out <out.csv>]")>
    <Group(CLIGrouping.LocalblastTools)>
    Public Function MapHits(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim mapping As String = args("/mapping")
        Dim out As String = args _
            .GetValue("/out", [in].TrimSuffix & "-" & mapping.BaseName & "_MapHits.csv")
        Dim exps = [in].LoadCsv(Of QueryArgument)
        Dim expList = (From x As QueryArgument
                       In exps
                       Select x,
                           query = x.Expression.Build(allowInStr:=False, anyDefault:=Tokens.op_AND),
                           bufs = New List(Of String)).ToArray
        Dim hits As New Dictionary(Of String, List(Of String))  ' maphits列表
        Dim splitSamples As Boolean = args.GetBoolean("/split.Samples")
        Dim fileTrack = args.GetValue("/sample.Name", "track")
        Dim expData = exps.ToDictionary

        Const mapHitsPrefix$ = "Map.Hits."

        For Each x In expList
            hits.Add(x.x.Name, x.bufs)
        Next
        For Each hit In mapping.LoadCsv(Of BlastnMapping)
            Dim LQuery = From x
                         In expList.AsParallel
                         Where x.query.Match(hit.Reference)
                         Select x.x.Name

            For Each n As String In LQuery
                Call hits(n).Add(hit.ReadQuery)

                If splitSamples Then
                    Dim name$ = hit.Extensions.TryGetValue(fileTrack)
                    name$ = mapHitsPrefix & name

                    If Not expData(n$).Data.ContainsKey(name) Then
                        Call expData(n$).Data.Add(name, "")
                    End If
                    expData(n$).Data(name) =
                        expData(n$).Data(name) & "; " & hit.ReadQuery
                End If
            Next
        Next

        For Each x In expList
            x.x.Data.Add("MapHits",
                x.bufs _
                .Distinct _
                .OrderBy(Function(s) s) _
                .JoinBy("; "))
        Next
        For Each x In exps
            For Each k$ In x.Data.Keys _
                .Where(Function(s) InStr(s, mapHitsPrefix) > 0) _
                .ToArray ' 避免出现错误: Collection was modified; enumeration operation may not execute.

                x.Data(k$) = x.Data(k$) _
                    .Split(";"c) _
                    .Select(AddressOf Trim) _
                    .Where(Function(s) Not String.IsNullOrEmpty(s)) _
                    .Distinct _
                    .JoinBy("; ")
            Next
        Next

        Return exps.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Map.Hits.Taxonomy",
               Usage:="/Map.Hits.Taxonomy /in <query.csv> /mapping <blastnMapping.csv/DIR> /tax <taxonomy.DIR:name/nodes> [/out <out.csv>]")>
    <ArgumentAttribute("/mapping", True,
              AcceptTypes:={GetType(BlastnMapping)},
              Description:="Data frame should have a ``taxid`` field.")>
    <Group(CLIGrouping.LocalblastTools)>
    Public Function MapHitsTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim mapping As String = args("/mapping")
        Dim out$
        Dim taxonomy As New NcbiTaxonomyTree(args("/tax"))

        If mapping.FileExists Then
            out = args.GetValue("/out", [in].TrimSuffix & "-" & mapping.BaseName & "_MapHits.csv")
        Else
            out = args.GetValue("/out", [in].TrimSuffix & "-" & mapping.BaseName & "_MapHits/")
        End If

        If mapping.FileExists Then
            Return mapping.mapFileData([in], taxonomy, out)
        Else
            For Each file$ In ls - l - r - "*.csv" <= mapping
                Dim save$ = out & "/" & [in].BaseName & "-" & file.BaseName & "_maphits.csv"
                Call file.mapFileData([in], taxonomy, save)
            Next
        End If

        Return 0
    End Function

    <Extension>
    Private Function mapFileData(mapping$, in$, taxonomy As NcbiTaxonomyTree, out$) As Integer
        Dim exps = [in].LoadCsv(Of QueryArgument)
        Dim expList = (From x As QueryArgument
                       In exps
                       Select x,
                           query = x.Expression.Build(allowInStr:=False, anyDefault:=Tokens.op_AND),
                           bufs = New Dictionary(Of Integer, List(Of String))).ToArray
        Dim hits As New Dictionary(Of String, Dictionary(Of Integer, List(Of String)))

        For Each x In expList
            Call hits.Add(x.x.Name, x.bufs)
        Next
        For Each hit In mapping.LoadCsv(Of BlastnMapping)
            Dim taxid% = CInt(Val(hit.Extensions("taxid")))
            Dim LQuery = From x
                         In expList.AsParallel
                         Where x.query.Match(hit.Reference)
                         Select x.x.Name

            For Each n As String In LQuery
                If Not hits(n).ContainsKey(taxid) Then
                    hits(n).Add(taxid, New List(Of String))
                End If
                Call hits(n)(taxid).Add(hit.ReadQuery)
            Next
        Next

        Dim output As New List(Of QueryArgument)

        For Each x In expList
            Dim data = x.bufs

            For Each taxid In data
                Dim nodes = taxonomy.GetAscendantsWithRanksAndNames(taxid.Key, True)
                Dim tree$ = nodes.BuildBIOM

                output += New QueryArgument With {
                    .Name = x.x.Name,
                    .Expression = x.x.Expression,
                    .Data = New Dictionary(Of String, String) From {
                        {"taxid", taxid.Key},
                        {"MapHits", taxid.Value.Distinct.JoinBy("; ")},
                        {"Taxonomy.Name", taxonomy(taxid.Key).name},
                        {"Taxonomy", tree}
                    }
                }
            Next
        Next

        Return output.SaveTo(out).CLICode
    End Function
End Module
