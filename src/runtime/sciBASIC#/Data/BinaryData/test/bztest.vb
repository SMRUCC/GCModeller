Imports System
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Bzip2
    ''' <summary>
    ''' Unit test for the BZip2 compression library
    ''' </summary>
    <TestClass>
    Public Class UnitTests
        Private Const BufferSizeLarge As Integer = 10000000 ' Almost 10 Mb
        Private Const BufferSizeSmall As Integer = 100000   ' Around 100 Kb
        Private Shared Buffer As Byte() = New Byte(9999999) {}

        ''' <summary>
        ''' Fills the test buffer with random values
        ''' </summary>
        <TestInitialize>
        Public Sub InitializeBuffer()
            Dim random = New Random()
            random.NextBytes(Buffer)
        End Sub

        ''' <summary>
        ''' Performs a CRC check and compare against well-known results
        ''' The buffer has different values
        ''' </summary>
        <TestMethod>
        Public Sub CrcAlgorithmDifferentValues()
            Dim buffer As Byte() = {&H01, &H02, &H03, &H04, &H05, &H06, &H07, &H08, &H09, &H0A, &HF1, &HF2, &HF3, &HF4, &HF5, &HF6, &HF7, &HF8, &HF9, &HFA}
            Dim crc = New CRC32()

            For i = 0 To buffer.Length - 1
                crc.UpdateCrc(buffer(i))
            Next

            Assert.AreEqual(crc.CRC, &H8AEE127A)
        End Sub

        ''' <summary>
        ''' Performs a CRC check and compare against well-known results
        ''' The buffer has different values
        ''' </summary>
        <TestMethod>
        Public Sub CrcAlgorithmSameValues()
            Dim crc = New CRC32()
            crc.UpdateCrc(&H55, 10)
            Assert.AreEqual(crc.CRC, &HA1E07747)
        End Sub

        ''' <summary>
        ''' Compresses the full buffer and checks for a reasonable compressed size
        ''' </summary>
        <TestMethod>
        Public Sub CompressSmokeLarge()
            Dim input = New MemoryStream(Buffer)
            Dim output = New MemoryStream()
            Dim compressor = New BZip2OutputStream(output, False)
            input.CopyTo(compressor)
            compressor.Close()

            ' Estimated size between inputSize*0.5 and inputSize*1.1
            Assert.IsTrue(output.Length > BufferSizeLarge * 0.5)
            Assert.IsTrue(output.Length < BufferSizeLarge * 1.1)
        End Sub

        ''' <summary>
        ''' Compresses a portion of the buffer and checks for a reasonable compressed size
        ''' </summary>
        <TestMethod>
        Public Sub CompressSmokeSmall()
            Dim input = New MemoryStream(Buffer, 0, BufferSizeSmall)
            Dim output = New MemoryStream()
            Dim compressor = New BZip2OutputStream(output, False)
            input.CopyTo(compressor)
            compressor.Close()

            ' Estimated size between inputSize*0.5 and inputSize*1.1
            Assert.IsTrue(output.Length > BufferSizeSmall * 0.5)
            Assert.IsTrue(output.Length < BufferSizeSmall * 1.1)
        End Sub

        ''' <summary>
        ''' Compresses and decompresses a long random buffer
        ''' </summary>
        <TestMethod>
        Public Sub CompressAndDecompress()
            Dim input = New MemoryStream(Buffer)
            Dim output = New MemoryStream()
            Dim compressor = New BZip2OutputStream(output, False)
            input.CopyTo(compressor)
            compressor.Close()
            Assert.IsTrue(output.Length > 4)
            output.Position = 0
            Dim output2 = New MemoryStream()
            Dim decompressor = New BZip2InputStream(output, False)
            decompressor.CopyTo(output2)
            Assert.AreEqual(Buffer.Length, output2.Length)
        End Sub
    End Class
End Namespace
