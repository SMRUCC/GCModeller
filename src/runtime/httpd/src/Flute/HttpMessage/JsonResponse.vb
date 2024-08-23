﻿#Region "Microsoft.VisualBasic::d3cda7cd70096b9ca5899b3fd4b4e20c, G:/GCModeller/src/runtime/httpd/src/Flute//HttpMessage/JsonResponse.vb"

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

    '   Total Lines: 17
    '    Code Lines: 13
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 406 B


    '     Class JsonResponse
    ' 
    '         Properties: code, info
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace AppEngine

    ''' <summary>
    ''' a json wrapper of the response message:  ``{code: int, info: <typeparamref name="T"/>}``
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class JsonResponse(Of T)

        ''' <summary>
        ''' the status code of the result response
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property code As Integer

        ''' <summary>
        ''' the response content data
        ''' </summary>
        ''' <returns></returns>
        <XmlText>
        Public Property info As T

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
