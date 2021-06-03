#Region "Microsoft.VisualBasic::5d153e66b949f643248d3e5ab41ff99a, markdown2pdf\JavaScript\font-awesome\Script.vb"

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

' Module VBScript
' 
'     Function: FromCSS
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Emit.CodeDOM_VBC
Imports Microsoft.VisualBasic.MIME.Html.Language.CSS

Public Module VBScript

    Public Function FromCSS(fontawesome As String) As String
        Dim css As CSSFile = CssParser.GetTagWithCSS(fontawesome.SolveStream, selectorFilter:="\.fa[-]")
        Dim icons = css.Selectors.Values.Where(Function(s) s.HasProperty("content")).ToArray
        Dim members = icons _
            .Select(Function(icon As Selector)
                        Dim member = icon.Selector _
                                         .Replace(".fa-", "") _
                                         .Replace(":before", "")
                        member = CodeHelper.EnumMember(member, True, newLine:=True)
                        member = $"<Content({icon!content})>" & vbCrLf & member

                        ' 因为在CSS文件里面，content的值里面已经存在双引号了
                        ' 所以在生成自定义属性的时候不需要再添加双引号了
                        ' 否则会出错

                        ' .fa-audible: before {
                        '     content: "\f373"
                        ' }

                        Return member
                    End Function) _
            .ToArray

        With New StringBuilder

            Call .AppendLine($"Imports {GetType(DescriptionAttribute).Namespace}")
            Call .AppendLine()
            Call .AppendLine($"Public Enum {NameOf(icons)}")

            For Each member In members
                Call .AppendLine(member)
            Next

            Call .AppendLine("End Enum")

            Return .ToString
        End With
    End Function
End Module
