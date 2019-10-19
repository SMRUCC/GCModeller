Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' This module provides some common operation in the shoal scripting.
''' </summary>
''' <remarks></remarks>
<[Namespace]("System.Extensions", Description:="This module provides some common operation in the shoal scripting.")>
Public Module InternalCommands

    <ExportAPI("Trim.Linux", Info:="Convert the text file format from Windows style into Linux style.")>
    Public Function TrimLinux(Path As String) As Boolean
        Dim s_Data As String() = System.IO.File.ReadAllLines(Path)
        Dim Temp As String = String.Join(vbCr, s_Data)
        Return Temp.SaveTo(Path, System.Text.Encoding.UTF8)
    End Function

    ''' <summary>
    ''' 批量执行指定的文件夹之中的所有Shoal脚本
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Invoke.Batch")>
    Public Function BatchInvoke(<Parameter("Source.Dir", "The directory which contains some shoal script for the batch invoke work.")> Optional dir As String = "./",
                                <Parameter("Parallel", "Please notices that some java program can be calling its threads automatically base on the current system cpu usage, so that this situation the parallel optional is not works well as you expect, then you should disable the parallel option..")> Optional Parallel As Boolean = True) As Integer
        Dim Shoal As String = ExecutablePath
        Dim ScriptSource = dir.LoadSourceEntryList({"*.txt", "*.shl"}).ToArray

        If Parallel Then
            Call System.Threading.Tasks.Parallel.ForEach(Of KeyValuePair(Of String, String))(
           ScriptSource,
           Sub(PathEntry As KeyValuePair(Of String, String))
               Call Threading.Thread.Sleep(5 * 1000)
               Dim proc = Process.Start(Shoal, PathEntry.Value.CLIPath) '执行脚本
               Call proc.WaitForExit()
           End Sub)
        Else
            For Each PathEntry In ScriptSource
                Dim proc = Process.Start(Shoal, PathEntry.Value.CLIPath)  '执行脚本
                Call Console.WriteLine($"[DEBUG {Now.ToString}] Execute task {PathEntry.Value.ToFileURL}.....")
                Call proc.WaitForExit()
            Next
        End If

        Call Console.WriteLine("Job Done!")
        Return 0
    End Function

    <Extension> Public Function Invoke(Of T As Class)(obj As T, Entry As String) As Double
        Dim Method = (From m In GetType(T).GetMethods(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
                      Where String.Equals(m.Name, Entry)
                      Select m).FirstOrDefault
        If Method Is Nothing Then
            Return -1
        Else
            Call Threading.Thread.Sleep(5 * 1000) '有可能出现同一个资源被占用的情况，则可能会出错，则批量调用的时候会有一个时间差以避免这个错误
            Dim sw = Stopwatch.StartNew
            Call Method.Invoke(obj, Nothing)
            Return sw.ElapsedMilliseconds
        End If
    End Function

    <ExportAPI("beep")>
    Public Function Beep() As Integer
        Call Console.Beep()
        Return 0
    End Function

    <ExportAPI("Sum")>
    Public Function Sum(dat As Generic.IEnumerable(Of Double)) As Double
        Return dat.Sum
    End Function

    <ExportAPI("located")>
    Public Function Locate(Optional path As String = "") As String
        If String.IsNullOrEmpty(path) OrElse Not FileIO.FileSystem.FileExists(path) Then
            path = My.Computer.FileSystem.CurrentDirectory
        End If

        path = FileIO.FileSystem.GetDirectoryInfo(path).FullName

        Call Process.Start(path)
        Return path
    End Function

    <ExportAPI("string.format")>
    Public Function Format(Expression As String, argvs As Generic.IEnumerable(Of Object)) As String
        Return String.Format(Expression, argvs.ToArray)
    End Function

    <ExportAPI("write.array")>
    Public Function WriteArray(array As Generic.IEnumerable(Of Object), saveTo As String) As Integer
        Call IO.File.WriteAllLines(saveTo, (From item In array Let strValue As String = item.ToString Select strValue).ToArray)
        Return array.Count
    End Function

    <ExportAPI("read.txt")>
    Public Function ReadTxt(file As String) As String
        Dim strData = FileIO.FileSystem.ReadAllText(FileIO.FileSystem.GetFileInfo(file).FullName)
        Return strData
    End Function

    <ExportAPI("read.text_lines")>
    Public Function ReadAllLines(path As String) As String()
        Return IO.File.ReadAllLines(path)
    End Function

    <ExportAPI("write.text_lines")>
    Public Function WriteAllLines(data As Generic.IEnumerable(Of String), saveto As String) As Boolean
        Call IO.File.WriteAllLines(saveto, data.ToArray)
        Return True
    End Function

    <ExportAPI("msgbox")>
    Public Function _Msgbox(Message As String, Optional Title As String = "") As String
        MsgBox(Message, MsgBoxStyle.Information, Title)
        Return Message
    End Function

    <ExportAPI("and")>
    Public Function [AND](logicals As Boolean()) As Boolean
        For Each item As Boolean In logicals
            If item = False Then
                Return False
            End If
        Next

        Return True
    End Function

    <ExportAPI("-gt")>
    Public Function GreaterThan(a As Double, b As Double) As Boolean
        Return a > b
    End Function

    <ExportAPI("or")>
    Public Function [OR](logicals As Boolean()) As Boolean
        Dim LQuery = (From item In logicals.AsParallel Where item = True Select 1).ToArray
        Return Not LQuery.IsNullOrEmpty
    End Function

    <ExportAPI("!")>
    Public Function [NOT](value As Boolean) As Boolean
        Return Not value
    End Function

    <ExportAPI("Read.Image")>
    Public Function ReadImage(path As String) As Image
        Return Image.FromFile(path)
    End Function

    <ExportAPI("collection.split")>
    Public Function SplitCollection(collection As Generic.IEnumerable(Of Object), n As Integer) As Object()()
        Return collection.Split(partitionSize:=n)
    End Function

    <ExportAPI("sleep")>
    Public Function Sleep(n As Integer) As Integer
        Call System.Threading.Thread.Sleep(n * 1000)
        Return n
    End Function

    <ExportAPI("sequence")>
    Public Function Sequence(n As Integer) As Integer()
        Return n.Sequence
    End Function

    <ExportAPI("Pause", Info:="Pause the process script, waits for user input, when user press any key on the console, then the process will be resume running.")>
    Public Function Pause() As Integer
        Call Console.WriteLine()
        Call Console.WriteLine("Press any key to continute...")
        Return Console.Read
    End Function

    ''' <summary>
    ''' 更加一般性的复制函数，当目标文件夹之中的文件数目非常的多的时候，可以使用这个函数进行批量的文件复制，只需要把文件名填入列表之中即可，大小写无关
    ''' </summary>
    ''' <returns>复制失败的文件名列表</returns>
    <ExportAPI("Source.Copy", Info:="If some files have the same file name but the extension name is different, then all of the files wilL be copy too.")>
    Public Function SourceCopy(<Parameter("ID.List")> IDList As Generic.IEnumerable(Of String),
                               <Parameter("Dir.Source")> Source As String,
                               <Parameter("Dir.CopyTo")> CopyTo As String) As String()

        Dim FailuredList As New List(Of String)
        Dim FileList = FileIO.FileSystem.GetFiles(Source, FileIO.SearchOption.SearchTopLevelOnly).AsList

        Call FileIO.FileSystem.CreateDirectory(CopyTo)

        For Each ID As String In IDList
            Dim Files = (From path As String In FileList.AsParallel Let Name As String = basename(path) Where String.Equals(ID, Name, StringComparison.OrdinalIgnoreCase) Select path).ToArray
            If Files.IsNullOrEmpty Then
                Call FailuredList.Add(ID)
            End If

            For Each path As String In Files
                Try
                    Call FileIO.FileSystem.CopyFile(path, CopyTo & "/" & FileIO.FileSystem.GetFileInfo(path).Name)
                Catch ex As Exception
                    Call Console.WriteLine(path.ToFileURL)
                    Call Console.WriteLine(ex.ToString)
                End Try
            Next
        Next

        Return FailuredList.ToArray
    End Function

#Const DEBUG = 1

#If DEBUG Then

    <ExportAPI("Exception_Handler.Test")>
    Public Function TestException() As Boolean
        Throw New Exception("Dont worried, this is a test for shellscript exception handler.")
    End Function

    <ExportAPI("___overloads_test()")>
    Public Function OverloadsTest(n As Integer) As Integer
        Call Console.WriteLine(Integer.MaxValue)
        Call Console.WriteLine(Rnd() * n)
        Return Integer.MaxValue
    End Function

    <ExportAPI("___overloads_test()")>
    Public Function OverloadsTest(<Parameter("test.string", "Tesing!")> s1 As String, <Parameter("any", "This is a description for the testing function parameter!")> s2 As String) As String
        Call Console.WriteLine(s1)
        Call Console.WriteLine(s2 & "   anysdfs")
        Return s1
    End Function
#End If

End Module
