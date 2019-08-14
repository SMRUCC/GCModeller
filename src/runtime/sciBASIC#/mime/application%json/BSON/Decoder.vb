Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Namespace BSON

    Public Class Decoder

        Dim mMemoryStream As MemoryStream
        Dim mBinaryReader As BinaryReader

        Friend Sub New(buf As Byte())
            mMemoryStream = New MemoryStream(buf)
            mBinaryReader = New BinaryReader(mMemoryStream)
        End Sub

        Public Function decodeDocument() As JsonObject
            Dim length As Integer = mBinaryReader.ReadInt32() - 4
            Dim obj As New JsonObject()
            Dim i As Integer = CInt(mBinaryReader.BaseStream.Position)

            While mBinaryReader.BaseStream.Position < i + length - 1
                Dim name As String = Nothing
                Dim value As JsonElement = decodeElement(name)

                obj.Add(name, value)
            End While

            mBinaryReader.ReadByte()
            ' zero
            Return obj
        End Function


        Private Function decodeArray() As JsonArray
            Dim obj As JsonObject = decodeDocument()
            Dim i As VBInteger = 0
            Dim array As New JsonArray()
            Dim key As Value(Of String) = ""

            While obj.ContainsKey(key = Convert.ToString(++i))
                Call array.Add(obj(key))
            End While

            Return array
        End Function

        Private Function decodeString() As String
            Dim length As Integer = mBinaryReader.ReadInt32()
            Dim buf As Byte() = mBinaryReader.ReadBytes(length)

            Return Encoding.UTF8.GetString(buf)
        End Function

        Private Function decodeCString() As String
            Dim ms = New MemoryStream()

            While True
                Dim buf As Byte = CByte(mBinaryReader.ReadByte())
                If buf = 0 Then
                    Exit While
                End If
                ms.WriteByte(buf)
            End While

            Return Encoding.UTF8.GetString(ms.GetBuffer(), 0, CInt(ms.Position))
        End Function


        Private Function decodeElement(ByRef name As String) As JsonElement
            Dim elementType As Byte = mBinaryReader.ReadByte()

            If elementType = &H1 Then
                ' Double
                name = decodeCString()

                Return New JsonValue(New BSONValue(mBinaryReader.ReadDouble()))
            ElseIf elementType = &H2 Then
                ' String
                name = decodeCString()

                Return New JsonValue(New BSONValue(decodeString()))
            ElseIf elementType = &H3 Then
                ' Document
                name = decodeCString()

                Return decodeDocument()
            ElseIf elementType = &H4 Then
                ' Array
                name = decodeCString()

                Return decodeArray()
            ElseIf elementType = &H5 Then
                ' Binary
                name = decodeCString()
                Dim length As Integer = mBinaryReader.ReadInt32()
                Dim binaryType As Byte = mBinaryReader.ReadByte()


                Return New JsonValue(New BSONValue(mBinaryReader.ReadBytes(length)))
            ElseIf elementType = &H8 Then
                ' Boolean
                name = decodeCString()

                Return New JsonValue(New BSONValue(mBinaryReader.ReadBoolean()))
            ElseIf elementType = &H9 Then
                ' DateTime
                name = decodeCString()
                Dim time As Int64 = mBinaryReader.ReadInt64()
                Return New JsonValue(New BSONValue(New DateTime(1970, 1, 1, 0, 0, 0,
                DateTimeKind.Utc) + New TimeSpan(time * 10000)))
            ElseIf elementType = &HA Then
                ' None
                name = decodeCString()
                Return New JsonValue(New BSONValue())
            ElseIf elementType = &H10 Then
                ' Int32
                name = decodeCString()
                Return New JsonValue(New BSONValue(mBinaryReader.ReadInt32()))
            ElseIf elementType = &H12 Then
                ' Int64
                name = decodeCString()
                Return New JsonValue(New BSONValue(mBinaryReader.ReadInt64()))
            End If

            Throw New Exception(String.Format("Don't know elementType={0}", elementType))
        End Function

    End Class
End Namespace