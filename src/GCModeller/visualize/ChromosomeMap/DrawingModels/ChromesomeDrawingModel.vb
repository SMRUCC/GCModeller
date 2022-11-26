#Region "Microsoft.VisualBasic::c7a984ef87110b4f5807312d18ac5c0a, GCModeller\visualize\ChromosomeMap\DrawingModels\ChromesomeDrawingModel.vb"

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

    '   Total Lines: 72
    '    Code Lines: 27
    ' Comment Lines: 38
    '   Blank Lines: 7
    '     File Size: 2.68 KB


    '     Class ChromesomeDrawingModel
    ' 
    '         Properties: COGs, Configuration, GeneObjects, Loci, MotifSiteColors
    '                     MotifSites, MutationDatas, TSSs
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.Extensions
Imports Oracle.Java.IO.Properties.Reflector
Imports SMRUCC.genomics.Assembly.NCBI.GenBank

Namespace DrawingModels

    ''' <summary>
    ''' Data model for described a chromosome drawing action invoked.(用于描述如何绘制一个基因组的图形数据的数据模型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ChromesomeDrawingModel : Inherits TabularFormat.Rpt

        ''' <summary>
        ''' 所需要进行绘制的基因组之中的基因对象，整个基因组之中的基本框架
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneObjects As SegmentObject()
        ''' <summary>
        ''' 基因的突变点的数据
        ''' </summary>
        ''' <returns></returns>
        Public Property MutationDatas As MultationPointData()
        ''' <summary>
        ''' 绘图设备的配置数据
        ''' </summary>
        ''' <returns></returns>
        Public Property Configuration As Configuration.DataReader
        ''' <summary>
        ''' 转录调控位点
        ''' </summary>
        ''' <returns></returns>
        Public Property MotifSites As MotifSite()
        ''' <summary>
        ''' 基因的转录起始位点
        ''' </summary>
        ''' <returns></returns>
        Public Property TSSs As TSSs()
        Public Property Loci As Loci()

        ''' <summary>
        ''' COG分类的颜色配置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property COGs As Dictionary(Of String, Brush)
        ''' <summary>
        ''' Motif位点的颜色配置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MotifSiteColors As Dictionary(Of String, Color)

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder

            Call sb.AppendLine("Drawing Model Brief Information:" & vbCrLf)
            Call sb.AppendLine($"GeneObject Segments:  {GeneObjects.TryCount}")
            Call sb.AppendLine($"Mutation Sites Data:  {MutationDatas.TryCount}")
            Call sb.AppendLine($"Motif Sites:          {MotifSites.TryCount}")
            Call sb.AppendLine($"Loci Sites:           {Loci.TryCount}")
            Call sb.AppendLine($"TSSs Sites:           {TSSs.TryCount}")

            Return sb.ToString
        End Function
    End Class
End Namespace
