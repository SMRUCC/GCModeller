#Region "Microsoft.VisualBasic::b839fb64ab5389edf311d41a7ed7fae7, visualize\DataVisualizationTools\NCBIBlastResult\ColorSchema.vb"

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

    '     Module ColorSchema
    ' 
    '         Function: BitScores, GetBlastnIdentitiesColor, GetColor, IdentitiesBrush, IdentitiesChromatic
    '                   IdentitiesMonoChrome
    '         Structure __brushHelper
    ' 
    '             Function: GetBrush
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models
Imports SMRUCC.genomics.Visualize.SyntenyVisualize.ComparativeGenomics.ModelAPI

Namespace NCBIBlastResult

    ''' <summary>
    ''' Blast结果之中的hit对象的颜色映射
    ''' </summary>
    Public Module ColorSchema

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scores">需要从这里得到分数</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function IdentitiesBrush(scores As Func(Of Hit, Double)) As ICOGsBrush
            Return AddressOf New __brushHelper With {
                .scores = scores,
                .colors = IdentitiesChromatic()
            }.GetBrush   ' 获取得到映射的颜色刷子的函数指针
        End Function

        Private Structure __brushHelper

            Public scores As Func(Of Hit, Double)
            Public colors As RangeList(Of Double, NamedValue(Of Color))

            Public Function GetBrush(gene As GeneBrief) As Brush
                Dim hit As New Hit With {.hitName = gene.Synonym}
                Dim score As Double = scores(hit)
                Dim color As Color = colors.GetColor(score)
                Return New SolidBrush(color)
            End Function
        End Structure

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p">p的值在0-1之间</param>
        ''' <param name="Schema"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function GetColor(schema As RangeList(Of Double, NamedValue(Of Color)), p As Double) As Color
            Return schema.GetBlastnIdentitiesColor(p * 100)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p">p的值在0-100之间</param>
        ''' <param name="Schema"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function GetBlastnIdentitiesColor(schema As RangeList(Of Double, NamedValue(Of Color)), p As Double) As Color
            Dim success As Boolean = False
            Dim cl As NamedValue(Of Color) = schema.SelectValue(p, [throw]:=False, success:=success)

            If Not success Then
                If p <= 0 Then
                    Return Color.Black
                Else
                    Return Color.Gray
                End If
            Else
                Return cl.Value
            End If
        End Function

        ''' <summary>
        ''' 使用<see cref="BBHIndex.GetIdentities(BBHIndex)"/>作为得分的彩色图案配色
        ''' </summary>
        ''' <returns></returns>
        Public Function IdentitiesChromatic() As RangeList(Of Double, NamedValue(Of Color))
            Return {
                New RangeTagValue(Of Double, NamedValue(Of Color))(0, 30, New NamedValue(Of Color)("<= 30%", Color.Black)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(30, 55, New NamedValue(Of Color)("30% - 55%", Color.Blue)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(55, 70, New NamedValue(Of Color)("55% - 70%", Color.Green)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(70, 90, New NamedValue(Of Color)("70% - 90%", Color.Purple)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(90, 100000, New NamedValue(Of Color)(">= 90%", Color.Red))
            }
        End Function

        ''' <summary>
        ''' 使用<see cref="BBHIndex.GetIdentities(BBHIndex)"/>作为得分，但是是黑白单色的
        ''' </summary>
        ''' <returns></returns>
        Public Function IdentitiesMonoChrome() As RangeList(Of Double, NamedValue(Of Color))
            Return {
                New RangeTagValue(Of Double, NamedValue(Of Color))(0, 50, New NamedValue(Of Color)("<= 50%", Color.LightGray)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(50, 75, New NamedValue(Of Color)("50% - 75%", Color.DarkGray)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(75, 10000, New NamedValue(Of Color)("> 75%", Color.Gray))
            }
        End Function

        Public Function BitScores() As RangeList(Of Double, NamedValue(Of Color))
            Return {
                New RangeTagValue(Of Double, NamedValue(Of Color))(0, 40, New NamedValue(Of Color)("< 40", Color.Black)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(40, 50, New NamedValue(Of Color)("40 - 50", Color.Blue)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(50, 80, New NamedValue(Of Color)("50 - 80", Color.Green)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(80, 200, New NamedValue(Of Color)("80 - 200", Color.Purple)),
                New RangeTagValue(Of Double, NamedValue(Of Color))(200, 10000, New NamedValue(Of Color)(">= 200", Color.Red))
            }
        End Function
    End Module
End Namespace
