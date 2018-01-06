Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel
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

        Public ReadOnly Property AllFeatureKeys As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return featureTags.Keys.ToArray
            End Get
        End Property

        Sub New(genome As IEnumerable(Of T), Optional name$ = "unnamed")
            featureTags = genome _
                .GroupBy(Function(g)
                             If g.Feature.IsNullOrEmpty Then
                                 Return "-"
                             Else
                                 Return g.Feature
                             End If
                         End Function) _
                .ToDictionary(Function(g) g.Key,
                              Function(g) g.ToArray)

            plus = selectByStrand(Strands.Forward)
            minus = selectByStrand(Strands.Reverse)
            contextName = name
            sequence = (plus.AsList + minus) _
                .OrderBy(Function(g)
                             Return g.Location.Left
                         End Function) _
                .ToArray
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

        Public Iterator Function SelectByRange(i%, j%) As IEnumerable(Of T)
            Dim range As New IntRange({i, j})
            Dim start As Boolean

            For Each gene As T In sequence
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