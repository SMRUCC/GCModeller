#Region "Microsoft.VisualBasic::d947c024ad149c0f633d678bba887978, R#\metagenomics_kit\metagenomicsKit.vb"

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

    ' Module metagenomicsKit
    ' 
    '     Function: CompoundOrigin, createEmptyCompoundOriginProfile
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Model.Network.Microbiome
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("metagenomics_kit")>
Module metagenomicsKit

    <ExportAPI("compounds.origin.profile")>
    Public Function createEmptyCompoundOriginProfile(taxonomy As NcbiTaxonomyTree, organism As String) As CompoundOrigins
        Return CompoundOrigins.CreateEmptyCompoundsProfile(taxonomy, organism)
    End Function

    ''' <summary>
    ''' create compound origin profile dataset
    ''' </summary>
    ''' <param name="annotations">a list of multiple organism protein functional annotation dataset.</param>
    ''' <param name="tree">the ncbi taxonomy tree</param>
    ''' <param name="rank">minimal rank for takes the most abondance taxonomy from the raw dataset.</param>
    ''' <param name="ranges"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("compounds.origin")>
    Public Function CompoundOrigin(annotations As list, tree As NcbiTaxonomyTree,
                                   Optional rank As TaxonomyRanks = TaxonomyRanks.Family,
                                   Optional ranges As String() = Nothing,
                                   Optional env As Environment = Nothing) As list

        Dim compounds As New Dictionary(Of String, List(Of String))

        For Each organism As KeyValuePair(Of String, Pathway()) In annotations.AsGeneric(Of Pathway())(env)
            For Each map As Pathway In organism.Value.SafeQuery
                For Each compound As NamedValue In map.compound.SafeQuery
                    If Not compounds.ContainsKey(compound.name) Then
                        Call compounds.Add(compound.name, New List(Of String))
                    End If

                    Call compounds(compound.name).Add(organism.Key)
                Next
            Next
        Next

        Dim origins As New Dictionary(Of String, Object)
        Dim ncbi_taxid As String()
        Dim taxonomyList As gast.Taxonomy()
        Dim consensusTree As TaxonomyTree
        Dim consensus As TaxonomyTree()
        Dim searchRanges As Metagenomics.Taxonomy() = ranges _
            .SafeQuery _
            .Select(Function(id)
                        If id.IsPattern("\d+") Then
                            Return New Metagenomics.Taxonomy(tree.GetAscendantsWithRanksAndNames(Integer.Parse(id), only_std_ranks:=True))
                        Else
                            Return New Metagenomics.Taxonomy(BIOMTaxonomy.TaxonomyParser(id))
                        End If
                    End Function) _
            .Select(Function(nodes) New Metagenomics.Taxonomy(nodes)) _
            .ToArray
        Dim Homo_sapiens As Boolean
        Dim Mus_musculus As Boolean
        Dim Rattus_norvegicus As Boolean

        For Each compound In compounds
            ncbi_taxid = compound.Value.Distinct.ToArray
            taxonomyList = ncbi_taxid _
                .Select(Function(id)
                            Return New gast.Taxonomy(New Metagenomics.Taxonomy(tree.GetAscendantsWithRanksAndNames(Integer.Parse(id), True))) With {
                                .ncbi_taxid = id
                            }
                        End Function) _
                .ToArray

            Homo_sapiens = taxonomyList.Any(Function(t) t.species = "Homo sapiens")
            Mus_musculus = taxonomyList.Any(Function(t) t.species = "Mus musculus")
            Rattus_norvegicus = taxonomyList.Any(Function(t) t.species = "Rattus norvegicus")

            If searchRanges.Length > 0 Then
                taxonomyList = searchRanges.RangeFilter(taxonomyList).ToArray
            End If

            consensusTree = TaxonomyTree.BuildTree(taxonomyList, Nothing, Nothing)
            consensus = consensusTree _
                .PopulateTaxonomy(rank) _
                .OrderByDescending(Function(node) node.hits) _
                .Take(10) _
                .ToArray

            taxonomyList = consensus _
                .Select(Function(tax) tax.PopulateTaxonomy(TaxonomyRanks.Species).First) _
                .ToArray

            origins(compound.Key) = New Dictionary(Of String, Object) From {
                {"kegg_id", compound.Key},
                {"ncbi_taxid", ncbi_taxid},
                {"taxonomy", taxonomyList.Select(Function(tax) New Metagenomics.Taxonomy(tax) With {.ncbi_taxid = tax.ncbi_taxid}).ToArray},
                {NameOf(Homo_sapiens), Homo_sapiens},
                {NameOf(Mus_musculus), Mus_musculus},
                {NameOf(Rattus_norvegicus), Rattus_norvegicus}
            }
        Next

        Return New list With {.slots = origins}
    End Function
End Module
