Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects

Module CommonExtensions

    ' ''' <summary>
    ' ''' 为整个ShellScript提供LINQ查询支持的LINQ运行时环境
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Public ReadOnly LINQFramework As LINQ.Framework.LQueryFramework = New LINQ.Framework.LQueryFramework(String.Format("{0}/LINQ.Framework.TypeDef.xml", My.Application.Info.DirectoryPath))

    Public Function GetPrintValue([Obj] As Object) As String()
        Dim Type As ObjectTypes = [GetType](Obj)
        If Type = ObjectTypes.Array Then
            Dim DataArray = I_MemoryManagementDevice.GetArrayValue(DataSource:=DirectCast(Obj, IEnumerable))
            Dim LQuery = (From item In DataArray Let value As String = item.ToString Select value).ToArray
            Return LQuery
        Else
            Return New String() {Obj.ToString}
        End If
    End Function

    Public Function CreatePrintValue(strData As String()) As String
        If strData.IsNullOrEmpty Then
            Return ""
        End If

        Dim sBuilder As StringBuilder =
            New StringBuilder("[0]  " & strData.First & vbCrLf, capacity:=1024)
        For i As Integer = 1 To strData.Count - 1
            Call sBuilder.AppendLine(String.Format("[{0}]  {1}", i, strData(i)))
        Next

        Return sBuilder.ToString
    End Function

    Public Enum ObjectTypes
        Value
        Array
        Reference
    End Enum

    Private ReadOnly TYPE_IENUMERABLE As System.Type = GetType(IEnumerable)
    Private ReadOnly TYPE_STRING As System.Type = GetType(String)

    Public Function [GetType](obj As Object) As ObjectTypes
        Dim Type As System.Type = obj.GetType

        If IsGenericEnumerable(Type) OrElse Type.IsArray Then
            Return ObjectTypes.Array
        ElseIf Type.IsClass AndAlso Not Type Is TYPE_STRING Then
            Return ObjectTypes.Reference
        Else
            Return ObjectTypes.Value
        End If
    End Function

    ''' <summary>
    ''' 判断目标类型是否为一个Array类型
    ''' </summary>
    ''' <param name="Type"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsGenericEnumerable(Type As Type) As Boolean
        Dim IsGenericType = Type.IsGenericType
        Dim p = Array.IndexOf(Type.GetInterfaces, TYPE_IENUMERABLE)
        Dim f = IsGenericType AndAlso p > -1
        Return f
    End Function
End Module
