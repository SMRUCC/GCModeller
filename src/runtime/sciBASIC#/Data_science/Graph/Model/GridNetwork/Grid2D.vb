﻿#Region "Microsoft.VisualBasic::bd5b978eac398b1f361814fc648109a0, Data_science\Graph\Model\GridNetwork\Grid2D.vb"

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

' Class Grid
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: Create, EnumerateData, GetData, (+2 Overloads) Query
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' a generic grid graph for fast query of the 2D geometry data
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class Grid(Of T)

    ReadOnly matrix2D As Dictionary(Of Long, Dictionary(Of Long, GridCell(Of T)))

    Public ReadOnly Property width As Integer
        Get
            Return matrix2D.Keys.Max
        End Get
    End Property

    Public ReadOnly Property height As Integer
        Get
            Return matrix2D.Values.Select(Function(d) d.Keys.Max).Max
        End Get
    End Property

    Public ReadOnly Property size As Integer
        Get
            Return Aggregate row In matrix2D Let rowCount = row.Value.Count Into Sum(rowCount)
        End Get
    End Property

    Private Sub New(points As IEnumerable(Of GridCell(Of T)))
        matrix2D = points _
            .GroupBy(Function(d) d.index.X) _
            .ToDictionary(Function(d) CLng(d.Key),
                          Function(d)
                              Return d _
                                  .GroupBy(Function(p) p.index.Y) _
                                  .ToDictionary(Function(p) CLng(p.Key),
                                                Function(p)
                                                    Return p.First
                                                End Function)
                          End Function)
    End Sub

    ''' <summary>
    ''' populate all of the cell data in current grid graph
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function EnumerateData() As IEnumerable(Of T)
        For Each row In matrix2D
            For Each col In row.Value
                Yield col.Value.data
            Next
        Next
    End Function

    Public Iterator Function LineScans() As IEnumerable(Of T)
        For Each row In matrix2D.OrderBy(Function(r) r.Key)
            For Each col In row.Value.OrderBy(Function(c) c.Key)
                Yield col.Value.data
            Next
        Next
    End Function

    Public Function ShuffleAll() As T()
        Return EnumerateData.Shuffles
    End Function

    ''' <summary>
    ''' get target cell data via a given pixel point
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="hit"></param>
    ''' <returns>
    ''' nothing will be returns if there is no data on the given ``[x,y]`` pixel point.
    ''' </returns>
    Public Function GetData(x As Integer, y As Integer, Optional ByRef hit As Boolean = False) As T
        Dim xkey = CLng(x), ykey = CLng(y)

        If Not matrix2D.ContainsKey(xkey) Then
            hit = False
            Return Nothing
        ElseIf Not matrix2D(xkey).ContainsKey(ykey) Then
            hit = False
            Return Nothing
        Else
            hit = True
        End If

        Return matrix2D(xkey)(ykey).data
    End Function

    ''' <summary>
    ''' get a range of nearby cell data via a given pixel point data 
    ''' and query size of the cell block rectangle.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="gridSize"></param>
    ''' <returns></returns>
    Public Function Query(x As Integer, y As Integer, gridSize As Integer) As IEnumerable(Of T)
        Return Query(x, y, New Size(gridSize, gridSize))
    End Function

    Public Iterator Function Query(x As Integer, y As Integer, gridSize As Size) As IEnumerable(Of T)
        Dim q As T
        Dim hit As Boolean = False

        For i As Integer = x - gridSize.Width To x + gridSize.Width
            For j As Integer = y - gridSize.Height To y + gridSize.Height
                q = GetData(i, j, hit)

                If hit Then
                    Yield q
                End If
            Next
        Next
    End Function

    Public Shared Function Create(data As IEnumerable(Of T), getX As Func(Of T, Integer), getY As Func(Of T, Integer)) As Grid(Of T)
        Return data _
            .SafeQuery _
            .Select(Function(d)
                        Return New GridCell(Of T)(getX(d), getY(d), d)
                    End Function) _
            .DoCall(Function(vec)
                        Return New Grid(Of T)(vec)
                    End Function)
    End Function

End Class

