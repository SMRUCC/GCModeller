#Region "Microsoft.VisualBasic::fcb5704a021c1b5ad81fe5bad6caa7ff, ..\interops\visualize\Circos\Circos\ConfFiles\Nodes\HighLight.vb"

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
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

Namespace Configurations.Nodes.Plots

    Public Class HighLight : Inherits TracksPlot(Of ValueTrackData)

        Public ReadOnly Property Highlights As Highlights
            Get
                Return DirectCast(Me.TracksData, Highlights)
            End Get
        End Property

        Sub New(HighlightsDataModel As Highlights)
            Call MyBase.New(HighlightsDataModel)
        End Sub

        <Circos> Public Overrides ReadOnly Property type As String
            Get
                Return "highlight"
            End Get
        End Property

        Protected Overrides Function GetProperties() As String()
            Return SimpleConfig.GenerateConfigurations(Of HighLight)(Me)
        End Function
    End Class
End Namespace
