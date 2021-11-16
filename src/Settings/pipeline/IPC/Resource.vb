#Region "Microsoft.VisualBasic::4e91e0e49a6b98ea3f960fabfed64282, pipeline\IPC\Resource.vb"

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

    ' Class Resource
    ' 
    '     Properties: contentType, fullName, size
    ' 
    '     Function: GetCollectionOf, GetDataOffset, GetMetaInfo, (+2 Overloads) GetObject
    ' 
    '     Sub: Write
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports CliPipeline = Microsoft.VisualBasic.CommandLine

Public Class Resource

    ''' <summary>
    ''' The memory mapping file name
    ''' </summary>
    ''' <returns></returns>
    Public Property fullName As String
    ''' <summary>
    ''' The type information of target data resource content.
    ''' </summary>
    ''' <returns></returns>
    Public Property contentType As TypeInfo
    ''' <summary>
    ''' The data size of the content resource.
    ''' </summary>
    ''' <returns></returns>
    Public Property size As Long

    Public Shared Function GetDataOffset(resourceName As String) As Long
        Using reader As New BinaryDataReader(CliPipeline.OpenForRead(resourceName))
            reader.Seek(4, SeekOrigin.Begin)
            Return reader.ReadInt64
        End Using
    End Function

    ' memorty layout
    '
    '  int32     long       byte()      byte()
    ' meta_size|data_pos|metainfo_bson|datablock

    ''' <summary>
    ''' 读取头部meta信息, 并不读取数据
    ''' </summary>
    ''' <param name="resourceName"></param>
    ''' <returns></returns>
    Public Shared Function GetMetaInfo(resourceName As String) As Resource
        Dim headerSize%
        Dim bson As JsonObject

        Using reader As New BinaryDataReader(CliPipeline.OpenForRead(resourceName))
            reader.Seek(Scan0, SeekOrigin.Begin)
            headerSize = reader.ReadInt32
            reader.Seek(4, SeekOrigin.Begin)
            bson = BSONFormat.Load(reader.ReadBytes(headerSize))

            Return bson.CreateObject(Of Resource)
        End Using
    End Function

    Public Sub Write()

    End Sub

    Public Function GetObject() As Object
        ' 获取得到类型信息, 然后做二进制反序列化?
        Dim type As Type = contentType.GetType(True)

        Using reader As New BinaryDataReader(CliPipeline.OpenForRead(fullName))

        End Using

        Throw New NotImplementedException
    End Function

    Public Function GetObject(begin&, size&) As Object
        Throw New NotImplementedException
    End Function

    Public Iterator Function GetCollectionOf() As IEnumerable(Of Object)

    End Function
End Class
