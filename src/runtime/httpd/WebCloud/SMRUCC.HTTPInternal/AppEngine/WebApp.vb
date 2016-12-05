#Region "Microsoft.VisualBasic::abe9c573f14a718774b0b85cae60ea3b, ..\httpd\HTTPServer\SMRUCC.HTTPInternal\AppEngine\WebApp.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.POSTParser
Imports SMRUCC.WebCloud.HTTPInternal.Core
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Namespace AppEngine

    ''' <summary>
    ''' API interface description: <see cref="IGET"/>, <see cref="IPOST"/>.
    ''' (外部对象需要继承这个基类才可以在App引擎之中注册自身为服务)
    ''' </summary>
    Public MustInherit Class WebApp : Inherits PlatformSub

        Public ReadOnly Property wwwroot As String
            Get
                Return PlatformEngine.wwwroot.FullName
            End Get
        End Property

        Sub New(main As PlatformEngine)
            Call MyBase.New(main)
        End Sub

        ''' <summary>
        ''' 通过复写这个方法可以使用自定义的404模板
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function Page404() As String

        Public Overrides Function ToString() As String
            Return $"{PlatformEngine.ToString} ==> {Me.GetType.Name}"
        End Function

        ''' <summary>
        ''' <see cref="APIMethods.[GET]"/>
        ''' </summary>
        ''' <param name="request"></param>
        ''' <returns></returns>
        Public Delegate Function IGET(request As HttpRequest, response As StreamWriter) As Boolean

        ''' <summary>
        ''' <see cref="APIMethods.POST"/>
        ''' </summary>
        ''' <param name="request"></param>
        ''' <param name="response"></param>
        ''' <returns></returns>
        Public Delegate Function IPOST(request As HttpPOSTRequest, response As StreamWriter) As Boolean

    End Class
End Namespace
