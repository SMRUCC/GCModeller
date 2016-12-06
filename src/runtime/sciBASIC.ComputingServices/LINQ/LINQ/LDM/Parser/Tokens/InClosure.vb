Imports Microsoft.VisualBasic.Linq.Script
Imports Microsoft.VisualBasic.Linq.Framework
Imports Microsoft.VisualBasic.Linq.Framework.Provider

Namespace LDM.Statements.Tokens

    ''' <summary>
    ''' 从字符串引用
    ''' </summary>
    Public Class UriRef : Inherits InClosure

        Public Property URI As String

        Public Overrides ReadOnly Property Type As SourceTypes
            Get
                Return SourceTypes.FileURI
            End Get
        End Property

        Public Overrides ReadOnly Property IsParallel As Boolean = False

        Sub New(tokens As ClosureTokens, parent As LinqStatement)
            Call MyBase.New(tokens, parent)
            URI = tokens.Tokens.First.TokenValue.GetString
        End Sub

        Public Overrides Function ToString() As String
            Return $"[In] uri:={URI}"
        End Function

        Public Overrides Function GetRepository(handle As GetLinqResource, runtime As DynamicsRuntime) As IEnumerable
            Return handle(URI)
        End Function
    End Class

    ''' <summary>
    ''' 引用运行时环境之中的某一个变量或者执行表达式
    ''' </summary>
    Public Class Reference : Inherits InClosure

        Public Overrides ReadOnly Property Type As SourceTypes
            Get
                Return SourceTypes.Reference
            End Get
        End Property

        Public Overrides ReadOnly Property IsParallel As Boolean
            Get
                Return False  ' Not implements yet
            End Get
        End Property

        Sub New(tokens As ClosureTokens, parent As LinqStatement)
            Call MyBase.New(tokens, parent)
        End Sub

        Public Overrides Function ToString() As String
            Return Source.ToString
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="handle">
        ''' 由于引用类型的数据源是runtime里面的一个变量，所以这个从url获取数据的方法在这里是没有用途的，这个只是为了保持接口的统一性
        ''' </param>
        ''' <param name="runtime"></param>
        ''' <returns></returns>
        ''' <remarks>这里是不是也需要进行动态编译？</remarks>
        Public Overrides Function GetRepository(handle As GetLinqResource, runtime As DynamicsRuntime) As IEnumerable
            Dim name As String = ""

            For Each x In Source.Tokens
                If x.Type = TokenIcer.Tokens.VarRef Then
                    name = x.Text
                    name = Mid(name, 2)

                    Exit For

                ElseIf x.Type = TokenIcer.Tokens.Code Then
                    name = x.Text

                    Exit For

                End If
            Next

            Return runtime.GetResource(name)
        End Function
    End Class

    Public Enum SourceTypes
        ''' <summary>
        ''' 目标集合类型为一个数据文件
        ''' </summary>
        ''' <remarks>只有一个元素，并且为字符串类型</remarks>
        FileURI
        ''' <summary>
        ''' 目标集合类型为一个内存对象的引用
        ''' </summary>
        ''' <remarks>为表达式，含有多个Token</remarks>
        Reference
    End Enum

    ''' <summary>
    ''' 表示目标对象的数据集合的文件路径或者内存对象的引用
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class InClosure : Inherits Closure

        Public MustOverride ReadOnly Property Type As SourceTypes
        Public MustOverride ReadOnly Property IsParallel As Boolean

        Sub New(token As ClosureTokens, parent As LinqStatement)
            Call MyBase.New(token, parent)
        End Sub

        Public Shared Function CreateObject(tokens As ClosureTokens(), parent As LinqStatement) As InClosure
            Dim source As ClosureTokens = Closure.GetTokens(TokenIcer.Tokens.In, tokens)
            If source.Tokens.Length = 1 AndAlso
               source.Tokens(Scan0).TokenName = TokenIcer.Tokens.String Then
                Return New UriRef(source, parent)
            Else
                Return New Reference(source, parent)
            End If
        End Function

        Public MustOverride Function GetRepository(handle As GetLinqResource, runtime As DynamicsRuntime) As IEnumerable
    End Class
End Namespace