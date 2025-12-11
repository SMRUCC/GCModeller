Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.IO

Namespace Kmers

    Public Class KmerBloomFilter

        ReadOnly bloomFilter As BloomFilter
        ReadOnly k As Integer
        ''' <summary>
        ''' the genome name
        ''' </summary>
        ReadOnly name As String
        ReadOnly ncbi_taxid As Integer

        Const magicNum As String = "kmer-bloom"

        Public Shared Function LoadFromFile(file As Stream) As KmerBloomFilter
            Dim bin As New BinaryDataReader(file, leaveOpen:=True) With {.ByteOrder = ByteOrder.LittleEndian}

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
