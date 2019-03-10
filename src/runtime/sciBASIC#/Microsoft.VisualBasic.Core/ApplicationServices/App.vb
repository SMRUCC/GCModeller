﻿#Region "Microsoft.VisualBasic::a641998267443b3ac9441d9bc4346209, Microsoft.VisualBasic.Core\ApplicationServices\App.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module App
    ' 
    '     Properties: AppSystemTemp, AssemblyName, BufferSize, Command, CommandLine
    '                 CPUCoreNumbers, CurrentDirectory, CurrentProcessTemp, Desktop, ExceptionLogFile
    '                 ExecutablePath, Github, HOME, Info, InputFile
    '                 IsConsoleApp, IsMicrosoftPlatform, LocalData, LocalDataTemp, LogErrDIR
    '                 NanoTime, NextTempName, OutFile, PID, Platform
    '                 PreviousDirectory, Process, ProductName, ProductProgramData, ProductSharedDIR
    '                 ProductSharedTemp, References, Running, RunTimeDirectory, StartTime
    '                 StartupDirectory, StdErr, StdOut, SysTemp, UserHOME
    '                 Version
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __cli, __completeCLI, __getTEMP, __getTEMPhash, __isMicrosoftPlatform
    '               __listFiles, __sysTEMP, (+2 Overloads) Argument, BugsFormatter, CLICode
    '               ElapsedMilliseconds, Exit, FormatTime, GenerateTemp, (+2 Overloads) GetAppLocalData
    '               GetAppSysTempFile, GetAppVariables, GetFile, GetProductSharedDIR, GetProductSharedTemp
    '               GetTempFile, GetVariable, (+3 Overloads) LogException, NullDevice, (+10 Overloads) RunCLI
    '               RunCLIInternal, SelfFolk, SelfFolks, Shell, TemporaryEnvironment
    '               TraceBugs
    ' 
    '     Sub: __GCThreadInvoke, __removesTEMP, AddExitCleanHook, FlushMemory, Free
    '          JoinVariable, (+2 Overloads) JoinVariables, Pause, (+2 Overloads) println, RunAsAdmin
    '          SetBufferSize, StartGC, StopGC
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.ApplicationServices.Windows.Forms.VistaSecurity
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Emit.CodeDOM_VBC
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language.UnixBash.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Parallel.Threads
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Text
Imports CLI = Microsoft.VisualBasic.CommandLine.CommandLine
Imports DevAssmInfo = Microsoft.VisualBasic.ApplicationServices.Development.AssemblyInfo

'                   _ooOoo_
'                  o8888888o
'                  88" . "88
'                  (| -_- |)
'                  O\  =  /O
'               ____/`---'\____
'             .'  \\|     |//  `.
'            /  \\|||  :  |||//  \
'           /  _||||| -:- |||||-  \
'           |   | \\\  -  /// |   |
'           | \_|  ''\---/''  |   |
'           \  .-\__  `-`  ___/-. /
'         ___`. .'  /--.--\  `. . __
'      ."" '<  `.___\_<|>_/___.'  >'"".
'     | | :  `- \`.;`\ _ /`;.`/ - ` : | |
'     \  \ `-.   \_ __\ /__ _/   .-` /  /
'======`-.____`-.___\_____/___.-`____.-'======
'                   `=---='
'^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
'           佛祖保佑       永无BUG
'           心外无法       法外无心

''' <summary>
''' Provides information about, and means to manipulate, the current environment Application information collection.
''' (More easily runtime environment information provider on <see cref="PlatformID.Unix"/>/LINUX server platform for VisualBasic program.)
''' (从命令行之中使用``/@set``参数赋值环境变量的时候，每一个变量之间使用分号进行分隔)
''' </summary>
'''
<Package("App", Description:="More easily runtime environment information provider on LINUX platform for VisualBasic program.",
                  Publisher:="amethyst.asuka@gcmodeller.org",
                  Url:="http://SourceForge.net/projects/shoal")>
