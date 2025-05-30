﻿#Region "Microsoft.VisualBasic::019165b176d8514956f66225a72a6821, annotations\WGCNA\test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module Module1

    Sub Main()
        Dim raw As Matrix = Matrix.LoadData("\\192.168.1.254\backup3\项目以外内容\测试项目\空间V2.1\FULL_MS_centriod_CHCA_20210819\20241220\MSI_analysis\tmp\workflow_tmp\exportPeaktable\region_samples.csv")
        Dim out = WGCNA.Analysis.Run(raw)

        Call out.hclust.Plot(layout:=Layouts.Horizon).Save("\GCModeller\src\GCModeller\annotations\WGCNA\run_test\metabolome.png")
        Call out.modules.GetJson.SaveTo("\GCModeller\src\GCModeller\annotations\WGCNA\run_test\metabolome.json")
        Call out.network.Tabular.Save("\GCModeller\src\GCModeller\annotations\WGCNA\run_test\metabolome")

        Pause()
    End Sub

End Module
