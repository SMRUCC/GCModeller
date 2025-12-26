#Region "Microsoft.VisualBasic::f298eb3504097f75bfbcc88259f274fe, modules\ExperimentDesigner\SampleNames.vb"

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

'   Total Lines: 75
'    Code Lines: 51 (68.00%)
' Comment Lines: 15 (20.00%)
'    - Xml Docs: 53.33%
' 
'   Blank Lines: 9 (12.00%)
'     File Size: 2.65 KB


' Module SampleNames
' 
'     Function: GuessPossibleGroups, ParseGroupInfo
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.Patterns

''' <summary>
''' Sample names helper
''' </summary>
Public Module SampleNames

    ''' <summary>
    ''' Guess all possible sample groups from the given name string collection.
    ''' </summary>
    ''' <param name="allSampleNames"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function GuessPossibleGroups(allSampleNames As IEnumerable(Of String),
                                                 Optional maxDepth As Boolean = False) As IEnumerable(Of NamedCollection(Of String))

        Dim commonTags As New CommonTagParser(allSampleNames, maxDepth)
        ' continute for extends group labels
        '          colIndex
        '          |
        ' iBAQ-AAA-1
        ' iBAQ-AAA-2
        ' iBAQ-BBB-1
        ' iBAQ-BBB-25
        Dim largeGroups As IGrouping(Of String, Char())() = commonTags.nameMatrix _
            .GroupBy(Function(cs)
                         Return cs.Take(commonTags.MaxColumnIndex + 1).CharString
                     End Function) _
            .ToArray

        For Each group As IGrouping(Of String, Char()) In largeGroups
            Yield commonTags.ParseGroupInfo(group)
        Next
    End Function

    <Extension>
    Private Function ParseGroupInfo(commonTags As CommonTagParser, group As IGrouping(Of String, Char())) As NamedCollection(Of String)
        Dim j As Integer
        Dim nameMatrix = group.ToArray
        Dim maxLen% = Aggregate name As Char()
                      In nameMatrix
                      Into Max(name.Length)
        Dim col As Char()

        For i As Integer = commonTags.MaxColumnIndex To maxLen - 1
            j = i
            col = nameMatrix _
                .Select(Function(name)
                            Return name.ElementAtOrNull(j)
                        End Function) _
                .ToArray

            If col.Distinct.Count > 1 Then
                Exit For
            End If
        Next

        Dim groupName As String = nameMatrix _
            .Select(Function(cs) cs.Take(j).CharString) _
            .First _
            .Trim(" "c, "-"c, "_"c, "~"c, "+"c, "."c)

        'If groupName.EndsWith("[^a-zA-Z]+\d+$", RegexICSng) Then
        '    Dim suffix As String = groupName.Match("[^a-zA-Z]+\d+$", RegexICSng)
        '    groupName = groupName.Substring(0, groupName.Length - suffix.Length)
        'End If

        Return New NamedCollection(Of String) With {
            .name = groupName,
            .value = nameMatrix _
                .Select(Function(name)
                            Return name.CharString
                        End Function) _
                .ToArray
        }
    End Function

    <Extension>
    Public Function GroupIndexing(sampleId As IEnumerable(Of String), sampleinfo As IEnumerable(Of SampleInfo), Optional strict As Boolean = True) As Dictionary(Of String, Integer())
        Dim sampleIndex As Index(Of String) = sampleId.Indexing
        Dim nameGroups = DataGroup.NameGroups(sampleinfo)
        Dim missing As New List(Of NamedCollection(Of String))
        Dim indexing As New Dictionary(Of String, Integer())

        For Each group In nameGroups
            Dim miss As New List(Of String)
            Dim offsets As New List(Of Integer)

            For Each id As String In group.Value
                Dim i As Integer = sampleIndex(id)

                If i < 0 Then
                    Call miss.Add(id)
                Else
                    Call offsets.Add(i)
                End If
            Next

            If offsets.Any Then
                Call indexing.Add(group.Key, offsets.ToArray)
            End If
            If miss.Any Then
                Call missing.Add(New NamedCollection(Of String)(group.Key, miss))
            End If
        Next

        If missing.Any Then
            Dim missing_groups = missing.Select(Function(s) s.JoinBy(", ") & $" from group '{s.name}'").JoinBy("; ")
            Dim msg As String = $"missing sample id: {missing_groups}!"

            If strict Then
                Throw New InvalidDataException(msg)
            Else
                Call msg.Warning
            End If
        End If

        Return indexing
    End Function
End Module
