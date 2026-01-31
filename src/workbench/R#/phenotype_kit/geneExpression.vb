#Region "Microsoft.VisualBasic::8acdbfb086a321475d9693d8d239ff21, R#\phenotype_kit\geneExpression.vb"

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

'   Total Lines: 1471
'    Code Lines: 903 (61.39%)
' Comment Lines: 432 (29.37%)
'    - Xml Docs: 94.68%
' 
'   Blank Lines: 136 (9.25%)
'     File Size: 60.29 KB


' Module geneExpression
' 
'     Function: add_gauss, Aggregate, applyPCA, average, castGenericRows
'               cmeans, CMeans3D, CmeansPattern, createDEGModels, createVectorList
'               DEGclass, depDataTable, dimensionNotAgree, dims, exp
'               expDataTable, filter, filterNaN, filterZeroGenes, filterZeroSamples
'               geneId, GetCmeansPattern, GetCmeansPatternA, getFuzzyPatternMembers, getMatrixInformation
'               imputeMissing, joinSamples, loadExpression, loadFromDataFrame, loadFromGenericDataSet
'               loadMatrixView, log, matrixSummary, ranking, readBinaryMatrix
'               readPattern, relative, representatives, savePattern, setGeneIDs
'               setSampleIDs, setTag, setZero, splitCMeansClusters, toClusters
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
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ExpressionPattern
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports clr_df = Microsoft.VisualBasic.Data.Framework.DataFrame
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports r_vec = SMRUCC.Rsharp.Runtime.Internal.Object.vector
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports std = System.Math
Imports std_vec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' the gene expression matrix data toolkit
''' </summary>
<Package("geneExpression")>
Module geneExpression

    Friend Sub Main()
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of ExpressionPattern)(Function(a) DirectCast(a, ExpressionPattern).ToSummaryText)
        REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(DEP_iTraq()), AddressOf depDataTable)
        REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(Matrix), AddressOf expDataTable)
        REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(DEGModel()), AddressOf degTable)
        REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(LimmaTable()), AddressOf limma_df)
        REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(ImpactResult()), AddressOf impact_table)
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of DEGModel)(Function(a) a.ToString)

        Call REnv.Internal.generic.add(
            name:="as.list",
            x:=GetType(ExpressionPattern),
            [overloads]:=AddressOf getFuzzyPatternMembers
        )
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function impact_table(genes As ImpactResult(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .rownames = genes.Keys,
            .columns = New Dictionary(Of String, Array)
        }

        Call df.add("name", From gene As ImpactResult In genes Select gene.name)
        Call df.add("category", From gene As ImpactResult In genes Select gene.class)

        Call df.add("impacts_total", From gene As ImpactResult In genes Select gene.total)
        Call df.add("top", From gene As ImpactResult In genes Select gene.top_group)
        Call df.add("max_impact", From gene As ImpactResult In genes Select gene.max)
        Call df.add("significants", From gene As ImpactResult In genes Select gene.significant)
        Call df.add("impacts", From gene As ImpactResult In genes Select gene.impacts.Select(Function(a) $"{a.Key}:{a.Value}").JoinBy("; "))

        Return df
    End Function

    <RGenericOverloads("as.data.frame")>
    Private Function limma_df(limma As LimmaTable(), args As list, env As Environment) As Rdataframe
        Dim df As New Rdataframe With {
            .rownames = limma.Keys,
            .columns = New Dictionary(Of String, Array)
        }

        Call df.add(NameOf(LimmaTable.logFC), From g As LimmaTable In limma Select g.logFC)
        Call df.add(NameOf(LimmaTable.AveExpr), From g As LimmaTable In limma Select g.AveExpr)
        Call df.add(NameOf(LimmaTable.t), From g As LimmaTable In limma Select g.t)
        Call df.add("P.Value", From g As LimmaTable In limma Select g.P_Value)
        Call df.add("adj.P.Val", From g As LimmaTable In limma Select g.adj_P_Val)
        Call df.add(NameOf(LimmaTable.B), From g As LimmaTable In limma Select g.B)
        Call df.add(NameOf(LimmaTable.class), From g As LimmaTable In limma Select g.class)

        Return df
    End Function

    <RGenericOverloads("as.data.frame")>
    Private Function degTable(degs As DEGModel(), args As list, env As Environment) As Rdataframe
        Dim df As New Rdataframe With {
            .rownames = degs.Keys.ToArray,
            .columns = New Dictionary(Of String, Array)
        }
        Dim metabolomics As Boolean = CLRVector.asScalarLogical(args.getBySynonyms("metabolite", "vip"))

        Call df.add("t", From gi As DEGModel In degs Select gi.t)
        Call df.add("log2fc", From gi As DEGModel In degs Select gi.logFC)
        Call df.add("p-value", From gi As DEGModel In degs Select gi.pvalue)

        If metabolomics Then
            Call df.add("vip", From gi As DEGModel In degs Select gi.VIP)
        End If

        Call df.add("class", From gi As DEGModel In degs Select gi.class)

        Return df
    End Function

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

    <ExportAPI("exp")>
    <ROperator("^")>
    Public Function exp(x As Matrix, p As Double) As Matrix
        Return Matrix.Exp(x, p)
    End Function

    <ROperator("+")>
    Public Function add(x As Matrix, y As Double) As Matrix
        Return Matrix.Add(x, y)
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
    ''' <param name="x">
    ''' a HTS data matrix of samples in column and gene features in row
    ''' </param>
    ''' <returns>
    ''' a tuple list that contains the dimension information of the 
    ''' gene expression matrix data:
    ''' 
    ''' + feature_size: the number of the matrix rows, or count of genes in matrix
    ''' + feature_names: a character vector of the gene ids for each rows
    ''' + sample_size: the number of the samples, or number of the matrix columns
    ''' + sample_names: the matrix column names, the sample id set
    ''' </returns>
    <ExportAPI("dims")>
    <RApiReturn("feature_size", "feature_names", "sample_size", "sample_names")>
    Public Function dims(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        If TypeOf x Is Matrix Then
            Dim mat As Matrix = DirectCast(x, Matrix)

            Return New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"feature_size", mat.expression.Length},
                    {"feature_names", mat.rownames},
                    {"sample_size", mat.sampleID.Length},
                    {"sample_names", mat.sampleID}
                }
            }
        Else
            Dim pull As pipeline = pipeline.TryCreatePipeline(Of IGeneExpression)(x, env)

            If pull.isError Then
                Return pull.getError
            End If

            Dim matrix As IGeneExpression() = pull.populates(Of IGeneExpression)(env).ToArray
            Dim sample_ids As String() = matrix _
                .Select(Function(g) g.Expression.Keys) _
                .IteratesALL _
                .Distinct _
                .OrderBy(Function(id) id) _
                .ToArray

            Return New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"feature_size", matrix.Length},
                    {"feature_names", matrix.Select(Function(g) g.Identity).ToArray},
                    {"sample_size", sample_ids.Length},
                    {"sample_names", sample_ids}
                }
            }
        End If
    End Function

    ''' <summary>
    ''' convert the matrix into row gene list
    ''' </summary>
    ''' <param name="expr0"></param>
    ''' <returns>a tuple list of the expression numeric vector, each slot data 
    ''' is the vector of expression value of a gene, slot key name is the 
    ''' corresponding gene id.</returns>
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
    ''' get gene expression vector data
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="geneId"></param>
    ''' <returns>a numeric vector of the target gene expression across multiple samples</returns>
    <ExportAPI("expression_vector")>
    <RApiReturn(TypeCodes.double)>
    Public Function expressionVector(x As Matrix, geneId As String) As Object
        Dim row As DataFrameRow = x(geneId)

        If row Is Nothing Then
            Call $"gene feature is missing from the expression matrix {x.tag}".warning
            Return Nothing
        Else
            Return New r_vec(x.sampleID, row.experiments)
        End If
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
    ''' get/set new sample id list to the matrix columns
    ''' </summary>
    ''' <param name="x">target gene expression matrix object</param>
    ''' <param name="sample_ids">
    ''' a character vector of the new sample id list for
    ''' set to the sample columns of the gene expression 
    ''' matrix.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' this function will get sample_id character vector from the input matrix if the 
    ''' <paramref name="sample_ids"/> parameter is missing, otherwise it will set the new 
    ''' sample id list to the input matrix object and return the modified matrix object.
    ''' 
    ''' if the input <paramref name="x"/> object is not a valid gene expression matrix object,
    ''' then a error message object will be returned.
    ''' </returns>
    ''' <remarks>
    ''' it is kind of ``colnames`` liked function for dataframe object.
    ''' </remarks>
    <ExportAPI("sample_id")>
    <RApiReturn(GetType(Matrix), GetType(MatrixViewer), GetType(String))>
    Public Function setSampleIDs(x As Object,
                                 <RByRefValueAssign>
                                 Optional sample_ids As String() = Nothing,
                                 Optional env As Environment = Nothing) As Object

        If sample_ids.IsNullOrEmpty Then
            ' getter
            If TypeOf x Is Matrix Then
                Return DirectCast(x, Matrix).sampleID
            ElseIf TypeOf x Is MatrixViewer Then
                Return DirectCast(x, MatrixViewer).SampleIDs.ToArray
            Else
                Return Message.InCompatibleType(GetType(Matrix), x.GetType, env)
            End If
        End If

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
        Return RInternal.debug.stop({
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
    <RApiReturn(GetType(Matrix))>
    Public Function filterZeroSamples(mat As Matrix, Optional env As Environment = Nothing) As Object
        Return mat.T.TrimZeros.T
    End Function

    ''' <summary>
    ''' removes the rows which all gene expression result is ZERO
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="env"></param>
    ''' <returns>A new expression matrix object that with gene row 
    ''' features subset from the original input raw matrix object.</returns>
    ''' <example>
    ''' 
    ''' </example>
    <ExportAPI("filterZeroGenes")>
    <RApiReturn(GetType(Matrix))>
    Public Function filterZeroGenes(mat As Matrix, Optional env As Environment = Nothing) As Object
        Return mat.TrimZeros
    End Function

    ''' <summary>
    ''' set the NaN missing value to default value
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="missingDefault">
    ''' set NA missing value to zero by default
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("filterNaNMissing")>
    <RApiReturn(GetType(Matrix))>
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
    ''' set the zero value to the half of the min positive value
    ''' </summary>
    ''' <param name="x">an expression matrix object that may contains zero</param>
    ''' <returns>
    ''' An expression data matrix with missing data filled
    ''' </returns>
    <ExportAPI("impute_missing")>
    <RApiReturn(GetType(Matrix))>
    Public Function imputeMissing(x As Matrix, Optional by_features As Boolean = False) As Object
        Dim samples = x.sampleID
        Dim v As Double()
        Dim posMin As Double
        Dim pos As Double()

        If by_features Then
            Call VBDebugger.EchoLine("fill missing data by gene feature rows")

            For Each gene As DataFrameRow In x.expression
                pos = gene.experiments.Where(Function(vi) (Not vi.IsNaNImaginary) AndAlso vi > 0).ToArray
                posMin = If(pos.Length > 0, pos.Min, 0) / 2

                For i As Integer = 0 To samples.Length - 1
                    If gene.experiments(i).IsNaNImaginary OrElse gene.experiments(i) <= 0 Then
                        gene.experiments(i) = posMin
                    End If
                Next
            Next
        Else
            For i As Integer = 0 To samples.Length - 1
                v = x.sample(i)
                pos = v.Where(Function(vi) (Not vi.IsNaNImaginary) AndAlso vi > 0).ToArray
                posMin = If(pos.Length > 0, pos.Min, 0) / 2

                For row As Integer = 0 To v.Length - 1
                    If x.gene(row)(i) <= 0 Then
                        x.gene(row).experiments(i) = posMin
                    End If
                Next
            Next
        End If

        Return x
    End Function

    ''' <summary>
    ''' load an expressin matrix data
    ''' </summary>
    ''' <param name="file">
    ''' the file path or the file stream data of the target 
    ''' expression matrix table file, or the expression data frame object
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
    ''' <example>
    ''' load.expr(file = "./rawdata.csv")
    ''' </example>
    <ExportAPI("load.expr")>
    <RApiReturn(GetType(Matrix))>
    Public Function loadExpression(file As Object,
                                   Optional exclude_samples As String() = Nothing,
                                   Optional rm_ZERO As Boolean = False,
                                   Optional makeNames As Boolean = False,
                                   Optional makeUnique As Boolean = True,
                                   Optional env As Environment = Nothing) As Object

        Dim ignores As Index(Of String) = If(exclude_samples, {})
        Dim x As Matrix = Nothing

        If TypeOf file Is String Then
            Dim filepath As String = DirectCast(file, String)
            Dim mat As Matrix = Matrix.LoadData(filepath, ignores, rm_ZERO)

            x = mat
        ElseIf TypeOf file Is Rdataframe Then
            x = DirectCast(file, Rdataframe).loadFromDataFrame(rm_ZERO, ignores)
        ElseIf REnv.isVector(Of DataSet)(file) Then
            x = DirectCast(REnv.asVector(Of DataSet)(file), DataSet()).loadFromGenericDataSet(rm_ZERO, ignores)
        ElseIf TypeOf file Is clr_df Then
            x = DirectCast(file, clr_df).loadFromClrDataframe(rm_ZERO, ignores)
        Else
            Return Message.InCompatibleType(GetType(Rdataframe), file.GetType, env)
        End If

        If makeUnique Then
            Return x.uniqueGeneId(makeNames)
        Else
            Return x
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
    Public Function readBinaryMatrix(<RRawVectorArgument>
                                     file As Object,
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
    ''' <example>
    ''' let expr_mat = load.expr(file = "./rawdata.csv");
    ''' let view = load.matrixView(mat = expr_mat);
    ''' </example>
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
    ''' <returns>
    ''' A tuple list object that contains the data information
    ''' which is extract from the given file:
    ''' 
    ''' 1. sampleID: a character vector that contains the matrix sample information(column features name)
    ''' 2. geneID: a character vector that contains the matrix gene features information(row features name)
    ''' 3. tag: the matrix source tag label, could be the file basename if the given input file is a file path to the matrix.
    ''' </returns>
    ''' <example>
    ''' str(matrix_info(file = "/path/to/expr_mat.csv"));
    ''' </example>
    <ExportAPI("matrix_info")>
    <RApiReturn("sampleID", "geneID", "tag", "mad")>
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
            summary.add("mad", HTS.expression.Select(Function(gene) gene.MAD).ToArray)

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
    ''' <param name="expr">The gene expression matrix object</param>
    ''' <param name="file">The file path to a csv matrix file that used 
    ''' for export the given <paramref name="expr"/> matrix data.</param>
    ''' <param name="id">The string content inside the first cell</param>
    ''' <param name="binary">
    ''' write matrix data in binary data format? default value 
    ''' is False means write matrix as csv matrix file.
    ''' </param>
    ''' <returns>
    ''' A logical vector for indicates that the expression 
    ''' matrix save success or not.
    ''' </returns>
    ''' <example>
    ''' geneExpression::write.expr_matrix(expr_mat, file = "/path/to/matrix.csv", id = "gene_id");
    ''' </example>
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
    ''' make matrix samples column projection
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="sampleIds"></param>
    ''' <returns></returns>
    <ExportAPI("project")>
    Public Function project(x As Matrix, <RRawVectorArgument> sampleIds As Object) As Matrix
        Dim samples As String() = CLRVector.asCharacter(sampleIds)

        If samples.IsNullOrEmpty Then
            Return Nothing
        Else
            Return x.Project(samples)
        End If
    End Function

    ''' <summary>
    ''' Filter the geneID rows
    ''' </summary>
    ''' <param name="HTS">A gene expression matrix object</param>
    ''' <param name="geneId">A character vector for run the matrix feature row filter</param>
    ''' <param name="exclude">matrix a subset of the data matrix excepts the 
    ''' input <paramref name="geneId"/> features or just make a subset which 
    ''' just contains the input <paramref name="geneId"/> features.
    ''' </param>
    ''' <returns>
    ''' A new expression matrix object that consist with gene feature
    ''' rows subset from the original matrix input.
    ''' </returns>
    <ExportAPI("filter")>
    <RApiReturn(GetType(Matrix))>
    Public Function filter(HTS As Matrix,
                           Optional geneId As String() = Nothing,
                           Optional instr As String = Nothing,
                           Optional exclude As Boolean = False,
                           Optional env As Environment = Nothing) As Object

        If geneId.IsNullOrEmpty AndAlso instr.StringEmpty Then
            Call env.AddMessage("no gene content was filtered due to the reason of no gene id list or title search text was provided!", MSG_TYPES.WRN)
            Return HTS
        End If

        Dim newMatrix As Matrix

        If Not geneId.IsNullOrEmpty Then
            Dim filterIndex As Index(Of String) = geneId.Indexing

            newMatrix = New Matrix With {
                .tag = $"filtered_geneids({HTS.tag})",
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
        Else
            newMatrix = New Matrix With {
                .tag = $"filtered_title({HTS.tag})",
                .sampleID = HTS.sampleID,
                .expression = HTS.expression _
                    .AsParallel _
                    .Where(Function(gene)
                               If exclude Then
                                   Return Strings.InStr(gene.geneID, instr, CompareMethod.Text) <= 0
                               Else
                                   Return Strings.InStr(gene.geneID, instr, CompareMethod.Text) > 0
                               End If
                           End Function) _
                    .ToArray
            }
        End If

        Return newMatrix
    End Function

    <Extension>
    Private Function loadFromClrDataframe(table As clr_df, rm_zero As Boolean, ignores As Index(Of String)) As Matrix
        Dim sampleNames As String() = table.features.Keys.Where(Function(c) Not c Like ignores).ToArray
        Dim genes As DataFrameRow() = New clr_df With {
                .rownames = table.rownames,
                .features = table.features _
                    .Where(Function(c) Not c.Key Like ignores) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Value
                                  End Function)
        } _
            .foreachRow() _
            .Select(Function(v)
                        Return New DataFrameRow With {
                            .geneID = v.name,
                            .experiments = v.value _
                                .Select(Function(obj) CDbl(obj)) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        If rm_zero Then
            genes = genes _
                .Where(Function(gene)
                           Return Not gene.experiments.All(Function(x) x = 0.0)
                       End Function) _
                .ToArray
        End If

        Return New Matrix With {
            .expression = genes,
            .sampleID = sampleNames,
            .tag = table.description
        }
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
        Dim unique As String() = If(makeNames, geneId.makeNames(unique:=True), geneId.UniqueNames)

        For i As Integer = 0 To unique.Length - 1
            m(i).geneID = unique(i)
        Next

        Return m
    End Function

    ''' <summary>
    ''' cast the HTS matrix object to the general dataset
    ''' </summary>
    ''' <param name="matrix">a gene expression matrix</param>
    ''' <returns>
    ''' A scibasic generic dataset object collection.
    ''' </returns>
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
    ''' evaluate the MAD value for each gene features
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("mad")>
    Public Function mad(x As Matrix) As list
        Dim val As list = list.empty

        For Each gene As DataFrameRow In x.expression
            val.slots(gene.geneID) = gene.MAD
        Next

        Return val
    End Function

    ''' <summary>
    ''' take top n expression feature by rank expression MAD value desc
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="top">take top N gene features</param>
    ''' <returns></returns>
    <ExportAPI("sort_mad")>
    Public Function sort_mad(x As Matrix, Optional top As Integer = 10000) As Matrix
        Dim sort = x.expression _
            .OrderByDescending(Function(xi) xi.MAD) _
            .Take(top) _
            .ToArray

        Return New Matrix With {
            .expression = sort,
            .sampleID = x.sampleID,
            .tag = $"sort_mad({x.tag})"
        }
    End Function

    ''' <summary>
    ''' function alias of the ``geneExpression::average`` function
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="groups"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("aggregate_samples")>
    <RApiReturn(GetType(Matrix))>
    Public Function aggregate_samples(matrix As Matrix, <RListObjectArgument> groups As list, Optional env As Environment = Nothing) As Object
        Dim sampleinfo As SampleInfo() = groups.slots _
            .Select(Iterator Function(a) As IEnumerable(Of SampleInfo)
                        For Each id As String In CLRVector.asCharacter(a.Value)
                            Yield New SampleInfo With {
                                .sample_info = a.Key,
                                .ID = id,
                                .sample_name = id
                            }
                        Next
                    End Function) _
            .IteratesALL _
            .ToArray

        If sampleinfo.IsNullOrEmpty Then
            Return RInternal.debug.stop("no sample group information for make sample column aggregation", env)
        End If

        Return Matrix.MatrixSum(matrix, sampleinfo, strict:=True)
    End Function

    <ExportAPI("aggregate_genes")>
    <RApiReturn(GetType(Matrix))>
    Public Function aggregate_genes(x As Matrix) As Object
        Return x.Aggregate(byrow:=True)
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
    ''' <param name="sampleinfo">The sample group data</param>
    ''' <param name="strict">
    ''' will try to ignores of the missing sample if strict option is off.
    ''' </param>
    ''' <returns>
    ''' this function return value is determined based on the sampleinfo parameter:
    ''' 
    ''' 1. for sampleinfo not nothing, a matrix with sample group as the sample feature data will be returns
    ''' 2. for missing sampleinfo data, a numeric vector of average value for each gene feature will be returns
    ''' </returns>
    <ExportAPI("average")>
    <RApiReturn(GetType(Matrix), GetType(Double))>
    <Extension>
    Public Function average(matrix As Matrix,
                            Optional sampleinfo As SampleInfo() = Nothing,
                            Optional strict As Boolean = True) As Object

        If sampleinfo.IsNullOrEmpty Then
            If Not sampleinfo Is Nothing Then
                Call "the provided sample information is not nothing, but collection is empty. numeric vector of average for each gene expression will be returns.".warning
            End If
            Return matrix.expression.Select(Function(v) v.Average).ToArray
        Else
            Return Matrix.MatrixAverage(matrix, sampleinfo, strict:=strict)
        End If
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
        Dim data As StatisticsObject = x.expression.CommonDataSet(x.sampleID)
        Dim pcaResult = PCA.PrincipalComponentAnalysis(data, maxPC:=npc)
        Dim pcaSpace = pcaResult.GetPCAScore
        Dim embedded As New Rdataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = x.rownames
        }
        Dim colnames As String() = pcaSpace.featureNames

        For i As Integer = 0 To npc - 1
            Call embedded.add($"PC{i + 1}", CLRVector.asNumeric(pcaSpace(colnames(i)).vector))
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
        Return TPM.Normalize(matrix, scale)
    End Function

    ''' <summary>
    ''' normalize data by feature rows
    ''' </summary>
    ''' <param name="matrix">a gene expression matrix</param>
    ''' <param name="median">
    ''' normalize the matrix row by median value of each row?
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' row/max(row)
    ''' </remarks>
    <ExportAPI("relative")>
    Public Function relative(matrix As Matrix, Optional median As Boolean = False) As Matrix
        Return New Matrix With {
            .sampleID = matrix.sampleID,
            .expression = matrix.expression _
                .Select(Function(gene)
                            Dim factor As Double = If(median,
                                gene.experiments.Median,
                                gene.experiments.Max)

                            If median AndAlso factor = 0.0 Then
                                Dim minmax As DoubleRange = gene.experiments

                                ' try to avoid divid zero
                                If minmax.Length = 0 Then
                                    ' all zero
                                    Return New DataFrameRow With {
                                        .geneID = gene.geneID,
                                        .experiments = gene.experiments.ToArray
                                    }
                                Else
                                    factor = minmax.Max / 2
                                End If
                            End If

                            Return New DataFrameRow With {
                                .geneID = gene.geneID,
                                .experiments = New std_vec(gene.experiments) / factor
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
    ''' <param name="file">a binary data pack file that contains the expression pattern raw data.
    ''' if this file is given by a csv file, then this csv file should be the cmeans cluster 
    ''' membership matrix outtput.</param>
    ''' <param name="samples">
    ''' should be a csv file path to the sample matrix data if the input <paramref name="file"/>
    ''' is a csv membership matrix file.
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function can also read the csv matrix file and 
    ''' then cast as the expression pattern data object.
    ''' </remarks>
    <ExportAPI("readPattern")>
    Public Function readPattern(file As String, Optional samples As String = Nothing) As ExpressionPattern
        If file.ExtensionSuffix("csv") Then
            Dim sampleData As DataSet() = Nothing

            If samples.FileExists Then
                sampleData = DataSet.LoadDataSet(samples, silent:=True).ToArray
            End If

            Return DataSet.LoadDataSet(file, silent:=True) _
                .ToArray _
                .CastAsPatterns(sampleData)
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

            Call tops.add("#" & (patternKey + 1), topId)
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
    ''' 3. 'pdf' is a pdf image that could be edit
    ''' 
    ''' </returns>
    <ExportAPI("peakCMeans")>
    Public Function cmeans(matrix As Matrix,
                           <RRawVectorArgument>
                           Optional nsize As Object = "3,3",
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
                           <RRawVectorArgument>
                           Optional margin As Object = "padding:100px 100px 300px 100px;",
                           Optional cluster_label_css As String = CSSFont.PlotSubTitle,
                           Optional legend_title_css As String = CSSFont.Win7Small,
                           Optional legend_tick_css As String = CSSFont.Win7Small,
                           Optional axis_tick_css$ = CSSFont.Win10Normal,
                           Optional axis_label_css$ = CSSFont.Win7Small,
                           Optional grid_fill As String = NameOf(Color.LightGray),
                           Optional grid_draw As Boolean = True,
                           Optional x_lab_rotate As Double = 45,
                           Optional env As Environment = Nothing) As Object

        Dim println As Action(Of Object) = env.WriteLineHandler
        Dim size As Size = InteropArgumentHelper.getSize(nsize, env).SizeParser
        Dim padStr As String = InteropArgumentHelper.getPadding(margin, default:="padding:100px 100px 300px 100px;", env:=env)

        If matrix Is Nothing OrElse matrix.size = 0 OrElse matrix.sampleID.IsNullOrEmpty Then
            Call env.AddMessage("The given expression matrix is empty!")
            Return Nothing
        End If

        Dim dpi As Integer = graphicsPipeline.getDpi(New Dictionary(Of String, Object), env, [default]:=300)
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
        Dim plot = Function(driver As Drivers) As GraphicsData
                       Return patterns.DrawMatrix(
                           size:=InteropArgumentHelper.getSize(plotSize, env, "8100,5200"),
                           colorSet:=colorSet,
                           xlab:=xlab,
                           ylab:=ylab,
                           xAxisLabelRotate:=x_lab_rotate,
                           padding:=padStr,
                           membershipCutoff:=memberCutoff,
                           topMembers:=top_members,
                           driver:=driver,
                           ppi:=dpi,
                           clusterLabelStyle:=cluster_label_css,
                           legendTitleStyle:=legend_title_css,
                           legendTickStyle:=legend_tick_css,
                           axisTickCSS:=axis_tick_css,
                           axisLabelCSS:=axis_label_css,
                           gridFill:=grid_fill,
                           gridDraw:=grid_draw
                       )
                   End Function

        Call println($"membership cutoff for the cmeans patterns is: {memberCutoff}")
        Call println(patterns.ToSummaryText(memberCutoff))

        Call output.add("image", plot(Drivers.GDI))
        Call output.add("pdf", plot(Drivers.PDF))

        Call println("export cmeans pattern matrix!")
        Call output.add("pattern", kmeans)
        Call output.add("cmeans", patterns)

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
    ''' The limma algorithm (Linear Models for Microarray Data) is a widely used statistical framework in R/Bioconductor 
    ''' for differential expression (DE) analysis of RNA-seq data. Originally designed for microarray studies, its 
    ''' flexibility and robustness have extended its utility to RNA-seq through the voomtransformation. 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="design"></param>
    ''' <returns></returns>
    <ExportAPI("limma")>
    Public Function limma(x As Matrix, design As DataAnalysis) As LimmaTable()
        Return x.LmFit(design).ToArray
    End Function

    <ExportAPI("read_limma")>
    Public Function readLimmaTable(file As String) As LimmaTable()
        Return LimmaTable.LoadTable(file).ToArray
    End Function

    <ExportAPI("limma_impactsort")>
    <RApiReturn(GetType(ImpactResult))>
    Public Function limma_impactSort(<RRawVectorArgument> x As Object,
                                     Optional top As Integer = Integer.MaxValue,
                                     Optional [class] As list = Nothing,
                                     Optional names As list = Nothing,
                                     Optional env As Environment = Nothing) As Object

        Dim groups As New List(Of NamedCollection(Of LimmaTable))
        Dim class_labels As New Dictionary(Of String, String)
        Dim nameSet As New Dictionary(Of String, String)

        If Not names Is Nothing Then
            nameSet = names.AsGeneric(Of String)(env)
        End If
        If Not [class] Is Nothing Then
            Dim class_groups = [class].AsGeneric(Of String())(env)

            For Each group In class_groups
                For Each id As String In group.Value
                    class_labels(id) = group.Key
                Next
            Next
        End If

        If TypeOf x Is list Then
            For Each group In DirectCast(x, list).slots
                Dim limma_genes As pipeline = pipeline.TryCreatePipeline(Of LimmaTable)(group.Value, env)

                If limma_genes.isError Then
                    Return limma_genes.getError
                End If

                Call groups.Add(New NamedCollection(Of LimmaTable)(
                     name:=group.Key,
                     data:=limma_genes.populates(Of LimmaTable)(env))
                )
            Next
        Else
            Dim single_group As pipeline = pipeline.TryCreatePipeline(Of LimmaTable)(x, env)

            If single_group.isError Then
                Return single_group.getError
            End If

            Call groups.Add(New NamedCollection(Of LimmaTable)(
                name:="single group",
                data:=single_group.populates(Of LimmaTable)(env)
            ))
        End If

        Dim impacts = groups.ImpactSort().Take(top).ToArray

        If class_labels.IsNullOrEmpty Then
            For Each gene As ImpactResult In impacts
                gene.class = class_labels.TryGetValue(gene.id)
            Next
        End If
        If Not nameSet.IsNullOrEmpty Then
            For Each gene As ImpactResult In impacts
                gene.name = nameSet.TryGetValue(gene.id, [default]:=gene.id)
            Next
        End If

        Return impacts
    End Function

    ''' <summary>
    ''' build limma table model from the dataframe columns
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="logFC"></param>
    ''' <param name="aveExpr"></param>
    ''' <param name="t"></param>
    ''' <param name="pval"></param>
    ''' <param name="adj_pval"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    <ExportAPI("limma_table")>
    Public Function createlimmaTable(<RRawVectorArgument> id As Object,
                                     <RRawVectorArgument> logFC As Object,
                                     <RRawVectorArgument> aveExpr As Object,
                                     <RRawVectorArgument> t As Object,
                                     <RRawVectorArgument> pval As Object,
                                     <RRawVectorArgument> adj_pval As Object,
                                     <RRawVectorArgument> b As Object) As LimmaTable()

        Dim idcol As String() = CLRVector.asCharacter(id)
        Dim logFCcol As Double() = CLRVector.asNumeric(logFC)
        Dim meancol As Double() = CLRVector.asNumeric(aveExpr)
        Dim tcol As Double() = CLRVector.asNumeric(t)
        Dim pvalcol As Double() = CLRVector.asNumeric(pval)
        Dim adjpvalcol As Double() = CLRVector.asNumeric(adj_pval)
        Dim bcol As Double() = CLRVector.asNumeric(b)
        Dim table As LimmaTable() = idcol _
            .Select(Function(gene_id, i)
                        Return New LimmaTable With {
                            .id = gene_id,
                            .adj_P_Val = adjpvalcol(i),
                            .AveExpr = meancol(i),
                            .B = bcol(i),
                            .logFC = logFCcol(i),
                            .P_Value = pvalcol(i),
                            .t = tcol(i)
                        }
                    End Function) _
            .ToArray

        Return table
    End Function

    ''' <summary>
    ''' log scale of the HTS raw matrix
    ''' </summary>
    ''' <param name="expr">should be a HTS expression matrix object</param>
    ''' <param name="base"></param>
    ''' <returns>this function may produce negative expression value if the value number is less than 1.</returns>
    <ExportAPI("log")>
    <RApiReturn(GetType(Matrix))>
    Public Function log(<RRawVectorArgument> expr As Object, Optional base As Double = std.E) As Object
        If TypeOf expr Is Matrix Then
            Return DirectCast(expr, Matrix).log(base)
        Else
            ' this function its function name is conflict with the math log function
            ' in the R# base runtime environment.
            '
            ' do math log of a numeric vector at here.
            Return CLRVector.asNumeric(expr) _
                .AsVector _
                .Log(base) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' min max normalization
    ''' 
    ''' (row - min(row)) / (max(row) - min(row))
    ''' 
    ''' this normalization method is usually used for the metabolomics data
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("minmax01Norm")>
    Public Function minmax01(x As Matrix) As Object
        Return x.MinMaxNorm
    End Function

    <ExportAPI("take_shuffle")>
    Public Function take_shuffle(x As Matrix, n As Integer) As dataframe
        Dim sample = x.expression.OrderBy(Function() randf.NextDouble()).Take(n).ToArray
        Dim data As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = sample _
                .Select(Function(r) r.geneID) _
                .ToArray
        }

        For Each gene As DataFrameRow In sample
            Call data.add(gene.geneID, gene.experiments)
        Next

        Return data
    End Function

    ''' <summary>
    ''' get gene Id list or byref set of the gene id alias set.
    ''' </summary>
    ''' <param name="x">
    ''' A collection of the deg/dep object or a raw HTS matrix object
    ''' </param>
    ''' <returns>A collection of the gene id set</returns>
    ''' <example>
    ''' let rnaseqs = load.expr("rnaseq.csv");
    ''' let alias = readLines("gene_id.txt");
    ''' 
    ''' print(rnaseqs |> geneId());
    ''' 
    ''' geneId(rnaseqs) &lt;- alias;
    ''' </example>
    <ExportAPI("geneId")>
    <RApiReturn(GetType(String))>
    Public Function geneId(<RRawVectorArgument> x As Object,
                           <RRawVectorArgument>
                           <RByRefValueAssign>
                           Optional set_id As Object = Nothing,
                           Optional env As Environment = Nothing) As Object

        If TypeOf x Is Matrix Then
            If Not set_id Is Nothing Then
                Dim set_idset As String() = CLRVector.asCharacter(set_id)
                Dim set_matrix As Matrix = DirectCast(x, Matrix)

                For i As Integer = 0 To set_matrix.expression.Length - 1
                    set_matrix.expression(i).geneID = set_idset(i)
                Next

                Call set_matrix.ResetIndex()
            End If

            Return DirectCast(x, Matrix).rownames
        Else
            Dim deps As pipeline = pipeline.TryCreatePipeline(Of DEP_iTraq)(x, env)

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
    ''' <param name="x">usually be a dataframe object of the different expression analysis result</param>
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

    ''' <summary>
    ''' set deg class label
    ''' </summary>
    ''' <param name="deg"></param>
    ''' <param name="class_labels">
    ''' set deg class label manually
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("deg.class")>
    <RApiReturn(GetType(DEGModel))>
    Public Function DEGclass(deg As DEGModel(), <RRawVectorArgument> Optional class_labels As Object = Nothing,
                             Optional logFC As Double = 1,
                             Optional pval_cutoff As Double = 0.05) As Object

        Dim classList As String() = CLRVector.asCharacter(class_labels)
        Dim getClass As Func(Of Integer, String)

        If classList.IsNullOrEmpty Then
            ' set sig/not_sig via logfc and pvalue cutoff
            classList = deg _
                .Select(Function(gene)
                            If gene.pvalue < pval_cutoff AndAlso std.Abs(gene.logFC) > logFC Then
                                Return "sig"
                            Else
                                Return "not_sig"
                            End If
                        End Function) _
                .ToArray
        End If

        getClass = GetVectorElement _
            .Create(Of String)(classList) _
            .Getter(Of String)

        Return deg _
            .Select(Function(d, i)
                        Return New DEGModel With {
                            .[class] = getClass(i),
                            .label = d.label,
                            .logFC = d.logFC,
                            .pvalue = d.pvalue,
                            .fdr = d.fdr,
                            .t = d.t,
                            .VIP = d.VIP
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
    Public Function joinSamples(samples As Matrix(), Optional strict As Boolean = True) As Matrix
        If samples.IsNullOrEmpty Then
            Return Nothing
        ElseIf samples.Length = 1 Then
            Return samples(Scan0)
        Else
            Return samples.MergeMultipleHTSMatrix(strict)
        End If
    End Function

    ''' <summary>
    ''' merge multiple gene expression matrix by gene features
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="strict"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("joinFeatures")>
    <RApiReturn(GetType(Matrix))>
    Public Function joinFeatures(<RRawVectorArgument> x As Object, Optional strict As Boolean = True, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of Matrix)(x, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim matrixList As Matrix() = pull _
            .populates(Of Matrix)(env) _
            .ToArray

        If matrixList.IsNullOrEmpty Then
            Return Nothing
        ElseIf matrixList.Length = 1 Then
            Return matrixList(Scan0)
        Else
            Return matrixList.MergeFeatures(strict)
        End If
    End Function

    ''' <summary>
    ''' merge row or column where the tag is identical
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="byrow">
    ''' default by gene feature row means merge the duplicated genes with the idential gene id as tag.
    ''' otherwise will merge the duplicated samples with the identical sample id name.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("aggregate")>
    <Extension>
    Public Function Aggregate(x As Matrix, Optional byrow As Boolean = True) As Object
        If byrow Then
            Dim rows As New Dictionary(Of String, std_vec)

            ' merge the duplicated genes
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

    ''' <summary>
    ''' add random gauss noise to the matrix
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="scale"></param>
    ''' <returns></returns>
    <ExportAPI("add_gauss")>
    Public Function add_gauss(x As Matrix, Optional scale As Double = 0.1) As Matrix
        Dim width As Integer = x.sampleID.Length

        For i As Integer = 0 To x.size - 1
            x.expression(i).experiments += (x.expression(i) * std_vec.rand(-scale, scale, width))
        Next

        Return x
    End Function

    <ExportAPI("as.abundance_matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function metagenome_matrix(<RRawVectorArgument> samples As Object,
                                      Optional normalized As Boolean = False,
                                      Optional env As Environment = Nothing) As Object

        Dim sampleList As New List(Of NamedValue(Of Dictionary(Of String, Double)))
        Dim input As list = TryCast(samples, list)

        If input Is Nothing Then
            Throw New NotImplementedException
        End If

        Dim null As New List(Of String)

        For Each name As String In input.getNames
            Dim data As Object = input.getByName(name)
            Dim v As Dictionary(Of String, Double)

            If data Is Nothing Then
                Call null.Add(name)
            End If

            Dim pull As pipeline = pipeline.TryCreatePipeline(Of IExpressionValue)(data, env, suppress:=True)

            If Not pull.isError Then
                v = pull.populates(Of IExpressionValue)(env).ExpressionValue
            Else
                If TypeOf data Is list Then
                    v = DirectCast(data, list).AsGeneric(Of Double)(env)
                ElseIf TypeOf data Is Dictionary(Of String, Double) Then
                    v = DirectCast(data, Dictionary(Of String, Double))
                ElseIf TypeOf data Is Dictionary(Of String, Integer) Then
                    v = DirectCast(data, Dictionary(Of String, Integer)).AsNumeric
                ElseIf TypeOf data Is Dictionary(Of Integer, Double) Then
                    v = DirectCast(data, Dictionary(Of Integer, Double)) _
                        .ToDictionary(Function(a) a.ToString,
                                      Function(a)
                                          Return a.Value
                                      End Function)
                Else
                    Throw New NotImplementedException(data.GetType.FullName)
                End If
            End If

            Call sampleList.Add(New NamedValue(Of Dictionary(Of String, Double))(name, v))
        Next

        If null.Any Then
            Call $"the sample data of '{null.JoinBy(", ")}' is nothing".warning
        End If

        Return MatrixBuilder.BuildAndNormalizeAbundanceMatrix(sampleList.ToArray, normalized)
    End Function
End Module
