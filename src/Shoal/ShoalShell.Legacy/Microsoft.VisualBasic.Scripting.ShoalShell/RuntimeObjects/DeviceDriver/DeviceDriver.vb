Imports System.Drawing
Imports System.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels.Exceptions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DeviceDriver.DriverHandles

Namespace DeviceDriver

    ''' <summary>
    ''' 字典之中的Key属性分别为所需要转换的目标类型的简称和输入的第一个参数的类型,目标方法只能够有一个参数
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InputDeviceDriver : Inherits TypeHandleEntryPointDriverrModule(Of InputDeviceHandle, Func(Of Object, Object), KeyValuePair(Of String, Type))

        Sub New(Memory As ShoalShell.Runtime.Objects.I_MemoryManagementDevice)
            Call MyBase.New(Memory.ScriptEngine)
            MyBase._InternalHandles = New Dictionary(Of KeyValuePair(Of String, Type), Func(Of Object, Object)) _
                From
                {
                    {New KeyValuePair(Of String, Type)("string", GetType(String)), AddressOf InputDeviceDriver.InternalReadTextFile},
                    {New KeyValuePair(Of String, Type)("string()", GetType(String)), AddressOf IO.File.ReadAllLines},
                    {New KeyValuePair(Of String, Type)("byte()", GetType(String)), AddressOf IO.File.ReadAllBytes},
                    {New KeyValuePair(Of String, Type)("int", GetType(String)), AddressOf InputDeviceDriver.InternalGetInteger},
                    {New KeyValuePair(Of String, Type)("int32", GetType(String)), AddressOf InputDeviceDriver.InternalGetInteger},
                    {New KeyValuePair(Of String, Type)("integer", GetType(String)), AddressOf InputDeviceDriver.InternalGetInteger}}
        End Sub

        <InputDeviceHandle("integer")>
        <InputDeviceHandle("int32")>
        <InputDeviceHandle("int")>
        Private Shared Function InternalGetInteger(value As String) As Integer
            Return CInt(Val(value))
        End Function

        <InputDeviceHandle("string")>
        Private Shared Function InternalReadTextFile(path As String) As String
            Return FileIO.FileSystem.ReadAllText(path)
        End Function

        <InputDeviceHandle("string()")>
        Private Shared Function InternalReadTextLines(path As String) As String()
            Return IO.File.ReadAllLines(path).ToArray
        End Function

        Public Overrides Function ImportsHandler([Module] As Type) As Integer
            Dim Entries = GetMethods([Module])
            Dim LQuery = (From item As TypeHandle In Entries
                          Let n = item.Handle
                          Let Method = item.MethodInfo
                          Let p = Method.GetParameters
                          Where p.Count = 1
                          Let objType = p.First.ParameterType
                          Let DeviceHandle = Function(obj As Object) Method.Invoke(Nothing, {obj})
                          Select DriverHandle = DeviceHandle, Entry = New KeyValuePair(Of String, Type)(n.TypeHandleId, objType), Mounts = Method).ToArray

            For Each Line In LQuery
                If _InternalHandles.ContainsKey(Line.Entry) Then
                    Call _InternalHandles.Remove(Line.Entry)
                End If

                Call _InternalHandles.Add(Line.Entry, Line.DriverHandle)
                Call _InternalRecordHandleTrace(Line.Entry, Line.Mounts)
            Next

            Return LQuery.Count
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj">目标函数的第一个参数，本方法会根据这个参数以及<paramref name="typeId"></paramref>参数查找最合适的句柄来处理本参数的输入</param>
        ''' <param name="typeId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TypeCasting(obj As Object, typeId As String) As Object
            Dim EntryType As System.Type = obj.GetType
            Dim LQuery = (From Hwnd As KeyValuePair(Of KeyValuePair(Of String, Type), Func(Of Object, Object))
                          In Me._InternalHandles
                          Where String.Equals(Hwnd.Key.Key, typeId, StringComparison.OrdinalIgnoreCase)
                          Select Val = Hwnd.Value).ToArray

            If LQuery.IsNullOrEmpty Then
                Dim exMsg As String = String.Format(MISSING_HANDLER, typeId, EntryType.FullName)
                Throw New MissingPrimaryKeyException(exMsg)
            End If

            Dim DriverHandle As Func(Of Object, Object) = LQuery.First
            Dim value As Object = DriverHandle(obj)
            Return value
        End Function

        Const MISSING_HANDLER As String = "[MISSING_HANDLER  {0}, {1}] You should imports a namespace which was contains the handle of this type and then run this program again."

        Protected Overrides Function HandleEntryToString(item As KeyValuePair(Of String, Type)) As String
            Return String.Format("[{0}, {1}]", item.Key, item.Value.FullName)
        End Function
    End Class
End Namespace