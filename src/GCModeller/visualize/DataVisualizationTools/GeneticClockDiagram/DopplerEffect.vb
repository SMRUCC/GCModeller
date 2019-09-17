#Region "Microsoft.VisualBasic::b86b4058171a1d6f5e4918bdffe99ba2, visualize\DataVisualizationTools\GeneticClockDiagram\DopplerEffect.vb"

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

    '     Class DopplerEffect
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CalculateDopplerEffect
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.SignalProcessing.Serials.PeriodAnalysis
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace GeneticClock

    Public Class DopplerEffect

        ReadOnly _source As SerialsVarialble()

        ''' <summary>
        ''' 首先使用实验数据，使用数据挖掘工具包之中的周期分析工具分析出周期数据
        ''' 在计算频率的变化趋势
        ''' 最后再次生成<see cref="NumericVector"></see>绘制遗传时钟图
        ''' </summary>
        ''' <param name="ExperimentData"></param>
        ''' <remarks></remarks>
        Sub New(ExperimentData As NumericVector())
            _source = (From item In ExperimentData Select New SerialsVarialble With {.Identifier = item.name, .SerialsData = item.vector}).ToArray  '执行数据转换
        End Sub

        Public Function CalculateDopplerEffect(lstLocus As IEnumerable(Of String), Optional WindowSize As Integer = 6) As NumericVector()
            Dim Data = (From strId As String
                        In lstLocus
                        Select UniqueId = strId,
                            Sample = _source.Analysis(strId, WindowSize))        '得到周期数据
            Dim DopplerEffects = (From Sample In Data
                                  Select New NumericVector With {
                                      .name = Sample.UniqueId,
                                      .vector = (From item In Sample.Sample.TSerials Select item.value).ToArray}).AsList   '计算出其中的周期变化
            Dim init As New NumericVector With {
                .name = "Time",
                .vector = (From n As Integer
                                In DopplerEffects.First.vector.Sequence
                           Select CDbl(n)).ToArray
            }
            Call DopplerEffects.Insert(0, init)
            Return DopplerEffects.ToArray
        End Function
    End Class
End Namespace
