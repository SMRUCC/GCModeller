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

Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' GenBank记录之中的``FEATURES``段: 位点注释数据的列表。
    ''' </summary>
    ''' <remarks>
    ''' 这里的索引(按照类型分组索引以及位置区间索引)全部是<b>惰性构建</b>的:
    ''' 当列表被修改的时候，索引会被标记为失效，在下一次查询的时候才会重新构建。
    ''' 由于读取大型数据库文件的时候一般只会进行查询操作，因此这种方式不会带来额外的初始化开销。
    ''' </remarks>
    Public Class FEATURES : Inherits KeyWord
        Implements IEnumerable(Of Feature)

        ''' <summary>
        ''' 默认第一个元素就是``source``元素
        ''' </summary>
        Protected Friend _innerList As New List(Of Feature)

        ''' <summary>
        ''' 匹配每一个特性位点的头部标签的格式
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend Const FEATURE_HEADER As String = " {6}[a-zA-Z_0-9]+.+\d+"

        Private _source As Feature
        Private keyIndex As FeatureKeyIndex
        Private intervalIndex As FeatureIntervalIndex
        Private indexDirty As Boolean = True

        ''' <summary>
        ''' 解析过程之中所跳过的非法行的数量
        ''' </summary>
        Public Property InvalidLines As Integer = 0

        ''' <summary>
        ''' 位点的数量
        ''' </summary>
        Public ReadOnly Property Count As Integer
            Get
                Return If(_innerList Is Nothing, 0, _innerList.Count)
            End Get
        End Property

        ''' <summary>
        ''' The ``source`` feature definition.
        ''' </summary>
        ''' <returns></returns>
        Public Property source As Feature
            Get
                If _source IsNot Nothing Then
                    Return _source
                ElseIf _innerList.IsNullOrEmpty Then
                    ' 空的位点列表(例如记录之中缺失了FEATURES段): 返回一个空的source位点，
                    ' 但是不会将其添加到列表之中，从而保证位点的数量以及写出的内容是准确的
                    If _source Is Nothing Then
                        _source = ParserHelper.__nullFeature()
                    End If

                    Return _source
                Else
                    Return _innerList(Scan0)
                End If
            End Get
            Friend Set(value As Feature)
                _source = value

                If value Is Nothing Then
                    Return
                End If

                ' 兼容旧版本的行为: 当source位点不在列表之中的时候，将其插入到列表的头部。
                ' 但是当source已经存在于列表之中的时候(这是正常情况)，不会重复添加，
                ' 否则会在写出文件的时候产生重复的source位点。
                If _innerList Is Nothing Then
                    _innerList = New List(Of Feature)
                End If

                If Not _innerList.Contains(value) Then
                    Call _innerList.Insert(Scan0, value)
                End If

                indexDirty = True
            End Set
        End Property

        ''' <summary>
        ''' 使索引失效(在列表被修改之后调用)
        ''' </summary>
        Public Sub InvalidateIndex()
            indexDirty = True
        End Sub

        ''' <summary>
        ''' 按照类型名称分组的索引
        ''' </summary>
        Public ReadOnly Property Keys As FeatureKeyIndex
            Get
                If indexDirty OrElse keyIndex Is Nothing Then
                    keyIndex = New FeatureKeyIndex(_innerList)
                End If

                Return keyIndex
            End Get
        End Property

        ''' <summary>
        ''' 位置区间索引
        ''' </summary>
        Public ReadOnly Property Interval As FeatureIntervalIndex
            Get
                If indexDirty OrElse intervalIndex Is Nothing Then
                    intervalIndex = New FeatureIntervalIndex()
                    Call intervalIndex.Rebuild(_innerList)
                    indexDirty = False
                End If

                Return intervalIndex
            End Get
        End Property

        ''' <summary>
        ''' 获取指定类型的全部feature
        ''' </summary>
        ''' <param name="key">feature的类型名称，例如``CDS``/``gene``/``mRNA``</param>
        Public Function ByKey(key As String) As Feature()
            Return Keys.GetByKey(key)
        End Function

        ''' <summary>
        '''Get feature site data by nucleotide location.
        ''' </summary>
        ''' <param name="left"></param>
        ''' <param name="right"></param>
        ''' <returns>与目标区间存在重叠的全部位点</returns>
        ''' <remarks>
        ''' 旧版本的实现是对整个列表进行线性扫描，对于人基因组这种具有数万个位点的
        ''' 染色体记录而言，每一次查询都是<c>O(n)</c>的；现在改为基于有序区间索引的
        ''' 二分查询，复杂度为<c>O(log n + k)</c>。
        ''' </remarks>
        Public Function GetByLocation(left As Integer, right As Integer) As Feature()
            Return Interval.Overlap(left, right)
        End Function

        Public Function GetByLocation(loci As NucleotideLocation) As Feature()
            Return Interval.Overlap(loci.left, loci.right)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="KeyValue">Part of the key value that to search in this genbank file.</param>
        ''' <param name="qKey">Qualifier enum value of the key</param>
        ''' <param name="Key">Feature type that to search, default is search all type of the feature.</param>
        ''' <returns>The feature list that match the query conditions.</returns>
        ''' <remarks></remarks>
        Public Function Query(KeyValue$,
                              Optional qKey As FeatureQualifiers = FeatureQualifiers.product,
                              Optional Key$ = "") As Feature()

            If String.IsNullOrEmpty(KeyValue) Then
                Return New Feature() {}
            End If

            Dim source As IEnumerable(Of Feature)

            If String.IsNullOrEmpty(Key) Then
                source = _innerList
            Else
                source = ByKey(Key)
            End If

            Dim out As New List(Of Feature)

            For Each e As Feature In source
                Dim value As String = e.Query(qKey)

                If Not String.IsNullOrEmpty(value) AndAlso value.IndexOf(KeyValue, StringComparison.Ordinal) >= 0 Then
                    Call out.Add(e)
                End If
            Next

            Return out.ToArray
        End Function

        ''' <summary>
        ''' 列举出所有 该字段之下的域的数据
        ''' </summary>
        ''' <param name="fieldName">
        ''' the feature name, example as gene/CDS/tRNA/rRNA, etc
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ListFeatures(fieldName As String) As IEnumerable(Of Feature)
            Return ByKey(fieldName)
        End Function

        ''' <summary>
        ''' 在CDS之前添加基因
        ''' </summary>
        Public Sub AddGenes()
            Dim fList = (From x In Me Where String.Equals("CDS", x.KeyName, StringComparison.OrdinalIgnoreCase) Select x).ToArray

            For Each x In fList
                Dim i As Integer = _innerList.IndexOf(x)
                Dim gene As New Feature With {
                    .KeyName = "gene",
                    .Location = x.Location,
                    .gb = x.gb
                }

                Call gene.SetValue(FeatureQualifiers.locus_tag, x.Query(FeatureQualifiers.locus_tag))
                Call _innerList.Insert(i, gene)
            Next

            indexDirty = True
        End Sub

        Public Sub Add(feature As Feature)
            Call _innerList.Add(feature)
            indexDirty = True
        End Sub

        Public Sub Delete(featureKey As String, locus_tag$)
            _innerList = _innerList _
                .Where(Function(feature)
                           Return Not (feature.KeyName = featureKey AndAlso feature.Query(FeatureQualifiers.locus_tag) = locus_tag)
                       End Function) _
                .AsList
            indexDirty = True
        End Sub

        Public Sub SetSourceFeature(source As Feature)
            Me.source = source
        End Sub

        Friend Sub LinkEntry()
            For Each feature As Feature In Me._innerList
                feature.gb = Me.gb
            Next
        End Sub

        Public Iterator Function GetEnumerator() As IEnumerator(Of Feature) Implements IEnumerable(Of Feature).GetEnumerator
            For i As Integer = 0 To Me._innerList.Count - 1
                Yield _innerList(i)
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
