#Region "Microsoft.VisualBasic::6c05c6ea84f66c7d89edac902f4f36bf, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\Features\FEATURES.vb"

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

    '   Total Lines: 145
    '    Code Lines: 92
    ' Comment Lines: 34
    '   Blank Lines: 19
    '     File Size: 5.44 KB


    '     Class FEATURES
    ' 
    '         Properties: source
    ' 
    '         Function: GetByLocation, GetEnumerator, GetEnumerator1, ListFeatures, Query
    ' 
    '         Sub: Add, AddGenes, Delete, LinkEntry, SetSourceFeature
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

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

        ''' <summary>
        ''' The ``source`` feature definition.
        ''' </summary>
        ''' <returns></returns>
        Public Property source As Feature
            Get
                Return _innerList(Scan0)
            End Get
            Friend Set(value As Feature)
                If _innerList.IsNullOrEmpty Then
                    _innerList = New List(Of Feature)(value)
                Else
                    _innerList.Insert(Scan0, value)
                End If
            End Set
        End Property

        ''' <summary>
        '''Get feature site data by nucleotide location.
        ''' </summary>
        ''' <param name="left"></param>
        ''' <param name="right"></param>
        ''' <returns></returns>
        Public Function GetByLocation(left As Integer, right As Integer) As Feature()
            Dim loci As New NucleotideLocation(left, right)
            Dim LQuery = LinqAPI.Exec(Of Feature) <=
                From x As Feature
                In _innerList
                Where loci.Equals(x.Location.ContiguousRegion, 3)
                Select x

            Return LQuery
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

            If String.IsNullOrEmpty(Key) Then
                Dim LQuery = From e As Feature
                             In Me._innerList
                             Where InStr(e.Query(qKey), KeyValue)
                             Select e '
                Return LQuery.ToArray
            Else
                Dim LQuery = From e As Feature
                             In Me._innerList
                             Where String.Equals(Key, e.KeyName) AndAlso InStr(e.Query(qKey), KeyValue)
                             Select e '
                Return LQuery.ToArray
            End If
        End Function

        ''' <summary>
        ''' 列举出所有 该字段之下的域的数据
        ''' </summary>
        ''' <param name="fieldName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ListFeatures(fieldName As String) As IEnumerable(Of Feature)
            Return From feature As Feature
                   In _innerList
                   Let assert As Boolean = String.Equals(feature.KeyName, fieldName, StringComparison.OrdinalIgnoreCase)
                   Where True = assert
                   Select feature
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
                    .Location = x.Location
                }

                Call gene.SetValue(FeatureQualifiers.locus_tag, x.Query(FeatureQualifiers.locus_tag))
                Call _innerList.Insert(i, gene)
            Next
        End Sub

        Public Sub Add(feature As Feature)
            Call _innerList.Add(feature)
        End Sub

        Public Sub Delete(featureKey As String, locus_tag$)
            _innerList = _innerList _
                .Where(Function(feature)
                           Return Not (feature.KeyName = featureKey AndAlso feature.Query(FeatureQualifiers.locus_tag) = locus_tag)
                       End Function) _
                .AsList
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
