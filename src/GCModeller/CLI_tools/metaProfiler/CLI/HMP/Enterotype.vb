#Region "Microsoft.VisualBasic::a3bff06e396939f7a0e814f83b7c1e13, CLI_tools\metaProfiler\CLI\HMP\Enterotype.vb"

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
    '     Function: DoEnterotypeCluster
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.Math.DataFrame.Impute
Imports SMRUCC.genomics.Analysis.Metagenome

Partial Module CLI

    <ExportAPI("/do.enterotype.cluster")>
    <Description("")>
    <Usage("/do.enterotype.cluster /in <dataset.csv/txt> [/iterations 50000 /parallel /out <clusters.csv>]")>
    Public Function DoEnterotypeCluster(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim iterations% = args("/iterations") Or 50000
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.DoEnterotypeCluster.csv"
        Dim data As DataSet() = DataSet _
            .LoadDataSet([in], tsv:=[in].ExtensionSuffix.TextEquals("txt")) _
            .SimulateMissingValues(byRow:=True, infer:=InferMethods.Min) _
            .ToArray
        Dim parallel As Boolean = args("/parallel")
        Dim result = data.JSD(parallel:=parallel).PAMclustering

        Return result.SaveTo(out).CLICode
    End Function
End Module
