#Region "Microsoft.VisualBasic::d45d6427a161d6a5f1b019d00cc7cae9, GCModeller\analysis\HTS_matrix\Matrix\Matrix.vb"

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

    '   Total Lines: 309
    '    Code Lines: 210
    ' Comment Lines: 62
    '   Blank Lines: 37
    '     File Size: 10.81 KB


    ' Class Matrix
    ' 
    '     Properties: expression, rownames, (+2 Overloads) sample, sampleID, size
    '                 tag
    ' 
    '     Function: GenericEnumerator, GetEnumerator, (+3 Overloads) IndexOf, LoadData, MatrixAverage
    '               Project, T, TakeSamples, ToString, TrimZeros
    ' 
    '     Sub: checkMatrix, eachGene
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' a data matrix of samples in column and gene features in row
''' </summary>
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
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return expression(i)
        End Get
    End Property

    ''' <summary>
    ''' get gene expression vector data by a specific <paramref name="geneId"/>
    ''' </summary>
    ''' <param name="geneId"></param>
    ''' <returns>
    ''' value nothing may be returned if the gene id is not exists in the matrix rows
    ''' </returns>
    Default Public ReadOnly Property gene(geneId As String) As DataFrameRow
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return expression.KeyItem(geneId)
        End Get
    End Property

    ''' <summary>
    ''' matrix subset by row
    ''' </summary>
    ''' <returns></returns>
    Default Public ReadOnly Property gene(flags As BooleanVector) As Matrix
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return New Matrix With {
                .sampleID = sampleID,
                .tag = tag,
                .expression = expression _
                    .Select(Function(r, i) (r, flags(i))) _
                    .Where(Function(t) t.Item2) _
                    .Select(Function(t) t.r) _
                    .ToArray
            }
        End Get
    End Property

    Public ReadOnly Property sample(sample_id As String) As Vector
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Me.sample(IndexOf(sample_id))
        End Get
    End Property

    ''' <summary>
    ''' take by column
    ''' </summary>
    ''' <param name="i"></param>
    ''' <returns></returns>
    Public Property sample(i As Integer) As Vector
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return expression.Select(Function(v) v(i)).AsVector
        End Get
        Set(value As Vector)
            Dim v As Double() = value.Array

            For j As Integer = 0 To value.Length - 1
                expression(j).experiments(i) = v(j)
            Next
        End Set
    End Property

    ''' <summary>
    ''' get all gene id list
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property rownames As String()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return expression.Select(Function(g) g.geneID).ToArray
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"[{tag}] {expression.Length} genes, {sampleID.Length} samples; {sampleID.GetJson}"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IndexOf(sampleGroup As DataGroup) As Integer()
        Return IndexOf(sampleGroup.sample_id)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IndexOf(sample_id As String) As Integer
        Return Me.sampleID.Indexing.IndexOf(sample_id)
    End Function

    Public Function IndexOf(sampleName As IEnumerable(Of String)) As Integer()
        Dim index = Me.sampleID.Indexing
        Dim i As Integer() = sampleName.Select(Function(name) index(name)).ToArray

        Return i
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

    ''' <summary>
    ''' matrix transpose
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <DebuggerStepThrough>
    Public Function T() As Matrix
        Dim mat As Double()() = expression _
            .Select(Function(i) i.experiments) _
            .MatrixTranspose _
            .ToArray
        Dim rows As DataFrameRow() = mat _
            .Select(Function(c, i)
                        Return New DataFrameRow With {
                            .geneID = _sampleID(i),
                            .experiments = c
                        }
                    End Function) _
            .ToArray

        Return New Matrix With {
            .sampleID = rownames,
            .expression = rows,
            .tag = $"t({tag})"
        }
    End Function

    ''' <summary>
    ''' removes the rows which all gene expression result is ZERO
    ''' </summary>
    ''' <returns></returns>
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
    Public Shared Iterator Function TakeSamples(data As DataFrameRow(),
                                                sampleVector As Integer(),
                                                reversed As Boolean) As IEnumerable(Of DataFrameRow)
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

    Public Sub eachGene(apply As Action(Of DataFrameRow, Integer))
        For i As Integer = 0 To expression.Length - 1
            Call apply(_expression(i), i)
        Next
    End Sub

    Private Sub checkMatrix()
        Dim samples As Integer = sampleID.Length

        If expression.Any(Function(gene) gene.samples <> samples) Then
            Throw New InvalidProgramException("invalid sample data size of " & expression.Where(Function(gene) gene.geneID).GetJson)
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadData(file As String,
                                    Optional excludes As Index(Of String) = Nothing,
                                    Optional trimZeros As Boolean = False) As Matrix

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
