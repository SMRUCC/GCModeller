#Region "Microsoft.VisualBasic::55980885847e1d9645d0d1c7646abca8, analysis\HTS_matrix\Matrix.vb"

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
'     Properties: expression, sampleID
' 
'     Function: LoadData, MatrixAverage, TakeSamples
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class Matrix : Implements INamedValue

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
    ''' gene list
    ''' </summary>
    ''' <returns></returns>
    Public Property expression As DataFrameRow()

    Default Public ReadOnly Property gene(i As Integer) As DataFrameRow
        Get
            Return expression(i)
        End Get
    End Property

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
            samples = x.experiments.Takes(sampleVector, reversed:=reversed)

            Yield New DataFrameRow With {
                .geneID = x.geneID,
                .experiments = samples
            }
        Next
    End Function

    Public Function Project(sampleNames As String()) As Matrix
        Dim index As Index(Of String) = sampleID
        Dim sampleVector As Integer() = sampleNames.Select(Function(id) index.IndexOf(id)).ToArray

        If sampleVector.Any(Function(i) i = -1) Then
            Throw New KeyNotFoundException($"missing sample names in your data matrix: {sampleVector.SeqIterator.Where(Function(a) a.value <> -1).Select(Function(i) sampleNames(i)).GetJson}")
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadData(file As String, Optional excludes As Index(Of String) = Nothing) As Matrix
        Return Document.LoadMatrixDocument(file, excludes)
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
End Class



