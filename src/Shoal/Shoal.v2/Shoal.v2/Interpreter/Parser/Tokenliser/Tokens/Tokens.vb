Namespace Interpreter.Parser.Tokens

    ''' <summary>
    ''' 一个表达式对象之中的某一个单词元素
    ''' </summary>
    Public Class Token : Inherits MachineElement

        Public Property DepthLevel As Integer
        ''' <summary>
        ''' 产生<see cref="DepthLevel"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property OprTag As Char

        Public Overridable ReadOnly Property TokenType As TokenTypes
            Get
                Return TokenTypes.Undefine
            End Get
        End Property

        Protected _TokenValue As String

        Public ReadOnly Property IsNullOrSpace As Boolean
            Get
                Return String.IsNullOrEmpty(_TokenValue) OrElse
                    String.IsNullOrEmpty(Trim(_TokenValue)) OrElse
                    String.Equals(_TokenValue, vbCr) OrElse
                    String.Equals(_TokenValue, vbLf) OrElse
                    String.Equals(_TokenValue, vbTab)
            End Get
        End Property

        Sub New(Level As Integer, s_Token As String)
            DepthLevel = Level
            _TokenValue = s_Token
        End Sub

        Public Sub Append(ch As String)
            _TokenValue &= ch
        End Sub

        Public Overrides Function ToString() As String
            If DepthLevel = 0 Then

                Return "+  " & _TokenValue '最顶层的调用

            Else
                Return $"{New String(vbTab, DepthLevel)} -> {_TokenValue}"
            End If
        End Function

        ''' <summary>
        ''' 获取得到原始的词元数据
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetTokenValue() As String
            Return _TokenValue
        End Function

        ''' <summary>
        ''' 假若是内部表达式或者变量引用的话，则可以使用这个方法来修建掉两边的括号或者引用的前导符号
        ''' </summary>
        ''' <returns></returns>
        Public Function GetTrimExpr() As String
            If Len(_TokenValue) <= 2 Then
                Return __getTrimVarRef()
            End If

            If (_TokenValue.First = "{"c AndAlso _TokenValue.Last = "}"c) OrElse
                (_TokenValue.First = "("c AndAlso _TokenValue.Last = ")"c) OrElse
                (_TokenValue.First = """"c OrElse _TokenValue.Last = """"c) Then

                Return Mid(_TokenValue, 2, Len(_TokenValue) - 2)
            Else
                Return __getTrimVarRef()
            End If
        End Function

        Private Function __getTrimVarRef() As String
            If _TokenValue.First = "$"c OrElse
                _TokenValue.First = "&"c OrElse
                _TokenValue.First = "*"c Then

                Return Mid(_TokenValue, 2)
            Else
                Return _TokenValue
            End If
        End Function

        Public Overrides Function ExceptionExpr() As String
            Return Me.ToString
        End Function
    End Class
End Namespace