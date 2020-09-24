#Region "Microsoft.VisualBasic::00e2a15031cd44411c4658400d33d294, data\KEGG\Procedures\DumpProcedures.vb"

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

    ' Module DumpProcedures
    ' 
    '     Sub: DumpCompounds, DumpKO, DumpReactions, DumpReferencePathwayMap
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.DATA
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Scripting
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Public Module DumpProcedures

    Public Sub DumpReferencePathwayMap(DIR$, save$)
        Dim data_pathways As New List(Of mysql.data_pathway)
        Dim references As New Dictionary(Of Long, mysql.data_references)
        Dim pathwayReferences As New List(Of mysql.xref_pathway_references)
        Dim pathwayModules As New List(Of mysql.xref_pathway_modules)
        Dim pathwayKOgenes As New List(Of mysql.xref_pathway_genes)
        Dim pathwayClass As New List(Of mysql.class_ko00001_pathway)
        Dim orthologyClass As New List(Of mysql.class_ko00001_orthology)
        Dim br As Dictionary(Of String, BriteHText) = htext.ko00001 _
            .Hierarchical _
            .EnumerateEntries _
            .Select(Function(x) x.parent) _
            .Where(Function(x)
                       Return Not x Is Nothing AndAlso
                            x.CategoryLevel = "C"c AndAlso
                            Not x.classLabel.StringEmpty
                   End Function) _
            .GroupBy(Function(pathway) pathway.classLabel.Split.First) _
            .ToDictionary(Function(x) x.Key,
                          Function(g)
                              Return g.First
                          End Function)

        For Each xml As String In ls - l - r - "*.XML" <= DIR
            Dim map As PathwayMap = xml.LoadXml(Of PathwayMap)
            Dim image$ = (xml.TrimSuffix & ".png").Open.GZipAsBase64

            data_pathways += New mysql.data_pathway With {
                .map = image,
                .KO = "ko" & map.briteID,
                .name = MySqlEscaping(map.name),
                .uid = Val(map.briteID),
                .description = MySqlEscaping(map.description)
            }

            Dim pathway& = data_pathways.Last.uid
            Dim KO = data_pathways.Last.KO

            For Each m In map.modules.SafeQuery
                pathwayModules += New mysql.xref_pathway_modules With {
                    .pathway = pathway,
                    .module = Val(m.name.Trim("M"c)),
                    .name = MySqlEscaping(m.text),
                    .KO = KO
                }
            Next

            Dim h As BriteHText = br.TryGetValue(map.briteID)

            For Each O In map.KEGGOrthology.Terms.SafeQuery
                orthologyClass += New mysql.class_ko00001_orthology With {
                    .function = MySqlEscaping(O.value),
                    .KEGG = O.name,
                    .Orthology = O.name.Trim("K"c),
                    .name = MySqlEscaping(.function.Split(";"c).First),
                    .level_C = h?.ClassLabel,
                    .level_B = h?.Parent?.ClassLabel,
                    .level_A = h?.Parent?.Parent?.ClassLabel
                }
            Next
        Next

        Call data_pathways.DumpTransaction(save)
        Call references.Values.DumpTransaction(save)
        Call pathwayReferences.DumpTransaction(save)
        Call orthologyClass.DumpTransaction(save)
        Call pathwayModules.DumpTransaction(save)

        Call pathwayModules.DumpToTable(save)
        Call orthologyClass.DumpToTable(save)
        Call data_pathways.DumpToTable(save)
        Call references.Values.DumpToTable(save)
        Call pathwayReferences.DumpToTable(save)
    End Sub

    Public Sub DumpKO(DIR$, save$)
        Dim data_orthology As New List(Of mysql.data_orthology)
        Dim KOgenes As New List(Of mysql.class_orthology_genes)

        For Each xml As String In ls - l - r - "*.XML" <= DIR
            Dim orthology As Orthology = xml.LoadXml(Of Orthology)

            data_orthology += New mysql.data_orthology With {
                .uid = orthology.Entry.Trim("K"c),
                .KEGG = orthology.Entry,
                .name = MySqlEscaping(orthology.Name),
                .definition = MySqlEscaping(orthology.Definition)
            }

            Dim KO = data_orthology.Last.uid

            For Each gene In orthology.Genes
                KOgenes += New mysql.class_orthology_genes With {
                    .orthology = KO,
                    .locus_tag = gene.locusID,
                    .organism = gene.speciesID,
                    .geneName = gene.description,
                    .uid = LocusTagGuid(.organism, .locus_tag)
                }
            Next
        Next

        Call data_orthology.DumpTransaction(save)
        Call KOgenes.DumpTransaction(save)

        Call data_orthology.DumpToTable(save)
        Call KOgenes.DumpToTable(save)
    End Sub

    Public Sub DumpReactions(DIR$, save$)

        Dim data_reactions As New List(Of mysql.data_reactions)
        Dim rnOrthology As New List(Of mysql.xref_ko_reactions)

        For Each xml As String In ls - l - r - "*.XML" <= DIR
            Dim rn As Reaction = xml.LoadXml(Of Reaction)

            data_reactions += New mysql.data_reactions With {
                .uid = Mid(rn.ID, 2),
                .definition = rn.Definition,
                .comment = rn.Comments,
                .name = rn.CommonNames.JoinBy("; "),
                .KEGG = rn.ID
            }

            Dim rnUid = data_reactions.Last.uid

            For Each KO In rn.Orthology.Terms.SafeQuery
                rnOrthology += New mysql.xref_ko_reactions With {
                    .KO = KO.name,
                    .KO_uid = .KO.Trim("K"c),
                    .name = KO.Comment,
                    .rn = rnUid
                }
            Next
        Next

        Call data_reactions.DumpToTable(save)
        Call rnOrthology.DumpToTable(save)

        Call data_reactions.DumpTransaction(save)
        Call rnOrthology.DumpTransaction(save)
    End Sub

    Public Sub DumpCompounds(DIR$, save$)

    End Sub
End Module
