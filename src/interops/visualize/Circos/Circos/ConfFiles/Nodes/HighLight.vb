#Region "Microsoft.VisualBasic::8fc0d970260a8a538fb3be6fca0828ce, visualize\Circos\Circos\ConfFiles\Nodes\Highlight.vb"

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

    '     Class Highlight
    ' 
    '         Properties: Highlights, type
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetProperties
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

Namespace Configurations.Nodes.Plots

    Public Class Highlight : Inherits TrackPlot(Of ValueTrackData)

        Public ReadOnly Property Highlights As Highlights
            Get
                Return TryCast(Me.TracksData, Highlights)
            End Get
        End Property

        ''' <summary>
        ''' 是否将这个高亮元素输出到顶层的 ``&lt;highlights>`` 块之中？
        ''' (默认为假，即作为 ``&lt;plots>`` 块之中的一个 ``type = highlight`` 的 plot 元素输出)
        ''' </summary>
        ''' <returns></returns>
        Public Property IsTopLevelBlock As Boolean = False

        <Circos> Public Overrides ReadOnly Property type As String
            Get
                Return "highlight"
            End Get
        End Property

        Public Overrides ReadOnly Property BlockName As String
            Get
                Return If(IsTopLevelBlock, CircosBlocks.highlights, CircosBlocks.plots)
            End Get
        End Property

        Public Overrides ReadOnly Property ElementTag As String
            Get
                Return If(IsTopLevelBlock, "highlight", "plot")
            End Get
        End Property

        Sub New(HighlightsDataModel As Highlights)
            Call MyBase.New(HighlightsDataModel)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return Me.GenerateConfigLines()
        End Function
    End Class
End Namespace
