#Region "Microsoft.VisualBasic::806c045eaab53292627d2b5516d815e9, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\SOURCE\ORGANISM.vb"

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

    '   Total Lines: 43
    '    Code Lines: 34
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.43 KB


    '     Class ORGANISM
    ' 
    '         Properties: Lineage, SpeciesName
    ' 
    '         Function: InternalParser, ToString, ToTaxonomy
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Taxon = SMRUCC.genomics.Metagenomics.Taxonomy

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    Public Class ORGANISM

        Public Property Lineage As String()
        Public Property SpeciesName As String

        Public Function ToTaxonomy() As Taxon
            Dim i As i32 = Scan0

            Return New Taxon With {
                .scientificName = SpeciesName,
                .kingdom = Lineage.ElementAtOrDefault(++i),
                .phylum = Lineage.ElementAtOrDefault(++i),
                .[class] = Lineage.ElementAtOrDefault(++i),
                .order = Lineage.ElementAtOrDefault(++i),
                .family = Lineage.ElementAtOrDefault(++i),
                .genus = Lineage.ElementAtOrDefault(++i),
                .species = Lineage.ElementAtOrDefault(++i)
            }
        End Function

        Public Overrides Function ToString() As String
            Return SpeciesName
        End Function

        Friend Shared Function InternalParser(str As String()) As ORGANISM
            Call KeyWord.__trimHeadKey(str)

            Dim lineage As New ORGANISM With {
                .SpeciesName = str.First,
                .Lineage = (From s As String In str.Skip(1) Select s.Trim) _
                    .JoinBy(" ") _
                    .StringSplit("[;]\s*")
            }

            Return lineage
        End Function
    End Class
End Namespace
