#Region "Microsoft.VisualBasic::26cd1b71992c8e0a5e01459b55299549, WGCNA\WGCNA\TOM.vb"

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

' Module TOM
' 
'     Function: CreateModules, Intermediate, Matrix
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports stdNum = System.Math

''' <summary>
''' Category 2: Functions for module detection.
''' 
''' Modules are defined as clusters Of densely interconnected genes
'''
''' (TOM矩阵)
''' </summary>
Public Module TOM

    ''' <summary>
    ''' I矩阵
    ''' </summary>
    ''' <param name="A"></param>
    ''' <returns></returns>
    Public Function Intermediate(A As GeneralMatrix) As GeneralMatrix
        Dim Iu As New GeneralMatrix(A.RowDimension, A.ColumnDimension)
        Dim x As Double()() = Iu.Array
        Dim m As Integer = A.RowDimension
        Dim n As Integer = A.ColumnDimension
        Dim alpha As Double()() = A.Array

        For u As Integer = 0 To m - 1
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    x(i)(j) += alpha(i)(u) * alpha(j)(u)
                Next
            Next
        Next

        Return Iu
    End Function

    Public Function Matrix(A As GeneralMatrix, K As Vector) As GeneralMatrix
        Dim Imat As GeneralMatrix = Intermediate(A)
        Dim W As New GeneralMatrix(A.RowDimension, A.ColumnDimension)
        Dim wmat As Double()() = W.Array

        For i As Integer = 0 To wmat.Length - 1
            For j As Integer = 0 To wmat.Length - 1
                wmat(i)(j) = (Imat(i, j) + A(i, j)) / (stdNum.Min(K(i), K(j)) + 1 - A(i, j))
            Next
        Next

        Return W
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="tree"></param>
    ''' <param name="distCut">a percentage threshold value in range ``[0,1]``</param>
    ''' <returns></returns>
    <Extension>
    Friend Function CreateModules(tree As Cluster, Optional distCut As Double = 0.6) As IEnumerable(Of NamedCollection(Of String))
        Return CreateModulesInternal(tree, distCut:=tree.TotalDistance * distCut)
    End Function

    <Extension>
    Private Iterator Function CreateModulesInternal(tree As Cluster, distCut As Double) As IEnumerable(Of NamedCollection(Of String))
        If tree.TotalDistance <= distCut Then
            Dim items As New List(Of String)
            Dim name As String = tree.Name

            If tree.isLeaf Then
                items.Add(tree.Name)
            Else
                For Each child As Cluster In tree.Children
                    Call child _
                        .CreateModulesInternal(distCut) _
                        .Select(Function(m) m.value) _
                        .IteratesALL _
                        .DoCall(AddressOf items.AddRange)
                Next
            End If

            Yield New NamedCollection(Of String)(name, items)
        Else
            For Each child As Cluster In tree.Children
                For Each m As NamedCollection(Of String) In child.CreateModulesInternal(distCut)
                    Yield m
                Next
            Next
        End If
    End Function
End Module

