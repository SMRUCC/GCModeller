#Region "Microsoft.VisualBasic::beaca91d1e0dfc56c090dc6523cc3992, ..\GCModeller\CLI_tools\GCModeller\CLI\Repository.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Model_Repository

Partial Module CLI

    <ExportAPI("/Install.genbank", Usage:="/Install.genbank /imports <all_genbanks.DIR> [/refresh]")>
    Public Function InstallGenbank(args As CommandLine) As Integer
        Dim [in] As String = args - "/imports"
        Return Installer.Install([in], args.GetBoolean("/refresh")).CLICode
    End Function

    <ExportAPI("-imports", Usage:="-imports <genbank_file/genbank_directory>")>
    Public Function [Imports](argvs As CommandLine) As Integer
        Return RQL.API.ImportsGBK(argvs.Parameters.First).CLICode
    End Function

    <ExportAPI("--install-COGs",
               Info:="Install the COGs database into the GCModeller database.",
               Usage:="--install-COGs /COGs <Dir.COGs>")>
    Public Function InstallCOGs(args As CommandLine) As Integer
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
    Public Function InstallCDD(args As CommandLine) As Integer
        Dim Repository As String = GCModeller.FileSystem.RepositoryRoot
        Repository &= "/CDD/"
        Dim buildFrom As String = args("/cdd")
        Call SMRUCC.genomics.Assembly.NCBI.CDD.DbFile.BuildDb(buildFrom, Repository)
        Return True
    End Function
End Module
