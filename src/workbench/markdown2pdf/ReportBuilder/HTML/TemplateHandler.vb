#Region "Microsoft.VisualBasic::e0f477987f97fd8cf38b70e78a824f21, markdown2pdf\ReportBuilder\HTML\TemplateHandler.vb"

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

    '     Class TemplateHandler
    ' 
    '         Properties: builder, path
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ToString
    ' 
    '         Sub: Flush, HtmlInterpolate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text

Namespace HTML

    ''' <summary>
    ''' A html template file handler
    ''' </summary>
    Public Class TemplateHandler

        ''' <summary>
        ''' 模板文件的文件全路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property path As String
        ''' <summary>
        ''' 模板文本字符串的缓存
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property builder As ScriptBuilder

        Sub New(file As String)
            ' 可能在将报告写入硬盘文件之前，文件系统的上下文已经变了
            ' 所以需要在这里获取得到全路径
            path = file.GetFullPath
            builder = New ScriptBuilder(Interpolation.Interpolate(path))
        End Sub

        ''' <summary>
        ''' Interpolated html report file save
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Flush()
            Call builder.Save(path, TextEncodings.UTF8WithoutBOM)
        End Sub

        Public Overrides Function ToString() As String
            Return path
        End Function
    End Class
End Namespace
