#Region "Microsoft.VisualBasic::d451d209856926a79bb356e70fa56a2f, WebCloud\SMRUCC.HTTPInternal\AppEngine\API\Abstract.vb"

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

    '     Delegate Function
    ' 
    ' 
    '     Delegate Function
    ' 
    ' 
    '     Delegate Function
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.WebCloud.HTTPInternal.Core

Namespace AppEngine.APIMethods

    ''' <summary>
    ''' WebApp API的抽象接口
    ''' </summary>
    ''' <param name="api">URL</param>
    ''' <param name="request">URL后面的参数请求</param>
    ''' <param name="response">返回的html页面的文档</param>
    ''' <returns>是否执行成功</returns>
    Public Delegate Function APIAbstract(api As String, request As HttpRequest, response As HttpResponse) As Boolean

    ''' <summary>
    ''' <see cref="[GET]"/> API interface
    ''' </summary>
    ''' <param name="request">url arguments</param>
    ''' <param name="response">output json or html page</param>
    ''' <returns>Execute success or not?</returns>
    Public Delegate Function _GET(request As HttpRequest, response As HttpResponse) As Boolean

    ''' <summary>
    ''' <see cref="POST"/> API interface
    ''' </summary>
    ''' <param name="request">url arguments and Form data</param>
    ''' <param name="response"></param>
    ''' <returns>Execute success or not?</returns>
    Public Delegate Function _POST(request As HttpPOSTRequest, response As HttpResponse) As Boolean
End Namespace
