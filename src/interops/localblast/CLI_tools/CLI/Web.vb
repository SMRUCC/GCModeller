#Region "Microsoft.VisualBasic::a992e756e51d3808693892d174053f40, localblast\CLI_tools\CLI\Web.vb"

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


    ' Code Statistics:

    '   Total Lines: 69
    '    Code Lines: 60 (86.96%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (13.04%)
    '     File Size: 2.78 KB


    ' Module CLI
    ' 
    '     Function: AlignmentTableTopBest, ExportWebAlignmentTable, ParseAlignmentTableGIlist
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast

Partial Module CLI

    <ExportAPI("/Export.AlignmentTable", Usage:="/Export.AlignmentTable /in <alignment.txt> [/split /header.split /out <outDIR/file>]")>
    <Description("Export the web alignment result file as csv table.")>
    <Group(CLIGrouping.WebTools)>
    Public Function ExportWebAlignmentTable(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim isSplit As Boolean = args("/split")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix)
        Dim headerSplit? = args("/header.split")
        Dim tables As IEnumerable(Of AlignmentTable) = [in].IterateTables(headerSplit)

        If isSplit Then
            out = out & "-EXPORT/"

            For Each table In tables
                Call table.Hits _
                    .SaveTo(out & "/" & table.Query & ".csv")
            Next
        Else
            Return tables _
                .Select(Function(x) x.Hits) _
                .Unlist _
                .SaveTo(out & ".csv") _
                .CLICode
        End If

        Return 0
    End Function

    <ExportAPI("/Export.AlignmentTable.giList",
               Usage:="/Export.AlignmentTable.giList /in <table.csv> [/out <gi.txt>]")>
    <Group(CLIGrouping.WebTools)>
    Public Function ParseAlignmentTableGIlist(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-gi-list.txt")
        Dim table = [in].LoadCsv(Of HitRecord)
        Dim list$() = table _
            .Select(Function(x) x.GI) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(s) s) _
            .ToArray

        Return list.FlushAllLines(out).CLICode
    End Function

    <ExportAPI("/AlignmentTable.TopBest", Usage:="/AlignmentTable.TopBest /in <table.csv> [/out <out.csv>]")>
    <Description("Export the top best hit result from the input web alignment table output.")>
    <Group(CLIGrouping.WebTools)>
    Public Function AlignmentTableTopBest(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".top_best.csv")
        Dim data = [in].LoadCsv(Of HitRecord)

        Return data.TopBest() _
            .ToArray _
            .SaveTo(out) _
            .CLICode
    End Function
End Module
