Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Scripting.Rscript

Public Module Clustering

    Public Function CreatePhenoGraph(data As DataSet(), Optional k As Integer = 30)

    End Function

    Public Function CreatePhenoGraph(data As GeneralMatrix, Optional k As Integer = 30)
        If k < 1 Then
            Throw New ArgumentException("k must be a positive integer!")
        ElseIf k > data.RowDimension - 2 Then
            Throw New ArgumentException("k must be smaller than the total number of points!")
        End If

        message("Run Rphenograph starts:", "\n",
        "  -Input data of ", nrow(data), " rows and ", ncol(data), " columns", "\n",
        "  -k is set to ", k)
    End Function
End Module
