﻿#Region "Microsoft.VisualBasic::57ec68789208df6d31c7ab135c3e3b4f, data\Rhea\TextParser.vb"

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
    '    Code Lines: 61
    ' Comment Lines: 0
    '   Blank Lines: 14
    '     File Size: 2.37 KB


    ' Module TextParser
    ' 
    '     Function: ParseCompounds, ParseList, ParseReactions
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text

Public Module TextParser

    <Extension>
    Public Iterator Function ParseCompounds(lines As IEnumerable(Of String)) As IEnumerable(Of Compound)
        For Each block As String() In lines.Split("///")
            Dim list As Dictionary(Of String, String) = block.ParseList
            Dim obj As New Compound With {
                .entry = list!ENTRY,
                .name = list!NAME,
                .formula = list!FORMULA,
                .reactions = (list!REACTION).Split
            }

            If list.ContainsKey("ENZYME") Then
                obj.enzyme = (list!ENZYME).Split
            End If

            Yield obj
        Next
    End Function

    <Extension>
    Private Function ParseList(block As IEnumerable(Of String)) As Dictionary(Of String, String)
        Dim data As New Dictionary(Of String, String)
        Dim sb As New StringBuilder
        Dim key As String = Nothing

        For Each line As String In block
            Dim tag As String = Mid(line, 1, 12).Trim
            Dim value As String = Mid(line, 13).Trim

            If Not tag.StringEmpty Then
                If Not key.StringEmpty Then
                    data.Add(key, sb.ToString)
                    sb.Clear()
                End If

                sb.Append(value)
                key = tag
            Else
                Call sb.Append(" ")
                Call sb.Append(value)
            End If
        Next

        If Not sb.Length = 0 Then
            Call data.Add(key, sb.ToString)
        End If

        Return data
    End Function

    <Extension>
    Public Iterator Function ParseReactions(lines As IEnumerable(Of String)) As IEnumerable(Of Reaction)
        For Each block As String() In lines.Split("///")
            Dim list As Dictionary(Of String, String) = block.ParseList
            Dim obj As New Reaction With {
                .entry = list!ENTRY,
                .definition = list!DEFINITION,
                .equation = Reaction.EquationParser(list!EQUATION)
            }

            obj.equation.Id = list!ENTRY

            If list.ContainsKey("ENZYME") Then
                obj.enzyme = (list!ENZYME).Split
            End If

            Yield obj
        Next
    End Function
End Module
