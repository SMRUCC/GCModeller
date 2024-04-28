Namespace Attributes

    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class RunApp : Inherits Attribute

        Public ReadOnly Property url As String
        Public ReadOnly Property method As HTTPInternal.AppEngine.APIMethods.APIMethod

        Sub New(app As Integer)
            ' url = app.Description
        End Sub
    End Class
End Namespace
