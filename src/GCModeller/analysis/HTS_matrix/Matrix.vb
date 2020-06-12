Imports Microsoft.VisualBasic.Linq

Public Class Matrix

    ''' <summary>
    ''' sample id of <see cref="DataFrameRow.experiments"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property sampleID As String()

    ''' <summary>
    ''' gene list
    ''' </summary>
    ''' <returns></returns>
    Public Property expression As DataFrameRow()

    Public Shared Iterator Function TakeSamples(data As DataFrameRow(), sampleVector As Integer(), reversed As Boolean) As IEnumerable(Of DataFrameRow)
        Dim samples As Double()

        For Each x As DataFrameRow In data
            samples = x.experiments.Takes(sampleVector, reversed:=reversed)

            Yield New DataFrameRow With {
                .geneID = x.geneID,
                .experiments = samples
            }
        Next
    End Function
End Class


