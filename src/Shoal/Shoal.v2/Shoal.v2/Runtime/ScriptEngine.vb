Imports System.ComponentModel
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Configuration
Imports Microsoft.VisualBasic.Scripting.ShoalShell.HTML
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.MMU
Imports Microsoft.VisualBasic.Linq

Namespace Runtime

    ''' <summary>
    ''' Shoal Shell script Engine, you can using this object to embedded a script engine into 
    ''' your application or using this script engine as a powerfully debugging tool.
    ''' Try using <see cref="Runtime.Dynamics"/> object to dynamics programming in your .NET program.
    ''' </summary>
    <Description("The new version of the shoal shell script engine, new language feature was introduced 
into this version of engine, for more details information please visit:  http://gcmodeller.org/dev-blogs/")>
    Public Class ScriptEngine : Inherits SCOM.RuntimeComponent

        ''' <summary>
        ''' Shoal Shell memory management device.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MMUDevice As MMU.MMUDevice
        Public ReadOnly Property Interpreter As Interpreter.Interpreter
        Public ReadOnly Property PMgrDb As SPM.PackageModuleDb
        Public ReadOnly Property WikiEngine As HTML.Wiki

        Public Function GetHelpInfo(obj As String, ShowManual As Boolean) As String()
            Return WikiEngine.WikiSearchView(obj)
        End Function

        Public Overrides ReadOnly Property ScriptEngine As ScriptEngine
            Get
                Return Me
            End Get
        End Property

        ''' <summary>
        ''' String interpolated services.(字符串服务)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Strings As Runtime.MMU.Strings
        Public ReadOnly Property ExecuteModel As ExecuteModel

        Public ReadOnly Property TopOfStack As Object
            Get
                Return MMUDevice.SystemReserved.Value
            End Get
        End Property

        Public ReadOnly Property ImportsAPI As String()
            Get
                Return Interpreter.EPMDevice.ImportedAPI.Select(Function(api) api.Value.Name).ToArray
            End Get
        End Property

        Public ReadOnly Property Config As Config

        ''' <summary>
        ''' Create the Shoal <see cref="Runtime.ScriptEngine"/> instance using the default configuration file: <see cref="ShoalShell.Configuration.Config.LoadDefault()"/>.
        ''' (采用默认的配置文件数据来初始化脚本引擎)
        ''' </summary>
        Sub New()
            Call Me.New(Config.LoadDefault)
        End Sub

        Sub New(Config As Config)
            Call MyBase.New(Nothing)

            Me.Config = Config
            PMgrDb = ShoalShell.SPM.PackageModuleDb.Load(Config.GetRegistryFile)
            Dim SPM = New SPM.ShoalPackageMgr(PMgrDb)

            ExecuteModel = New ExecuteModel(Me)
            MMUDevice = New MMU.MMUDevice(Me, Config.GetInitHeapSize)
            Interpreter = New Interpreter.Interpreter(Me, SPM)
            Strings = New Runtime.MMU.Strings(Me)
            WikiEngine = New Wiki(Me)

            Call Interpreter.EPMDevice.AnonymousDelegate.CdTemp()
            Call [Imports](Of ShoalShell.Configuration.Configuration)(New ShoalShell.Configuration.Configuration(Me))
        End Sub

        ''' <summary>
        ''' Evaluate the script line which is user input from the terminal.
        ''' (处理来自于终端输入的一行脚本代码)
        ''' </summary>
        ''' <param name="Script">User input from the terminal.</param>
        ''' <returns></returns>
        Public Function Exec(Script As String) As Object
            Dim Expr As PrimaryExpression =
                ShoalShell.Interpreter.Interpreter.InternalExpressionParser(Script)

            If Expr.IsNonExecuteCode = True Then Return Nothing

#Const DEBUG_ENABLE = 0

#If DEBUG_ENABLE Then
            Return __execInner(Expr)
#Else
            Try
                Return __execInner(Expr)
            Catch ex As Exception
                ex = New Exception(vbCrLf & vbCrLf &
                                   $"  ({Script.GetHashCode}::ERROR_LINE)  ==>     ""  {Script}  """ &
                                   vbCrLf & vbCrLf, ex)
                Call ex.PrintException
                Call App.LogException(ex)
                Return Nothing
            End Try
#End If
        End Function

        Private Function __execInner(Expr As PrimaryExpression) As Object
            Dim value As Object = ExecuteModel.Exec(Expr)  '从终端输入的脚本需要额外的错误处理来避免崩溃
            Return value
        End Function

        ''' <summary>
        ''' Evaluate script which is on a specific file system location: <paramref name="path"/>.
        ''' (处理来自于文件之中的脚本代码)
        ''' </summary>
        ''' <param name="path">The file path of the script file.</param>
        ''' <returns></returns>
        Public Function Source(path As String) As Object
            Dim Pager As Interpreter.LDM.SyntaxModel = Interpreter.ParseFile(path)
            Dim value As Object =
                New Runtime.FSMMachine(Me, Pager).Execute
            Return value
        End Function

        Public Function GetValue(Name As String) As Object
            Return MMUDevice(Name).Value
        End Function

        Public Function [TypeOf](Name As String) As Type
            Return MMUDevice(Name).TypeOf
        End Function

        Public Overrides Function ToString() As String
            Return MMUDevice.HeapSize
        End Function

        Public Function [Imports](type As Type) As Boolean
            Return Me.Interpreter.EPMDevice.Imports(type)
        End Function

        Public Function [Imports](Of T As Class)(obj As T) As Boolean
            Try
                Call Interpreter.EPMDevice.ImportsInstance(Of T)(obj)
            Catch ex As Exception
                Call App.LogException(ex, $"{NameOf(ScriptEngine)}::{NameOf(Runtime.ScriptEngine.Imports)}")
                Return False
            End Try

            Return True
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            Call Interpreter.Dispose()
            Call Me.Config.Save()
            Call MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace