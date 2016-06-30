Imports System.Dynamic
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace Runtime.Objects

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

        Dim _ShoalShellScriptEngine As ShoalShell.Runtime.Objects.ShellScript

        ''' <summary>
        ''' 获取Shoal之中的系统保留变量$的值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TopStackValue As Object
            Get
                Return _ShoalShellScriptEngine._EngineMemoryDevice("")
            End Get
        End Property

        Sub New(LibraryRegistry As ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry)
            _ShoalShellScriptEngine = New ShellScript(LibraryRegistry)
        End Sub

        Sub New(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript)
            _ShoalShellScriptEngine = ScriptEngine
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateDefaultEnvironment() As Dynamics
            Return New ShoalShell.Runtime.Objects.Dynamics(ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateDefault)
        End Function

        Public Shared Function CreateDefaultEnvironment(scanPlugins As String) As Dynamics
            Dim Environment = New ShoalShell.Runtime.Objects.Dynamics(ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateDefault)
            Call Environment.ScanPlugins(scanPlugins)
            Call Environment._ShoalShellScriptEngine.ReloadLibraryRegistry()
            Return Environment
        End Function

        Public Function [Imports]([Namespace] As String) As Integer
            Return Me._ShoalShellScriptEngine._Interpreter._InternalCommands.ImportsNamespace([Namespace])
        End Function

        Public Function Install(AssemblyPath As String) As Boolean
            Return Me._ShoalShellScriptEngine.InstallModules(AssemblyPath)
        End Function

        Public Function Evaluate(script As String) As Object
            Call _ShoalShellScriptEngine.EXEC(script)
            Return _ShoalShellScriptEngine.GetStackValue
        End Function

        Const CMDL_SCAN_PLUG_INS As String = "-scan.plugins -dir ""{0}"""

        <Command("-scan.plugins", usage:="-scan.plugins -dir <dir>[ -ext *.*/*.dll/*.exe/*.lib]",
            info:="Scanning all of the avaliable shoal plugin modules in the specific directory and install all of them into the shoal registry.",
            example:="-scan.plugins -dir ./ -ext *.dll")>
        Public Function ScanPlugins(argvs As CommandLine.CommandLine) As Integer
            Dim Dir As String = argvs("-dir"), Ext As String = argvs("-ext")
            Ext = If(String.IsNullOrEmpty(Ext), "*.*", Ext)

            Dim FilesForScan = FileIO.FileSystem.GetFiles(Dir, FileIO.SearchOption.SearchTopLevelOnly, Ext)
            Dim Registry As Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry = Me._ShoalShellScriptEngine.TypeLibraryRegistry

            For Each File As String In FilesForScan
                On Error Resume Next
                Call Registry.RegisterAssemblyModule(File, "")
            Next

            Call Registry.Save()

            Return 0
        End Function

