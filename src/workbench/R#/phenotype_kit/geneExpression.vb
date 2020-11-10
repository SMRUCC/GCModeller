#Region "Microsoft.VisualBasic::a07661e38be11be09ef8a5a9b3f7ec42, phenotype_kit\geneExpression.vb"

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
'     Constructor: (+1 Overloads) Sub New
'     Function: average, loadExpression, relative
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ExpressionPattern
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports Vec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' the gene expression matrix data toolkit
''' </summary>
<Package("geneExpression")>
Module geneExpression

    Sub New()
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of ExpressionPattern)(Function(a) DirectCast(a, ExpressionPattern).ToSummaryText)
    End Sub

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
                                   Optional env As Environment = Nothing) As Object

        Dim ignores As Index(Of String) = If(exclude_samples, {})

        If TypeOf file Is String Then
            Return Matrix.LoadData(DirectCast(file, String), ignores, rm_ZERO)
        ElseIf TypeOf file Is Rdataframe Then
            Dim table As Rdataframe = DirectCast(file, Rdataframe)
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
                    .Where(Function(gene) Not gene.experiments.All(Function(x) x = 0.0)) _
                    .ToArray
            End If

            Return New Matrix With {
                .expression = genes,
                .sampleID = sampleNames
            }
        ElseIf REnv.isVector(Of DataSet)(file) Then
            Dim rows As DataSet() = REnv.asVector(Of DataSet)(file)
            Dim matrix As New Matrix With {.sampleID = rows.PropertyNames}
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
                    .Where(Function(gene) Not gene.experiments.All(Function(x) x = 0.0)) _
                    .ToArray
            End If

            matrix.expression = genes

            Return matrix
        Else
            Return Message.InCompatibleType(GetType(Rdataframe), file.GetType, env)
        End If
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
                                  Optional threshold# = 0.001) As ExpressionPattern

        Return InteropArgumentHelper _
            .getSize([dim], "3,3") _
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
End Module
