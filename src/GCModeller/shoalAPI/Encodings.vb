#Region "Microsoft.VisualBasic::51eda79f09459cc6a25d56ce6aea00a2, ..\GCModeller\shoalAPI\Encodings.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' 可以通过这个模块之中的方法来获取文本编码
''' </summary>
''' 
<[Namespace]("Text.Encodings")>
Public Module Encodings

    <ExportAPI("ASCII")>
    Public Function ASCII() As System.Text.Encoding
        Return System.Text.Encoding.ASCII
    End Function

    <ExportAPI("UTF8")>
    Public Function UTF8() As System.Text.Encoding
        Return System.Text.Encoding.UTF8
    End Function

    <ExportAPI("Unicode")>
    Public Function Unicode() As System.Text.Encoding
        Return System.Text.Encoding.Unicode
    End Function

    <ExportAPI("UTF32")>
    Public Function UTF32() As System.Text.Encoding
        Return System.Text.Encoding.UTF32
    End Function

    <ExportAPI("GB2312", Info:="Text file encoding format for chinese characters.")>
    Public Function GB2312() As System.Text.Encoding
        Return System.Text.Encoding.GetEncoding("GB2312")
    End Function

End Module

