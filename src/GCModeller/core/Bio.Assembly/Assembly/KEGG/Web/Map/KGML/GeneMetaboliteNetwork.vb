#Region "Microsoft.VisualBasic::20f4b8452869ef03d29577aefb1daf6b, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\GeneMetaboliteNetwork.vb"

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

    '   Total Lines: 177
    '    Code Lines: 93 (52.54%)
    ' Comment Lines: 67 (37.85%)
    '    - Xml Docs: 79.10%
    ' 
    '   Blank Lines: 17 (9.60%)
    '     File Size: 8.71 KB


    '     Class GeneMetaboliteNetwork
    ' 
    '         Properties: compound_id, compound_name, gene_id, ko_id, pathway_id
    '                     pathway_title, reaction_id, role
    ' 
    '         Function: ExtractNetwork
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' A network of gene -> compound that is extracted from the KGML metabolic reaction data.
    ''' 
    ''' Each KGML &lt;reaction> element connects a set of metabolites (substrates and products)
    ''' and is linked to a gene entry (type="gene") by sharing the same id value. This model
    ''' expands the reaction network into a long-table form: every (gene_id, compound_id) pair
    ''' produces one row, so that it is friendly for CSV export and downstream network analysis.
    ''' </summary>
    Public Class GeneMetaboliteNetwork

        ''' <summary>
        ''' The gene identifier(s) of the enzyme entry that catalyzes the reaction. In the original
        ''' KGML entry this is the ``name`` attribute of a ``type="gene"`` entry, kept in its full
        ''' KEGG form (e.g. ``taes:123057580``).
        ''' </summary>
        ''' <returns></returns>
        Public Property gene_id As String
        ''' <summary>
        ''' The KO identifier (e.g. ``ko:K20039``) resolved from the ortholog entry that shares the
        ''' same reaction id, or an empty string when no ortholog mapping is available in the
        ''' species-specific pathway map.
        ''' </summary>
        ''' <returns></returns>
        Public Property ko_id As String
        ''' <summary>
        ''' The metabolite identifier (e.g. ``C00082``) referenced by a substrate or product node.
        ''' </summary>
        ''' <returns></returns>
        Public Property compound_id As String
        Public Property compound_name As String
        ''' <summary>
        ''' The reaction identifier, kept in its full KEGG form (e.g. ``rn:R00737``). A reaction may
        ''' reference several rn ids separated by spaces.
        ''' </summary>
        ''' <returns></returns>
        Public Property reaction_id As String
        ''' <summary>
        ''' The pathway identifier (e.g. ``path:taes00940``).
        ''' </summary>
        ''' <returns></returns>
        Public Property pathway_id As String
        ''' <summary>
        ''' The pathway title / name.
        ''' </summary>
        ''' <returns></returns>
        Public Property pathway_title As String
        ''' <summary>
        ''' The role of the metabolite in the reaction: ``substrate`` or ``product``.
        ''' </summary>
        ''' <returns></returns>
        Public Property role As String

        ''' <summary>
        ''' Extract the gene-compound metabolic reaction network from a loaded KGML pathway object.
        ''' </summary>
        ''' <param name="kgml">The pathway object loaded by <see cref="pathway.LoadMap"/>.</param>
        ''' <param name="combineGenes">
        ''' When <see langword="True"/>, all gene identifiers on a single gene node are joined into
        ''' one unified id (separated by semicolons) placed in <see cref="gene_id"/>; otherwise each
        ''' gene identifier is emitted as a separate row (the default long-table behavior).
        ''' </param>
        ''' <returns>
        ''' A long-table enumeration, one row per (gene_id, compound_id) pair, with the reaction
        ''' direction recorded in <see cref="role"/>.
        ''' </returns>
        Public Shared Iterator Function ExtractNetwork(kgml As pathway, Optional combineGenes As Boolean = False) As IEnumerable(Of GeneMetaboliteNetwork)
            If kgml Is Nothing OrElse kgml.reactions Is Nothing Then
                Return
            End If

            ' Index entries by their id so that we can resolve the gene entry of each reaction
            ' in O(1). In KGML the <reaction id> always equals the id of its catalyzing gene
            ' entry (verified across all target taes pathway maps).
            Dim entryById = kgml.entries.SafeQuery.ToDictionary(Function(e) e.id)

            ' Build a reaction-id -> ko list index from the ortholog entries. Ortholog entries
            ' carry their catalyzed reaction(s) in the "reaction" attribute and the KO ids in the
            ' "name" attribute. This mirrors the existing ReactionNetworkExport approach.
            Dim koByRn = kgml.entries _
                .SafeQuery _
                .Where(Function(e) e.type = "ortholog") _
                .Select(Function(o)
                            Dim kos = o.name.SafeQuery.Select(Function(n) n.GetTagValue(":").Value).ToArray
                            Return o.reaction.StringSplit(" ").Select(Function(rid) (rid, kos))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(t) t.rid) _
                .ToDictionary(Function(g) g.Key,
                              Function(g)
                                  Return g.SelectMany(Function(t) t.kos).Distinct.ToArray
                              End Function)

            For Each rxn As reaction In kgml.reactions
                ' Resolve the catalyzing gene entry via the shared id.
                Dim geneEntry = entryById.TryGetValue(rxn.id)

                If geneEntry Is Nothing OrElse geneEntry.type <> "gene" Then
                    ' If no gene entry shares the reaction id, fall back to entries that declare
                    ' the same reaction id in their "reaction" attribute.
                    geneEntry = kgml.entries _
                        .SafeQuery _
                        .Where(Function(e) e.type = "gene" AndAlso e.reaction.StringSplit(" ").Contains(rxn.name)) _
                        .FirstOrDefault
                End If

                If geneEntry Is Nothing Then
                    Continue For
                End If

                Dim genes = geneEntry.name _
                    .SafeQuery _
                    .Select(Function(n) n.Trim) _
                    .Where(Function(n) n.Length > 0) _
                    .Distinct _
                    .ToArray

                ' When combineGenes is enabled, all gene identifiers on this node are merged into a
                ' single unified id (semicolon separated); otherwise each gene keeps its own row.
                Dim geneIds As String() = If(combineGenes, {String.Join(";", genes)}, genes)

                ' Resolve KO ids from any of the reaction's rn identifiers.
                Dim kos = rxn.name _
                    .StringSplit(" ") _
                    .SelectMany(Function(rid) koByRn.TryGetValue(rid).SafeQuery) _
                    .Distinct _
                    .ToArray

                Dim pathwayId = kgml.name
                Dim pathwayTitle = kgml.title
                Dim reactionId = rxn.name

                ' Substrates and products are expanded into gene x compound pairs.
                For Each c As compound In rxn.substrates.SafeQuery
                    Dim compoundId = c.name.GetTagValue(":").Value
                    Dim name As String = GeneNetworkExport.cpdNames(compoundId)

                    For Each geneId In geneIds
                        Yield New GeneMetaboliteNetwork With {
                            .gene_id = geneId,
                            .ko_id = If(kos.Any, kos(0), ""),
                            .compound_id = compoundId,
                            .reaction_id = reactionId,
                            .pathway_id = pathwayId,
                            .pathway_title = pathwayTitle,
                            .role = "substrate",
                            .compound_name = name
                        }
                    Next
                Next

                For Each c As compound In rxn.products.SafeQuery
                    Dim compoundId = c.name.GetTagValue(":").Value
                    Dim name As String = GeneNetworkExport.cpdNames(compoundId)

                    For Each geneId In geneIds
                        Yield New GeneMetaboliteNetwork With {
                            .gene_id = geneId,
                            .ko_id = If(kos.Any, kos(0), ""),
                            .compound_id = compoundId,
                            .reaction_id = reactionId,
                            .pathway_id = pathwayId,
                            .pathway_title = pathwayTitle,
                            .role = "product",
                            .compound_name = name
                        }
                    Next
                Next
            Next
        End Function
    End Class
End Namespace

