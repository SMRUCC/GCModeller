#Region "Microsoft.VisualBasic::cecfa8fef07a6ff47476dc632a28b5c7, R#\metagenomics_kit\OTUTableTools.vb"

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

'   Total Lines: 80
'    Code Lines: 41 (51.25%)
' Comment Lines: 33 (41.25%)
'    - Xml Docs: 87.88%
' 
'   Blank Lines: 6 (7.50%)
'     File Size: 3.09 KB


' Module OTUTableTools
' 
'     Function: filter, relativeAbundance
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Internal.[Object].Converts
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports Taxonomy = SMRUCC.genomics.Metagenomics.Taxonomy
Imports Vector = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' Tools for handling OTU table data
''' </summary>
''' <remarks>
''' ### Operational taxonomic unit (OTU)
''' 
''' OTU's are used to categorize bacteria based on sequence similarity.
''' 
''' In 16S metagenomics approaches, OTUs are cluster of similar sequence variants of the 
''' 16S rDNA marker gene sequence. Each of these cluster is intended to represent a 
''' taxonomic unit of a bacteria species or genus depending on the sequence similarity 
''' threshold. Typically, OTU cluster are defined by a 97% identity threshold of the 16S 
''' gene sequences to distinguish bacteria at the genus level.
'''
''' Species separation requires a higher threshold Of 98% Or 99% sequence identity, Or 
''' even better the use Of exact amplicon sequence variants (ASV) instead Of OTU sequence 
''' clusters.
''' </remarks>
<Package("OTU_table")>
<RTypeExport("OTU_table", GetType(OTUTable))>
Module OTUTableTools

    Sub Main()
        Call makeDataframe.addHandler(GetType(OTUTable()), AddressOf castTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Friend Function castTable(otutable As OTUTable(), args As list, env As Environment) As rdataframe
        Dim df As New rdataframe With {
            .rownames = otutable.Keys,
            .columns = New Dictionary(Of String, Array)
        }
        Dim sample_ids As String() = otutable.PropertyNames

        Call df.add("taxonomy", From otu As OTUTable
                                In otutable
                                Select otu.taxonomy.BIOMTaxonomyString)

        For Each name As String In sample_ids
            Call df.add(name, From otu As OTUTable In otutable Select otu(name))
        Next

        Return df
    End Function

    ''' <summary>
    ''' Transform abundance data in an otu_table to relative abundance, sample-by-sample. 
    ''' 
    ''' Transform abundance data into relative abundance, i.e. proportional data. This is 
    ''' an alternative method of normalization and may not be appropriate for all datasets,
    ''' particularly if your sequencing depth varies between samples.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("relative_abundance")>
    <RApiReturn(GetType(OTUTable))>
    Public Function relativeAbundance(x As OTUTable()) As Object
        Dim sample_ids As String() = x.PropertyNames
        Dim v As Vector

        For Each name As String In sample_ids
            v = x.Select(Function(otu) otu(name)).AsVector
            v = v / v.Sum

            For i As Integer = 0 To x.Length - 1
                x(i)(name) = v(i)
            Next
        Next

        Return x
    End Function

    <ExportAPI("median_scale")>
    Public Function median_scale(x As OTUTable()) As Object
        Dim sample_ids As String() = x.PropertyNames

        For Each otu As OTUTable In x.SafeQuery
            Dim pos As Double() = otu.Vector.Where(Function(a) a > 0).ToArray

            If pos.Length > 0 Then
                ' not all zero
                Dim median As Double = otu.Vector.Median

                For Each id As String In sample_ids
                    otu(id) = otu(id) / median
                Next
            End If
        Next

        Return x
    End Function

    <ExportAPI("impute_missing")>
    Public Function impute_missing(x As OTUTable()) As Object
        Dim sample_ids As String() = x.PropertyNames

        For Each otu As OTUTable In x.SafeQuery
            Dim pos As Double() = otu.Vector.Where(Function(a) a > 0).ToArray

            If pos.Length > 0 Then
                ' not all zero
                Dim min_pos As Double = pos.Min / 2

                For Each id As String In sample_ids
                    If otu(id) <= 0 Then
                        otu(id) = min_pos
                    End If
                Next
            End If
        Next

        Return x
    End Function

    ''' <summary>
    ''' filter the otu data which has relative abundance greater than the given threshold
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="relative_abundance"></param>
    ''' <returns></returns>
    <ExportAPI("filter")>
    <RApiReturn(GetType(OTUTable))>
    Public Function filter(x As OTUTable(), relative_abundance As Double) As Object
        Return x _
            .Where(Function(otu)
                       Return otu.Properties _
                          .Values _
                          .Any(Function(xi)
                                   Return xi > relative_abundance
                               End Function)
                   End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' read 16s OTU table
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="sum_duplicated">sum all OTU data if theirs taxonomy information is the same</param>
    ''' <returns></returns>
    <ExportAPI("read.OTUtable")>
    Public Function readOTUTable(file As String,
                                 Optional sum_duplicated As Boolean = False,
                                 Optional OTUTaxonAnalysis As Boolean = False) As OTUTable()
        Dim otus As OTUTable()

        If OTUTaxonAnalysis Then
            otus = OTU _
                .LoadOTUTaxonAnalysis(file, tsv:=Not file.ExtensionSuffix("csv")) _
                .ToArray
        Else
            otus = DataSet.LoadDataSet(Of OTUTable)(file, mute:=True).ToArray
        End If

        If sum_duplicated Then
            Return OTUTable.SumDuplicatedOTU(otus).ToArray
        Else
            Return otus
        End If
    End Function

    <ExportAPI("read.OTUdata")>
    Public Function readOTuData(file As String) As OTUData(Of Double)()
        Return file.LoadCsv(Of OTUData(Of Double))(mute:=True).ToArray
    End Function

    ''' <summary>
    ''' combine of two batch data
    ''' </summary>
    ''' <param name="batch1"></param>
    ''' <param name="batch2"></param>
    ''' <returns></returns>
    <ExportAPI("batch_combine")>
    Public Function batch_combine(batch1 As OTUTable(), batch2 As OTUTable()) As OTUTable()
        Return batch1.BatchCombine(batch2).ToArray
    End Function

    ''' <summary>
    ''' cast the expression matrix to the otu data
    ''' </summary>
    ''' <param name="x">
    ''' an expression matrix which use the biom taxonomy string as feature unique id reference.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("otu_from_matrix")>
    Public Function fromMatrix(x As Matrix) As OTUData(Of Double)()
        Return x.FromExpressionMatrix.ToArray
    End Function

    ''' <summary>
    ''' Create expression matrix data from a given otu table
    ''' </summary>
    ''' <param name="otu_table"></param>
    ''' <returns></returns>
    <ExportAPI("as.hts_matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function cast_matrix(otu_table As OTUTable()) As Object
        Return otu_table.CastMatrix
    End Function

    ''' <summary>
    ''' convert the mothur rank tree as the OTU table
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("as.OTU_table")>
    <RApiReturn(GetType(OTUTable))>
    Public Function asOTUTable(x As Object,
                               Optional id As String = "OTU_num",
                               Optional taxonomy As String = "taxonomy",
                               Optional env As Environment = Nothing) As Object

        If x Is Nothing Then
            Return Nothing
        End If

        If TypeOf x Is MothurRankTree Then
            Return DirectCast(x, MothurRankTree).GetOTUTable
        ElseIf TypeOf x Is rdataframe Then
            Return asOTUTable(DirectCast(x, rdataframe), id, taxonomy)
        Else
            Return Message.InCompatibleType(GetType(MothurRankTree), x.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' convert the dataframe object to OTU table
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="id"></param>
    ''' <param name="taxonomy"></param>
    ''' <returns></returns>
    Public Function asOTUTable(table As rdataframe,
                               Optional id As String = "OTU_num",
                               Optional taxonomy As String = "taxonomy") As OTUTable()

        Dim unique_id As String() = CLRVector.asCharacter(table.getColumnVector(id))
        Dim taxonomyStr As Taxonomy() = CLRVector.asCharacter(table.getColumnVector(taxonomy)) _
            .Select(Function(str) New Taxonomy(BIOMTaxonomy.TaxonomyParser(str))) _
            .ToArray

        Call table.columns.Remove(id)
        Call table.columns.Remove(taxonomy)

        Dim samples = table.forEachRow.ToArray
        Dim sample_ids As String() = table.colnames

        Return unique_id _
            .Select(Function(tax, i)
                        Return New OTUTable With {
                            .ID = tax,
                            .taxonomy = taxonomyStr(i),
                            .Properties = samples(i) _
                                .Select(Function(sample, j) (sample, sample_ids(j))) _
                                .ToDictionary(Function(a) a.Item2,
                                              Function(a)
                                                  Return Val(a.Item1)
                                              End Function)
                        }
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("make_otu_table")>
    <RApiReturn(GetType(OTUTable))>
    Public Function MakeOTUTable(<RRawVectorArgument> samples As Object, taxonomy_tree As NcbiTaxonomyTree,
                                 Optional filter_missing As Boolean = True,
                                 Optional env As Environment = Nothing) As Object

        Dim samplesData As New List(Of NamedCollection(Of ITaxonomyAbundance))

        If samples Is Nothing Then
            Return Nothing
        End If

        If TypeOf samples Is list Then
            Dim list As list = DirectCast(samples, list)

            For Each sample_name As String In list.getNames
                Dim data As pipeline = pipeline.TryCreatePipeline(Of ITaxonomyAbundance)(list(sample_name), env)

                If data.isError Then
                    Return data.getError
                End If

                Call samplesData.Add(New NamedCollection(Of ITaxonomyAbundance)(
                     sample_name,
                     data.populates(Of ITaxonomyAbundance)(env))
                )
            Next
        Else
            Return Message.InCompatibleType(GetType(ITaxonomyAbundance), samples.GetType, env)
        End If

        Dim otuTable = samplesData.MakeOUTTable(taxonomy_tree).ToArray

        If filter_missing Then
            otuTable = otuTable _
                .Where(Function(o) Not o.taxonomy.IsMissing) _
                .ToArray
        End If

        Return otuTable
    End Function

    ''' <summary>
    ''' make OTU tree graph via JSD correlation method
    ''' </summary>
    ''' <param name="otus"></param>
    ''' <param name="equals"></param>
    ''' <param name="gt"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("makeTreeGraph")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function makeTreeGraph(<RRawVectorArgument> otus As Object,
                                  Optional equals As Double = 0.85,
                                  Optional gt As Double = 0.6,
                                  Optional rank_colors As TaxonomyRanks = TaxonomyRanks.Phylum,
                                  Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of OTUTable)(otus, env)
        Dim OtuHashset As New List(Of OTUTable)

        If pull.isError Then
            Return pull.getError
        Else
            For Each otu As OTUTable In pull.populates(Of OTUTable)(env)
                otu = New OTUTable(otu)
                otu.ID = otu.ID.MD5
                OtuHashset.Add(otu)
            Next
        End If

        Dim graph As NetworkGraph = OtuHashset.BuildClusterTree(equals, gt)
        Dim nodes As Node() = graph.vertex.ToArray
        Dim labels As String() = nodes.Select(Function(v) v.data(rank_colors.ToString.ToLower)).ToArray
        Dim colorset As New CategoryColorProfile(labels.Distinct, colorSet:="paper")

        For i As Integer = 0 To nodes.Length - 1
            nodes(i).data.color = New SolidBrush(colorset.GetColor(labels(i)))
        Next

        Return graph
    End Function

    <ExportAPI("makeUPGMATree")>
    <RApiReturn(GetType(UPGMATree.Taxa))>
    Public Function makeUPGMATree(<RRawVectorArgument> otus As Object,
                                  Optional as_graph As Boolean = False,
                                  Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of OTUTable)(otus, env)
        Dim OtuHashset As New List(Of OTUTable)

        If pull.isError Then
            Return pull.getError
        Else
            For Each otu As OTUTable In pull.populates(Of OTUTable)(env)
                otu = New OTUTable(otu)
                otu.ID = otu.ID.MD5
                OtuHashset.Add(otu)
            Next
        End If

        Dim tree As Taxa = UPGMATree.BuildTree(OtuHashset)

        If as_graph Then
            Return UPGMATree.TaxaTreeGraph(tree)
        Else
            Return tree
        End If
    End Function
End Module
