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
Imports Microsoft.VisualBasic.Language
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
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports stdNum = System.Math
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

        Call REnv.Internal.generic.add(
            name:="as.list",
            x:=GetType(ExpressionPattern),
            [overloads]:=AddressOf getFuzzyPatternMembers
        )
    End Sub

    Private Function getFuzzyPatternMembers(x As Object, args As list, env As Environment) As Object
        Dim cutoff As Double = args.getValue("cutoff", env, [default]:=0.6)
        Dim top As Integer = args.getValue("top", env, [default]:=300)
        Dim parts = DirectCast(x, ExpressionPattern).GetPartitionMatrix(cutoff, top).IteratesALL.ToArray
        Dim pop As New list With {.slots = New Dictionary(Of String, Object)}

        For Each block As Matrix In parts
            Call pop.add("#" & block.tag, block.rownames)
        Next

        Return pop
    End Function

    ''' <summary>
    ''' as.data.frame of the HTS matrix
    ''' </summary>
    ''' <param name="exp"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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

        table.columns("FC.avg") = dep.Select(Function(p) p.foldchange).ToArray
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
    ''' <param name="mat">
    ''' a HTS data matrix of samples in column and gene features in row
    ''' </param>
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
    ''' set new sample id list to the matrix columns
    ''' </summary>
    ''' <param name="x">target gene expression matrix object</param>
    ''' <param name="sample_ids">
    ''' a character vector of the new sample id list for
    ''' set to the sample columns of the gene expression 
    ''' matrix.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' it is kind of ``colnames`` liked function for dataframe object.
    ''' </remarks>
    <ExportAPI("setSamples")>
    <RApiReturn(GetType(Matrix), GetType(MatrixViewer))>
    Public Function setSampleIDs(x As Object, sample_ids As String(), Optional env As Environment = Nothing) As Object
        If TypeOf x Is Matrix Then
            Dim expr0 As Matrix = DirectCast(x, Matrix)

            If expr0.sampleID.Length <> sample_ids.Length Then
                Return dimensionNotAgree(expr0.sampleID.Length, sample_ids.Length, "sample", env)
            Else
                expr0.sampleID = sample_ids.ToArray
                Return expr0
            End If
        ElseIf TypeOf x Is MatrixViewer Then
            Dim expr1 As MatrixViewer = DirectCast(x, MatrixViewer)

            If expr1.SampleIDs.Count <> sample_ids.Length Then
                Return dimensionNotAgree(expr1.SampleIDs.Count, sample_ids.Length, "sample", env)
            Else
                Call expr1.SetNewSampleIDs(sampleIDs:=sample_ids)
                Return expr1
            End If
        Else
            Return Message.InCompatibleType(GetType(Matrix), x.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' set new gene id list to the matrix rows
    ''' </summary>
    ''' <param name="x">target gene expression matrix object</param>
    ''' <param name="gene_ids">
    ''' a collection of the new gene ids to set to the feature
    ''' rows of the gene expression matrix.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' it is kind of ``rownames`` liked function for dataframe object.
    ''' </remarks>
    <ExportAPI("setFeatures")>
    <RApiReturn(GetType(Matrix), GetType(MatrixViewer))>
    Public Function setGeneIDs(x As Object,
                               gene_ids As String(),
                               Optional env As Environment = Nothing) As Object

        If TypeOf x Is Matrix Then
            Dim expr0 As Matrix = DirectCast(x, Matrix)

            If expr0.expression.Length <> gene_ids.Length Then
                Return dimensionNotAgree(expr0.expression.Length, gene_ids.Length, "gene", env)
            Else
                For i As Integer = 0 To gene_ids.Length - 1
                    expr0.expression(i).geneID = gene_ids(i)
                Next

                Return expr0
            End If
        ElseIf TypeOf x Is MatrixViewer Then
            Dim expr1 As MatrixViewer = DirectCast(x, MatrixViewer)

            If expr1.FeatureIDs.Count <> gene_ids.Length Then
                Return dimensionNotAgree(expr1.FeatureIDs.Count, gene_ids.Length, "gene", env)
            Else
                Call expr1.SetNewGeneIDs(geneIDs:=gene_ids)
                Return expr1
            End If
        Else
            Return Message.InCompatibleType(GetType(Matrix), x.GetType, env)
        End If
    End Function

    Private Function dimensionNotAgree(geneSize As Integer, geneIdSize As Integer, type As String, env As Environment) As Message
        Return Internal.debug.stop({
            $"dimension({geneSize} {type}) of the matrix feature must be equals to the dimension({geneIdSize} names) of the name vector!",
            $"number of {type} in matrix: {geneSize}",
            $"number of {type} id: {geneIdSize}"
        }, env)
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
    ''' removes the rows which all gene expression result is ZERO
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("filterZeroGenes")>
    Public Function filterZeroGenes(mat As Matrix, Optional env As Environment = Nothing) As Object
        Return mat.TrimZeros
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
    ''' <param name="file">
    ''' the file path or the file stream data of the target 
    ''' expression matrix table file.
    ''' </param>
    ''' <param name="exclude_samples">
    ''' will removes some sample column data from the expression
    ''' matrix which is specificed by this parameter value.
    ''' </param>
    ''' <returns>
    ''' a HTS data matrix of samples in column and gene features in row
    ''' </returns>
    ''' <remarks>
    ''' the table file format that handled by this function
    ''' could be a csv table file or tsv table file.
    ''' </remarks>
    <ExportAPI("load.expr")>
    <RApiReturn(GetType(Matrix))>
    Public Function loadExpression(file As Object,
                                   Optional exclude_samples As String() = Nothing,
                                   Optional rm_ZERO As Boolean = False,
                                   Optional makeNames As Boolean = False,
                                   Optional env As Environment = Nothing) As Object

        Dim ignores As Index(Of String) = If(exclude_samples, {})

        If TypeOf file Is String Then
            Dim filepath As String = DirectCast(file, String)
            Dim mat As Matrix = Matrix.LoadData(filepath, ignores, rm_ZERO)

            Return mat.uniqueGeneId(makeNames)
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
    ''' <returns>
    ''' a HTS data matrix of samples in column and gene features in row
    ''' </returns>
    <ExportAPI("load.expr0")>
    <RApiReturn(GetType(Matrix), GetType(HTSMatrixReader))>
    Public Function readBinaryMatrix(file As Object,
                                     Optional lazy As Boolean = False,
                                     Optional env As Environment = Nothing) As Object
        Dim stream = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If stream Like GetType(Message) Then
            Return stream.TryCast(Of Message)
        ElseIf lazy Then
            Return New HTSMatrixReader(stream.TryCast(Of Stream))
        Else
            Return BinaryMatrix.LoadStream(stream.TryCast(Of Stream))
        End If
    End Function

    ''' <summary>
    ''' Load the HTS matrix into a lazy matrix viewer
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <returns></returns>
    <ExportAPI("load.matrixView")>
    Public Function loadMatrixView(mat As Matrix) As HTSMatrixViewer
        Return New HTSMatrixViewer(mat)
    End Function

    ''' <summary>
    ''' get matrix summary information
    ''' </summary>
    ''' <param name="file">
    ''' could be a file path or the HTS matrix data object
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("matrix_info")>
    <RApiReturn("sampleID", "geneID", "tag")>
    Public Function getMatrixInformation(file As Object) As Object
        If TypeOf file Is String Then
            Dim filepath As String = file

            If filepath.ExtensionSuffix("csv", "tsv", "xls") Then
                Throw New NotImplementedException
            Else
                Using reader As New HTSMatrixReader(filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
                    Return reader.matrixSummary
                End Using
            End If
        End If

        If TypeOf file Is HTSMatrixReader Then
            Return DirectCast(file, HTSMatrixReader).matrixSummary
        ElseIf TypeOf file Is Matrix Then
            Dim HTS As Matrix = DirectCast(file, Matrix)
            Dim summary As New list With {.slots = New Dictionary(Of String, Object)}

            summary.add("tag", HTS.tag)
            summary.add("sampleID", HTS.sampleID.ToArray)
            summary.add("geneID", HTS.rownames.ToArray)

            Return summary
        Else
            Throw New NotImplementedException
        End If
    End Function

    <Extension>
    Private Function matrixSummary(reader As HTSMatrixReader) As Object
        Dim summary As New list With {.slots = New Dictionary(Of String, Object)}

        summary.add("tag", reader.tag)
        summary.add("sampleID", reader.SampleIDs.ToArray)
        summary.add("geneID", reader.FeatureIDs.ToArray)

        Return summary
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
    ''' Z-score normalized of the expression data matrix
    ''' 
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
    ''' <returns>
    ''' the HTS matrix object has been normalized in each gene 
    ''' expression row, z-score is calculated for each gene row
    ''' across multiple sample expression data.
    ''' </returns>
    ''' <remarks>
    ''' #### Standard score(z-score)
    ''' 
    ''' In statistics, the standard score is the signed number of standard deviations by which the value of 
    ''' an observation or data point is above the mean value of what is being observed or measured. Observed 
    ''' values above the mean have positive standard scores, while values below the mean have negative 
    ''' standard scores. The standard score is a dimensionless quantity obtained by subtracting the population 
    ''' mean from an individual raw score and then dividing the difference by the population standard deviation. 
    ''' This conversion process is called standardizing or normalizing (however, "normalizing" can refer to 
    ''' many types of ratios; see normalization for more).
    ''' 
    ''' > https://en.wikipedia.org/wiki/Standard_score
    ''' </remarks>
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
    ''' <param name="file">a binary data pack file that contains the expression pattern raw data</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function can also read the csv matrix file and 
    ''' then cast as the expression pattern data object.
    ''' </remarks>
    <ExportAPI("readPattern")>
    Public Function readPattern(file As String) As ExpressionPattern
        If file.ExtensionSuffix("csv") Then
            Return DataSet.LoadDataSet(file, silent:=True) _
                .ToArray _
                .CastAsPatterns
        Else
            Return Reader.ReadExpressionPattern(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        End If
    End Function

    ''' <summary>
    ''' get cluster membership matrix
    ''' </summary>
    ''' <param name="pattern"></param>
    ''' <returns></returns>
    <ExportAPI("cmeans_matrix")>
    <RApiReturn(GetType(EntityClusterModel))>
    Public Function GetCmeansPatternA(<RRawVectorArgument>
                                      pattern As Object,
                                      Optional memberCutoff As Double = 0.8,
                                      Optional empty_shared As Integer = 2,
                                      Optional max_cluster_shared As Integer = 3,
                                      Optional env As Environment = Nothing) As Object

        Dim objs = toClusters(pattern, env)

        If objs Like GetType(Message) Then
            Return objs.TryCast(Of Message)
        End If

        Dim kmeans As EntityClusterModel() = objs.TryCast(Of EntityClusterModel())
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
                    .Take(empty_shared) _
                    .Select(Function(cl) cl.Key) _
                    .JoinBy("; ")
            Else
                item.Cluster = tags _
                    .Take(max_cluster_shared) _
                    .JoinBy("; ")
            End If
        Next

        Return kmeans
    End Function

    Private Function toClusters(pattern As Object, env As Environment) As [Variant](Of EntityClusterModel(), Message)
        If TypeOf pattern Is ExpressionPattern Then
            Dim result As DataSet() = DirectCast(pattern, ExpressionPattern) _
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

            Return result _
                .ToKMeansModels _
                .ToArray
        ElseIf TypeOf pattern Is Rdataframe Then
            Dim df As Rdataframe = DirectCast(pattern, Rdataframe)
            Dim fields As String() = df.colnames _
                .Where(Function(c) Not c.TextEquals(NameOf(EntityClusterModel.Cluster))) _
                .ToArray
            Dim rows = df.forEachRow(colKeys:=fields).ToArray
            Dim colIndex = fields.SeqIterator.ToArray
            Dim entities = rows _
                .Select(Function(r)
                            Return New EntityClusterModel With {
                                .ID = r.name,
                                .Properties = colIndex.ToDictionary(Function(i) i.value, Function(i) CDbl(r.value(i.i)))
                            }
                        End Function) _
                .ToArray

            Return entities
        Else
            Dim data As pipeline = pipeline.TryCreatePipeline(Of EntityClusterModel)(pattern, env)

            If data.isError Then
                Return data.getError
            Else
                Return data.populates(Of EntityClusterModel)(env).ToArray
            End If
        End If
    End Function

    <Extension>
    Private Function GetCmeansPattern(pattern As ExpressionPattern,
                                      Optional memberCutoff As Double = 0.8,
                                      Optional empty_shared As Integer = 2,
                                      Optional max_cluster_shared As Integer = 3,
                                      Optional env As Environment = Nothing) As EntityClusterModel()

        Return GetCmeansPatternA(pattern, memberCutoff, empty_shared, max_cluster_shared, env)
    End Function

    ''' <summary>
    ''' get the top n representatives genes in each expression pattern
    ''' </summary>
    ''' <param name="pattern"></param>
    ''' <param name="top">top n cmeans membership items</param>
    ''' <returns></returns>
    <ExportAPI("pattern_representatives")>
    Public Function representatives(pattern As ExpressionPattern, Optional top As Integer = 3) As Object
        Dim allPatterns As Integer() = pattern.Patterns _
            .Select(Function(c) c.memberships.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim tops As New list With {.slots = New Dictionary(Of String, Object)}
        Dim topId As String()

        For Each patternKey As Integer In allPatterns
            topId = pattern.Patterns _
                .OrderByDescending(Function(p) p.memberships(key:=patternKey)) _
                .Take(top) _
                .Select(Function(a) a.uid) _
                .ToArray

            Call tops.add("#" & patternKey, topId)
        Next

        Return tops
    End Function

    ''' <summary>
    ''' ### split the cmeans cluster output
    ''' 
    ''' split the cmeans cluster output into multiple parts based on the cluster tags
    ''' </summary>
    ''' <param name="cmeans">the cmeans cluster result</param>
    ''' <returns>
    ''' A list object that contains the input cluster result 
    ''' data is split into multiple cluster parts.
    ''' </returns>
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
    ''' ### clustering analysis of time course data
    ''' 
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
                           Optional empty_shared As Integer = 2,
                           Optional max_cluster_shared As Integer = 3,
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
        Dim kmeans As EntityClusterModel() = patterns.GetCmeansPattern(memberCutoff, empty_shared, max_cluster_shared, env)

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

    <ExportAPI("expr_ranking")>
    Public Function ranking(x As Matrix, sampleinfo As SampleInfo()) As Ranking()
        Return x.TRanking(sampleinfo).ToArray
    End Function

    ''' <summary>
    ''' do t-test across specific analysis comparision
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="sampleinfo"></param>
    ''' <param name="treatment">group name of the treatment group</param>
    ''' <param name="control">group name of the control group</param>
    ''' <param name="level">log2FC cutoff level</param>
    ''' <param name="pvalue">the t-test pvalue cutoff</param>
    ''' <param name="FDR">the FDR cutoff</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("deg.t.test")>
    Public Function Ttest(x As Matrix,
                          sampleinfo As SampleInfo(),
                          treatment$,
                          control$,
                          Optional level# = 1.5,
                          Optional pvalue# = 0.05,
                          Optional FDR# = 0.05,
                          Optional env As Environment = Nothing) As DEP_iTraq()

        Dim i = sampleinfo.TakeGroup(treatment).SampleIDs.ToArray
        Dim j = sampleinfo.TakeGroup(control).SampleIDs.ToArray

        Return x _
            .Ttest(treatment:=i, control:=j) _
            .DepFilter2(
                level:=level,
                pvalue:=pvalue,
                FDR_threshold:=FDR
            )
    End Function

    ''' <summary>
    ''' log scale of the HTS raw matrix
    ''' </summary>
    ''' <param name="expr">the HTS expression matrix object</param>
    ''' <param name="base"></param>
    ''' <returns></returns>
    <ExportAPI("log")>
    Public Function log(expr As Matrix, Optional base As Double = stdNum.E) As Matrix
        Return expr.log(base)
    End Function

    ''' <summary>
    ''' get gene Id list
    ''' </summary>
    ''' <param name="dep">
    ''' A collection of the deg/dep object or a raw HTS matrix object
    ''' </param>
    ''' <returns>A collection of the gene id set</returns>
    <ExportAPI("geneId")>
    <RApiReturn(GetType(String))>
    Public Function geneId(<RRawVectorArgument> dep As Object, Optional env As Environment = Nothing) As Object
        If TypeOf dep Is Matrix Then
            Return DirectCast(dep, Matrix).rownames
        Else
            Dim deps As pipeline = pipeline.TryCreatePipeline(Of DEP_iTraq)(dep, env)

            If deps.isError Then
                Return deps.getError
            Else
                Return deps _
                    .populates(Of DEP_iTraq)(env) _
                    .Select(Function(a) a.ID) _
                    .ToArray
            End If
        End If
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
            Dim foldchanges As Double() = CLRVector.asNumeric(table(logFC))
            Dim pvalues As Double() = CLRVector.asNumeric(table(pvalue))
            Dim labels As String() = CLRVector.asCharacter(table(label))

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
        Dim classList As String() = CLRVector.asCharacter(classLabel)
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
    ''' <param name="samples">
    ''' matrix in multiple batches data should be normalized at
    ''' first before calling this data batch merge function.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("joinSample")>
    Public Function joinSamples(samples As Matrix()) As Matrix
        If samples.IsNullOrEmpty Then
            Return Nothing
        ElseIf samples.Length = 1 Then
            Return samples(Scan0)
        Else
            Return MergeMultipleHTSMatrix(samples)
        End If
    End Function

    ''' <summary>
    ''' merge row or column where the tag is identical
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="byrow"></param>
    ''' <returns></returns>
    <ExportAPI("aggregate")>
    Public Function Aggregate(x As Matrix, Optional byrow As Boolean = True) As Object
        If byrow Then
            Dim rows As New Dictionary(Of String, Vec)

            For Each gene As DataFrameRow In x.expression
                If rows.ContainsKey(gene.geneID) Then
                    rows(gene.geneID) += gene.experiments
                Else
                    rows(gene.geneID) = gene.experiments.AsVector
                End If
            Next

            Return New Matrix With {
                .expression = rows _
                    .Select(Function(r)
                                Return New DataFrameRow With {
                                    .geneID = r.Key,
                                    .experiments = r.Value.ToArray
                                }
                            End Function) _
                    .ToArray,
                .sampleID = x.sampleID,
                .tag = $"aggregate({x.tag})"
            }
        Else
            Throw New NotImplementedException
        End If
    End Function
End Module
