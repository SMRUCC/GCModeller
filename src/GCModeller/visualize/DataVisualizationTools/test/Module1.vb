#Region "Microsoft.VisualBasic::afdac16dfc58cf67111b880b7bd8e92c, visualize\DataVisualizationTools\test\Module1.vb"

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
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize.NCBIBlastResult

Module Module1

    Sub Main()
        Dim rand As New Random
        Dim max# = 1000
        Dim getCategory = Function(i As Integer) As OrthologyProfile
                              Return New OrthologyProfile With {
                                  .Category = "category " & (i + 1),
                                  .HomologyDegrees = {
                                      New NamedValue(Of Color)("a", Color.Red, rand.NextDouble * max * 0.65),
                                      New NamedValue(Of Color)("b", Color.LightBlue, rand.NextDouble * max),
                                      New NamedValue(Of Color)("c", Color.SkyBlue, rand.NextDouble * max * 0.65),
                                      New NamedValue(Of Color)("d", Color.Blue, rand.NextDouble * max),
                                      New NamedValue(Of Color)("e", Color.DarkBlue, rand.NextDouble * max * 0.3),
                                      New NamedValue(Of Color)("f", Color.Gray, rand.NextDouble * max * 0.5)
                                  }
                              }
                          End Function

        Dim profiles = 20.SeqRandom.Select(Function(null) getCategory(null)).RenderColors.ToArray

        Call profiles.Plot.AsGDIImage.SaveAs("./test.png")

    End Sub
End Module
