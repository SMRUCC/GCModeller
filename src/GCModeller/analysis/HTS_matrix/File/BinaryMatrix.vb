#Region "Microsoft.VisualBasic::3a8eec8510b2f14b27fdb26df5a8c531, GCModeller\analysis\HTS_matrix\File\BinaryMatrix.vb"

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

'   Total Lines: 157
'    Code Lines: 112
' Comment Lines: 14
'   Blank Lines: 31
'     File Size: 5.07 KB


' Module BinaryMatrix
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: LoadStream, networkByteOrderDecoder, networkByteOrderEncoder, (+2 Overloads) readMatrix, Save
' 
'     Sub: save
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' data encoded in network byte order
''' </summary>
Public Module BinaryMatrix

    ReadOnly magic As Byte()
    ReadOnly bin As New NetworkByteOrderBuffer

    Sub New()
        magic = Encoding.ASCII.GetBytes("GCModeller/HTS_matrix")
    End Sub

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
            exp = bin.decode(bytes)

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
            Call file.Write(bin.encode(feature.experiments))
        Next
    End Sub

End Module
