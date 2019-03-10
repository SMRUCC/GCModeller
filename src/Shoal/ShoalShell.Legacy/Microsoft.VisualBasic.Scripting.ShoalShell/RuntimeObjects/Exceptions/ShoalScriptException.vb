Imports System.Text

Namespace Runtime.Objects.ObjectModels.Exceptions

    Public MustInherit Class ShoalScriptException : Inherits Exception

        Public Property LineNumber As Integer
        Public Property ScriptLine As String

        Public MustOverride ReadOnly Property ExceptionType As String

        Dim InnerExceptionMessage As String

        Public ReadOnly Property ExceptionInformation As String
            Get
                Return InnerExceptionMessage
            End Get
        End Property

        Sub New(Message As String)
            MyBase.New(Message)
            InnerExceptionMessage = Message
        End Sub
    End Class
End Namespace