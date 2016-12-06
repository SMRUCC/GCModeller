Imports Microsoft.VisualBasic.Linq.Framework
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode
Imports Microsoft.VisualBasic.Linq.Framework.Provider

Namespace LDM.Statements.Tokens

    ''' <summary>
    ''' The init variable.
    ''' </summary>
    Public Class FromClosure : Inherits Closure

        ''' <summary>
        ''' 变量的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name As String
        ''' <summary>
        ''' 变量的类型标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TypeId As String

        Sub New(tokens As ClosureTokens(), parent As LinqStatement)
            Call MyBase.New(TokenIcer.Tokens.From, tokens, parent)

            Name = Source.Tokens(Scan0).TokenValue
            TypeId = Source.Tokens(2).TokenValue
        End Sub

        Public Overloads Function [GetType](defs As TypeRegistry) As Type
            Dim value = defs.Find(TypeId)
            If value Is Nothing Then
                Return Scripting.GetType(TypeId)
            Else
                Dim type As Type = value.GetType

                If Not type Is Nothing Then
                    Return type
                Else
                    Return value.GetType
                End If
            End If
        End Function

        Public Function GetEntityRepository(defs As TypeRegistry) As Provider.GetLinqResource
            Dim handle As GetLinqResource = defs.GetHandle(TypeId)
            If handle Is Nothing Then
                Throw New Exception
            Else
                Return handle
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("Dim {0} As {1}", Name, TypeId)
        End Function
    End Class
End Namespace