Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text

Namespace vcXML

    Public Class Writer : Implements IDisposable

        Dim frameCount As i32 = 1
        Dim fs As StreamWriter
        Dim doReverse As Boolean = False
        Dim index As New List(Of offset)
        Dim serializer As New XmlSerializer(GetType(frame))
        Dim emptyNamespace As New XmlSerializerNamespaces()
        Dim md5 As New List(Of String)

        Sub New(file As String, Optional networkByteOrder As Boolean = True)
            fs = file.OpenWriter(Encodings.UTF8WithoutBOM)
            doReverse = networkByteOrder AndAlso BitConverter.IsLittleEndian
            emptyNamespace.Add(String.Empty, String.Empty)

            Call writeInit()
        End Sub

        Private Sub writeInit()
            fs.WriteLine("<?xml version=""1.0"" encoding=""utf8""?>")
            fs.WriteLine("<vcXML>")
            fs.WriteLine($"<run time=""{Now.ToString}"" software=""GCModeller"" />")
            fs.WriteLine("<vcRun>")
        End Sub

        Public Sub addFrame(time As Double, module$, type$, data As IEnumerable(Of Double))
            Dim vecBase64$ = encode(data)
            Dim frame As New frame With {
                .frameTime = time,
                .[module] = [module],
                .num = ++frameCount,
                .vector = New vector With {
                    .data = vecBase64,
                    .contentType = type
                }
            }

            index += New offset With {.id = frame.num, .offset = fs.BaseStream.Position}
            md5 += frame.vector.data.MD5

            Dim output As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(output, New XmlWriterSettings With {.OmitXmlDeclaration = True})

            serializer.Serialize(writer, frame, emptyNamespace)

            fs.WriteLine(output.ToString)
        End Sub

        Private Function encode(data As IEnumerable(Of Double)) As String
            Dim ms As New MemoryStream

            Using write As New BinaryWriter(ms)
                For Each d As Double In data
                    write.Write(d)
                Next

                write.Flush()
            End Using

            Dim buffer As Byte() = ms.ToArray

            If doReverse Then
                Array.Reverse(buffer)
            End If

            ms = New MemoryStream(buffer).GZipStream

            Dim base64$ = ms.ToArray.ToBase64String
            Return base64
        End Function

        Private Sub writeIndex()
            Dim index As New index With {.name = "frame", .offsets = Me.index}

            fs.WriteLine("</vcRun>")

            Dim indexoffset As Long = fs.BaseStream.Position
            Dim serializer As New XmlSerializer(GetType(index))
            Dim output As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(output, New XmlWriterSettings With {.OmitXmlDeclaration = True})

            serializer.Serialize(writer, index, emptyNamespace)

            fs.WriteLine(output.ToString)
            fs.WriteLine($"<indexOffset>{indexoffset}</indexOffset>")
            fs.WriteLine($"<md5>{md5.JoinBy("+").MD5}</md5>")
            fs.WriteLine("</vcXML>")
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call writeIndex()
                    Call fs.Flush()
                    Call fs.Close()
                    Call fs.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace