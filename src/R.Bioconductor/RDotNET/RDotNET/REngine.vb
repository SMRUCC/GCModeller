#Region "Microsoft.VisualBasic::5fcaa53d548bb5d4c5ac3e066206886b, RDotNET\RDotNET\REngine.vb"

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

    ' Class REngine
    ' 
    '     Properties: AutoPrint, BaseNamespace, Disposed, DllVersion, EmptyEnvironment
    '                 EnableLock, EngineName, GlobalEnvironment, ID, IsRunning
    '                 LastErrorMessage, NilValue, UnboundValue
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: BuildRArgv, CreateFromNativeSexp, CreateInstance, (+2 Overloads) Defer, EncodeNonAsciiCharacters
    '               (+2 Overloads) Evaluate, EvenStringDelimitors, GetInstance, GetPredefinedSymbol, (+2 Overloads) GetSymbol
    '               GetVisible, IndexOfAll, IsClosedString, Parse, processInputString
    '               processLine, ProcessRDllFileName, Segment, splitOnFirst, splitOnNewLines
    '               splitOnStatementSeparators
    ' 
    '     Sub: CheckEngineIsRunning, Dispose, ForceGarbageCollection, Initialize, OnDisposing
    '          SetCommandLineArguments, SetCstackChecking, SetEnvironmentVariables, (+2 Overloads) SetSymbol
    '     Delegate Function
    ' 
    '         Properties: NaString, NaStringPointer
    ' 
    '         Sub: ClearGlobalEnvironment, doDetachPackages, DoDotNetGarbageCollection
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports RDotNet.Devices
Imports RDotNet.Internals
Imports RDotNet.NativeLibrary
Imports RDotNet.Utilities

