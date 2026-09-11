#Region "Microsoft.VisualBasic::b89fdbc94d308869c218f243b3459121, visualize\Circos\Circos\ConfFiles\Nodes\Connector.vb"

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

    '     Class Connector
    ' 
    '         Properties: type
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetProperties
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' 可以用来表示调控关系
    ''' </summary>
    Public Class Connector : Inherits TracksPlot(Of RegionTrackData)

        <Circos> Public Overrides ReadOnly Property type As String
            Get
                Return "connector"
            End Get
        End Property

        ''' <summary>
        ''' Connector dimensions, five comma delimited values which control the shape
        ''' of the connector line: ``radius1_start, pad1, radius2_start, pad2, pad3``.
        '''
        ''' The connector line is formed by five concatenated segments: a radial
        ''' segment outwards, an angular padding segment, a radial segment, another
        ''' angular padding segment and finally a radial padding segment.
        '''
        ''' (see ``etc/tracks/connector.conf`` in the circos distribution)
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property connector_dims As String = "0,0.3,0.4,0.3,0"

        Sub New(data As IEnumerable(Of RegionTrackData))
            Call MyBase.New(New TrackDatas.Connector(data))

            Call applyDefaults()
        End Sub

        Sub New(doc As TrackDatas.Connector)
            Call MyBase.New(doc)

            Call applyDefaults()
        End Sub

        Private Sub applyDefaults()
            ' 默认值取自 circos 发行版之中的 etc/tracks/connector.conf
            fill_color = "black"
            thickness = "1"
            r1 = "0.60r"
            r0 = "0.55r"
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return Me.GenerateConfigLines()
        End Function
    End Class
End Namespace
