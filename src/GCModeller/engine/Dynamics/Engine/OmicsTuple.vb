Namespace Engine

    Public Class OmicsTuple(Of T)

        Public ReadOnly transcriptome As T
        Public ReadOnly proteome As T
        Public ReadOnly metabolome As T

        Sub New(transcriptome As T, proteome As T, metabolome As T)
            Me.transcriptome = transcriptome
            Me.proteome = proteome
            Me.metabolome = metabolome
        End Sub

        Public Overrides Function ToString() As String
            Return GetType(T).Name
        End Function
    End Class
End Namespace