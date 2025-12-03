#Region "Microsoft.VisualBasic::f84587b85bfeb1fd6aaa620fdfd5b14e, R#\metagenomics_kit\microbiomeKit.vb"

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

    '   Total Lines: 317
    '    Code Lines: 219 (69.09%)
    ' Comment Lines: 59 (18.61%)
    '    - Xml Docs: 94.92%
    ' 
    '   Blank Lines: 39 (12.30%)
    '     File Size: 13.39 KB


    ' Module microbiomeKit
    ' 
    '     Function: asTaxonomyVector, castTable, CompoundOrigin, createEmptyCompoundOriginProfile, indexMatrix
    '               (+2 Overloads) OTUtable, parsegreenGenesTaxonomy, predict_metagenomes, readPICRUSt, readPICRUStMatrix
    '               similar
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.PICRUSt
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Model.Network.Microbiome
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' tools for metagenomics and microbiome
''' </summary>
<Package("microbiome")>
<RTypeExport("PICRUSt", GetType(MetaBinaryReader))>
Module microbiomeKit

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(OTUData(Of Double)()), AddressOf castTable)

        Call RInternal.generic.add("readBin.PICRUSt", GetType(Stream), AddressOf readPICRUSt)
    End Sub

    Private Function readPICRUSt(file As Stream, args As list, env As Environment) As Object
        Return New MetaBinaryReader(file)
    End Function

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

    ''' <summary>
    ''' parse the otu taxonomy data file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("parse.otu_taxonomy")>
    <RApiReturn(GetType(otu_taxonomy))>
    Public Function parsegreenGenesTaxonomy(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Return otu_taxonomy.Load(buf.TryCast(Of Stream)).ToArray
    End Function

    ''' <summary>
    ''' build PICRUSt binary database file
    ''' </summary>
    ''' <param name="ggtax">A helper table gg_13_8_99.gg.tax for make OTU id mapping to taxonomy information</param>
    ''' <param name="ko_13_5_precalculated">file connection to the ``ko_13_5_precalculated.tab``</param>
    ''' <param name="save">the file connection for save the compiled PICRUSt binary database file</param>
    ''' <param name="copyNumbers_16s">
    ''' a list of the 16s RNA copy number, [#OTU_IDs => 16S_rRNA_Count]
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' write the data matrix via <see cref="MetaBinaryWriter"/>
    ''' </remarks>
    <ExportAPI("build.PICRUSt_db")>
    Public Function indexMatrix(ggtax As otu_taxonomy(),
                                copyNumbers_16s As list,
                                ko_13_5_precalculated As Stream,
                                save As Stream,
                                Optional env As Environment = Nothing) As Boolean

        Dim copyNumbers As Dictionary(Of String, Double) = copyNumbers_16s.AsGeneric(Of Double)(env)

        Using file As MetaBinaryWriter = MetaBinaryWriter.CreateWriter(ggtax, copyNumbers, save)
            Call file.ImportsComputes(ko_13_5_precalculated)

            Return True
        End Using
    End Function

    ''' <summary>
    ''' read the compiled PICRUSt binary database file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read_PICRUSt")>
    Public Function readPICRUStMatrix(file As Stream) As MetaBinaryReader
        Return New MetaBinaryReader(file)
    End Function

    ''' <summary>
    ''' creates the final metagenome functional predictions. It 
    ''' multiplies each normalized OTU abundance by each predicted 
    ''' functional trait abundance to produce a table of functions 
    ''' (rows) by samples (columns).
    ''' </summary>
    ''' <param name="table">
    ''' should be a merged OTU dataframe object, that should be in format like:
    ''' 
    ''' 1. the colnames should be the sample name, and the column field value is the relative abundance value of each otu in each sample
    ''' 2. the rows in this dataframe should be the otu expression value across samples
    ''' 
    ''' the GCModeller internal <see cref="HTSMatrix"/> is also avaiable 
    ''' for this parameter.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("predict_metagenomes")>
    <RApiReturn(GetType(OTUData(Of Double)))>
    Public Function predict_metagenomes(PICRUSt As MetaBinaryReader, table As Object, Optional env As Environment = Nothing) As Object
        Dim OTUtable As OTUData(Of Double)()
        Dim println As Action(Of String, Boolean) = Sub(line, newLine) Call base.cat(line & If(newLine, "\n", ""),,, env:=env)

        If TypeOf table Is dataframe Then
            OTUtable = DirectCast(table, dataframe).OTUtable.ToArray
        ElseIf TypeOf table Is HTSMatrix Then
            OTUtable = DirectCast(table, HTSMatrix).OTUtable.ToArray
        Else
            Return Message.InCompatibleType(GetType(HTSMatrix), table.GetType, env)
        End If

        Dim predictor As New PredictMetagenome(PICRUSt, println)
        Dim result = predictor.PredictMetagenome(OTUtable).ToArray

        Return result
    End Function

    <Extension>
    Private Iterator Function OTUtable(matrix As HTSMatrix) As IEnumerable(Of OTUData(Of Double))
        Dim i As i32 = 1

        For Each OTU As DataFrameRow In matrix.expression
            Yield New OTUData(Of Double) With {
                .OTU = ++i,
                .data = OTU.ToDataSet(matrix.sampleID),
                .taxonomy = OTU.geneID
            }
        Next
    End Function

    <Extension>
    Private Iterator Function OTUtable(table As dataframe) As IEnumerable(Of OTUData(Of Double))
        Dim i As i32 = 1
        Dim sampleNames As String() = table.colnames

        For Each OTU As NamedCollection(Of Object) In table.forEachRow
            Dim v As Double() = CLRVector.asNumeric(OTU.value)
            Dim samples As New Dictionary(Of String, Double)

            For idx As Integer = 0 To sampleNames.Length - 1
                Call samples.Add(sampleNames(idx), v(idx))
            Next

            Yield New OTUData(Of Double) With {
                .OTU = ++i,
                .taxonomy = OTU.name,
                .data = samples
            }
        Next
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
    ''' <remarks>
    ''' compares on a specific <see cref="TaxonomyRanks"/>
    ''' </remarks>
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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="otus">
    ''' the otu table data
    ''' </param>
    ''' <returns>a tuple list of the <see cref="RankLevelView"/> in different taxonomy
    ''' rank levels.
    ''' </returns>
    <ExportAPI("taxonomy.rank_table")>
    <RApiReturn(GetType(RankLevelView), GetType(HTSMatrix))>
    Public Function taxonomyRankTable(<RRawVectorArgument> otus As Object, Optional as_matrix As Boolean = False, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of OTUData(Of Double))(otus, env)
        Dim all_ranks As NamedCollection(Of RankLevelView)()
        Dim ranks As list = list.empty

        If Not pull.isError Then
            all_ranks = pull _
                .populates(Of OTUData(Of Double))(env) _
                .ExportByRanks _
                .ToArray
        Else
            pull = pipeline.TryCreatePipeline(Of OTUTable)(otus, env)

            If pull.isError Then
                Return pull.getError
            End If

            all_ranks = pull.populates(Of OTUTable)(env) _
                .ExportByRanks _
                .ToArray
        End If

        For Each rank As NamedCollection(Of RankLevelView) In all_ranks
            Call ranks.add(
                name:=rank.name,
                value:=If(as_matrix,
                    RankLevelView.ToMatrix(rank.value, rank.name),
                    rank.value)
            )
        Next

        Return ranks
    End Function
End Module
