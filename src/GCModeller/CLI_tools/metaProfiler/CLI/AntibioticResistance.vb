﻿#Region "Microsoft.VisualBasic::cedc180903f7cf55ff88c65b86045215, CLI_tools\meta-assmebly\CLI\AntibioticResistance.vb"

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
'     Function: AROSeqTable
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction

Partial Module CLI

    <ExportAPI("/ARO.fasta.header.table")>
    <Usage("/ARO.fasta.header.table /in <directory> [/out <out.csv>]")>
    <Group(CLIGroups.AntibioticResistance_cli)>
    Public Function AROSeqTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.ARO_fasta.headers.csv"
        Dim table = CARDdata.FastaParser([in]).ToArray

        Call table.Keys.Distinct.FlushAllLines(out.TrimSuffix & ".accession.list")
        Return table.SaveTo(out).CLICode
    End Function
End Module
