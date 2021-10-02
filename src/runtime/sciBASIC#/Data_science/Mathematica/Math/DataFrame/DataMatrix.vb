﻿#Region "Microsoft.VisualBasic::a013852a4c8a789526635da422a5ca37, Data_science\Mathematica\Math\DataFrame\DataMatrix.vb"

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

    ' Class DataMatrix
    ' 
    '     Properties: keys, size
    ' 
    '     Constructor: (+3 Overloads) Sub New
    '     Function: GetVector, PopulateRowEntitys, PopulateRowObjects, PopulateRows, ToString
    '               Visit
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 1 Then
Imports System.Data
#End If

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class DataMatrix : Implements IBucketVector

    Protected Friend ReadOnly names As Index(Of String)
    Protected Friend ReadOnly matrix As Double()()

    Default Public Overridable Property dist(a$, b$) As Double
        Get
            Return Me(names(a), names(b))
        End Get
        Set
            Me(names(a), names(b)) = Value
        End Set
    End Property

    Default Public Overridable Property dist(i%, j%) As Double
        Get
            Return matrix(j)(i)
        End Get
        Set(value As Double)
            matrix(j)(i) = value
        End Set
    End Property

    Public ReadOnly Property keys As String()
        Get
            Return names.Objects
        End Get
    End Property

    Public ReadOnly Property size As Integer
        Get
            Return matrix.Length
        End Get
    End Property

    Sub New(names As IEnumerable(Of String))
        Me.names = names.Indexing
        Me.matrix = MAT(Of Double)(Me.names.Count, Me.names.Count)
    End Sub

    Sub New(names As Index(Of String), matrix As Double()())
        Me.names = names
        Me.matrix = matrix

        If Me.names.Count <> matrix.Length Then
            Throw New InvalidConstraintException("the given member names is not equals to the matrix size!")
        End If
    End Sub

    Sub New(M%, N%)
        Me.matrix = MAT(Of Double)(M, N)
        Me.names = New Index(Of String)
    End Sub

    Public Function Visit(Of DataSet As {New, INamedValue, DynamicPropertyBase(Of Double)})(projectName As String, direction As MatrixVisit) As DataSet
        Dim v As New DataSet With {.Key = projectName}
        Dim i As Integer = names(projectName)

        If direction = MatrixVisit.ByRow Then
            For Each name As SeqValue(Of String) In names
                Call v.Add(name.value, matrix(i)(name.i))
            Next
        Else
            For Each name As SeqValue(Of String) In names
                Call v.Add(name.value, matrix(name.i)(i))
            Next
        End If

        Return v
    End Function

    Public Iterator Function PopulateRows() As IEnumerable(Of IReadOnlyCollection(Of Double))
        For Each row As Double() In matrix
            Yield DirectCast(row, IReadOnlyCollection(Of Double))
        Next
    End Function

    ''' <summary>
    ''' when matrix is a (NxN) matrix
    ''' </summary>
    ''' <typeparam name="DataSet"></typeparam>
    ''' <returns></returns>
    Public Iterator Function PopulateRowObjects(Of DataSet As {New, INamedValue, DynamicPropertyBase(Of Double)})() As IEnumerable(Of DataSet)
        Dim names As String() = Me.names.Objects

        For Each item As SeqValue(Of String) In names.SeqIterator
            Yield New DataSet With {
                .Key = item,
                .Properties = names _
                    .ToDictionary(Function(a) a,
                                  Function(a)
                                      Return Me(a, item.value)
                                  End Function)
            }
        Next
    End Function

    Public Iterator Function PopulateRowEntitys(Of DataSet As {New, INamedValue, DynamicPropertyBase(Of Double)})(propertyNames As String()) As IEnumerable(Of DataSet)
        Dim names As String() = Me.names.Objects

        For i As Integer = 0 To names.Length - 1
            Dim v As Double() = matrix(i)
            Dim propVec As New Dictionary(Of String, Double)

            For j As Integer = 0 To propertyNames.Length - 1
                propVec(propertyNames(j)) = v(j)
            Next

            Yield New DataSet With {
                .Key = names(i),
                .Properties = propVec
            }
        Next
    End Function

    Public Overrides Function ToString() As String
        If names.Count <= 6 Then
            Return names.Objects.GetJson
        Else
            Return "[" & names.Objects.Take(6).JoinBy(", ") & "..."
        End If
    End Function

    Public Shared Narrowing Operator CType(mat As DataMatrix) As NumericMatrix
        Return New NumericMatrix(mat.matrix)
    End Operator

    Public Function GetVector() As IEnumerable Implements IBucketVector.GetVector
        Return PopulateRows.IteratesALL.ToArray
    End Function
End Class
