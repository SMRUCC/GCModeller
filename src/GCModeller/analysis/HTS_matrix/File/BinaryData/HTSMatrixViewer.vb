Imports System.Runtime.CompilerServices

Public Class HTSMatrixViewer : Inherits MatrixViewer

    ReadOnly matrix As Matrix

    Public Overrides ReadOnly Property SampleIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return matrix.sampleID
        End Get
    End Property

    Public Overrides ReadOnly Property FeatureIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return matrix.rownames
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(matrix As Matrix)
        Me.matrix = matrix
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSampleOrdinal(sampleID As String) As Integer
        Return matrix.sampleID.IndexOf(sampleID)
    End Function

    Public Overrides Function GetGeneExpression(geneID As String) As Double()
        Dim gene As DataFrameRow = matrix.gene(geneID)

        If gene Is Nothing Then
            Return New Double(matrix.sampleID.Length - 1) {}
        Else
            Return gene.experiments
        End If
    End Function
End Class