Imports Microsoft.VisualBasic.Serialization.JSON

Namespace OBO

    Public Class Instance : Inherits base

        Public Property is_anonymous As String
        Public Property alt_id As String
        Public Property comment As String
        Public Property synonym As String
        Public Property xref As String
        Public Property instance_of As String
        Public Property property_value As String
        Public Property is_obsolete As String
        Public Property replaced_by As String
        Public Property consider As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace