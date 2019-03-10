Imports System.Text

Namespace Runtime.Objects.ObjectModels.Exceptions

    ''' <summary>
    ''' Could not found the method entry point in the registry.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MethodNotFoundException : Inherits ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ShoalScriptException

        Const METHOD_NOT_FOUND As String = "Could not found the method entry point for ""{0}""!"

        Public Property MethodName As String
        Public Property Info As String

        Sub New(MethodName As String, Details As String)
            Call MyBase.New(String.Format(METHOD_NOT_FOUND, MethodName))
            Me.Info = Details
            Me.MethodName = MethodName
        End Sub

        Public Overrides ReadOnly Property ExceptionType As String
            Get
                Return NameOf(MethodNotFoundException)
            End Get
        End Property
    End Class
End Namespace