#Region "Microsoft.VisualBasic::767eabbe71aadb82b63e7397e6e02314, visualize\Circos\Test\mchrTest\Module1.vb"

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
'     Sub: Main, run
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.Circos
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.NtProps

''' <summary>
''' 使用文档之中的工作流演示 circos 绘图的完整流程
''' </summary>
''' <remarks>
''' 原来的演示代码所依赖的数据文件(``H:\5.14.circos\6.22\Af293.fna`` 等)已经不可用了，
''' 所以这里改为使用 <see cref="DemoSyntheticData"/> 程序化生成的虚构数据。
''' </remarks>
Module Module1

    ''' <summary>
    ''' 1. 创建 circos 文档对象
    ''' 2. 通过适配器将生物信息学数据转换为 circos 之中的 tracks 对象
    ''' 3. 保存 circos 文档
    ''' 4. 通过命令行调用 circos 程序进行绘图
    ''' </summary>
    Private Sub run(Optional outDIR As String = "Z:\circos-test\mchr\")
        ' 虚构的基因组序列数据
        Dim genome As Dictionary(Of String, String) = DemoSyntheticData.Genome()
        ' 基因组的骨架信息
        Dim skeleton As GenomeKaryotype = DemoSyntheticData.Karyotype()

        Call skeleton.Save($"{Circos.NormalizeDirectory(outDIR)}/data/karyotype.txt")

        Dim circos As New Configurations.Circos

        circos.skeletonKaryotype = skeleton
        circos.karyotype = "data/karyotype.txt"
        circos.includes.Add(New Configurations.IdeogramInclude(circos))
        circos.includes.Add(New Configurations.TicksInclude(circos))

        circos.Ideogram.Ideogram.show_label = "yes"
        circos.Ideogram.Ideogram.Spacing.default = "0.2u"
        circos.chromosomes_units = "1000000"

        ' GC 含量与 GC skew
        Dim gc As ValueTrackData() = DemoSyntheticData.GCContent(genome)
        Dim skew As ValueTrackData() = DemoSyntheticData.GCSkew(genome)

        circos.AddTrack(New Histogram(New TrackDataDocument(Of ValueTrackData)(gc)), autoLayout:=False)
        circos.AddTrack(New Histogram(New TrackDataDocument(Of ValueTrackData)(skew)), autoLayout:=False)
        ' 渐变色高亮映射
        circos.AddTrack(New Highlight(New GradientMappings(gc, "Jet")), autoLayout:=False)

        Call CircosAPI.SetRadius(circos, rMax:=0.75, rMin:=0.35)

        ' 保存配置文件并调用 circos 程序绘图
        Dim result As CircosRenderResult = CircosRender.Render(circos, outDIR, outputFile:="mchr.png")

        Call Console.WriteLine(result.ToString)
    End Sub
End Module
