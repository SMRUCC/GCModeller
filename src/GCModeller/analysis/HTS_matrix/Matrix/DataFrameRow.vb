#Region "Microsoft.VisualBasic::dcde3511f87cb9ef8692c9631538800b, GCModeller\analysis\HTS_matrix\Matrix\DataFrameRow.vb"

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

    '   Total Lines: 73
    '    Code Lines: 45
    ' Comment Lines: 16
    '   Blank Lines: 12
    '     File Size: 2.34 KB


    ' Class DataFrameRow
    ' 
    '     Properties: experiments, geneID, samples
    ' 
    '     Function: CreateVector, ToDataSet, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' The gene expression data samples file.(基因的表达数据样本)
''' </summary>
''' <remarks></remarks>
Public Class DataFrameRow : Implements INamedValue, IVector

    Public Property geneID As String Implements INamedValue.Key

    ''' <summary>
    ''' This gene's expression value in the different experiment condition.(同一个基因在不同实验之下的表达值)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property experiments As Double() Implements IVector.Data

    Default Public ReadOnly Property Value(i As Integer) As Double
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
    ''' <returns></returns>
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

    Public Function CreateVector() As Vector
        Return experiments.AsVector
    End Function

    Public Overrides Function ToString() As String
        Return $"{geneID} -> {experiments.Select(Function(a) a.ToString("F3")).JoinBy(", ")}"
    End Function
End Class
