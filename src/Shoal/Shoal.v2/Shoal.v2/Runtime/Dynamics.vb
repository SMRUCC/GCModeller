Imports System.Dynamic
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.MMU
Imports Microsoft.VisualBasic.Text

Namespace Runtime

    ''' <summary>
    ''' Dynamics programming environment. If the environment thorw member not found exception, then you should consider of the 
    ''' target method namespace may be not registered yet so then you should try using 
    ''' <see cref="Dynamics.CreateDefaultEnvironment"></see> to initialize the environment.
    ''' (Shoal脚本的动态编程环境，注意：假若提示找不到方法或者空引用，请使用带参数的<see cref="Dynamics.CreateDefaultEnvironment">
    ''' 创建方法</see>在扫描完插件文件夹之后再来创建对象实例)
    ''' 
    ''' <see cref="IConvertible"></see>所返回的是Shoal的系统保留变量的值
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Dynamics : Inherits Dynamic.DynamicObject
        Implements System.IDisposable
        Implements IConvertible

        Public ReadOnly Property ScriptEngine As Runtime.ScriptEngine

        Public ReadOnly Property SysTopStack As Object
            Get
                Return ScriptEngine.MMUDevice.SystemReserved.Value
            End Get
        End Property

        Sub New(ScriptEngine As Runtime.ScriptEngine)
            Me.ScriptEngine = ScriptEngine
        End Sub

        ''' <summary>
        ''' Initialize the dynamics programming runtime environment from the <see cref="Configuration.Config.DefaultFile"/> configuration data.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateDefaultEnvironment() As Dynamics
            Return New ShoalShell.Runtime.Dynamics(New ScriptEngine(ShoalShell.Configuration.Config.LoadDefault))
        End Function

        Public Shared Function CreateDefaultEnvironment(scanPlugins As String) As Dynamics
            Call Runtime.SCOM.RuntimeEnvironment.ScanPlugins($"-scan.plugins -dir {scanPlugins.CLIPath}")
            Return Dynamics.CreateDefaultEnvironment
        End Function

        Public Function [Imports]([Namespace] As String) As Integer
            Return Me.ScriptEngine.Interpreter.EPMDevice.Imports([Namespace]).API?.Length
        End Function

        Public Function Install(AssemblyPath As String) As Boolean
            Dim b As Boolean = Not ScriptEngine.Interpreter.SPMDevice.Imports(AssemblyPath).IsNullOrEmpty
            Call ScriptEngine.Interpreter.SPMDevice.UpdateDb()
            Return b
        End Function

        Public Function Evaluate(script As String) As Object
            Return ScriptEngine.Exec(script)
        End Function

