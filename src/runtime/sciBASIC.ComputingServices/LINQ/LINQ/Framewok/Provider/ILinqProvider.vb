Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Framework.Provider

    ''' <summary>
    ''' Get a Collection of the target LINQ entity from file object.(从文件对象获取目标LINQ实体对象的集合)
    ''' </summary>
    ''' <param name="uri">File path or resource from url</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Delegate Function GetLinqResource(uri As String) As IEnumerable

    Public Module RegistryReader

        ''' <summary>
        ''' 数据源的示例函数
        ''' </summary>
        ''' <param name="uri"></param>
        ''' <returns></returns>
        <LinqEntity("typeDef", GetType(TypeEntry))>
        Public Function GetResource(uri As String) As IEnumerable
            Dim registry As TypeRegistry = TypeRegistry.Load(uri)
            Return registry.typeDefs
        End Function

        ''' <summary>
        ''' 获取数据源的读取接口
        ''' </summary>
        ''' <param name="provider"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetHandle(provider As TypeEntry) As GetLinqResource
            Try
                Dim type As Type = provider.Repository.GetType
                Dim method As MethodInfo = type.GetMethod(provider.Func, types:={GetType(String)})
                Dim [delegate] As New DelegateProvider(method)
                Dim handle As GetLinqResource = AddressOf [delegate].GetLinqResource
                Return handle
            Catch ex As Exception
                ex = New Exception(provider.name, ex)
                ex = New Exception(provider.GetJson, ex)
                Call App.LogException(ex)
                Return Nothing
            End Try
        End Function
    End Module

    Public Class DelegateProvider

        Public ReadOnly Property MethodInfo As MethodInfo

        ''' <summary>
        ''' 函数句柄
        ''' </summary>
        ''' <param name="h"></param>
        Sub New(h As MethodInfo)
            MethodInfo = h
        End Sub

        ''' <summary>
        ''' 实现数据源的读取接口
        ''' </summary>
        ''' <param name="uri"></param>
        ''' <returns></returns>
        Public Function GetLinqResource(uri As String) As IEnumerable
            Dim value As Object = MethodInfo.Invoke(Nothing, {uri})
            Return DirectCast(value, IEnumerable)
        End Function

        Public Overrides Function ToString() As String
            Return MethodInfo.FullName(True)
        End Function
    End Class
End Namespace