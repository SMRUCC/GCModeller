#Region "Microsoft.VisualBasic::ef3e2ee0d6a3809607de3283fe49d5d5, ..\GCModeller\core\Bio.Assembly\ComponentModel\DBLinkBuilder\Abstract.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.DBLinkBuilder

    Public Interface IMetabolite
        Property ChEBI As String()
        Property KEGGCompound As String
    End Interface

    Public Interface IDBLink
        Property locusId As String
        Property Address As String
        ''' <summary>
        ''' 将对象模型转换为含有格式的字符串的值用以写入文件之中
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetFormatValue() As String
    End Interface
End Namespace
