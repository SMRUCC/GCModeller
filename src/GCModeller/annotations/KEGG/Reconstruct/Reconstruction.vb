#Region "Microsoft.VisualBasic::0a44bdef08f9dd677842be43230fc371, GCModeller\annotations\KEGG\Reconstruct\Reconstruction.vb"

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

'   Total Lines: 268
'    Code Lines: 200
' Comment Lines: 38
'   Blank Lines: 30
'     File Size: 10.92 KB


' Module Reconstruction
' 
'     Function: AssignCompounds, CreateIndex, createPathwayModel, GetEnzymeNumbers, GetFluxInMaps
'               (+2 Overloads) KEGGReconstruction
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
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

    <Extension>
    Private Iterator Function PopulateIdSet(r As ReactionTable, indexByCompounds As Boolean) As IEnumerable(Of String())
        Yield {r.entry}
        Yield r.EC
        Yield r.KO
        Yield r.geneNames

        If indexByCompounds Then
            Yield r.products
            Yield r.substrates
        End If
    End Function

    ''' <summary>
    ''' create kegg reaction index by kegg reaction id/KO/ECnumber
    ''' </summary>
    ''' <param name="reactions"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateIndex(reactions As IEnumerable(Of ReactionTable), Optional indexByCompounds As Boolean = False) As Dictionary(Of String, ReactionTable())
        Return reactions _
            .Select(Function(r)
                        Return r.PopulateIdSet(indexByCompounds) _
                            .IteratesALL _
                            .Distinct _
                            .Select(Function(id) (id, r))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(obj) obj.id) _
            .ToDictionary(Function(id) id.Key,
                          Function(g)
                              Return g.Select(Function(obj) obj.r).ToArray
                          End Function)
    End Function

    <Extension>
    Public Function GetEnzymeNumbers(pathway As DBGET.bGetObject.Pathway) As IEnumerable(Of NamedValue)
        Static enzymes As Dictionary(Of String, EnzymeEntry()) = EnzymeEntry _
            .ParseEntries _
            .GroupBy(Function(a) a.KO) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)

        Dim rawEntries = pathway.KOpathway _
            .SafeQuery _
            .Where(Function(ko) enzymes.ContainsKey(ko.name)) _
            .Select(Function(ko) enzymes(ko.name)) _
            .IteratesALL _
            .ToArray

        Return rawEntries _
            .Select(Function(a)
                        Return New NamedValue With {
                            .name = a.EC,
                            .text = a.fullName
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' we are not going to add the non-enzymics reaction into each pathway map
    ''' because this operation will caused all of the pathway map contains the 
    ''' similar compound profile which is bring by all of the non-enzymics reactions.
    ''' </summary>
    ''' <param name="pathway"></param>
    ''' <param name="reactions"></param>
    ''' <param name="enzymes"></param>
    ''' <returns></returns>
    <Extension>
    Private Function GetFluxInMaps(pathway As DBGET.bGetObject.Pathway, reactions As Dictionary(Of String, ReactionTable()), Optional ByRef enzymes As NamedValue() = Nothing) As IEnumerable(Of ReactionTable)
        Dim koMaps = pathway.modules _
            .Where(Function(id) reactions.ContainsKey(id.name)) _
            .Select(Function(id) reactions(id.name)) _
            .IteratesALL _
            .AsList

        enzymes = GetEnzymeNumbers(pathway).ToArray

        Dim enzymeMaps As ReactionTable() = enzymes _
            .Select(Function(a) reactions.TryGetValue(a.name)) _
            .IteratesALL _
            .ToArray

        ' non-enzymatic
        'Dim none As ReactionTable() = reactions.Values _
        '    .IteratesALL _
        '    .Where(Function(a)
        '               Return a.KO.IsNullOrEmpty AndAlso a.EC.IsNullOrEmpty
        '           End Function) _
        '    .ToArray

        Return koMaps + enzymeMaps ' + none
    End Function

    ''' <summary>
    ''' Assign a compound list to ``<see cref="DBGET.bGetObject.Pathway.compound"/>``.
    ''' </summary>
    ''' <param name="pathway"></param>
    ''' <param name="reactions"></param>
    ''' <param name="names">the names list of the kegg compounds</param>
    ''' <param name="classes">
    ''' apply as the compund filter
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function AssignCompounds(pathway As DBGET.bGetObject.Pathway,
                                    reactions As Dictionary(Of String, ReactionTable()),
                                    Optional names As Dictionary(Of String, String) = Nothing,
                                    Optional classes As Dictionary(Of String, ReactionClassTable()) = Nothing) As DBGET.bGetObject.Pathway

        Dim enzymes As NamedValue() = Nothing
        Dim fluxInMap As ReactionTable() = GetFluxInMaps(pathway, reactions, enzymes).ToArray

        If names Is Nothing Then
            names = New Dictionary(Of String, String)
        End If

        pathway.compound = fluxInMap _
            .Select(Function(rxn)
                        Dim rawList As String() = rxn.substrates.AsList + rxn.products

                        If classes Is Nothing OrElse Not classes.ContainsKey(rxn.entry) Then
                            Return DirectCast(rawList, IEnumerable(Of String))
                        Else
                            Dim classList As ReactionClassTable() = classes(rxn.entry)
                            Dim compoundIndex As Index(Of String) = classList _
                                .Select(Iterator Function(c)
                                            Yield c.from
                                            Yield c.to
                                        End Function) _
                                .IteratesALL _
                                .Distinct _
                                .Indexing

                            Return rawList.Where(Function(id) id Like compoundIndex)
                        End If
                    End Function) _
            .IteratesALL _
            .Distinct _
            .Select(Function(cid)
                        Return New NamedValue With {
                            .name = cid,
                            .text = names.TryGetValue(cid)
                        }
                    End Function) _
            .ToArray
        pathway.modules = enzymes

        Return pathway
    End Function

    ''' <summary>
    ''' Reconstruct of the kegg pathway network based on the given protein annotation information.
    ''' </summary>
    ''' <param name="reference"></param>
    ''' <param name="genes"></param>
    ''' <param name="min_cov"></param>
    ''' <returns></returns>
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
            .description = map.name,
            .EntryId = map.EntryId,
            .name = map.name,
            .KOpathway = kopathway,
            .genes = proteins _
                .Select(Function(g)
                            Return New GeneName With {
                                .geneId = g.geneId,
                                .description = g.description
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
