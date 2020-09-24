#Region "Microsoft.VisualBasic::f20d7281cc169f58cab07a62c787497f, CLI_tools\metaProfiler\CLI\HMP\HMP.vb"

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

    ' Module CLI
    ' 
    '     Function: Download16sSeq, ExportFileList, ExportsOTUTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Data.Repository.NIH.HMP
Imports SMRUCC.genomics.foundation.BIOM.v10
Imports BIOM = SMRUCC.genomics.foundation.BIOM

Partial Module CLI

    ''' <summary>
    ''' 进行文件下载操作
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/handle.hmp.manifest")>
    <Usage("/handle.hmp.manifest /in <manifest.tsv> [/out <save.directory>]")>
    <Description("Download files from HMP website through http/fasp.")>
    <Group(CLIGroups.HMP_cli)>
    Public Function Download16sSeq(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = (args <= "/out") Or $"{[in].TrimSuffix}/".AsDefault

        Return manifest _
            .LoadTable([in]) _
            .HandleFileDownloads(save:=out) _
            .ToArray _
            .SaveTo(out & "/HMP_client.log", Encoding.UTF8) _
            .CLICode
    End Function

    <ExportAPI("/hmp.manifest.files")>
    <Usage("/hmp.manifest.files /in <manifest.tsv> [/out <list.txt>]")>
    <Group(CLIGroups.HMP_cli)>
    Public Function ExportFileList(args As CommandLine) As Integer

        VBDebugger.ForceSTDError = True

        Dim tsv$ = args.OpenStreamInput("/in").ReadToEnd
        Dim manifest As manifest() = tsv.ImportsData(Of manifest)(delimiter:=ASCII.TAB)
        Dim list$ = manifest _
            .Select(Function(sample) sample.HttpURL) _
            .Where(Function(url) Not url.StringEmpty) _
            .Distinct _
            .JoinBy(ASCII.LF)

        Using out = args.OpenStreamOutput("/out")
            Call out.WriteLine(list)
        End Using

        Return 0
    End Function

    <ExportAPI("/hmp.otu_table")>
    <Usage("/hmp.otu_table /in <download.directory> [/out <out.csv>]")>
    <Description("Export otu table from hmp biom files.")>
    <Argument("/in", False, CLITypes.File,
              Description:="A directory contains the otu BIOM files which is download by ``/handle.hmp.manifest`` command.")>
    Public Function ExportsOTUTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.otu_table.csv"
        Dim matrix As DataSet() = Iterator Function() As IEnumerable(Of BIOMDataSet(Of Double))
                                      For Each biomHdf5 As String In ls - l - r - "*.biom" <= [in]
                                          Yield BIOM.ReadAuto(biomHdf5, denseMatrix:=True)
                                      Next
                                  End Function().Union().ToArray
        Return matrix _
            .SaveTo(out, metaBlank:=0) _
            .CLICode
    End Function
End Module
