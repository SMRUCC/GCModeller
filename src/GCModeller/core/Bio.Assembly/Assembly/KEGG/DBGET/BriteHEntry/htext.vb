﻿#Region "Microsoft.VisualBasic::4251d3cc73d9d5d108242eb485686377, Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\htext.vb"

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

    '     Class htext
    ' 
    '         Properties: Descript, Hierarchical, MaxDepth, Schema, Title
    ' 
    '         Function: br08201, GetEntryDictionary, ko00001, StreamParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' KEGG BRITE is a collection of manually created hierarchical text (htext) files capturing 
    ''' functional hierarchies of various biological objects, especially those represented as 
    ''' KEGG objects. In contrast to KEGG PATHWAY, which is limited to molecular interactions and 
    ''' reactions, KEGG BRITE incorporates many different types of relationships. 
    '''
    ''' Some BRITE hierarchy files are computationally expanded, adding a hierarchy level Or a 
    ''' tab-delimited column, by incorporating additional information. This Is accomplished by the 
    ''' Join Brite operation Of KEGG Mapper, which combines a BRITE hierarchy file (such As ATC 
    ''' drug classification) And a binary relation file (such As D number To target relationships 
    ''' extracted from KEGG DRUG). 
    '''
    ''' Some other BRITE hierarchy files contain tab-delimited attributes that are manually added. 
    ''' Recently introduced BRITE table files are essentailly the same As such multi-column BRITE 
    ''' hierarchy files. Because the table representation Is often easier To understand, certain 
    ''' hierarchy files For disease And drug information have been converted To table files.
    ''' </summary>
    Public Class htext

        Public Property MaxDepth As String
        Public Property Descript As String
        Public Property Title As String
        Public Property Schema As NamedValue(Of String)

        ''' <summary>
        ''' <see cref="BriteHText"/>层次分类数据
        ''' </summary>
        ''' <returns></returns>
        Public Property Hierarchical As BriteHText

        ''' <summary>
        ''' ``KOxxxxx``为键名，注释信息为对应的键值
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetEntryDictionary() As Dictionary(Of String, BriteHText)
            Return Hierarchical _
                .EnumerateEntries _
                .Where(Function(x) Not x.EntryId.StringEmpty) _
                .GroupBy(Function(t) t.EntryId) _
                .ToDictionary(Function(k) k.Key,
                              Function(o) o.First)
        End Function

        Public Overrides Function ToString() As String
            Return Title
        End Function

        ''' <summary>
        ''' 从文本文件之中进行解析操作，<paramref name="res"/>参数为文本内容或者文件的路径
        ''' </summary>
        ''' <param name="res$"></param>
        ''' <returns></returns>
        Public Shared Function StreamParser(res$) As htext
            If (res.IndexOf(ASCII.LF) = -1 AndAlso res.LastIndexOf(ASCII.CR) = -1 AndAlso res.FileExists(True)) Then
                res = res.ReadAllText
            End If

            Dim lines$() = res.Replace("<b>", "").Replace("</b>", "").LineTokens
            Dim header$() = lines(Scan0).Split(ASCII.TAB)
            Dim title As String = lines(1)
            Dim defs As New List(Of String)
            Dim i As VBInteger = 2

            Do While lines(i) <> "!"
                Call defs.Add(lines(++i))
            Loop

            defs = New List(Of String)(defs.Skip(1).Take(defs.Count - 2))
            lines = lines _
                .Skip(i + 1) _
                .Where(Function(s) _
                    Len(s) > 1 AndAlso
                    s.First <> "#"c AndAlso
                    s.First <> "!"c) _
                .ToArray

            Dim schema As New NamedValue(Of String)

            With defs.Select(Function(s) s.GetTagValue(trim:=True)).ToDictionary
                If .ContainsKey("#ENTRY") Then
                    schema.Value = .Item("#ENTRY").Value
                End If
                If .ContainsKey("#DEFINITION") Then
                    schema.Description = .Item("#DEFINITION").Value
                End If
                If .ContainsKey("#NAME") Then
                    schema.Name = .Item("#NAME").Value
                End If
            End With

            Return New htext With {
                .MaxDepth = header(Scan0).Last,
                .Descript = If(header.Length > 1, header(1), ""),
                .Title = title,
                .Schema = schema,
                .Hierarchical = BriteHText.Load(lines, .MaxDepth)
            }
        End Function

        ''' <summary>
        ''' KEGG Orthology (KO)
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function ko00001() As htext
            Return StreamParser(My.Resources.ko00001)
        End Function

        ''' <summary>
        ''' Enzymatic Reactions
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function br08201() As htext
            Return StreamParser(My.Resources.br08201)
        End Function
    End Class
End Namespace
