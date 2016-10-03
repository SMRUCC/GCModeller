#Region "Microsoft.VisualBasic::87d93e954aa2dd1b2bd5b2eed6503bbd, ..\interops\visualize\Circos\Circos\ConfFiles\Nodes\Connector.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights
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

        Sub New(data As IEnumerable(Of RegionTrackData))
            Call MyBase.New(New TrackDatas.Connector(data))
        End Sub

        Sub New(doc As TrackDatas.Connector)
            Call MyBase.New(doc)
        End Sub

        Protected Overrides Function GetProperties() As String()
            Return SimpleConfig.GenerateConfigurations(Of Connector)(Me)
        End Function
    End Class
End Namespace
