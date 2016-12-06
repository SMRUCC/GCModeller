Imports System.Text.RegularExpressions

Namespace Framework.Provider

    ''' <summary>
    ''' LINQ entity type
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class LinqEntity : Inherits Attribute

        Public ReadOnly Property Type As String
        Public ReadOnly Property RefType As Type

        Public Shared ReadOnly Property ILinqEntity As Type = GetType(LinqEntity)

        ''' <summary>
        ''' 方法应该申明在模块之中，或者Class之中应该是共享的静态方法
        ''' </summary>
        ''' <param name="type">类型的简称</param>
        ''' <param name="ref">实际引用的类型位置</param>
        Sub New(type As String, ref As Type)
            Me.Type = type
            Me.RefType = ref
        End Sub

        Public Overrides Function ToString() As String
            Return Type
        End Function

        ''' <summary>
        ''' 获取目标类型上的自定义属性中的LINQEntity类型对象中的EntityType属性值
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetEntityType(type As Type) As String
            Dim attr As Object() = type.GetCustomAttributes(ILinqEntity, True)
            If attr.IsNullOrEmpty Then
                Return ""
            Else
                Return DirectCast(attr(Scan0), LinqEntity).Type
            End If
        End Function
    End Class
End Namespace