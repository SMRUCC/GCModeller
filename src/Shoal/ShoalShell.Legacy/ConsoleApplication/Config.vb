Imports Microsoft.VisualBasic.ComponentModel.Settings

Public Class Config : Inherits Microsoft.VisualBasic.ComponentModel.ITextFile
    Implements Microsoft.VisualBasic.ComponentModel.Settings.IProfile

#Region "Configuration storage property region"

    ''' <summary>
    ''' 应用程序启动的时候的初始工作目录，默认为应用程序所在的文件夹
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ProfileItem(name:="init_dir")> Public Property InitDir As String
    ''' <summary>
    ''' 脚本引擎的类型注册表的文件位置，为空的话会使用默认的文件位置
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ProfileItem(name:="registry_location")> Public Property RegistryFileLocation As String
        Get
            Return _RegistryFileLocation
        End Get
        Set(value As String)
            value = FileIO.FileSystem.GetFileInfo(value).FullName
            _RegistryFileLocation = value
        End Set
    End Property

    <ProfileItem(name:="last_dir_as_init")> Public Property LastDirAsInit As String
    <ProfileItem(name:="preload_modules")> Public Property PreLoadModules As String
    <ProfileItem(name:="indexed_manual")> Public Property ManualPreferIndexPages As String = "TRUE"
    <ProfileItem(name:="save_history")> Public Property SaveHistory As String = "TRUE"

    Dim _RegistryFileLocation As String

#End Region

    Public Function get_PreferIndexingManual() As Boolean
        Return ManualPreferIndexPages.getBoolean
    End Function

    Public Function get_PreLoadedModules() As String()
        Return PreLoadModules.Split
    End Function

    Public Function get_LastDir_AsInit() As Boolean
        Return LastDirAsInit.getBoolean
    End Function

    Public Function get_InitDirectory() As String
        If String.IsNullOrEmpty(InitDir) Then
            Return My.Application.Info.DirectoryPath
        Else
            If Not FileIO.FileSystem.DirectoryExists(InitDir) Then
                Try
                    Call FileIO.FileSystem.CreateDirectory(InitDir)
                Catch ex As Exception
                    Call Console.WriteLine("[DEBUG] The specific work directory ""{0}"" is unable to located, using the application directory ""{1}"" insteaded.", InitDir, My.Application.Info.DirectoryPath)
                    Return My.Application.Info.DirectoryPath
                End Try
            End If

            Return FileIO.FileSystem.GetDirectoryInfo(InitDir).FullName
        End If
    End Function

    ''' <summary>
    ''' Get shoal shell registry file path.(获取注册表的文件路径)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function get_RegistryFile() As String
        If String.IsNullOrEmpty(RegistryFileLocation) Then
            Return Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.DefaultFile
        Else
            Return FileIO.FileSystem.GetFileInfo(RegistryFileLocation).FullName
        End If
    End Function

    Public Shared ReadOnly Property DefaultFile As String
        Get
            Return String.Format("{0}/{1}.cfg", My.Application.Info.DirectoryPath, My.Application.Info.AssemblyName)
        End Get
    End Property

    Public Shared ReadOnly Property [Default] As Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of Config)
        Get
            Return Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of Config).LoadFile(XmlFile:=Config.DefaultFile)
        End Get
    End Property

    Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As System.Text.Encoding = Nothing) As Boolean
        If get_LastDir_AsInit() Then
            InitDir = FileIO.FileSystem.GetDirectoryInfo(My.Computer.FileSystem.CurrentDirectory).FullName
        End If

        FilePath = Me.getPath(FilePath)
        Return Me.GetXml.SaveTo(FilePath, Encoding)
    End Function
End Class
