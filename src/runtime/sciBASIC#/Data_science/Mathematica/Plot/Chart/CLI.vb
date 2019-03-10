﻿#Region "Microsoft.VisualBasic::584f02c0e696c6e3b28b322d4e0a5310, Data_science\Mathematica\Plot\Chart\CLI.vb"

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
    '     Function: KMeansCluster, Scatter
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize
Imports Microsoft.VisualBasic.DataMining.KMeans

Module CLI

    <ExportAPI("/Scatter")>
    <Usage("/Scatter /in <data.csv> /x <fieldX> /y <fieldY> [/label.X <labelX> /label.Y <labelY> /color <default=black> /out <out.png>]")>
    <Description("")>
    Public Function Scatter(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim fx$ = args <= "/x"
        Dim fy$ = args <= "/y"
        Dim labelX$ = args("/label.X") Or fx
        Dim labelY$ = args("/label.Y") Or fy
        Dim color$ = args("/color") Or "black"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}_[{fx.NormalizePathString},{fy.NormalizePathString}].png"
        Dim csv = DataSet.LoadDataSet([in]).ToArray

        Throw New NotImplementedException
    End Function

    <ExportAPI("/kmeans")>
    <Usage("/kmeans /in <matrix.csv> [/n <expected_cluster_numbers, default=3> /out <clusters.csv>]")>
    <Group("Data tools")>
    Public Function KMeansCluster(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim n% = args("/n") Or 5
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.kmeans.csv"
        Dim data As DataSet() = DataSet.LoadDataSet([in]).ToArray
        Dim clusters As IEnumerable(Of EntityClusterModel) =
            data _
            .ToKMeansModels _
            .Kmeans(expected:=n)

        Return clusters.SaveTo(out).CLICode
    End Function
End Module
