Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository
Imports SMRUCC.genomics.Repository

Partial Module CLI

    <ExportAPI("/Install.genbank", Usage:="/Install.genbank /imports <all_genbanks.DIR> [/refresh]")>
    Public Function InstallGenbank(args As CommandLine.CommandLine) As Integer
        Dim [in] As String = args - "/imports"
        Return Installer.Install([in], args.GetBoolean("/refresh")).CLICode
    End Function

    <ExportAPI("-imports", Usage:="-imports <genbank_file/genbank_directory>")>
    Public Function [Imports](argvs As CommandLine.CommandLine) As Integer
        Return RQL.API.ImportsGBK(argvs.Parameters.First).CLICode
    End Function

    <ExportAPI("--install-COGs",
               Info:="Install the COGs database into the GCModeller database.",
               Usage:="--install-COGs /COGs <Dir.COGs>")>
    Public Function InstallCOGs(args As CommandLine.CommandLine) As Integer
        Dim COGsDir As String = args("/COGs")
        Dim protFasta As String = FileIO.FileSystem.GetFiles(COGsDir,
                                                             FileIO.SearchOption.SearchAllSubDirectories,
                                                             "*.fasta", "*.fa", "*.fsa").FirstOrDefault
        If String.IsNullOrEmpty(protFasta) Then
            Call $"Could not found any fasta file from directory {COGsDir}".__DEBUG_ECHO
            Return -1
        Else
            Call $"{protFasta.ToFileURL} was found!".__DEBUG_ECHO
        End If

        If String.IsNullOrEmpty(GCModeller.FileSystem.RepositoryRoot) Then
            Settings.SettingsFile.RepositoryRoot = App.HOME & "/Repository/"
        End If

        COGsDir = GCModeller.FileSystem.RepositoryRoot & "/COGs/bbh/"
        Call $"COGs fasta database will be installed at location {COGsDir}".__DEBUG_ECHO
        Return SMRUCC.genomics.Assembly.NCBI.COG.COGs.SaveRelease(protFasta, COGsDir).CLICode
    End Function

    <ExportAPI("--install-CDD", Usage:="--install-CDD /cdd <cdd.DIR>")>
    Public Function InstallCDD(args As CommandLine.CommandLine) As Integer
        Dim Repository As String = GCModeller.FileSystem.RepositoryRoot
        Repository &= "/CDD/"
        Dim buildFrom As String = args("/cdd")
        Call SMRUCC.genomics.Assembly.NCBI.CDD.DbFile.BuildDb(buildFrom, Repository)
        Return True
    End Function
End Module