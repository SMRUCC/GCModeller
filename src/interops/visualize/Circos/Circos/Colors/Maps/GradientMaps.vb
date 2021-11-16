#Region "Microsoft.VisualBasic::8962586826e1ff53f4c6397ed7640abf, visualize\Circos\Circos\Colors\Maps\GradientMaps.vb"

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

    '     Module GradientMaps
    ' 
    '         Function: GradientMappings, MapAnnotations
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports ColorPattern = Microsoft.VisualBasic.Imaging.ColorMap

Namespace Colors

    ''' <summary>
    ''' Crcos程序的所支持的颜色
    ''' </summary>
    ''' <remarks></remarks>
    <Package("Circos.Color.GradientMaps",
                      Category:=APICategories.UtilityTools,
                      Publisher:="amethyst.asuka@gcmodeller.org",
                      Description:="Provides of the mappings between the scale vector and the circos color string, 
                  and also provides the mappings option between the .NET color value and the circos color name string.")>
    Public Module GradientMaps

        <ExportAPI("Map.Annotations")>
        Public Function MapAnnotations(<Parameter("map.Name")> mapName As String,
                                       <Parameter("map.Levels")> mapLevel As Integer,
                                       <Parameter("Title")> title As String,
                                       <Parameter("Is.Percentage?")> Optional isPercentage As Boolean = True,
                                       Optional min As Double = 0,
                                       Optional max As Double = 1,
                                       Optional offsetPercentage# = 0.06) As Image

            Dim g As Graphics2D = New Size(1024, 300).CreateGDIDevice
            Dim titleFont As New Font(FontFace.MicrosoftYaHei, 64, FontStyle.Regular)
            Dim sz As SizeF = g.Graphics.MeasureString(title, titleFont)

            Call g.DrawString(title, titleFont, Brushes.Black, New Point)

            Dim Y As Integer = CInt(5 + sz.Height) - 20
            Dim X As Double = 5
            Dim maps As New ColorPattern(mapLevel * 2)
            Dim colorSeq As Color() = ColorSequence(maps, mapName).Reverse.ToArray
            Dim offset = CInt(colorSeq.Length * offsetPercentage)
            Dim drWidth As Integer = g.Width - 5 * 2
            Dim dx As Double = drWidth / (colorSeq.Length - offset)
            Dim drHeight As Integer = CInt((g.Height - sz.Height) * 0.85)

            Dim sMin, sMax As String
            Dim ruleFont As New Font(FontFace.MicrosoftYaHei, 24, FontStyle.Bold)

            If isPercentage Then
                sMin = Mid(CStr(min * 100), 1, 6) & "%"
                sMax = Mid(CStr(max * 100), 1, 6) & "%"
            Else
                sMin = Mid(CStr(min), 1, 6)
                sMax = Mid(CStr(max), 1, 6)
            End If

            Call g.DrawString(sMin, ruleFont, Brushes.Black, New Point(CInt(X), Y + drHeight))

            For idx As Integer = offset To colorSeq.Length - 1
                Dim color As Color = colorSeq(idx)
                Dim currX As Integer = CInt(X)

                X += dx

                Dim currXNext As Integer = CInt(X)
                Dim rect As New Rectangle(New Point(currX, Y), New Size(CInt(dx), drHeight))

                Call g.FillRectangle(New SolidBrush(color), rect)
            Next

            sz = g.MeasureString(sMax, ruleFont)
            Call g.DrawString(sMax, ruleFont, Brushes.Black, New Point(CInt(g.Width - sz.Width), Y + drHeight))

            Return g.ImageResource
        End Function

        ''' <summary>
        ''' Creates a scale gradient color mappings between a vector and the circos RGB color.
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="mapName"><see cref="ColorMap"/></param>
        ''' <param name="mapLevel"></param>
        ''' <param name="offset"></param>
        ''' <param name="replaceBase"></param>
        ''' <returns></returns>
        <ExportAPI("Gradient.Mappings",
                   Info:="Creates a scale gradient color mappings between a vector and the circos RGB color.")>
        <Extension>
        Public Function GradientMappings(values As IEnumerable(Of Double),
                                         Optional mapName As String = "Jet",
                                         Optional mapLevel As Integer = 512,
                                         Optional offset As Double = Double.NaN,
                                         Optional replaceBase As Boolean = False,
                                         Optional offsetPercentage# = 0.06) As Mappings()

            If Not Double.IsNaN(offset) Then
                values = values.Join(offset)
            End If

            Dim mapLvs = values.GenerateMapping(mapLevel)
            Dim maps As New LevelMapGenerator(values, mapName, mapLevel, offsetPercentage, replaceBase)

            Call mapLvs.Max.__DEBUG_ECHO
            Call mapLvs.Min.__DEBUG_ECHO

            Dim mappings As Mappings() =
                mapLvs.Select(AddressOf maps.CreateMaps).ToArray
            Return mappings
        End Function
    End Module
End Namespace
