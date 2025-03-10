﻿#Region "Microsoft.VisualBasic::c6b8182203d82ab1df8dfe8b846771b2, Microsoft.VisualBasic.Core\src\Text\StringSimilarity\IEqualityComparer.vb"

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

    '   Total Lines: 99
    '    Code Lines: 63 (63.64%)
    ' Comment Lines: 22 (22.22%)
    '    - Xml Docs: 90.91%
    ' 
    '   Blank Lines: 14 (14.14%)
    '     File Size: 3.65 KB


    '     Class StringEqualityHelper
    ' 
    '         Properties: BinaryEquals, TextEquals
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Equals, GetHashCode, ToString
    ' 
    '     Class DirectTextComparer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Equals, GetHashCode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.DataStructures

Namespace Text.Similarity

    ''' <summary>
    ''' thresholds:
    ''' 
    ''' + 0: text equals
    ''' + 1: binary equals
    ''' + (0, 1): similarity threshold
    ''' </summary>
    Public Class StringEqualityHelper : Implements IEqualityComparer(Of String)

        ''' <summary>
        ''' + 0: text equals
        ''' + 1: binary equals
        ''' + (0, 1): similarity threshold
        ''' </summary>
        Dim threshold#
        Dim compare As GenericLambda(Of String).IEquals

        ''' <summary>
        ''' thresholds:
        ''' 
        ''' + 0: text equals
        ''' + 1: binary equals
        ''' + (0, 1): similarity threshold
        ''' </summary>
        Sub New(threshold#)
            Me.threshold = threshold

            If threshold = 0R Then
                compare = Function(s1, s2) s1.TextEquals(s2)
            ElseIf threshold = 1 Then
                compare = Function(s1, s2) s1 = s2
            Else
                compare = Function(s1, s2)
                              With LevenshteinDistance.ComputeDistance(s1, s2)
                                  Return?.MatchSimilarity >= threshold
                              End With
                          End Function
            End If
        End Sub

        Public Shared ReadOnly Property TextEquals As New StringEqualityHelper(0)
        Public Shared ReadOnly Property BinaryEquals As New StringEqualityHelper(1)

        Public Overrides Function ToString() As String
            If threshold = 0R Then
                Return "Text Equals Of the Two String"
            ElseIf threshold = 1.0R Then
                Return "Binary Equals Of the Two String"
            Else
                Return "String Similarity With threshold " & threshold
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function Equals(x As String, y As String) As Boolean Implements IEqualityComparer(Of String).Equals
            Return compare(x, y)
        End Function

        Public Overloads Function GetHashCode(obj As String) As Integer Implements IEqualityComparer(Of String).GetHashCode
            If obj Is Nothing Then
                Return 0
            End If
            Return obj.GetHashCode
        End Function
    End Class

    ''' <summary>
    ''' A wrapper of the <see cref="TextEquals"/> function
    ''' </summary>
    Public Class DirectTextComparer : Implements IEqualityComparer(Of String)

        ReadOnly null_equals As Boolean = False
        ReadOnly empty_equals As Boolean = True

        Sub New(Optional null_equals As Boolean = False, Optional empty_equals As Boolean = True)
            Me.null_equals = null_equals
            Me.empty_equals = empty_equals
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function Equals(x As String, y As String) As Boolean Implements IEqualityComparer(Of String).Equals
            Return x.TextEquals(y, null_equals, empty_equals)
        End Function

        Public Overloads Function GetHashCode(obj As String) As Integer Implements IEqualityComparer(Of String).GetHashCode
            If obj Is Nothing Then
                Return 0
            End If
            Return obj.GetHashCode
        End Function
    End Class
End Namespace
