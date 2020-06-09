Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module GenericBackground

    ''' <summary>
    ''' Create the KO generic background
    ''' </summary>
    ''' <param name="KO_terms"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CreateKOGeneric(KO_terms As String(), kegg As IEnumerable(Of Map), nsize As Integer) As Background
        Return GSEA.CreateBackground(
            db:=KO_terms,
            createGene:=AddressOf createTermGenericGene,
            getTerms:=Function(term) {term},
            define:=GSEA.KEGGClusters(kegg),
            genomeName:="generic",
            taxonomy:="generic",
            outputAll:=False
        ).With(Sub(background)
                   background.size = nsize
               End Sub)
    End Function

    Private Function createTermGenericGene(term As String, terms As String()) As BackgroundGene
        Return New BackgroundGene With {
            .accessionID = term,
            .[alias] = terms,
            .locus_tag = New NamedValue With {
                .name = term,
                .text = term
            },
            .name = term,
            .term_id = terms
        }
    End Function

    <Extension>
    Public Function CreateGOGeneric(GO_terms As IEnumerable(Of String), go As GetClusterTerms, nsize As Integer) As Background
        Return GSEA.CreateBackground(
            db:=GO_terms,
            createGene:=AddressOf createTermGenericGene,
            getTerms:=Function(term) {term},
            define:=go,
            genomeName:="generic",
            taxonomy:="generic",
            outputAll:=False
        ).With(Sub(background)
                   background.size = nsize
               End Sub)
    End Function
End Module
