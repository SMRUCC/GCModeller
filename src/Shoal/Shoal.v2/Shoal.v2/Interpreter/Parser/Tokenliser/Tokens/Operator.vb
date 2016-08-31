Namespace Interpreter.Parser.Tokens

    ''' <summary>
    ''' 该表达式之中的操作符
    ''' </summary>
    Public Class [Operator] : Inherits Token

        Public Enum Operators As Integer

            NULL = -1

            ''' <summary>
            ''' &lt;- Assign value to variable;
            ''' </summary>
            ValueAssign
            ''' <summary>
            ''' -> Extension method calling;
            ''' </summary>
            ExtCall
            ''' <summary>
            ''' &lt;= Collection and hash table operations;
            ''' </summary>
            CollectionOprOrDelegate
            ''' <summary>
            ''' = Self type cast;
            ''' </summary>
            SelfCast
            ''' <summary>
            ''' &lt;&lt; Hybrids scripting;
            ''' </summary>
            HybridsScript
            ''' <summary>
            ''' >> Setup variable of hybrids scripting;
            ''' </summary>
            HybirdsScriptPush
            ''' <summary>
            ''' => 函数指针
            ''' </summary>
            HashOprOrDelegate

            ''' <summary>
            ''' &lt;
            ''' </summary>
            DynamicsCast
            ''' <summary>
            ''' >
            ''' </summary>
            IODevice
        End Enum

        Public ReadOnly Property Type As [Operator].Operators

        Public Overrides ReadOnly Property TokenType As TokenTypes
            Get
                Return TokenTypes.Operator
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="opr">
        ''' [&lt;- Assign value to variable;]
        ''' 
        ''' 
        ''' [-> Extension method calling;]
        ''' 
        ''' 
        ''' [&lt;= Collection and hash table operations;]
        ''' 
        ''' 
        ''' [= Self type cast;]
        ''' 
        ''' 
        ''' [&lt;&lt; Hybrids scripting;]
        ''' 
        ''' 
        ''' [>> Setup variable of hybrids scripting;]
        ''' 
        ''' </param>
        Sub New(opr As String)
            Call MyBase.New(0, opr)
            Type = GetOperator(opr)
        End Sub

        Public Shared Function GetOperator(opr As String) As Operators
            Select Case Trim(opr)

                Case "<-", "=" : Return Operators.ValueAssign
                Case "->" : Return Operators.ExtCall
                Case "<=" : Return Operators.CollectionOprOrDelegate
                ' Case "=" : Return Operators.ValueAssign
                Case "<<" : Return Operators.HybridsScript
                Case ">>" : Return Operators.HybirdsScriptPush
                Case "=>" : Return Operators.HashOprOrDelegate
                Case "<" : Return Operators.DynamicsCast
                Case ">" : Return Operators.IODevice

                Case Else
                    Return Operators.NULL
                    ' Throw New NotImplementedException($"The operator {NameOf(opr)}:={opr} is currently not support yet!")
            End Select

        End Function

        Public Overrides Function ToString() As String
            Return $"( {_TokenValue } )  {Type.GetType.FullName}.{Type.ToString}"
        End Function
    End Class
End Namespace