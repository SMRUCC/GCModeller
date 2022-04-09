Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' data encoded in network byte order
''' </summary>
Public Module BinaryMatrix

    ReadOnly encode As Func(Of Double(), Byte())
    ReadOnly decode As Func(Of Byte(), Double())
    ReadOnly magic As Byte()

    Sub New()
        magic = Encoding.ASCII.GetBytes("GCModeller/HTS_matrix")

        If BitConverter.IsLittleEndian Then
            ' reverse bytes
            encode = AddressOf networkByteOrderEncoder
            decode = AddressOf networkByteOrderDecoder

            Call Console.WriteLine("system byte order is little endian.")
        Else
            ' no bytes sequence reverse
            encode = Function(nums)
                         Dim bytes As New List(Of Byte)

                         For Each d As Double In nums
                             Call bytes.AddRange(BitConverter.GetBytes(d))
                         Next

                         Return bytes.ToArray
                     End Function
            decode = Function(buffer)
                         Dim nums As Double() = New Double(buffer.Length / 8 - 1) {}

                         For i As Integer = 0 To nums.Length - 1
                             nums(i) = BitConverter.ToDouble(buffer, i * 8)
                         Next

                         Return nums
                     End Function
        End If
    End Sub

    Private Function networkByteOrderDecoder(buffer As Byte()) As Double()
        Dim nums As Double() = New Double(buffer.Length / 8 - 1) {}
        Dim bytes As Byte() = New Byte(8 - 1) {}

        For i As Integer = 0 To nums.Length - 1
            Call Array.ConstrainedCopy(buffer, i * 8, bytes, Scan0, bytes.Length)
            Call Array.Reverse(bytes)

            nums(i) = BitConverter.ToDouble(bytes, Scan0)
        Next

        Return nums
    End Function

    Private Function networkByteOrderEncoder(nums As Double()) As Byte()
        Dim bytes As New List(Of Byte)
        Dim buffer As Byte()

        For Each d As Double In nums
            buffer = BitConverter.GetBytes(d)

            Call Array.Reverse(buffer)
            Call bytes.AddRange(buffer)
        Next

        Return bytes.ToArray
    End Function

    Public Function LoadStream(file As Stream) As Matrix
        Using reader As New BinaryReader(file, Encoding.UTF8)
            Return reader.readMatrix
        End Using
    End Function

    <Extension>
    Private Function readMatrix(file As BinaryReader) As Matrix
        ' read magic
        Dim bytes As Byte() = file.ReadBytes(magic.Length)
        Dim str As String
        Dim mat As New Matrix

        If Not bytes.SequenceEqual(magic) Then
            Throw New InvalidDataException("invalid magic header string!")
        Else
            ' read tag string
            str = file.ReadString
            mat.tag = str
        End If

        ' read nsamples
        Dim nsamples = file.ReadInt32
        Dim mfeatures = file.ReadInt32
        Dim geneIds As Pointer(Of String)

        str = file.ReadString
        mat.sampleID = str.LoadJSON(Of String())
        str = file.ReadString
        geneIds = New Pointer(Of String)(str.LoadJSON(Of String()))
        mat.expression = readMatrix(geneIds, file, nsamples).ToArray

        Return mat
    End Function

    Private Iterator Function readMatrix(geneIds As Pointer(Of String), file As BinaryReader, nsamples As Integer) As IEnumerable(Of DataFrameRow)
        Dim buffer As Stream = file.BaseStream
        Dim bytes As Byte() = New Byte(nsamples * 8 - 1) {}
        Dim exp As Double()

        Do While buffer.Length > buffer.Position
            bytes = file.ReadBytes(bytes.Length)
            exp = decode(bytes)

            Yield New DataFrameRow With {
                .experiments = exp,
                .geneID = ++geneIds
            }
        Loop
    End Function

    <Extension>
    Public Function Save(mat As Matrix, file As Stream) As Boolean
        Using writer As New BinaryWriter(file, Encoding.UTF8)
            Call writer.Write(magic)
            Call mat.save(writer)
            Call writer.Flush()
        End Using

        Return True
    End Function

    <Extension>
    Private Sub save(mat As Matrix, file As BinaryWriter)
        ' write matrix tag
        Call file.Write(If(mat.tag, "NA"))
        ' n samples
        Call file.Write(mat.sampleID.Length)
        ' m features
        Call file.Write(mat.size)
        ' write all sample id
        Call file.Write(mat.sampleID.GetJson)
        ' write all feature id
        Call file.Write(mat.rownames.GetJson)

        ' write each feature row data
        For Each feature As DataFrameRow In mat.expression
            Call file.Write(encode(feature.experiments))
        Next
    End Sub

End Module