#Region "Dynamics Support"

        Public Overrides Function GetDynamicMemberNames() As IEnumerable(Of String)
            Return (From var In ScriptEngine.MMUDevice Select var.Value.Name).ToArray
        End Function

        Public Overrides Function TryGetMember(binder As GetMemberBinder, ByRef result As Object) As Boolean
            If ScriptEngine.MMUDevice.Exists(binder.Name) Then
                result = ScriptEngine.MMUDevice(binder.Name).Value
            Else
                result = Nothing
            End If

            Return True
        End Function

        Public Overrides Function TrySetMember(binder As SetMemberBinder, value As Object) As Boolean
            Call ScriptEngine.MMUDevice.Update(binder.Name, value)
            Return False
        End Function

        Dim _VirtualInvokeAPINsCache As StringBuilder = New StringBuilder(1024)

        ''' <summary>
        ''' 首先尝试查看<see cref="_VirtualInvokeAPINsCache"></see>里面的数据，假若没有的话在直接查找，假若有数据，则执行命名空间的连接操作之后在查找执行
        ''' </summary>
        ''' <param name="binder"></param>
        ''' <param name="args">按照函数的定义排序的</param>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function TryInvokeMember(binder As InvokeMemberBinder, args() As Object, ByRef result As Object) As Boolean
            Dim MethodName As String

            If _VirtualInvokeAPINsCache.Length = 0 Then
                MethodName = binder.Name
            Else
                MethodName = String.Format("{0}.{1}", _VirtualInvokeAPINsCache.ToString, binder.Name)
            End If

            Try
                Dim API = ScriptEngine.Interpreter.TryGetAPI(MethodName)
                result = Interpreter.Linker.APIHandler.APIInvoker.TryInvoke(API, args)
                _VirtualInvokeAPINsCache.Clear()
            Catch ex As Exception
                If _VirtualInvokeAPINsCache.Length = 0 Then
                    _VirtualInvokeAPINsCache.Append(binder.Name)
                Else
                    Call _VirtualInvokeAPINsCache.Append("." & binder.Name)
                End If

                result = Me
            End Try

            Return True
        End Function

        Public Overrides Function TryConvert(binder As ConvertBinder, ByRef result As Object) As Boolean
            Return MyBase.TryConvert(binder, result)
        End Function
#End Region

#Region "Implements IConvertible"

#Region "GetTypeCode"

        Public Shared ReadOnly Property TypeCodes As SortedDictionary(Of Type, TypeCode) =
            New SortedDictionary(Of Type, TypeCode) From {
 _
            {GetType(Object), TypeCode.Object},
            {GetType(Boolean), TypeCode.Boolean},
            {GetType(Byte), TypeCode.Byte},
            {GetType(Char), TypeCode.Char},
            {GetType(DateTime), TypeCode.DateTime},
            {GetType(DBNull), TypeCode.DBNull},
            {GetType(Decimal), TypeCode.Decimal},
            {GetType(Double), TypeCode.Double},
            {GetType(Int16), TypeCode.Int16},
            {GetType(Int32), TypeCode.Int32},
            {GetType(Int64), TypeCode.Int64},
            {GetType(SByte), TypeCode.SByte},
            {GetType(Single), TypeCode.Single},
            {GetType(String), TypeCode.String},
            {GetType(UInt16), TypeCode.UInt16},
            {GetType(UInt32), TypeCode.UInt32},
            {GetType(UInt64), TypeCode.UInt64}
        }

        ''' <summary>
        ''' Specifies the type of an object.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetTypeCode() As TypeCode Implements IConvertible.GetTypeCode
            Dim typeRef = InputHandler.GetType(Me.SysTopStack, True)

            If _TypeCodes.ContainsKey(typeRef) Then
                Return _TypeCodes(typeRef)
            Else
                Return TypeCode.Object
            End If
        End Function
#End Region

        Public Function ToBoolean(provider As IFormatProvider) As Boolean Implements IConvertible.ToBoolean
            Return InputHandler.ToString(SysTopStack).getBoolean
        End Function

        Public Function ToByte(provider As IFormatProvider) As Byte Implements IConvertible.ToByte
            Return CType(SysTopStack, Byte)
        End Function

        Public Function ToChar(provider As IFormatProvider) As Char Implements IConvertible.ToChar
            Dim value As String = InputHandler.ToString(SysTopStack)
            If String.IsNullOrEmpty(value) Then
                Return ASCII.NUL
            Else
                Return value(Scan0)
            End If
        End Function

        Public Function ToDateTime(provider As IFormatProvider) As Date Implements IConvertible.ToDateTime
            Dim value As Object = SysTopStack
            If value Is Nothing Then
                Return New Date
            Else
                Dim d As DateTime = CType(value.ToString, DateTime)
                Return d
            End If
        End Function

        Public Function ToDecimal(provider As IFormatProvider) As Decimal Implements IConvertible.ToDecimal
            Dim value As Object = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return CType(value, Decimal)
            End If
        End Function

        Public Function ToDouble(provider As IFormatProvider) As Double Implements IConvertible.ToDouble
            Dim value = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return Val(value.ToString)
            End If
        End Function

        Public Function ToInt16(provider As IFormatProvider) As Short Implements IConvertible.ToInt16
            Dim value = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return CType(Val(value.ToString), Int16)
            End If
        End Function

        Public Function ToInt32(provider As IFormatProvider) As Integer Implements IConvertible.ToInt32
            Dim value = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return CInt(Val(value.ToString))
            End If
        End Function

        Public Function ToInt64(provider As IFormatProvider) As Long Implements IConvertible.ToInt64
            Dim value = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return CLng(Val(value.ToString))
            End If
        End Function

        Public Function ToSByte(provider As IFormatProvider) As SByte Implements IConvertible.ToSByte
            Dim value = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return CType(Val(value.ToString), SByte)
            End If
        End Function

        Public Function ToSingle(provider As IFormatProvider) As Single Implements IConvertible.ToSingle
            Dim value = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return CSng(Val(value.ToString))
            End If
        End Function

        Public Overloads Function ToString(provider As IFormatProvider) As String Implements IConvertible.ToString
            Return InputHandler.ToString(Me.SysTopStack)
        End Function

        Public Function ToType(conversionType As Type, provider As IFormatProvider) As Object Implements IConvertible.ToType
            Return SysTopStack.GetType
        End Function

        Public Function ToUInt16(provider As IFormatProvider) As UShort Implements IConvertible.ToUInt16
            Dim value = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return CType(Val(value.ToString), UInt16)
            End If
        End Function

        Public Function ToUInt32(provider As IFormatProvider) As UInteger Implements IConvertible.ToUInt32
            Dim value = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return CType(Val(value.ToString), UInteger)
            End If
        End Function

        Public Function ToUInt64(provider As IFormatProvider) As ULong Implements IConvertible.ToUInt64
            Dim value = SysTopStack
            If value Is Nothing Then
                Return 0
            Else
                Return CType(Val(value.ToString), ULong)
            End If
        End Function
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace