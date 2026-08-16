#Region "Microsoft.VisualBasic::e28160b2560a8fdcb3182423b4ad25da, src\Flute\HttpMessage\JsonResponse.vb"

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

    '   Total Lines: 30
    '    Code Lines: 13 (43.33%)
    ' Comment Lines: 12 (40.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (16.67%)
    '     File Size: 838 B


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
        ''' <returns>the integer status/application code.</returns>
        <XmlAttribute>
        Public Property code As Integer

        ''' <summary>
        ''' the response content data
        ''' </summary>
        ''' <returns>the payload carried by this response.</returns>
        <XmlText>
        Public Property info As T

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
