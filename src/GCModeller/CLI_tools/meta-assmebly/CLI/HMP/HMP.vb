#Region "Microsoft.VisualBasic::e9d7521df3b5d67e25fa1cb9961dcf56, ..\GCModeller\CLI_tools\meta-assmebly\CLI\HMP\HMP.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.Repository.NIH.HMP

Partial Module CLI

    <ExportAPI("/handle.hmp.manifest")>
    <Usage("/handle.hmp.manifest /in <manifest.tsv> [/out <save.directory>]")>
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
End Module

