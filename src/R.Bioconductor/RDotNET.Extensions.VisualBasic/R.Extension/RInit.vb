Imports System.IO

Public Module RInit

    ''' <summary>
    ''' Automatically search for the path of the R system and then construct a R session for you.
    ''' (如果在注册表之中已经存在了R的路径的值或者你已经设置好了环境变量，则可以直接使用本函数进行初始化操作)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartEngineServices() As REngine
        Dim Directories As String() = ProgramPathSearchTool.SearchDirectory("R", "")
        If Directories.IsNullOrEmpty Then
            Throw New Exception(INIT_FAILURE)
        End If

        Dim R_HOME As String = ""

        For Each Direactory As String In Directories
            Dim Files As String() = ProgramPathSearchTool.SearchProgram(Direactory, "R")
            If Not Files.IsNullOrEmpty Then
                R_HOME = Files.First
                Exit For
            End If
        Next

        If String.IsNullOrEmpty(R_HOME) Then
            Throw New Exception(INIT_FAILURE)
        Else
            R_HOME = R_HOME.ParentPath
        End If

        Return RInit.StartEngineServices(R_HOME:=R_HOME)
    End Function

    Const INIT_FAILURE As String = "Could not initialize the R session automatically!"
    Const R_HOME_NOT_FOUND As String = "Could not found the specified path to the directory containing R.dll: "

    ''' <summary>
    ''' Manual setup the R system path.(这个函数是在没有自动设置好环境变量的时候进行手动的环境变量设置所使用的初始化方法)
    ''' </summary>
    ''' <param name="R_HOME"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartEngineServices(R_HOME As String) As REngine
        Dim oldPath As String = Environment.GetEnvironmentVariable("PATH")
        Dim rPath As String = IIf(Environment.Is64BitProcess,
                                  $"{R_HOME}/x64",
                                  $"{R_HOME}/i386")

        If Directory.Exists(rPath) = False Then
            Throw New DirectoryNotFoundException(R_HOME_NOT_FOUND & " ---> """ & rPath & """")
        End If

        Dim newPath = String.Format("{0}{1}{2}", rPath, Path.PathSeparator, oldPath)
        Dim rHome As String = ""

        Select Case (Environment.OSVersion.Platform)
            Case PlatformID.Win32NT
            Case PlatformID.MacOSX : rHome = "/Library/Frameworks/R.framework/Resources"
            Case PlatformID.Unix : rHome = "/usr/lib/R"
            Case Else
                Throw New NotSupportedException($"No support such platform: {Environment.OSVersion.Platform.ToString}")
        End Select

        Call Environment.SetEnvironmentVariable("PATH", newPath)

        If Not String.IsNullOrEmpty(rHome) Then
            Call Environment.SetEnvironmentVariable("R_HOME", rHome)
        End If

        Dim REngine As REngine = REngine.CreateInstance("RDotNet")
        Call REngine.Initialize()

        Return REngine
    End Function
End Module
