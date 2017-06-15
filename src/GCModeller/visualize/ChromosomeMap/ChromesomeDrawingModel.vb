#Region "Microsoft.VisualBasic::9a0f11782bfb66ca78311e1eb8bd74af, ..\visualize\ChromosomeMap\ChromesomeDrawingModel.vb"

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

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.Java.IO.Properties.Reflector
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

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
        Public Property GeneObjects As DrawingModels.SegmentObject()
        ''' <summary>
        ''' 基因的突变点的数据
        ''' </summary>
        ''' <returns></returns>
        Public Property MutationDatas As DrawingModels.MultationPointData()
        ''' <summary>
        ''' 绘图设备的配置数据
        ''' </summary>
        ''' <returns></returns>
        Public Property Configuration As Configuration.DataReader
        ''' <summary>
        ''' 转录调控位点
        ''' </summary>
        ''' <returns></returns>
        Public Property MotifSites As DrawingModels.MotifSite()
        ''' <summary>
        ''' 基因的转录起始位点
        ''' </summary>
        ''' <returns></returns>
        Public Property TSSs As DrawingModels.TSSs()
        Public Property Loci As DrawingModels.Loci()

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
            Call sb.AppendLine($"GeneObject Segments:  {GeneObjects.DataCounts}")
            Call sb.AppendLine($"Mutation Sites Data:  {MutationDatas.DataCounts}")
            Call sb.AppendLine($"Motif Sites:          {MotifSites.DataCounts}")
            Call sb.AppendLine($"Loci Sites:           {Loci.DataCounts}")
            Call sb.AppendLine($"TSSs Sites:           {TSSs.DataCounts}")

            Return sb.ToString
        End Function
    End Class
End Namespace
