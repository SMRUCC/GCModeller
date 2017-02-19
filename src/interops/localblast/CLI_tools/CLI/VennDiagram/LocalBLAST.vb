#Region "Microsoft.VisualBasic::9591af6faca75d05e29c46fed4b34091, ..\interops\localblast\CLI_tools\CLI\VennDiagram\LocalBLAST.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService

Partial Module CLI

    <ExportAPI("blast", Info:="In order to draw as venn diagram for a specific set of genome and study the diferrence and consists between " &
                                  "these genomes, you should do the blast operation from the protein amino aciad sequence first. The blastp " &
                                  "operation can be performenced by the blast+ program which you can download from the NCBI website, this command " &
                                  "is a interop service for the NCBI blast program， you should install the blast+ program at first.",
        Usage:="blast -i <file_directory> -blast_bin <BLAST_program_directory> -program <program_type_name> [-ld <log_dir> -xld <xml_log_dir>]",
        Example:="blast -i /home/xieguigang/GCModeller/examples/blast_test/ -ld ~/Desktop/logs -xld ~/Desktop/xmls -blast_bin ~/BLAST/bin")>
    <Argument("-i", False,
        Description:="The input data directory which is contains the FASTA format protein amino acid sequence data file.",
        Example:="~/Desktop/8004")>
    <Argument("-xld", True,
        Description:="Optional, the parsed and well organized blast log file output directory, if this switch value is not specific by " &
                     "the user then the user desktop directoy will be used as default.",
        Example:="~/Desktop/xml_logs")>
    <Argument("-ld", True,
        Description:="Optional, the blastp log file output directory for the NCBI blast+ program. If this switch value is not specific " &
                     "by the user then the user desktop directory will be specific for the logs file output as default.",
        Example:="~/Desktop/logs/")>
    <Argument("-blast_bin", False,
        Description:="The localtion for the blast+ program, you should specific this switch value or this program will throw an exception.",
        Example:="~/BLAST/bin")>
    <Argument("-program", False,
        Description:="The program type name for the NCBI local blast executable assembly.",
        Example:="blast+")>
    Public Function BLASTA(args As CommandLine) As Integer
        Dim FileDir As String = args("-i")
        Dim LogDir As String = args("-ld")
        Dim XmlLDir As String = args("-xld")
        Dim BlastBin As String = args("-blast_bin")
        Dim Program As String = args("-program")

        If String.IsNullOrEmpty(FileDir) OrElse Not FileIO.FileSystem.DirectoryExists(FileDir) Then
            printf("Could not found the input data directory, operation was unable to continute.")
            Return -1
        End If
        If String.IsNullOrEmpty(LogDir) Then
            LogDir = My.Computer.FileSystem.SpecialDirectories.Desktop & "/blast_logs/"
        End If
        If String.IsNullOrEmpty(XmlLDir) Then
            XmlLDir = My.Computer.FileSystem.SpecialDirectories.Desktop & "/blast_xml_logs/"
        End If
        If String.IsNullOrEmpty(BlastBin) Then
            printf("Unable to load BLAST program kernel as you did not spepcific the blast bin directory switch '-blast_bin'")
            Return -1
        End If

        Dim p As InitializeParameter = New InitializeParameter(BlastBin, Type:=GetProgram(Program))

        Call FileIO.FileSystem.CreateDirectory(XmlLDir)
        Call FileIO.FileSystem.CreateDirectory(LogDir)

        Dim Result As New LogsPair With {
            .Logs = BLAST(
                    FileIO.FileSystem.GetFiles(FileDir, FileIO.SearchOption.SearchTopLevelOnly, "*.txt", "*.fsa", "*.fasta").ToArray,
                    LogDir, p).ToArray,
            .LogsDir = LogDir
        }
        Dim FileText = Result.GetXml  '将结果文件保存至日志文件夹
        Call FileIO.FileSystem.WriteAllText(LogsPair.GetFileName(LogDir), FileText, append:=False)

        Result = ToXml(Result, XmlLDir)
        FileText = Result.GetXml
        Call FileIO.FileSystem.WriteAllText(LogsPair.GetXmlFileName(XmlLogsDir:=XmlLDir), FileText, append:=False)

        Return 0
    End Function

    Private Function ToXml(FileList As LogsPair, XmlLogsDir As String) As LogsPair
        Dim ReturnedPair As List(Of QueryPair()) = New List(Of QueryPair())

        For Each Pairs As QueryPair() In FileList.Logs
            Dim TEMPList As List(Of QueryPair) = New List(Of QueryPair)

            For Each Pair In Pairs
                Pair.Query = ToXml(Pair.Query, XmlLogsDir)
                Pair.Target = ToXml(Pair.Target, XmlLogsDir)
                Call TEMPList.Add(Pair)
            Next

            Call ReturnedPair.Add(TEMPList.ToArray)
        Next

        Return New LogsPair With {
            .Logs = ReturnedPair.ToArray,
            .LogsDir = XmlLogsDir
        }
    End Function

    Private Function ToXml(LogFile As String, SavedDir As String) As String
        Dim Xml As Legacy.BLASTOutput = Legacy.BLASTOutput.TryParse(LogFile)
        Dim File As String = String.Format("{0}/{1}.xml", SavedDir, FileIO.FileSystem.GetName(LogFile))
        Call Xml.Save(File)

        Return File
    End Function
End Module
