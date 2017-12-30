Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
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
                                   Return .Intersect(kolist).Count / .Count >= cutoff
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

Public Class TaxonomyIndex

    <XmlElement(NameOf(ref))>
    Public Property ref As Summary()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uniprotRef$"><see cref="TaxonomyRepository"/></param>
    ''' <param name="maps$"><see cref="MapRepository"/></param>
    ''' <returns></returns>
    Public Shared Function Summary(uniprotRef$, maps$) As TaxonomyIndex
        Return New TaxonomyIndex With {
            .ref = TaxonomyIndexExtensions _
                .IteratesModels(uniprotRef) _
                .Summary(ref:=maps.LoadXml(Of MapRepository)) _
                .ToArray
        }
    End Function
End Class

''' <summary>
''' 用于加速<see cref="PathwayProfile"/>计算的已经预先计算好的基因组摘要数据
''' </summary>
Public Class Summary

    Public Property ncbi_taxon_id As String
    Public Property scientificName As String
    Public Property lineageGroup As NamedValue()
    Public Property Maps As String()

    Public Overrides Function ToString() As String
        Return scientificName
    End Function
End Class