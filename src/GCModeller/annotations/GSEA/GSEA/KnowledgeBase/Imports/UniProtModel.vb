#Region "Microsoft.VisualBasic::f800b51c20dc4f602d1830ed6ef8fe8d, GCModeller\annotations\GSEA\GSEA\KnowledgeBase\Imports\UniProtModel.vb"

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

    '   Total Lines: 148
    '    Code Lines: 125
    ' Comment Lines: 3
    '   Blank Lines: 20
    '     File Size: 5.29 KB


    ' Module UniProtModel
    ' 
    '     Function: extractKeywords, extractLocations, proteinLocusTag, SubcellularLocation, uniprotGeneModel
    '               UniprotGoHits, UniprotKeywordsModel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Module UniProtModel

    Public Iterator Function UniprotGoHits(uniprot As IEnumerable(Of entry)) As IEnumerable(Of BackgroundGene)
        Dim extractTerms = UniProtGetGOTerms()
        Dim go_terms As String()

        For Each protein In uniprot
            go_terms = extractTerms(protein)

            If Not go_terms.IsNullOrEmpty Then
                Yield protein.uniprotGeneModel(go_terms)
            End If
        Next
    End Function

    <Extension>
    Public Function SubcellularLocation(uniprot As IEnumerable(Of entry)) As Background
        Dim locations = uniprot _
            .Select(Function(i) i.extractLocations) _
            .IteratesALL _
            .GroupBy(Function(i) i.Name) _
            .ToArray
        Dim clusters = locations _
            .Select(Function(c)
                        Return New Cluster With {
                            .ID = c.Key,
                            .members = c _
                                .Select(Function(i) i.Value) _
                                .GroupBy(Function(i) i.accessionID) _
                                .Select(Function(a) a.First) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Return New Background With {
            .build = Now,
            .clusters = clusters
        }
    End Function

    <Extension>
    Public Function UniprotKeywordsModel(uniprot As IEnumerable(Of entry)) As Background
        Dim words = uniprot _
            .Select(Function(i) i.extractKeywords) _
            .IteratesALL _
            .GroupBy(Function(i) i.Name) _
            .ToArray
        Dim clusters = words _
            .Select(Function(c)
                        Return New Cluster With {
                            .ID = c.Key,
                            .members = c _
                                .Select(Function(i) i.Value) _
                                .GroupBy(Function(i) i.accessionID) _
                                .Select(Function(a) a.First) _
                                .ToArray,
                            .names = c.First.Description,
                            .description = .names
                        }
                    End Function) _
            .ToArray

        Return New Background With {
            .build = Now,
            .clusters = clusters
        }
    End Function

    <Extension>
    Private Iterator Function extractKeywords(protein As entry) As IEnumerable(Of NamedValue(Of BackgroundGene))
        Dim keywords = protein.keywords
        Dim gene As BackgroundGene = protein.uniprotGeneModel

        If Not keywords.IsNullOrEmpty Then
            For Each word In keywords
                Yield New NamedValue(Of BackgroundGene) With {
                    .Description = word.value,
                    .Name = word.id,
                    .Value = gene
                }
            Next
        End If
    End Function

    ''' <summary>
    ''' removes EMBL id due to the reason of too much id that can be extract from this kind of id
    ''' </summary>
    ReadOnly xrefDbNames As Index(Of String) = {"RefSeq", "AlphaFoldDB", "STRING", "Ensembl", "UCSC", "eggNOG", "GeneTree", "Bgee"}

    <Extension>
    Friend Function proteinLocusTag(protein As entry, accessionID$) As NamedValue
        Dim tag$ = accessionID

        If protein.xrefs.ContainsKey("KEGG") Then
            tag = protein.xrefs("KEGG").First.id
        End If

        Return New NamedValue With {
            .name = tag,
            .text = protein.protein.fullName
        }
    End Function

    <Extension>
    Friend Function uniprotGeneModel(protein As entry, Optional terms As String() = Nothing) As BackgroundGene
        Dim dbxref = protein.dbReferences _
            .Where(Function(id)
                       Return id.type Like xrefDbNames
                   End Function) _
            .Select(Function(i) i.id) _
            .Distinct _
            .ToArray

        Return New BackgroundGene With {
            .accessionID = protein.accessions.First,
            .[alias] = protein.accessions.JoinIterates(dbxref).ToArray,
            .locus_tag = protein.proteinLocusTag(.accessionID),
            .name = protein.name,
            .term_id = terms
        }
    End Function

    <Extension>
    Private Iterator Function extractLocations(protein As entry) As IEnumerable(Of NamedValue(Of BackgroundGene))
        Dim locs As comment() = protein.CommentList.TryGetValue("subcellular location")
        Dim gene As BackgroundGene = protein.uniprotGeneModel

        If Not locs.IsNullOrEmpty Then
            For Each location As value In locs _
                .Select(Function(l) l.subcellularLocations) _
                .IteratesALL _
                .Select(Function(l) l.locations) _
                .IteratesALL

                Yield New NamedValue(Of BackgroundGene)(location.value, gene)
            Next
        End If
    End Function

End Module
