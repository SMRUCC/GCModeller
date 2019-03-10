﻿#Region "Microsoft.VisualBasic::0ce3e330b16e00e1219e3134e4695aa2, WebCloud\SMRUCC.HTTPInternal\AppEngine\API\args\HttpRequest.vb"

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

    '     Class HttpRequest
    ' 
    '         Properties: HttpHeaders, HTTPMethod, IsWWWRoot, Remote, URL
    '                     URLParameters, version
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class HttpPOSTRequest
    ' 
    '         Properties: POSTData
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.POSTParser
Imports SMRUCC.WebCloud.HTTPInternal.Core
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Namespace AppEngine.APIMethods.Arguments

    ''' <summary>
    ''' Data of the http request
    ''' </summary>
    Public Class HttpRequest

        ''' <summary>
        ''' GET/POST/PUT/DELETE....
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HTTPMethod As String
        Public ReadOnly Property URL As String
        ''' <summary>
        ''' <see cref="HttpProcessor.http_protocol_versionstring"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property version As String
        Public ReadOnly Property HttpHeaders As Dictionary(Of String, String)

        ''' <summary>
        ''' Remote client ip address
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Remote As String

        ''' <summary>
        ''' If current request url is indicates the HTTP root:  index.html
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsWWWRoot As Boolean
            Get
                Return String.Equals("/", URL)
            End Get
        End Property

        Default Public Overridable ReadOnly Property Argument(name As String) As DefaultString
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New DefaultString(URLParameters(name))
            End Get
        End Property

        Public Property URLParameters As NameValueCollection

        Sub New(request As HttpProcessor)
            HTTPMethod = request.http_method
            URL = request.http_url
            version = request.http_protocol_versionstring
            HttpHeaders = request.httpHeaders
            Remote = request.socket.Client.RemoteEndPoint.ToString.Split(":"c).First
        End Sub

        Sub New()
        End Sub

        ''' <summary>
        ''' Debug use
        ''' </summary>
        ''' <param name="args"></param>
        Friend Sub New(args As IEnumerable(Of NamedValue(Of String)))
            HTTPMethod = "GET"
            URL = "memory://debug"
            version = "2.1"
            HttpHeaders = New Dictionary(Of String, String)
            Remote = "127.0.0.1"
            URLParameters = args.NameValueCollection
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class HttpPOSTRequest : Inherits HttpRequest

        Public ReadOnly Property POSTData As PostReader

        Default Public Overrides ReadOnly Property Argument(name As String) As DefaultString
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If URLParameters.ContainsKey(name) Then
                    Return New DefaultString(URLParameters(name))
                Else
                    Return New DefaultString(POSTData.Form(name))
                End If
            End Get
        End Property

        Shared ReadOnly uploadfile As DefaultValue(Of String) = NameOf(uploadfile)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="request"></param>
        ''' <param name="inputData">一个临时文件的文件路径,POST上传的原始数据都被保存在这个临时文件中</param>
        Sub New(request As HttpProcessor, inputData$)
            Call MyBase.New(request)

            POSTData = New PostReader(
                inputData,
                HttpHeaders(PlatformEngine.contentType),
                Encoding.UTF8,
                HttpHeaders.TryGetValue("fileName") Or uploadfile
            )
        End Sub
    End Class
End Namespace
