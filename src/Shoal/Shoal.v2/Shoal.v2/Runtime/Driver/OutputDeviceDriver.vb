Imports System.Drawing
Imports System.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Exceptions

Namespace Runtime.DeviceDriver

    ''' <summary>
    ''' Output support module for that data type.(各种数据类型的输出模块)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OutputDeviceDriver : Inherits DriverBase(Of OutputDeviceHandle, Func(Of Object, Object), Type)

        Sub New(ScriptEngine As Runtime.ScriptEngine)
            MyBase.New(ScriptEngine)
            MyBase._innerHwnds = New Dictionary(Of Type, Func(Of Object, Object)) _
                From
                {
                    {GetType(String), AddressOf OutputDeviceDriver.WriteLineText},
                    {GetType(Generic.IEnumerable(Of String)), AddressOf OutputDeviceDriver.WriteStringCollection},
                    {GetType(Generic.IEnumerable(Of Object)), AddressOf OutputDeviceDriver.WriteObjectCollection},
                    {GetType(Double), AddressOf OutputDeviceDriver.WriteLineText},
                    {GetType(Integer), AddressOf OutputDeviceDriver.WriteLineText},
                    {GetType(UInteger), AddressOf OutputDeviceDriver.WriteLineText},
                    {GetType(Short), AddressOf OutputDeviceDriver.WriteLineText},
                    {GetType(Long), AddressOf OutputDeviceDriver.WriteLineText},
                    {GetType(Date), AddressOf OutputDeviceDriver.WriteLineText},
                    {GetType(Boolean), AddressOf OutputDeviceDriver.WriteLineText},
                    {GetType(Byte), AddressOf OutputDeviceDriver.WriteLineText},
                    {GetType(Object), AddressOf OutputDeviceDriver.WriteLineText}}
        End Sub

        Private Shared Function WriteLineText(s As Object) As Object
            Call Console.WriteLine("    = [0] " & s.ToString)
            Return s
        End Function

        Private Shared Function WriteObjectCollection(data As Generic.IEnumerable(Of Object)) As Object()
            If data.IsNullOrEmpty Then
                Call Console.WriteLine("    = null array")
            End If

            Call WriteStringCollection((From obj In data Select InputHandler.ToString(obj)).ToArray)
            Return data.ToArray
        End Function

        Private Shared Function WriteStringCollection(data As Generic.IEnumerable(Of String)) As String()
            If data.IsNullOrEmpty Then
                Call Console.WriteLine("    = null array")
            End If

            Call Console.WriteLine("    =")
            For i As Integer = 0 To data.Count - 1
                Call Console.WriteLine("      [{0}] {1}", i, data(i))
            Next

            Return data.ToArray
        End Function

        Public Overrides Function ImportsHandler([Module] As Type) As Integer
            Dim Methods As __TYPEHwnd() = GetMethods([Module])

            For Each item As __TYPEHwnd In Methods

                If _innerHwnds.ContainsKey(item.Type) Then
                    Call Console.WriteLine("[UPDATE] {0} ==> {1}::{2}", item.Type.FullName, [Module].FullName, item.MethodInfo.Name)
                    Call _innerHwnds.Remove(item.Type)  '如果已经存在该类型的写入方法，则进行更新
                End If

                Call _innerHwnds.Add(item.Type, value:=Function(data As Object) item.MethodInfo.Invoke(Nothing, New Object() {data}))
                Call __recordHandleTrace(item.Type, item.MethodInfo)
            Next

            Return Methods.Length
        End Function

        Const EXCEPTION_MESSAGE_OUTPUT_HANDLER_MISSING As String = "Output handler method is missing for type: {0}, please try using ""imports <namespace>"" command for imports the io method."
        Const NULL As String = "NULL"

        Public Function HandleOutput(var As Object) As Object
            If var Is Nothing Then var = NULL ' var是空值，则输出空值所指示的字符串

            Dim Type As Type = var.GetType

            If _innerHwnds.ContainsKey(Type) Then
                Dim DriverHandle = _innerHwnds(Type)
                Return DriverHandle(var)
            Else
                Return __processArrayOutput(Type, var)
            End If
        End Function

        Private Function __processArrayOutput(TypeInfo As Type, var As Object) As Object
            Dim GenericCollection As Type = TypeInfo.Collection2GenericIEnumerable

            If _innerHwnds.ContainsKey(GenericCollection) Then
                Return _innerHwnds(GenericCollection)(var)
            Else
                Try
                    Dim cString As String = var.ToString '尝试将目标对象转换为字符串进行输出
                    Call Console.WriteLine(cString)
                    Return cString
                Catch ex As Exception '但是转换失败了，最终只能够跑出错误
                    Dim Msg As String = String.Format(EXCEPTION_MESSAGE_OUTPUT_HANDLER_MISSING, TypeInfo.FullName)
                    ex = New Exception(Msg, ex)
                    Throw New MethodNotFoundException(ex.ToString, ScriptEngine)
                End Try
            End If
        End Function

        Protected Overrides Function HandleEntryToString(item As Type) As String
            Return item.FullName
        End Function
    End Class
End Namespace