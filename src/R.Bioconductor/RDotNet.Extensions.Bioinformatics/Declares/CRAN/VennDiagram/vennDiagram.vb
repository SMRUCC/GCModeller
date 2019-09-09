#Region "Microsoft.VisualBasic::308cd65d051036eeb52aa53c78dde521, RDotNet.Extensions.Bioinformatics\Declares\CRAN\VennDiagram\vennDiagram.vb"

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

    '     Class vennDiagramPlot
    ' 
    '         Properties: [sub], areaVector, categoryNames, compression, directArea
    '                     filename, fill, forceUnique, height, hyperTest
    '                     imagetype, main, mainCex, mainCol, mainFontface
    '                     mainFontfamily, mainJust, mainPos, na, printMode
    '                     resolution, sigdigs, subCex, subCol, subFontface
    '                     subFontfamily, subJust, subPos, totalPopulation, units
    '                     width, x
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Copy
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace VennDiagram

    ''' <summary>
    ''' This function takes a list and creates a publication-quality TIFF Venn Diagram
    ''' </summary>
    <RFunc("venn.diagram")> Public Class vennDiagramPlot : Inherits vennBase

        ''' <summary>
        ''' A list of vectors (e.g., integers, chars), with each component corresponding to a separate circle in the Venn diagram
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' Filename for image output, Or if NULL returns the grid object itself
        ''' </summary>
        ''' <returns></returns>
        <Parameter("filename", ValueTypes.Path)> Public Property filename As String
        ''' <summary>
        ''' Integer giving the height Of the output figure In units
        ''' </summary>
        ''' <returns></returns>
        Public Property height As Integer = 4000
        ''' <summary>
        ''' Integer giving the width of the output figure in units
        ''' </summary>
        ''' <returns></returns>
        Public Property width As Integer = 7000
        ''' <summary>
        ''' Resolution of the final figure in DPI
        ''' </summary>
        ''' <returns></returns>
        Public Property resolution As Integer = 600
        ''' <summary>
        ''' Specification of the image format (e.g. tiff, png or svg)
        ''' </summary>
        ''' <returns></returns>
        Public Property imagetype As String = "tiff"
        ''' <summary>
        ''' Size-units to use for the final figure
        ''' </summary>
        ''' <returns></returns>
        Public Property units As String = "px"
        ''' <summary>
        ''' What compression algorithm should be applied to the final tiff
        ''' </summary>
        ''' <returns></returns>
        Public Property compression As String = "lzw"
        ''' <summary>
        ''' Missing value handling method: "none", "stop", "remove"
        ''' </summary>
        ''' <returns></returns>
        Public Property na As String = "stop"
        ''' <summary>
        ''' Character giving the main title of the diagram
        ''' </summary>
        ''' <returns></returns>
        Public Property main As RExpression = NULL
        ''' <summary>
        ''' Character giving the subtitle of the diagram
        ''' </summary>
        ''' <returns></returns>
        Public Property [sub] As RExpression = NULL
        ''' <summary>
        ''' Vector of length 2 indicating (x,y) of the main title
        ''' </summary>
        ''' <returns></returns>
        <Parameter("main.pos")> Public Property mainPos As RExpression = c(0.5, 1.05)
        ''' <summary>
        ''' Character giving the fontface (font style) of the main title
        ''' </summary>
        ''' <returns></returns>
        <Parameter("main.fontface")> Public Property mainFontface As String = "plain"
        ''' <summary>
        ''' Character giving the fontfamily (font type) of the main title
        ''' </summary>
        ''' <returns></returns>
        <Parameter("main.fontfamily")> Public Property mainFontfamily As String = "serif"
        ''' <summary>
        ''' Character giving the colour of the main title
        ''' </summary>
        ''' <returns></returns>
        <Parameter("main.col")> Public Property mainCol As String = "black"
        ''' <summary>
        ''' Number giving the cex (font size) of the main title
        ''' </summary>
        ''' <returns></returns>
        <Parameter("main.cex")> Public Property mainCex As Integer = 1
        ''' <summary>
        ''' Vector of length 2 indicating horizontal and vertical justification of the main title
        ''' </summary>
        ''' <returns></returns>
        <Parameter("main.just")> Public Property mainJust As RExpression = c(0.5, 1.0R)
        ''' <summary>
        ''' Vector of length 2 indicating (x,y) of the subtitle
        ''' </summary>
        ''' <returns></returns>
        <Parameter("sub.pos")> Public Property subPos As RExpression = c(0.5, 1.05)
        ''' <summary>
        ''' Character giving the fontface (font style) of the subtitle
        ''' </summary>
        ''' <returns></returns>
        <Parameter("sub.fontface")> Public Property subFontface As String = "plain"
        ''' <summary>
        ''' Character giving the fontfamily (font type) of the subtitle
        ''' </summary>
        ''' <returns></returns>
        <Parameter("sub.fontfamily")> Public Property subFontfamily As String = "serif"
        ''' <summary>
        ''' Character Colour of the subtitle
        ''' </summary>
        ''' <returns></returns>
        <Parameter("sub.col")> Public Property subCol As String = "black"
        ''' <summary>
        ''' Number giving the cex (font size) of the subtitle
        ''' </summary>
        ''' <returns></returns>
        <Parameter("sub.cex")> Public Property subCex As Integer = 1
        ''' <summary>
        ''' Vector of length 2 indicating horizontal and vertical justification of the subtitle
        ''' </summary>
        ''' <returns></returns>
        <Parameter("sub.just")> Public Property subJust As RExpression = c(0.5, 1.0R)
        ''' <summary>
        ''' Allow specification of category names using plotmath syntax
        ''' </summary>
        ''' <returns></returns>
        <Parameter("category.names")> Public Property categoryNames As RExpression = names("x")
        ''' <summary>
        ''' Logical specifying whether to use only unique elements in each item of the input list or use all elements. Defaults to FALSE
        ''' </summary>
        ''' <returns></returns>
        <Parameter("force.unique")> Public Property forceUnique As Boolean = True
        ''' <summary>
        ''' Can be either 'raw' or 'percent'. This is the format that the numbers will be printed in. Can pass in a vector with the second element being printed under the first
        ''' </summary>
        ''' <returns></returns>
        <Parameter("print.mode")> Public Property printMode As String = "raw"
        ''' <summary>
        ''' If one of the elements in print.mode is 'percent', then this is how many significant digits will be kept
        ''' </summary>
        ''' <returns></returns>
        Public Property sigdigs As Integer = 3
        ''' <summary>
        ''' If this is equal to true, then the vector passed into area.vector will be directly assigned to the areas of the corresponding regions. Only use this if you know which positions in the vector correspond to which regions in the diagram
        ''' </summary>
        ''' <returns></returns>
        <Parameter("direct.area")> Public Property directArea As Boolean = False
        ''' <summary>
        ''' An argument to be used when direct.area is true. These are the areas of the corresponding regions in the Venn Diagram
        ''' </summary>
        ''' <returns></returns>
        <Parameter("area.vector")> Public Property areaVector As Integer = 0
        ''' <summary>
        ''' If there are only two categories in the venn diagram and total.population is not NULL, then perform the hypergeometric test and add it to the sub title.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("hyper.test")> Public Property hyperTest As Boolean = False
        ''' <summary>
        ''' An argument to be used when hyper.test is true. This is the total population size
        ''' </summary>
        ''' <returns></returns>
        <Parameter("total.population")> Public Property totalPopulation As RExpression = NULL

        ''' <summary>
        ''' The partition fill color
        ''' </summary>
        ''' <returns></returns>
        Public Property fill As RExpression

        Sub New()
        End Sub

        Sub New(x As String, colors As String, title As String, tiff As String)
            Me.x = x
            Me.fill = colors
            Me.main = Rstring(title)
            Me.filename = tiff
            Me.categoryNames = names(x)
        End Sub

        Public Function Copy(x As String, colors As String, Optional cat As RExpression = Nothing) As vennDiagramPlot
            Dim clone As vennDiagramPlot = DirectCast(Me.MemberwiseClone, vennDiagramPlot)

            clone.x = x
            clone.fill = colors
            clone.categoryNames = cat

            If clone.categoryNames Is Nothing Then
                clone.categoryNames = names(x)
            End If

            Return clone
        End Function
    End Class
End Namespace
