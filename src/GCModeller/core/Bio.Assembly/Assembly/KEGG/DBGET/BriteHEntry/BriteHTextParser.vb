#Region "Microsoft.VisualBasic::c9954c47cb5ede881a31ecd67e349a5a, Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHTextParser.vb"

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

    '     Module BriteHTextParser
    ' 
    '         Function: (+2 Overloads) Load, LoadData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Module BriteHTextParser

        Friend ReadOnly ClassLevels As Char() = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data$">文本内容或者文件的路径</param>
        ''' <returns></returns>
        Public Function Load(data$) As BriteHText
            Dim lines As String() = LinqAPI.Exec(Of String) <=
 _
                From s As String
                In data.Replace("<b>", "").Replace("</b>", "").LineTokens
                Where Not String.IsNullOrEmpty(s) AndAlso (Array.IndexOf(ClassLevels, s.First) > -1 AndAlso Len(s) > 1)
                Select s

            Return Load(lines, data(1))
        End Function

        Public Function Load(lines$(), Optional depth$ = "Z"c) As BriteHText
            Dim classes As New List(Of BriteHText)
            Dim p As Integer = 0
            Dim root As New BriteHText With {
                .classLabel = "/",
                .level = -1,
                .degree = depth
            }

            Do While p < lines.Length - 1
                Call classes.Add(LoadData(lines, p, level:=0, parent:=root))
            Loop

            root.categoryItems = classes

            Return root
        End Function

        ''' <summary>
        ''' 递归加载层次数据
        ''' </summary>
        ''' <param name="lines"></param>
        ''' <param name="p"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Private Function LoadData(lines$(), ByRef p%, level%, parent As BriteHText) As BriteHText
            Dim category As New BriteHText With {
                .level = level,
                .classLabel = Mid(lines(p), 2).Trim,
                .parent = parent
            }

            p += 1

            If p > lines.Length - 1 Then
                Return category
            End If

            If lines(p).First > category.CategoryLevel Then
                Dim subCategory As New List(Of BriteHText)

                Do While lines(p).First > category.CategoryLevel
                    subCategory += LoadData(lines, p, level + 1, parent:=category)

                    If p > lines.Length - 1 Then
                        Exit Do
                    End If
                Loop

                category.categoryItems = subCategory
            End If

            Return category
        End Function
    End Module
End Namespace
