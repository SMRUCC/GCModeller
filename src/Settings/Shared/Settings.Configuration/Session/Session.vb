#Region "Microsoft.VisualBasic::c6b78f18f5991bbf73cecf4cecd5b1cb, Shared\Settings.Configuration\Session\Session.vb"

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

'     Module Session
' 
' 
'     Module Session
' 
'         Properties: DataCache, Initialized, LogDIR, ProfileData, SettingsDIR
'                     SettingsFile, SHA256Provider, TEMP, Templates
' 
'         Function: FolkShoalThread, GetSettings, GetSettingsFile, Initialize, InstallJavaBin
'                   InstallPython, List, Mothur, MothurHome, SetValue
'                   TryGetShoalShellBin
' 
'         Sub: Finallize, Save, saveProfile
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports ProfileEngine = Microsoft.VisualBasic.ComponentModel.Settings

Namespace Settings

    ''' <summary>
    ''' GCModeller program profile session.(GCModeller的应用程序配置会话) 
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
#If ENABLE_API_EXPORT Then
    <Package("GCModeller.Profile.Session",
                    Description:="The application profile data session for the GCModeller application modules.",
                    Publisher:="amethyst.asuka@gcmodeller.org")>
    Module Session
#Else
    Module Session
#End If

        Dim worksapce As String = App.SysTemp & "/GCModeller/" & App.AssemblyName & ".EXE"
        Dim cache As String = App.SysTemp & "/GCModeller/.cache/" & App.AssemblyName & ".EXE"
        Dim initFlag As Boolean
        Dim _profileData As Settings(Of Settings.File)

        Public ReadOnly Property ProfileData As Settings(Of Settings.File)
            Get
                Return Session._profileData
            End Get
        End Property

        Public ReadOnly Property SHA256Provider As SecurityString.SHA256 =
            New SecurityString.SHA256("http://gcmodeller.org/", "GCModeller")

        ''' <summary>
        ''' Temp data directory for this application session.(本应用程序会话的临时数据文件夹)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TEMP As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return worksapce
            End Get
        End Property

        ''' <summary>
        ''' Templates directory of the GCModeller inputs data.
        ''' (在这个文件夹里面存放的是一些分析工具的输入的数据的模板文件)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Templates As String = App.HOME & "/Templates/"

        Public ReadOnly Property SettingsFile As Settings.File
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return ProfileData.SettingsData
            End Get
        End Property

        ''' <summary>
        ''' this function will do <see cref="Initialize"/> automatically if 
        ''' the settings session has not been <see cref="Initialized"/>.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetSettingsFile() As Settings.File
            If Not Initialized Then
                Call Initialize()
            End If

            Return Session.SettingsFile
        End Function

        ''' <summary>
        ''' The cache data directory for this application session.(本应用程序的数据缓存文件夹)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DataCache As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return cache
            End Get
        End Property

        ''' <summary>
        ''' This property indicates that whether this application session is initialized or not.(应用程序是否已经初始化完毕)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Initialized As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Session.initFlag
            End Get
        End Property

        ''' <summary>
        ''' Directory for stores the application log file.(存放应用程序的日志文件的文件系统目录)
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly _LogDir As String = App.HOME & "/.logs/"

        ''' <summary>
        ''' Directory for stores the application log file.(存放应用程序的日志文件的文件系统目录)
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property LogDIR As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return _LogDir
            End Get
        End Property

        ''' <summary>
        ''' Data storage directory for the program settings.(配置文件所存放的文件夹)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SettingsDIR As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return File.DefaultXmlFile.ParentPath
            End Get
        End Property

        ''' <summary>
        ''' Initialize the application session and get the program profile data.
        ''' 
        ''' (初始化应用程序会话，并获取应用程序的配置数据，请注意，如果涉及到配置文件数据的修改保存操作，
        ''' 请确保在调用当前的这个初始化方法之前已经使用<see cref="Initialized"/>进行判断，否则新修改
        ''' 的配置数据将不会被保存到文件之中，因为调用这个方法将会加载新的配置引擎对象，因为在修改的时候所引用
        ''' 的为旧的配置引擎，所以新修改的配置值将无法写入到旧的配置引擎之中，因为对象的引用关系已经在调用
        ''' 这个初始化方法之后发生变化了)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function Initialize() As Settings.File
            Call FileIO.FileSystem.CreateDirectory(SettingsDIR)
            Call FileIO.FileSystem.CreateDirectory(_LogDir)
            Call FileIO.FileSystem.CreateDirectory(TEMP)
            Call FileIO.FileSystem.CreateDirectory(DataCache)

            Dim settings As String = Session.SettingsDIR & "/Settings.xml"
            Dim save As Action(Of Settings.File, String) = AddressOf saveProfile
#If DEBUG Then
            Call $"Load GCModeller settings data from xml file: {settings.ToFileURL}".__DEBUG_ECHO
#End If
            Session._profileData = ProfileEngine.Settings(Of Settings.File).LoadFile(settings, save)
            Session.initFlag = True

            Call App.JoinVariable("Settings", settings)
            Call App.JoinVariable("Workspace", worksapce)
            Call App.JoinVariable("Cache", cache)

            Return SettingsFile
        End Function

        Private Sub saveProfile(profile As Settings.File, path$)
            Call profile.GetXml.SaveTo(path)
        End Sub

        ''' <summary>
        ''' 获取得到安装有Mothur程序的Docker容器的ID, 或者可执行文件路径
        ''' </summary>
        ''' <returns></returns>
        Public Function Mothur() As String
