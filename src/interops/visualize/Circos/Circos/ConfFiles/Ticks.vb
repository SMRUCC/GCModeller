#Region "Microsoft.VisualBasic::35ac8c7b14a08ba0c630bed61211b698, ..\Circos\Circos\ConfFiles\Ticks.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports System.Text

Namespace Configurations

    Public Class Ticks : Inherits CircosConfig
        Implements ICircosDocument

        <Circos> Public Property show_ticks As String = yes
        <Circos> Public Property show_tick_labels As String = yes

        <Circos> Public Property show_grid As String = no
        <Circos> Public Property grid_start As String = "dims(ideogram,radius_inner)-0.5r"
        <Circos> Public Property grid_end As String = "dims(ideogram,radius_inner)"

        Public Property Ticks As Nodes.Ticks

        Sub New(Circos As Circos)
            Call MyBase.New("ticks.conf", Circos)
            Ticks = Nodes.Ticks.DefaultConfiguration
        End Sub

        Protected Overrides Function GenerateDocument(IndentLevel As Integer) As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            For Each strLine As String In SimpleConfig.GenerateConfigurations(Me)
                Call sBuilder.AppendLine(strLine)
            Next

            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(Ticks.GenerateDocument(IndentLevel + 2))

            Return sBuilder.ToString
        End Function
    End Class
End Namespace
