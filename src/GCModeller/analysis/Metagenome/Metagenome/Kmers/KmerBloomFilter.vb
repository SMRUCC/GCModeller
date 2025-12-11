Imports System.IO
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

        Public Shared Function LoadFromFile(file As Stream) As KmerBloomFilter
            Dim bin As New BinaryDataReader(file, leaveOpen:=True) With {.ByteOrder = ByteOrder.LittleEndian}

        End Function

        Public Sub Save(file As Stream)
            Dim bin As New BinaryDataWriter(file, leaveOpen:=True) With {.ByteOrder = ByteOrder.LittleEndian}


        End Sub

    End Class

End Namespace
