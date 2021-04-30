#Region "Microsoft.VisualBasic::2b029d0e38457229b72556c0cbbaa613, RDotNet.Extensions.Bioinformatics\Declares\LDheatmap\LDheatmapAPI.vb"

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

'     Module API
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: LDheatmap
' 
'     Class LDheatmapS4Object
' 
'         Properties: color, distance, geneticsDistance, labels, LDmatrix
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API.base
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace LDheatmap

    Public Module API

        Sub New()
            Call require("LDheatmap")
        End Sub

        ''' <summary>
        ''' ``LDheatmap()`` is used to produce a graphical display, as a heat map, of pairwise linkage disequilibrium (LD) measurements for SNPs. 
        ''' The heat map is a false color image in the upper-left diagonal of a square plot. Optionally, a line parallel to the diagonal of the 
        ''' image indicating the physical or genetic map positions of the SNPs may be added, along with text reporting the total length of the 
        ''' genomic region considered.
        ''' </summary>
        ''' <param name="gdat">SNP data: a data frame of genotype objects, a square matrix of pairwise linkage disequilibrium measurements or an object of class "LDheatmap" (the returned object of this function).</param>
        ''' <param name="geneticDistances">A numeric vector of map locations of the SNPs, in the same order as SNPs listed in gdat, in terms of genetic or physical distances. Physical distances should be in bases, genetic distances should be in centiMorgans (cM). When gdat is not an object of class LDheatmap, the default is a vector that represents equi-spaced markers, 1kb (1000 bases) apart. When gdat is an object of class LDheatmap, the genetic.distances argument is taken to be the genetic.distances list item of gdat.</param>
        ''' <param name="distances">A character string to specify whether the provided map locations are in physical or genetic distances. If distances="physical" (default), the text describing the total length of the region will be “Physical Length:XXkb” where XX is the length of the region in kilobases. If distances="genetic", the text will be “Genetic Map Length:YYcM” where YY is the length of the region in centiMorgans. If gdat is an object of class LDheatmap, distances is taken from gdat.</param>
        ''' <param name="LDmeasure">A character string specifying the measure of LD - either allelic correlation r^2 or Lewontin's |D'|; default = "r" for r^2; type "D'" for |D'|. This argument is ignored when the user has already supplied calculated LD measurements through gdat (i.e., when gdat is a matrix of pairwise LD measurements or an object of class "LDheatmap").</param>
        ''' <param name="title">A character string for the main title of the plot. Default is “Pairwise LD”.</param>
        ''' <param name="addMap">If TRUE (default), a diagonal line indicating the physical or genetic map positions of the SNPs will be added to the plot, along with text indicating the total length of the genetic region.</param>
        ''' <param name="addKey">If TRUE (default) the color legend is drawn.</param>
        ''' <param name="geneMapLocation">A numeric value specifying the position of the line parallel to the diagonal of the matrix; the larger the value, the farther it lies from the matrix diagonal. Ignored when add.map=FALSE.</param>
        ''' <param name="geneMapLabelX">A numeric value specifying the x-coordinate of the text indicating the total length of the genomic region being considered. Ignored when add.map=FALSE.</param>
        ''' <param name="geneMapLabelY">A numeric value specifying the y-coordinate of the text indicating the total length of the genomic region being considered. Ignored when add.map=FALSE.</param>
        ''' <param name="SNPname">A vector of character string(s) of SNP name(s) to be labelled. Should match the names of SNPs in the provided object gdat, otherwise nothing is done.</param>
        ''' <param name="color">A range of colors to be used for drawing the heat map. Default is grey.colors(20).</param>
        ''' <param name="newpage">If TRUE (default), the heat map will be drawn on a new page.</param>
        ''' <param name="name">A character string specifying the name of the LDheatmap graphical object (grob) to be produced.</param>
        ''' <param name="vpname">A character string specifying the name of the viewport where the heat map is going to be drawn.</param>
        ''' <param name="pop">If TRUE, the viewport where the heat map is drawn is popped (i.e. removed) from the viewport tree after drawing. Default=FALSE.</param>
        ''' <param name="flip">If TRUE, the LDheatmap plot is flipped below a horizontal line, in the style of Haploview. Default is FALSE.</param>
        ''' <param name="text">If TRUE, the LD measurements are printed on each cell.</param>
        ''' <returns>For gdat, any square matrix with values between 0 and 1 inclusive can be passed, and the values above the diagonal will be plotted. In the display of LD, SNPs appear in the order supplied by the user as the horizontal and vertical coordinates are increased and one moves along the off-diagonal line, from the bottom-left to the top-right corner. To achieve this, the conventions of the image() function have been adopted, in which horizontal coordinates correspond to the rows of the matrix and vertical coordinates correspond to columns, and vertical coordinates are indexed in increasing order from bottom to top.
        ''' For the argument color, an appropriate color palette for quantitative data Is recommended, as outlined in the help page of the brewer.pal() function of the RColorBrewer package.
        ''' See the package vignette LDheatmap For more examples And details Of the implementation. Examples Of adding “tracks” Of genomic annotation above a flipped heatmap are In the package vignette addTracks.
        ''' </returns>
        ''' <remarks>The produced heat map can be modified in two ways. First, it is possible to edit interactively the grob components of the heat map, by using the function grid.edit; the function will not work if there is no open graphical device showing the heat map. Alternatively, the user can use the function editGrob and work with the grob LDheatmapGrob returned by LDheatmap. See Examples for usage.
        ''' LDheatmap() uses Grid, which does Not respond to par() settings. Hence modifying par() settings of mfrow And mfcol will Not work with LDheatmap(). The Examples section shows how to display multiple heat maps on one plot without the use of par().
        ''' </remarks>
        Public Function LDheatmap(gdat As String,
               Optional geneticDistances As String = NULL,
               Optional distances As String = "physical",
               Optional LDmeasure As String = "r",
               Optional title As String = "Pairwise LD",
               Optional addMap As Boolean = True,
               Optional addKey As Boolean = True,
               Optional geneMapLocation As Double = 0.15,
               Optional geneMapLabelX As String = NULL,
               Optional geneMapLabelY As String = NULL,
               Optional SNPname As String = NULL,
               Optional color As String = NULL,
               Optional newpage As Boolean = True,
               Optional name As String = "ldheatmap",
               Optional vpname As String = NULL,
               Optional pop As Boolean = False,
               Optional flip As String = NULL,
               Optional text As Boolean = False,
               Optional ByRef LDmatrix As String = Nothing) As LDheatmapS4Object

            Dim tmp As String = App.NextTempName
            Dim out = $"{tmp} <- LDheatmap({gdat}, genetic.distances={geneticDistances}, distances={Rstring(distances)},
	LDmeasure={Rstring(LDmeasure)}, title={Rstring(title)}, add.map={addMap.λ}, add.key={addKey.λ},
	geneMapLocation={geneMapLocation}, geneMapLabelX={geneMapLabelX}, geneMapLabelY={geneMapLabelY},
	SNP.name={SNPname}, color={color}, newpage={newpage.λ},
	name={Rstring(name)}, vp.name={vpname}, pop={pop.λ}, flip={flip}, text={text.λ})".__call

            Dim ld As New LDheatmapS4Object
            Dim list = out.AsList.ToArray

            ld.LDmatrix = list(0).AsNumeric.ToArray
            ld.geneticsDistance = list(4).AsInteger.ToArray
            ld.distance = list(5).AsCharacter.ToArray()(Scan0)
            ld.color = list(6).AsCharacter.ToArray

            If LDmatrix Is Nothing Then
                LDmatrix = TempFileSystem.GetAppSysTempFile(".csv")
            End If
            LDmatrix = Rstring(LDmatrix.UnixPath)

            Call $"success <- write.csv({tmp}$LDmatrix, {LDmatrix})".__call
            LDmatrix = LDmatrix.GetString

            Return ld
        End Function
    End Module

    Public Class LDheatmapS4Object

        Public Property LDmatrix As Double()
        Public Property geneticsDistance As Integer()
        Public Property distance As String
        Public Property color As String()
        Public Property labels As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
