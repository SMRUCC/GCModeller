Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

Public Module Reconstruction

    ''' <summary>
    ''' create kegg reaction index by kegg reaction id/KO/ECnumber
    ''' </summary>
    ''' <param name="reactions"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateIndex(reactions As IEnumerable(Of ReactionTable)) As Dictionary(Of String, ReactionTable())
        Return reactions _
            .Select(Function(r)
                        Return {r.entry} _
                            .JoinIterates(r.KO) _
                            .JoinIterates(r.EC) _
                            .Select(Function(id) (id, r))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(obj) obj.id) _
            .ToDictionary(Function(id) id.Key,
                          Function(g)
                              Return g.Select(Function(obj) obj.r).ToArray
                          End Function)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pathway"></param>
    ''' <param name="reactions"></param>
    ''' <param name="names">the names list of the kegg compounds</param>
    ''' <returns></returns>
    <Extension>
    Public Function AssignCompounds(pathway As DBGET.bGetObject.Pathway, reactions As Dictionary(Of String, ReactionTable()), Optional names As Dictionary(Of String, String) = Nothing) As DBGET.bGetObject.Pathway
        Dim fluxInMap = pathway.modules _
            .Where(Function(id) reactions.ContainsKey(id.name)) _
            .Select(Function(id) reactions(id.name)) _
            .IteratesALL _
            .ToArray
        Dim enzymes As EnzymeEntry() = EnzymeEntry.ParseEntries

        If names Is Nothing Then
            names = New Dictionary(Of String, String)
        End If

        pathway.compound = fluxInMap _
            .Select(Function(rxn) rxn.substrates.AsList + rxn.products) _
            .IteratesALL _
            .Distinct _
            .Select(Function(cid)
                        Return New NamedValue With {
                            .name = cid,
                            .text = names.TryGetValue(cid)
                        }
                    End Function) _
            .ToArray
        pathway.modules = Nothing

        Return pathway
    End Function

    <Extension>
    Public Iterator Function KEGGReconstruction(reference As IEnumerable(Of Map),
                                                genes As IEnumerable(Of ProteinAnnotation),
                                                Optional min_cov As Double = 0.3) As IEnumerable(Of DBGET.bGetObject.Pathway)

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
        Dim mapReconstruct As New Value(Of DBGET.bGetObject.Pathway)

        For Each map As Map In reference
            If Not mapReconstruct = map.KEGGReconstruction(KOindex, min_cov) Is Nothing Then
                Yield mapReconstruct
            End If
        Next
    End Function

    <Extension>
    Private Function KEGGReconstruction(map As Map, KOindex As Dictionary(Of String, ProteinAnnotation()), min_cov#) As DBGET.bGetObject.Pathway
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
            Return map.createPathwayModel(proteins, idIndex)
        Else
            Return Nothing
        End If
    End Function

    <Extension>
    Private Function createPathwayModel(map As Map, proteins As ProteinAnnotation(), idIndex As IEnumerable(Of String)) As DBGET.bGetObject.Pathway
        Dim kopathway As NamedValue() = proteins _
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
            .ToArray

        Return New DBGET.bGetObject.Pathway With {
            .description = map.Name,
            .EntryId = map.id,
            .name = map.Name,
            .KOpathway = kopathway,
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
                .Select(Function(id)
                            Return New NamedValue With {
                                .name = id
                            }
                        End Function) _
                .ToArray
        }
    End Function
End Module
