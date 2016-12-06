Imports System.CodeDom
Imports Microsoft.VisualBasic.CodeDOM_VBC
Imports Microsoft.VisualBasic.Linq.Framework
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode.VBC
Imports Microsoft.VisualBasic.Linq.Framework.LQueryFramework
Imports Microsoft.VisualBasic.Linq.Framework.Provider

Namespace LDM.Expression

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
        Public ReadOnly Property Name As String
        ''' <summary>
        ''' 变量的类型标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeId As String

        Public ReadOnly Property RegistryType As TypeEntry

        Sub New(source As Statements.Tokens.FromClosure, registry As TypeRegistry)
            Call MyBase.New(source)

            Me.Name = source.Name
            Me.TypeId = source.TypeId
            Me.RegistryType = registry.Find(source.TypeId)

            Dim fieldType As Type = RegistryType.GetType
            Me.DeclaredField = CodeDOMExpressions.Field(Name, fieldType)

            Call __init()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("Dim {0} As {1}", Name, TypeId)
        End Function

        Public ReadOnly Property DeclaredField As CodeMemberField

        ''' <summary>
        ''' 解析为类的域
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function __parsing() As CodeExpression
            Dim code As CodeExpression = CodeDOMExpressions.FieldRef(Name)
            Return code
        End Function
    End Class
End Namespace