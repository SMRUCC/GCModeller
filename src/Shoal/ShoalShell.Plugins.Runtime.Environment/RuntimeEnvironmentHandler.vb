Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.SPM
Imports Microsoft.Win32

''' <summary>
''' 假若将本模块直接集成在Shoal的主程序之中，则在Windows 7或者更高版本的Windows之中，假若开启了UAC的话，则程序每一次执行都必须要管理员权限了，所以现在将涉及到系统底层的操作都放在一个模块之中以避免这个问题
''' </summary>
''' <remarks></remarks>
''' 
<[Namespace]("Shoal.Runtime")>
Public Module RuntimeEnvironmentHandler

    Const UAC_EXCEPTION As String = "UAC is turn on on your windows, you should run Shoal As Administrator and then using this command to associate the script file!"

    <ExportAPI("Associates.ShoalScriptFile")>
    Public Function FileAssociations() As Boolean
        Try

            Dim IconPath As String = FileIO.FileSystem.GetFileInfo(My.Application.Info.DirectoryPath & "/icons/shl.ico").FullName
            Dim InteropPath As String = My.Application.Info.DirectoryPath & "/icons/shoalex.exe"
            Dim ShoalPath As String = FileIO.FileSystem.GetFileInfo(String.Format("{0}/{1}.exe", My.Application.Info.DirectoryPath, My.Application.Info.AssemblyName)).FullName
            Dim argvs As String = String.Format("""{0}"" ""{1}""", ShoalPath, IconPath)

            Call My.Resources.Shoal.FlushStream(IconPath)
            Call My.Resources.ShoalEx.FlushStream(InteropPath)
            Call Process.Start(InteropPath, argvs)
            Call Threading.Thread.Sleep(100)
            Call FileIO.FileSystem.DeleteFile(InteropPath, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)

        Catch ex As Exception
            Call App.LogException(ex)
            Call Console.WriteLine(UAC_EXCEPTION)
        End Try

        Return True
    End Function

    <ExportAPI("Plugins.Scan")>
    Public Function ScanPlugIns(DIR As String, Optional ext As String = "*.*") As PackageModuleDb
        ext = If(String.IsNullOrEmpty(ext), "*.*", ext)

        Dim FilesForScan = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, ext)
        Dim SPMgrDb As PackageModuleDb = PackageModuleDb.LoadDefault

        Using SMgr As New ShoalPackageMgr(SPMgrDb)
            For Each File As String In FilesForScan
                Call SMgr.Imports(File)
            Next
        End Using

        Return SPMgrDb
    End Function

    Const WMIC_PATH_CMD As String = "ENVIRONMENT where ""name='path' and username='<system>'"" set VariableValue=""%path%;{0}"""

    <ExportAPI("Init.Environment_var")>
    Public Function SetupEnvironmentVariable() As Boolean
        Dim path As String = My.Application.Info.DirectoryPath
        Dim cmdl As String = String.Format(WMIC_PATH_CMD, path)
        Dim Process = System.Diagnostics.Process.Start("wmic", cmdl)
        Call Process.WaitForExit()

        Return Process.ExitCode = 0
    End Function
End Module
