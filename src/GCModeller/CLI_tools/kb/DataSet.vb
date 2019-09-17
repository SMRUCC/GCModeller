#Region "Microsoft.VisualBasic::ebf13e0e5acf835e7d2c4941e79f3e25, CLI_tools\kb\DataSet.vb"

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
    '     Function: KEGGCompoundDataSet, KEGGMapsBackground
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports RDotNet.Extensions.GCModeller
Imports SMRUCC.genomics.Data

Partial Module CLI

    <ExportAPI("/KEGG.compound.rda")>
    <Usage("/KEGG.compound.rda /repo <directory> [/out <save.rda>]")>
    <Description("Create a kegg organism-compound maps dataset and save in rda file.")>
    Public Function KEGGCompoundDataSet(args As CommandLine) As Integer
        Dim in$ = args <= "/repo"
        Dim dataset = OrganismCompounds.LoadData(repo:=[in])
        Dim out$ = args("/out") Or $"{[in].TrimDIR}/{dataset.code}.rda"

        Return dataset _
            .DoCall(Function(d) OrganismCompounds.WriteRda(d, rdafile:=out)) _
            .CLICode
    End Function

    <ExportAPI("/KEGG.maps.background")>
    <Usage("/KEGG.maps.background /in <reference_maps.directory> [/out <gsea_background.rda>]")>
    Public Function KEGGMapsBackground(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in]}/gsea_background.rda"

        Return PathwayRepository.ScanModels([in]) _
            .AsEnumerable _
            .SaveBackgroundRda(rdafile:=out) _
            .CLICode
    End Function
End Module
