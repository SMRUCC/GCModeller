#Region "Microsoft.VisualBasic::0f63a852b5096a5b671d88c2c40d6e65, analysis\HTS_matrix\Matrix\DataFrameRow.vb"

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

    '   Total Lines: 163
    '    Code Lines: 89 (54.60%)
    ' Comment Lines: 54 (33.13%)
    '    - Xml Docs: 98.15%
    ' 
    '   Blank Lines: 20 (12.27%)
    '     File Size: 5.24 KB


    ' Class DataFrameRow
    ' 
    '     Properties: experiments, geneID, samples
    ' 
    '     Constructor: (+4 Overloads) Sub New
    '     Function: Average, CreateVector, Max, Sum, ToDataSet
    '               ToString
    '     Operators: -
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' The gene expression data samples file.
''' (基因的表达数据样本)
''' </summary>
''' <remarks>the gene expression model implements the <see cref="IVector"/> model, 
''' could be converts a vector.</remarks>
Public Class DataFrameRow : Implements INamedValue, IVector

    ''' <summary>
    ''' The unique reference id of current expression data vector
    ''' </summary>
    ''' <returns></returns>
    Public Property geneID As String Implements INamedValue.Key

    ''' <summary>
    ''' This gene's expression value in the different experiment condition.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>(同一个基因在不同实验之下的表达值)</remarks>
    Public Property experiments As Double() Implements IVector.Data

    ''' <summary>
    ''' Get the sample expression value via a given index
    ''' </summary>
    ''' <param name="i"></param>
    ''' <returns></returns>
    Default Public ReadOnly Property Value(i As Integer) As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _experiments(i)
        End Get
    End Property

    ''' <summary>
    ''' get subset of the vector by a specific sample id offsets
    ''' </summary>
    ''' <param name="i"></param>
    ''' <returns></returns>
    Default Public ReadOnly Property Value(i As Integer()) As Double()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return i _
                .Select(Function(idx) _experiments(idx)) _
                .ToArray
        End Get
    End Property

    ''' <summary>
    ''' Gets the sample counts of current gene expression data.(获取基因表达数据样本数目)
    ''' </summary>
    ''' <value></value>
    ''' <returns>
    ''' the length of the <see cref="experiments"/> expression vector.
    ''' </returns>
    ''' <remarks></remarks>
    Public ReadOnly Property samples As Integer
        Get
            If experiments Is Nothing Then
                Return 0
            Else
                Return experiments.Length
            End If
        End Get
    End Property

    ''' <summary>
    ''' MAD（Median Absolute Deviation，中位数绝对偏差）是一种鲁棒的统计量，用于衡量单变量数据的离散程度。其核心思想是通过中位数计算偏差，避免异常值对结果的影响。数学定义为：
    ''' 
    ''' ```
    ''' MAD=median(∣Xi−median(X)∣)
    ''' ```
    ''' 
    ''' 其中 X表示基因在所有样本中的表达值向量，Xi为单个样本的表达值，median(X)是基因表达值的中位数。
    ''' 与标准差不同，MAD使用中位数而非均值，因此不受极端值干扰。例如，若某基因在多数样本中表达稳定，但个别样本异常高/低，标准差会显著增大，而MAD几乎不变。
    ''' 
    ''' WGCNA等共表达网络分析中，需筛选高变异基因（如取MAD值最高的前5000个基因）。高MAD值表明基因表达在样本间波动大，可能具有生物学意义（如调控关键通路）。
    ''' </summary>
    Public ReadOnly Property MAD As Double
        Get
            Return New Vector(experiments).MAD
        End Get
    End Property

    Sub New()
    End Sub

    <DebuggerStepThrough>
    Sub New(id As String, Optional expr As IEnumerable(Of Double) = Nothing)
        Me.geneID = id
        Me.experiments = expr.ToArray
    End Sub

    Sub New(sample As NamedCollection(Of Double))
        Me.geneID = sample.name
        Me.experiments = sample.value
    End Sub

    Sub New(clone As DataFrameRow)
        Me.geneID = clone.geneID
        Me.experiments = clone.experiments.ToArray
    End Sub

    ''' <summary>
    ''' cast the numeric vector as the labeled list.
    ''' </summary>
    ''' <param name="labels"></param>
    ''' <returns></returns>
    Public Function ToDataSet(labels As String()) As Dictionary(Of String, Double)
        Dim table As New Dictionary(Of String, Double)
        Dim i As Integer = 0

        If labels.Length <> experiments.Length Then
            Throw New ArgumentException("the size of the experiment labels is not equals to the size of experiments data!")
        End If

        For Each label As String In labels
            table.Add(label, experiments(i))
            i += 1
        Next

        Return table
    End Function

    ''' <summary>
    ''' Cast current expression vector as the standard math <see cref="Vector"/> object.
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CreateVector() As Vector
        Return experiments.AsVector
    End Function

    Public Overrides Function ToString() As String
        Return $"{geneID} -> {experiments.Select(Function(a) a.ToString("F3")).JoinBy(", ")}"
    End Function

    ''' <summary>
    ''' get sum of current expression vector
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Sum() As Double
        Return experiments.Sum
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Max() As Double
        If samples = 0 Then
            Return 0
        Else
            Return experiments.Max
        End If
    End Function

    ''' <summary>Computes the average of a sequence of Double values.
    ''' </summary>
    ''' <returns>The average of the sequence of values.</returns>
    ''' <remarks>
    ''' this function returns ZERO if the sample count is ZERO
    ''' </remarks>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Average() As Double
        If samples = 0 Then
            Return 0
        Else
            Return experiments.Average
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Operator -(gene As DataFrameRow, x As Double) As Vector
        Return gene.CreateVector - x
    End Operator
End Class
