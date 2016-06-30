Imports System.Drawing
Imports System.Reflection

Namespace Runtime.DeviceDriver.DriverHandles

    Public MustInherit Class HwndEntryPoint : Inherits Attribute

        Public ReadOnly Property SupportType As Type

        Sub New(dataType As Type)
            _SupportType = dataType
        End Sub
    End Class

    ''' <summary>
    ''' 将目标变量在终端进行输出
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class OutputDeviceHandle : Inherits HwndEntryPoint

        Sub New(DataType As Type)
            Call MyBase.New(DataType)
        End Sub
    End Class

    ''' <summary>
    ''' <see cref="InputDeviceHandle.TypeId"></see>参数指的是目标函数所返回的值的类型，驱动程序会自动根据函数的参数的类型来决定函数方法的调用
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=True, Inherited:=True)>
    Public Class InputDeviceHandle : Inherits HwndEntryPoint

        Public ReadOnly Property Description As String
        Public ReadOnly Property TypeHandleId As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="TypeId">
        ''' The brief name of the type information, usage syntax of this input type casting driver in the shoal scripting is:  var &lt; (TypeId) $variable
        ''' (类型简称，在脚本之中的使用语法为：  var &lt; (TypeId) $variable)
        ''' </param>
        ''' <remarks></remarks>
        Sub New(TypeId As String, Optional Description As String = "")
            Call MyBase.New(Nothing)
            Me.TypeHandleId = TypeId
            Me._Description = Description
        End Sub

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Description) Then
                Return $"Input Device Handle => ""{Me.TypeHandleId}"""
            Else
                Return $"Input Device Handle => ""{Me.TypeHandleId}""; //{_Description}"
            End If
        End Function
    End Class

    ''' <summary>
    ''' Target delegate must compatible with delegate Func(Of T, String, Boolean), the first parameter in the delegate is the 
    ''' data type and the data to write to the file, the second parameter is the file path of the IO operation and the last bool 
    ''' return value indicated that the io operation success or not. please notices that, for the considerations of the data 
    ''' type compatible, the collection data type is recommended implement as generic enumeration interface.
    ''' (目标接口委托对象必须要符合以下接口类型Func(Of T, String, Boolean)，其中第一个将要写文件的数据类型，第二个参数为
    ''' 文件路径，最后一个参数是文件是否写入成功，请注意，对于任意的集合类型推荐使用<see cref="Generic.IEnumerable"></see>泛型集合)
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class IO_DeviceHandle : Inherits HwndEntryPoint

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DataType">目标数据类型，脚本引擎会自动根据函数的数据类型自动选择文件系统的写入方式的驱动程序</param>
        ''' <remarks></remarks>
        Sub New(DataType As Type)
            Call MyBase.New(DataType)
        End Sub

        Sub New(TypeId As String)
            Call MyBase.New(System.Type.GetType(TypeId, throwOnError:=True, ignoreCase:=True))
        End Sub

        Public Overrides Function ToString() As String
            Return $"IOSupport::{SupportType.FullName}"
        End Function
    End Class

End Namespace