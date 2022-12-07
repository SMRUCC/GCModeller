#Region "Microsoft.VisualBasic::915fa060cab3efeba6f452e4f91f3099, R#\phenotype_kit\geneExpression.vb"

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

    '   Total Lines: 958
    '    Code Lines: 632
    ' Comment Lines: 232
    '   Blank Lines: 94
    '     File Size: 37.47 KB


    ' Module geneExpression
    ' 
    '     Function: applyPCA, average, castGenericRows, cmeans, CMeans3D
    '               CmeansPattern, createDEGModels, createVectorList, DEGclass, depDataTable
    '               dims, expDataTable, filter, filterNaN, filterZeroSamples
    '               geneId, GetCmeansPattern, joinSamples, loadExpression, loadFromDataFrame
    '               loadFromGenericDataSet, mergeMultiple, readBinaryMatrix, readPattern, relative
    '               savePattern, setGeneIDs, setTag, setZero, splitCMeansClusters
    '               totalSumNorm, tr, Ttest, uniqueGeneId, writeMatrix
    '               zscore
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Prcomp
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ExpressionPattern
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports Vec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' the gene expression matrix data toolkit
''' </summary>
<Package("geneExpression")>
Module geneExpression

    Friend Sub Main()
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of ExpressionPattern)(Function(a) DirectCast(a, ExpressionPattern).ToSummaryText)
        REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(DEP_iTraq()), AddressOf depDataTable)
        REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(Matrix), AddressOf expDataTable)
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of DEGModel)(Function(a) a.ToString)
    End Sub

    Private Function expDataTable(exp As Matrix, args As list, env As Environment) As Rdataframe
        Dim table As New Rdataframe With {.columns = New Dictionary(Of String, Array)}

        For i As Integer = 0 To exp.sampleID.Length - 1
#Disable Warning
            table.columns(exp.sampleID(i)) = exp.expression _
                .Select(Function(gene)
                            Return gene.experiments(i)
                        End Function) _
                .ToArray
#Enable Warning
        Next

        table.rownames = exp.expression _
            .Select(Function(gene) gene.geneID) _
            .ToArray

        Return table
    End Function

    Private Function depDataTable(dep As DEP_iTraq(), args As list, env As Environment) As Rdataframe
        Dim table As New Rdataframe With {
            .rownames = dep.Keys,
            .columns = New Dictionary(Of String, Array)
        }

        table.columns("FC.avg") = dep.Select(Function(p) p.FCavg).ToArray
        table.columns(NameOf(DEP_iTraq.log2FC)) = dep.Select(Function(p) p.log2FC).ToArray
        table.columns("p.value") = dep.Select(Function(p) p.pvalue).ToArray
        table.columns(NameOf(DEP_iTraq.FDR)) = dep.Select(Function(p) p.FDR).ToArray
        table.columns("is.DEP") = dep.Select(Function(p) p.isDEP).ToArray

        For Each sampleName As String In dep.PropertyNames
            table.columns(sampleName) = dep _
                .Select(Function(p) p(sampleName)) _
                .ToArray
        Next

        Return table
    End Function

    ''' <summary>
    ''' do matrix transpose
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <returns></returns>
    <ExportAPI("tr")>
    Public Function tr(mat As Matrix) As Matrix
        Return mat.T
    End Function

    ''' <summary>
    ''' get summary information about the HTS matrix dimensions
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <returns></returns>
    <ExportAPI("dims")>
    <RApiReturn("feature_size", "feature_names", "sample_size", "sample_names")>
    Public Function dims(mat As Matrix) As list
        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"feature_size", mat.expression.Length},
                {"feature_names", mat.rownames},
                {"sample_size", mat.sampleID.Length},
                {"sample_names", mat.sampleID}
            }
        }
    End Function

    ''' <summary>
    ''' convert the matrix into row gene list
    ''' </summary>
    ''' <param name="expr0"></param>
    ''' <returns></returns>
    <ExportAPI("as.expr_list")>
    Public Function createVectorList(expr0 As Matrix) As list
        Return New list With {
            .slots = expr0.expression _
                .ToDictionary(Function(a) a.geneID,
                              Function(a)
                                  Return CObj(a.experiments)
                              End Function)
        }
    End Function

    ''' <summary>
    ''' set a new tag string to the matrix
    ''' </summary>
    ''' <param name="expr0"></param>
    ''' <param name="tag"></param>
    ''' <returns></returns>
    <ExportAPI("setTag")>
    Public Function setTag(expr0 As Matrix, tag As String) As Matrix
        expr0.tag = tag
        Return expr0
    End Function

    ''' <summary>
    ''' set the expression value to zero 
    ''' 
    ''' if the expression value is less than a given threshold
    ''' </summary>
    ''' <param name="expr0"></param>
    ''' <param name="q"></param>
    ''' <returns></returns>
    <ExportAPI("setZero")>
    Public Function setZero(expr0 As Matrix, Optional q As Double = 0.1) As Matrix
        For Each gene As DataFrameRow In expr0.expression
            Dim qk = gene.experiments.GKQuantile
            Dim qcut As Double = qk.Query(q)

            For i As Integer = 0 To gene.experiments.Length - 1
                If gene.experiments(i) <= qcut Then
                    gene.experiments(i) = 0
                End If
            Next
        Next

        Return expr0
    End Function

    ''' <summary>
    ''' set new gene id list to the matrix rows
    ''' </summary>
    ''' <param name="expr0"></param>
    ''' <param name="gene_ids"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("setFeatures")>
    <RApiReturn(GetType(Matrix))>
    Public Function setGeneIDs(expr0 As Matrix,
                               gene_ids As String(),
                               Optional env As Environment = Nothing) As Object

        If expr0.expression.Length <> gene_ids.Length Then
            Return Internal.debug.stop({$"dimension({expr0.expression.Length} genes) of the matrix feature must be equals to the dimension({gene_ids.Length} names) of the name vector!"}, env)
        End If

        For i As Integer = 0 To gene_ids.Length - 1
            expr0.expression(i).geneID = gene_ids(i)
        Next

        Return expr0
    End Function

    ''' <summary>
    ''' filter out all samples columns which its expression vector is ZERO!
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("filterZeroSamples")>
    Public Function filterZeroSamples(mat As Matrix, Optional env As Environment = Nothing) As Object
        Return mat.T.TrimZeros.T
    End Function

    ''' <summary>
    ''' set the NaN missing value to default value
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="missingDefault"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("filterNaNMissing")>
    Public Function filterNaN(x As Matrix, Optional missingDefault As Double = 0, Optional env As Environment = Nothing) As Object
        For Each gene As DataFrameRow In x.expression
            For i As Integer = 0 To gene.experiments.Length - 1
                If gene.experiments(i).IsNaNImaginary Then
                    gene.experiments(i) = missingDefault
                End If
            Next
        Next

        Return x
    End Function

    ''' <summary>
    ''' load an expressin matrix data
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="exclude_samples"></param>
    ''' <returns></returns>
    <ExportAPI("load.expr")>
    <RApiReturn(GetType(Matrix))>
    Public Function loadExpression(file As Object,
                                   Optional exclude_samples As String() = Nothing,
                                   Optional rm_ZERO As Boolean = False,
                                   Optional makeNames As Boolean = False,
                                   Optional env As Environment = Nothing) As Object

        Dim ignores As Index(Of String) = If(exclude_samples, {})

        If TypeOf file Is String Then
            Return Matrix.LoadData(DirectCast(file, String), ignores, rm_ZERO).uniqueGeneId(makeNames)
        ElseIf TypeOf file Is Rdataframe Then
            Return DirectCast(file, Rdataframe) _
                .loadFromDataFrame(rm_ZERO, ignores) _
                .uniqueGeneId(makeNames)
        ElseIf REnv.isVector(Of DataSet)(file) Then
            Return DirectCast(REnv.asVector(Of DataSet)(file), DataSet()) _
                .loadFromGenericDataSet(rm_ZERO, ignores) _
                .uniqueGeneId(makeNames)
        Else
            Return Message.InCompatibleType(GetType(Rdataframe), file.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' read the binary matrix data file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("load.expr0")>
    <RApiReturn(GetType(Matrix))>
    Public Function readBinaryMatrix(file As Object, Optional env As Environment = Nothing) As Object
        Dim stream = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If stream Like GetType(Message) Then
            Return stream.TryCast(Of Message)
        Else
            Return BinaryMatrix.LoadStream(stream.TryCast(Of Stream))
        End If
    End Function

    ''' <summary>
    ''' write the gene expression data matrix file
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="file"></param>
    ''' <param name="id"></param>
    ''' <param name="binary">
    ''' write matrix data in binary data format? default value 
    ''' is False means write matrix as csv matrix file.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("write.expr_matrix")>
    Public Function writeMatrix(expr As Matrix, file As String,
                                Optional id As String = "geneID",
                                Optional binary As Boolean = False) As Boolean
        If binary Then
            Using buffer As Stream = file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Return expr.Save(file:=buffer)
            End Using
        Else
            Return expr.SaveMatrix(file, id)
        End If
    End Function

    ''' <summary>
    ''' Filter the geneID rows
    ''' </summary>
    ''' <param name="HTS"></param>
    ''' <param name="geneId"></param>
    ''' <param name="exclude">matrix a subset of the data matrix excepts the 
    ''' input <paramref name="geneId"/> features or just make a subset which 
    ''' just contains the input <paramref name="geneId"/> features.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("filter")>
    Public Function filter(HTS As Matrix, geneId As String(), Optional exclude As Boolean = False) As Matrix
        Dim filterIndex As Index(Of String) = geneId
        Dim newMatrix As New Matrix With {
            .tag = HTS.tag,
            .sampleID = HTS.sampleID,
            .expression = HTS.expression _
                .Where(Function(gene)
                           If exclude Then
                               Return Not gene.geneID Like filterIndex
                           Else
                               Return gene.geneID Like filterIndex
                           End If
                       End Function) _
                .ToArray
        }

        Return newMatrix
    End Function

    <Extension>
    Private Function loadFromDataFrame(table As Rdataframe, rm_ZERO As Boolean, ignores As Index(Of String)) As Matrix
        Dim sampleNames As String() = table.columns.Keys.Where(Function(c) Not c Like ignores).ToArray
        Dim genes As DataFrameRow() = table _
            .forEachRow(colKeys:=sampleNames) _
            .Select(Function(v)
                        Return New DataFrameRow With {
                            .geneID = v.name,
                            .experiments = v.value _
                                .Select(Function(obj) CDbl(obj)) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        If rm_ZERO Then
            genes = genes _
                .Where(Function(gene)
                           Return Not gene.experiments.All(Function(x) x = 0.0)
                       End Function) _
                .ToArray
        End If

        Return New Matrix With {
            .expression = genes,
            .sampleID = sampleNames
        }
    End Function

    <Extension>
    Private Function loadFromGenericDataSet(rows As DataSet(), rm_ZERO As Boolean, ignores As Index(Of String)) As Matrix
        Dim matrix As New Matrix With {
            .sampleID = rows _
                .PropertyNames _
                .Where(Function(name) Not name Like ignores) _
                .ToArray
        }
        Dim genes As DataFrameRow() = New DataFrameRow(rows.Length - 1) {}

        For i As Integer = 0 To genes.Length - 1
#Disable Warning
            genes(i) = New DataFrameRow With {
                .geneID = rows(i).ID,
                .experiments = matrix.sampleID _
                    .Select(Function(name) rows(i)(name)) _
                    .ToArray
            }
#Enable Warning
        Next

        If rm_ZERO Then
            genes = genes _
                .Where(Function(gene)
                           Return Not gene.experiments.All(Function(x) x = 0.0)
                       End Function) _
                .ToArray
        End If

        matrix.expression = genes

        Return matrix
    End Function

    <Extension>
    Private Function uniqueGeneId(m As Matrix, makeNames As Boolean) As Matrix
        Dim geneId As String() = m.expression _
            .Select(Function(gene) gene.geneID) _
            .ToArray
        Dim unique As String() = If(makeNames, geneId.makeNames(unique:=True), geneId.uniqueNames)

        For i As Integer = 0 To unique.Length - 1
            m(i).geneID = unique(i)
        Next

        Return m
    End Function

    ''' <summary>
    ''' cast the HTS matrix object to the general dataset
    ''' </summary>
    ''' <param name="matrix">a gene expression matrix</param>
    ''' <returns></returns>
    <ExportAPI("as.generic")>
    Public Function castGenericRows(matrix As Matrix) As DataSet()
        Dim sampleNames As String() = matrix.sampleID
        Dim geneNodes As DataSet() = matrix.expression _
            .AsParallel _
            .Select(Function(gene)
                        Dim vector As New Dictionary(Of String, Double)

                        For i As Integer = 0 To sampleNames.Length - 1
                            Call vector.Add(sampleNames(i), gene.experiments(i))
                        Next

                        Return New DataSet With {
                            .ID = gene.geneID,
                            .Properties = vector
                        }
                    End Function) _
            .ToArray

        Return geneNodes
    End Function

    ''' <summary>
    ''' calculate average value of the gene expression for
    ''' each sample group.
    ''' 
    ''' this method can be apply for reduce data size when 
    ''' create some plot for visualize the gene expression
    ''' patterns across the sample groups.
    ''' </summary>
    ''' <param name="matrix">a gene expression matrix</param>
    ''' <param name="sampleinfo"></param>
    ''' <returns></returns>
    <ExportAPI("average")>
    Public Function average(matrix As Matrix, sampleinfo As SampleInfo()) As Matrix
        Return Matrix.MatrixAverage(matrix, sampleinfo)
    End Function

    ''' <summary>
    ''' To avoid the influence of expression level to the 
    ''' clustering analysis, z-score transformation can 
    ''' be applied to covert the expression values to 
    ''' z-scores by performing the following formula:
    ''' 
    ''' ```
    ''' z = (x - u) / sd
    ''' ```
    ''' 
    ''' x is value to be converted (e.g., a expression value 
    ''' of a genomic feature in one condition), µ is the 
    ''' population mean (e.g., average expression value Of 
    ''' a genomic feature In different conditions), σ Is the 
    ''' standard deviation (e.g., standard deviation of 
    ''' expression of a genomic feature in different conditions).
    ''' </summary>
    ''' <param name="x">a gene expression matrix</param>
    ''' <returns></returns>
    <ExportAPI("z_score")>
    Public Function zscore(x As Matrix) As Matrix
        Return New Matrix With {
            .sampleID = x.sampleID,
            .tag = $"z-score({x.tag})",
            .expression = x.expression _
                .Select(Function(expr)
                            ' each row is the gene expression data across experiments
                            ' do z-score transformation for each gene
                            Return New DataFrameRow With {
                                .geneID = expr.geneID,
                                .experiments = expr.experiments _
                                    .AsVector _
                                    .Z _
                                    .ToArray
                            }
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' do PCA on a gene expressin matrix
    ''' </summary>
    ''' <param name="x">a gene expression matrix</param>
    ''' <param name="npc"></param>
    ''' <returns></returns>
    <ExportAPI("pca")>
    Public Function applyPCA(x As Matrix, Optional npc As Integer = 3) As Rdataframe
        Dim mat As Double()() = x.expression _
            .Select(Function(gene) gene.experiments) _
            .ToArray
        Dim pca As New PCA(mat, center:=False)
        Dim pcaSpace As Vec() = pca.Project(npc)
        Dim embedded As New Rdataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = x.rownames
        }

        For i As Integer = 0 To npc - 1
#Disable Warning
            Dim v As Double() = pcaSpace _
                .Select(Function(r) r(i)) _
                .ToArray
            Dim name As String = $"PC{i + 1}"

            Call embedded.add(name, v)
#Enable Warning
        Next

        Return embedded
    End Function

    ''' <summary>
    ''' normalize data by sample column
    ''' </summary>
    ''' <param name="matrix">a gene expression matrix</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' apply for the metabolomics data usually
    ''' </remarks>
    <ExportAPI("totalSumNorm")>
    Public Function totalSumNorm(matrix As Matrix, Optional scale As Double = 10000) As Matrix
        Dim samples = matrix.sampleID _
            .Select(Function(ref)
                        Dim v As Vec = matrix.sample(ref)
                        Dim col As New NamedValue(Of Vec)(ref, scale * v / v.Sum)

                        Return col
                    End Function) _
            .ToArray
        Dim norm As New Matrix With {
           .sampleID = matrix.sampleID,
           .tag = $"totalSumNorm({matrix.tag})",
           .expression = matrix.expression _
               .Select(Function(gene, i)
                           Return New DataFrameRow With {
                               .geneID = gene.geneID,
                               .experiments = samples _
                                   .Select(Function(v) v.Value(i)) _
                                   .ToArray
                           }
                       End Function) _
               .ToArray
        }

        Return norm
    End Function

    ''' <summary>
    ''' normalize data by feature rows
    ''' </summary>
    ''' <param name="matrix">a gene expression matrix</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' row/max(row)
    ''' </remarks>
    <ExportAPI("relative")>
    Public Function relative(matrix As Matrix) As Matrix
        Return New Matrix With {
            .sampleID = matrix.sampleID,
            .expression = matrix.expression _
                .Select(Function(gene)
                            Return New DataFrameRow With {
                                .geneID = gene.geneID,
                                .experiments = New Vec(gene.experiments) / gene.experiments.Max
                            }
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' This function performs clustering analysis of time course data. 
    ''' Calculate gene expression pattern by cmeans algorithm.
    ''' </summary>
    ''' <param name="matrix">
    ''' the gene expression matrix object which could be generated by 
    ''' <see cref="loadExpression"/> api.
    ''' </param>
    ''' <param name="dim">
    ''' the partition matrix size, it is recommended 
    ''' that width should be equals to the height of the partition 
    ''' matrix.</param>
    ''' <param name="fuzzification">the cmeans fuzzification parameter</param>
    ''' <param name="threshold">the cmeans threshold parameter</param>
    ''' <returns></returns>
    <ExportAPI("expression.cmeans_pattern")>
    Public Function CmeansPattern(matrix As Matrix,
                                  <RRawVectorArgument>
                                  Optional [dim] As Object = "3,3",
                                  Optional fuzzification# = 2,
                                  Optional threshold# = 0.001,
                                  Optional env As Environment = Nothing) As ExpressionPattern

        Return InteropArgumentHelper _
            .getSize([dim], env, "3,3") _
            .Split(","c) _
            .Select(AddressOf Integer.Parse) _
            .DoCall(Function(dimension)
                        Return ExpressionPattern.CMeansCluster(
                            matrix:=matrix,
                            [dim]:=dimension.ToArray,
                            fuzzification:=fuzzification,
                            threshold:=threshold
                        )
                    End Function)
    End Function

    ''' <summary>
    ''' run cmeans clustering in 3 patterns
    ''' </summary>
    ''' <param name="matrix">a gene expression matrix object</param>
    ''' <param name="fuzzification">the cmeans fuzzification parameter</param>
    ''' <param name="threshold">the cmeans threshold parameter</param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("expression.cmeans3D")>
    Public Function CMeans3D(matrix As Matrix, Optional fuzzification# = 2, Optional threshold# = 0.001) As ExpressionPattern
        Return ExpressionPattern.CMeansCluster3D(matrix, fuzzification, threshold)
    End Function

    ''' <summary>
    ''' save the cmeans expression pattern result to local file
    ''' </summary>
    ''' <param name="pattern"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("savePattern")>
    Public Function savePattern(pattern As ExpressionPattern, file As String) As Boolean
        Return Writer.WriteExpressionPattern(pattern, file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
    End Function

    ''' <summary>
    ''' read the cmeans expression pattern result from file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("readPattern")>
    Public Function readPattern(file As String) As ExpressionPattern
        Return Reader.ReadExpressionPattern(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
    End Function

    ''' <summary>
    ''' get cluster membership matrix
    ''' </summary>
    ''' <param name="pattern"></param>
    ''' <returns></returns>
    <ExportAPI("cmeans_matrix")>
    <Extension>
    Public Function GetCmeansPattern(pattern As ExpressionPattern,
                                     Optional memberCutoff As Double = 0.8,
                                     Optional env As Environment = Nothing) As Object

        Dim result As DataSet() = pattern _
            .Patterns _
            .Select(Function(a)
                        Return New DataSet With {
                            .ID = a.uid,
                            .Properties = a.memberships _
                                .ToDictionary(Function(c) $"#{c.Key + 1}",
                                              Function(c)
                                                  Return c.Value
                                              End Function)
                        }
                    End Function) _
            .ToArray
        Dim kmeans As EntityClusterModel() = result _
            .ToKMeansModels _
            .ToArray
        Dim clusterId As String() = kmeans _
            .Select(Function(p) p.Properties.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim max = clusterId _
            .ToDictionary(Function(id) id,
                            Function(id)
                                Return kmeans _
                                    .Select(Function(v) v(id)) _
                                    .Max
                            End Function)

        For Each item As EntityClusterModel In kmeans
            Dim tags As String() = item.Properties _
                .Where(Function(c) c.Value / max(c.Key) > memberCutoff) _
                .OrderByDescending(Function(c) c.Value) _
                .Select(Function(c) c.Key) _
                .ToArray

            If tags.IsNullOrEmpty Then
                item.Cluster = item.Properties _
                    .OrderByDescending(Function(c) c.Value) _
                    .Take(1) _
                    .Select(Function(cl) cl.Key) _
                    .JoinBy("; ")
            ElseIf tags.Length = max.Count Then
                item.Cluster = item.Properties _
                    .OrderByDescending(Function(c) c.Value) _
                    .Take(3) _
                    .Select(Function(cl) cl.Key) _
                    .JoinBy("; ")
            Else
                item.Cluster = tags.Take(3).JoinBy("; ")
            End If
        Next

        Return kmeans
    End Function

    ''' <summary>
    ''' split the cmeans cluster output into multiple parts based on the cluster tags
    ''' </summary>
    ''' <param name="cmeans"></param>
    ''' <returns></returns>
    <ExportAPI("split.cmeans_clusters")>
    Public Function splitCMeansClusters(cmeans As EntityClusterModel()) As Object
        Dim split = cmeans _
            .Select(Iterator Function(c) As IEnumerable(Of EntityClusterModel)
                        For Each id As String In c.Cluster.StringSplit(";\s*")
                            Yield New EntityClusterModel With {
                                .Cluster = id,
                                .ID = c.ID,
                                .Properties = New Dictionary(Of String, Double)(c.Properties)
                            }
                        Next
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(c) c.Cluster) _
            .ToArray

        Return split.ToDictionary(Function(a) a.Key, Function(a) a.ToArray)
    End Function

    ''' <summary>
    ''' This function performs clustering analysis of time course data
    ''' </summary>
    ''' <param name="matrix">A gene expression data matrix object</param>
    ''' <param name="nsize">
    ''' the layout of the cmeans clustering visualization
    ''' </param>
    ''' <param name="threshold">the cmeans threshold</param>
    ''' <param name="plotSize">the image size of the cmeans plot</param>
    ''' <param name="colorSet">
    ''' the color palatte name
    ''' </param>
    ''' <param name="fuzzification">
    ''' cmeans fuzzification parameter
    ''' </param>
    ''' <param name="memberCutoff">
    ''' the cmeans membership cutoff value for create a molecule cluster
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' this function returns a tuple list that contains the pattern 
    ''' cluster matrix and the cmeans pattern plots.
    ''' 
    ''' 1. 'pattern' is a dataframe object that contains the object cluster patterns
    ''' 2. 'image' is a bitmap image that plot based on the object cluster patterns data.
    ''' 
    ''' </returns>
    <ExportAPI("peakCMeans")>
    Public Function cmeans(matrix As Matrix,
                           Optional nsize$ = "3,3",
                           Optional threshold As Double = 10,
                           Optional fuzzification# = 2,
                           <RRawVectorArgument>
                           Optional plotSize As Object = "8100,5200",
                           Optional colorSet As String = "Jet",
                           Optional memberCutoff As Double = 0.8,
                           Optional xlab As String = "Spatial Regions",
                           Optional ylab As String = "z-score(Normalized Intensity)",
                           Optional top_members As Double = 0.2,
                           Optional env As Environment = Nothing) As Object

        Dim println As Action(Of Object) = env.WriteLineHandler
        Dim size As Size = InteropArgumentHelper.getSize(nsize, env).SizeParser

        If matrix Is Nothing OrElse matrix.size = 0 OrElse matrix.sampleID.IsNullOrEmpty Then
            Call env.AddMessage("The given expression matrix is empty!")
            Return Nothing
        End If

        Dim patterns As ExpressionPattern = ExpressionPattern.CMeansCluster(
            matrix:=matrix,
            [dim]:={size.Width, size.Height},
            fuzzification:=fuzzification,
            threshold:=threshold
        )
        Dim output As New list With {
            .slots = New Dictionary(Of String, Object)
        }
        Dim kmeans = patterns.GetCmeansPattern(memberCutoff, env)

        Call println($"membership cutoff for the cmeans patterns is: {memberCutoff}")
        Call println(patterns.ToSummaryText(memberCutoff))
        Call patterns _
            .DrawMatrix(
                size:=InteropArgumentHelper.getSize(plotSize, env, "8100,5200"),
                colorSet:=colorSet,
                xlab:=xlab,
                ylab:=ylab,
                xAxisLabelRotate:=45,
                padding:="padding:100px 100px 300px 100px;",
                membershipCutoff:=memberCutoff,
                topMembers:=top_members
            ).AsGDIImage _
                .DoCall(Sub(img)
                            Call output.add("image", img)
                        End Sub)

        Call println("export cmeans pattern matrix!")
        Call output.add("pattern", kmeans)

        Return output
    End Function

    <ExportAPI("deg.t.test")>
    Public Function Ttest(matrix As Matrix,
                          sampleinfo As SampleInfo(),
                          treatment$,
                          control$,
                          Optional level# = 1.5,
                          Optional pvalue# = 0.05,
                          Optional FDR# = 0.05,
                          Optional env As Environment = Nothing) As DEP_iTraq()

        Return matrix _
            .Ttest(
                treatment:=sampleinfo.TakeGroup(treatment).SampleIDs,
                control:=sampleinfo.TakeGroup(control).SampleIDs
            ) _
            .DepFilter2(level, pvalue, FDR)
    End Function

    ''' <summary>
    ''' get gene Id list
    ''' </summary>
    ''' <param name="dep"></param>
    ''' <returns></returns>
    <ExportAPI("geneId")>
    Public Function geneId(dep As DEP_iTraq()) As String()
        Return dep.Select(Function(a) a.ID).ToArray
    End Function

    ''' <summary>
    ''' create gene expression DEG model
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="logFC"></param>
    ''' <param name="pvalue"></param>
    ''' <param name="label"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.deg")>
    <RApiReturn(GetType(DEGModel))>
    Public Function createDEGModels(<RRawVectorArgument> x As Object,
                                    Optional logFC As String = "logFC",
                                    Optional pvalue As String = "pvalue",
                                    Optional label As String = "id",
                                    Optional env As Environment = Nothing) As Object

        If TypeOf x Is Rdataframe Then
            Dim table As Rdataframe = DirectCast(x, Rdataframe)
            Dim foldchanges As Double() = REnv.asVector(Of Double)(table(logFC))
            Dim pvalues As Double() = REnv.asVector(Of Double)(table(pvalue))
            Dim labels As String() = REnv.asVector(Of String)(table(label))

            Return foldchanges _
                .Select(Function(fc, i)
                            Return New DEGModel With {
                                .label = labels(i),
                                .logFC = foldchanges(i),
                                .pvalue = pvalues(i)
                            }
                        End Function) _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(Rdataframe), x.GetType, env)
        End If
    End Function

    <ExportAPI("deg.class")>
    Public Function DEGclass(deg As DEGModel(), <RRawVectorArgument> classLabel As Object) As DEGModel()
        Dim classList As String() = REnv.asVector(Of String)(classLabel)
        Dim getClass As Func(Of Integer, String)

        If classList.Length = 1 Then
            getClass = Function() classList(Scan0)
        Else
            getClass = Function(i) classList(i)
        End If

        Return deg _
            .Select(Function(d, i)
                        Return New DEGModel With {
                            .[class] = getClass(i),
                            .label = d.label,
                            .logFC = d.logFC,
                            .pvalue = d.pvalue
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' do matrix join by samples
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("joinSample")>
    Public Function joinSamples(samples As Matrix()) As Matrix
        If samples.IsNullOrEmpty Then
            Return Nothing
        ElseIf samples.Length = 1 Then
            Return samples(Scan0)
        Else
            Return mergeMultiple(samples)
        End If
    End Function

    Private Function mergeMultiple(multiple As Matrix()) As Matrix
        Dim matrix As Matrix = multiple(Scan0)
        Dim geneIndex = matrix.expression.ToDictionary(Function(g) g.geneID)
        Dim sampleList As New List(Of String)(matrix.sampleID)

        For Each append As Matrix In multiple.Skip(1)
            For Each gene As DataFrameRow In append.expression
                Dim v As Double() = New Double(sampleList.Count + append.sampleID.Length - 1) {}
                Dim a As Double()
                Dim b As Double() = gene.experiments

                If geneIndex.ContainsKey(gene.geneID) Then
                    a = geneIndex(gene.geneID).experiments
                Else
                    a = New Double(sampleList.Count - 1) {}
                End If

                Call Array.ConstrainedCopy(a, Scan0, v, Scan0, a.Length)
                Call Array.ConstrainedCopy(b, Scan0, v, a.Length, b.Length)

                geneIndex(gene.geneID) = New DataFrameRow With {
                    .experiments = v,
                    .geneID = gene.geneID
                }
            Next

            Call sampleList.AddRange(append.sampleID)
        Next

        Return New Matrix With {
            .expression = geneIndex.Values.ToArray,
            .sampleID = sampleList.ToArray,
            .tag = multiple _
                .Select(Function(m) m.tag) _
                .JoinBy("+")
        }
    End Function

End Module
