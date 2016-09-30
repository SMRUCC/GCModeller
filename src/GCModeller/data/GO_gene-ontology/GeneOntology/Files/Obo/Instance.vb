Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.foundation.OBO_Foundry

Namespace OBO

    Public Class Instance : Inherits base

        <Field("is_anonymous")> Public Property is_anonymous As String
        <Field("alt_id")> Public Property alt_id As String
        <Field("comment")> Public Property comment As String
        <Field("synonym")> Public Property synonym As String
        <Field("xref")> Public Property xref As String
        <Field("instance_of")> Public Property instance_of As String
        <Field("property_value")> Public Property property_value As String
        <Field("is_obsolete")> Public Property is_obsolete As String
        <Field("replaced_by")> Public Property replaced_by As String
        <Field("consider")> Public Property consider As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace