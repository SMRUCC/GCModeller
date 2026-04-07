#Region "Microsoft.VisualBasic::3da52dbdb858373a98c2d3bfe7f683c5, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\SOURCE\SOURCE.vb"

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

    '   Total Lines: 62
    '    Code Lines: 35 (56.45%)
    ' Comment Lines: 15 (24.19%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (19.35%)
    '     File Size: 1.94 KB


    '     Class SOURCE
    ' 
    '         Properties: BiomString, OrganismHierarchy, SpeciesName
    ' 
    '         Function: GetTaxonomy, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Metagenomics

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' 物种信息
    ''' </summary>
    Public Class SOURCE : Inherits KeyWord

        Public Property SpeciesName As String
        ''' <summary>
        ''' lineage
        ''' </summary>
        ''' <returns></returns>
        Public Property OrganismHierarchy As ORGANISM

        ''' <summary>
        ''' taxonomy lineage string in BIOM format
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BiomString As String
            Get
                Return BIOMTaxonomy.TaxonomyString(OrganismHierarchy.Lineage.Join(SpeciesName).ToArray)
            End Get
        End Property

        ''' <summary>
        ''' Get ncbi taxonomy information
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetTaxonomy() As Metagenomics.Taxonomy
            Dim tax = OrganismHierarchy.ToTaxonomy
            Dim strain = gb.SourceFeature.Query("strain")

            If Not strain.StringEmpty Then
                tax.scientificName = tax.scientificName & " " & strain
            End If

            tax.ncbi_taxid = gb.Taxon

            Return tax
        End Function

        Public Overrides Function ToString() As String
            Return OrganismHierarchy.ToString
        End Function

        Public Shared Widening Operator CType(str As String()) As SOURCE
            Dim source As New SOURCE

            If Not str.IsNullOrEmpty Then
                Call __trimHeadKey(str)
                source.SpeciesName = str.First
                source.OrganismHierarchy = ORGANISM.InternalParser(str.Skip(1).ToArray)
            End If

            Return source
        End Operator
    End Class
End Namespace
