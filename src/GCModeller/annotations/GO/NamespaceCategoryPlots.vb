#Region "Microsoft.VisualBasic::b342abf17c2956ba52222782517d5410, annotations\GO\NamespaceCategoryPlots.vb"

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

    ' Module NamespaceCategoryPlots
    ' 
    '     Function: doSingleBarplot, NamespaceEnrichmentPlot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module NamespaceCategoryPlots

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data">输出的结果都应该是不重复的</param>
    ''' <param name="GO_terms"></param>
    ''' <param name="size"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function NamespaceEnrichmentPlot(Of EnrichmentTerm As IGoTermEnrichment)(
                    data As IEnumerable(Of EnrichmentTerm),
                    GO_terms As Dictionary(Of Term),
                    Optional pvalue# = 0.05,
                    Optional size$ = "2700,2100",
                    Optional tick# = 1,
                    Optional usingCorrected As Boolean = False,
                    Optional top% = -1,
                    Optional colorSchema$ = "Set1:c6",
                    Optional nolabelTrim As Boolean = False) As IEnumerable(Of NamedValue(Of GraphicsData))

        Dim namespaceProfiles = data.CreateEnrichmentProfiles(GO_terms, usingCorrected, top, pvalue)
        Dim image As GraphicsData
        Dim namespaceTitle$

        For Each [namespace] In namespaceProfiles
            namespaceTitle = [namespace].Key
            image = [namespace] _
                .Value _
                .doSingleBarplot(namespaceTitle, size, tick, colorSchema, nolabelTrim)

            If TypeOf image Is SVGData Then
                DirectCast(image, SVGData).title = "Go enrichment of " & namespaceTitle
            End If

            Yield New NamedValue(Of GraphicsData) With {
                .Name = [namespace].Key,
                .Value = image
            }
        Next
    End Function

    <Extension>
    Private Function doSingleBarplot(profiles As NamedValue(Of Double)(), namespace$, size$, tick#, colorSchema$, nolabelTrim As Boolean) As GraphicsData
        Return LevelBarplot.Plot(
            data:=profiles,
            size:=size,
            title:=[namespace],
            levelColorSchema:=colorSchema,
            legendTitle:="p.adjust",
            valueTitle:="-log10(p.value)",
            labelFontCSS:=CSSFont.PlotTitle,
            titleFontCSS:=CSSFont.Win7VeryVeryLargeNormal,
            tickFontCSS:=CSSFont.Win7VeryLarge,
            valueTitleFontCSS:="font-style: normal; font-size: 48; font-family: " & FontFace.MicrosoftYaHei & ";",
            nolabelTrim:=nolabelTrim
        )
    End Function

End Module
