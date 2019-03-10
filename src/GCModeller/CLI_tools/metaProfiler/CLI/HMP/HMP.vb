﻿#Region "Microsoft.VisualBasic::d224fce2caa0baa183c86607d2b13eda, CLI_tools\meta-assmebly\CLI\HMP\HMP.vb"

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
'     Function: Download16sSeq, ExportFileList
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Data.Repository.NIH.HMP

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
End Module
