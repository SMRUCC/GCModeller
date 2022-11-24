Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports Microsoft.VisualBasic.Linq
Imports System.Runtime.CompilerServices

''' <summary>
''' helper for create enrichment background model for multiple omics data analysis
''' </summary>
Public Module MultipleOmics

    <Extension>
    Public Function CreateOmicsBackground(model As IEnumerable(Of Pathway)) As Background
        Dim clusters As Cluster() = model _
            .Select(Function(m) getCluster(m)) _
            .Where(Function(c) c.size > 0 AndAlso Not c.ID.StringEmpty) _
            .ToArray

        Return New Background With {
            .clusters = clusters,
            .build = Now,
            .comments = "KEGG pathway multiple omics analysis",
            .id = "",
            .name = "",
            .size = .clusters.BackgroundSize
        }
    End Function

    ''' <summary>
    ''' create background model of combine genes with compounds
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    Private Function getCluster(model As Pathway) As Cluster
        Dim molecules As New List(Of BackgroundGene)(model.GetGeneMembers)

        For Each compound As NamedValue In model.compound.SafeQuery
            molecules.Add(New BackgroundGene With {
                .accessionID = compound.name,
                .[alias] = {compound.name},
                .term_id = {compound.name},
                .name = compound.text,
                .locus_tag = compound
            })
        Next

        Return New Cluster With {
            .ID = model.EntryId,
            .names = model.name,
            .description = model.description,
            .members = molecules.ToArray
        }
    End Function
End Module