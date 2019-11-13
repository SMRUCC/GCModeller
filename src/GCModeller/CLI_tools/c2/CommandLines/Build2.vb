#Region "Microsoft.VisualBasic::00a84350d8845abaed8424db0cb353ec, CLI_tools\c2\CommandLines\Build2.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    ' Module CommandLines
    ' 
    '     Function: Reconstruct
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.consoledevice.STDIO

Partial Module CommandLines

    ''' <summary>
    ''' Reconstruct a metacyc database for a specific specie bacteria base on the homologous model data.(根据同源模型进行目标物种的MetaCyc数据库的重构操作)
    ''' </summary>o
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("build2",
        Info:="Reengineering the genetic clock model using the exists metacy database schema model and the local blast computational method.",
        Usage:="build2 -m <dir> -o <file> -t <dir> -grep_m <regx pattern> -grep_t <regx pattern> [-f <gbk/fsa/metacyc>] [-compile_export <export_model_file>]",
        Example:="build2 -m /home/xieguigang/metacyc/8004/data/ -o /home/xieguigang/project/8004.xml -t /home/xieguigang/metacyc/mynx/data/ -grep_m metacyc-uid -grep_t metacyc-uid -f gbk")>
    <ParameterInfo("-rct", Description:="The data source of the modelling target, if can be a fasta sequence data file path or a metacyc database directory.", Example:="/home/xieguigang/metacyc/8004/data/")>
    <ParameterInfo("-subject", Description:="The data directory of the target metacyc database which use as a homologous model for the reconstruction modelling.", Example:="/home/xieguigang/metacyc/mynx/data/")>
    <ParameterInfo("-grep_m",
        Description:="A regular expression for grep the protein name of the modelling query object from the blast log file." & vbCrLf &
                     "metacyc-uid  - The program will use the metacyc database unique-id property to grep the protein id, if you not sure for the your input expression will work or not, please use this value." & vbCrLf &
                     "<input_regx> - The program will use the user input regular expression to grep the protein id from the log file.",
        Example:="metacyc-uid")>
    <ParameterInfo("-grep_t",
        Description:="A regular expression for grep the protein name of the target homologous metacyc database from the blast log file." & vbCrLf &
                     "metacyc-uid  - The program will use the metacyc database unique-id property to grep the protein id, if you not sure for the your input expression will work or not, please use this value." & vbCrLf &
                     "<input_regx> - The program will use the user input regular expression to grep the protein id from the log file.",
        Example:="/home/xieguigang/metacyc/8004/data/")>
    <ParameterInfo("-export", True,
        Description:="Optional, The output directory for the reconstructed metacyc database." & vbCrLf &
        " - If not specific than the reconstructed data will write to the input position that the -m switch specific;" & vbCrLf &
        " - Or when you specific this parameter then the data will write to the location that you input in the commandline.",
        Example:="/home/xieguigang/metacyc/reconstruct-8004/data/")>
    <ParameterInfo("-f", True,
        Description:="Optional, The data source format of the modelling target." & vbCrLf &
                     "gbk     - for ncbi genbank database file;" & vbCrLf &
                     "fsa     - for general fasta sequence data file, it must be a protein sequence file;" & vbCrLf &
                     "metacyc - for metacyc database data directory, recommond for this format.",
        Example:="gbk")>
    <ParameterInfo("-compile_export", True,
        Description:="Optional, indicate that compile the reconstructed metacyc into the model file or not and the model file saved filename.",
        Example:="/home/xieguigang/Desktop/8004.xml")>
    Public Function Reconstruct(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim Reconstructed As String = CommandLine("-rct").Replace("\", "/")
        Dim Export As String = CommandLine("-export").Replace("\", "/")   '所重建出的目标模型文件的保存位置
        Dim Subject As String = CommandLine("-subject").Replace("\", "/")
        Dim CompileExport As String = CommandLine("-compile_export").Replace("\", "/")

        Using LogFile As Microsoft.VisualBasic.Logging.LogFile = New Logging.LogFile(Settings.LogDIR & "/C2.log")

            Call IORedirect(LogFile)

            Printf("[c2.exe, version %s]\n  c2 compiler, the genetic clock model alternative compiler\n", My.Application.Info.Version.ToString)
            Printf("Initialize a session for reconstructe the target MetaCyc database...")

            Using ReconstructOperation As New c2.Reconstruction.Reconstruction
                Call ReconstructOperation.Invoke(Reconstructed, Subject, WorkDir:=Settings.TEMP, ReconstructedExport:=Export)
                If Not String.IsNullOrEmpty(CompileExport) Then
                    Call ReconstructOperation.Compile(CompileExport, Settings.Session.LogDIR & "/C2.log")
                End If
            End Using
        End Using

        Return 0
    End Function
End Module
