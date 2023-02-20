#Region "Microsoft.VisualBasic::b40b48a5d2bef1852cba560ae47564cc, GCModeller\annotations\Proteomics\LabelFree\LabelFreeTtest.vb"

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


    ' Code Statistics:

    '   Total Lines: 114
    '    Code Lines: 87
    ' Comment Lines: 18
    '   Blank Lines: 9
    '     File Size: 4.66 KB


    ' Module LabelFreeTtest
    ' 
    '     Function: logFCtest, significantA, ttest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' avg(A)/avg(B) = FC
''' </summary>
Public Module LabelFreeTtest

    ''' <summary>
    ''' 一次只计算出一组实验设计的结果
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="analysis"></param>
    ''' <param name="level#"></param>
    ''' <param name="pvalue#"></param>
    ''' <param name="fdrThreshold#"></param>
    ''' <param name="significantA">
    ''' 是否使用<see cref="SignificanceAB.SignificanceA(Vector)"/>算法来计算pvalue值? 默认是使用T检验
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function logFCtest(data As IEnumerable(Of DataSet),
                              analysis As AnalysisDesigner,
                              sampleInfo As SampleGroup(),
                              Optional level# = 1.5,
                              Optional pvalue# = 0.05,
                              Optional fdrThreshold# = 0.05,
                              Optional significantA As Boolean = False) As DEP_iTraq()

        Dim experiment$() = sampleInfo.TakeGroup(analysis.treatment).SampleNames
        Dim controls$() = sampleInfo.TakeGroup(analysis.controls).SampleNames
        Dim allSamples$() = experiment.AsList + controls

        ' calc the different expression proteins
        Dim rmNA = data _
            .Where(Function(d)
                       ' 当原始数据全部都是NaN的时候，R会出错，在这里直接忽略掉这些原始样本
                       Return Not d.Properties _
                           .Values _
                           .IsNaNImaginary _
                           .All
                   End Function) _
            .Where(Function(d)
                       ' 所有结果都是零的蛋白也都剔除掉
                       Return Not d.Properties _
                           .Values _
                           .All(Function(x) x = 0R)
                   End Function) _
            .ToArray

        If significantA Then
            Return rmNA _
                .significantA(experiment, controls, allSamples) _
                .ApplyDEPFilter(level, pvalue, fdrThreshold)
        Else
            Return rmNA _
                .ttest(experiment, controls, allSamples) _
                .ApplyDEPFilter(level, pvalue, fdrThreshold)
        End If
    End Function

    <Extension>
    Private Iterator Function ttest(data As IEnumerable(Of DataSet), experiment$(), controls$(), allSamples$()) As IEnumerable(Of DEP_iTraq)
        For Each protein As DataSet In data
            Dim foldChange# = protein(experiment).Average / protein(controls).Average
            Dim log2FC# = Math.Log(foldChange, newBase:=2)
            Dim tResult = t.Test(
                protein(experiment),
                protein(controls),
                varEqual:=True
            )

            Yield New DEP_iTraq With {
                .ID = protein.ID,
                .foldchange = foldChange,
                .log2FC = log2FC,
                .pvalue = tResult.Pvalue,
                .Properties = protein _
                    .SubSet(allSamples) _
                    .Properties _
                    .AsCharacter
            }
        Next
    End Function

    <Extension>
    Private Iterator Function significantA(data As IEnumerable(Of DataSet), experiment$(), controls$(), allSamples$()) As IEnumerable(Of DEP_iTraq)
        Dim proteins As DataSet() = data.ToArray
        Dim foldChanges As Vector = proteins _
            .Select(Function(protein)
                        Return protein(experiment).Average / protein(controls).Average
                    End Function) _
            .AsVector
        Dim pvalue As Vector = SignificanceAB.SignificanceA(ratio:=foldChanges)
        Dim log2FC As Vector = foldChanges.Log(base:=2)

        For i As Integer = 0 To proteins.Length - 1
            Yield New DEP_iTraq With {
                .ID = proteins(i).ID,
                .foldchange = foldChanges(i),
                .log2FC = log2FC(i),
                .pvalue = pvalue(i),
                .Properties = proteins(i) _
                    .SubSet(allSamples) _
                    .Properties _
                    .AsCharacter
            }
        Next
    End Function
End Module
