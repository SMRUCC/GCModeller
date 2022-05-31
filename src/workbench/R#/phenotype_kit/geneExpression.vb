#Region "Microsoft.VisualBasic::b4b681effba1805a72bab6be9fab1319, R#\phenotype_kit\geneExpression.vb"

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

' Module geneExpression
' 
'     Function: average, castGenericRows, CMeans3D, CmeansPattern, createDEGModels
'               DEGclass, depDataTable, expDataTable, filter, geneId
'               GetCmeansPattern, loadExpression, loadFromDataFrame, loadFromGenericDataSet, relative
'               Ttest, uniqueGeneId
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.MetaData
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
    ''' 
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
    ''' <param name="matrix"></param>
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
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    <ExportAPI("z_score")>
    Public Function zscore(matrix As Matrix) As Matrix
        Return New Matrix With {
            .sampleID = matrix.sampleID,
            .tag = $"z-score({matrix.tag})",
            .expression = matrix.expression _
                .Select(Function(expr)
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
    ''' normalize data by sample column
    ''' </summary>
    ''' <param name="matrix"></param>
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
    ''' <param name="matrix"></param>
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("expression.cmeans3D")>
    Public Function CMeans3D(matrix As Matrix, Optional fuzzification# = 2, Optional threshold# = 0.001) As ExpressionPattern
        Return ExpressionPattern.CMeansCluster3D(matrix, fuzzification, threshold)
    End Function

    ''' <summary>
    ''' get cluster membership matrix
    ''' </summary>
    ''' <param name="pattern"></param>
    ''' <returns></returns>
    <ExportAPI("cmeans_matrix")>
    Public Function GetCmeansPattern(pattern As ExpressionPattern, Optional kmeans_n As Integer = -1, Optional env As Environment = Nothing) As Object
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

        If kmeans_n > 0 Then
            If kmeans_n >= result.Length Then
                Return Internal.debug.stop({
                    "kmeans centers can not be greater than or equals to the data points!",
                    "data: " & result.Length,
                    "kmeans_n: " & kmeans_n
                }, env)
            Else
                Return result _
                    .ToKMeansModels _
                    .Kmeans(expected:=kmeans_n, debug:=env.globalEnvironment.debugMode) _
                    .ToArray
            End If
        Else
            Return result
        End If
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
End Module
