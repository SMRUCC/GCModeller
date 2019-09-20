#Region "Microsoft.VisualBasic::820a51dab8a04f9831ecfa2bd3f5cd34, CLI_tools\metaProfiler\CLI\02_Alpha_Diversity.vb"

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
    '     Function: Rank_Abundance
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Analysis.Metagenome

Partial Module CLI

    <ExportAPI("/Rank_Abundance")>
    <Description("https://en.wikipedia.org/wiki/Rank_abundance_curve")>
    <Usage("/Rank_Abundance /in <OTU.table.csv> [/schema <color schema, default=Rainbow> /out <out.DIR>]")>
    <Group(CLIGroups.Alpha_Diversity)>
    Public Function Rank_Abundance(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim schema$ = args.GetValue("/schema", "Rainbow")
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".Rank_Abundance/")
        Dim OTUs As OTUTable() = OTUTable.LoadSample([in])
        Dim rankAbundance = OTUs.RankAbundance

        Call rankAbundance.SaveTo(out & "/Rank_Abundance.csv")

        Return 0
    End Function
End Module
