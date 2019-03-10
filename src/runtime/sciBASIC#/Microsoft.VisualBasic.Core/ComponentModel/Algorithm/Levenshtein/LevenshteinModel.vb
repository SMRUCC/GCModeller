﻿#Region "Microsoft.VisualBasic::6ac0df2fcb666d45d92a8d277e2b6fb8, Microsoft.VisualBasic.Core\ComponentModel\Algorithm\Levenshtein\LevenshteinModel.vb"

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

    '     Class DistResult
    ' 
    '         Properties: Distance, DistEdits, DistTable, Hypotheses, Matches
    '                     MatchSimilarity, NumMatches, Path, Reference, Score
    ' 
    '         Function: __getReference, __getSubject, __innerInsert, CopyTo, IsPath
    '                   ToString, TrimMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports sys = System.Math

Namespace Text.Levenshtein

    Public Class DistResult

        Public Property Reference As String
        Public Property Hypotheses As String
        Public Property DistTable As Streams.Array.Double()
        ''' <summary>
        ''' How doest the <see cref="Hypotheses"/> evolve from <see cref="Reference"/>.(这个结果描述了subject是如何变化成为Query的)
        ''' </summary>
        ''' <returns></returns>
        Public Property DistEdits As String
        Public Property Path As Coordinate()
        Public Property Matches As String

        Public Overrides Function ToString() As String
            Return $"{Reference} => {Hypotheses} // {DistEdits}"
        End Function

        Public Function IsPath(i As Integer, j As Integer) As Boolean
            Dim pt As New Point With {.X = i, .Y = j}
            Dim LQuery% = (From c As Coordinate
                           In Path
                           Where c = pt
                           Select 100).FirstOrDefault
            Return LQuery > 50
        End Function

        Public ReadOnly Property Distance As Double
            Get
                If DistTable.IsNullOrEmpty Then
                    Return 0
                End If

                Dim reference As String = __getReference()
                Dim hypotheses As String = __getSubject()

                Return DistTable(reference.Length) _
                    .Values(hypotheses.Length)
            End Get
        End Property

        ''' <summary>
        ''' 可以简单地使用这个数值来表述所比较的两个对象之间的相似度
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Score As Double
            Get
                If String.IsNullOrEmpty(DistEdits) Then
                    Return 0
                End If

                Dim view As String = DistEdits.ToLower
                Dim m As Integer = view.Count("m"c)
                Dim i As Integer = view.Count("i"c)
                Dim d As Integer = view.Count("d"c)
                Dim s As Integer = view.Count("s"c)
                Dim len As Integer = view.Length

                Return (m - (i * 0.5 + d * 0.3 + s * 0.2)) / len
            End Get
        End Property

        ''' <summary>
        ''' ``m+`` scores.(0-1之间)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MatchSimilarity As Double
            Get
                Dim ms As String() = Regex.Matches(DistEdits, "m+").ToArray
                Dim mg = (From x In ms Select x Group x By x Into Count).ToArray
                Dim len = DistEdits.Length
                Dim score As Double

                For Each x In mg
                    score += (x.x.Length / len) * x.Count
                Next

                Return score
            End Get
        End Property

        ''' <summary>
        ''' 比对上的对象的数目
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NumMatches As Integer
            Get
                If String.IsNullOrEmpty(DistEdits) Then
                    Return 0
                End If

                Dim view As String = DistEdits.ToLower
                Dim m As Integer = view.Count("m"c)
                Return m
            End Get
        End Property

        Public Function CopyTo(Of T As DistResult)(ByRef obj As T) As T
            obj.Path = Path
            obj.DistEdits = DistEdits
            obj.DistTable = DistTable
            obj.Hypotheses = Hypotheses
            obj.Matches = Matches
            obj.Reference = Reference
            Return obj
        End Function

        Public Function TrimMatrix(l As Integer) As Streams.Array.Double()
            Me.DistTable = Me.DistTable _
                .Select(Function(row)
                            Dim values#() = row _
                                .Values _
                                .Select(Function(n) sys.Round(n, l)) _
                                .ToArray

                            Return New Streams.Array.Double With {
                                .Values = values
                            }
                        End Function) _
                .ToArray

            Return Me.DistTable
        End Function

        Protected Friend Overridable Function __innerInsert() As String
            Return ""
        End Function

        Protected Friend Overridable Function __getReference() As String
            Return Reference
        End Function

        Protected Friend Overridable Function __getSubject() As String
            Return Hypotheses
        End Function
    End Class
End Namespace