Public Module App

    ''' <summary>
    ''' 运行时环境所安装的文件夹的位置
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RunTimeDirectory As String

    ''' <summary>
    ''' Gets the number of ticks that represent the date and time of this instance.
    ''' 
    ''' The number of ticks that represent the date and time of this instance. The value
    ''' is between DateTime.MinValue.Ticks and DateTime.MaxValue.Ticks.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property NanoTime As Long
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Now.Ticks
        End Get
    End Property

    ''' <summary>
    ''' Numbers of the CPU kernels on the current machine.
    ''' (获取当前的系统主机的CPU核心数)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property CPUCoreNumbers As Integer = LQuerySchedule.CPU_NUMBER
    ''' <summary>
    ''' 判断当前运行的程序是否为Console类型的应用和程序，由于在执行初始化的时候，
    ''' 最先被初始化的是这个模块，所以没有任何代码能够先执行<see cref="Console.IsErrorRedirected"/>了，
    ''' 在这里使用<see cref="Console.IsErrorRedirected"/>这个来进行判断是可靠的
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsConsoleApp As Boolean = Not Console.IsErrorRedirected
    ''' <summary>
    ''' Get the referenced dll list of current running ``*.exe`` program.
    ''' (获取得到当前的这个所运行的应用程序所引用的dll文件列表)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property References As New Lazy(Of String())(Function() ReferenceSolver.ExecutingReferences)
    ''' <summary>
    ''' Gets a path name pointing to the Desktop directory.
    ''' </summary>
    ''' <returns>The path to the Desktop directory.</returns>
    Public ReadOnly Property Desktop As String
    Public ReadOnly Property StdErr As New StreamWriter(Console.OpenStandardError)

    ''' <summary>
    ''' <see cref="Console.OpenStandardOutput()"/> as default text output device.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property StdOut As DefaultValue(Of TextWriter) = Console.OpenStandardOutput.OpenTextWriter

    ''' <summary>
    ''' Get the <see cref="System.Diagnostics.Process"/> id(PID) of the current program process.
    ''' </summary>
    Public ReadOnly Property PID As Integer = Process.GetCurrentProcess.Id
    ''' <summary>
    ''' Gets a new <see cref="System.Diagnostics.Process"/> component and associates it with the currently active process.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Process As Process = Process.GetCurrentProcess

    ''' <summary>
    ''' Gets the command-line arguments for this <see cref="Process"/>.
    ''' </summary>
    ''' <returns>Gets the command-line arguments for this process.</returns>
    Public ReadOnly Property CommandLine As CommandLine.CommandLine = __cli()

    ''' <summary>
    ''' Get argument value from <see cref="CommandLine"/>.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="name$"></param>
    ''' <returns></returns>
    Public Function Argument(Of T)(name$) As T
        With CommandLine(name).DefaultValue
            If .StringEmpty Then
                Return Nothing
            Else
                Return Scripting.CTypeDynamic(Of T)(.ByRef)
            End If
        End With
    End Function

    ''' <summary>
    ''' Get argument value string from <see cref="CommandLine"/>.
    ''' </summary>
    ''' <param name="name$"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Argument(name$) As String
        Return CommandLine(name)
    End Function

    ''' <summary>
    ''' Enable .NET application running from git bash terminal
    ''' </summary>
    Const gitBash$ = "C:/Program Files/Git"

    ''' <summary>
    ''' Makes compatibility with git bash: <see cref="gitBash"/> = ``C:/Program Files/Git``
    ''' </summary>
    ''' <returns></returns>
    Private Function __cli() As CommandLine.CommandLine
        ' 第一个参数为应用程序的文件路径，不需要
        Dim tokens$() =
            Environment.GetCommandLineArgs _
            .Skip(1) _
            .Select(Function(t) t.Replace(gitBash, "")) _
            .ToArray
        Dim cliString$ = tokens.JoinBy(" ")
        Dim cli = CLITools.TryParse(tokens, False, cliString)
        Return cli
    End Function

    Public ReadOnly Property Github As String = LICENSE.githubURL

    ''' <summary>
    ''' Returns the argument portion of the <see cref="Microsoft.VisualBasic.CommandLine.CommandLine"/> used to start Visual Basic or
    ''' an executable program developed with Visual Basic. The My feature provides greater
    ''' productivity and performance than the <see cref="microsoft.VisualBasic.Interaction.Command"/> function. For more information,
    ''' see <see cref="ConsoleApplicationBase.CommandLineArgs"/>.
    ''' </summary>
    ''' <returns>Gets the command-line arguments for this process.</returns>
    Public ReadOnly Property Command As String = CLITools.Join(App.CommandLine.Tokens)

    ''' <summary>
    ''' The file path of the current running program executable file.(本应用程序的可执行文件的文件路径)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ExecutablePath As String
    ''' <summary>
    ''' Get assembly info of current running ``*.exe`` program.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Info As DevAssmInfo

    ''' <summary>
    ''' Gets the name, without the extension, of the assembly file for the application.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AssemblyName As String
    Public ReadOnly Property ProductName As String

    ''' <summary>
    ''' The program directory of the current running program.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property HOME As String
    ''' <summary>
    ''' Getting the path of the home directory
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property UserHOME As String

    ''' <summary>
    ''' Gets the ``/in`` commandline value as the input file path.
    ''' </summary>
    ''' <returns></returns>
    Public Property InputFile As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return App.CommandLine("/in")
        End Get
        Friend Set(value As String)
            App.CommandLine.Add("/in", value)
        End Set
    End Property

    Dim _out$

    ''' <summary>
    ''' Gets the ``/out`` commandline value as the output file path.
    ''' </summary>
    ''' <returns></returns>
    Public Property OutFile As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            If _out.StringEmpty Then
                _out = App.CommandLine("/out")
            End If

            Return _out
        End Get
        Set(value As String)
            _out = value
        End Set
    End Property

    ''' <summary>
    ''' Found the file path based on the current application context.
    ''' 
    ''' 1. 直接查找(这个查找已经包含了在当前的文件夹之中查找)
    ''' 2. 从<see cref="App.InputFile"/>所在的文件夹之中查找
    ''' 3. 从<see cref="App.OutFile"/>所在的文件夹之中查找
    ''' 4. 从<see cref="App.Home"/>文件夹之中查找
    ''' 5. 从<see cref="App.UserHOME"/>文件夹之中查找
    ''' 6. 从<see cref="App.ProductProgramData"/>文件夹之中查找
    ''' </summary>
    ''' <param name="fileName$"></param>
    ''' <returns></returns>
    Public Function GetFile(fileName$) As String
        If fileName.FileExists Then
            Return fileName.GetFullPath
        End If

        Dim path As New Value(Of String)

        On Error Resume Next

        If Not App.InputFile.StringEmpty AndAlso
            (path = App.InputFile.ParentPath & "/" & fileName).FileExists Then

            Return path
        End If
        If Not App.OutFile.StringEmpty AndAlso
            (path = App.OutFile.ParentPath & "/" & fileName).FileExists Then

            Return path
        End If

        For Each DIR As String In {
            App.HOME,
            App.UserHOME,
            App.ProductProgramData,
            App.ProductSharedDIR
        }
            If (path = DIR & "/" & fileName).FileExists Then
                Return path
            End If
        Next

        Return App.CurrentDirectory & "/" & fileName
    End Function

    ''' <summary>
    ''' The currrent working directory of this application.(应用程序的当前的工作目录)
    ''' </summary>
    ''' <returns></returns>
    Public Property CurrentDirectory As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            ' 由于会因为切换目录而发生变化，所以这里不适用简写形式了
            Return FileIO.FileSystem.CurrentDirectory
        End Get
        Set(value As String)
            If String.Equals(value, "-") Then  ' 切换到前一个工作目录
                value = PreviousDirectory
            Else
                _PreviousDirectory = FileIO.FileSystem.CurrentDirectory
            End If

            FileIO.FileSystem.CreateDirectory(value)
            FileIO.FileSystem.CurrentDirectory = value
        End Set
    End Property

    ''' <summary>
    ''' -
    ''' Linux里面的前一个文件夹
    ''' </summary>
    ''' <remarks>
    ''' 假设你之前好不容易进入了一个很深的目录，然后不小心敲了个 ``cd /``，是不是快气晕了啊，不用着急，通过下面的指令可以轻松的回到前一个指令：
    '''
    ''' ```bash
    ''' cd -
    ''' ```
    ''' </remarks>
    Public ReadOnly Property PreviousDirectory As String

    ''' <summary>
    ''' Gets the path for the executable file that started the application, not including the executable name.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property StartupDirectory As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Application.StartupPath
        End Get
    End Property

    ''' <summary>
    ''' The repository root of the product application program data.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ProductProgramData As String

    ''' <summary>
    ''' The shared program data directory for a group of app which have the same product series name.
    ''' (同一產品程序集所共享的數據文件夾)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ProductSharedDIR As String

    Sub New()
        ' On Error Resume Next ' 在Linux服务器上面不起作用？？？
        PreviousDirectory = App.StartupDirectory

#Region "公共模块内的所有的文件路径初始化"
        ' 因为vb的基础运行时环境在Linux平台上面对文件系统的支持还不是太完善，所以不能够放在属性的位置直接赋值，否则比较难处理异常
        ' 现在放在这个构造函数之中，强制忽略掉错误继续执行，提升一些稳定性，防止出现程序无法启动的情况出现。

        ' 请注意，这里的变量都是有先后的初始化顺序的
        Try
            App.RunTimeDirectory = FileIO.FileSystem _
                .GetDirectoryInfo(RuntimeEnvironment.GetRuntimeDirectory) _
                .FullName _
                .Replace("/", "\")
            App.Desktop = My.Computer.FileSystem.SpecialDirectories.Desktop
            App.ExecutablePath = FileIO.FileSystem.GetFileInfo(Application.ExecutablePath).FullName    ' (Process.GetCurrentProcess.StartInfo.FileName).FullName
            App.Info = ApplicationInfoUtils.CurrentExe()
            App.AssemblyName = BaseName(App.ExecutablePath)
            App.ProductName = Application.ProductName Or AssemblyName.AsDefault(Function(s) String.IsNullOrEmpty(s))
            App.HOME = FileIO.FileSystem.GetParentPath(App.ExecutablePath)
            App.UserHOME = PathMapper.HOME.GetDirectoryFullPath("App.New(.cctor)")
            App.ProductProgramData = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/{ProductName}".GetDirectoryFullPath("App.New(.cctor)")
            App.ProductSharedDIR = $"{ProductProgramData}/.shared".GetDirectoryFullPath
            App.LocalData = App.GetAppLocalData(ProductName, AssemblyName, "App.New(.cctor)")
            App.CurrentProcessTemp = GenerateTemp(App.SysTemp & "/tmp.io", App.PID).GetDirectoryFullPath("App.New(.cctor)")
            App.ProductSharedTemp = App.ProductSharedDIR & "/tmp/"
            App.LogErrDIR = App.LocalData & $"/.logs/err/"
        Catch ex As Exception

        End Try
#End Region

        If App.HOME.StringEmpty Then
            App.HOME = System.IO.Directory.GetCurrentDirectory
        End If

        Call FileIO.FileSystem.CreateDirectory(AppSystemTemp)
        Call FileIO.FileSystem.CreateDirectory(App.HOME & "/Resources/")

        ' 2018-08-14 因为经过测试发现text encoding模块会优先于命令行参数设置模块的初始化的加载
        ' 所以会导致环境变量为空
        ' 故而text encoding可能总是系统的默认值，无法从命令行设置
        ' 在这里提前进行初始化，可以消除此bug的出现
        Dim envir As Dictionary(Of String, String) = App _
            .CommandLine _
            .EnvironmentVariables

        Call App.JoinVariables(
            envir _
            .SafeQuery _
            .Select(Function(x)
                        Return New NamedValue(Of String) With {
                            .Name = x.Key,
                            .Value = x.Value
                        }
                    End Function) _
            .ToArray)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAppLocalData(app$, assemblyName$, <CallerMemberName> Optional track$ = Nothing) As String
        Return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/{app}/{assemblyName}".GetDirectoryFullPath(track)
    End Function

    Public Function GetAppLocalData(exe$) As String
        Dim app As DevAssmInfo = Assembly.LoadFile(path:=IO.Path.GetFullPath(exe)).FromAssembly
        Return GetAppLocalData(app:=app.AssemblyProduct, assemblyName:=exe.BaseName)
    End Function

#Region "这里的环境变量方法主要是操作从命令行之中所传递进来的额外的参数的"

    Dim __joinedVariables As New Dictionary(Of NamedValue(Of String))

    ''' <summary>
    ''' 添加参数到应用程序的环境变量之中
    ''' </summary>
    ''' <param name="name$"></param>
    ''' <param name="value$"></param>
    Public Sub JoinVariable(name$, value$)
        __joinedVariables(name) =
            New NamedValue(Of String) With {
                .Name = name,
                .Value = value
        }
    End Sub

    ''' <summary>
    ''' 添加参数集合到应用程序的环境变量之中
    ''' </summary>
    ''' <param name="vars"></param>
    Public Sub JoinVariables(ParamArray vars As NamedValue(Of String)())
        For Each v As NamedValue(Of String) In vars
            __joinedVariables(v.Name) = v
        Next
    End Sub

    Public Sub JoinVariables(vars As Dictionary(Of String, String))
        Call App.JoinVariables(vars.Select(
            Function(x)
                Return New NamedValue(Of String) With {
                    .Name = x.Key,
                    .Value = x.Value
                }
            End Function).ToArray)
    End Sub

    ''' <summary>
    ''' If the parameter <paramref name="name"/> is ignored, then the value from <see cref="CallerMemberNameAttribute"/> 
    ''' will be used as variable name.
    ''' (这个函数只是会从设置的变量之中查找，本模块之中的变量请直接从属性进行引用，对于查找失败的变量，这个函数会返回空值
    ''' 假若忽略掉<paramref name="name"/>参数的话，则这个函数会使用<see cref="CallerMemberNameAttribute"/>来获取变量
    ''' 的名称)
    ''' </summary>
    ''' <param name="name$">
    ''' 因为由于是从命令行之中输入进来的，所以可能有些时候大小写会影响直接字典查找，在这里需要用字符串手工查找
    ''' </param>
    ''' <returns>当没有查找到相对应的环境变量的时候会返回空值</returns>
    Public Function GetVariable(<CallerMemberName> Optional name$ = Nothing) As String
        If __joinedVariables.ContainsKey(name) Then
            Return __joinedVariables(name).Value
        Else
            For Each v As NamedValue(Of String) In __joinedVariables.Values
                If v.Name.TextEquals(name) Then
                    Return v.Value
                End If
            Next
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' 获取<see cref="App"/>的可读属性值来作为环境变量
    ''' </summary>
    ''' <returns></returns>
    Public Function GetAppVariables() As NamedValue(Of String)()
        Dim type As Type = GetType(App)
        Dim pros = type.Schema(PropertyAccess.Readable, BindingFlags.Public Or BindingFlags.Static)
        Dim out As New List(Of NamedValue(Of String))(__joinedVariables.Values)
        Dim value$
        Dim o

        For Each prop As PropertyInfo
            In pros.Values _
                .Where(Function(p)
                           Return p.PropertyType.Equals(GetType(String)) AndAlso
                                  p.GetIndexParameters _
                                   .IsNullOrEmpty
                       End Function)

            o = prop.GetValue(Nothing, Nothing)
            value = Scripting.ToString(o)
            out += New NamedValue(Of String) With {
                .Name = prop.Name,
                .Value = value
            }
        Next

        Return out.ToArray
    End Function

#End Region

    ''' <summary>
    ''' 其他的模块可能也会依赖于这个初始化参数
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property BufferSize As Integer = 4 * 1024 * 1024

    ''' <summary>
    ''' Set value of <see cref="BufferSize"/>
    ''' </summary>
    ''' <param name="size"></param>
    Public Sub SetBufferSize(size As Integer)
        _BufferSize = size
    End Sub

    ''' <summary>
    ''' 假若有些时候函数的参数要求有一个输出流，但是并不想输出任何数据的话，则可以使用这个进行输出
    ''' </summary>
    ''' <returns></returns>
    Public Function NullDevice(Optional encoding As Encodings = Encodings.ASCII) As StreamWriter
        Dim ms As New MemoryStream(capacity:=BufferSize)
        Dim codePage As Encoding = encoding.CodePage
        Return New StreamWriter(ms, encoding:=codePage)
    End Function

    ''' <summary>
    ''' java <see cref="printf"/> + <see cref="Console.WriteLine(String)"/>
    ''' </summary>
    ''' <param name="s$"></param>
    ''' <param name="args"></param>
    Public Sub println(s$, ParamArray args As Object())
        If Not args.IsNullOrEmpty Then
            s = sprintf(s, args)
        Else
            s = CLangStringFormatProvider.ReplaceMetaChars(s)
        End If

        Call My.InnerQueue.AddToQueue(
            Sub()
                Call Console.WriteLine(s)
            End Sub)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub println()
        Call My.InnerQueue.AddToQueue(AddressOf Console.WriteLine)
    End Sub

    Public Declare Function SetProcessWorkingSetSize Lib "kernel32.dll" (process As IntPtr, minimumWorkingSetSize As Integer, maximumWorkingSetSize As Integer) As Integer

    ''' <summary>
    ''' Rabbish collection to free the junk memory.(垃圾回收)
    ''' </summary>
    ''' <remarks></remarks>
    '''
    <ExportAPI("FlushMemory", Info:="Rabbish collection To free the junk memory.")>
    Public Sub FlushMemory()
        Call GC.Collect()
        Call GC.WaitForPendingFinalizers()

        If (Environment.OSVersion.Platform = PlatformID.Win32NT) Then
            Call SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1)
        End If
    End Sub

    ''' <summary>
    ''' Free this variable pointer in the memory.(销毁本对象类型在内存之中的指针)
    ''' </summary>
    ''' <typeparam name="T">假若该对象类型实现了<see cref="System.IDisposable"></see>接口，则函数还会在销毁前调用该接口的销毁函数</typeparam>
    ''' <param name="obj"></param>
    ''' <remarks></remarks>
    <Extension> Public Sub Free(Of T As Class)(ByRef obj As T)
        If Not obj Is Nothing Then
            Dim TypeInfo As Type = obj.GetType
            If Array.IndexOf(TypeInfo.GetInterfaces, GetType(IDisposable)) > -1 Then
                Try
                    Call DirectCast(obj, IDisposable).Dispose()
                Catch ex As Exception

                End Try
            End If
        End If

        obj = Nothing

        ' Will not working on Linux platform
        If App.IsMicrosoftPlatform Then
            Call FlushMemory()
        End If
    End Sub

    ''' <summary>
    ''' Pause the console program.
    ''' </summary>
    ''' <param name="Prompted"></param>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Pause", Info:="Pause the console program.")>
    Public Sub Pause(Optional prompted$ = "Press any key to continute...")
        Call My.InnerQueue.WaitQueue()
        Call Console.WriteLine(prompted)

        ' 2018-6-26 如果不是命令行程序的话，可能会因为没有地方进行输入而导致程序在这里停止运行
        ' 所以会需要进行一些判断，只在命令行模式下才会要求输入
        If App.IsConsoleApp Then
            Call Console.Read()
        End If
    End Sub

    ''' <summary>
    ''' 使用<see cref="ProductSharedDIR"/>的位置会变化的，则使用本函数则会使用获取当前的模块的文件夹，即使其不是exe程序而是一个dll文件
    ''' </summary>
    ''' <param name="type"></param>
    ''' <returns></returns>
    Public Function GetProductSharedDIR(type As Type) As String
        Dim assm As Assembly = type.Assembly
        Dim productName As String = ApplicationInfoUtils.GetProductName(assm)

        If String.IsNullOrEmpty(productName) Then
            productName = BaseName(assm.Location)
        End If

        Return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/{productName}"
    End Function

    ''' <summary>
    ''' The time tag of the application started.(应用程序的启动的时间)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property StartTime As Long = Now.ToBinary

    ''' <summary>
    ''' The distance of time that this application running from start and to current time.
    ''' (当前距离应用程序启动所逝去的时间)
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("Elapsed.Milliseconds")>
    Public Function ElapsedMilliseconds() As Long
        Dim nowLng As Long = Now.ToBinary
        Dim d As Long = nowLng - StartTime
        Return d
    End Function

    ''' <summary>
    ''' The local data dir of the application in the %user%/&lt;CurrentUser>/Local/Product/App
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LocalData As String

    ''' <summary>
    ''' The temp directory in the application local data.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LocalDataTemp As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return App.LocalData & "/Temp/"
        End Get
    End Property

    ''' <summary>
    ''' The directory path of the system temp data.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SysTemp As String = App.__sysTEMP

    ''' <summary>
    ''' <see cref="FileIO.FileSystem.GetParentPath"/>(<see cref="FileIO.FileSystem.GetTempFileName"/>)
    ''' 当临时文件夹被删除掉了的时候，会出现崩溃。。。。所以弃用改用读取环境变量
    ''' </summary>
    ''' <returns></returns>
    Private Function __sysTEMP() As String
        Dim DIR As String = Environment.GetEnvironmentVariable("TMP") ' Linux系统可能没有这个东西

        If String.IsNullOrEmpty(DIR) Then
            DIR = IO.Path.GetTempPath
        End If

        Try
            Call FileIO.FileSystem.CreateDirectory(DIR)
        Catch ex As Exception
            ' 不知道应该怎样处理，但是由于只是得到一个路径，所以在这里干脆忽略掉这个错误就可以了
            Call New Exception(DIR, ex).PrintException
        End Try

        Return DIR
    End Function

    ''' <summary>
    ''' Application temp data directory in the system temp root: %<see cref="App.SysTemp"/>%/<see cref="App.AssemblyName"/>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AppSystemTemp As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return SysTemp & "/" & App.AssemblyName
        End Get
    End Property

    ''' <summary>
    ''' Gets the product version associated with this application.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Version As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Trim(Application.ProductVersion)
        End Get
    End Property

    ''' <summary>
    ''' Simply log application exception data into a log file which saves at location: %<see cref="App.LocalData"/>%/.logs/err/.
    ''' (简单日志记录，函数返回空值)
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <param name="trace">调用函数的位置，这个参数一般为空，编译器会自动生成Trace位点参数</param>
    ''' <returns>这个函数总是返回空值的</returns>
    <ExportAPI("LogException")>
    Public Function LogException(ex As Exception, <CallerMemberName> Optional ByRef trace$ = "") As Object
        Try
            trace = App.TraceBugs(ex, trace)
        Catch ex2 As Exception
            ' 错误日志文件的存放位置不可用或者被占用了不可写，则可能会出错，
            ' 在这里将原来的错误打印在终端上面就行了， 扔弃掉这个错误日志
            Call ex.PrintException
        End Try

        Return Nothing
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function TemporaryEnvironment(newLocation As String) As FileIO.TemporaryEnvironment
        Return New FileIO.TemporaryEnvironment(newLocation)
    End Function

    ''' <summary>
    ''' Function returns the file path of the application log file.
    ''' (函数返回的是日志文件的文件路径)
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("TraceBugs")>
    Public Function TraceBugs(ex As Exception, <CallerMemberName> Optional trace$ = "") As String
        Dim entry$ = $"{Now.FormatTime("-")}_{App.__getTEMPhash}"
        Dim log$ = $"{App.LogErrDIR}/{entry}.log"
        Call App.LogException(ex, trace:=trace, fileName:=log)
        Return log
    End Function

    ''' <summary>
    ''' MySql时间格式： ``yy-mm-dd, 00:00:00``
    ''' </summary>
    ''' <param name="time"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FormatTime(time As DateTime, Optional sep$ = ":") As String
        Dim yy = Format(time.Year, "0000")
        Dim mm = Format(time.Month, "00")
        Dim dd = Format(time.Day, "00")
        Dim hh = Format(time.Hour, "00")
        Dim mi = Format(time.Minute, "00")
        Dim ss = Format(time.Second, "00")

        Return $"{yy}-{mm}-{dd}, {hh}{sep}{mi}{sep}{ss}"
    End Function

    ''' <summary>
    ''' Is this application running on a Microsoft OS platform.(是否是运行于微软的操作系统平台？)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsMicrosoftPlatform As Boolean = App.__isMicrosoftPlatform

    ''' <summary>
    ''' 这个主要是判断一个和具体的操作系统平台相关的Win32 API是否能够正常的工作？
    ''' </summary>
    ''' <returns></returns>
    Private Function __isMicrosoftPlatform() As Boolean
#If UNIX Then
        Return False
#Else
        Dim pt As PlatformID = Platform

        ' 枚举值在.NET和Mono之间可能会不一样??
        If pt.ToString = NameOf(PlatformID.Unix) Then
            Return False
        ElseIf pt.ToString = NameOf(PlatformID.MacOSX) Then
            Return False
        End If

        Return pt = PlatformID.Win32NT OrElse
            pt = PlatformID.Win32S OrElse
            pt = PlatformID.Win32Windows OrElse
            pt = PlatformID.WinCE OrElse
            pt = PlatformID.Xbox
#End If
    End Function

    ''' <summary>
    ''' Example: ``tmp2A10.tmp``
    ''' </summary>
    Dim _tmpHash As New Uid(Not IsMicrosoftPlatform)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function __getTEMPhash() As String
        SyncLock _tmpHash
            Return FormatZero(++_tmpHash, "00000")
        End SyncLock
    End Function

    ''' <summary>
    ''' 由于可能会运行多个使用本模块的进程，单独考哈希来作为表示会产生冲突，所以这里使用应用程序的启动时间戳以及当前的哈希值来生成唯一标示
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function __getTEMP() As String
        Return $"tmp{App.__getTEMPhash}"
    End Function

    ''' <summary>
    ''' 是名称，不是文件路径
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property NextTempName As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return __getTEMP()
        End Get
    End Property

    ''' <summary>
    ''' Error default log fie location from function <see cref="App.LogException(Exception, ByRef String)"/>.(存放自动存储的错误日志的文件夹)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LogErrDIR As String

    ''' <summary>
    ''' Simply log application exception data into a log file which saves at a user defined location parameter: <paramref name="FileName"/>.
    ''' (简单日志记录)
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <param name="Trace"></param>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("LogException")>
    Public Function LogException(ex As Exception, fileName$, <CallerMemberName> Optional trace$ = Nothing) As Object
        Call BugsFormatter(ex, trace).SaveTo(fileName)
        Return Nothing
    End Function

    ''' <summary>
    ''' Generates the formatted error log file content.(生成简单的日志板块的内容)
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <param name="trace"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("Bugs.Formatter")>
    Public Function BugsFormatter(ex As Exception, <CallerMemberName> Optional trace$ = "") As String
        Dim logs = ex.ToString.LineTokens
        Dim stackTrace = logs _
            .Where(Function(s)
                       Return InStr(s, "   在 ") = 1 OrElse InStr(s, "   at ") = 1
                   End Function) _
            .AsList
        Dim message = logs _
            .Where(Function(s)
                       Return Not s.IsPattern("\s+[-]{3}.+?[-]{3}\s*") AndAlso stackTrace.IndexOf(s) = -1
                   End Function) _
            .JoinBy(ASCII.LF) _
            .Trim _
            .StringSplit("\s[-]{3}>\s")

        Return New StringBuilder() _
            .AppendLine("TIME:  " & Now.ToString) _
            .AppendLine("TRACE: " & trace) _
            .AppendLine(New String("=", 120)) _
            .Append(LogFile.SystemInfo) _
            .AppendLine(New String("=", 120)) _
            .AppendLine() _
            .AppendLine($"Environment Variables from {GetType(App).FullName}:") _
            .AppendLine(ConfigEngine.Prints(App.GetAppVariables)) _
            .AppendLine(New String("=", 120)) _
            .AppendLine() _
            .AppendLine(ex.GetType.FullName & ":") _
            .AppendLine() _
            .AppendLine(message _
                .Select(Function(s) "    ---> " & s) _
                .JoinBy(ASCII.LF)) _
            .AppendLine() _
            .AppendLine(stackTrace _
                .Select(Function(s)
                            If InStr(s, "   在 ") = 1 Then
                                Return Mid(s, 6).Trim
                            ElseIf InStr(s, "   at ") = 1 Then
                                Return Mid(s, 7).Trim
                            Else
                                Return s
                            End If
                        End Function) _
                .Select(Function(s) "   at " & s) _
                .JoinBy(ASCII.LF)) _
            .ToString()
    End Function

    ''' <summary>
    ''' This is the custom message of the exception, not extract from the function <see cref="Exception.ToString()"/>
    ''' </summary>
    ''' <param name="exMsg">This is the custom message of the exception, not extract from the function <see cref="Exception.ToString()"/></param>
    ''' <param name="Trace"></param>
    ''' <returns></returns>
    <ExportAPI("Exception.Log")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LogException(exMsg$, <CallerMemberName> Optional Trace$ = "") As Object
        Return App.LogException(New Exception(exMsg), Trace)
    End Function

    ''' <summary>
    ''' <see cref="App.LocalData"/>/error.log
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ExceptionLogFile As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return App.LocalData & "/error.log"
        End Get
    End Property

#Region "CLI interpreter"

    Public ReadOnly Property Running As Boolean = True

    ''' <summary>
    '''  Terminates this <see cref="System.Diagnostics.Process"/> and gives the underlying operating system the specified exit code.
    '''  (这个方法还会终止本应用程序里面的自动GC线程)
    ''' </summary>
    ''' <param name="state">Exit code to be given to the operating system. Use 0 (zero) to indicate that the process completed successfully.</param>
    '''
    <SecuritySafeCritical> Public Function Exit%(Optional state% = 0)
        App._Running = False

        Call My.InnerQueue.WaitQueue()
        Call App.StopGC()
        Call __GCThread.Dispose()
        Call Environment.Exit(state)

        Return state
    End Function

#If FRAMEWORD_CORE Then

    ''' <summary>
    ''' Running the <see cref="String"/> as cli command line and the specific type define as a <see cref="CommandLine.Interpreter"/>.
    ''' (请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.</param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    '''
    <ExportAPI("RunCLI", Info:="Running the string as cli command line and the specific type define as a interpreter.")>
    <Extension>
    Public Function RunCLI(Interpreter As Type, args$, <CallerMemberName> Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLIInternal(CLITools.TryParse(args), caller, Nothing, Nothing, Nothing)
    End Function

    ''' <summary>
    ''' Running the string as a cli command line.(请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.</param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    '''
    <ExportAPI("RunCLI",
             Info:="Running the string as cli command line and the specific type define as a interpreter.")>
    <Extension> Public Function RunCLI(Interpreter As Type, args As CLI, <CallerMemberName> Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLIInternal(args, caller, Nothing, Nothing, Nothing)
    End Function

    ''' <summary>
    ''' Running the string as a cli command line.(请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.</param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    '''
    <ExportAPI("RunCLI",
             Info:="Running the string as cli command line and the specific type define as a interpreter.")>
    <Extension> Public Function RunCLI(Interpreter As Type, args As CLI, executeEmpty As ExecuteEmptyCLI,
                                       <CallerMemberName>
                                       Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLIInternal(args, caller, executeEmpty, Nothing, Nothing)
    End Function

    ''' <summary>
    ''' Running the string as a cli command line.(请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.</param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    '''
    <ExportAPI("RunCLI")>
    <Extension> Public Function RunCLI(Interpreter As Type, args$, executeEmpty As ExecuteEmptyCLI,
                                       <CallerMemberName>
                                       Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLIInternal(CLITools.TryParse(args), caller, executeEmpty, Nothing, Nothing)
    End Function

    ''' <summary>
    ''' Running the string as a cli command line.(请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">
    ''' The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.
    ''' </param>
    ''' <param name="executeNotFound">
    ''' ```vbnet
    ''' Public Delegate Function ExecuteNotFound(args As CommandLine) As Integer
    ''' ```
    ''' </param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    '''
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("RunCLI")>
    <Extension> Public Function RunCLI(Interpreter As Type, args$, executeEmpty As ExecuteEmptyCLI, executeNotFound As ExecuteNotFound,
                                       <CallerMemberName>
                                       Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLIInternal(CLITools.TryParse(args), caller, executeEmpty, executeNotFound, Nothing)
    End Function

    ''' <summary>
    ''' Running the string as a cli command line.(请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">
    ''' The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.
    ''' </param>
    ''' <param name="executeNotFound">
    ''' ```vbnet
    ''' Public Delegate Function ExecuteNotFound(args As <see cref="CLI"/>) As <see cref="Integer"/>
    ''' ```
    ''' </param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    '''
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("RunCLI")>
    <Extension> Public Function RunCLI(Interpreter As Type, args As CLI, executeEmpty As ExecuteEmptyCLI, executeNotFound As ExecuteNotFound,
                                       <CallerMemberName>
                                       Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLIInternal(args, caller, executeEmpty, executeNotFound, Nothing)
    End Function

    <Extension>
    Private Function RunCLIInternal(App As Type, args As CLI, caller$,
                                    executeEmpty As ExecuteEmptyCLI,
                                    executeNotFound As ExecuteNotFound,
                                    executeFile As ExecuteFile) As Integer
#If DEBUG Then
        Call args.__DEBUG_ECHO
#End If
        Call args.InitDebuggerEnvir(caller)

        If args.Name.TextEquals("/i") Then
            ' 交互式终端模式
            Dim console As New InteractiveConsole(App)
            Return __completeCLI(console.RunApp)
        Else
            Dim program As New Interpreter(App, caller:=caller) With {
                .ExecuteEmptyCli = executeEmpty,
                .ExecuteNotFound = executeNotFound,
                .ExecuteFile = executeFile
            }

            Return __completeCLI(program.Execute(args))
        End If
    End Function

    ''' <summary>
    ''' Running the string as a cli command line.(请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.</param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    '''
    <ExportAPI("RunCLI")>
    <Extension> Public Function RunCLI(Interpreter As Type, args$, executeFile As ExecuteFile, <CallerMemberName> Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLIInternal(CLITools.TryParse(args), caller, Nothing, Nothing, executeFile)
    End Function

    ''' <summary>
    ''' Running the string as a cli command line.(请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.</param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    ''' <param name="executeFile">
    ''' 函数指针：
    ''' ```vbnet
    ''' Function ExecuteFile(path As <see cref="String"/>, args As <see cref="CommandLine"/>) As <see cref="Integer"/>
    ''' ```
    ''' </param>
    <ExportAPI("RunCLI")>
    <Extension> Public Function RunCLI(Interpreter As Type, args As CLI, executeFile As ExecuteFile, <CallerMemberName> Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLIInternal(args, caller, Nothing, Nothing, executeFile)
    End Function

    ''' <summary>
    ''' Running the string as a cli command line.(请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.</param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    '''
    <ExportAPI("RunCLI")>
    <Extension> Public Function RunCLI(Interpreter As Type, args$, executeFile As ExecuteFile, executeEmpty As ExecuteEmptyCLI,
                                       <CallerMemberName>
                                       Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLI(TryParse(args), executeFile, executeEmpty, caller)
    End Function

    ''' <summary>
    ''' Running the string as a cli command line.(请注意，在调试模式之下，命令行解释器会在运行完命令之后暂停，而Release模式之下则不会。
    ''' 假若在调试模式之下发现程序有很长一段时间处于cpu占用为零的静止状态，则很有可能已经运行完命令并且等待回车退出)
    ''' </summary>
    ''' <param name="args">The command line arguments value, which its value can be gets from the <see cref="Command()"/> function.</param>
    ''' <returns>Returns the function execute result to the operating system.</returns>
    '''
    <ExportAPI("RunCLI")>
    <Extension> Public Function RunCLI(Interpreter As Type, args As CLI, executeFile As ExecuteFile, executeEmpty As ExecuteEmptyCLI,
                                       <CallerMemberName>
                                       Optional caller$ = Nothing) As Integer
        Return Interpreter.RunCLIInternal(args, caller, executeEmpty, Nothing, executeFile)
    End Function
#End If

    ''' <summary>
    ''' IF the flag is True, that means cli API execute successfully, function returns ZERO, or a negative integer(Default -100) for failures.
    ''' </summary>
    ''' <param name="b"></param>
    ''' <param name="Failed"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function CLICode(b As Boolean, Optional Failed As Integer = -100) As Integer
        Return If(b, 0, Failed)
    End Function

#End Region

    ''' <summary>
    ''' Creates a uniquely named zero-byte temporary file on disk and returns the full
    ''' path of that file.
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("GetTempFile")>
    Public Function GetTempFile() As String
        Dim Temp As String = FileIO.FileSystem.GetTempFileName
        Return GenerateTemp(Temp, "")
    End Function

    ''' <summary>
    ''' Get temp file name in app system temp directory.
    ''' </summary>
    ''' <param name="ext"></param>
    ''' <param name="sessionID"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("GetTempFile.AppSys")>
    Public Function GetAppSysTempFile(Optional ext$ = ".tmp", Optional sessionID$ = "") As String
        Dim tmp As String = App.SysTemp & "/" & __getTEMP() & ext  '  FileIO.FileSystem.GetTempFileName.Replace(".tmp", ext)
        tmp = GenerateTemp(tmp, sessionID)
        Call FileIO.FileSystem.CreateDirectory(FileIO.FileSystem.GetParentPath(tmp))
        tmp = FileIO.FileSystem.GetFileInfo(tmp).FullName.Replace("\", "/")
        Return tmp
    End Function

    Public ReadOnly Property CurrentProcessTemp As String

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="sysTemp">临时文件路径</param>
    ''' <returns></returns>
    '''
    <ExportAPI("CreateTempFile")>
    Public Function GenerateTemp(sysTemp$, SessionID$) As String
        Dim Dir As String = FileIO.FileSystem.GetParentPath(sysTemp)
        Dim Name As String = FileIO.FileSystem.GetFileInfo(sysTemp).Name
        sysTemp = $"{Dir}/{App.AssemblyName}/{SessionID}/{Name}"
        Return sysTemp
    End Function

    ''' <summary>
    ''' Gets a temp file name which is located at directory <see cref="App.ProductSharedDIR"/>.
    ''' (获取位于共享文件夹<see cref="App.ProductSharedDIR"/>里面的临时文件)
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("Shared.TempFile")>
    Public Function GetProductSharedTemp() As String
        'Dim Temp As String = FileIO.FileSystem.GetTempFileName
        Dim Name As String = App.__getTEMPhash  'FileIO.FileSystem.GetFileInfo(Temp).Name
        'Name = Name.ToUpper.Replace("TMP", "")
        Dim Temp = $"{App.ProductSharedTemp}/{App.AssemblyName}-{Name}.tmp"
        Return Temp
    End Function

    Public ReadOnly Property ProductSharedTemp As String

    ''' <summary>
    ''' Gets a <see cref="System.PlatformID"/> enumeration value that identifies the operating system
    ''' platform.
    ''' </summary>
    ''' <remarks>One of the System.PlatformID values.</remarks>
    Public ReadOnly Property Platform As PlatformID = Environment.OSVersion.Platform

    ''' <summary>
    ''' Self call this program itself for batch parallel task calculation.
    ''' (调用自身程序，这个通常是应用于批量的数据的计算任务的实现)
    ''' </summary>
    ''' <param name="CLI"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("Folk.Self")>
    Public Function SelfFolk(CLI As String) As IIORedirectAbstract
        Return Shell(App.ExecutablePath, CLI, CLR:=True)
    End Function

    ''' <summary>
    ''' Folk helper for running the other .NET application.
    ''' (请注意，这个函数只能够运行.NET程序, 假若是在Linux系统之上，还需要安装mono运行时环境)
    ''' </summary>
    ''' <param name="app">External application file full path</param>
    ''' <param name="CLI">Commandline string that running the application <paramref name="app$"/></param>
    ''' <param name="CLR">Is the calling external application is a .NET application?
    ''' (是否为.NET程序?)
    ''' </param>
    ''' <returns></returns>
    ''' <remarks><see cref="IORedirectFile"/>这个建议在进行外部调用的时候才使用</remarks>
    Public Function Shell(app$, CLI$,
                          Optional CLR As Boolean = False,
                          Optional stdin$ = Nothing,
                          Optional ioRedirect As Boolean = False) As IIORedirectAbstract

        If Not IsMicrosoftPlatform Then
            If CLR Then
                Dim process As New ProcessEx With {
                    .Bin = "mono",
                    .CLIArguments = app.CLIPath & " " & CLI
                }
                Return process
            Else
                Dim process As New IORedirectFile(app, CLI, stdin:=stdin)
                Return process
            End If
        Else
            If CLR Then
                ' 由于是重新调用自己，所以这个重定向是没有多大问题的
                Return New IORedirect(app, CLI, IOredirect:=ioRedirect)
            Else
                Dim process As New IORedirectFile(app, CLI, stdin:=stdin)
                Return process
            End If
        End If
    End Function

    ''' <summary>
    ''' Folk this program itself for the large amount data batch processing.
    ''' </summary>
    ''' <param name="CLI">Self folk processing commandline collection.</param>
    ''' <param name="parallel">If this parameter value less than 1, then will be a single 
    ''' thread task. Any positive value that greater than 1 will be parallel task.
    ''' (小于等于零表示非并行化，单线程任务)
    ''' </param>
    ''' <param name="smart">Smart mode CPU load threshold, if the <paramref name="parallel"/> 
    ''' parameter value is less than or equals to 1, then this parameter will be disabled.
    ''' </param>
    ''' <returns>
    ''' Returns the total executation time for running this task collection.
    ''' (返回任务的执行的总时长)
    ''' </returns>
    <ExportAPI("Folk.Self")>
    Public Function SelfFolks&(CLI As IEnumerable(Of String),
                               Optional parallel% = 0,
                               Optional smart# = 0)

        Dim sw As Stopwatch = Stopwatch.StartNew

        If parallel <= 0 Then
            For Each args As String In CLI
                Call App.SelfFolk(args).Run()
            Next
        Else
            Dim Tasks As Func(Of Integer)() = LinqAPI.Exec(Of Func(Of Integer)) <=
 _
                From args As String
                In CLI
                Let io As IIORedirectAbstract = App.SelfFolk(args)
                Let task As Func(Of Integer) = AddressOf io.Run
                Select task

            Call BatchTask(Of Integer)(Tasks, parallel, TimeInterval:=200)
        End If

        Return sw.ElapsedMilliseconds
    End Function

#Region "Auto Garbage Cleaner"

    ''' <summary>
    ''' 自动垃圾回收线程
    ''' </summary>
    ReadOnly __GCThread As New UpdateThread(10 * 60 * 1000, AddressOf App.__GCThreadInvoke)

    Dim _CLIAutoClean As Boolean = False
    Dim __exitHooks As New List(Of Action)

    ''' <summary>
    ''' 这里添加在应用程序退出执行的时候所需要完成的任务
    ''' </summary>
    ''' <param name="hook"></param>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub AddExitCleanHook(hook As Action)
        SyncLock __exitHooks
            With __exitHooks
                Call .Add(hook)
            End With
        End SyncLock
    End Sub

    ''' <summary>
    ''' 自动停止GC当前程序的线程
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    Private Function __completeCLI(state As Integer) As Integer
        App._Running = False

        If _CLIAutoClean Then
            Call StopGC()
        End If

        ' 在这里等待终端的内部线程输出工作完毕，防止信息的输出错位

        Call My.InnerQueue.WaitQueue()
        Call Console.WriteLine()

        For Each hook As Action In __exitHooks
            Call hook()
        Next

        Call My.InnerQueue.WaitQueue()
        Call Console.WriteLine()

#If DEBUG Then
        ' this option enable you disable the pause in debug mode 
        ' when the program is going to end.
        If Not App.GetVariable("pause.disable").ParseBoolean = True Then
            ' 应用程序在 debug 模式下会自动停止在这里
            Call Pause()
        End If
#End If
        Return state
    End Function

    ''' <summary>
    ''' Start the automatic garbage collection threads.
    ''' (这条线程只会自动清理*.tmp临时文件，因为假若不清理临时文件的话，有时候临时文件比较多的时候，会严重影响性能，甚至无法运行应用程序框架里面的IO重定向操作)
    ''' </summary>
    Public Sub StartGC(autoClose As Boolean)
        ' 因为有一部分程序假若在执行一个很长的任务的话，是会将一些中间文件存放在临时文件夹的
        ' 使用这个自动清理功能的函数，可能会将这些有用的中间文件给删除掉
        ' 所以在这里给出一条警告信息，方便在调试的时候了解这个自动垃圾回收线程是否被启动了
        Call App.__GCThread.Start()
        Call "Garbage auto collection thread started!".Warning

        App._CLIAutoClean = autoClose
    End Sub

    ''' <summary>
    ''' 自动垃圾回收线程
    ''' </summary>
    Private Sub __GCThreadInvoke()

        Call App.__removesTEMP(App.SysTemp)
        Call App.__removesTEMP(App.AppSystemTemp)
        Call App.__removesTEMP(App.ProductSharedTemp)
        Call App.__removesTEMP(App.LocalDataTemp)

        Call FlushMemory()
    End Sub

    <Extension>
    Private Function __listFiles(DIR As String) As IEnumerable(Of String)
        Try
            Return ls - l - r - {"*.tmp", "*.temp"} <= DIR
        Catch ex As Exception
            Call App.LogException(ex)
            Return {}
        End Try
    End Function

    ''' <summary>
    ''' The Windows file system have a limit of the numbers in a folder, so the long time running application 
    ''' required a thread to make the temp directory cleanup, or the application will no able to create temp 
    ''' file when the temp folder reach its file number upbound(This may caused the application crashed).
    ''' </summary>
    ''' <param name="TEMP"></param>
    Private Sub __removesTEMP(TEMP As String)
        For Each file As String In TEMP.__listFiles
            Try
                Call FileIO.FileSystem.DeleteFile(file)
            Finally
                ' DO Nothing
            End Try
        Next
    End Sub

    ''' <summary>
    ''' Stop the automatic garbage collection threads.
    ''' </summary>
    Public Sub StopGC()
        Call App.__GCThread.Stop()
    End Sub
#End Region

    ''' <summary>
    ''' Restart the current process with administrator credentials.(以管理员的身份重启本应用程序)
    ''' </summary>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub RunAsAdmin(Optional args$ = "")
        Call RestartElevated(args)
    End Sub
End Module
