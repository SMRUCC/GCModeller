#Region "Microsoft.VisualBasic::500384d078aa4c830aaaec68b7068a9f, GCModeller\engine\IO\Raw\vcXML\Writer.vb"

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

    '   Total Lines: 231
    '    Code Lines: 173
    ' Comment Lines: 14
    '   Blank Lines: 44
    '     File Size: 8.75 KB


    '     Class Writer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: encode
    ' 
    '         Sub: addFrame, (+2 Overloads) Dispose, writeArguments, writeHeader, writeIndex
    '              writeInit
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML.XML
Imports XmlOffset = SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML.XML.offset

Namespace vcXML

    Public Class Writer : Implements IDisposable

        Dim frameCount As i32 = 1
        Dim fs As StreamWriter
        Dim doReverse As Boolean = False
        Dim index As New List(Of XmlOffset)
        Dim entityIndex As New List(Of XmlOffset)
        Dim serializer As New XmlSerializer(GetType(frame))
        Dim md5 As New List(Of String)
        Dim xmlConfig As XmlWriterSettings

        Friend Sub New(file As String, xmlSettings As XmlWriterSettings, Optional networkByteOrder As Boolean = True)
            fs = file.OpenWriter(Encodings.UTF8WithoutBOM)
            doReverse = networkByteOrder AndAlso BitConverter.IsLittleEndian
            xmlConfig = xmlSettings
        End Sub

        Friend Sub writeInit(entities As VcellAdapterDriver, args As FluxBaseline)
            fs.WriteLine("<?xml version=""1.0"" encoding=""utf8""?>")
            fs.WriteLine("<vcXML xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
xmlns:GCModeller=""https://gcmodeller.org"" 
xmlns=""https://bioCAD.gcmodeller.org/XML/schema_revision/vcellXML_1.10.33"">")

            fs.WriteLine($"<run time=""{Now.ToString}"" software=""GCModeller"" />")
            fs.WriteLine("<vcRun>")

            Call writeArguments(args)

            fs.WriteLine("<dataEntity>")
            fs.Flush()

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

        Private Sub writeArguments(args As FluxBaseline)
            Dim params As New parameters With {
                .args = DataFramework.Schema(Of FluxBaseline)(PropertyAccess.Readable, True, True) _
                    .Select(Function(p)
                                Return New NamedValue With {
                                    .name = p.Key,
                                    .text = p.Value.GetValue(args)
                                }
                            End Function) _
                    .ToArray
            }

            Call fs.WriteLine(XmlHelper.getXmlFragment(params, xmlConfig))
        End Sub

        Private Sub writeHeader(module$, type$, list As String())
            Dim ms As New MemoryStream

            Using write As New BinaryDataWriter(ms)
                For Each id As String In list
                    Call write.Write(id, BinaryStringFormat.ZeroTerminated)
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
            Dim offset As New XmlOffset With {
                .id = entityIndex.Count + 1,
                .offset = fs.BaseStream.Position,
                .content_type = type,
                .[module] = [module]
            }

            Call entityIndex.Add(offset)
            Call fs.WriteLine(XmlHelper.getXmlFragment(objects, xmlConfig))
            Call fs.Flush()
        End Sub

        Public Sub addFrame(time As Double, module$, type$, data As IEnumerable(Of Double))
            Dim vecBase64$ = encode(data)
            Dim frame As New frame With {
                .frameTime = time,
                .tick = time,
                .[module] = [module],
                .num = ++frameCount,
                .vector = New vector With {
                    .data = vecBase64,
                    .contentType = type
                }
            }

            index += New XmlOffset With {
                .id = frame.num,
                .offset = fs.BaseStream.Position,
                .content_type = type,
                .[module] = [module],
                .tick = time
            }
            md5 += frame.vector.data.MD5

            Dim output As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(output, xmlConfig)

            serializer.Serialize(writer, frame, emptyNamespace)

            fs.WriteLine(output.ToString)
            fs.Flush()
        End Sub

        Private Function encode(data As IEnumerable(Of Double)) As String
            Dim ms As New MemoryStream
            Dim vec As Double() = data.ToArray

            Using write As New BinaryWriter(ms)
                For Each d As Double In vec
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
            Dim entityIndex As New index With {
                .name = "entity",
                .offsets = Me.entityIndex,
                .size = .offsets.Length
            }

            fs.WriteLine("</raw>")
            fs.WriteLine("</vcRun>")
            fs.Flush()

            Dim indexoffset As Long = fs.BaseStream.Position

            fs.WriteLine(XmlHelper.getXmlFragment(index, xmlConfig))
            fs.WriteLine(XmlHelper.getXmlFragment(entityIndex, xmlConfig))
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
