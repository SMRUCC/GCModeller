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

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Repository.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Install.genbank", Usage:="/Install.genbank /imports <all_genbanks.DIR> [/refresh]")>
    Public Function InstallGenbank(args As CommandLine) As Integer
        Dim [in] As String = args - "/imports"
        Return Installer.Install([in], args.GetBoolean("/refresh")).CLICode
    End Function

    '<ExportAPI("-imports", Usage:="-imports <genbank_file/genbank_directory>")>
    'Public Function [Imports](argvs As CommandLine) As Integer
    '    Return RQL.API.ImportsGBK(argvs.Parameters.First).CLICode
    'End Function

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

    <ExportAPI("--install.ncbi_nt", Usage:="--install.ncbi_nt /nt <nt.fasta> [/EXPORT <DATA_dir>]")>
    Public Function Install_NCBI_nt(args As CommandLine) As Integer
        Dim nt As String = args("/nt")
        Dim EXPORT$ = args.GetValue("/EXPORT", nt.TrimSuffix & "-$DATA/")
        Dim mysql As MySQL = Nothing

        Call mysql.[Imports](nt, EXPORT, False)

        Return 0
    End Function

    <ExportAPI("/nt.repository.query", Usage:="/nt.repository.query /query <arguments.csv> /DATA <DATA_dir> [/out <out_DIR>]")>
    <ParameterInfo("/query", AcceptTypes:={GetType(QueryArgument)})>
    Public Function ntRepositoryExports(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim DATA As String = args("/DATA")
        Dim out As String = args.GetValue("/out", query.TrimSuffix & "-EXPORT/")
        Dim repo As New QueryEngine

        Call repo.ScanSeqDatabase(DATA)

        For Each x In query.LoadCsv(Of QueryArgument)
            Dim path$ = $"{out}/{x.Name.NormalizePathString}.fasta"

            Call path.__DEBUG_ECHO

            Using fasta As StreamWriter = path.OpenWriter(Encodings.ASCII)
                For Each result As FastaToken In repo.Search(query:=x.Expression$)
                    Call fasta.WriteLine(result.GenerateDocument(120))
                    Call Console.Write(".")
                Next
            End Using
        Next

        Return 0
    End Function

    Public Class QueryArgument
        Public Property Name As String
        Public Property Expression As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Module
