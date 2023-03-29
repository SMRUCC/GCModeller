#Region "Microsoft.VisualBasic::87232e3d7a33fbad4ee37d9036c4d90b, Bio.Repository\KEGG\MessagePack\KEGGMapPack.vb"

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

'     Class KEGGMapPack
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetMapSchema, GetObjectSchema, GetShapeSchema, ReadKeggDb, WriteKeggDb
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace KEGG.Metabolism

    Public Class KEGGMapPack : Inherits SchemaProvider(Of Map)

        Shared Sub New()
            Call MsgPackSerializer.DefaultContext.RegisterSerializer(New KEGGMapPack)
        End Sub

        Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
            Yield (GetType(Map), GetMapSchema)
            Yield (GetType(Area), GetShapeSchema)
        End Function

        Private Shared Function GetMapSchema() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(Map.id), NilImplication.MemberDefault},
                {NameOf(Map.Name), NilImplication.MemberDefault},
                {NameOf(Map.URL), NilImplication.MemberDefault},
                {NameOf(Map.description), NilImplication.MemberDefault},
                {NameOf(Map.PathwayImage), NilImplication.MemberDefault},
                {NameOf(Map.shapes), NilImplication.MemberDefault}
            }
        End Function

        Private Shared Function GetShapeSchema() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(Area.class), NilImplication.MemberDefault},
                {NameOf(Area.coords), NilImplication.MemberDefault},
                {NameOf(Area.data_coords), NilImplication.MemberDefault},
                {NameOf(Area.data_id), NilImplication.MemberDefault},
                {NameOf(Area.entry), NilImplication.MemberDefault},
                {NameOf(Area.href), NilImplication.MemberDefault},
                {NameOf(Area.moduleId), NilImplication.MemberDefault},
                {NameOf(Area.refid), NilImplication.MemberDefault},
                {NameOf(Area.shape), NilImplication.MemberDefault},
                {NameOf(Area.title), NilImplication.MemberDefault}
            }
        End Function

        ''' <summary>
        ''' load binary data repository with format auto checked
        ''' </summary>
        ''' <param name="file">
        ''' a stream data of kegg pathway maps in messagepack or HDS stream pack format
        ''' data format will be check via the magic number
        ''' </param>
        ''' <returns></returns>
        Public Shared Function ReadKeggDb(file As Stream) As Map()
            If file.CanSeek Then
                Dim scan0 As Long = file.Position
                Dim magic_hds As Byte() = New Byte(3 - 1) {}

                Call file.Read(magic_hds, scan0, magic_hds.Length)
                Call file.Seek(scan0, SeekOrigin.Begin)

                If StreamPack.TestMagic(magic_hds) Then
                    ' is in HDS stream pack format
                    Dim pack As New StreamPack(file)
                    Dim KEGG_maps As StreamGroup = pack.GetObject("/KEGG_maps/")
                    ' fetch all xml files
                    Dim xmlfiles = KEGG_maps.ListFiles _
                        .Where(Function(f) TypeOf f Is StreamBlock) _
                        .Select(Function(f) DirectCast(f, StreamBlock)) _
                        .Where(Function(f) f.referencePath.ExtensionSuffix("xml")) _
                        .ToArray
                    Dim maps As Map() = xmlfiles _
                        .Select(Function(f) pack.ReadText(f).LoadXml(Of Map)) _
                        .ToArray

                    Return maps
                Else
                    ' is in messagepack format
                    Return MsgPackSerializer.Deserialize(Of Map())(file)
                End If
            Else
                ' messagepack format parser by default
                Return MsgPackSerializer.Deserialize(Of Map())(file)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="maps"></param>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' data will be auto flush to <paramref name="file"/>.
        ''' </remarks>
        Public Shared Function WriteKeggDb(maps As IEnumerable(Of Map), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(maps.ToArray, file)
                Call file.Flush()
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace
