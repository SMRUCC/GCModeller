#Region "Microsoft.VisualBasic::2d092b5e21ad607b41aa69cbca0b9035, ..\GCModeller\annotations\Proteomics\LabelFree\LabelFreeTtest.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports RDotNET.Extensions.VisualBasic.API
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
    ''' <returns></returns>
    <Extension>
    Public Function logFCtest(data As IEnumerable(Of DataSet),
                              analysis As AnalysisDesigner,
                              sampleInfo As SampleInfo(),
                              Optional level# = 1.5,
                              Optional pvalue# = 0.05,
                              Optional fdrThreshold# = 0.05) As DEP_iTraq()

        Dim experiment$() = sampleInfo.TakeGroup(analysis.Treatment).SampleNames
        Dim controls$() = sampleInfo.TakeGroup(analysis.Controls).SampleNames
        Dim allSamples$() = experiment.AsList + controls

        ' calc the different expression proteins
        Dim calc = data _
            .Where(Function(d)
                       Return Not d.Properties _
                           .Values _
                           .All(AddressOf IsNaNImaginary)  ' 当原始数据全部都是NaN的时候，R会出错，在这里直接忽略掉这些原始样本
                   End Function) _
            .Select(Function(protein)
                        Dim FC# = protein(experiment).Average / protein(controls).Average
                        Dim log2FC# = Math.Log(FC, newBase:=2)
                        Dim p_value# = stats.Ttest(
                            protein(experiment),
                            protein(controls),
                            varEqual:=True).pvalue

                        Dim DEP As New DEP_iTraq With {
                            .ID = protein.ID,
                            .FCavg = FC,
                            .log2FC = log2FC,
                            .pvalue = p_value,
                            .Properties = protein _
                                .SubSet(allSamples) _
                                .Properties _
                                .AsCharacter
                        }

                        Return DEP
                    End Function) _
            .ApplyDEPFilter(level, pvalue, fdrThreshold)

        Return calc
    End Function
End Module

