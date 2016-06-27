Namespace LocalBLAST.Programs

    Public NotInheritable Class RpsBLAST : Inherits NCBI.Extensions.LocalBLAST.InteropService.LocalBlastProgramGroup

        Protected Friend ReadOnly _InternalExecutableMakeProfileDb As String
        Protected Friend ReadOnly _InternalExecutableRpsBLAST As String

        Public ReadOnly Property LastBLASTOutputFilePath As String
            Get
                Return _InternalLastBLASTOutputFile
            End Get
        End Property

        Sub New(BLASTBin As String)
            Me._innerBLASTBinDIR = BLASTBin
            Me._InternalExecutableMakeProfileDb = BLASTBin & "/makeprofiledb.exe"
            Me._InternalExecutableRpsBLAST = BLASTBin & "/rpsblast.exe"
        End Sub

        Public Function MakeProfileDb(pnList As String) As CommandLine.IORedirect
            Dim TargetAssembly As String = FileIO.FileSystem.GetParentPath(pnList) & "/makeprofiledb.exe"
            Dim Cmdl As CommandLine.IORedirect = New CommandLine.IORedirect(TargetAssembly, String.Format("-in ""{0}"" -dbtype rps", pnList))
            Call FileIO.FileSystem.CopyFile(Me._InternalExecutableMakeProfileDb, TargetAssembly)
            Call Console.WriteLine("[RPSBLAST_MAKEPROFILEDB]{0}  ---> ""{1}""", vbCrLf, Cmdl.ToString)

            Return Cmdl
        End Function

        Public Function Performance(FsaDb As String, rpsDb As String, Evalue As String, Output As String) As CommandLine.IORedirect
            Dim Argv As String = String.Format("-query ""{0}"" -evalue {1} -out ""{2}"" -db ""{3}""", FsaDb, Evalue, Output, rpsDb)
            Dim Cmdl As CommandLine.IORedirect = New CommandLine.IORedirect(Me._InternalExecutableRpsBLAST, Argv)
            _InternalLastBLASTOutputFile = Output
            Call Console.WriteLine("[RPSBLAST]{0}  ---> ""{1}""", vbCrLf, Cmdl.ToString)

            Return Cmdl
        End Function
    End Class
End Namespace