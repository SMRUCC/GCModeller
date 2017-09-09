#Region "Microsoft.VisualBasic::028ca77faf985f606de7456b26d1f1f7, ..\localblast\CLI_tools\CLI\Web.vb"

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

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult

Partial Module CLI

    <ExportAPI("/Export.AlignmentTable",
               Usage:="/Export.AlignmentTable /in <alignment.txt> [/split /header.split /out <outDIR/file>]")>
    <Group(CLIGrouping.WebTools)>
    Public Function ExportWebAlignmentTable(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim isSplit As Boolean = args.GetBoolean("/split")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix)
        Dim headerSplit? = args.GetBoolean("/header.split")
        Dim tables As IEnumerable(Of AlignmentTable) =
            [in].IterateTables(headerSplit)

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

    <ExportAPI("/AlignmentTable.TopBest",
               Usage:="/AlignmentTable.TopBest /in <table.csv> [/out <out.csv>]")>
    <Group(CLIGrouping.WebTools)>
    Public Function AlignmentTableTopBest(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".top_best.csv")
        Dim data = [in].LoadCsv(Of HitRecord)

        Return HitRecord.TopBest(data) _
            .ToArray _
            .SaveTo(out) _
            .CLICode
    End Function
End Module
