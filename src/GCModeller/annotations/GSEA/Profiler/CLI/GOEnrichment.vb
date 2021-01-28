#Region "Microsoft.VisualBasic::b62922f2c50f9cd491a1f920ad614e04, GSEA\Profiler\CLI\GOEnrichment.vb"

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

    ' Module CLI
    ' 
    '     Function: GOEnrichmentBarPlot
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Partial Module CLI

    ''' <summary>
    ''' 这个与eggHTS工具中的GO富集条形图有一些不一样
    ''' 这个条形图是分开的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/GO.enrichment.barplot")>
    <Usage("/GO.enrichment.barplot /in <result.csv> [/go <go.obo> /disable.label_trim /top <default=35> /colors <schemaName, default=YlGnBu:c8> /tiff /out <output_directory>]")>
    Public Function GOEnrichmentBarPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.go_enrichment.barplots/"
        Dim goDB As String = args("/go") Or (GCModeller.FileSystem.GO & "/go.obo")
        Dim terms = GO_OBO.Open(goDB).ToDictionary
        Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
        Dim saveInTiff As Boolean = args("/tiff")
        Dim outFile$
        Dim tiff As TiffWriter
        Dim disableLabelTrim As Boolean = args("/disable.label_trim")

        For Each plot As NamedValue(Of GraphicsData) In enrichments.NamespaceEnrichmentPlot(
            GO_terms:=terms,
            top:=args("/top") Or 35,
            colorSchema:=args("/colors") Or "YlGnBu:c8",
            nolabelTrim:=disableLabelTrim
        )
            If TypeOf plot.Value Is ImageData Then
                If saveInTiff Then
                    outFile = $"{out}/{plot.Name}.tiff"
                    tiff = New TiffWriter(DirectCast(plot.Value, ImageData).Image)
                    tiff.MultipageTiffSave(outFile)
                Else
                    outFile = $"{out}/{plot.Name}.png"
                    plot.Value.Save(outFile)
                End If
            Else
                outFile = $"{out}/{plot.Name}.svg"
                plot.Value.Save(outFile)
            End If

            If outFile.FileExists Then
                Call outFile.__INFO_ECHO
            Else
                Call outFile.Warning
            End If
        Next

        Return 0
    End Function
End Module
