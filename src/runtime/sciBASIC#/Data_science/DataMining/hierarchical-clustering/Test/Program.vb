﻿#Region "Microsoft.VisualBasic::583d38cd65c9661433bbf1ef70cf6744, Data_science\DataMining\hierarchical-clustering\Test\Program.vb"

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

    ' Module Program
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: createSampleCluster
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering.DendrogramVisualize
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program

    Sub New()
        ' VBDebugger.Mute = True
    End Sub

    Public Sub Main()
        Dim cluster As Cluster = createSampleCluster()
        Dim dp As New DendrogramPanel With {
            .LineColor = Color.Blue,
            .ScaleValueDecimals = 0,
            .ScaleValueInterval = 1,
            .Model = cluster,
            .ClassTable = New Dictionary(Of String, String) From {
                {"O1", "green"},
                {"O2", "green"},
                {"O3", "blue"},
                {"O4", "yellow"},
                {"O5", "red"},
                {"O6", "red"}
            }
        }

        Using g As Graphics2D = New Size(1024, 768).CreateGDIDevice(filled:=Color.White)
            Call dp.Paint(g, New Rectangle(300, 100, 500, 500), layout:=Layouts.Vertical).GetJson(True).__DEBUG_ECHO
            Call g.Save("../../dendrogram-visualize-vertical.png", ImageFormats.Png)
        End Using

        Using g As Graphics2D = New Size(1024, 768).CreateGDIDevice(filled:=Color.White)

            dp.ShowScale = False
            dp.ShowDistanceValues = False
            dp.Debug = True

            Call dp.Paint(g, New Rectangle(100, 100, 800, 600), layout:=Layouts.Horizon).GetJson(True).__DEBUG_ECHO
            Call g.Save("../../dendrogram-visualize-horizon.png", ImageFormats.Png)
        End Using

        Pause()
    End Sub

    Private Function createSampleCluster() As Cluster
        Dim distances = {
            {0#, 1, 9, 7, 11, 14},
            {1, 0, 4, 3, 8, 10},
            {9, 4, 0, 9, 2, 8},
            {7, 3, 9, 0, 6, 13},
            {11, 8, 2, 6, 0, 10},
            {14, 10, 8, 13, 10, 0}
        }
        Dim names$() = {"O1", "O2", "O3", "O4", "O5", "O6"}
        Dim alg As ClusteringAlgorithm = New DefaultClusteringAlgorithm
        Dim cluster As Cluster = alg.performClustering(
            distances.RowIterator.ToArray,
            names,
            New AverageLinkageStrategy)

        Call cluster.Print()

        Return cluster
    End Function
End Module
