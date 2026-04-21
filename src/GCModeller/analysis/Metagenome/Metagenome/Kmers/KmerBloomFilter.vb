#Region "Microsoft.VisualBasic::141c110be038ad22113aad2e3c579e5e, analysis\Metagenome\Metagenome\Kmers\KmerBloomFilter.vb"

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

'   Total Lines: 249
'    Code Lines: 169 (67.87%)
' Comment Lines: 33 (13.25%)
'    - Xml Docs: 93.94%
' 
'   Blank Lines: 47 (18.88%)
'     File Size: 9.96 KB


'     Class KmerBloomFilter
' 
'         Properties: k, ncbi_taxid
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: (+2 Overloads) Create, KmerHitNumber, (+2 Overloads) KmerHits, (+2 Overloads) LoadFromFile, ToString
' 
'         Sub: Save
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.IO
Imports SMRUCC.genomics.Assembly.NCBI.Entrez
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Namespace Kmers

    ''' <summary>
    ''' kmer bloom filter of a specific genome
    ''' </summary>
    Public Class KmerBloomFilter

        ReadOnly bloomFilter As BloomFilter

        ''' <summary>
        ''' the genome name(multiple chromosome name)
        ''' </summary>
        ReadOnly names As String()

        ''' <summary>
        ''' the length of the k-mer
        ''' </summary>
        Public ReadOnly Property k As Integer
        ''' <summary>
        ''' the genome taxonomy id
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ncbi_taxid As Integer

        Const magicNum As String = "kmer-bloom"

        Sub New(k As Integer, name As IEnumerable(Of String), ncbi_taxid As Integer, bloomFilter As BloomFilter)
            Me.k = k
            Me.names = name.ToArray
            Me.ncbi_taxid = ncbi_taxid
            Me.bloomFilter = bloomFilter
        End Sub

        Public Overrides Function ToString() As String
            Return $"ncbi_taxid: {ncbi_taxid}; " & names(0)
        End Function

        Public Function KmerHits(seq As ISequenceProvider) As Dictionary(Of String, Integer)
            Return KmerHits(KSeq.KmerSpans(seq.GetSequenceData, k))
        End Function

        Public Function KmerHitNumber(kmers As IEnumerable(Of String)) As Integer
            Dim hits As Integer = 0

            For Each kmer As String In kmers
                If bloomFilter(kmer) Then
                    hits += 1
                End If
            Next

            Return hits
        End Function

        Public Function KmerHits(kmers As IEnumerable(Of String)) As Dictionary(Of String, Integer)
            Dim hits As New Dictionary(Of String, Integer)

            For Each kmer As String In kmers
                If bloomFilter(kmer) Then
                    If Not hits.ContainsKey(kmer) Then
                        hits.Add(kmer, 1)
                    Else
                        hits(kmer) += 1
                    End If
                End If
            Next

            Return hits
        End Function

        Public Shared Function LoadFromFile(filepath As String) As KmerBloomFilter
            Using s As Stream = filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return LoadFromFile(s)
            End Using
        End Function

        Public Shared Function LoadFromFile(file As Stream) As KmerBloomFilter
            Dim bin As New BinaryDataReader(file, leaveOpen:=True) With {.ByteOrder = ByteOrder.LittleEndian}
            Dim magic As String = Encoding.ASCII.GetString(bin.ReadBytes(magicNum.Length))

            If magic <> magicNum Then
                Throw New InvalidDataException("invalid magic number for the k-mer bloom filter model file!")
            End If

            Dim k As Integer = bin.ReadInt32
            Dim taxid As Integer = bin.ReadInt32
            Dim nameNum As Integer = bin.ReadInt32
            Dim names As New List(Of String)

            For i As Integer = 0 To nameNum - 1
                Call names.Add(bin.ReadString)
            Next

            Dim pk As Integer = bin.ReadInt32 ' k
            Dim m As Integer = bin.ReadInt32 ' m
            Dim compress As Boolean = bin.ReadByte <> 0
            Dim byteSize As Integer = bin.ReadInt32
            Dim bytes As Byte() = bin.ReadBytes(byteSize)

            If compress Then
#If NET48 Then
                Throw New NotSupportedException("decompression of brotli stream is not supported in .net 4.8 runtime!")
#Else
                Using compressedStream As New MemoryStream(bytes)
                    Using brotliStream As New BrotliStream(compressedStream, CompressionMode.Decompress)
                        Using resultStream As New MemoryStream()
                            brotliStream.CopyTo(resultStream)
                            bytes = resultStream.ToArray() ' 就是解压后的原始数据
                        End Using
                    End Using
                End Using
#End If
            End If

            Dim bloom As New BloomFilter(bytes, m, pk)

            Return New KmerBloomFilter(k, names, taxid, bloom)
        End Function

        Public Sub Save(file As Stream)
            Dim bin As New BinaryDataWriter(file, leaveOpen:=True) With {.ByteOrder = ByteOrder.LittleEndian}

            Call bin.Write(Encoding.ASCII.GetBytes(magicNum))
            Call bin.Write(k)
            Call bin.Write(ncbi_taxid)
            Call bin.Write(names.Length)

            For Each name As String In names
                Call bin.Write(name)
            Next

            Dim bits As Byte() = bloomFilter.ToArray
            Dim compress As Byte = 0

#If NETCOREAPP Then
            If bits.Length > ByteSize.KB Then
                compress = 1

                Using originalStream As New MemoryStream(bits)
                    Using compressedStream As New MemoryStream()
                        Using brotliStream As New BrotliStream(compressedStream, CompressionLevel.Optimal)
                            originalStream.CopyTo(brotliStream)
                        End Using

                        bits = compressedStream.ToArray()
                    End Using
                End Using
            End If
#End If

            Call bin.Write(bloomFilter.k)
            Call bin.Write(bloomFilter.m)
            Call bin.Write(compress)
            Call bin.Write(bits.Length)
            Call bin.Write(bits)
            Call bin.Flush()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="Fasta"></typeparam>
        ''' <param name="genomics">the genomics sequence data of a organism, single chromosome sequence data</param>
        ''' <param name="ncbi_taxid"></param>
        ''' <param name="k"></param>
        ''' <param name="desiredFPR"></param>
        ''' <param name="spanSize"></param>
        ''' <returns></returns>
        Public Shared Function Create(Of Fasta As IFastaProvider)(genomics As Fasta,
                                                                  ncbi_taxid As Integer,
                                                                  Optional k As Integer = 35,
                                                                  Optional desiredFPR As Double = 0.00001,
                                                                  Optional spanSize As Integer = 50 * ByteSize.MB) As KmerBloomFilter

            Dim estimatedKmers As Integer = Math.Max(0, Math.Min(spanSize, genomics.length - k + 1))
            Dim filter As BloomFilter = BloomFilter.Create(estimatedKmers, desiredFPR)
            Dim ntseq As String = genomics.GetSequenceData

            For i As Integer = 0 To ntseq.Length Step spanSize
                Dim len As Integer = spanSize

                If i + len > ntseq.Length Then
                    len = ntseq.Length - i
                End If

                For Each kmer As String In KSeq.KmerSpans(ntseq.Substring(i, len), k)
                    Call filter.Add(kmer)
                Next
            Next

            Return New KmerBloomFilter(k, {genomics.title}, ncbi_taxid, filter)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="Fasta"></typeparam>
        ''' <param name="genomics">the genomics sequence data of a organism, multiple chromosome sequence data</param>
        ''' <param name="ncbi_taxid"></param>
        ''' <param name="k"></param>
        ''' <param name="desiredFPR"></param>
        ''' <param name="spanSize"></param>
        ''' <returns></returns>
        Public Shared Function Create(Of Fasta As IFastaProvider)(genomics As IEnumerable(Of Fasta),
                                                                  ncbi_taxid As Integer,
                                                                  Optional k As Integer = 35,
                                                                  Optional desiredFPR As Double = 0.00001,
                                                                  Optional spanSize As Integer = 50 * ByteSize.MB) As KmerBloomFilter
            Dim pool As Fasta() = genomics.ToArray
            Dim estimatedKmers As Integer = Math.Max(0, Math.Min(spanSize, pool.Max(Function(s) s.length)) - k + 1)
            Dim filter As BloomFilter = BloomFilter.Create(estimatedKmers, desiredFPR)
            Dim names As New List(Of String)
            Dim bar As ProgressBar = Nothing

            For Each nt As Fasta In TqdmWrapper.Wrap(pool, bar:=bar)
                Dim ntseq As String = nt.GetSequenceData

                Call names.Add(nt.title)
                Call bar.SetLabel(nt.title)

                For i As Integer = 0 To ntseq.Length Step spanSize
                    Dim len As Integer = spanSize

                    If i + len > ntseq.Length Then
                        len = ntseq.Length - i
                    End If

                    For Each kmer As String In KSeq.KmerSpans(ntseq.Substring(i, len), k)
                        Call filter.Add(kmer)
                    Next
                Next
            Next

            Return New KmerBloomFilter(k, names, ncbi_taxid, filter)
        End Function

    End Class

End Namespace

