Imports System.CodeDom
Imports Microsoft.VisualBasic.LINQ.Framework
Imports Microsoft.VisualBasic.LINQ.Framework.Provider
Imports Microsoft.VisualBasic.LINQ.Script
Imports Microsoft.VisualBasic.Linq.LDM.Statements.Tokens

Namespace LDM.Expression

    ''' <summary>
    ''' 表示目标对象的数据集合的文件路径或者内存对象的引用
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InClosure : Inherits Closure

        Public ReadOnly Property Type As SourceTypes

        Public ReadOnly Property IsParallel As Boolean

        Dim handle As GetLinqResource
        Dim resource As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source">[in] var -> parallel</param>
        Sub New(source As Statements.Tokens.InClosure, from As FromClosure, registry As TypeRegistry)
            Call MyBase.New(source)

            If source.Type = SourceTypes.FileURI Then
                resource = DirectCast(source, UriRef).URI
                handle = registry.GetHandle(from.TypeId)
            Else
                resource = DirectCast(source, Reference).Source.Tokens(Scan0).TokenValue
            End If
        End Sub

        Public Function GetResource(runtime As DynamicsRuntime) As IEnumerable
            If Type = SourceTypes.FileURI Then
                Return handle(resource)
            Else
                Return runtime.GetResource(Me.resource)
            End If
        End Function

        Protected Overrides Function __parsing() As CodeExpression

        End Function
    End Class
End Namespace