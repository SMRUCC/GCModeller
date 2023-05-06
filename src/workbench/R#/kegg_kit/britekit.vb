#Region "Microsoft.VisualBasic::d8dc011910e98850da2ac968c5d7c6c1, R#\kegg_kit\britekit.vb"

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

    '   Total Lines: 238
    '    Code Lines: 175
    ' Comment Lines: 38
    '   Blank Lines: 25
    '     File Size: 9.28 KB


    ' Module britekit
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: briteMaps, BriteTable, getHtextTable, getIdPrefix, KOgeneNames
    '               ParseBriteJson, ParseBriteTree
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Toolkit for process the kegg brite text file
''' </summary>
<Package("brite")>
Module britekit

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(htext), AddressOf getHtextTable)
    End Sub

    Private Function getHtextTable(x As Object, args As list, env As Environment) As rdataframe
        Dim rows As EntityObject() = BriteTable(
            htext:=x,
            entryId_pattern:=args.getValue("entryId_pattern", env, "[a-z]+\d+"),
            env:=env
        )
        Dim table As New rdataframe With {.columns = New Dictionary(Of String, Array)}

        table.columns("class") = rows.Vector("class")
        table.columns("category") = rows.Vector("category")
        table.columns("subcategory") = rows.Vector("subcategory")
        table.columns("order") = rows.Vector("order")
        table.columns("entry") = rows.Vector("entry")
        table.columns("name") = rows.Vector("name")

        Return table
    End Function

    ''' <summary>
    ''' Convert the kegg brite htext tree to plant table
    ''' </summary>
    ''' <param name="htext">a htex object</param>
    ''' <param name="entryId_pattern"></param>
    ''' <returns></returns>
    <ExportAPI("brite.as.table")>
    Public Function BriteTable(htext As Object, Optional entryId_pattern$ = "[a-z]+\d+", Optional env As Environment = Nothing) As Object
        Dim terms As IEnumerable(Of BriteTerm)

        If htext Is Nothing Then
            Return REnv.debug.stop("htext object is nothing!", env)
        ElseIf htext.GetType Is GetType(htext) Then
            terms = DirectCast(htext, htext).Deflate(entryId_pattern)
        ElseIf htext.GetType Is GetType(htextJSON) Then
            terms = DirectCast(htext, htextJSON).DeflateTerms
        Else
            Return REnv.debug.stop(New NotSupportedException(htext.GetType.FullName), env)
        End If

        Return terms _
            .Select(Function(term)
                        Return New EntityObject With {
                            .ID = term.kegg_id,
                            .Properties = New Dictionary(Of String, String) From {
                                {NameOf(term.class), term.class},
                                {NameOf(term.category), term.category},
                                {NameOf(term.subcategory), term.subcategory},
                                {NameOf(term.order), term.order},
                                {NameOf(term.entry), term.entry.Key},
                                {"name", term.entry.Value}
                            }
                        }
                    End Function) _
            .GroupBy(Function(term) term.ID) _
            .Select(Function(termGroup)
                        Return termGroup.First
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Do parse of the kegg brite text file.
    ''' </summary>
    ''' <param name="file">
    ''' The file text content, brite id or its file path, example as:
    ''' 
    ''' 1. ``br08901`` could be used at here as the kegg pathway map 
    '''    brite id, which is parsed from the internal resource data
    '''    
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("parse")>
    <RApiReturn(GetType(htext))>
    Public Function ParseBriteTree(file$, Optional env As Environment = Nothing) As Object
        If file.IsPattern("[a-z]+\d+", RegexICSng) Then
            Select Case file.ToLower
                ' enzymatic reactions
                Case NameOf(htext.br08201) : Return htext.br08201
                ' reaction class
                Case NameOf(htext.br08204) : Return htext.br08204
                Case CompoundBrite.cpd_br08001,
                     CompoundBrite.cpd_br08002,
                     CompoundBrite.cpd_br08003,
                     CompoundBrite.cpd_br08005,
                     CompoundBrite.cpd_br08006,
                     CompoundBrite.cpd_br08007,
                     CompoundBrite.cpd_br08008,
                     CompoundBrite.cpd_br08009,
                     CompoundBrite.cpd_br08010,
                     CompoundBrite.cpd_br08021

                    Return htext.GetInternalResource(file)
                Case NameOf(htext.ko00001) : Return htext.ko00001
                ' kegg pathway maps
                Case NameOf(htext.br08901) : Return htext.br08901
                Case Else
                    Return REnv.debug.stop({$"Invalid brite id: {file}", $"brite id: {file}"}, env)
            End Select
        ElseIf file.StartsWith("KO:") Then
            Dim Tcode As String = file.GetTagValue(":").Value
            Dim fileTmp As String = TempFileSystem.TempDir & $"/KO/{Tcode}.kegg"

            If Not fileTmp.FileLength > 100 Then
                Call ($"https://www.kegg.jp/kegg-bin/download_htext?htext={Tcode}00001&format=htext&filedir=") _
                    .GET _
                    .SaveTo(fileTmp)
            End If

            Return htext.StreamParser(res:=fileTmp)
        Else
            Return htext.StreamParser(res:=file)
        End If
    End Function

    ''' <summary>
    ''' Do parse of the kegg brite json file.
    ''' </summary>
    ''' <param name="file">the htext json file path</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("brite.parseJSON")>
    Public Function ParseBriteJson(file$, Optional env As Environment = Nothing) As Object
        Return htextJSON.parseJSON(file)
    End Function

    ''' <summary>
    ''' Parse gene names for each KO number from the default internal htext resource.
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("KO.geneNames")>
    Public Function KOgeneNames() As Dictionary(Of String, String)
        Dim brites = PathwayMapping.DefaultKOTable
        Dim names As New Dictionary(Of String, String)
        Dim name As String

        For Each term In brites
            name = term.Value.description
            name = name.StringSplit(";\s*").First

            If name.StringEmpty Then
                names.Add(term.Key, term.Key)
            Else
                names.Add(term.Key, name)
            End If
        Next

        Return names
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="htext"></param>
    ''' <param name="geneId"></param>
    ''' <param name="level">
    ''' class|category|subcategory
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("briteMaps")>
    Public Function briteMaps(htext As htext, geneId As String(), Optional level$ = "class") As String()
        Dim prefix As String = geneId.getIdPrefix
        Dim table = htext.Deflate($"{prefix}\d+") _
            .GroupBy(Function(gene) gene.entry.Key) _
            .ToDictionary(Function(gene) gene.Key,
                          Function(gene)
                              Select Case level
                                  Case "class"
                                      Return gene.First.class
                                  Case "category"
                                      Return gene.First.category
                                  Case "subcategory"
                                      Return gene.First.subcategory
                                  Case Else
                                      Return "n/a"
                              End Select
                          End Function)

        Return geneId _
            .Select(Function(id)
                        Return table.TryGetValue(HeaderFormats.TrimAccessionVersion(id), [default]:="n/a")
                    End Function) _
            .ToArray
    End Function

    <Extension>
    Private Function getIdPrefix(names As String()) As String
        Dim minName As String = names.MinLengthString
        Dim index As Integer
        Dim uniqchars As Char()

        For i As Integer = 0 To minName.Length - 1
            index = i
            uniqchars = names _
                .Select(Function(str) str(index)) _
                .Distinct _
                .ToArray

            If uniqchars.Length > 1 Then
                Exit For
            End If
        Next

        Dim prefix As String

        If index = 0 Then
            prefix = names _
                .Select(Function(str) str(index)) _
                .GroupBy(Function(c) c) _
                .OrderByDescending(Function(c) c.Count) _
                .First _
                .Key
        Else
            prefix = names(Scan0).Substring(0, index)
        End If

        Return prefix
    End Function
End Module
