Namespace ComponentModel

    Public MustInherit Class IDataEmbedding

        Public MustOverride ReadOnly Property dimension As Integer

        ' Public MustOverride Function LoadMatrix(matrix As IEnumerable(Of Double())) As Integer

        ''' <summary>
        ''' get projection result
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetEmbedding() As Double()()

    End Class
End Namespace