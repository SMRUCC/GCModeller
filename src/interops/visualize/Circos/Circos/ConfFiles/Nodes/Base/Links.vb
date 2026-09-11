#Region "Microsoft.VisualBasic::8f49afdd3db4de41052f1d8cc32aa972, visualize\Circos\Circos\ConfFiles\Nodes\Base\Links.vb"

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

    '     Class Links
    ' 
    ' 
    ' 
    '     Class link
    ' 
    '         Properties: bezier_radius, color, radius, type
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetProperties
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' The ``&lt;links>`` top level block helper.(用于生成 circos 配置文件之中的顶层 ``&lt;links>`` 块)
    ''' </summary>
    ''' <remarks>
    ''' ``link`` 并不是一个合法的 ``&lt;plot>`` 类型(circos 的 ``lib/Circos/Track.pm`` 之中
    ''' 声明的类型白名单为 ``scatter line histogram heatmap highlight tile text connector``)，
    ''' 所以 connection 关系必须被放置在顶层的 ``&lt;links>`` 块之中进行输出。
    ''' </remarks>
    Public Module LinksBlock

        ''' <summary>
        ''' 生成 circos 配置文件之中的顶层 ``&lt;links>`` 配置块
        ''' </summary>
        ''' <param name="links">被输出到这个块之中的所有的 ``&lt;link>`` 节点</param>
        ''' <param name="directory"></param>
        ''' <returns>当没有 link 元素的时候返回空字符串</returns>
        Public Function Build(links As IEnumerable(Of ICircosDocNode), directory$) As String
            Return BuildCircosBlock(CircosBlocks.links, links, directory)
        End Function
    End Module

    ''' <summary>
    ''' Links are defined in ``&lt;link>`` blocks enclosed in a ``&lt;links>`` block. 
    ''' The links start at a radial position defined by 'radius' and have their
    ''' control point (adjusts curvature) at the radial position defined by
    ''' 'bezier_radius'. In this example, I use the segmental duplication
    ''' data Set, which connects regions Of similar sequence (90%+
    ''' similarity, at least 1kb In size).
    ''' </summary>
    ''' <remarks>
    ''' Data format of the link:
    '''
    ''' ```
    ''' chr12 1000 5000 chr15 5000 7000
    ''' ```
    ''' </remarks>
    Public Class LinkPlot : Inherits TrackPlot(Of LinkData)

        <Circos> Public Overrides ReadOnly Property type As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return "link"
            End Get
        End Property

        ''' <summary>
        ''' 因为这个类型的元素不是合法的 plot 类型，所以其必须被输出到顶层的 ``&lt;links>`` 块之中
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property BlockName As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return CircosBlocks.links
            End Get
        End Property

        ''' <summary>
        ''' ``&lt;links>`` 块之中的元素标签为 ``&lt;link>``
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property ElementTag As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return "link"
            End Get
        End Property

        ''' <summary>
        ''' Links start at this radius.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property radius As String = "0.8r"
        ''' <summary>
        ''' The radial position of the bezier curve control point, which adjusts curvature.
        ''' 0r means that the control point Is at the center Of the image.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property bezier_radius As String = "0r"
        ''' <summary>
        ''' The size Of the deviation Of the bezier control point from the midpoint
        ''' Between the start/End Of the link. Is applied Only when bezier_radius Is negative.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property crest As String = "0.5"
        ''' <summary>
        ''' The direction In And radius multiplier To which As control-point adjusting factor 
        ''' bezier_radius_purity Is applied In the special case With bezier_radius=0r.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property bezier_radius_purity As String = "0.75"
        ''' <summary>
        ''' Draw the link as a ribbon (and sample the thickness from the data value).
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property ribbon As String = no
        <Circos> Public Property color As String = "black_a4"

        Sub New(data As TrackDataDocument(Of LinkData))
            Call MyBase.New(data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return Me.GenerateConfigLines()
        End Function
    End Class
End Namespace
