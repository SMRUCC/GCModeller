Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Scripting.ShoalShell.SPM

Namespace Configuration

    ''' <summary>
    ''' 脚本引擎的配置文件
    ''' </summary>
    ''' 
    <XmlType("Shoal.Configuration", [Namespace]:="http://gcmodeller.org/shoal.config")>
    Public Class Config : Inherits ITextFile
        Implements IProfile

#Region "Configuration storage property region"

        ''' <summary>
        ''' 应用程序启动的时候的初始工作目录，默认为应用程序所在的文件夹
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ComponentModel.Settings.ProfileItem(Name:="init_dir")> Public Property InitDir As String
        ''' <summary>
        ''' 脚本引擎的类型注册表的文件位置，为空的话会使用默认的文件位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ComponentModel.Settings.ProfileItem(Name:="registry_location")> Public Property SPMRegistry As String
            Get
                Return _RegistryFileLocation
            End Get
            Set(value As String)
                value = VisualBasic.FileIO.FileSystem.GetFileInfo(value).FullName
                _RegistryFileLocation = value
            End Set
        End Property

        <ComponentModel.Settings.ProfileItem(Name:="last_dir_as_init")> Public Property LastDirAsInit As String = "True"
        <ComponentModel.Settings.ProfileItem(Name:="preload_modules")> Public Property PreLoadModules As String
        <ComponentModel.Settings.ProfileItem(Name:="indexed_manual")> Public Property ManualPreferIndexPages As String = "TRUE"
        <ComponentModel.Settings.ProfileItem(Name:="save_history")> Public Property SaveHistory As String = "TRUE"
        <ComponentModel.Settings.ProfileItem(Name:="init.heapsize")> Public Property InitHeapSize As String = 1024
        <ComponentModel.Settings.ProfileItem(Name:="exts")> Public Property ScriptExt As String = "*.shl;*.txt"
        <ComponentModel.Settings.ProfileItem(Name:="Console.Title.Auto")> Public Property ConsoleTitleAutoDisplay As String = "true"

        Dim _RegistryFileLocation As String

#End Region

        Public Function EnableConsoleTitle() As Boolean
            Return ConsoleTitleAutoDisplay.ParseBoolean
        End Function

        Public Function GetExtensionList() As String()
            Dim list = Strings.Split(ScriptExt, ";"c)
            If list.IsNullOrEmpty Then
                ScriptExt = "*.shl"
                Return {ScriptExt}
            Else
                Return list
            End If
        End Function

        Public Function GetInitHeapSize() As Integer
            Dim value As Integer = CInt(Val(InitHeapSize))
            If value <= 0 Then
                InitHeapSize = 1024
                value = 1024
            End If

            Return value
        End Function

        Public Function PreferIndexingManual() As Boolean
            Return ManualPreferIndexPages.ParseBoolean
        End Function

        Public Function PreLoadedModules() As String()
            If String.IsNullOrEmpty(Me.PreLoadModules) Then
                Return New String() {}
            Else
                Return PreLoadModules.Split(";"c)
            End If
        End Function

        Public Function LastDir_AsInit() As Boolean
            Return LastDirAsInit.ParseBoolean
        End Function

        Public Function InitDirectory() As String
            If String.IsNullOrEmpty(InitDir) Then
                Return My.Application.Info.DirectoryPath
            End If

            If VisualBasic.FileIO.FileSystem.DirectoryExists(Me.InitDir) Then
                Return VisualBasic.FileIO.FileSystem.GetDirectoryInfo(Me.InitDir).FullName
            End If
            Try
                Call VisualBasic.FileIO.FileSystem.CreateDirectory(Me.InitDir)
            Catch ex As Exception
                Call $"[DEBUG] The specific work directory ""{InitDir}"" is unable to located, using the application directory ""{App.HOME}"" insteaded.".__DEBUG_ECHO
                InitDir = App.HOME
                Return App.HOME
            End Try

            Return VisualBasic.FileIO.FileSystem.GetDirectoryInfo(Me.InitDir).FullName
        End Function

        ''' <summary>
        ''' Get shoal shell registry file path.(获取注册表的文件路径)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRegistryFile() As String
            If String.IsNullOrEmpty(SPMRegistry) Then
                Return PackageModuleDb.DefaultFile
            Else
                Return VisualBasic.FileIO.FileSystem.GetFileInfo(Me.SPMRegistry).FullName
            End If
        End Function

        ''' <summary>
        ''' The default file location of the shoal <see cref="Runtime.scriptengine"/> configuration data.
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property DefaultFile As String
            Get
                Return $"{App.LocalData}/.Settings/{FileIO.FileSystem.GetFileInfo(App.ExecutablePath).Name}.conf"
            End Get
        End Property

        Public Shared ReadOnly Property [Default] As Settings(Of Config)
            Get
                Return Settings(Of Config).LoadFile(XmlFile:=Config.DefaultFile)
            End Get
        End Property

        ''' <summary>
        ''' Load the configuration file from the default file location: <see cref="DefaultFile"/>.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function LoadDefault() As Config
            Return [Default].SettingsData
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As System.Text.Encoding = Nothing) As Boolean
            If LastDir_AsInit() Then
                Me.InitDir = VisualBasic.FileIO.FileSystem.GetDirectoryInfo(My.Computer.FileSystem.CurrentDirectory).FullName
            End If

            FilePath = Me.getPath(FilePath)
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function

        Protected Overrides Function __getDefaultPath() As String
            Return FilePath
        End Function
    End Class
End Namespace