#If UNIX Then

#End If

            ' 如果配置文件之中不存在,则尝试从命令行之中获取
            'If SettingsFile.Mothur.StringEmpty Then
            '    Return App.GetVariable
            'Else
            '    Return SettingsFile.Mothur
            'End If

            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Function MothurHome() As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 首先尝试通过配置文件得到脚本环境，假若没有配置这个值，则会尝试通过本身程序来测试，因为这个函数的调用可能是来自于Shoal脚本的
        ''' </summary>
        ''' <returns></returns>
        Public Function TryGetShoalShellBin() As String
            If Not Initialized Then
                Call Initialize()
            End If

            If Session.SettingsFile.ShoalShell.FileExists Then
                Return Session.SettingsFile.ShoalShell
            Else
                Call $"The shoal shell bin in the settings file is not a valid path value, try to using self as interpreter....".Warning
            End If

            '没有找到，由于这个函数本身可能就是从Shoal脚本之中调用的，则尝试使用自身作为解释器程序
            Dim AppPath As String = $"{App.HOME}/{App.AssemblyName}.exe"
            Dim AskVersion = New IORedirectFile(AppPath, "--version")
            Call AskVersion.Run()

            If Not Regex.Match(AskVersion.StandardOutput, "Shoal Shell \d+(\.\d+)*").Success Then
                Call $"Could not found the ShoalShell interpreter environment!".PrintException
                Return ""
            Else
                Call $"Test self ""{AppPath}"" as the ShoalShell interpreter!".PrintException
            End If

            Session.SettingsFile.ShoalShell = AppPath
            Session.SettingsFile.Save()

            Return AppPath
        End Function

        ''' <summary>
        ''' Just specific the java.exe installed location to running some external Java program in the GCModeller. 
        ''' The path value Of the java program usually Is In the location Like: ""C:\Program Files\Java\jre&lt;version>\bin\java.exe""
        ''' </summary>
        ''' <param name="java"></param>
        ''' <returns></returns>
        Public Function InstallJavaBin(<Parameter("Java.exe", "The exe path of the java program.")> java As String) As String
            If java.FileExists Then

                If Not Initialized Then
                    Call Session.Initialize()
                End If

                SettingsFile.Java = java
                SettingsFile.Save()
                Call Console.WriteLine($"Set up java.exe path to {java.CLIPath}")

                Return java
            Else
                Call Console.WriteLine($"The Java path is not exists on {java.CLIPath}!")
                Return ""
            End If
        End Function

        <ExportAPI("Install.Python")>
        Public Function InstallPython(Python As String) As String
            If Python.FileExists Then

                If Not Initialized Then
                    Call Session.Initialize()
                End If

                SettingsFile.Python = Python
                SettingsFile.Save()
                Call $"Set up python.exe path to {Python.CLIPath}".__DEBUG_ECHO

                Return Python
            Else
                Call $"The {NameOf(Python)} path is not exists on {Python.CLIPath}!".__DEBUG_ECHO
                Return ""
            End If
        End Function

        ''' <summary>
        ''' Close the application session and save the settings file.
        ''' </summary>
        ''' <remarks></remarks>
        <ExportAPI("Profile.Save")>
        Public Sub Finallize()
            Call ProfileData.Save(Nothing)
        End Sub

        <ExportAPI("Get.Settings")>
        Public Function GetSettings(Key As String) As String
            Return ProfileData.GetSettings(Name:=Key)
        End Function

#If Not DISABLE_BUG_UNKNOWN Then
        ''' <summary>
        ''' Listing all of the settings data to the user console.
        ''' </summary>
        ''' <returns></returns>
        Public Function List() As Dictionary(Of String, String)
            Dim ChunkBuffer As BindMapping() = ProfileData.AllItems
            Dim data As Dictionary(Of String, String) = New Dictionary(Of String, String)

            For Each x In ChunkBuffer
                Call data.Add(x.Name, x.Value)
            Next
            Call Console.WriteLine(ConfigEngine.Prints(ChunkBuffer))
            Return data
        End Function
#End If

        ''' <summary>
        ''' Writes the settings data into the GCModeller profile sessions.
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function SetValue(name As String, value As String) As Boolean
            Return ProfileData.Set(name, value)
        End Function

        ''' <summary>
        ''' For unawareless of overrides the original data file, this function will automatcly add a .std_out extension to the STDOUT filepath.
        ''' (新构建一个Shoal实例运行一个分支脚本任务，在.NET之中线程的效率要比进程的效率要低，使用这种多线程的方法来实现并行的效率要高很多？？？？)
        ''' </summary>
        ''' <param name="Script">脚本文件的文件文本内容</param>
        ''' <param name="STDOUT">Standard output redirect to this file. Facility the GCModeller debugging.</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Exec")>
        Public Function FolkShoalThread(Script As String, STDOUT As String) As Integer
            If Not Initialized Then
                Call Initialize()
            End If

            Dim ShoalShell As String = Session.TryGetShoalShellBin
            Dim ScriptPath As String = FileIO.FileSystem.GetTempFileName

            Call Script.SaveTo(ScriptPath)

            STDOUT = STDOUT & ".std_out"
            Call $"{NameOf(STDOUT)} >>> {STDOUT.ToFileURL}".__DEBUG_ECHO

            Return New IORedirectFile(ShoalShell, argv:=ScriptPath.CLIPath, stdRedirect:=STDOUT).Run
        End Function
    End Module
End Namespace
