#Region "Microsoft.VisualBasic::f4ffea182863f381c6db4606348e2163, R#\metagenomics_kit\microbiomeKit.vb"

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

'   Total Lines: 234
'    Code Lines: 173
' Comment Lines: 32
'   Blank Lines: 29
'     File Size: 10.21 KB


' Module microbiomeKit
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: asTaxonomyVector, castTable, CompoundOrigin, createEmptyCompoundOriginProfile, indexMatrix
'               parsegreenGenesTaxonomy, predict_metagenomes, readPICRUStMatrix, similar
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.PICRUSt
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Model.Network.Microbiome
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object

''' <summary>
''' tools for metagenomics and microbiome
''' </summary>
<Package("microbiome")>
Module microbiomeKit

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(OTUData(Of Double)()), AddressOf castTable)
    End Sub

    Private Function castTable(data As OTUData(Of Double)(), args As list, env As Environment) As dataframe
        Dim id As String() = data.Select(Function(otu) otu.OTU).ToArray
        Dim taxonomy As String() = data.Select(Function(otu) otu.taxonomy).ToArray
        Dim allNames As String() = data _
            .Select(Function(otu) otu.data.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim table As New dataframe With {
            .rownames = id,
            .columns = New Dictionary(Of String, Array) From {
                {"taxonomy", taxonomy}
            }
        }

        For Each name As String In allNames
            table.columns(name) = data _
                .Select(Function(otu) otu.data(name)) _
                .ToArray
        Next

        Return table
    End Function

    <ExportAPI("parse.otu_taxonomy")>
    Public Function parsegreenGenesTaxonomy(file As Stream) As otu_taxonomy()
        Return otu_taxonomy.Load(file).ToArray
    End Function

    <ExportAPI("save.PICRUSt_matrix")>
    Public Function indexMatrix(ggtax As otu_taxonomy(), ko_13_5_precalculated As Stream, save As Stream) As Boolean
        Using file As MetaBinaryWriter = MetaBinaryWriter.CreateWriter(ggtax, save)
            Call file.ImportsComputes(ko_13_5_precalculated)

            Return True
        End Using
    End Function

    <ExportAPI("read.PICRUSt_matrix")>
    Public Function readPICRUStMatrix(file As Stream) As MetaBinaryReader
        Return New MetaBinaryReader(file)
    End Function

    ''' <summary>
    ''' creates the final metagenome functional predictions. It 
    ''' multiplies each normalized OTU abundance by each predicted 
    ''' functional trait abundance to produce a table of functions 
    ''' (rows) by samples (columns).
    ''' </summary>
    ''' <param name="table"></param>
    ''' <returns></returns>
    <ExportAPI("predict_metagenomes")>
    Public Function predict_metagenomes(PICRUSt As MetaBinaryReader,
                                        table As dataframe,
                                        Optional env As Environment = Nothing) As OTUData(Of Double)()

        Dim sampleNames As String() = table.colnames
        Dim OTUtable As OTUData(Of Double)() = table.forEachRow _
            .Select(Function(OTU, i)
                        Dim samples As New Dictionary(Of String, Double)

                        For idx As Integer = 0 To sampleNames.Length - 1
                            samples.Add(sampleNames(idx), CDbl(OTU.Item(idx)))
                        Next

                        Return New OTUData(Of Double) With {
                            .OTU = i + 1,
                            .taxonomy = OTU.name,
                            .data = samples
                        }
                    End Function) _
            .ToArray
        Dim println As Action(Of String, Boolean) = Sub(line, newLine) Call base.cat(line & If(newLine, "\n", ""),,, env)
        Dim result = OTUtable.PredictMetagenome(
            precalculated:=PICRUSt,
            println:=println
        )

        Return result
    End Function

    ''' <summary>
    ''' evaluate the similarity of two taxonomy data vector
    ''' </summary>
    ''' <param name="v1">
    ''' the names of the list should be the BIOM taxonomy string, 
    ''' content value of the list is the relative abundance data.
    ''' </param>
    ''' <param name="v2">
    ''' the names of the list should be the BIOM taxonomy string, 
    ''' content value of the list is the relative abundance data.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("diff.entropy")>
    Public Function similar(v1 As list, v2 As list, Optional rank As TaxonomyRanks = TaxonomyRanks.Genus, Optional env As Environment = Nothing) As Double
        Dim x1 As Dictionary(Of String, Double) = v1.asTaxonomyVector(rank, env)
        Dim x2 As Dictionary(Of String, Double) = v2.asTaxonomyVector(rank, env)
        Dim S As Double = x1.DiffEntropy(x2)

        Return S
    End Function

    <Extension>
    Private Function asTaxonomyVector(v As list, rank As TaxonomyRanks, env As Environment) As Dictionary(Of String, Double)
        Return v.getNames _
            .Select(Function(tax)
                        Return (BIOMTaxonomyParser.Parse(tax).BIOMTaxonomyString(rank), v.getValue(Of Double)(tax, env))
                    End Function) _
            .GroupBy(Function(d) d.Item1) _
            .ToDictionary(Function(d) d.Key,
                          Function(d)
                              Return Aggregate xi In d Into Sum(xi.Item2)
                          End Function)
    End Function

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
