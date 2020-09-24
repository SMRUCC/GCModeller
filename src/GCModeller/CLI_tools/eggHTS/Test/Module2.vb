#Region "Microsoft.VisualBasic::cf9234d8271ea6bb4b74405ab499df1b, CLI_tools\eggHTS\Test\Module2.vb"

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

    ' Module Module2
    ' 
    '     Sub: Main, plotTest, ptest
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Text
Imports RDotNet
Imports RDotNet.Extensions.VisualBasic.API
Imports RDotNet.Extensions.VisualBasic.RSystem
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize

Module Module2

    Sub ptest()

        Dim fc As Vector = rand(1000, {0, 4})
        Dim p = SignificanceA(fc)
        Dim pp As Vector

        SyncLock R
            With R
                .Evaluate("
get_significance <- function(ratio){
  ratio <- log2(as.numeric(ratio))
  order_ratio <- ratio[order(ratio)]
  quantiletmp <- quantile(order_ratio, c(0.1587,0.5,0.8413))
  rl <- as.numeric(quantiletmp[1])      #对应公式中的r-1
  rm <- as.numeric(quantiletmp[2])      #对应公式中的r0
  rh <- as.numeric(quantiletmp[3])      #对应公式中的r1
  p <- unlist(lapply(ratio, function(x){
    if (x > rm){
      z <- (x-rm)/(rh-rm)
      pnorm(z,lower.tail = F)
    }else{
      z <- (rm-x)/(rm-rl)
      pnorm(z,lower.tail = F)
    }
  }))
}
")
                pp = .Evaluate($"get_significance({base.c(fc)});").AsNumeric.ToArray
            End With
        End SyncLock

        Pause()
    End Sub

    Sub Main()
        ' Call ptest()
        '  Call matrixSplitTest()

        Pause()
    End Sub

    '   Sub matrixSplitTest()

    '       Dim maps = EntityObject.LoadDataSet("D:\Resources\sampleinfo.txt", tsv:=True).ToDictionary(Function(r) r.ID, Function(r) r!sample_name)
    '       Dim rawMatrix = DataSet.LoadDataSet("D:\Resources\DOG_proteinGroups20190129.csv", fieldNameMaps:=maps).SimulateMissingValuesBySample(Function(v) v.Average).TotalSumNormalize.ToArray
    '       Dim sampleInfo = {
    '           New SampleGroup With {.sample_group = "0d", .sample_name = "dog1-0"},
    '           New SampleGroup With {.sample_group = "0d", .sample_name = "dog2-0"},
    '           New SampleGroup With {.sample_group = "0d", .sample_name = "dog3-0"},
    '           New SampleGroup With {.sample_group = "0d", .sample_name = "dog4-0"},
    '           New SampleGroup With {.sample_group = "0d", .sample_name = "dog5-0"},
    '_
    '           New SampleGroup With {.sample_group = "1d", .sample_name = "dog1-1"},
    '           New SampleGroup With {.sample_group = "1d", .sample_name = "dog2-1"},
    '           New SampleGroup With {.sample_group = "1d", .sample_name = "dog3-1"},
    '           New SampleGroup With {.sample_group = "1d", .sample_name = "dog4-1"},
    '           New SampleGroup With {.sample_group = "1d", .sample_name = "dog5-1"},
    '_
    '           New SampleGroup With {.sample_group = "3d", .sample_name = "dog1-3"},
    '           New SampleGroup With {.sample_group = "3d", .sample_name = "dog2-3"},
    '           New SampleGroup With {.sample_group = "3d", .sample_name = "dog3-3"},
    '           New SampleGroup With {.sample_group = "3d", .sample_name = "dog4-3"},
    '           New SampleGroup With {.sample_group = "3d", .sample_name = "dog5-3"},
    '_
    '           New SampleGroup With {.sample_group = "7d", .sample_name = "dog1-7"},
    '           New SampleGroup With {.sample_group = "7d", .sample_name = "dog2-7"},
    '           New SampleGroup With {.sample_group = "7d", .sample_name = "dog3-7"},
    '           New SampleGroup With {.sample_group = "7d", .sample_name = "dog4-7"},
    '           New SampleGroup With {.sample_group = "7d", .sample_name = "dog5-7"},
    '_
    '           New SampleGroup With {.sample_group = "10d", .sample_name = "dog1-10"},
    '           New SampleGroup With {.sample_group = "10d", .sample_name = "dog2-10"},
    '           New SampleGroup With {.sample_group = "10d", .sample_name = "dog3-10"},
    '           New SampleGroup With {.sample_group = "10d", .sample_name = "dog4-10"},
    '           New SampleGroup With {.sample_group = "10d", .sample_name = "dog5-10"},
    '_
    '           New SampleGroup With {.sample_group = "14d", .sample_name = "dog1-14d"},
    '           New SampleGroup With {.sample_group = "14d", .sample_name = "dog3-14d"},
    '           New SampleGroup With {.sample_group = "14d", .sample_name = "dog4-14d"},
    '           New SampleGroup With {.sample_group = "14d", .sample_name = "dog5-14d"},
    '           New SampleGroup With {.sample_group = "14d", .sample_name = "dog2-14d"},
    '_
    '           New SampleGroup With {.sample_group = "21d", .sample_name = "dog1-21d"},
    '           New SampleGroup With {.sample_group = "21d", .sample_name = "dog2-21d"},
    '           New SampleGroup With {.sample_group = "21d", .sample_name = "dog3-21d"},
    '           New SampleGroup With {.sample_group = "21d", .sample_name = "dog4-21d"},
    '           New SampleGroup With {.sample_group = "21d", .sample_name = "dog5-21d"}
    '       }

    '       Dim analysis = {
    '        New AnalysisDesigner With {.Controls = "0d", .Treatment = "1d"},
    '         New AnalysisDesigner With {.Controls = "0d", .Treatment = "10d"},
    '          New AnalysisDesigner With {.Controls = "0d", .Treatment = "3d"},
    '           New AnalysisDesigner With {.Controls = "0d", .Treatment = "7d"},
    '            New AnalysisDesigner With {.Controls = "0d", .Treatment = "14d"},
    '             New AnalysisDesigner With {.Controls = "0d", .Treatment = "21d"}
    '      }

    '       For Each design In analysis
    '           Call Proteomics.LabelFreeTtest.logFCtest(rawMatrix, design, sampleInfo, significantA:=True, level:=1.25).SaveTo($"D:\Resources\{design.Title}.csv", Encodings.UTF8)
    '       Next


    '       'For Each analysisDesign In FoldChangeMatrix.iTraqMatrix(rawMatrix, sampleInfo, analysis, True)
    '       '    Call analysisDesign.SaveTo($"D:\test\HXB\{analysisDesign.Name}.csv")
    '       'Next
    '   End Sub

    Sub plotTest()
        Dim sample = EntityObject.LoadDataSet("G:\GCModeller\GCModeller\R\vocano\qlfTable.csv")
        Call Volcano.PlotDEGs(sample, pvalue:="PValue", displayLabel:=LabelTypes.DEG).Save("x:\test.png")
    End Sub
End Module
