#Region "Microsoft.VisualBasic::1c46a3df6bdd5ab410b4bb6e17484b67, GCModeller\models\Networks\Microbiome\UniProt\TaxonomyIndexExtensions.vb"

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

    '   Total Lines: 65
    '    Code Lines: 58
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 2.57 KB


    ' Module TaxonomyIndexExtensions
    ' 
    '     Function: IteratesModels, Summary
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Metagenomics
Imports XmlLinq = Microsoft.VisualBasic.Text.Xml.Linq.Data

Public Module TaxonomyIndexExtensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IteratesModels(path As String) As IEnumerable(Of TaxonomyRef)
        Return XmlLinq.LoadXmlDataSet(Of TaxonomyRef)(path, "taxonomy", forceLargeMode:=True)
    End Function

    <Extension>
    Public Iterator Function Summary(genomes As IEnumerable(Of TaxonomyRef), ref As MapRepository, Optional cutoff# = 0.3) As IEnumerable(Of Summary)
        Dim ranks As TaxonomyRanks() = {
            TaxonomyRanks.Class,
            TaxonomyRanks.Order,
            TaxonomyRanks.Family,
            TaxonomyRanks.Genus,
            TaxonomyRanks.Species
        }

        For Each genome As TaxonomyRef In genomes
            With genome
                If .genome.Terms.IsNullOrEmpty Then
                    Continue For
                End If

                Dim kolist$() = .genome.EntityList
                Dim maps = ref _
                    .QueryMapsByMembers(kolist) _
                    .Where(Function(map)
                               With map.KOIndex
                                   Return .Intersect(collection:=kolist).Count / .Count >= cutoff
                               End With
                           End Function) _
                    .Keys

                If maps.IsNullOrEmpty Then
                    Continue For
                Else
                    Dim lineage = New gast.Taxonomy(.TaxonomyString)
                    Dim groups = ranks _
                        .Select(Function(rank)
                                    Return New NamedValue With {
                                        .name = rank.ToString,
                                        .text = lineage.BIOMTaxonomyString(rank)
                                    }
                                End Function) _
                        .ToArray

                    Yield New Summary With {
                        .ncbi_taxon_id = genome.TaxonID,
                        .Maps = maps,
                        .scientificName = genome.organism.scientificName,
                        .lineageGroup = groups
                    }
                End If
            End With
        Next
    End Function
End Module
