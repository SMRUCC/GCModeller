#Region "Microsoft.VisualBasic::c94223f4b777969b3fabc721fdcdeeca, annotations\GSEA\GSEA\KnowledgeBase\Imports\UniProtModel.vb"

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

'   Total Lines: 151
'    Code Lines: 125 (82.78%)
' Comment Lines: 6 (3.97%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 20 (13.25%)
'     File Size: 5.61 KB


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
Imports keyword = SMRUCC.genomics.Assembly.Uniprot.XML.value

''' <summary>
''' data source is comes from the annotation metadata inside the uniprot database <see cref="entry"/>
''' </summary>
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

    ''' <summary>
    ''' Extract the uniprot keyword as protein function background model
    ''' </summary>
    ''' <param name="uniprot"></param>
    ''' <returns></returns>
    <Extension>
    Public Function UniprotKeywordsModel(uniprot As IEnumerable(Of entry), Optional db_xref As String = Nothing) As Background
        Dim words As NamedValue(Of BackgroundGene)() = uniprot _
            .Select(Function(i) i.extractKeywords(db_xref:=db_xref)) _
            .IteratesALL _
            .ToArray
        Dim clusters As Cluster() = words.buildKeywordClusters.ToArray

        Return New Background With {
            .build = Now,
            .clusters = clusters
        }
    End Function

    <Extension>
    Private Iterator Function buildKeywordClusters(words As IEnumerable(Of NamedValue(Of BackgroundGene))) As IEnumerable(Of Cluster)
        For Each cgroup As IGrouping(Of String, NamedValue(Of BackgroundGene)) In words.GroupBy(Function(i) i.Name)
            Yield New Cluster With {
                .ID = cgroup.Key,
                .members = cgroup _
                    .Select(Function(i) i.Value) _
                    .GroupBy(Function(i) i.accessionID) _
                    .Select(Function(a) a.First) _
                    .ToArray,
                .names = cgroup.First.Description,
                .description = .names
            }
        Next
    End Function

    <Extension>
    Private Iterator Function extractKeywords(protein As entry, db_xref As String) As IEnumerable(Of NamedValue(Of BackgroundGene))
        Dim keywords As keyword() = protein.keywords
        Dim gene As BackgroundGene = protein.uniprotGeneModel(db_xref:=db_xref)

        If Not keywords.IsNullOrEmpty Then
            For Each word As keyword In keywords
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
    Friend Function uniprotGeneModel(protein As entry, Optional terms As String() = Nothing, Optional db_xref As String = Nothing) As BackgroundGene
        Dim dbxref As String()

        If db_xref Is Nothing Then
            dbxref = protein.dbReferences _
                .Where(Function(id)
                           Return id.type Like xrefDbNames
                       End Function) _
                .Select(Function(i) i.id) _
                .Distinct _
                .ToArray
            dbxref = protein.accessions _
                .JoinIterates(dbxref) _
                .ToArray
        Else
            dbxref = protein _
                .DbReferenceIds(db_xref) _
                .ToArray
        End If

        Return New BackgroundGene With {
            .accessionID = protein.accessions.First,
            .[alias] = dbxref,
            .locus_tag = protein.proteinLocusTag(.accessionID),
            .name = protein.name,
            .term_id = BackgroundGene.UnknownTerms(terms).ToArray
        }
    End Function

    <Extension>
    Private Iterator Function extractLocations(protein As entry) As IEnumerable(Of NamedValue(Of BackgroundGene))
        Dim locs As comment() = protein.CommentList.TryGetValue("subcellular location")
        Dim gene As BackgroundGene = protein.uniprotGeneModel

        If Not locs.IsNullOrEmpty Then
            For Each location As keyword In locs _
                .Select(Function(l) l.subcellularLocations) _
                .IteratesALL _
                .Select(Function(l) l.locations) _
                .IteratesALL

                Yield New NamedValue(Of BackgroundGene)(location.value, gene)
            Next
        End If
    End Function

End Module
