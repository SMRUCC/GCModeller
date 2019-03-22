#Region "Microsoft.VisualBasic::19f02da8a09b48d3677ffe2eba024f3d, CLI_tools\eggHTS\CLI\UniProtTools.vb"

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
'     Function: UniProtIDList
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Partial Module CLI

    ''' <summary>
    ''' ```
    ''' 0  1      2
    ''' tr|Q3KBE6|Q3KBE6_PSEPF
    ''' ```
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/UniProt.IDs")>
    <Usage("/UniProt.IDs /in <list.csv/txt> [/out <list.txt>]")>
    Public Function UniProtIDList(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.uniprotIDs.txt"
        Dim list$()

        With [in].ExtensionSuffix
            If .TextEquals("csv") Then
                list = EntityObject.LoadDataSet([in]).Keys
            ElseIf .TextEquals("txt") Then
                list = [in].ReadAllLines
            Else
                Throw New NotImplementedException(.ByRef)
            End If
        End With

        Return list _
            .Select(Function(id) id.Split("|"c)(1)) _
            .ToArray _
            .SaveTo(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 提供一个类似于 https://www.uniprot.org/uploadlists/ 的功能, 在本地批量的将其他的数据库编号转换为UniProt编号
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Retrieve.ID.mapping")>
    <Usage("/Retrieve.ID.mapping /list <geneID.list> /uniprot <uniprot/uniparc.Xml> [/out <map.list.csv>]")>
    <Description("Convert the protein id from other database to UniProtKB.")>
    Public Function RetrieveIDmapping(args As CommandLine) As Integer
        Dim in$ = args <= "/list"
        Dim uniprot$ = args <= "/uniprot"
        Dim out$ = args("/out") Or $"{in$.TrimSuffix}.uniprotKB.csv"
        ' Removes all of the version number
        Dim list As Index(Of String) = [in] _
            .ReadAllLines _
            .Where(Function(line) Not line.StringEmpty) _
            .Select(Function(id) id.Split("."c).First) _
            .Indexing
        Dim maps As New Value(Of String())
        Dim isUniParc As Boolean = UniProtXML.GetType(uniprot).TextEquals("uniparc")

        Using mapping As StreamWriter = out.OpenWriter
            Call mapping.WriteLine(New RowObject("ID", "source", "UniProtKB").AsLine)

            For Each protein As entry In UniProtXML.EnumerateEntries(uniprot, isUniParc)
                Dim uniprotKB As List(Of String)

                If isUniParc Then
                    uniprotKB = protein.dbReferences _
                        .Where(Function(r) r.type = "UniProtKB/TrEMBL") _
                        .Select(Function(r) r.id) _
                        .AsList
                Else
                    uniprotKB = protein.accessions.AsList
                End If

                For Each id In protein.EnumerateAllIDs
                    If id.xrefID Like list Then
                        Call mapping.WriteLine(New RowObject(maps = {id.xrefID, id.Database} + uniprotKB).AsLine)
                        Call list.Delete(id.xrefID)
                        Call maps.GetJson.__DEBUG_ECHO

                        If list.Count = 0 Then
                            GoTo Break
                        End If
                    End If
                Next
            Next
Break:
        End Using

        Return 0
    End Function
End Module
