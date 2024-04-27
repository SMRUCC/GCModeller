#Region "Microsoft.VisualBasic::1de4717f55dbea70112fa188ccfb28e5, G:/GCModeller/src/GCModeller/core/Bio.Assembly//Metagenomics/TaxonomyRanks.vb"

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

    '   Total Lines: 44
    '    Code Lines: 15
    ' Comment Lines: 27
    '   Blank Lines: 2
    '     File Size: 1.24 KB


    '     Enum TaxonomyRanks
    ' 
    '         NA, Strain
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Namespace Metagenomics

    ''' <summary>
    ''' the organism taxonomy rank levels
    ''' </summary>
    ''' <remarks>
    ''' 枚举值减去100即可得到index值
    ''' </remarks>
    Public Enum TaxonomyRanks As Integer
        NA
        ''' <summary>
        ''' 1. 界
        ''' </summary>
        <Description(NcbiTaxonomyTree.superkingdom)> Kingdom = 100
        ''' <summary>
        ''' 2. 门
        ''' </summary>
        <Description(NcbiTaxonomyTree.phylum)> Phylum
        ''' <summary>
        ''' 3A. 纲
        ''' </summary>
        <Description(NcbiTaxonomyTree.class)> [Class]
        ''' <summary>
        ''' 4B. 目
        ''' </summary>
        <Description(NcbiTaxonomyTree.order)> Order
        ''' <summary>
        ''' 5C. 科
        ''' </summary>
        <Description(NcbiTaxonomyTree.family)> Family
        ''' <summary>
        ''' 6D. 属
        ''' </summary>
        <Description(NcbiTaxonomyTree.genus)> Genus
        ''' <summary>
        ''' 7E. 种
        ''' </summary>
        <Description(NcbiTaxonomyTree.species)> Species
        Strain
    End Enum
End Namespace
