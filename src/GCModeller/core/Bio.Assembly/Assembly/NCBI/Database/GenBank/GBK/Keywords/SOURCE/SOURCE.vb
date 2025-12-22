#Region "Microsoft.VisualBasic::0e058eebdc6ec0ce1b9a5a9c813ad9af, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\SOURCE\SOURCE.vb"

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

    '   Total Lines: 45
    '    Code Lines: 29 (64.44%)
    ' Comment Lines: 7 (15.56%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (20.00%)
    '     File Size: 1.42 KB


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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetTaxonomy() As Metagenomics.Taxonomy
            Return OrganismHierarchy.ToTaxonomy
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
