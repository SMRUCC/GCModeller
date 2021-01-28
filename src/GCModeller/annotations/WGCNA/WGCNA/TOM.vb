
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
''' <summary>
''' Category 2: Functions for module detection.
''' 
''' Modules are defined as clusters Of densely interconnected genes
'''
''' (TOM矩阵)
''' </summary>
Public Module TOM

    Public Function Intermediate(A As GeneralMatrix) As GeneralMatrix
        Dim Iu As New GeneralMatrix(A.RowDimension, A.ColumnDimension)
        Dim x As Double()() = Iu.Array
        Dim m As Integer = A.RowDimension
        Dim n As Integer = A.ColumnDimension
        Dim buffer As Double()() = A.Array

        For i As Integer = 0 To m - 1
            For j As Integer = 0 To n - 1
                x(i)(j) = buffer(i)(j)
            Next
        Next

        Return Iu
    End Function
End Module
