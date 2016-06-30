Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Configuration

Namespace Runtime.SCOM

    Public Module RuntimeEnvironment

        Public Const SCAN_PLUGINS_ARGVS As String = "-scan.plugins -dir ""{0}"""

        ''' <summary>
        ''' -scan.plugins -dir &lt;dir>[ -ext *.*/*.dll/*.exe/*.lib /top_only /clean]
        ''' </summary>
        ''' <param name="args"></param>
        ''' <returns></returns>
        <ExportAPI("-scan.plugins", Usage:="-scan.plugins -dir <dir>[ -ext *.*/*.dll/*.exe/*.lib /recursive /clean]",
            Info:="Scanning all of the avaliable shoal plugin modules in the specific directory and install all of them into the shoal registry.",
            Example:="-scan.plugins -dir ./ -ext *.dll")>
        Public Function ScanPlugins(args As CommandLine.CommandLine) As Integer
            Dim Dir As String = args("-dir"), Ext As String = args("-ext")
            Ext = If(String.IsNullOrEmpty(Ext), "*.dll;*.exe;*.lib", Ext)
            Dim ExtList As String() = Ext.Split(";"c)
            Dim [option] = If(args.GetBoolean("/recursive") = False,
                FileIO.SearchOption.SearchTopLevelOnly,
                FileIO.SearchOption.SearchAllSubDirectories)
            Dim FilesForScan = FileIO.FileSystem.GetFiles(Dir, [option], ExtList)

            If args.GetBoolean("/clean") Then
                Try
                    Call FileIO.FileSystem.DeleteFile(Config.LoadDefault.GetRegistryFile)
                Catch ex As Exception

                End Try
            End If

            Dim Db As Microsoft.VisualBasic.Scripting.ShoalShell.SPM.PackageModuleDb =
                Microsoft.VisualBasic.Scripting.ShoalShell.SPM.PackageModuleDb.Load(Config.LoadDefault.GetRegistryFile)

            Using SPM As New Scripting.ShoalShell.SPM.ShoalPackageMgr(Db)
                For Each File As String In FilesForScan
                    Call SPM.Imports(File)
                Next

                Call SPM.UpdateDb()
            End Using

            Return 0
        End Function
    End Module
End Namespace