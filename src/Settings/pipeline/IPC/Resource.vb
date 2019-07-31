#Region "Microsoft.VisualBasic::96278f2d282221c1957894405f7f93c9, pipeline\IPC\Resource.vb"

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
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports CliPipeline = Microsoft.VisualBasic.CommandLine

Public Class Resource

    ''' <summary>
    ''' The memory mapping file name
    ''' </summary>
    ''' <returns></returns>
    Public Property fullName As String
    ''' <summary>
    ''' json_base64 of <see cref="TypeInfo"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property contentType As String
    ''' <summary>
    ''' The data size of the content resource.
    ''' </summary>
    ''' <returns></returns>
    Public Property size As Long

    ' memorty layout
    '
    '  long      long     string        byte()
    ' meta_size|data_pos|metainfo_json|datablock

    Public Shared Function GetMetaInfo(resourceName As String) As Resource
        Dim headerSize&
        Dim json$

        Using reader As New BinaryDataReader(CliPipeline.OpenForRead(resourceName))
            reader.Seek(Scan0, SeekOrigin.Begin)
            headerSize = reader.ReadInt64
            reader.Seek(8, SeekOrigin.Begin)
            json = reader.ReadBytes(headerSize).GetString(Encodings.UTF8)

            Return json.LoadJSON(Of Resource)
        End Using
    End Function

    Public Function GetObject() As Object

    End Function

    Public Function GetObject(begin&, size&) As Object

    End Function

    Public Iterator Function GetCollectionOf() As IEnumerable(Of Object)

    End Function
End Class

