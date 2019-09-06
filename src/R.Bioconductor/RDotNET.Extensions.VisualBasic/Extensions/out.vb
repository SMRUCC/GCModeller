#Region "Microsoft.VisualBasic::a3d4f5472e37b868320d90359408bb10, RDotNET.Extensions.VisualBasic\Extensions\out.vb"

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

    ' Class out
    ' 
    ' 
    '     Delegate Function
    ' 
    '         Properties: Parser
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 这个自定义属性是装饰于类型上面, 用于帮助<see cref="var.As(Of T)()"/>函数进行结果数据解析操作的
''' </summary>
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)>
Public Class out : Inherits Attribute

    ''' <summary>
    ''' 从R变量之中解析出一个.NET对象
    ''' </summary>
    ''' <param name="var">R变量名称</param>
    ''' <returns>.NET对象</returns>
    Public Delegate Function RObjectParser(var As String) As Object

    ''' <summary>
    ''' 将会调用这个接口函数进行R环境之中的结果解析为.NET语言环境之中的对象
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Parser As RObjectParser

    Sub New(func As RObjectParser)
        Parser = func
    End Sub

    Public Overrides Function ToString() As String
        Return Parser.ToString
    End Function
End Class
