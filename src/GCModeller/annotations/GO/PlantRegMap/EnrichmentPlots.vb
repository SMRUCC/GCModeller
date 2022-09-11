#Region "Microsoft.VisualBasic::8cd53c6811f5a482236bbd768bfbc26a, GCModeller\annotations\GO\PlantRegMap\EnrichmentPlots.vb"

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


    ' Code Statistics:

    '   Total Lines: 19
    '    Code Lines: 16
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 800 B


    '     Module EnrichmentPlots
    ' 
    '         Function: PlantEnrichmentPlot
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Namespace PlantRegMap

    Public Module EnrichmentPlots

        <Extension>
        Public Function PlantEnrichmentPlot(data As IEnumerable(Of PlantRegMap_GoTermEnrichment),
                                            GO_terms As Dictionary(Of Term),
                                            Optional pvalue# = 0.05,
                                            Optional size$ = "2200,2000",
                                            Optional tick# = 1) As GraphicsData
            Return data.EnrichmentPlot(GO_terms, pvalue, size, tick)
        End Function
    End Module
End Namespace
