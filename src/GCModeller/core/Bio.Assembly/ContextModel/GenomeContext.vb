#Region "Microsoft.VisualBasic::37baa66c45b7dcc49a61a8de199a8876, GCModeller\core\Bio.Assembly\ContextModel\GenomeContext.vb"

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

    '   Total Lines: 187
    '    Code Lines: 129
    ' Comment Lines: 37
    '   Blank Lines: 21
    '     File Size: 6.91 KB


    '     Class GenomeContext
    ' 
    '         Properties: AllFeatureKeys, N, size
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Absent, Delta, GetByFeature, SelectByRange, selectByStrand
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    Public Class GenomeContext(Of T As IGeneBrief)

        Dim plus As T()
        Dim minus As T()
        ''' <summary>
        ''' 按照<see cref="NucleotideLocation.Left"/>从小到大排序的
        ''' </summary>
        Dim sequence As T()
        Dim featureTags As Dictionary(Of String, T())
        ''' <summary>
        ''' The name of this genome
        ''' </summary>
        Dim contextName$

        Default Public ReadOnly Property Feature(i As Integer) As T
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return sequence(i)
            End Get
        End Property

        ''' <summary>
        ''' The number of genes in this genome
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property N As Integer
            Get
                Return plus.Length + minus.Length
            End Get
        End Property

        ''' <summary>
        ''' 得到根据所输入的位点信息估算出目标基因组可能的大小
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            Get
                Return sequence _
                    .Select(Function(g) g.Location) _
                    .Select(Function(loci) {loci.Left, loci.Right}) _
                    .IteratesALL _
                    .Max
            End Get
        End Property

        Public ReadOnly Property AllFeatureKeys As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return featureTags.Keys.ToArray
            End Get
        End Property

        Sub New(genome As IEnumerable(Of T), Optional name$ = "unnamed")
            featureTags = genome _
                .GroupBy(Function(g)
                             If g.Feature.StringEmpty Then
                                 Return "-"
                             Else
                                 Return g.Feature
                             End If
                         End Function) _
                .ToDictionary(Function(g) g.Key,
                              Function(genes)
                                  Return genes.ToArray
                              End Function)

            ' plus的时候，左边是序列的起始方向
            ' minus的时候，右边是序列的起始方向
            plus = selectByStrand(Strands.Forward) _
                .OrderBy(Function(gene) gene.Location.Left) _
                .ToArray
            minus = selectByStrand(Strands.Reverse) _
                .OrderByDescending(Function(gene) gene.Location.Right) _
                .ToArray
            sequence = (plus.AsList + minus) _
                .OrderBy(Function(gene) gene.Location.Left) _
                .ToArray

            contextName = name
        End Sub

        ''' <summary>
        ''' The number of genes between feature 1 and feature 2.
        ''' </summary>
        ''' <param name="feature1$"></param>
        ''' <param name="feature2$"></param>
        ''' <returns></returns>
        Public Function Delta(feature1$, feature2$) As Double
            Dim l1 = GetByFeature(feature1)
            Dim l2 = GetByFeature(feature2).AsList
            Dim d As New List(Of Integer)

            ' 两两组合，取距离最小的一对ij作为计算的对象，然后取均值？
            For Each i In l1
                Dim j = l2.OrderBy(Function(lj) lj.Location.GetATGDistance(i)).First
                l2 -= j

                ' 然后数这个区间内存在多少个基因
                If i.Location.Right < j.Location.Left Then
                    ' i --> j
                    d += SelectByRange(i.Location.Right, j.Location.Left).Count
                Else
                    ' j --> i
                    d += SelectByRange(j.Location.Right, i.Location.Left).Count
                End If
            Next

            If d.Count = 1 Then
                Return d.First
            Else
                Return d.Average
            End If
        End Function

        ''' <summary>
        ''' 将基因组上面的某一区域内的基因对象都查找出来
        ''' </summary>
        ''' <param name="i%"></param>
        ''' <param name="j%"></param>
        ''' <param name="strand">
        ''' 默认不限制链的方向
        ''' </param>
        ''' <returns></returns>
        Public Iterator Function SelectByRange(i%, j%, Optional strand As Strands = Strands.Unknown) As IEnumerable(Of T)
            Dim range As New IntRange({i, j})
            Dim start As Boolean
            Dim source As T()

            Select Case strand
                Case Strands.Forward
                    source = plus
                Case Strands.Reverse
                    source = minus
                Case Else
                    source = sequence
            End Select

            For Each gene As T In source
                If range.IsOverlapping(gene.Location) OrElse range.IsInside(gene.Location) Then
                    start = True
                    Yield gene
                Else
                    ' 因为sequence是按照left排序的，所以假若start之后没有结果了，
                    ' 则肯定就没有结果了，在这里跳出循环节省时间
                    If start Then
                        Exit For
                    End If
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function selectByStrand(strand As Strands) As T()
            Return featureTags _
                .Values _
                .IteratesALL _
                .Where(Function(g) g.Location.Strand = strand) _
                .ToArray
        End Function

        Public Function GetByFeature(feature As String) As T()
            If featureTags.ContainsKey(feature) Then
                Return featureTags(feature)
            Else
                Return {}
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Absent(feature As String) As Boolean
            Return Not featureTags.ContainsKey(feature)
        End Function

        Public Overrides Function ToString() As String
            Return $"{contextName}: {plus.Length} (+), {minus.Length} (-) with {featureTags.Count} features."
        End Function
    End Class
End Namespace
