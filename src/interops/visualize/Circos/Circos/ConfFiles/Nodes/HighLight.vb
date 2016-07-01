#Region "Microsoft.VisualBasic::f56bf26df7843f4750205cfd0966b5a4, ..\Circos\Circos\ConfFiles\Nodes\HighLight.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Circos.TrackDatas
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Circos.TrackDatas.Highlights
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Scripting

Namespace Configurations.Nodes.Plots

    Public Class HighLight : Inherits TracksPlot(Of ValueTrackData)

        Public ReadOnly Property Highlights As Highlights
            Get
                Return Me.TracksData.As(Of Highlights)
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
