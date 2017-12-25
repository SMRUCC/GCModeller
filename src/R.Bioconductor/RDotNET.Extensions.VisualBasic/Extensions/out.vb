#Region "Microsoft.VisualBasic::0e5c5b72f988c07a960bc40b783f754b, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\Extensions\out.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)>
Public Class out : Inherits Attribute

    ''' <summary>
    ''' 从R变量之中解析出一个.NET对象
    ''' </summary>
    ''' <param name="var">R变量名称</param>
    ''' <returns>.NET对象</returns>
    Public Delegate Function RObjectParser(var As String) As Object

    Public ReadOnly Property Parser As RObjectParser

    Sub New(func As RObjectParser)
        Parser = func
    End Sub

    Public Overrides Function ToString() As String
        Return Parser.ToString
    End Function
End Class
