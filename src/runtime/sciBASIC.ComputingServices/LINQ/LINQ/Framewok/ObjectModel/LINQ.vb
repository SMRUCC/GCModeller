Imports Microsoft.VisualBasic.Linq.Script
Imports Microsoft.VisualBasic.Linq.LDM.Statements
Imports Microsoft.VisualBasic.Linq.LDM.Statements.Tokens
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode.LinqClosure
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode

Namespace Framework.ObjectModel

    ''' <summary>
    ''' LINQ查询表达式的对象模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Linq : Implements System.IDisposable

        Protected ReadOnly __linq As LinqStatement
        Protected ReadOnly __runtime As DynamicsRuntime
        Protected ReadOnly __project As IProject

        Public ReadOnly Property [Error] As String

        Sub New(Expr As LinqStatement, Runtime As DynamicsRuntime)
            __linq = Expr
            __runtime = Runtime
            __project = __runtime.Compiler.Compile(Expr, [Error])
        End Sub

        Protected Function __getSource() As IEnumerable
            Dim handle = __linq.var.GetEntityRepository(__runtime.Types)
            Return __linq.source.GetRepository(handle, __runtime)
        End Function

        Public Overridable Function EXEC() As IEnumerable
            Dim Linq = (From x As Object In __getSource()
                        Let value As LinqValue = __project(x)
                        Where value.IsTrue
                        Select value.Projects)
            Return Linq
        End Function

        Public Overrides Function ToString() As String
            Return __linq.ToString
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace