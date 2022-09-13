#Region "Microsoft.VisualBasic::634e4253bf0ce45df706a49fa567a530, modules\ExperimentDesigner\SampleNames.vb"

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

    '   Total Lines: 103
    '    Code Lines: 70
    ' Comment Lines: 20
    '   Blank Lines: 13
    '     File Size: 3.54 KB


    ' Module SampleNames
    ' 
    '     Function: GuessPossibleGroups
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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
    Public Iterator Function GuessPossibleGroups(allSampleNames As IEnumerable(Of String), Optional maxDepth As Boolean = False) As IEnumerable(Of NamedCollection(Of String))
        Dim nameMatrix As Char()() = allSampleNames.Select(Function(name) name.ToArray).ToArray
        Dim maxLen% = Aggregate name As Char() In nameMatrix Into Max(name.Length)
        Dim col As Char()
        Dim colIndex As Integer

        For i As Integer = 0 To maxLen - 1
            colIndex = i
            col = nameMatrix _
                .Select(Function(name)
                            Return name.ElementAtOrNull(colIndex)
                        End Function) _
                .ToArray

            '      colIndex
            '      |
            ' iBAQ-AA-1
            ' iBAQ-BB-2

            If maxDepth Then
                If Not nameMatrix _
                    .GroupBy(Function(cs)
                                 Return cs.Take(colIndex + 1).CharString
                             End Function) _
                    .All(Function(c)
                             Return c.Count > 1
                         End Function) Then

                    colIndex -= 1
                    Exit For
                End If
            Else
                If col.Distinct.Count > 1 Then
                    ' group label at here
                    Exit For
                End If
            End If
        Next

        ' continute for extends group labels
        '          colIndex
        '          |
        ' iBAQ-AAA-1
        ' iBAQ-AAA-2
        ' iBAQ-BBB-1
        ' iBAQ-BBB-25
        Dim largeGroups As IGrouping(Of String, Char())() = nameMatrix _
            .GroupBy(Function(cs)
                         Return cs.Take(colIndex + 1).CharString
                     End Function) _
            .ToArray

        For Each group As IGrouping(Of String, Char()) In largeGroups
            Dim j As Integer

            nameMatrix = group.ToArray
            maxLen% = Aggregate name As Char()
                      In nameMatrix
                      Into Max(name.Length)

            For i As Integer = colIndex To maxLen - 1
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

            Yield New NamedCollection(Of String) With {
                .name = groupName,
                .value = nameMatrix _
                    .Select(Function(name)
                                Return name.CharString
                            End Function) _
                    .ToArray
            }
        Next
    End Function
End Module
