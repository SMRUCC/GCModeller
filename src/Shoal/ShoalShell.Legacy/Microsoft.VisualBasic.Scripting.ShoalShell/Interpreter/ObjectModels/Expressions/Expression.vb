Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.Tokens

Namespace Interpreter.ObjectModels.Statements

    Public MustInherit Class Statement

        Public MustOverride ReadOnly Property TypeID As Statement.Types
        ''' <summary>
        ''' 本句代码在脚本之中的原始的行数
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalLineNumber As Integer
        ''' <summary>
        ''' 语句后面所出现的注释信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Comments As String '语句后面所出现的注释信息

        Protected _originalExpression As String

        ''' <summary>
        ''' 原始的表达式字符串
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Expression As String
            Get
                Return _originalExpression
            End Get
        End Property

        Sub New(Expression As String)
            _originalExpression = Expression
        End Sub

        Public Overrides Function ToString() As String
            Return _originalExpression
        End Function

        Public Enum Types
            Comment
            BlankCode

            Expression
            [GoTo]
            GotoTag
            [Imports]
            SyntaxError
            OutputRef
            MethodCalling
            OnErrorResumeNext
            Library
            Include

            VariableDeclaration
        End Enum
    End Class

    ''' <summary>
    ''' Dynamics install a external module in the runtime.
    ''' </summary>
    Public Class Library : Inherits Statement

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.Library
            End Get
        End Property

        Public Property Assembly As String

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class

    Public Class Include : Inherits Statement

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.Include
            End Get
        End Property

        ''' <summary>
        ''' The file path of the external script
        ''' </summary>
        ''' <returns></returns>
        Public Property ExternalScript As String

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class

    Public Class SyntaxError : Inherits Statement

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.SyntaxError
            End Get
        End Property

        Dim _Ex As String

#Region "If any one of the property in this region is true, that means the line of this code is not a True syntax error, it just can't be parsing as any statement expression in the shoal language."

        ''' <summary>
        ''' 对于空白行，是无法被解析出来的，但是空白行不是语法错误
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsBlankLineSyntax As Boolean
            Get
                Return String.IsNullOrEmpty(MyBase._originalExpression.TrimA)
            End Get
        End Property

        Public ReadOnly Property IsCommentsSyntax As Boolean
            Get
                If IsBlankLineSyntax Then
                    Return False
                End If

                Return Trim(_originalExpression).First = "#"c
            End Get
        End Property
#End Region

        Public ReadOnly Property IsSyntaxError As Boolean
            Get
                Return Not (IsBlankLineSyntax OrElse IsCommentsSyntax)
            End Get
        End Property

        Sub New(ex As String, Expression As String)
            Call MyBase.New(Expression)
            _Ex = If(IsBlankLineSyntax OrElse IsCommentsSyntax, Expression, ex)
        End Sub

        Public Overrides Function ToString() As String
            Return _Ex & "  at line: " & Me.OriginalLineNumber
        End Function
    End Class

    ''' <summary>
    ''' 解析出来的表达式之中只含有一个词元，并且不是注释，则默认认为是变量查看操作，值默认赋值给系统变量$
    ''' </summary>
    Public Class OutputRef : Inherits Statement

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.OutputRef
            End Get
        End Property

        Public ReadOnly Property InnerExpression As Tokens.InternalExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
            _InnerExpression = New InternalExpression(Expression)
        End Sub
    End Class

    Public Class VariableDeclaration : Inherits Statement

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.VariableDeclaration
            End Get
        End Property

        Public Property Variable As String
        ''' <summary>
        ''' 变量的类型约束
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As String
        Public Property InterExpression As InternalExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class

    ''' <summary>
    ''' Goto语句
    ''' </summary>
    Public Class [GOTO] : Inherits Statement

        'GOTO <FLAG> When <boolean_expression>
        'GOTO 33 When {$boolean}

        Public Property BooleanExpression As Tokens.InternalExpression
        Public Property GotoFlag As Tokens.InternalExpression

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.GoTo
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Overrides Function ToString() As String
            Return $"GOTO {GotoFlag} When {BooleanExpression.ToString}"
        End Function

    End Class

    ''' <summary>
    ''' Goto的标签
    ''' </summary>
    Public Class GotoTag : Inherits Statement

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.GotoTag
            End Get
        End Property

        Public Property TagData As String

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class [Imports] : Inherits Statement

        ''' <summary>
        ''' 字符串常量或者内部表达式
        ''' </summary>
        ''' <returns></returns>
        Public Property [Namespace] As String

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.Imports
            End Get
        End Property

        Public ReadOnly Property IsNamespaceInternalExpression As Boolean
            Get
                Return [Namespace].First = "{"c AndAlso [Namespace].Last = "}"c
            End Get
        End Property

        Public Function Execute(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript) As Boolean

            Dim nsValue As String = [Namespace]

            If IsNamespaceInternalExpression Then
                nsValue = New InternalExpression(nsValue).GetValue(ScriptEngine)
            End If

            Return ScriptEngine.InternalEntryPointManager.ImportsNamespace(nsValue) > 0
        End Function
    End Class

    Public Class OnErrorResumeNext : Inherits Statement

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.OnErrorResumeNext
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Shared Function IsOnErrorResumeNext(Tokens As Tokens.Token()) As Boolean

            If Tokens.Count <> 4 Then
                Return False
            End If

            Dim Flag As String() = {"On", "Error", "Resume", "Next"}

            For p As Integer = 0 To Flag.Count - 1
                If Not String.Equals(Tokens(p).GetTokenValue, Flag(p), StringComparison.OrdinalIgnoreCase) Then
                    Return False
                End If
            Next

            Return True
        End Function
    End Class
End Namespace