#Region "Dynamics Support"

        Public Overrides Function GetDynamicMemberNames() As IEnumerable(Of String)
            Return (From item In _ShoalShellScriptEngine._EngineMemoryDevice Select item.Key).ToArray
        End Function

        Public Overrides Function TryGetMember(ByVal binder As GetMemberBinder, ByRef result As Object) As Boolean
            If _ShoalShellScriptEngine._EngineMemoryDevice.ExistsVariable(binder.Name) Then
                result = _ShoalShellScriptEngine._EngineMemoryDevice.TryGetValue(binder.Name)
            Else
                result = Nothing
            End If

            Return True
        End Function

        Public Overrides Function TrySetMember(binder As SetMemberBinder, value As Object) As Boolean
            If _ShoalShellScriptEngine._EngineMemoryDevice.ExistsVariable(binder.Name) Then
                Call _ShoalShellScriptEngine._EngineMemoryDevice.SetValue(binder.Name, value)
                Return True
            End If

            Return False
        End Function

        Dim _InternalVirtualInvokeMethodNamespaceCache As StringBuilder = New StringBuilder(1024)

        ''' <summary>
        ''' 首先尝试查看<see cref="_InternalVirtualInvokeMethodNamespaceCache"></see>里面的数据，假若没有的话在直接查找，假若有数据，则执行命名空间的连接操作之后在查找执行
        ''' </summary>
        ''' <param name="binder"></param>
        ''' <param name="args"></param>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function TryInvokeMember(binder As InvokeMemberBinder, args() As Object, ByRef result As Object) As Boolean
            Dim MethodName As String

            If _InternalVirtualInvokeMethodNamespaceCache.Length = 0 Then
                MethodName = binder.Name
            Else
                MethodName = String.Format("{0}.{1}", _InternalVirtualInvokeMethodNamespaceCache.ToString, binder.Name)
            End If

            Try
                Dim Method = _ShoalShellScriptEngine._Interpreter.TryGetCommand(MethodName)
                result = Method.TryInvoke(Nothing, args)
                _InternalVirtualInvokeMethodNamespaceCache.Clear()
            Catch ex As Exception
                If _InternalVirtualInvokeMethodNamespaceCache.Length = 0 Then
                    _InternalVirtualInvokeMethodNamespaceCache.Append(binder.Name)
                Else
                    Call _InternalVirtualInvokeMethodNamespaceCache.Append("." & binder.Name)
                End If

                result = Me
            End Try

            Return True
        End Function

        Public Overrides Function TryConvert(binder As ConvertBinder, ByRef result As Object) As Boolean
            Return MyBase.TryConvert(binder, result)
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

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

#Region "Implements IConvertible"

        Protected ReadOnly _TypeCodes As Dictionary(Of Type, TypeCode) =
            New Dictionary(Of Type, TypeCode) From
            {
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
                {GetType(UInt64), TypeCode.UInt64}}

        Public Function GetTypeCode() As TypeCode Implements IConvertible.GetTypeCode
            Dim value = Me.TopStackValue

            If value Is Nothing Then
                Return TypeCode.Object
            End If

            Dim valueType As Type = value.GetType

            If _TypeCodes.ContainsKey(valueType) Then
                Return _TypeCodes(valueType)
            Else
                Return TypeCode.Object
            End If
        End Function

        Public Function ToBoolean(provider As IFormatProvider) As Boolean Implements IConvertible.ToBoolean
            Return TopStackValue.ToString.getBoolean
        End Function

        Public Function ToByte(provider As IFormatProvider) As Byte Implements IConvertible.ToByte
            Return CType(TopStackValue, Byte)
        End Function

        Public Function ToChar(provider As IFormatProvider) As Char Implements IConvertible.ToChar
            Dim value As Object = TopStackValue
            If value Is Nothing Then
                Return Chr(0)
            Else
                Return value.ToString.First
            End If
        End Function

        Public Function ToDateTime(provider As IFormatProvider) As Date Implements IConvertible.ToDateTime
            Dim value As Object = TopStackValue
            If value Is Nothing Then
                Return Now
            Else
                Dim d As DateTime = CType(value.ToString, DateTime)
                Return d
            End If
        End Function

        Public Function ToDecimal(provider As IFormatProvider) As Decimal Implements IConvertible.ToDecimal
            Dim value As Object = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return CType(value, Decimal)
            End If
        End Function

        Public Function ToDouble(provider As IFormatProvider) As Double Implements IConvertible.ToDouble
            Dim value = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return Val(value.ToString)
            End If
        End Function

        Public Function ToInt16(provider As IFormatProvider) As Short Implements IConvertible.ToInt16
            Dim value = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return Val(value.ToString)
            End If
        End Function

        Public Function ToInt32(provider As IFormatProvider) As Integer Implements IConvertible.ToInt32
            Dim value = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return Val(value.ToString)
            End If
        End Function

        Public Function ToInt64(provider As IFormatProvider) As Long Implements IConvertible.ToInt64
            Dim value = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return Val(value.ToString)
            End If
        End Function

        Public Function ToSByte(provider As IFormatProvider) As SByte Implements IConvertible.ToSByte
            Dim value = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return CType(Val(value.ToString), SByte)
            End If
        End Function

        Public Function ToSingle(provider As IFormatProvider) As Single Implements IConvertible.ToSingle
            Dim value = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return Val(value.ToString)
            End If
        End Function

        Public Overloads Function ToString(provider As IFormatProvider) As String Implements IConvertible.ToString
            Dim value = TopStackValue
            If value Is Nothing Then
                Return ""
            Else
                Return value.ToString
            End If
        End Function

        Public Function ToType(conversionType As Type, provider As IFormatProvider) As Object Implements IConvertible.ToType
            Return TopStackValue.GetType
        End Function

        Public Function ToUInt16(provider As IFormatProvider) As UShort Implements IConvertible.ToUInt16
            Dim value = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return Val(value.ToString)
            End If
        End Function

        Public Function ToUInt32(provider As IFormatProvider) As UInteger Implements IConvertible.ToUInt32
            Dim value = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return Val(value.ToString)
            End If
        End Function

        Public Function ToUInt64(provider As IFormatProvider) As ULong Implements IConvertible.ToUInt64
            Dim value = TopStackValue
            If value Is Nothing Then
                Return 0
            Else
                Return Val(value.ToString)
            End If
        End Function
#End Region

    End Class
End Namespace