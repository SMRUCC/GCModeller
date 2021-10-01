#Region "Microsoft.VisualBasic::5d50e64f5fde0aa14f7aa325517150af, core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHText\htext.vb"

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
    '         Function: br08201, br08204, br08901, GetEntryDictionary, GetInternalResource
    '                   GetInternalResourceText, getResourceCache, ko00001, StreamParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder.VBLanguage
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models

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
                .Where(Function(x) Not x.entryID.StringEmpty) _
                .GroupBy(Function(t) t.entryID) _
                .ToDictionary(Function(k) k.Key,
                              Function(o)
                                  Return o.First
                              End Function)
        End Function

        Public Overrides Function ToString() As String
            Return Title
        End Function

        ''' <summary>
        ''' 从文本文件之中进行解析操作，<paramref name="res"/>参数为文本内容或者文件的路径
        ''' </summary>
        ''' <param name="res$"></param>
        ''' <returns></returns>
        Public Shared Function StreamParser(res As String) As htext
            If (res.IndexOf(ASCII.LF) = -1 AndAlso res.LastIndexOf(ASCII.CR) = -1 AndAlso res.FileExists(True)) Then
                res = res.ReadAllText
            End If

            Dim lines$() = res.Replace("<b>", "").Replace("</b>", "").LineTokens
            Dim header$() = lines(Scan0).Split(ASCII.TAB)
            Dim title As String = lines(1)
            Dim defs As New List(Of String)
            Dim i As i32 = 2

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
                .Hierarchical = BriteHTextParser.Load(lines, .MaxDepth)
            }
        End Function

        ''' <summary>
        ''' KEGG Orthology (KO)
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ko00001() As htext
            Return StreamParser(My.Resources.ko00001)
        End Function

        ''' <summary>
        ''' Enzymatic Reactions
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function br08201() As htext
            Return StreamParser(My.Resources.br08201)
        End Function

        ''' <summary>
        ''' reaction class
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function br08204() As htext
            Return StreamParser(My.Resources.br08204)
        End Function

        ''' <summary>
        ''' KEGG pathway maps
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function br08901() As htext
            Return StreamParser(My.Resources.br08901)
        End Function

#If netcore5 = 0 Then
        Private Shared Function getResourceCache() As ResourcesSatellite
            Static satellite As New ResourcesSatellite(GetType(LICENSE))
            Return satellite
        End Function
#End If

        Private Shared Function GetInternalResourceText(resourceName As String) As String
            Return My.Resources.ResourceManager.GetString(resourceName)
        End Function

        Public Shared Function GetInternalResource(resourceName As String) As htext
            Dim resource$ = Nothing

            If resourceName.IsPattern(Patterns.Identifer, RegexICSng) Then
#If netcore5 = 0 Then
                resource = getResourceCache.GetString(resourceName)
#Else
                resource = GetInternalResourceText(resourceName)
#End If

            ElseIf resourceName.IsURLPattern Then
                With resourceName.Split("?"c).Last.Match("[0-9a-zA-Z_]+\.keg")
                    If Not .StringEmpty Then
#If netcore5 = 0 Then
                        resource = getResourceCache.GetString(.Replace(".keg", ""))
#Else
                        Throw New NotImplementedException
#End If
                    End If
                End With

                If resource.StringEmpty Then
                    resource = resourceName.GET
                End If
            ElseIf resourceName.FileExists Then
                resource = resourceName.ReadAllText
            Else
                Throw New NotImplementedException(resourceName)
            End If

            Return htext.StreamParser(resource)
        End Function
    End Class
End Namespace
