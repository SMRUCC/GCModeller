#Region "Microsoft.VisualBasic::1a86765966d54082711a403384518767, CLI_tools\gcc\CLI\iGEM.vb"

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
    '     Function: QueryParts, SelectParts
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly

Partial Module CLI

    <ExportAPI("/iGEM.select.parts")>
    <Usage("/iGEM.select.parts /list <id.list.txt> /allparts <ALL_parts.fasta> [/out <table.xls>]")>
    <Description("Select iGEM part sequence by given id list.")>
    <Group(Program.iGEMTools)>
    Public Function SelectParts(args As CommandLine) As Integer
        Dim in$ = args <= "/list"
        Dim allparts = iGEM.PartSeq.Parse(args <= "/allparts").GroupBy(Function(p) p.PartName).ToDictionary(Function(p) p.Key, Function(group) group.ToArray)
        Dim out$ = args("out") Or $"{[in].TrimSuffix}.iGEM_parts.xls"
        Dim idList = [in].IterateAllLines _
            .Select(Function(line)
                        Return Strings.Trim(line) _
                            .StringSplit("[\s,\t]+") _
                            .FirstOrDefault
                    End Function) _
            .Where(Function(id) Not id.StringEmpty) _
            .ToArray

        If [in].ExtensionSuffix = "csv" Then
            ' skip header
            idList = idList.Skip(1).ToArray
        End If

        Dim subset = idList _
            .Select(Function(id) As IEnumerable(Of iGEM.PartSeq)
                        If allparts.ContainsKey(id) Then
                            Return allparts(id)
                        Else
                            ' empty line for sequence not found
                            Return {New iGEM.PartSeq With {
                                .PartName = id
                            }}
                        End If
                    End Function) _
            .IteratesALL _
            .ToArray

        Return subset.SaveTo(out, tsv:=True).CLICode
    End Function

    <ExportAPI("/iGEM.query.parts")>
    <Usage("/iGEM.query.parts /list <id.list.txt> [/out <table.xls>]")>
    <Description("Query parts data from iGEM server by given id list.")>
    <Group(Program.iGEMTools)>
    Public Function QueryParts(args As CommandLine) As Integer
        Dim in$ = args <= "/list"
        Dim out$ = args("out") Or $"{[in].TrimSuffix}.iGEM_parts.xls"
        Dim idList = [in].IterateAllLines _
            .Select(Function(line)
                        Return Strings.Trim(line) _
                            .StringSplit("[\s,\t]+") _
                            .FirstOrDefault
                    End Function) _
            .Where(Function(id) Not id.StringEmpty) _
            .ToArray

        If [in].ExtensionSuffix = "csv" Then
            ' skip header
            idList = idList.Skip(1).ToArray
        End If

        Dim result As New List(Of EntityObject)

        For Each parts As iGEM.rsbpml In New iGEM.iGEMQuery(cache:=out.ParentPath & "/.iGEM").Query(Of iGEM.rsbpml)(idList)
            result += parts.part_list _
                .Select(Function(part)
                            Return New EntityObject With {
                                .ID = part.part_name,
                                .Properties = New Dictionary(Of String, String) From {
                                    {"part_id", part.part_id},
                                    {"short_name", part.part_short_name},
                                    {"short_desc", part.part_short_desc},
                                    {"type", part.part_type},
                                    {"sample", part.sample_status},
                                    {"author", part.part_author},
                                    {"sequence", part.sequences _
                                        .SequenceData _
                                        .LineTokens _
                                        .Select(AddressOf Strings.Trim) _
                                        .JoinBy("")}
                                }
                            }
                        End Function)
        Next

        Return result.SaveTo(out, tsv:=True)
    End Function
End Module
