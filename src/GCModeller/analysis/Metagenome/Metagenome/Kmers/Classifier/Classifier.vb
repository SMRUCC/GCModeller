#Region "Microsoft.VisualBasic::71175fef5e062f331d5012f1c14ef2f2, analysis\Metagenome\Metagenome\Kmers\Classifier\Classifier.vb"

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

    '   Total Lines: 102
    '    Code Lines: 66 (64.71%)
    ' Comment Lines: 19 (18.63%)
    '    - Xml Docs: 26.32%
    ' 
    '   Blank Lines: 17 (16.67%)
    '     File Size: 3.76 KB


    '     Class Classifier
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: MakeClassify
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel

Namespace Kmers

    Public Class Classifier : Implements IDisposable

        Dim kmers As DatabaseReader

        Sub New(kmers As DatabaseReader)
            Me.kmers = kmers
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>
        ''' the ncbi taxonomy id that this reads data be classfied, zero value means no class
        ''' </returns>
        Public Function MakeClassify(reads As String) As SequenceHit
            ' parse input reads sequence as kmers
            ' and then get kmer hits from the database
            Dim kmerHits As KmerSeed() = KSeq.KmerSpans(reads, kmers.k) _
                .Select(Function(k) kmers.GetKmer(k)) _
                .ToArray
            Dim total As Integer = kmerHits.Length
            Dim scoreMap As New Dictionary(Of UInteger, Double)
            Dim hitsMap As New Dictionary(Of UInteger, Integer)
            Dim totalScore As Double = 0
            Dim w As Double

            For Each seed As KmerSeed In kmerHits
                If seed Is Nothing Then
                    Continue For
                Else
                    w = seed.weight
                    totalScore += w
                End If

                ' 一个k-mer可能对应多个来源，需要将权重分配给所有来源
                For Each src As KmerSource In seed.source
                    If scoreMap.ContainsKey(src.seqid) Then
                        scoreMap(src.seqid) += w
                        hitsMap(src.seqid) += 1
                    Else
                        scoreMap.Add(src.seqid, w)
                        hitsMap.Add(src.seqid, 1)
                    End If
                Next
            Next

            If scoreMap.Count = 0 Then
                Return SequenceHit.Unknown
            End If

            Dim topHitSeq = scoreMap.OrderByDescending(Function(a) a.Value).First
            Dim topdata = kmers.SequenceInfomation(topHitSeq.Key)

            If topdata Is Nothing Then
                Return SequenceHit.Unknown
            End If

            Dim ratio As Double = hitsMap(topHitSeq.Key) / total
            Dim identifies As Double = topHitSeq.Value / totalScore

            Return New SequenceHit(topdata) With {
                .identities = identifies,
                .total = totalScore,
                .score = topHitSeq.Value,
                .ratio = ratio
            }
        End Function

        Dim disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call kmers.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