''' <summary>
''' REngine handles R environment through evaluation of R statement.
''' </summary>
''' <example>This example generates and outputs five random numbers from standard normal distribution.
''' <code>
''' Environment.SetEnvironmentVariable("PATH", @"C:\Program Files\R\R-2.12.0\bin\i386");
''' using (REngine engine = REngine.CreateInstance("RDotNet"))
''' {
'''   engine.Initialize();
'''	NumericVector random = engine.Evaluate("rnorm(5, 0, 1)").AsNumeric();
'''	foreach (double r in random)
'''	{
'''		Console.Write(r + " ");
'''	}
''' }
''' </code>
''' </example>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class REngine
    Inherits DynamicInterop.UnmanagedDll
    Private Shared ReadOnly DefaultDevice As ICharacterDevice = New ConsoleDevice()

    Private ReadOnly m_id As String
    Private adapter As CharacterDeviceAdapter
    Private m_isRunning As Boolean
    Private parameter As StartupParameter
    Private Shared environmentIsSet As Boolean = False
    Private Shared engine As REngine = Nothing

    ''' <summary>
    ''' Create a new REngine instance
    ''' </summary>
    ''' <param name="id">The identifier of this object</param>
    ''' <param name="dll">The name of the file that is the shared R library, e.g. "R.dll"</param>
    Protected Sub New(id As String, dll As String)
        MyBase.New(dll)
        Me.m_id = id
        Me.m_isRunning = False
        Me.Disposed = False
        ' See https://rdotnet.codeplex.com/workitem/113; it seems wise to enable it by default.
        Me.EnableLock = False
        Me.AutoPrint = True
    End Sub

    ''' <summary>
    ''' Gets/sets whether the call to Preserve and Unpreserve on symbolic expressions
    ''' should be using a lock to prevent thread concurrency issues. Default is false;
    ''' </summary>
    ''' <remarks>Thanks to gchapman for proposing the fix. See https://rdotnet.codeplex.com/workitem/67 for details</remarks>
    Public Property EnableLock() As Boolean

    ''' <summary>
    ''' Gets whether this instance is running.
    ''' </summary>
    Public ReadOnly Property IsRunning() As Boolean
        Get
            Return Me.m_isRunning
        End Get
    End Property

    ''' <summary>
    ''' Gets the version of R.DLL.
    ''' </summary>
    Public ReadOnly Property DllVersion() As String
        Get
            ' As R's version definitions are defined in #define preprocessor,
            ' C# cannot access them dynamically.
            ' But, on Win32 platform, we can get the version string via getDLLVersion function.
            If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                Throw New NotImplementedException()
            End If
            Dim getVersion = GetFunction(Of _getDLLVersion)("getDLLVersion")
            Return Marshal.PtrToStringAnsi(getVersion())
        End Get
    End Property

    ''' <summary>
    ''' Gets the ID of this instance.
    ''' </summary>
    Public ReadOnly Property ID() As String
        Get
            Return Me.m_id
        End Get
    End Property

    ''' <summary>
    ''' Gets the global environment.
    ''' </summary>
    Public ReadOnly Property GlobalEnvironment() As REnvironment
        Get
            CheckEngineIsRunning()
            Return GetPredefinedSymbol("R_GlobalEnv").AsEnvironment()
        End Get
    End Property

    Private Sub CheckEngineIsRunning()
        If Not IsRunning Then
            Throw New InvalidOperationException("This engine is not running. You may have forgotten to call Initialize")
        End If
    End Sub

    ''' <summary>
    ''' Gets the root environment.
    ''' </summary>
    Public ReadOnly Property EmptyEnvironment() As REnvironment
        Get
            CheckEngineIsRunning()
            Return GetPredefinedSymbol("R_EmptyEnv").AsEnvironment()
        End Get
    End Property

    ''' <summary>
    ''' Gets the base environment.
    ''' </summary>
    Public ReadOnly Property BaseNamespace() As REnvironment
        Get
            CheckEngineIsRunning()
            Return GetPredefinedSymbol("R_BaseNamespace").AsEnvironment()
        End Get
    End Property

    ''' <summary>
    ''' Gets the <c>NULL</c> value.
    ''' </summary>
    Public ReadOnly Property NilValue() As SymbolicExpression
        Get
            CheckEngineIsRunning()
            Return GetPredefinedSymbol("R_NilValue")
        End Get
    End Property

    ''' <summary>
    ''' Gets the unbound value.
    ''' </summary>
    Public ReadOnly Property UnboundValue() As SymbolicExpression
        Get
            CheckEngineIsRunning()
            Return GetPredefinedSymbol("R_UnboundValue")
        End Get
    End Property

    ''' <summary>
    ''' Gets the name of the R engine instance (singleton).
    ''' </summary>
    Public Shared ReadOnly Property EngineName() As String
        Get
            Return "R.NET"
        End Get
    End Property

    ''' <summary>
    ''' Gets a reference to the R engine, creating and initializing it if necessary. In most cases users need not provide any parameter to this method.
    ''' </summary>
    ''' <param name="dll">The file name of the library to load, e.g. "R.dll" for Windows. You usually do not need need to provide this optional parameter</param>
    ''' <param name="initialize">Initialize the R engine after its creation. Default is true</param>
    ''' <param name="parameter">If 'initialize' is 'true', you can optionally specify the specific startup parameters for the R native engine</param>
    ''' <param name="device">If 'initialize' is 'true', you can optionally specify a character device for the R engine to use</param>
    ''' <returns>The engine.</returns>
    ''' <example>
    ''' <p>A minimalist approach is to just call GetInstance</p>
    ''' <code>
    ''' REngine.SetEnvironmentVariables();
    ''' var engine = REngine.GetInstance();
    ''' engine.Evaluate("letters[1:26]");
    ''' </code>
    ''' <p>In unusual circumstances you may need to elaborate on the initialization in a separate method call</p>
    ''' <code>
    ''' REngine.SetEnvironmentVariables(rPath=@"c:\my\peculiar\path\to\R\bin\x64");
    ''' var engine = REngine.GetInstance(initialize=false);
    ''' StartupParameter sParams=new StartupParameter(){NoRenviron=true;};
    ''' ICharacterDevice device = new YourCustomDevice();
    ''' engine.Initialize(parameter: sParams, device: device);
    ''' engine.Evaluate("letters[1:26]");
    ''' </code>
    ''' </example>
    Public Shared Function GetInstance(Optional dll As String = Nothing, Optional initialize As Boolean = True, Optional parameter As StartupParameter = Nothing, Optional device As ICharacterDevice = Nothing) As REngine
        If Not environmentIsSet Then
            ' should there be a warning? and how?
            SetEnvironmentVariables()
        End If
        If engine Is Nothing Then
            engine = CreateInstance(EngineName, dll)
            If initialize Then
                engine.Initialize(parameter, device)
            End If
        End If
        If engine.Disposed Then
            Throw New InvalidOperationException("The single REngine instance has already been disposed of (i.e. shut down). Multiple engine restart is not possible.")
        End If
        Return engine
    End Function

    ''' <summary>
    ''' Creates a new instance that handles R.DLL.
    ''' </summary>
    ''' <param name="id">ID.</param>
    ''' <param name="dll">The file name of the library to load, e.g. "R.dll" for Windows. You should usually not provide this optional parameter</param>
    ''' <returns>The engine.</returns>
    Private Shared Function CreateInstance(id As String, Optional dll As String = Nothing) As REngine
        If id Is Nothing Then
            Throw New ArgumentNullException("id", "Empty ID is not allowed.")
        End If
        If id = String.Empty Then
            Throw New ArgumentException("Empty ID is not allowed.", "id")
        End If
        dll = ProcessRDllFileName(dll)
        Dim engine = New REngine(id, dll)
        Return engine
    End Function

    ''' <summary>
    ''' if the parameter is null or empty string, return the default names of the R shared library file depending on the platform
    ''' </summary>
    ''' <param name="dll">The name of the library provided, possibly null or empty</param>
    ''' <returns>A candidate for the file name of the R shared library</returns>
    Protected Shared Function ProcessRDllFileName(dll As String) As String
        If Not String.IsNullOrEmpty(dll) Then
            Return dll
        End If
        Return NativeUtility.GetRLibraryFileName()
    End Function

    Private Shared Function EncodeNonAsciiCharacters(value As String) As String
        Dim sb As New StringBuilder()
        For Each c As Char In value
            If AscW(c) > 127 Then
                Dim encodedValue As String = "\u" & (AscW(c)).ToString("x4")
                sb.Append(encodedValue)
            Else
                sb.Append(c)
            End If
        Next
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Perform the necessary setup for the PATH and R_HOME environment variables.
    ''' </summary>
    ''' <param name="rPath">The path of the directory containing the R native library.
    ''' If null (default), this function tries to locate the path via the Windows registry, or commonly used locations on MacOS and Linux</param>
    ''' <param name="rHome">The path for R_HOME. If null (default), the function checks the R_HOME environment variable. If none is set,
    ''' the function uses platform specific sensible default behaviors.</param>
    ''' <remarks>
    ''' This function has been designed to limit the tedium for users, while allowing custom settings for unusual installations.
    ''' </remarks>
    Public Shared Sub SetEnvironmentVariables(Optional rPath As String = Nothing, Optional rHome As String = Nothing)
        environmentIsSet = True
        NativeUtility.CreateNew().SetEnvironmentVariables(rPath:=rPath, rHome:=rHome)
    End Sub

    ''' <summary>
    ''' Initialize this REngine object. Only the first call has an effect. Subsequent calls to this function are ignored.
    ''' </summary>
    ''' <param name="parameter">The optional startup parameters</param>
    ''' <param name="device">The optional character device to use for the R engine</param>
    ''' <param name="setupMainLoop">if true, call the functions to initialise the embedded R</param>
    Public Sub Initialize(Optional parameter As StartupParameter = Nothing, Optional device As ICharacterDevice = Nothing, Optional setupMainLoop As Boolean = True)
        '         Console.WriteLine("REngine.Initialize start");
        If Me.m_isRunning Then
            Return
        End If
        '         Console.WriteLine("REngine.Initialize, after isRunning checked as false");
        Me.parameter = If(parameter, New StartupParameter())
        Me.adapter = New CharacterDeviceAdapter(If(device, DefaultDevice))
        ' Disabling the stack checking here, to try to avoid the issue on Linux.
        ' The disabling used to be here around end Nov 2013. Was moved later in this
        ' function to cater for disabling on Windows, @ rev 305, however this may have
        ' re-broken on Linux. so we may need to call it twice.
        SetCstackChecking()
        '         Console.WriteLine("Initialize-SetCstackChecking; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

        If Not setupMainLoop Then
            Me.m_isRunning = True
            Return
        End If

        Dim R_argv As String() = BuildRArgv(Me.parameter)
        'string[] R_argv = new[]{"rdotnet_app",  "--interactive",  "--no-save",  "--no-restore-data",  "--max-ppsize=50000"};
        'rdotnet_app --quiet --interactive --no-save --no-restore-data --max-mem-size=18446744073709551615 --max-ppsize=50000
        GetFunction(Of R_setStartTime)()()
        Dim R_argc As Integer = R_argv.Length
        '         Console.WriteLine("Initialize-R_setStartTime; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

        ' Attempted Fix for https://rdotnet.codeplex.com/workitem/110; not working
        ' Tried to make sure that the command line options are taken into account. They are NOT effectively so via Rf_initialize_R only.
        ' The problem is that cmdlineoptions assumes it is called by RGui.exe or RTerm.exe, and overrides R_HOME

        '   GetFunction<R_set_command_line_arguments>()(R_argc, R_argv);
        '   GetFunction<cmdlineoptions>()(R_argc, R_argv);
        If NativeUtility.GetPlatform() = PlatformID.Win32NT Then
        End If

        Dim status = GetFunction(Of Rf_initialize_R)()(R_argc, R_argv)
        If status <> 0 Then
            Throw New Exception("A call to Rf_initialize_R returned a non-zero; status=" & Convert.ToString(status))
        End If
        '         Console.WriteLine("Initialize-Rf_initialize_R; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));
        SetCstackChecking()

        ' following in RInside: may not be needed.
        'GetFunction<R_ReplDLLinit> () ();
        'this.parameter.Interactive = true;
        Me.adapter.Install(Me, Me.parameter)
        'Console.WriteLine("Initialize-adapter installation; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));
        Select Case NativeUtility.GetPlatform()
            Case PlatformID.Win32NT
                GetFunction(Of R_SetParams_Windows)("R_SetParams")(Me.parameter.start)
                Exit Select

            Case PlatformID.MacOSX, PlatformID.Unix
                GetFunction(Of R_SetParams_Unix)("R_SetParams")(Me.parameter.start.Common)
                'Console.WriteLine("Initialize-R_SetParams_Unix; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));
                Exit Select
        End Select
        GetFunction(Of setup_Rmainloop)()()
        'Console.WriteLine("Initialize-after setup_Rmainloop; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

        ' See comments in the first call to SetCstackChecking in this function as to why we (may) need it twice.
        SetCstackChecking()
        Me.m_isRunning = True

        'Console.WriteLine("Initialize-just before leaving; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

        ' Partial Workaround (hopefully temporary) for https://rdotnet.codeplex.com/workitem/110
        If NativeUtility.GetPlatform() = PlatformID.Win32NT Then
            Evaluate(String.Format("invisible(memory.limit({0}))", (Me.parameter.MaxMemorySize \ 1048576UL)))
        End If
    End Sub

    Private Sub SetCstackChecking()
        ' Don't do any stack checking, see R Exts, '8.1.5 Threading issues',
        ' https://rdotnet.codeplex.com/discussions/462947
        ' https://rdotnet.codeplex.com/workitem/115
        WriteInt32("R_CStackLimit", -1)
        Select Case NativeUtility.GetPlatform()
            Case PlatformID.MacOSX, PlatformID.Unix
                WriteInt32("R_SignalHandlers", 0)
                ' RInside does this for non-WIN32.
                Exit Select
        End Select
    End Sub

    ''' <summary>
    ''' Creates the command line arguments corresponding to the specified startup parameters
    ''' </summary>
    ''' <param name="parameter"></param>
    ''' <returns></returns>
    ''' <remarks>While not obvious from the R documentation, it seems that command line arguments need to be passed
    ''' to get the startup parameters taken into account. Passing the StartupParameter to the API seems not to work as expected.
    ''' While this function may appear like an oddity to a reader, it proved necessary to the initialisation of the R engine
    ''' after much trial and error.</remarks>
    Public Shared Function BuildRArgv(parameter As StartupParameter) As String()
        Dim platform = NativeUtility.GetPlatform()
        Dim argv = New List(Of String)()
        argv.Add("rdotnet_app")
        ' Not sure whether I should add no-readline
        '[MarshalAs(UnmanagedType.Bool)]
        'public bool R_Quiet;
        If parameter.Quiet AndAlso Not parameter.Interactive Then
            argv.Add("--quiet")
        End If
        ' --quite --interactive to R embedded crashed...
        '[MarshalAs(UnmanagedType.Bool)]
        'public bool R_Slave;
        If parameter.Slave Then
            argv.Add("--slave")
        End If

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool R_Interactive;
        If platform <> PlatformID.Win32NT Then
            ' RTerm.exe --help shows no such option; Unix only.
            If parameter.Interactive Then
                argv.Add("--interactive")
            End If
        End If

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool R_Verbose;
        If parameter.Verbose Then
            argv.Add("--verbose")
        End If

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool LoadSiteFile;
        If Not parameter.LoadSiteFile Then
            argv.Add("--no-site-file")
        End If

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool LoadInitFile;
        If Not parameter.LoadInitFile Then
            argv.Add("--no-init-file")
        End If

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool DebugInitFile;
        'if (parameter.Quiet) argv.Add("--quiet");

        'public StartupRestoreAction RestoreAction;
        'public StartupSaveAction SaveAction;
        'internal UIntPtr vsize;
        'internal UIntPtr nsize;
        'internal UIntPtr max_vsize;
        'internal UIntPtr max_nsize;
        'internal UIntPtr ppsize;

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool NoRenviron;
        If parameter.NoRenviron Then
            argv.Add("--no-environ")
        End If

        Select Case parameter.SaveAction
            Case StartupSaveAction.NoSave
                argv.Add("--no-save")
                Exit Select

            Case StartupSaveAction.Save
                argv.Add("--save")
                Exit Select
        End Select
        Select Case parameter.RestoreAction
            Case StartupRestoreAction.NoRestore
                argv.Add("--no-restore-data")
                Exit Select

            Case StartupRestoreAction.Restore
                argv.Add("--restore")
                Exit Select
        End Select

        ' This creates a nasty crash if using the default MaxMemorySize. found out in Rdotnet workitem 72
        ' do nothing
        If parameter.MaxMemorySize = (If(Environment.Is64BitProcess, ULong.MaxValue, UInteger.MaxValue)) Then
        Else
            If platform = PlatformID.Win32NT Then
                ' On unix, otherwise led to https://rdotnet.codeplex.com/workitem/137
                argv.Add("--max-mem-size=" & parameter.MaxMemorySize)
            End If
        End If
        argv.Add("--max-ppsize=" & parameter.StackSize)
        Return argv.ToArray()
    End Function

    ''' <summary>
    ''' Forces garbage collection.
    ''' </summary>
    Public Sub ForceGarbageCollection()
        GetFunction(Of R_gc)()()
    End Sub

    ''' <summary>
    ''' Gets a symbol defined in the global environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <returns>The symbol.</returns>
    Public Function GetSymbol(name As String) As SymbolicExpression
        CheckEngineIsRunning()
        Return GlobalEnvironment.GetSymbol(name)
    End Function

    ''' <summary>
    ''' Gets a symbol defined in the global environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <param name="environment">The environment. If <c>null</c> is passed, <see cref="GlobalEnvironment"/> is used.</param>
    ''' <returns>The symbol.</returns>
    Public Function GetSymbol(name As String, environment As REnvironment) As SymbolicExpression
        CheckEngineIsRunning()
        If environment Is Nothing Then
            environment = GlobalEnvironment
        End If
        Return environment.GetSymbol(name)
    End Function

    ''' <summary>
    ''' Assign a value to a name in the global environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <param name="expression">The symbol.</param>
    Public Sub SetSymbol(name As String, expression As SymbolicExpression)
        CheckEngineIsRunning()
        GlobalEnvironment.SetSymbol(name, expression)
    End Sub

    ''' <summary>
    ''' Assign a value to a name in a specific environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <param name="expression">The symbol.</param>
    ''' <param name="environment">The environment. If <c>null</c> is passed, <see cref="GlobalEnvironment"/> is used.</param>
    Public Sub SetSymbol(name As String, expression As SymbolicExpression, environment As REnvironment)
        CheckEngineIsRunning()
        If environment Is Nothing Then
            environment = GlobalEnvironment
        End If
        environment.SetSymbol(name, expression)
    End Sub

    ''' <summary>
    ''' Evaluates a statement in the given string.
    ''' </summary>
    ''' <param name="statement">The statement.</param>
    ''' <returns>Last evaluation.</returns>
    Public Overridable Function Evaluate(statement As String) As SymbolicExpression
        CheckEngineIsRunning()
        Return Defer(EncodeNonAsciiCharacters(statement)).LastOrDefault()
    End Function

    ''' <summary>
    ''' Evaluates a statement in the given stream.
    ''' </summary>
    ''' <param name="stream">The stream.</param>
    ''' <returns>Last evaluation.</returns>
    Public Function Evaluate(stream As Stream) As SymbolicExpression
        CheckEngineIsRunning()
        Return Defer(stream).LastOrDefault()
    End Function

    ''' <summary>
    ''' Evaluates a statement in the given string.
    ''' </summary>
    ''' <param name="statement">The statement.</param>
    ''' <returns>Each evaluation.</returns>
    Private Iterator Function Defer(statement As String) As IEnumerable(Of SymbolicExpression)
        Call CheckEngineIsRunning()

        If statement Is Nothing Then
            Throw New ArgumentNullException()
        End If

        Using reader As TextReader = New StringReader(statement)
            Dim incompleteStatement As New StringBuilder()
            Dim line As Value(Of String) = ""

            While (line = reader.ReadLine()) IsNot Nothing
                For Each segmentText As String In Segment(line)
                    Dim result = Parse(segmentText, incompleteStatement)
                    If result IsNot Nothing Then
                        Yield result
                    End If
                Next
            End While
        End Using
    End Function

    ''' <summary>
    ''' Evaluates a statement in the given stream.
    ''' </summary>
    ''' <param name="stream">The stream.</param>
    ''' <returns>Each evaluation.</returns>
    Public Iterator Function Defer(stream As Stream) As IEnumerable(Of SymbolicExpression)
        CheckEngineIsRunning()
        If stream Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not stream.CanRead Then
            Throw New ArgumentException()
        End If

        Using reader As TextReader = New StreamReader(stream)
            Dim incompleteStatement = New StringBuilder()
            Dim line As Value(Of String) = ""

            While (line = reader.ReadLine()) IsNot Nothing
                For Each segmentText As String In Segment(line)
                    Dim result = Parse(segmentText, incompleteStatement)
                    If result IsNot Nothing Then
                        Yield result
                    End If
                Next
            End While
        End Using
    End Function

    Private Shared Iterator Function Segment(line As String) As IEnumerable(Of String)
        Dim segments = processInputString(line)

        For index = 0 To segments.Length - 1
            If index = segments.Length - 1 Then
                If segments(index) <> String.Empty Then
                    Yield segments(index) & vbLf
                End If
            Else
                Yield segments(index) & ";"
            End If
        Next
    End Function

    Private Shared Function processInputString(input As String) As String()
        ' Fixes for
        ' https://rdotnet.codeplex.com/workitem/165
        ' https://github.com/jmp75/rdotnet/issues/14
        Dim lines As String() = splitOnNewLines(input)
        Dim statements As New List(Of String)()
        For i As Integer = 0 To lines.Length - 1
            statements.AddRange(processLine(lines(i)))
        Next
        Return statements.ToArray()
    End Function

    Private Shared Function splitOnNewLines(input As String) As String()
        input = input.Replace(vbLf & vbCr, vbLf)
        Return input.Split(ControlChars.Lf)
    End Function

    Private Shared Function processLine(line As String) As String()
        Dim trimmedLine = line.Trim()
        If trimmedLine = String.Empty Then
            Return New String() {}
        End If
        If trimmedLine.StartsWith("#") Then
            Return New String() {line}
        End If

        Dim theRest As String = ""
        Dim statement As String = splitOnFirst(line, theRest, ";"c)

        Dim result = New List(Of String)()
        If Not statement.Contains("#") Then
            result.Add(statement)
            result.AddRange(processLine(theRest))
        Else
            ' paste('this contains ### characters', " this too ###", 'Oh, and this # one too') # but "this" 'rest' is commented
            ' Find the fist # character such that before that, there is an 
            ' even number of " and an even number of ' characters

            Dim whereHash As Integer() = IndexOfAll(statement, "#")
            Dim firstComment As Integer = EvenStringDelimitors(statement, whereHash)
            If firstComment < 0 Then
                ' incomplete statement??? such as:
                ' paste('this is the # ', ' start of an incomplete # statement
                result.Add(statement)
                result.AddRange(processLine(theRest))
            Else
                ' firstComment is a valid comment marker - not need to process "the rest"
                result.Add(statement.Substring(0, firstComment))
            End If
            Dim restFirstStatement As String = ""
            Dim beforeComment As String = splitOnFirst(statement, restFirstStatement, "#"c)
        End If
        Return result.ToArray()
    End Function

    Private Shared Function EvenStringDelimitors(statement As String, whereHash As Integer()) As Integer
        For i As Integer = 0 To whereHash.Length - 1
            Dim s = statement.Substring(0, whereHash(i))
            If IsClosedString(s) Then
                Return whereHash(i)
            End If
        Next
        Return -1
    End Function

    Private Shared Function IsClosedString(s As String) As Boolean
        ' paste("#hashtag")
        ' paste("#hashtag''''")
        ' paste('#hashtag""""')
        ' paste('#hashtag""#""')
        ' paste('#hashtag""#""', "#hash ''' ")
        Dim inSingleQuote As Boolean = False, inDoubleQuotes As Boolean = False
        For i As Integer = 0 To s.Length - 1
            If s(i) = "'"c Then
                If i > 0 Then
                    If s(i - 1) = "\"c Then
                        Continue For
                    End If
                End If
                If inDoubleQuotes Then
                    Continue For
                End If
                inSingleQuote = Not inSingleQuote
            End If
            If s(i) = """"c Then
                If i > 0 Then
                    If s(i - 1) = "\"c Then
                        Continue For
                    End If
                End If
                If inSingleQuote Then
                    Continue For
                End If
                inDoubleQuotes = Not inDoubleQuotes
            End If
        Next
        Return (Not inSingleQuote) AndAlso (Not inDoubleQuotes)
    End Function

    Private Shared Function splitOnFirst(statement As String, <Out> ByRef rest As String, sep As Char) As String
        Dim split = statement.Split({sep}, 2)
        If split.Length = 1 Then
            rest = String.Empty
        Else
            rest = split(1)
        End If
        Return split(0)
    End Function

    Public Shared Function IndexOfAll(sourceString As String, matchString As String) As Integer()
        matchString = Regex.Escape(matchString)
        Dim res = (From match As Match In Regex.Matches(sourceString, matchString) Select match.Index)
        Return res.ToArray()
    End Function

    Private Shared Function splitOnStatementSeparators(line As String, ByRef theRest As String) As String
        Throw New NotImplementedException()
    End Function

    Private Function Parse(statement As String, incompleteStatement As StringBuilder) As SymbolicExpression
        incompleteStatement.Append(statement)
        Dim s = GetFunction(Of Rf_mkString)()(InternalString.NativeUtf8FromString(incompleteStatement.ToString()))
        Dim errorStatement As String
        Using New ProtectedPointer(Me, s)
            Dim status As ParseStatus
            Dim vector = New ExpressionVector(Me, GetFunction(Of R_ParseVector)()(s, -1, status, NilValue.DangerousGetHandle()))

            Select Case status
                Case ParseStatus.OK
                    incompleteStatement.Clear()
                    If vector.Length = 0 Then
                        Return Nothing
                    End If
                    Using New ProtectedPointer(vector)
                        Dim result As SymbolicExpression = Nothing
                        If Not vector.First().TryEvaluate(GlobalEnvironment, result) Then
                            Throw New EvaluationException(LastErrorMessage)
                        End If

                        If AutoPrint AndAlso Not result.IsInvalid AndAlso GetVisible() Then
                            GetFunction(Of Rf_PrintValue)()(result.DangerousGetHandle())
                        End If
                        Return result
                    End Using
                Case ParseStatus.Incomplete
                    Return Nothing

                Case ParseStatus.[Error]
                    ' TODO: use LastErrorMessage if below is just a subset
                    Dim parseErrorMsg = Me.GetAnsiString("R_ParseErrorMsg")
                    errorStatement = incompleteStatement.ToString()
                    incompleteStatement.Clear()
                    Throw New ParseException(status, errorStatement, parseErrorMsg)
                Case Else
                    errorStatement = incompleteStatement.ToString()
                    incompleteStatement.Clear()
                    Throw New ParseException(status, errorStatement, "")
            End Select
        End Using
    End Function

    ''' <summary>
    ''' Gets or sets a value indicating whether this <see cref="RDotNet.REngine"/> auto print R evaluation results, if they are visible.
    ''' </summary>
    ''' <value><c>true</c> if auto print; otherwise, <c>false</c>.</value>
    Public Property AutoPrint() As Boolean

    Private Function GetVisible() As Boolean
        Dim symbol = DangerousGetHandle("R_Visible")
        ' If the R_Visible symbol is not exported by the current R engine (happens on R-2.14.1), then just return 'true'
        ' https://github.com/BlueMountainCapital/FSharpRProvider/pull/152
        If symbol = CType(0, System.IntPtr) Then
            Return True
        Else
            Dim value = Marshal.ReadInt32(symbol)
            Dim result = Convert.ToBoolean(value)
            Return result
        End If
    End Function

    ''' <summary>
    ''' A cache of the unevaluated R expression 'geterrmessage'
    ''' </summary>
    ''' <remarks>do_geterrmessage is in Rdll.hide, so we cannot access at the C API level.
    ''' We use the 'geterrmessage()' R evaluation, but not using the same mechanism as other REngine evaluation
    ''' to avoid recursions issues</remarks>
    Private geterrmessage As Expression = Nothing

    ''' <summary>
    ''' Gets the last error message in the R engine; see R function geterrmessage.
    ''' </summary>
    Friend ReadOnly Property LastErrorMessage() As String
        Get
            If geterrmessage Is Nothing Then
                Dim statement = "geterrmessage()" & vbLf
                Dim s = GetFunction(Of Rf_mkString)()(InternalString.NativeUtf8FromString(statement))
                Dim status As ParseStatus
                Dim vector = New ExpressionVector(Me, GetFunction(Of R_ParseVector)()(s, -1, status, NilValue.DangerousGetHandle()))
                If status <> ParseStatus.OK Then
                    Throw New ParseException(status, statement, "")
                End If
                If vector.Length = 0 Then
                    Throw New ParseException(status, statement, "Failed to create expression vector!")
                End If
                geterrmessage = vector.First()
            End If
            Dim result As SymbolicExpression = Nothing
            If geterrmessage.TryEvaluate(GlobalEnvironment, result) Then
                Dim msgs = SymbolicExpressionExtension.AsCharacter(result).ToArray()
                If msgs.Length > 1 Then
                    Throw New Exception("Unexpected multiple error messages returned")
                End If
                If msgs.Length = 0 Then
                    Throw New Exception("No error messages returned (zero length)")
                End If
                Return msgs(0)
            Else
                Throw New EvaluationException("Unable to retrieve an R error message. Evaluating 'geterrmessage()' fails. The R engine is not in a working state.")
            End If
        End Get
    End Property

    ''' <summary>
    ''' Sets the command line arguments.
    ''' </summary>
    ''' <param name="args">The arguments.</param>
    Public Sub SetCommandLineArguments(args As String())
        CheckEngineIsRunning()
        Dim newArgs = ArrayConverter.Prepend(ID, args)
        GetFunction(Of R_set_command_line_arguments)()(newArgs.Length, newArgs)
    End Sub

    ''' <summary>
    ''' Event triggered when disposing of this REngine
    ''' </summary>
    Public Event Disposing As EventHandler

    ''' <summary>
    ''' Called on disposing of this REngine
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overridable Sub OnDisposing(e As EventArgs)
        RaiseEvent Disposing(Me, e)
    End Sub

    ''' <summary>
    ''' Gets whether this object has been disposed of already.
    ''' </summary>
    Public Property Disposed() As Boolean
        Get
            Return m_Disposed
        End Get
        Private Set
            m_Disposed = Value
        End Set
    End Property
    Private m_Disposed As Boolean

    ''' <summary>
    ''' Dispose of this REngine, including using the native R API to clean up, if the parameter is true
    ''' </summary>
    ''' <param name="disposing">if true, release native resources, using the native R API to clean up.</param>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Me.m_isRunning = False
        OnDisposing(EventArgs.Empty)
        If disposing AndAlso Not Disposed Then
            GetFunction(Of R_RunExitFinalizers)()()
            GetFunction(Of Rf_CleanEd)()()
            GetFunction(Of R_CleanTempDir)()()
            Disposed = True
        End If

        If disposing AndAlso Me.adapter IsNot Nothing Then
            Me.adapter.Dispose()
            Me.adapter = Nothing
        End If
        If Disposed Then
            Return
        End If
        GC.KeepAlive(Me.parameter)
        MyBase.Dispose(disposing)
    End Sub

    ''' <summary>
    ''' Gets the predefined symbol with the specified name.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <returns>The symbol.</returns>
    Public Function GetPredefinedSymbol(name As String) As SymbolicExpression
        CheckEngineIsRunning()
        Try
            Dim pointer = DangerousGetHandle(name)
            Return New SymbolicExpression(Me, Marshal.ReadIntPtr(pointer))
        Catch ex As Exception
            Throw New ArgumentException(Nothing, ex)
        End Try
    End Function

    ''' <summary>
    ''' Create a SymbolicExpression wrapping an existing native R symbolic expression
    ''' </summary>
    ''' <param name="sexp">A pointer to the R symbolic expression</param>
    ''' <returns></returns>
    Public Function CreateFromNativeSexp(sexp As IntPtr) As SymbolicExpression
        Return New SymbolicExpression(Me, sexp)
    End Function

#Region "Nested type: _getDLLVersion"

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Private Delegate Function _getDLLVersion() As IntPtr

#End Region

    ''' <summary>
    ''' Removes variables from the R global environment, and whether garbage collections should be forced
    ''' </summary>
    ''' <param name="garbageCollectR">if true (default) request an R garbage collection. This happens after the .NET garbage collection if both requested</param>
    ''' <param name="garbageCollectDotNet">If true (default), triggers CLR garbage collection and wait for pending finalizers.</param>
    ''' <param name="removeHiddenRVars">Should hidden variables (starting with '.', such as '.Random.seed') be removed. Default is false.</param>
    ''' <param name="detachPackages">If true, detach some packages and other attached resources. Default is false. See 'detach' function in R</param>
    ''' <param name="toDetach">names of resources to dettach, e.g. an array of names such as 'mpg', 'package:lattice'.
    ''' If null, entries found in 'search()' between the first item and 'package:base' are detached. See 'search' function documentation in R</param>
    Public Sub ClearGlobalEnvironment(Optional garbageCollectR As Boolean = True, Optional garbageCollectDotNet As Boolean = True, Optional removeHiddenRVars As Boolean = False, Optional detachPackages As Boolean = False, Optional toDetach As String() = Nothing)
        If detachPackages Then
            doDetachPackages(toDetach)
        End If
        Dim rmStatement = If(removeHiddenRVars, "rm(list=ls(all.names=TRUE))", "rm(list=ls())")
        Me.Evaluate(rmStatement)
        If garbageCollectDotNet Then
            DoDotNetGarbageCollection()
            DoDotNetGarbageCollection()
        End If
        If garbageCollectR Then
            ForceGarbageCollection()
        End If
    End Sub

    Private Sub doDetachPackages(toDetach As String())
        If toDetach Is Nothing Then
            toDetach = Evaluate("search()[2:(which(search()=='package:stats')-1)]").AsCharacter().ToArray()
        End If
        For Each dbName In toDetach
            Evaluate("detach('" & dbName & "')")
        Next
    End Sub

    ''' <summary>
    ''' Triggers a .NET garbage collection. May be useful in some testing circumstance, but users should avoid using this.
    ''' </summary>
    Public Shared Sub DoDotNetGarbageCollection()
        GC.Collect()
        GC.WaitForPendingFinalizers()
    End Sub

    Private stringNAPointer As IntPtr = IntPtr.Zero

    ''' <summary>
    ''' Native pointer to the SEXP representing NA for strings (character vectors in R terminology).
    ''' </summary>
    Public ReadOnly Property NaStringPointer() As IntPtr
        Get
            If stringNAPointer = IntPtr.Zero Then
                stringNAPointer = NaString.DangerousGetHandle()
            End If
            Return stringNAPointer
        End Get
    End Property

    Private stringNaSexp As SymbolicExpression = Nothing

    ''' <summary>
    ''' SEXP representing NA for strings (character vectors in R terminology).
    ''' </summary>
    Public ReadOnly Property NaString() As SymbolicExpression
        Get
            If stringNaSexp Is Nothing Then
                stringNaSexp = Me.GetPredefinedSymbol("R_NaString")
            End If
            Return stringNaSexp
        End Get
    End Property
End Class

