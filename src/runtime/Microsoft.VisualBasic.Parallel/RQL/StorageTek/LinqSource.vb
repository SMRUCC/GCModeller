Imports System.Reflection
Imports Microsoft.VisualBasic.Linq.Framework.Provider

Namespace StorageTek

    Public Module LinqSource

        Const NO_LINQ As String = "The target function pointer handle have not defined any Linq source entry yet..."

        ''' <summary>
        ''' 生成Linq数据源
        ''' </summary>
        ''' <param name="res"></param>
        ''' <param name="handle"></param>
        ''' <returns></returns>
        Public Function Source(res As String, handle As GetLinqResource) As EntityProvider
            Dim mINFO As MethodInfo = handle.Method
            Dim Linq As TypeEntry = TypeRegistry.ParsingEntry(mINFO)
            If Linq Is Nothing Then
                Dim ex As New Exception(NO_LINQ)
                ex = New DataException(mINFO.GetFullName(True), ex)
                Throw ex
            End If
            Return New EntityProvider(Linq, res)
        End Function
    End Module
End Namespace