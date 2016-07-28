Imports Microsoft.VisualBasic.Linq.LDM.Statements.TokenIcer
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace LDM.Parser


    ''' <summary>
    ''' Represents a token that is parsed out by the <see cref="Tokenizer"/>.
    ''' </summary>
    Public NotInheritable Class Token : Inherits Token(Of Tokens)

        ''' <summary>
        ''' If the token can be parsed into a type like an integer, this property holds that value.
        ''' </summary>
        Public ReadOnly Property ParsedObject() As Object

        ''' <summary>
        ''' Token priority
        ''' </summary>
        Public ReadOnly Property Priority() As TokenPriority

        ''' <summary>
        ''' Constructor for tokens that are not parsed.
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="type"></param>
        ''' <param name="priority"></param>
        Public Sub New(text As String, type As Tokens, priority As TokenPriority)
            Call MyBase.New(type, text)
            _Priority = priority
            _ParsedObject = text
        End Sub

        Sub New(source As Token(Of Tokens), priority As TokenPriority)
            Call Me.New(source.TokenValue, source.TokenName, priority)
        End Sub

        Sub New(source As Token(Of Tokens))
            Call Me.New(source.TokenValue, source.TokenName, TokenPriority.None)
        End Sub

        ''' <summary>
        ''' Constructor for tokens that are parsed.
        ''' </summary>
        ''' <param name="parsedObj"></param>
        ''' <param name="type"></param>
        ''' <param name="priority"></param>
        Public Sub New(parsedObj As Object, type As Tokens, priority As TokenPriority)
            Call MyBase.New(type, Scripting.ToString(parsedObj))
            _ParsedObject = parsedObj
            _Priority = priority
        End Sub

        ''' <summary>
        ''' The null token represents a state where the <see cref="Tokenizer"/> encountered an error
        ''' or has not begun parsing yet.
        ''' </summary>
        Public Shared ReadOnly Property NullToken As New Token("", Tokens.UNDEFINED, TokenPriority.None)

        Public Overrides Function ToString() As String
            Return MyBase.ToString()
        End Function
    End Class
End Namespace