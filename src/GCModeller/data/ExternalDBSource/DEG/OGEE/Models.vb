#Region "Microsoft.VisualBasic::399ac73e1e9df9f52687cbf8e9b722a5, ExternalDBSource\DEG\OGEE\Models.vb"

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

    '     Class datasets
    ' 
    '         Properties: datasetID, dataSource, dataSubType, dataType, dateadded
    '                     definitionOfEssential, experimentalConditionBrief, experimentalTechBrief, kingdom, note
    '                     sciName, taxID, url
    ' 
    '     Class gene_essentiality
    ' 
    '         Properties: datasetID, essential, fitnessScore, id, kingdom
    '                     locus, pubmedID, sciName, taxID, thumbdown
    '                     thumbup, valid
    ' 
    '     Class genes
    ' 
    '         Properties: codingSeq, description, kingdom, locus, proteinLength
    '                     proteinSeq, sciName, seqType, symbols, taxID
    ' 
    '     Class geneSetInfo
    ' 
    '         Properties: dataset, essentiality, gene
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DEG.OGEE.Models

    Public Class datasets
        Public Property sciName As String
        Public Property kingdom As String
        Public Property taxID As String
        Public Property dataSource As String
        Public Property url As String
        Public Property dataType As String
        Public Property dataSubType As String
        Public Property experimentalTechBrief As String
        Public Property experimentalConditionBrief As String
        Public Property definitionOfEssential As String
        Public Property note As String
        Public Property dateadded As String
        Public Property datasetID As String
    End Class

    Public Class gene_essentiality
        Public Property sciName As String
        Public Property kingdom As String
        Public Property datasetID As String
        Public Property locus As String
        Public Property essential As String
        Public Property pubmedID As String
        Public Property taxID As String
        Public Property thumbup As String
        Public Property thumbdown As String
        Public Property valid As String
        Public Property fitnessScore As String
        Public Property id As String
    End Class

    Public Class genes
        Public Property sciName As String
        Public Property kingdom As String
        Public Property taxID As String
        Public Property locus As String
        Public Property symbols As String
        Public Property description As String
        Public Property proteinSeq As String
        Public Property codingSeq As String
        Public Property seqType As String
        Public Property proteinLength As String

    End Class

    ''' <summary>
    ''' Table join of <see cref="genes"/>, <see cref="gene_essentiality"/>, <see cref="datasets"/>
    ''' </summary>
    Public Class geneSetInfo

        Public Property dataset As datasets
        Public Property essentiality As gene_essentiality
        Public Property gene As genes

    End Class
End Namespace
