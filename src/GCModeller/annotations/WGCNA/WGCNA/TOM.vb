Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
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

    <Extension>
    Friend Iterator Function CreateModules(tree As Cluster) As IEnumerable(Of NamedCollection(Of String))

    End Function
End Module
