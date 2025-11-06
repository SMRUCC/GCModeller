#Region "Microsoft.VisualBasic::37f6d0a9a14723a657a41bee932365b0, analysis\SequenceToolkit\SequencePatterns.Abstract\Residue.vb"

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

    '   Total Lines: 86
    '    Code Lines: 74 (86.05%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (13.95%)
    '     File Size: 2.39 KB


    ' Structure Residue
    ' 
    '     Properties: frequency, Hi, index, isConserved, isEmpty
    '                 topChar
    ' 
    '     Function: GetEmpty, Max, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Information

Public Structure Residue

    Public Property frequency As Dictionary(Of String, Double)
    Public Property index As Integer

    Public ReadOnly Property topChar As Char
        Get
            Return Max(Me)
        End Get
    End Property

    Default Public ReadOnly Property getFrequency(base As Char) As Double
        Get
            Return _frequency(base)
        End Get
    End Property

    Public ReadOnly Property isEmpty As Boolean
        Get
            If frequency.IsNullOrEmpty Then
                Return True
            ElseIf frequency.Values.All(Function(p) p = 0.0) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property Hi As Double
        Get
            Return frequency.Values.ShannonEntropy()
        End Get
    End Property

    Public ReadOnly Property isConserved As Boolean
        Get
            Return frequency.Values.Max > 0.5
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Dim max As Double = -99999
        Dim maxChar As Char?

        For Each b In frequency
            If b.Value > max Then
                max = b.Value
                maxChar = b.Key
            End If
        Next

        If maxChar Is Nothing Then
            Return "-"
        ElseIf max >= 0.5 Then
            Return Char.ToUpper(maxChar)
        Else
            Return Char.ToLower(maxChar)
        End If
    End Function

    Public Shared Function GetEmpty() As Residue
        Return New Residue With {
            .frequency = New Dictionary(Of String, Double),
            .index = -1
        }
    End Function

    Public Shared Function Max(r As Residue) As Char
        With r.frequency.ToArray
            If .Values.All(Function(p) p = 0R) Then
                Return "-"c
            Else
                Return .ElementAt(which.Max(.Values)) _
                       .Key
            End If
        End With
    End Function
End Structure
