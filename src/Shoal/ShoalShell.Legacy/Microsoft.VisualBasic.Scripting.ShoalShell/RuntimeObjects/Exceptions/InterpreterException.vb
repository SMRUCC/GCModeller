Imports System.Text

Namespace Runtime.Objects.ObjectModels.Exceptions

    Public Class InterpreterException : Inherits ShoalScriptException

        Sub New(Message As String)
            Call MyBase.New(Message)
        End Sub

        Public Overrides ReadOnly Property ExceptionType As String
            Get
                Return GetType(InterpreterException).FullName
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Syntax error in the shoal shell interpreter script text parsing.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SyntaxErrorException : Inherits InterpreterException

        Sub New(message As String)
            Call MyBase.New(message)
        End Sub

        Public Overrides ReadOnly Property ExceptionType As String
            Get
                Return GetType(SyntaxErrorException).FullName
            End Get
        End Property
    End Class
End Namespace