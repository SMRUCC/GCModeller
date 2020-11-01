Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
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
''' 	NumericVector random = engine.Evaluate("rnorm(5, 0, 1)").AsNumeric();
''' 	foreach (double r in random)
''' 	{
''' 		Console.Write(r + " ");
''' 	}
''' }
''' </code>
''' </example>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class REngine
    Inherits DynamicInterop.UnmanagedDll

    ''' <summary>
    ''' Gets the R compatibility mode, based on the version of R used.
    ''' </summary>
    Dim _Compatibility As RDotNet.REngine.CompatibilityMode

    ''' <summary>
    ''' Gets whether this object has been disposed of already.
    ''' </summary>
    Dim _Disposed As Boolean

    ''' <summary>
    ''' Flag for working on pre or post R 3.5 and its ALTREP mode.  
    ''' </summary>
    Public Enum CompatibilityMode
        ''' <summary>
        ''' Pre ALTREP includes all versions before R 3.5.  This uses a 32-bit sxpinfo structure.
        ''' </summary>
        PreALTREP = 0

        ''' <summary>
        ''' ALTREP includes all versions R 3.5 and above.  Core header structures were introduced in R 3.5 with
        ''' the ALTREP feature which required introducing the compability mode.  It uses a 64-bit sxpinfo structure.
        ''' </summary>
        ALTREP = 1
    End Enum

    Private Shared ReadOnly DefaultDevice As ICharacterDevice = New ConsoleDevice()
    Private ReadOnly idField As String
    Private adapter As CharacterDeviceAdapter
    Private isRunningField As Boolean
    Private parameter As StartupParameter
    Private Shared environmentIsSet As Boolean = False
    Private Shared nativeUtil As NativeUtility = Nothing
    Private Shared engine As REngine = Nothing

    ' Type cache to allow faster dynamic casting
    Private Shared sexprecType As Type = Nothing
    Private Shared symsxpType As Type = Nothing
    Private Shared vectorSexprecType As Type = Nothing
    Private Shared ReadOnly RDllVersionDelimiter As Char() = {"."c}

    ''' <summary>
    ''' Create a new REngine instance
    ''' </summary>
    ''' <param name="id">The identifier of this object</param>
    ''' <param name="dll">The name of the file that is the shared R library, e.g. "R.dll"</param>
    Protected Sub New(ByVal id As String, ByVal dll As String)
        MyBase.New(dll)
        idField = id
        isRunningField = False
        Disposed = False
        EnableLock = True ' See https://rdotnet.codeplex.com/workitem/113; it seems wise to enable it by default.
        AutoPrint = False  ' 2019-05 changing to false by default, as this impacts the default performance drastically. There was an argument for a true default, but now I things this is superseded.
    End Sub

    ''' <summary>
    ''' Gets/sets whether the call to Preserve and Unpreserve on symbolic expressions
    ''' should be using a lock to prevent thread concurrency issues. Default is false;
    ''' </summary>
    ''' <remarks>Thanks to gchapman for proposing the fix. See https://rdotnet.codeplex.com/workitem/67 for details</remarks>
    Public Property EnableLock As Boolean

    ''' <summary>
    ''' Gets whether this instance is running.
    ''' </summary>
    Public ReadOnly Property IsRunning As Boolean
        Get
            Return isRunningField
        End Get
    End Property

    ''' <summary>
    ''' Gets the version of R.DLL.
    ''' </summary>
    Public ReadOnly Property DllVersion As String
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
    Public ReadOnly Property ID As String
        Get
            Return idField
        End Get
    End Property

    Public Property Compatibility As CompatibilityMode
        Get
            Return _Compatibility
        End Get
        Private Set(ByVal value As CompatibilityMode)
            _Compatibility = value
        End Set
    End Property


    ''' <summary>
    ''' Gets the global environment.
    ''' </summary>
    Public ReadOnly Property GlobalEnvironment As REnvironment
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
    Public ReadOnly Property EmptyEnvironment As REnvironment
        Get
            CheckEngineIsRunning()
            Return GetPredefinedSymbol("R_EmptyEnv").AsEnvironment()
        End Get
    End Property

    ''' <summary>
    ''' Gets the base environment.
    ''' </summary>
    Public ReadOnly Property BaseNamespace As REnvironment
        Get
            CheckEngineIsRunning()
            Return GetPredefinedSymbol("R_BaseNamespace").AsEnvironment()
        End Get
    End Property

    ''' <summary>
    ''' Gets the <c>NULL</c> value.
    ''' </summary>
    Public ReadOnly Property NilValue As SymbolicExpression
        Get
            CheckEngineIsRunning()
            Return GetPredefinedSymbol("R_NilValue")
        End Get
    End Property

    ''' <summary>
    ''' Gets the unbound value.
    ''' </summary>
    Public ReadOnly Property UnboundValue As SymbolicExpression
        Get
            CheckEngineIsRunning()
            Return GetPredefinedSymbol("R_UnboundValue")
        End Get
    End Property

    ''' <summary>
    ''' Gets the name of the R engine instance (singleton).
    ''' </summary>
    Public Shared ReadOnly Property EngineName As String
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
    Public Shared Function GetInstance(ByVal Optional dll As String = Nothing, ByVal Optional initialize As Boolean = True, ByVal Optional parameter As StartupParameter = Nothing, ByVal Optional device As ICharacterDevice = Nothing) As REngine
        If Not environmentIsSet Then SetEnvironmentVariables() ' should there be a warning? and how?

        If engine Is Nothing Then
            engine = CreateInstance(EngineName, dll)
            If initialize Then engine.Initialize(parameter, device)
        End If

        If engine.Disposed Then Throw New InvalidOperationException("The single REngine instance has already been disposed of (i.e. shut down). Multiple engine restart is not possible.")
        Return engine
    End Function

    ''' <summary>
    ''' Creates a new instance that handles R.DLL.
    ''' </summary>
    ''' <param name="id">ID.</param>
    ''' <param name="dll">The file name of the library to load, e.g. "R.dll" for Windows. You should usually not provide this optional parameter</param>
    ''' <returns>The engine.</returns>
    Private Shared Function CreateInstance(ByVal id As String, ByVal Optional dll As String = Nothing) As REngine
        If Equals(id, Nothing) Then
            Throw New ArgumentNullException("id", "Empty ID is not allowed.")
        End If

        If Equals(id, String.Empty) Then
            Throw New ArgumentException("Empty ID is not allowed.", "id")
        End If

        dll = ProcessRDllFileName(dll)
        Dim engine = New REngine(id, dll)
        DetermineCompatibility(engine)
        Return engine
    End Function

    Private Shared Sub DetermineCompatibility(ByVal engine As REngine)
        If engine Is Nothing Then
            Return
        End If

        ' If there is no DLL version information, we are going to start with an arbitrary default
        ' compatibility version to support R 3.5+
        engine.Compatibility = CompatibilityMode.ALTREP
        ' engine.DllVersion is not implemented because the R native library has no entry point to getDllVersion which is Windows only. 
        ' Not sure yet if there is a way to programatically query the R version on Linux, without bumping in a chicken and egg problem.
        If NativeUtility.IsUnix Then Return

        If String.IsNullOrWhiteSpace(engine.DllVersion) Then
            Return
        End If

        Dim versionParts = engine.DllVersion.Split(RDllVersionDelimiter)

        If versionParts.Length < 2 Then
            Return
        End If

        Dim major = 0
        Dim minor = 0

        If Integer.TryParse(versionParts(0), major) AndAlso Integer.TryParse(versionParts(1), minor) Then
            ' Pre-ALTREP is <= 3.4
            If major <= 3 AndAlso minor <= 4 Then
                engine.Compatibility = CompatibilityMode.PreALTREP
            Else
                engine.Compatibility = CompatibilityMode.ALTREP
            End If
        End If
    End Sub
    ''' <summary>
    ''' Gets the type of SEXPREC pre or post ALTREP
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSEXPRECType() As Type
        If sexprecType Is Nothing Then
            Select Case Compatibility
                Case CompatibilityMode.ALTREP
                    sexprecType = GetType(ALTREP.SEXPREC)
                Case CompatibilityMode.PreALTREP
                    sexprecType = GetType(PreALTREP.SEXPREC)
                Case Else
                    Throw New InvalidCastException("No SEXPREC type is available for this compatibility level")
            End Select
        End If

        Return sexprecType
    End Function

    ''' <summary>
    ''' Gets the type of symsxp pre or post ALTREP
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSymSxpType() As Type
        If symsxpType Is Nothing Then
            Select Case Compatibility
                Case CompatibilityMode.ALTREP
                    symsxpType = GetType(ALTREP.symsxp)
                Case CompatibilityMode.PreALTREP
                    symsxpType = GetType(PreALTREP.symsxp)
                Case Else
                    Throw New InvalidCastException("No symsxp type is available for this compatibility level")
            End Select
        End If

        Return symsxpType
    End Function

    ''' <summary>
    ''' Gets the type of VECTOR_SEXPREC pre or post ALTREP
    ''' </summary>
    ''' <returns></returns>
    Public Function GetVectorSexprecType() As Type
        If vectorSexprecType Is Nothing Then
            Select Case Compatibility
                Case CompatibilityMode.ALTREP
                    vectorSexprecType = GetType(ALTREP.VECTOR_SEXPREC)
                Case CompatibilityMode.PreALTREP
                    vectorSexprecType = GetType(PreALTREP.VECTOR_SEXPREC)
                Case Else
                    Throw New InvalidCastException("No symsxp type is available for this compatibility level")
            End Select
        End If

        Return vectorSexprecType
    End Function

    ''' <summary>
    ''' if the parameter is null or empty string, return the default names of the R shared library file depending on the platform
    ''' </summary>
    ''' <param name="dll">The name of the library provided, possibly null or empty</param>
    ''' <returns>A candidate for the file name of the R shared library</returns>
    Protected Shared Function ProcessRDllFileName(ByVal dll As String) As String
        If Not String.IsNullOrEmpty(dll) Then Return dll
        Return NativeUtility.GetRLibraryFileName()
    End Function

    Private Shared Function EncodeNonAsciiCharacters(ByVal value As String) As String
        Dim sb As StringBuilder = New StringBuilder()

        For Each C As Char In value

            If AscW(C) > 127 Then
                Dim encodedValue As String = "\u" & AscW(C).ToString("x4")
                sb.Append(encodedValue)
            Else
                sb.Append(C)
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
    Public Shared Sub SetEnvironmentVariables(ByVal Optional rPath As String = Nothing, ByVal Optional rHome As String = Nothing)
        environmentIsSet = True
        nativeUtil = New NativeUtility()
        nativeUtil.SetEnvironmentVariables(rPath:=rPath, rHome:=rHome)
    End Sub

    Private Shared Sub resetCachedEnvironmentVariables()
        If environmentIsSet <> True Then Throw New Exception("resetCachedEnvironmentVariables cannot be called if the R environment variables were not first set")
        nativeUtil.SetCachedEnvironmentVariables()
    End Sub

    ''' <summary>
    ''' Initialize this REngine object. Only the first call has an effect. Subsequent calls to this function are ignored.
    ''' </summary>
    ''' <param name="parameter">The optional startup parameters</param>
    ''' <param name="device">The optional character device to use for the R engine</param>
    ''' <param name="setupMainLoop">if true, call the functions to initialise the embedded R</param>
    Public Sub Initialize(ByVal Optional parameter As StartupParameter = Nothing, ByVal Optional device As ICharacterDevice = Nothing, ByVal Optional setupMainLoop As Boolean = True)
        '         Console.WriteLine("REngine.Initialize start");
        If isRunningField Then Return
        '         Console.WriteLine("REngine.Initialize, after isRunning checked as false");
        Me.parameter = If(parameter, DefaultStartupParameter())
        adapter = New CharacterDeviceAdapter(If(device, DefaultDevice))
        ' Disabling the stack checking here, to try to avoid the issue on Linux.
        ' The disabling used to be here around end Nov 2013. Was moved later in this
        ' function to cater for disabling on Windows, @ rev 305, however this may have
        ' re-broken on Linux. so we may need to call it twice.
        SetCstackChecking()
        '         Console.WriteLine("Initialize-SetCstackChecking; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

        If Not setupMainLoop Then
            isRunningField = True
            Return
        End If

        Dim R_argv = BuildRArgv(Me.parameter)
        'string[] R_argv = new[]{"rdotnet_app",  "--interactive",  "--no-save",  "--no-restore-data",  "--max-ppsize=50000"};
        'rdotnet_app --quiet --interactive --no-save --no-restore-data --max-mem-size=18446744073709551615 --max-ppsize=50000
        GetFunction(Of R_setStartTime)()()
        Dim R_argc = R_argv.Length
        '         Console.WriteLine("Initialize-R_setStartTime; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

        If NativeUtility.GetPlatform() = PlatformID.Win32NT Then
            ' Attempted Fix for https://rdotnet.codeplex.com/workitem/110; not working
            ' Tried to make sure that the command line options are taken into account. They are NOT effectively so via Rf_initialize_R only.
            ' The problem is that cmdlineoptions assumes it is called by RGui.exe or RTerm.exe, and overrides R_HOME

            '   GetFunction<R_set_command_line_arguments>()(R_argc, R_argv);
            '   GetFunction<cmdlineoptions>()(R_argc, R_argv);
        End If

        Dim status = GetFunction(Of Rf_initialize_R)()(R_argc, R_argv)
        If status <> 0 Then Throw New Exception("A call to Rf_initialize_R returned a non-zero; status=" & status)
        ' also workaround for https://github.com/rdotnet/rdotnet/issues/127  : R.dll is intent on overriding R_HOME and PATH even if --no-environ is specified...
        If NativeUtility.GetPlatform() = PlatformID.Win32NT Then
            Call resetCachedEnvironmentVariables()
        End If
        '         Console.WriteLine("Initialize-Rf_initialize_R; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));
        SetCstackChecking()

        ' following in RInside: may not be needed.
        'GetFunction<R_ReplDLLinit> () ();
        'this.parameter.Interactive = true;
        adapter.Install(Me, Me.parameter)
        'Console.WriteLine("Initialize-adapter installation; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));
        Select Case NativeUtility.GetPlatform()
            Case PlatformID.Win32NT
                GetFunction(Of R_SetParams_Windows)("R_SetParams")(Me.parameter.start)
                ' also workaround for https://github.com/rdotnet/rdotnet/issues/127  : R.dll is intent on overriding R_HOME and PATH even if --no-environ is specified...
                resetCachedEnvironmentVariables()
            Case PlatformID.MacOSX, PlatformID.Unix
                'Console.WriteLine("Initialize-R_SetParams_Unix; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));
                GetFunction(Of R_SetParams_Unix)("R_SetParams")(Me.parameter.start.Common)
        End Select

        GetFunction(Of setup_Rmainloop)()()
        'Console.WriteLine("Initialize-after setup_Rmainloop; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

        ' See comments in the first call to SetCstackChecking in this function as to why we (may) need it twice.
        SetCstackChecking()
        isRunningField = True

        'Console.WriteLine("Initialize-just before leaving; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

        If NativeUtility.GetPlatform() = PlatformID.Win32NT Then
            ' also workaround for https://github.com/rdotnet/rdotnet/issues/127  : R.dll is intent on overriding R_HOME and PATH even if --no-environ is specified...
            resetCachedEnvironmentVariables()
            ' Partial Workaround (hopefully temporary) for https://rdotnet.codeplex.com/workitem/110
            ' Evaluate($"invisible(memory.limit({Me.parameter.MaxMemorySize / 1048576UL}))")
        End If
    End Sub

    Private Function DefaultStartupParameter() As StartupParameter
        Dim p = New StartupParameter()
        ' to avoid https://github.com/rdotnet/rdotnet/issues/127 ?
        p.NoRenviron = True
        p.LoadInitFile = False
        p.LoadSiteFile = False
        Return p
    End Function

    Private Shared Sub currentEnvVars(<Out> ByRef path As String, <Out> ByRef rhome As String)
        path = Environment.GetEnvironmentVariable("PATH")
        rhome = Environment.GetEnvironmentVariable("R_HOME")
    End Sub

    Private Sub SetCstackChecking()
        ' Don't do any stack checking, see R Exts, '8.1.5 Threading issues',
        ' https://rdotnet.codeplex.com/discussions/462947
        ' https://rdotnet.codeplex.com/workitem/115
        WriteInt32("R_CStackLimit", -1)

        Select Case NativeUtility.GetPlatform()
            Case PlatformID.MacOSX, PlatformID.Unix
                ' RInside does this for non-WIN32.
                WriteInt32("R_SignalHandlers", 0)
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
    Public Shared Function BuildRArgv(ByVal parameter As StartupParameter) As String()
        Dim platform = NativeUtility.GetPlatform()
        Dim argv = New List(Of String)()
        argv.Add("rdotnet_app")
        ' Not sure whether I should add no-readline
        '[MarshalAs(UnmanagedType.Bool)]
        'public bool R_Quiet;
        If parameter.Quiet AndAlso Not parameter.Interactive Then argv.Add("--quiet")  ' --quite --interactive to R embedded crashed...

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool R_Slave;
        If parameter.Slave Then argv.Add("--slave")

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool R_Interactive;
        If platform <> PlatformID.Win32NT Then ' RTerm.exe --help shows no such option; Unix only.
            If parameter.Interactive Then argv.Add("--interactive")
        End If

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool R_Verbose;
        If parameter.Verbose Then argv.Add("--verbose")

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool LoadSiteFile;
        If Not parameter.LoadSiteFile Then argv.Add("--no-site-file")

        '[MarshalAs(UnmanagedType.Bool)]
        'public bool LoadInitFile;
        If Not parameter.LoadInitFile Then argv.Add("--no-init-file")

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
        If parameter.NoRenviron Then argv.Add("--no-environ")

        Select Case parameter.SaveAction
            Case StartupSaveAction.NoSave
                argv.Add("--no-save")
            Case StartupSaveAction.Save
                argv.Add("--save")
        End Select

        Select Case parameter.RestoreAction
            Case StartupRestoreAction.NoRestore
                argv.Add("--no-restore-data")
            Case StartupRestoreAction.Restore
                argv.Add("--restore")
        End Select
        ' This creates a nasty crash if using the default MaxMemorySize. found out in Rdotnet workitem 72
        ' do nothing
        If parameter.MaxMemorySize = If(Environment.Is64BitProcess, ULong.MaxValue, UInteger.MaxValue) Then
        Else
            If platform = PlatformID.Win32NT Then argv.Add("--max-mem-size=" & parameter.MaxMemorySize) ' On unix, otherwise led to https://rdotnet.codeplex.com/workitem/137
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
    Public Function GetSymbol(ByVal name As String) As SymbolicExpression
        CheckEngineIsRunning()
        Return GlobalEnvironment.GetSymbol(name)
    End Function

    ''' <summary>
    ''' Gets a symbol defined in the global environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <param name="environment">The environment. If <c>null</c> is passed, <see cref="GlobalEnvironment"/> is used.</param>
    ''' <returns>The symbol.</returns>
    Public Function GetSymbol(ByVal name As String, ByVal environment As REnvironment) As SymbolicExpression
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
    Public Sub SetSymbol(ByVal name As String, ByVal expression As SymbolicExpression)
        CheckEngineIsRunning()
        GlobalEnvironment.SetSymbol(name, expression)
    End Sub

    ''' <summary>
    ''' Assign a value to a name in a specific environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <param name="expression">The symbol.</param>
    ''' <param name="environment">The environment. If <c>null</c> is passed, <see cref="GlobalEnvironment"/> is used.</param>
    Public Sub SetSymbol(ByVal name As String, ByVal expression As SymbolicExpression, ByVal environment As REnvironment)
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
    ''' <param name="environment">The environment in which to evaluate the statement. Advanced feature.</param>
    ''' <returns>Last evaluation.</returns>
    Public Overridable Function Evaluate(ByVal statement As String, ByVal Optional environment As REnvironment = Nothing) As SymbolicExpression
        CheckEngineIsRunning()
        Return Defer(EncodeNonAsciiCharacters(statement), environment).LastOrDefault()
    End Function

    ''' <summary>
    ''' Evaluates a statement in the given stream.
    ''' </summary>
    ''' <param name="stream">The stream.</param>
    ''' <param name="environment">The environment in which to evaluate the statement. Advanced feature.</param>
    ''' <returns>Last evaluation.</returns>
    Public Function Evaluate(ByVal stream As Stream, ByVal Optional environment As REnvironment = Nothing) As SymbolicExpression
        CheckEngineIsRunning()
        Return Defer(stream, environment).LastOrDefault()
    End Function

    ''' <summary>
    ''' Evaluates a statement in the given string.
    ''' </summary>
    ''' <param name="statement">The statement.</param>
    ''' <param name="environment">The environment in which to evaluate the statement. Advanced feature.</param>
    ''' <returns>Each evaluation.</returns>
    Private Iterator Function Defer(ByVal statement As String, ByVal Optional environment As REnvironment = Nothing) As IEnumerable(Of SymbolicExpression)
        CheckEngineIsRunning()

        If Equals(statement, Nothing) Then
            Throw New ArgumentNullException()
        End If

        Using reader As TextReader = New StringReader(statement)
            Dim incompleteStatement = New StringBuilder()
            Dim line As Value(Of String) = ""

            While Not (line = reader.ReadLine()) Is Nothing
                Dim lines As String() = REngine.Segment(line).ToArray

                For Each segment As String In lines
                    Dim result = Me.Parse(segment, incompleteStatement, environment)

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
    ''' <param name="environment">The environment in which to evaluate the statement. Advanced feature.</param>
    ''' <returns>Each evaluation.</returns>
    Public Iterator Function Defer(ByVal stream As Stream, ByVal Optional environment As REnvironment = Nothing) As IEnumerable(Of SymbolicExpression)
        CheckEngineIsRunning()

        If stream Is Nothing Then
            Throw New ArgumentNullException()
        End If

        If Not stream.CanRead Then
            Throw New ArgumentException()
        End If

        Using reader As TextReader = New StreamReader(stream)
            Dim incompleteStatement = New StringBuilder()
            Dim line As Value(Of String) = Nothing

            While Not (line = reader.ReadLine()) Is Nothing
                Dim lines As String() = REngine.Segment(line).ToArray

                For Each segment As String In lines
                    Dim result = Me.Parse(segment, incompleteStatement, environment)

                    If result IsNot Nothing Then
                        Yield result
                    End If
                Next
            End While
        End Using
    End Function

    Private Shared Iterator Function Segment(ByVal line As String) As IEnumerable(Of String)
        Dim segments = processInputString(line)

        For index = 0 To segments.Length - 1

            If index = segments.Length - 1 Then
                If Not Equals(segments(index), String.Empty) Then
                    Yield segments(index) & vbLf
                End If
            Else
                Yield segments(index) & ";"
            End If
        Next
    End Function

    Private Shared Function processInputString(ByVal input As String) As String()
        ' Fixes for
        ' https://rdotnet.codeplex.com/workitem/165
        ' https://github.com/jmp75/rdotnet/issues/14
        Dim lines As String() = input.LineTokens
        Dim statements As List(Of String) = New List(Of String)()

        For i = 0 To lines.Length - 1
            statements.AddRange(processLine(lines(i)))
        Next

        Return statements.ToArray()
    End Function

    Private Shared Function processLine(ByVal line As String) As String()
        Dim trimmedLine = line.Trim()
        If Equals(trimmedLine, String.Empty) Then Return New String() {}
        If trimmedLine.StartsWith("#") Then Return New String() {line}
        Dim theRest As String = Nothing
        Dim statement = splitOnFirst(line, theRest, ";"c)
        Dim result = New List(Of String)()

        If Not statement.Contains("#") Then
            result.Add(statement)
            result.AddRange(processLine(theRest))
        Else
            ' paste('this contains ### characters', " this too ###", 'Oh, and this # one too') # but "this" 'rest' is commented
            ' Find the fist # character such that before that, there is an 
            ' even number of " and an even number of ' characters

            Dim whereHash = IndexOfAll(statement, "#")
            Dim firstComment = EvenStringDelimitors(statement, whereHash)
            ' incomplete statement??? such as:
            ' paste('this is the # ', ' start of an incomplete # statement
            If firstComment < 0 Then
                result.Add(statement)
                result.AddRange(processLine(theRest))
            Else
                result.Add(statement.Substring(0, firstComment))
                ' firstComment is a valid comment marker - not need to process "the rest"
            End If

            Dim restFirstStatement As String = Nothing
            Dim beforeComment = splitOnFirst(statement, restFirstStatement, "#"c)
        End If

        Return result.ToArray()
    End Function

    Private Shared Function EvenStringDelimitors(ByVal statement As String, ByVal whereHash As Integer()) As Integer
        For i = 0 To whereHash.Length - 1
            Dim s = statement.Substring(0, whereHash(i))
            If IsClosedString(s) Then Return whereHash(i)
        Next

        Return -1
    End Function

    Private Shared Function IsClosedString(ByVal s As String) As Boolean
        ' paste("#hashtag")
        ' paste("#hashtag''''")
        ' paste('#hashtag""""')
        ' paste('#hashtag""#""')
        ' paste('#hashtag""#""', "#hash ''' ")
        Dim inSingleQuote = False, inDoubleQuotes = False

        For i = 0 To s.Length - 1

            If s(i) = "'"c Then
                If i > 0 Then
                    If s(i - 1) = "\"c Then Continue For
                End If

                If inDoubleQuotes Then Continue For
                inSingleQuote = Not inSingleQuote
            End If

            If s(i) = """"c Then
                If i > 0 Then
                    If s(i - 1) = "\"c Then Continue For
                End If

                If inSingleQuote Then Continue For
                inDoubleQuotes = Not inDoubleQuotes
            End If
        Next

        Return Not inSingleQuote AndAlso Not inDoubleQuotes
    End Function

    Private Shared Function splitOnFirst(ByVal statement As String, <Out> ByRef rest As String, ByVal sep As Char) As String
        Dim split = statement.Split({sep}, 2)

        If split.Length = 1 Then
            rest = String.Empty
        Else
            rest = split(1)
        End If

        Return split(0)
    End Function

    ''' <summary> Searches for the first all.</summary>
    '''
    ''' <param name="sourceString"> Source string.</param>
    ''' <param name="matchString">  The match string.</param>
    '''
    ''' <returns> The zero-based index of the found all, or -1 if no match was found.</returns>
    Private Shared Function IndexOfAll(ByVal sourceString As String, ByVal matchString As String) As Integer()
        matchString = Regex.Escape(matchString)
        Dim res = From match As Match In Regex.Matches(sourceString, matchString) Select match.Index
        Return res.ToArray()
    End Function

    Private Shared Function splitOnStatementSeparators(ByVal line As String, <Out> ByRef theRest As String) As String
        Throw New NotImplementedException()
    End Function

    Private Function Parse(ByVal statement As String, ByVal incompleteStatement As StringBuilder, ByVal Optional environment As REnvironment = Nothing) As SymbolicExpression
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
                        Dim exp As Expression = Enumerable.First(vector)
                        Dim env As REnvironment = If(environment, GlobalEnvironment)

                        If Not exp.TryEvaluate(env, result) Then
                            Throw New EvaluationException(LastErrorMessage)
                        End If

                        If AutoPrint AndAlso Not result.IsInvalid AndAlso GetVisible() Then
                            GetFunction(Of Rf_PrintValue)()(result.DangerousGetHandle())
                        End If

                        Return result
                    End Using

                Case ParseStatus.Incomplete
                    Return Nothing
                Case ParseStatus.Error
                    ' TODO: use LastErrorMessage if below is just a subset
                    Dim parseErrorMsg = GetAnsiString("R_ParseErrorMsg")
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
    Public Property AutoPrint As Boolean

    Private Function GetVisible() As Boolean
        Dim symbol = DangerousGetHandle("R_Visible")
        ' If the R_Visible symbol is not exported by the current R engine (happens on R-2.14.1), then just return 'true'
        ' https://github.com/BlueMountainCapital/FSharpRProvider/pull/152
        If symbol = CType(0, IntPtr) Then
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
    Friend ReadOnly Property LastErrorMessage As String
        Get

            If geterrmessage Is Nothing Then
                Dim statement = "geterrmessage()" & Microsoft.VisualBasic.Constants.vbLf
                Dim s = GetFunction(Of Rf_mkString)()(InternalString.NativeUtf8FromString(statement))
                Dim status As ParseStatus
                Dim vector = New ExpressionVector(Me, GetFunction(Of R_ParseVector)()(s, -1, status, NilValue.DangerousGetHandle()))
                If status <> ParseStatus.OK Then Throw New ParseException(status, statement, "")
                If vector.Length = 0 Then Throw New ParseException(status, statement, "Failed to create expression vector!")
                geterrmessage = vector.First()
            End If

            Dim result As SymbolicExpression = Nothing

            If geterrmessage.TryEvaluate(GlobalEnvironment, result) Then
                Dim msgs = result.AsCharacter().ToArray()
                If msgs.Length > 1 Then Throw New Exception("Unexpected multiple error messages returned")
                If msgs.Length = 0 Then Throw New Exception("No error messages returned (zero length)")
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
    Public Sub SetCommandLineArguments(ByVal args As String())
        CheckEngineIsRunning()
        Dim newArgs = Prepend(ID, args)
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
    Protected Overridable Sub OnDisposing(ByVal e As EventArgs)
        RaiseEvent Disposing(Me, e)
    End Sub

    Public Property Disposed As Boolean
        Get
            Return _Disposed
        End Get
        Private Set(ByVal value As Boolean)
            _Disposed = value
        End Set
    End Property

    ''' <summary>
    ''' Dispose of this REngine, including using the native R API to clean up, if the parameter is true
    ''' </summary>
    ''' <param name="disposing">if true, release native resources, using the native R API to clean up.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        isRunningField = False
        OnDisposing(EventArgs.Empty)

        If disposing AndAlso Not Disposed Then
            GetFunction(Of R_RunExitFinalizers)()()
            GetFunction(Of Rf_CleanEd)()()
            GetFunction(Of R_CleanTempDir)()()
            Disposed = True
        End If

        If disposing AndAlso adapter IsNot Nothing Then
            adapter.Dispose()
            adapter = Nothing
        End If

        If Disposed Then Return
        GC.KeepAlive(parameter)
        MyBase.Dispose(disposing)
    End Sub

    ''' <summary>
    ''' Gets the predefined symbol with the specified name.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <returns>The symbol.</returns>
    Public Function GetPredefinedSymbol(ByVal name As String) As SymbolicExpression
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
    Public Function CreateFromNativeSexp(ByVal sexp As IntPtr) As SymbolicExpression
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
    Public Sub ClearGlobalEnvironment(ByVal Optional garbageCollectR As Boolean = True, ByVal Optional garbageCollectDotNet As Boolean = True, ByVal Optional removeHiddenRVars As Boolean = False, ByVal Optional detachPackages As Boolean = False, ByVal Optional toDetach As String() = Nothing)
        If detachPackages Then doDetachPackages(toDetach)
        Dim rmStatement = If(removeHiddenRVars, "rm(list=ls(all.names=TRUE))", "rm(list=ls())")
        Evaluate(rmStatement)

        If garbageCollectDotNet Then
            DoDotNetGarbageCollection()
            DoDotNetGarbageCollection()
        End If

        If garbageCollectR Then ForceGarbageCollection()
    End Sub

    Private Sub doDetachPackages(ByVal toDetach As String())
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
    Public ReadOnly Property NaStringPointer As IntPtr
        Get
            If stringNAPointer = IntPtr.Zero Then stringNAPointer = NaString.DangerousGetHandle()
            Return stringNAPointer
        End Get
    End Property

    Private stringNaSexp As SymbolicExpression = Nothing

    ''' <summary>
    ''' SEXP representing NA for strings (character vectors in R terminology).
    ''' </summary>
    Public ReadOnly Property NaString As SymbolicExpression
        Get
            If stringNaSexp Is Nothing Then stringNaSexp = GetPredefinedSymbol("R_NaString")
            Return stringNaSexp
        End Get
    End Property
End Class
