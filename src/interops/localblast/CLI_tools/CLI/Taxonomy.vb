#Region "Microsoft.VisualBasic::1dda19bc49308327b727fbadfa672225, ..\localblast\CLI_tools\CLI\Taxonomy.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Metagenomics

Partial Module CLI

    <ExportAPI("/ref.gi.list", Usage:="/ref.gi.list /in <blastnMaps.csv/DIR> [/out <out.csv>]")>
    <Group(CLIGrouping.TaxonomyTools)>
    Public Function GiList(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".gi.list.txt")
        Dim list$() = RequestData(Of BlastnMapping)(handle:=[in]) _
            .Select(Function(x) Regex.Match(x.Reference, "gi\|\d+") _
            .Value _
            .Split("|"c) _
            .Last) _
            .Distinct _
            .ToArray

        Return list.FlushAllLines(out).CLICode
    End Function

    <ExportAPI("/ref.acc.list", Usage:="/ref.acc.list /in <blastnMaps.csv/DIR> [/out <out.csv>]")>
    <Group(CLIGrouping.TaxonomyTools)>
    Public Function AccessionList(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".accid.list.txt")
        Dim list$() = RequestData(Of BlastnMapping)(handle:=[in]) _
            .Select(Function(x) x.Reference _
            .Split(" "c).First) _
            .Distinct _
            .ToArray

        Return list.FlushAllLines(out).CLICode
    End Function

    <ExportAPI("/Reads.OTU.Taxonomy",
               Info:="If the blastnmapping data have the duplicated OTU tags, then this function will makes a copy of the duplicated OTU tag data. top-best data will not.",
               Usage:="/Reads.OTU.Taxonomy /in <blastnMaps.csv> /OTU <OTU_data.csv> /tax <taxonomy:nodes/names> [/fill.empty /out <out.csv>]")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(BlastnMapping)},
              Description:="This input data should have a column named ``taxid`` for the taxonomy information.")>
    <Argument("/fill.empty", True, AcceptTypes:={GetType(Boolean)},
              Description:="If this options is true, then this function will only fill the rows which have an empty ``Taxonomy`` field column.")>
    <Argument("/OTU", False, AcceptTypes:={GetType(OTUData)})>
    <Group(CLIGrouping.TaxonomyTools)>
    Public Function ReadsOTU_Taxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim OTU As String = args("/OTU")
        Dim tax As String = args("/tax")
        Dim fillEmpty As Boolean = args.GetBoolean("/fill.empty")
        Dim out As String = args.GetValue(
            "/out",
            [in].TrimSuffix & "-" & OTU.BaseName & $".Taxonomy{If(fillEmpty, ".fillEmpty", "")}.csv")
        Dim maps = [in].LoadCsv(Of BlastnMapping)
        Dim data = OTU.LoadCsv(Of OTUData)
        Dim taxonomy As New NcbiTaxonomyTree(tax)
        Dim readsTable = (From x As BlastnMapping
                          In maps
                          Select x
                          Group x By x.ReadQuery Into Group) _
            .ToDictionary(Function(x) x.ReadQuery,
                          Function(x) x.Group.ToArray)
        Dim output As New List(Of OTUData)
        Dim diff As New List(Of String)

        For Each r As OTUData In data
            If fillEmpty Then
                If Not String.IsNullOrEmpty(r.Data.TryGetValue("Taxonomy")) Then
                    ' 包含有这个值，则不进行关联，直接添加进入输出数据之中
                    output += r
                    Continue For
                End If
            End If

                Dim reads As BlastnMapping() = readsTable.TryGetValue(r.OTU)

            If reads.IsNullOrEmpty Then
                ' 由于可能是从所有的数据data之中匹配部分maps的数据，所以肯定会出现找不到的对象，在这里记录下来就行了，不需要报错
                diff += r.OTU
                Continue For
            End If

            For Each o As BlastnMapping In reads
                Dim copy As New OTUData(r)
                Dim taxid% = CInt(o.Extensions("taxid"))
                Dim nodes = taxonomy.GetAscendantsWithRanksAndNames(taxid, True)
                copy.Data("taxid") = taxid
                copy.Data("Taxonomy") = TaxonomyNode.BuildBIOM(nodes)
                copy.Data("Reference") = o.Reference
                copy.Data("gi") = Regex.Match(o.Reference, "gi\|\d+").Value

                output += copy
            Next
        Next

        Call App.LogException(diff.GetJson)  ' 将缺失的记录下来

        Return output.SaveTo(out).CLICode
    End Function
End Module
