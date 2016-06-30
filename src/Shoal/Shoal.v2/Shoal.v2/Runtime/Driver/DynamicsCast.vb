Imports System.Drawing
Imports System.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles

Namespace Runtime.DeviceDriver

    ''' <summary>
    ''' 字典之中的Key属性分别为所需要转换的目标类型的简称和输入的第一个参数的类型,目标方法只能够有一个参数
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DynamicsCast : Inherits DriverBase(Of
            InputDeviceHandle,
            Func(Of Object, Object),
            KeyValuePair(Of String, Type))

        Sub New(ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New(ScriptEngine)
            MyBase._innerHwnds =
                New Dictionary(Of KeyValuePair(Of String, Type), Func(Of Object, Object)) From {
 _
                    {New KeyValuePair(Of String, Type)("string", GetType(String)), AddressOf DynamicsCast.__readText},
                    {New KeyValuePair(Of String, Type)("string()", GetType(String)), AddressOf IO.File.ReadAllLines},
                    {New KeyValuePair(Of String, Type)("byte()", GetType(String)), AddressOf IO.File.ReadAllBytes},
                    {New KeyValuePair(Of String, Type)("int", GetType(String)), AddressOf DynamicsCast.__getInteger},
                    {New KeyValuePair(Of String, Type)("int32", GetType(String)), AddressOf DynamicsCast.__getInteger},
                    {New KeyValuePair(Of String, Type)("integer", GetType(String)), AddressOf DynamicsCast.__getInteger}}
        End Sub

        <InputDeviceHandle("integer")>
        <InputDeviceHandle("int32")>
        <InputDeviceHandle("int")>
        Private Shared Function __getInteger(value As String) As Integer
            Return CInt(Val(value))
        End Function

        <InputDeviceHandle("string")>
        Private Shared Function __readText(path As String) As String
            Return VisualBasic.FileIO.FileSystem.ReadAllText(path)
        End Function

        <InputDeviceHandle("string()")>
        Private Shared Function InternalReadTextLines(path As String) As String()
            Return IO.File.ReadAllLines(path).ToArray
        End Function

        Public Overrides Function ImportsHandler([Module] As Type) As Integer
            Dim Entries = GetMethods([Module])
            Dim LQuery = (From hwnd As __TYPEHwnd In Entries
                          Let n = hwnd.Handle
                          Let Method = hwnd.MethodInfo
                          Let p = Method.GetParameters
                          Where p.Length = 1
                          Let objType = p.First.ParameterType
                          Let DeviceHandle = Function(obj As Object) Method.Invoke(Nothing, {obj})
                          Select DriverHandle = DeviceHandle,
                              Entry = New KeyValuePair(Of String, Type)(n.TypeHandleId, objType),
                              Mounts = Method).ToArray

            For Each Line In LQuery
                If _innerHwnds.ContainsKey(Line.Entry) Then
                    Call _innerHwnds.Remove(Line.Entry)
                End If

                If Line.Entry.Value.Equals(GetType(String)) Then
                    Call InputHandler.CapabilityPromise(
                        Line.Entry.Key,
                        Line.Mounts.ReturnType,
                        Function(s) Line.Mounts.Invoke(Nothing, {s}))
                End If

                Call _innerHwnds.Add(Line.Entry, Line.DriverHandle)
                Call __recordHandleTrace(Line.Entry, Line.Mounts)
            Next

            Return LQuery.Length
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj">目标函数的第一个参数，本方法会根据这个参数以及<paramref name="typeId"></paramref>参数查找最合适的句柄来处理本参数的输入</param>
        ''' <param name="typeId">类型标记信息的简写</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TypeCastDynamics(obj As Object, typeId As String) As Object
            Dim EntryType As System.Type = obj.GetType
            Dim LQuery = (From Hwnd As KeyValuePair(Of KeyValuePair(Of String, Type), Func(Of Object, Object))
                          In Me._innerHwnds
                          Where String.Equals(Hwnd.Key.Key, typeId, StringComparison.OrdinalIgnoreCase)
                          Select Val = Hwnd.Value).ToArray

            If LQuery.IsNullOrEmpty Then
                Dim ex As String = String.Format(MISSING_HANDLE, typeId, EntryType.FullName)
                Throw New MissingPrimaryKeyException(ex)
            End If

            Dim DriverHandle As Func(Of Object, Object) = LQuery.First
            Dim value As Object = DriverHandle(obj)
            Return value
        End Function

        Const MISSING_HANDLE As String = "[MISSING_HANDLER  {0}, {1}] You should imports a namespace which was contains the handle of this type and then run this program again."

        Protected Overrides Function HandleEntryToString(hwnd As KeyValuePair(Of String, Type)) As String
            Return $"[{hwnd.Key}, {hwnd.Value.FullName}]"
        End Function
    End Class
End Namespace