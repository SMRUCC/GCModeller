Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' 内部的一些简单的常用命令
''' </summary>
''' 
<Package("System.Extensions",
                  Description:="Shoal system internal plugins module to provides some basically operation on your file system or scripting.",
                  Publisher:="xie.guigang@gmail.com",
                  Url:="http://SourceForge.net/projects/shoal")>
Public Module InternalExtension

    Public ReadOnly Property License As String
        Get
            Return My.Resources.license
        End Get
    End Property

    <Extension> Friend Function [As](Of T, V)(o As T) As V
        Return DirectCast(CObj(o), V)
    End Function

    ''' <summary>
    ''' 脚本的执行入口点的定义
    ''' </summary>
    ''' <param name="Script">脚本中的内容</param>
    ''' <param name="parameters">如果为Nothing，则说明目标脚本的执行不需求参数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Delegate Function ScriptSourceHandle(Script As String, parameters As KeyValuePair(Of String, Object)()) As Object

    <ExportAPI("basename", Info:="Get the name property of a specific file object or folder." & vbCrLf &
                               "If the target path is a file, then returns its file name without its extension name.")>
    Public Function basename(path As String) As String
        If FileIO.FileSystem.DirectoryExists(path) Then
            Return FileIO.FileSystem.GetDirectoryInfo(path).Name
        Else
            Return basename(path)
        End If
    End Function

    <ExportAPI("ver")>
    Public Function Version() As Version
        Call Cowsay(String.Format("The version of Shoal is {0}", My.Application.Info.Version.ToString))
        Return My.Application.Info.Version
    End Function

    <ExportAPI("wiki", Info:="https://sourceforge.net/p/shoal/wiki/search/?q=keyword")>
    Public Function Wiki(<MetaData.Parameter("search.keyword")> Optional keyword As String = "") As String
        Dim url As String = If(String.IsNullOrEmpty(keyword), "https://sourceforge.net/p/shoal/wiki/", "https://sourceforge.net/p/shoal/wiki/search/?q=" & keyword.Replace(" ", "%20"))
        Call Process.Start(url)
        Return url
    End Function

    <ExportAPI("license")>
    Public Function GetLicenseInfo() As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        Call sBuilder.AppendLine(My.Resources.license)
        Call sBuilder.AppendLine()
        Call sBuilder.AppendLine()
        Call sBuilder.AppendLine("Using gpl() command to see the whole GPL3 license.")

        Call Console.WriteLine(sBuilder.ToString)

        Return sBuilder.ToString
    End Function

    <ExportAPI("gpl", Info:="Prints the gpl3 license to the console screen that it applied to the shoal shell.")>
    Public Function GPL() As String
        Call Console.WriteLine(My.Resources.gpl)
        Return My.Resources.gpl
    End Function

    <ExportAPI("echo", Info:="Print the message text on the terminal console.")>
    Public Function Echo(<MetaData.Parameter("Message", "")> Message As String) As String
        Call Console.WriteLine(Message)
        Return Message
    End Function

    <ExportAPI("cowsay", Info:="A cowsay trick to print the message on the console.")>
    Public Function Cowsay(Optional msg As String = "", Optional dead As Boolean = True) As String
        If String.IsNullOrEmpty(msg) Then
            Return CowsayTricks.RunCowsay("Moo. Hi!", dead)
        End If

        Return CowsayTricks.RunCowsay(msg, dead)
    End Function

    <ExportAPI("cat", Info:="Read the text file data from the parameter path and then print the text data on the console.")>
    Public Function Cat(path As String) As String
        Dim s As String = FileIO.FileSystem.ReadAllText(path)
        Call Console.WriteLine(s)
        Return s
    End Function

    <ExportAPI("clear", Info:="Clear the output console.")>
    Public Function Clear() As Integer
        Call Console.Clear()
        Return 0
    End Function

    <ExportAPI("pwd", Info:="Gets the currently working directory of shoal shell.")>
    Public Function Pwd() As String
        Call Console.WriteLine(My.Computer.FileSystem.CurrentDirectory)
        Return My.Computer.FileSystem.CurrentDirectory
    End Function

    ''' <summary>
    ''' 当指定了文件拓展名之后，函数只会返回文件名列表，其他的情况会返回文件名列表和文件夹列表
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("ls", Info:="using -ext parameter to specific the parameter, -d to specific the target directory, empty value for the current directory.")>
    Public Function List(<MetaData.Parameter("-d")> Optional dir As String = "", <MetaData.Parameter("-ext")> Optional ext As String = "") As String()
        If String.IsNullOrEmpty(dir) Then
            dir = My.Computer.FileSystem.CurrentDirectory
        End If

        If String.IsNullOrEmpty(ext) Then
            ext = "*.*"
        End If

        Dim Dirs = FileIO.FileSystem.GetDirectories(dir, FileIO.SearchOption.SearchTopLevelOnly)
        Dim Files = FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly, ext)
        Dim DriveInfo = FileIO.FileSystem.GetDriveInfo(dir)
        Dim sBuilder As StringBuilder = New StringBuilder(1024)

        Call sBuilder.AppendLine(String.Format(" Volume in drive {0} is {1}", DriveInfo.RootDirectory, DriveInfo.VolumeLabel))
        Call sBuilder.AppendLine(String.Format(" Volume drive Format is {0}" & vbCrLf, DriveInfo.DriveFormat))
        Call sBuilder.AppendLine(String.Format(" Directory of {0}" & vbCrLf, dir))
        If Not Dirs.IsNullOrEmpty Then Call sBuilder.AppendLine(String.Format("  {0} Directories", Dirs.Count))
        If Not Files.IsNullOrEmpty Then Call sBuilder.AppendLine(String.Format("  {0} Files", Files.Count))
        Call sBuilder.AppendLine()

        For Each sDir As String In Dirs
            Dim dirInfo = FileIO.FileSystem.GetDirectoryInfo(sDir)
            Call sBuilder.AppendLine(String.Format("{0} <DIR>  {1}", dirInfo.LastWriteTimeUtc.DateToString, dirInfo.Name))
        Next
        Call sBuilder.AppendLine()
        For Each File As String In Files
            Dim fileInfo = FileIO.FileSystem.GetFileInfo(File)

            If String.Equals(fileInfo.Extension, ".shl") Then
                Call sBuilder.AppendLine(String.Format("{0} <SHL>  {1}", fileInfo.LastWriteTimeUtc.DateToString, fileInfo.Name))
            Else
                Call sBuilder.AppendLine(String.Format("{0}        {1}", fileInfo.LastWriteTimeUtc.DateToString, fileInfo.Name))
            End If
        Next
        Call Console.WriteLine(sBuilder.ToString)

        If String.Equals(ext, "*.*") Then
            '返回所有
            Dim ChunkBuffer = Dirs.ToArray.Join(Files)
            Return ChunkBuffer.ToArray
        Else
            Return Files.ToArray
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="argv"></param>
    ''' <returns></returns>
    <ExportAPI("system", Info:="Folk a process from the specific command line arguments of this function.", Usage:="system ""<path>[ args]""")>
    Public Function ProcessStart(argv As CommandLine.CommandLine) As Process
        Dim file As String = FileIO.FileSystem.GetFileInfo(argv("-file")).FullName, arguments As String = argv("-argv")
        Dim Process As Process = New Process()
        Process.StartInfo = New System.Diagnostics.ProcessStartInfo(file)
        If Not String.IsNullOrEmpty(arguments) Then
            Process.StartInfo.Arguments = arguments
        End If
        Call Process.Start()
        Return Process
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("source",
               Info:="Call the external script file in the shoal shell scripting, you can get the returns value from the 'Return' command.",
               Usage:="source <script_file> [argumentsName argumentValue]")>
    Public Function Source(path As String, Optional args As Generic.IEnumerable(Of KeyValuePair(Of String, Object)) = Nothing) As Object
        Dim Environment As New Runtime.ScriptEngine()
        Dim requiredCwork As Boolean = Not (InStr(path, "http://", CompareMethod.Text) > 0 OrElse InStr(path, "https://", CompareMethod.Text) > 0)  ' url 无法切换路径
        Dim currentWork As String = My.Computer.FileSystem.CurrentDirectory

        If Not args.IsNullOrEmpty Then
            For Each item As KeyValuePair(Of String, Object) In args
                Call Environment.MMUDevice.InitLocals(item.Key, item.Value, "string")
            Next
        End If

        If requiredCwork Then
            My.Computer.FileSystem.CurrentDirectory = FileIO.FileSystem.GetParentPath(path)
#If DEBUG Then
            Call $"Change current work directory to {My.Computer.FileSystem.CurrentDirectory}".__DEBUG_ECHO
#End If
        End If
        Call Environment.Source(path)
        If requiredCwork Then
            My.Computer.FileSystem.CurrentDirectory = currentWork
        End If

        Return Environment.MMUDevice.SystemReserved.Value
    End Function
End Module
