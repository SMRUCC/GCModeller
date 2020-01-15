#Region "Microsoft.VisualBasic::d5e21aa838d42957fde3903a157184ad, modules\ReportBuilder\HTML\TemplateHandler.vb"

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
    '         Properties: Builder, Path
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
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

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
            builder = New ScriptBuilder(path.ReadAllText)

            Call HtmlInterpolate()
        End Sub

        Const InterpolateRef As String = "[$]\{.+?\}"

        ''' <summary>
        ''' 在模板之中可能还会存在html碎片的插值
        ''' 在这里进行模板的html碎片的加载
        ''' </summary>
        Private Sub HtmlInterpolate()
            ' 模板的插值格式为${relpath}
            Dim relpath = r.Matches(builder.ToString, InterpolateRef, RegexICSng).ToArray
            Dim dir As String = path.ParentPath

            For Each refpath As String In relpath
                With refpath.GetStackValue("{", "}")
                    Call builder.Replace(refpath, $"{dir}/{ .ByRef}".ReadAllText)
                End With
            Next
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
