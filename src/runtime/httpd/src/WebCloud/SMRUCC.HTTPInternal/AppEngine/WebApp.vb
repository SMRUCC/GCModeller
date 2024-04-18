﻿#Region "Microsoft.VisualBasic::1b347483126aba4d25951c20c38ef1be, WebCloud\SMRUCC.HTTPInternal\AppEngine\WebApp.vb"

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

    '     Class WebApp
    ' 
    '         Properties: wwwroot
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    '         Delegate Function
    ' 
    ' 
    '         Delegate Function
    ' 
    '             Function: GetAPIMethod
    ' 
    '             Sub: AddDynamics
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.Core
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Namespace AppEngine

    ''' <summary>
    ''' API interface description: <see cref="IGET"/>, <see cref="IPOST"/>.
    ''' (外部对象需要继承这个基类才可以在App引擎之中注册自身为服务)
    ''' </summary>
    ''' <remarks>
    ''' vbs = vb server
    ''' </remarks>
    Public MustInherit Class WebApp : Inherits Submodule

        ''' <summary>
        ''' 网页文档的根目录
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property wwwroot As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return PlatformEngine.wwwroot.FullName
            End Get
        End Property

        Sub New(main As PlatformEngine)
            Call MyBase.New(main)

            methods = MyClass _
                .GetType _
                .GetMethods(BindingFlags.Public Or BindingFlags.Instance) _
                .Where(Function(m)
                           Return Not m.GetCustomAttribute(GetType(APIMethod)) Is Nothing
                       End Function) _
                .ToDictionary(Function(m) m.Name)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{PlatformEngine.ToString} ==> {Me.GetType.Name}"
        End Function

        ''' <summary>
        ''' <see cref="APIMethods.[GET]"/>
        ''' </summary>
        ''' <param name="request"></param>
        ''' <returns></returns>
        Public Delegate Function IGET(request As HttpRequest, response As HttpResponse) As Boolean

        ''' <summary>
        ''' <see cref="APIMethods.POST"/>
        ''' </summary>
        ''' <param name="request"></param>
        ''' <param name="response"></param>
        ''' <returns></returns>
        Public Delegate Function IPOST(request As HttpPOSTRequest, response As HttpResponse) As Boolean

        ''' <summary>
        ''' 只会加载有<see cref="APIMethod"/>属性标记的实例方法
        ''' </summary>
        ReadOnly methods As Dictionary(Of String, MethodInfo)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url$"></param>
        ''' <param name="method"></param>
        ''' <param name="API">方法对象必须要是在当前的这个实例类型之中所定义的</param>
        ''' <param name="help$"></param>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub AddDynamics(url$, method As APIMethod, API As MethodInfo, Optional help$ = "")
            Call PlatformEngine.AppManager.Join(url, method, API, app:=Me, help:=help)
        End Sub

        ''' <summary>
        ''' 请注意，这个方法所获取的对象都必须是具备有<see cref="APIMethod"/>属性标记的方法对象，
        ''' 假若你的方法找不到的话，即出现了NullReference的错误，请检查是否对函数对象添加了
        ''' <see cref="APIMethod"/>自定义属性标记？
        ''' </summary>
        ''' <param name="name$">必须要使用NameOf操作符来获取</param>
        ''' <returns></returns>
        Public Function GetAPIMethod(name$) As MethodInfo
            If methods.ContainsKey(name) Then
                Return methods(name)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
