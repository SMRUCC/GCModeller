#Region "Microsoft.VisualBasic::8b85293e029371f3d26a13b2d4b45d85, visualize\Circos\Circos\ConfFiles\Circos.vb"

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

    '     Class Circos
    ' 
    '         Properties: chromosomes, chromosomes_breaks, chromosomes_color, chromosomes_display_default, chromosomes_order
    '                     chromosomes_radius, chromosomes_reverse, chromosomes_scale, chromosomes_units, colors
    '                     genome, Ideogram, karyotype, numberOfTracks, Plots
    '                     show_heatmap, show_heatmaps, show_highlight, show_highlights, show_histogram
    '                     show_line, show_links, show_scatter, show_text, show_tile
    '                     size, skeletonKaryotype, track_start, track_step, track_width
    '                     use_rules
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Build, CreateObject, GetEnumerator, IEnumerable_GetEnumerator, Save
    ' 
    '         Sub: AddTrack, (+2 Overloads) ForceAutoLayout
    ' 
    '         Operators: +
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Karyotype

Namespace Configurations

    ''' <summary>
    ''' ``circos.conf``
    ''' 
    ''' ```
    '''                                     ____ _
    '''                                    / ___(_)_ __ ___ ___  ___
    '''                                   | |   | | '__/ __/ _ \/ __|
    '''                                   | |___| | | | (_| (_) \__ \
    '''                                    \____|_|_|  \___\___/|___/
    '''
    '''                                                round Is good
    '''
    ''' circos - generate circularly composited information graphics
    ''' ```
    ''' 
    ''' (Circo基因组绘图程序的主配置文件)
    ''' </summary>
    ''' <remarks>
    ''' ![](https://raw.githubusercontent.com/SMRUCC/GCModeller.Circos/master/manual/workflow.png)
    ''' 
    ''' Typically a central configuration file which defines data track information (circos.conf) imports other 
    ''' configuration files that store parameters that change less frequently 
    ''' (tick marks, ideogram size, grid, etc). 
    ''' 
    ''' Data for each data track Is stored in a file And the same file can be used for multiple tracks.
    ''' 
    ''' + PNG image output Is ideal For immediate viewing, web-based reporting Or presentation. 
    ''' + SVG output Is most suitable For generating very high resolution line art For publication And For customizing aspects Of the figure.
    ''' </remarks>
    Public Class Circos : Inherits CircosConfig
        Implements ICircosDocument
        Implements IEnumerable(Of ITrackPlot)

        ''' <summary>
        ''' The basically genome structure plots: Chromosome name, size and color definition.(基本的数据文件)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Public Property karyotype As String = "data/genes.txt"
        <Circos> Public Property genome As String = null
        <Circos> Public Property use_rules As String = yes

        ''' <summary>
        ''' The chromosomes_unit value is used as a unit (suffix "u") to shorten
        ''' values In other parts Of the configuration file. Some parameters,
        ''' such As ideogram And tick spacing, accept "u" suffixes, so instead Of
        '''
        ''' ```
        ''' spacing = 10000000
        ''' ```
        ''' 
        ''' you can write
        ''' 
        ''' ```
        ''' spacing = 10u
        ''' ```
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property chromosomes_units As String = "5000"
        ''' <summary>
        ''' The default behaviour is to display all chromosomes defined in the
        ''' karyotype file. In this example, I Select only a subset.
        '''
        ''' The 'chromosomes' parameter has several uses, and selecting which
        ''' chromosomes To show Is one Of them. You can list them
        '''
        ''' ```
        ''' hs1;hs2;hs3;hs4
        ''' ```
        ''' 
        ''' Or provide a regular expression that selects them based On a successful match
        ''' 
        ''' ```
        ''' /hs[1-4]$/
        ''' ```
        ''' 
        ''' The ``$`` anchor Is necessary, otherwise chromosomes Like *hs10, hs11 And
        ''' hs20* are also matched.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property chromosomes_display_default As String = yes
        <Circos> Public Property chromosomes As String = null
        ''' <summary>
        ''' By default, the scale progression is clockwise. You can set the
        ''' Global angle progression Using 'angle_orientation' in the ``&lt;image>``
        ''' block (clockwise Or counterclockwise). To reverse it For one Or
        ''' several ideograms, use 'chromosomes-reverse'
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property chromosomes_reverse As String = null
        ''' <summary>
        ''' The default radial position for all ideograms is set by 'radius' in
        ''' the ``&lt;ideogram>`` block (see ideogram.conf). To change the value For
        ''' specific ideograms, use chromosomes_radius.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property chromosomes_radius As String = null
        ''' <summary>
        ''' The size of the ideogram on the figure can be adjusted using an
        ''' absolute Or relative magnification. Absolute scaling,
        '''
        ''' ```
        ''' hs1=0.5
        ''' ```
        ''' 
        ''' shrinks Or expands the ideogram by a fixed factor. When the "r"
        ''' suffix Is used, the magnification becomes relative To the
        ''' circumference Of the figure. Thus, 
        '''
        ''' ```
        ''' hs1=0.5r
        ''' ```
        ''' 
        ''' makes ``hs1`` To occupy 50% Of the figure. To uniformly distribute
        ''' several ideogram within a fraction Of the figure, use a regular
        ''' expression that selects the ideograms And the "rn" suffix (relative
        ''' normalized).
        '''
        ''' ```
        ''' /hs[234]/=0.5Rn
        ''' ```
        ''' 
        ''' Will match ``hs2, hs3, hs4`` And divide them evenly into 50% Of the figure. 
        ''' Each ideogram will be about **16%** Of the figure.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property chromosomes_scale As String = null
        ''' <summary>
        ''' The color of each ideogram is taken from the karyotype file. To
        ''' change it, use 'chromosomes_color'.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property chromosomes_color As String = null
        <Circos> Public Property chromosomes_order As String = null
        <Circos> Public Property chromosomes_breaks As String = null

        <Circos> Public Property show_scatter As String = yes
        <Circos> Public Property show_line As String = yes
        <Circos> Public Property show_histogram As String = yes
        <Circos> Public Property show_heatmap As String = yes
        <Circos> Public Property show_tile As String = yes
        <Circos> Public Property show_highlight As String = yes
        <Circos> Public Property show_links As String = yes
        <Circos> Public Property show_highlights As String = yes
        <Circos> Public Property show_text As String = yes
        <Circos> Public Property show_heatmaps As String = yes

        <Circos> Public Property track_width As String = null
        <Circos> Public Property track_start As String = null
        <Circos> Public Property track_step As String = null

        Public Property colors As OverwritesColors

        ''' <summary>
        ''' 基因组的骨架信息
        ''' </summary>
        ''' <returns></returns>
        Public Property skeletonKaryotype As SkeletonInfo

        ReadOnly plotTracks As New List(Of ITrackPlot)

        Public ReadOnly Property Ideogram As Ideogram
            Get
                For Each include In Me.includes
                    If TypeOf include Is Ideogram Then
                        Return DirectCast(include, Ideogram)
                    End If
                Next

                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' The genome size.(基因组的大小，当<see cref="SkeletonKaryotype"/>为空值的时候返回数值0)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            Get
                If skeletonKaryotype Is Nothing Then
                    Return 0
                End If
                Return _skeletonKaryotype.size - skeletonKaryotype.loopHole
            End Get
        End Property

        ''' <summary>
        ''' 内部元素是有顺序的区别的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Plots As ITrackPlot()
            Get
                Return plotTracks.ToArray
            End Get
        End Property

        ''' <summary>
        ''' Gets the number of the tracks that defined in this circos model
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property numberOfTracks As Integer
            Get
                Return plotTracks.Count
            End Get
        End Property

        Sub New()
            Call MyBase.New("circos.conf", Nothing)
            Me.main = Me
        End Sub

        Public Overloads Overrides Function Save(directory$, encoding As Encoding) As Boolean
            Dim base = directory Or FilePath.ParentPath.AsDefault
            Dim dataDIR As String = $"{base}/data/"

            Call FilePath.SetValue($"{base}/{FileIO.FileSystem.GetFileInfo(FilePath).Name}")
            Call FileIO.FileSystem.CreateDirectory(dataDIR)

            For Each i As SeqValue(Of ITrackPlot) In plotTracks.SeqIterator
                Dim track As ITrackPlot = i.value
                Dim path$ = $"data/{track.type}_data_{i.i + 1}.txt"

                ' 首先保存数据文件
                track.file = path
                track.Save($"{directory}/{path}", Encoding.ASCII)
            Next

            Call _skeletonKaryotype.Save($"{base}/{karyotype}", encoding:=Encoding.ASCII)

            ' 最后在这里生成配置文件
            Return Build(0, directory:=base).SaveTo(FilePath, Encoding.ASCII)
        End Function

        Public Overloads Shared Function CreateObject() As Circos
            Dim circos As New Circos With {
                .includes = New List(Of CircosConfig)
            }

            Call circos.includes.Add(CircosDistributed.ColorFontsPatterns)
            Call circos.includes.Add(CircosDistributed.HouseKeeping)
            Call circos.includes.Add(CircosDistributed.Image)

            Return circos
        End Function

#Region "默认的图形属性"

        Dim stroke_thickness As String = "0"
        Dim stroke_color As String = ""
#End Region

        ''' <summary>
        ''' 函数会根据元素的个数的情况自动的调整在圈内的位置
        ''' </summary>
        ''' <param name="track"></param>
        ''' <remarks></remarks>
        Public Sub AddTrack(track As ITrackPlot, Optional autoLayout As Boolean = True)
            Call Me.plotTracks.Add(track)

            If Not String.IsNullOrEmpty(stroke_thickness) Then
                track.stroke_thickness = stroke_thickness
            End If
            If Not String.IsNullOrEmpty(stroke_color) Then
                track.stroke_color = stroke_color
            End If

            If autoLayout Then
                Call ForceAutoLayout(Me.Plots)
            End If
        End Sub

        ''' <summary>
        ''' 强制所有的元素都自动布局
        ''' </summary>
        Public Sub ForceAutoLayout()
            Call ForceAutoLayout(Me.Plots)
        End Sub

        ''' <summary>
        ''' 强制所指定的绘图元素自动布局
        ''' </summary>
        ''' <param name="tracks"></param>
        Public Shared Sub ForceAutoLayout(tracks As ITrackPlot())
            Dim d = 0.8 / tracks.Length / 2
            Dim p As Double = 0.95

            For Each track As ITrackPlot In tracks
                track.r1 = p & "r"
                p -= d
                track.r0 = p & "r"
                p -= d / 5
            Next
        End Sub

        Protected Overrides Function Build(IndentLevel As Integer, directory$) As String
            Dim sb As New ScriptBuilder(1024)

            sb += Me.GenerateIncludes(directory)
            sb += ""

            For Each line As String In SimpleConfig.GenerateConfigurations(Of Circos)(Me)
                Call sb.AppendLine(line)
            Next

            If Not colors Is Nothing Then
                sb += DirectCast(colors, ICircosDocument).Build(Scan0, directory)
            End If

            If Not Plots.IsNullOrEmpty Then
                Call sb.AppendLine(vbCrLf & "<plots>")

                For Each plotRule As ITrackPlot In plotTracks
                    Call sb.AppendLine()
                    Call sb.AppendLine(plotRule.Build(IndentLevel + 2, directory))
                Next

                Call sb.AppendLine()
                Call sb.AppendLine("</plots>")
            End If

            Return sb.ToString
        End Function

        Public Shared Operator +(circos As Circos, track As ITrackPlot) As Circos
            Call circos.AddTrack(track)
            Return circos
        End Operator

        Public Iterator Function GetEnumerator() As IEnumerator(Of ITrackPlot) Implements IEnumerable(Of ITrackPlot).GetEnumerator
            For Each track As ITrackPlot In plotTracks
                Yield track
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
