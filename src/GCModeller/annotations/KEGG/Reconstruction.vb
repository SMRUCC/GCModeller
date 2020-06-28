Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

Public Module Reconstruction

    <Extension>
    Public Function AssignCompounds(pathway As Pathway, reactions As Dictionary(Of String, ReactionTable())) As Pathway
        Dim fluxInMap = pathway.modules _
            .Where(Function(id) reactions.ContainsKey(id.name)) _
            .Select(Function(id) reactions(id.name)) _
            .IteratesALL _
            .ToArray

        pathway.compound = fluxInMap _
            .Select(Function(rxn) rxn.substrates.AsList + rxn.products) _
            .IteratesALL _
            .Distinct _
            .Select(Function(cid) New NamedValue With {.name = cid}) _
            .ToArray
        pathway.modules = Nothing

        Return pathway
    End Function

    <Extension>
    Public Iterator Function KEGGReconstruction(reference As IEnumerable(Of Map),
                                                genes As IEnumerable(Of ProteinAnnotation),
                                                Optional min_cov As Double = 0.3) As IEnumerable(Of Pathway)

        Dim KOindex As Dictionary(Of String, ProteinAnnotation()) = genes _
            .Where(Function(g) g.attributes.ContainsKey("ko")) _
            .Select(Function(g)
                        Return g.attributes("ko").Select(Function(ko) (ko, g))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(g) g.ko) _
            .ToDictionary(Function(ko) ko.Key,
                          Function(g)
                              Return g.Select(Function(i) i.g).ToArray
                          End Function)
        Dim mapReconstruct As New Value(Of Pathway)

        For Each map As Map In reference
            If Not mapReconstruct = map.KEGGReconstruction(KOindex, min_cov) Is Nothing Then
                Yield mapReconstruct
            End If
        Next
    End Function

    <Extension>
    Private Function KEGGReconstruction(map As Map, KOindex As Dictionary(Of String, ProteinAnnotation()), min_cov#) As Pathway
        Dim all As Integer = 0
        Dim hits As New List(Of ProteinAnnotation())
        Dim objs As String()
        Dim idIndex As New List(Of String)

        For Each entity As Area In map.shapes
            objs = entity.IDVector

            If objs.Any(Function(id) id.IsPattern("K\d+")) Then
                Dim tmp As New List(Of ProteinAnnotation)

                For Each id As String In objs
                    If KOindex.ContainsKey(id) Then
                        tmp += KOindex(id)
                    End If
                Next

                If tmp > 0 Then
                    idIndex += objs
                    hits += tmp.ToArray
                End If

                all += 1
            End If
        Next

        Dim coverage As Double = hits.Count / all
        Dim proteins As ProteinAnnotation() = hits _
            .IteratesALL _
            .GroupBy(Function(g) g.geneId) _
            .Select(Function(g) g.First) _
            .ToArray

        If coverage >= min_cov Then
            Return New Pathway With {
                .description = map.Name,
                .EntryId = map.id,
                .name = map.Name,
                .KOpathway = proteins _
                    .Select(Function(prot)
                                Return prot.attributes("ko") _
                                    .Select(Function(ko)
                                                Return New NamedValue With {
                                                    .name = ko,
                                                    .text = prot.description
                                                }
                                            End Function)
                            End Function) _
                    .IteratesALL _
                    .ToArray,
                .genes = proteins _
                    .Select(Function(g)
                                Return New NamedValue With {
                                    .name = g.geneId,
                                    .text = g.description
                                }
                            End Function) _
                    .ToArray,
                .modules = idIndex _
                    .Distinct _
                    .Select(Function(id) New NamedValue With {.name = id}) _
                    .ToArray
            }
        Else
            Return Nothing
        End If
    End Function
End Module
