#Region "Microsoft.VisualBasic::5f48b3eee751d593e1417952a9021824, localblast\CLI_tools\CLI\BBH\UniProtTools.vb"

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
    '     Function: UniProtBBHMapTable
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Partial Module CLI

    <ExportAPI("/UniProt.bbh.mappings")>
    <Usage("/UniProt.bbh.mappings /in <bbh.csv> [/reverse /out <mappings.txt>]")>
    Public Function UniProtBBHMapTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.mappings.txt"
        Dim mappings = [in].LoadCsv(Of BiDirectionalBesthit)
        Dim reversed As Boolean = args("/reverse")
        Dim table As Dictionary(Of String, String) = mappings _
            .Where(Function(query)
                       Return Not (query.HitName.StringEmpty OrElse query.HitName.TextEquals(HITS_NOT_FOUND))
                   End Function) _
            .ToDictionary(Function(query)
                              Return query.QueryName
                          End Function,
                          Function(query)
                              Return query.HitName.Split("|"c)(1)
                          End Function)

        Return table.Tsv(out,, reversed:=reversed).CLICode
    End Function
End Module
