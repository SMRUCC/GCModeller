#Region "Microsoft.VisualBasic::fe3e0a67b1b71edf43ae416ac571d820, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\Features\FEATURES.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    Public Class FEATURES : Inherits KeyWord
        Implements Generic.IEnumerable(Of Feature)

        Protected Friend _innerList As List(Of Feature) = New List(Of Feature)

        ''' <summary>
        ''' 匹配每一个特性位点的头部标签的格式
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend Const FEATURE_HEADER As String = " {6}[a-zA-Z_0-9]+.+\d+"

        Public Property SourceFeature As Feature

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="left"></param>
        ''' <param name="right"></param>
        ''' <returns></returns>
        Public Function GetByLocation(left As Integer, right As Integer) As Feature()
            Dim loci As New NucleotideLocation(left, right)
            Dim LQuery = (From x In _innerList Where loci.Equals(x.Location.ContiguousRegion, 3) Select x).ToArray
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
        Public Function Query(KeyValue As String,
                              Optional qKey As FeatureQualifiers = FeatureQualifiers.product,
                              Optional Key As String = "") As Feature()

            If String.IsNullOrEmpty(Key) Then
                Dim LQuery = From e As Feature In Me._innerList.AsParallel
                             Where InStr(e.Query(qKey), KeyValue)
                             Select e '
                Return LQuery.ToArray
            Else
                Dim LQuery = From e As Feature In Me._innerList.AsParallel
                             Where String.Equals(Key, e.KeyName) AndAlso InStr(e.Query(qKey), KeyValue)
                             Select e '
                Return LQuery.ToArray
            End If
        End Function

        ''' <summary>
        ''' 列举出所有 该字段之下的域的数据
        ''' </summary>
        ''' <param name="FieldName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ListFeatures(FieldName As String) As Feature()
            Dim features As Feature() =
                LinqAPI.Exec(Of Feature) <= From feature As Feature
                                            In Me._innerList
                                            Where String.Equals(
                                                feature.KeyName,
                                                FieldName,
                                                StringComparison.OrdinalIgnoreCase)
                                            Select feature
            Return features
        End Function

        ''' <summary>
        ''' 读取从某一个行号开始的文本块
        ''' </summary>
        ''' <param name="Start">The start index of the reading.(读取的起始位置)</param>
        ''' <param name="data">The text data source to read.(所读取的数据源)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function readBlock(Start As Long, data As String()) As String()
            Dim Index As Long = Start + 1

            Do While data(Index).Chars(Scan0) = "/"
                Index += 1
                If Index = data.Length Then Exit Do
            Loop

            Index -= 1
            Dim ChunkBuffer(Index - Start) As String

            Call Array.ConstrainedCopy(data, Start, ChunkBuffer, Scan0, ChunkBuffer.Count)

            Return ChunkBuffer
        End Function

        Public Shared Widening Operator CType(strData As String()) As FEATURES
            Dim Features As FEATURES = New FEATURES
            Dim Index As Long = 0
            Dim TempData As String()

            strData = __formatString(strData)

            Do While Index < strData.Length - 1
                TempData = readBlock(Index, strData)
                Index += TempData.Length

                Features._innerList.Add(item:=TempData)
            Loop

            Try
                Features.SourceFeature = (From FtureSite As Feature
                                          In Features._innerList.AsParallel
                                          Where String.Equals("source", FtureSite.KeyName)
                                          Select FtureSite).First
            Catch ex As Exception
                Call App.LogException(New Exception(strData.JoinBy(vbCrLf)))
                Features.SourceFeature = __nullFeature()
            End Try

            Return Features
        End Operator

        Private Shared Function __nullFeature() As Feature
            Return New Feature With {
                .KeyName = "source",
                .Location = New GBFF.Keywords.FEATURES.Location With {
                    .Complement = False,
                    .Locations = New RegionSegment() {
                        New RegionSegment With {
                            .Left = 0,
                            .Right = 0
                        }
                    }
                }
            }
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

        Private Shared Sub __append(str As String, ByRef new_strData As List(Of String), ByRef sBuilder As StringBuilder)
            Call new_strData.Add(sBuilder.ToString)
            Call sBuilder.Clear()
            Call sBuilder.Append(str.Trim)
        End Sub

        ''' <summary>
        ''' 去除数据中的断行
        ''' </summary>
        ''' <param name="strData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function __formatString(strData As String()) As String()
            Dim sBuilder As StringBuilder = New StringBuilder(4096)
            Dim new_strData As New List(Of String)

            For Each Line As String In strData
                If String.IsNullOrEmpty(Line) Then
                    Continue For  '  忽略掉空白行
                End If
                If Line.Length < 21 Then
                    Throw New Exception(Line & "  is not enough data! Check your GenBank file's format!")
                End If
                If Line(21) = "/"c OrElse Line(6) <> " "c Then 'this means read a new qualifier or a new feature
                    Call __append(Line, new_strData, sBuilder)
                Else
                    Call sBuilder.Append(" " & Line.Trim)
                End If
            Next
            Call new_strData.Add(sBuilder.ToString) '添加最后一行的数据

            Return new_strData.Skip(1).ToArray
        End Function

        Protected Friend Sub LinkEntry()
            For Each Feature In Me._innerList
                Feature.gbRaw = Me.gbRaw
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
