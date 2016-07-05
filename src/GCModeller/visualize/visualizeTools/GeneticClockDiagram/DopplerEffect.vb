#Region "Microsoft.VisualBasic::674ee24dc2f147aa044239e7a8f27068, ..\GCModeller\visualize\visualizeTools\GeneticClockDiagram\DopplerEffect.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Drawing
Imports SMRUCC.genomics.InteractionModel
Imports Microsoft.VisualBasic.DataMining.Framework.Serials.PeriodAnalysis
Imports Microsoft.VisualBasic

Namespace GeneticClock

    Public Class DopplerEffect

        ReadOnly _source As SerialsVarialble()

        ''' <summary>
        ''' 首先使用实验数据，使用数据挖掘工具包之中的周期分析工具分析出周期数据
        ''' 在计算频率的变化趋势
        ''' 最后再次生成<see cref="SerialsData"></see>绘制遗传时钟图
        ''' </summary>
        ''' <param name="ExperimentData"></param>
        ''' <remarks></remarks>
        Sub New(ExperimentData As SerialsData())
            _source = (From item In ExperimentData Select New SerialsVarialble With {.Identifier = item.Tag, .SerialsData = item.ChunkBuffer}).ToArray  '执行数据转换
        End Sub

        Public Function CalculateDopplerEffect(lstLocus As IEnumerable(Of String), Optional WindowSize As Integer = 6) As SerialsData()
            Dim Data = (From strId As String
                        In lstLocus
                        Select UniqueId = strId,
                            Sample = _source.Analysis(strId, WindowSize))        '得到周期数据
            Dim DopplerEffects = (From Sample In Data
                                  Select New SerialsData With {
                                      .Tag = Sample.UniqueId,
                                      .ChunkBuffer = (From item In Sample.Sample.TSerials Select item.Value).ToArray}).ToList   '计算出其中的周期变化
            Dim init As New SerialsData With {
                .Tag = "Time",
                .ChunkBuffer = (From n As Integer
                                In DopplerEffects.First.ChunkBuffer.Sequence
                                Select CDbl(n)).ToArray
            }
            Call DopplerEffects.Insert(0, init)
            Return DopplerEffects.ToArray
        End Function
    End Class
End Namespace
