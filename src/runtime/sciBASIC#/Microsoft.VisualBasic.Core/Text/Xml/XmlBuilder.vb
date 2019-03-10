﻿#Region "Microsoft.VisualBasic::3d00cbb3236855ce2e0e755ce8aa9501, Microsoft.VisualBasic.Core\Text\Xml\XmlBuilder.vb"

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

'     Class XmlBuilder
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: Unescape
'         Operators: +
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder

Namespace Text.Xml.Models

    ''' <summary>
    ''' Builder for XML and html
    ''' </summary>
    Public Class XmlBuilder : Inherits ScriptBuilder

        ''' <summary>
        ''' 创建一个新的Xml文档构建工具，可以通过这个工具来构建出``Xml/Html``文档
        ''' </summary>
        Sub New()
            Call MyBase.New(1024)
        End Sub

        ''' <summary>
        ''' 从当前文本位置处添加一行``Xml/Html``注释文本
        ''' </summary>
        ''' <param name="comment"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AddComment(comment As String) As XmlBuilder
            Call AppendLine($"<!-- {comment} -->")
            Return Me
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Unescape() As XmlBuilder
            Call Replace("&lt;", "<").Replace("&gt;", ">")
            Return Me
        End Function

        Public Overloads Shared Operator +(xb As XmlBuilder, node As XElement) As XmlBuilder
            Call xb.Script.AppendLine(node.ToString)
            Return xb
        End Operator
    End Class
End Namespace
