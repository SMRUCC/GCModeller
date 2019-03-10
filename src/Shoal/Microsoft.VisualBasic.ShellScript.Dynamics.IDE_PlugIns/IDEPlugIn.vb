Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime
Imports Microsoft.VisualBasic.Windows.Forms
Imports Microsoft.VisualBasic.Language.UnixBash

<[Namespace]("dynamics.ide_plugins")>
Public Module IDEPlugIn

    Dim CommandName As String
    Dim Iconpath As String
    Dim Target As ToolStripMenuItem
    Dim _currentPath As String

    <ExportAPI("plugin.set_name")>
    Public Function SetName(value As String) As String
        CommandName = value
        Return IDEPlugIn.CommandName
    End Function

    <ExportAPI("plugin.set_icon")>
    Public Function SetIcon(value As String) As String
        Try
            IDEPlugIn.Iconpath = FileIO.FileSystem.GetFileInfo(value).FullName
        Catch ex As Exception
            Call Console.WriteLine("FILE_NOT_FOUND::menu icon image file ""{0}"" is not found on the filesystem, image will not load.", value)
        End Try
        Return IDEPlugIn.Iconpath
    End Function

    <ExportAPI("plugin.initialize", Info:="This method should be the last command that you call in the shellscript")>
    Public Function Initialize(action As System.Action) As Boolean
        Dim CommandEntry = PlugIns.MenuAPI.AddCommand(Target, _currentPath, CommandName)
        If Not String.IsNullOrEmpty(Iconpath) AndAlso FileIO.FileSystem.FileExists(Iconpath) Then CommandEntry.Image = System.Drawing.Image.FromFile(Iconpath)
        AddHandler CommandEntry.Click, Sub() Call action()       '关联命令

        Return 0
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Entry">插件的载入点</param>
    ''' <param name="pluginDIR">ShellScript插件脚本的文件夹路径</param>
    ''' <remarks></remarks>
    Public Sub LoadScripts(Entry As ToolStripMenuItem, pluginDIR As String)
        Target = Entry
        pluginDIR = FileIO.FileSystem.GetDirectoryInfo(pluginDIR).FullName

        Using shoalShell As ScriptEngine = New ScriptEngine
            For Each script As String In ls - l - r - wildcards("*.txt", "*.vbss") <= pluginDIR
                _currentPath = script.Replace(pluginDIR, "")
                Call shoalShell.Exec(script)
            Next
        End Using
    End Sub
End Module
