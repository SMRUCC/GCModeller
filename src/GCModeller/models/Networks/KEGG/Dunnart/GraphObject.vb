Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Dunnart

    ''' <summary>
    ''' json model for visualize of the cola network graph
    ''' </summary>
    Public Class GraphObject

        Public Property nodes As Node()
        Public Property links As Link()
        Public Property constraints As Constraint()
        Public Property groups As Group()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace