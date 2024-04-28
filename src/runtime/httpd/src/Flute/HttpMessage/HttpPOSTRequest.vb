#Region "Microsoft.VisualBasic::f503f1056c744430d154e228dab1ef95, G:/GCModeller/src/runtime/httpd/src/Flute//HttpMessage/HttpPOSTRequest.vb"

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

    '   Total Lines: 68
    '    Code Lines: 54
    ' Comment Lines: 5
    '   Blank Lines: 9
    '     File Size: 2.65 KB


    '     Class HttpPOSTRequest
    ' 
    '         Properties: POSTData
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetBoolean, HasValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Flute.Http.Core.HttpStream
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.Default

Namespace Core.Message

    Public Class HttpPOSTRequest : Inherits HttpRequest

        Public ReadOnly Property POSTData As PostReader

        Default Public Overrides ReadOnly Property Argument(name As String) As DefaultString
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If URL.query.ContainsKey(name) Then
                    Return New DefaultString(URL.query(name).ElementAtOrNull(Scan0))
                Else
                    Return New DefaultString(POSTData.Form(name))
                End If
            End Get
        End Property

        Shared ReadOnly uploadfile As [Default](Of String) = NameOf(uploadfile)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="request"></param>
        ''' <param name="inputData">一个临时文件的文件路径,POST上传的原始数据都被保存在这个临时文件中</param>
        Sub New(request As HttpProcessor, inputData$, Optional parseJSON As PostReader.JSONParser = Nothing)
            Call MyBase.New(request)

            If inputData.FileLength > 0 AndAlso HttpHeaders.ContainsKey(HttpHeader.RequestHeaders.ContentType) Then
                POSTData = New PostReader(
                    inputData,
                    HttpHeaders(HttpHeader.RequestHeaders.ContentType),
                    Encoding.UTF8,
                    HttpHeaders.TryGetValue("fileName") Or uploadfile,
                    parseJSON:=parseJSON
                )
            Else
                POSTData = New PostReader(
                    input:=inputData,
                    contentType:="application/octet-stream",
                    encoding:=Encoding.ASCII,
                    fileName:=HttpHeaders.TryGetValue("fileName") Or uploadfile
                )
            End If
        End Sub

        Public Overrides Function GetBoolean(name As String) As Boolean
            If HasValue(name) Then
                Return Argument(name).DefaultValue.ParseBoolean
            Else
                Return False
            End If
        End Function

        Public Overrides Function HasValue(name As String) As Boolean
            If Not URL.query.ContainsKey(name) Then
                Return POSTData.Form.ContainsKey(name)
            Else
                Return True
            End If
        End Function
    End Class
End Namespace
