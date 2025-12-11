Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.IO

Namespace Kmers

    Public Class KmerBloomFilter

        ReadOnly bloomFilter As BloomFilter
        ''' <summary>
        ''' the length of the k-mer
        ''' </summary>
        ReadOnly k As Integer
        ''' <summary>
        ''' the genome name
        ''' </summary>
        ReadOnly name As String
        ReadOnly ncbi_taxid As Integer

        Const magicNum As String = "kmer-bloom"

        Sub New(k As Integer, name As String, ncbi_taxid As Integer, bloomFilter As BloomFilter)
            Me.k = k
            Me.name = name
            Me.ncbi_taxid = ncbi_taxid
            Me.bloomFilter = bloomFilter
        End Sub

        Public Shared Function LoadFromFile(file As Stream) As KmerBloomFilter
            Dim bin As New BinaryDataReader(file, leaveOpen:=True) With {.ByteOrder = ByteOrder.LittleEndian}
            Dim magic As String = Encoding.ASCII.GetString(bin.ReadBytes(magicNum.Length))

            If magic <> magicNum Then
                Throw New InvalidDataException("invalid magic number for the k-mer bloom filter model file!")
            End If

            Dim k As Integer = bin.ReadInt32
            Dim taxid As Integer = bin.ReadInt32
            Dim name As String = bin.ReadString
            Dim pk As Integer = bin.ReadInt32
            Dim bytes As Byte() = bin.ReadBytes(bin.ReadInt32 \ 8)
            Dim bloom As New BloomFilter(bytes, pk)

            Return New KmerBloomFilter(k, name, taxid, bloom)
        End Function

        Public Sub Save(file As Stream)
            Dim bin As New BinaryDataWriter(file, leaveOpen:=True) With {.ByteOrder = ByteOrder.LittleEndian}

            Call bin.Write(Encoding.ASCII.GetBytes(magicNum))
            Call bin.Write(k)
            Call bin.Write(ncbi_taxid)
            Call bin.Write(name)
            Call bin.Write(bloomFilter.k)
            Call bin.Write(bloomFilter.m)
            Call bin.Write(bloomFilter.ToArray)
            Call bin.Flush()
        End Sub

    End Class

End Namespace
