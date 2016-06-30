Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace Tables

    Public Class Edge
        Public Property SUID As Integer
        Public Property Confidence As Double
        Public Property EdgeBetweenness As Double
        <Column("interaction name")> Public Property Interaction As String
        <Column("shared interaction")> Public Property SharedInteraction As String
        <Column("shared name")> Public Property SharedName As String

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}", SUID, SharedName)
        End Function
    End Class
End Namespace