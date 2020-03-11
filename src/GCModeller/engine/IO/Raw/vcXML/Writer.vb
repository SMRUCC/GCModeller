Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.IO
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

        Friend Sub New(file As String, Optional networkByteOrder As Boolean = True)
            fs = file.OpenWriter(Encodings.UTF8WithoutBOM)
            doReverse = networkByteOrder AndAlso BitConverter.IsLittleEndian
            emptyNamespace.Add(String.Empty, String.Empty)
        End Sub

        Friend Sub writeInit(entities As VcellAdapterDriver)
            fs.WriteLine("<?xml version=""1.0"" encoding=""utf8""?>")
            fs.WriteLine("<vcXML xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
xmlns:GCModeller=""https://gcmodeller.org"" 
xmlns=""https://bioCAD.gcmodeller.org/XML/schema_revision/vcellXML_1.10.33"">")

            fs.WriteLine($"<run time=""{Now.ToString}"" software=""GCModeller"" />")
            fs.WriteLine("<vcRun>")
            fs.WriteLine("<dataEntity>")

            Call writeHeader(NameOf(entities.mass.transcriptome), "mass_profile", entities.mass.transcriptome)
            Call writeHeader(NameOf(entities.mass.proteome), "mass_profile", entities.mass.proteome)
            Call writeHeader(NameOf(entities.mass.metabolome), "mass_profile", entities.mass.metabolome)

            Call writeHeader(NameOf(entities.flux.transcriptome), "activity", entities.flux.transcriptome)
            Call writeHeader(NameOf(entities.flux.proteome), "activity", entities.flux.proteome)
            Call writeHeader(NameOf(entities.flux.metabolome), "flux_size", entities.flux.metabolome)

            Call fs.WriteLine("</dataEntity>")

            Call fs.WriteLine("<raw>")
            Call fs.Flush()
        End Sub

        Private Sub writeHeader(module$, type$, list As String())
            Dim ms As New MemoryStream

            Using write As New BinaryDataWriter(ms)
                For Each id As String In list
                    Call write.Write(id, BinaryStringFormat.ByteLengthPrefix)
                Next

                write.Flush()
            End Using

            Dim buffer As Byte() = ms.ToArray

            If doReverse Then
                Array.Reverse(buffer)
            End If

            ms = New MemoryStream(buffer).GZipStream

            Dim objects As New omicsDataEntities With {
                .entities = ms.ToArray.ToBase64String,
                .[module] = [module],
                .content_type = type
            }

            Dim serializer As New XmlSerializer(GetType(omicsDataEntities))
            Dim output As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(output, New XmlWriterSettings With {.OmitXmlDeclaration = True, .Indent = True})

            serializer.Serialize(writer, objects, emptyNamespace)

            Call fs.WriteLine(output.ToString)
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

            index += New offset With {
                .id = frame.num,
                .offset = fs.BaseStream.Position,
                .content_type = type,
                .[module] = [module]
            }
            md5 += frame.vector.data.MD5

            Dim output As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(output, New XmlWriterSettings With {.OmitXmlDeclaration = True, .Indent = True, .NewLineOnAttributes = True})

            serializer.Serialize(writer, frame, emptyNamespace)

            fs.WriteLine(output.ToString)
            fs.Flush()
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
            Dim index As New index With {
                .name = "frame",
                .offsets = Me.index,
                .size = .offsets.Length
            }

            fs.WriteLine("</raw>")
            fs.WriteLine("</vcRun>")
            fs.Flush()

            Dim indexoffset As Long = fs.BaseStream.Position
            Dim serializer As New XmlSerializer(GetType(index))
            Dim output As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(output, New XmlWriterSettings With {.OmitXmlDeclaration = True, .Indent = True})

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