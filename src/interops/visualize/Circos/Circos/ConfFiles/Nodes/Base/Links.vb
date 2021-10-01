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
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' Links are defined in ``&lt;link>`` blocks enclosed in a ``&lt;links>`` block. 
    ''' The links start at a radial position defined by 'radius' and have their
    ''' control point (adjusts curvature) at the radial position defined by
    ''' 'bezier_radius'. In this example, I use the segmental duplication
    ''' data Set, which connects regions Of similar sequence (90%+
    ''' similarity, at least 1kb In size).
    ''' </summary>
    Public Class Links

    End Class

    Public Class link : Inherits TracksPlot(Of TrackDatas.link)

        Public Overrides ReadOnly Property type As String
            Get
                Return "link"
            End Get
        End Property

        Public Property radius As String = "0.8r"
        Public Property bezier_radius As String = "0r"
        Public Property color As String = "black_a4"

        Sub New(data As data(Of TrackDatas.link))
            Call MyBase.New(data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return SimpleConfig _
                .GenerateConfigurations(Of link)(Me) _
                .ToArray
        End Function
    End Class
End Namespace
