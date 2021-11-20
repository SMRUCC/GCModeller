#Region "Microsoft.VisualBasic::e6ee266ed35f5f49ffe7b97dad6eeeee, analysis\HTS_matrix\Matrix.vb"

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

' Class Matrix
' 
'     Properties: expression, sampleID, size, tag
' 
'     Function: GenericEnumerator, GetEnumerator, LoadData, MatrixAverage, Project
'               TakeSamples, ToString, TrimZeros
' 
'     Sub: checkMatrix
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class Matrix : Implements INamedValue, Enumeration(Of DataFrameRow)

    ''' <summary>
    ''' the tag data of current expression matrix
    ''' </summary>
    ''' <returns></returns>
    Public Property tag As String Implements INamedValue.Key

    ''' <summary>
    ''' sample id of <see cref="DataFrameRow.experiments"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property sampleID As String()

    ''' <summary>
    ''' gene list, vector element is the sample data
    ''' </summary>
    ''' <returns></returns>
    Public Property expression As DataFrameRow()

    ''' <summary>
    ''' the row numbers of the expression matrix(number of genes)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property size As Integer
        Get
            Return expression.Length
        End Get
    End Property

    ''' <summary>
    ''' take by row
    ''' </summary>
    ''' <param name="i"></param>
    ''' <returns></returns>
    Default Public ReadOnly Property gene(i As Integer) As DataFrameRow
        Get
            Return expression(i)
        End Get
    End Property

    Public Property sample(i As Integer) As Vector
        Get

        End Get
        Set(value As Vector)

        End Set
    End Property

    Public Overrides Function ToString() As String
        Return $"[{tag}] {expression.Length} genes, {sampleID.Length} samples; {sampleID.GetJson}"
    End Function

    ''' <summary>
    ''' make column projection via <see cref="TakeSamples(DataFrameRow(), Integer(), Boolean)"/>.
    ''' </summary>
    ''' <param name="sampleNames"></param>
    ''' <returns></returns>
    Public Function Project(sampleNames As String()) As Matrix
        Dim index As Index(Of String) = sampleID
        Dim sampleVector As Integer() = sampleNames _
            .Select(Function(id)
                        Return index.IndexOf(id)
                    End Function) _
            .ToArray

        If sampleVector.Any(Function(i) i = -1) Then
            With sampleVector _
                .SeqIterator _
                .Where(Function(a) a.value <> -1) _
                .Select(Function(i)
                            Return sampleNames(i)
                        End Function)

                Throw New KeyNotFoundException($"missing sample names in your data matrix: { .GetJson}")
            End With
        End If

        Return New Matrix With {
            .sampleID = sampleNames,
            .tag = tag,
            .expression = TakeSamples(
                data:=expression,
                sampleVector:=sampleVector,
                reversed:=False
             ).ToArray
        }
    End Function

    Public Function TrimZeros() As Matrix
        Return New Matrix With {
            .sampleID = sampleID,
            .tag = tag,
            .expression = expression _
                .Where(Function(gene) Not gene.experiments.All(Function(x) x = 0.0)) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' matrix subset by a given collection of sample names
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="sampleVector"></param>
    ''' <param name="reversed"></param>
    ''' <returns></returns>
    Public Shared Iterator Function TakeSamples(data As DataFrameRow(), sampleVector As Integer(), reversed As Boolean) As IEnumerable(Of DataFrameRow)
        Dim samples As Double()

        For Each x As DataFrameRow In data
            samples = x.experiments.Takes(
                index:=sampleVector,
                reversed:=reversed
            )

            Yield New DataFrameRow With {
                .geneID = x.geneID,
                .experiments = samples
            }
        Next
    End Function

    Private Sub checkMatrix()
        Dim samples As Integer = sampleID.Length

        If expression.Any(Function(gene) gene.samples <> samples) Then
            Throw New InvalidProgramException("invalid sample data size of " & expression.Where(Function(gene) gene.geneID).GetJson)
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadData(file As String, Optional excludes As Index(Of String) = Nothing, Optional trimZeros As Boolean = False) As Matrix
        Dim matrix As Matrix = Document.LoadMatrixDocument(file, excludes)

        Call matrix.checkMatrix()

        If trimZeros Then
            Return matrix.TrimZeros
        Else
            Return matrix
        End If
    End Function

    ''' <summary>
    ''' calculate average value of the gene expression for
    ''' each sample group.
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="sampleInfo"></param>
    ''' <returns></returns>
    Public Shared Function MatrixAverage(matrix As Matrix, sampleInfo As SampleInfo()) As Matrix
        Dim sampleIndex As Index(Of String) = matrix.sampleID
        Dim groups As NamedCollection(Of Integer)() = sampleInfo _
            .GroupBy(Function(a) a.sample_info) _
            .Select(Function(g)
                        Return New NamedCollection(Of Integer) With {
                            .name = g.Key,
                            .value = g _
                                .Select(Function(sample)
                                            Return sampleIndex.IndexOf(sample.ID)
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray
        Dim genes As DataFrameRow() = matrix.expression _
            .Select(Function(g)
                        Dim mean As Double() = groups _
                            .Select(Function(group)
                                        Return Aggregate index As Integer
                                               In group
                                               Let x As Double = g.experiments(index)
                                               Into Average(x)
                                    End Function) _
                            .ToArray

                        Return New DataFrameRow With {
                            .geneID = g.geneID,
                            .experiments = mean
                        }
                    End Function) _
            .ToArray

        Return New Matrix With {
            .sampleID = groups.Keys,
            .expression = genes
        }
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of DataFrameRow) Implements Enumeration(Of DataFrameRow).GenericEnumerator
        For Each gene As DataFrameRow In expression
            Yield gene
        Next
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of DataFrameRow).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class
