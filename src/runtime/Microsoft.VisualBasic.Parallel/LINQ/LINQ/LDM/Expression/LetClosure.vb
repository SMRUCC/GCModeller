Imports System.Text.RegularExpressions
Imports System.Text
Imports System.CodeDom
Imports Microsoft.VisualBasic.LINQ.Framework.Provider
Imports Microsoft.VisualBasic.Linq.LDM.Statements
Imports Microsoft.VisualBasic.LINQ.Extensions
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.CodeDOM_VBC

Namespace LDM.Expression

    ''' <summary>
    ''' Object declared using a LET expression.(使用Let语句所声明的只读对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class LetClosure : Inherits Closure

        Public ReadOnly Property FieldDeclaration As CodeMemberField

        Sub New(source As Statements.Tokens.LetClosure, registry As TypeRegistry)
            Call MyBase.New(source)

            Dim type As Type = __getType(registry)
            FieldDeclaration = Field(source.Name, type.GetType)
            Call __init()
        End Sub

        Private Function __getType(registry As TypeRegistry) As Type
            Dim source As Statements.Tokens.LetClosure =
                DirectCast(_source, Statements.Tokens.LetClosure)
            Dim type As TypeEntry = registry.Find(source.Type)
            If type Is Nothing Then
                ' 尝试系统类型
                Dim typeDef As Type = Scripting.GetType(source.Type, True)
                Return typeDef
            Else
                Return type.GetType
            End If
        End Function

        Public Overrides Function ToString() As String
            Return Me._source.ToString
        End Function

        ''' <summary>
        ''' 在这里解析初始化赋值的表达式
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function __parsing() As CodeExpression
            Dim init As Func(Of TokenIcer.Tokens) =
                DirectCast(_source, Tokens.LetClosure).Expression.Args.First
        End Function
    End Class
End Namespace