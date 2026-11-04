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

Imports Microsoft.VisualBasic.Linq

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' 引用文献的解析模块，一个引用文献模块之中包含有若干篇引用文献<see cref="RefInfo"></see>
    ''' </summary>
    ''' <remarks></remarks>
    Public Class REFERENCE : Inherits KeyWord

        Public Property ReferenceList As RefInfo()

        ''' <summary>
        ''' 解析``REFERENCE``段
        ''' </summary>
        ''' <param name="chunkText">``REFERENCE``段的全部文本行</param>
        ''' <remarks>
        ''' 旧版本的实现会针对每一个``REFERENCE``头对整个文本行数组进行<c>Array.IndexOf</c>定位，
        ''' 复杂度为<c>O(n^2)</c>；这里改为顺序分块，复杂度为<c>O(n)</c>。
        ''' </remarks>
        Public Shared Function InternalParser(chunkText As String()) As REFERENCE
            If chunkText.IsNullOrEmpty Then
                Return New REFERENCE With {.ReferenceList = New RefInfo() {}}
            End If

            Dim blocks As New List(Of List(Of String))
            Dim current As List(Of String) = Nothing

            For Each line As String In chunkText
                If line.StartsWith("REFERENCE", StringComparison.Ordinal) Then
                    current = New List(Of String)
                    Call blocks.Add(current)
                End If

                If current IsNot Nothing Then
                    Call current.Add(line)
                End If
            Next

            Dim refList As New List(Of RefInfo)

            For Each block As List(Of String) In blocks
                Try
                    Call refList.Add(RefInfo.InternalParser(block.ToArray()))
                Catch ex As Exception
                    Call App.LogException(New GbkFormatException("failed to parse the REFERENCE section!", lineText:=block(Scan0), innerException:=ex))
                End Try
            Next

            Return New REFERENCE With {.ReferenceList = refList.ToArray}
        End Function
    End Class

    ''' <summary>
    ''' <see cref="REFERENCE"/>里面的一篇引用文献
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RefInfo

        Public Property BaseLocation As ComponentModel.Loci.Location
        Public Property Id As Integer
        Public Property Title As String
        Public Property Journal As String
        Public Property Pubmed As String
        Public Property Consrtm As String
        ''' <summary>
        ''' ``REMARK``字段(旧版本缺失了这个字段的解析)
        ''' </summary>
        Public Property Remark As String
        Public Property Authors As String()

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Pubmed) Then
                Return String.Join("; ", Authors.SafeQuery) & "  " & Journal
            End If
            Return String.Format("[PUBMED {0}] {1}  {2}. ({3})", Pubmed, String.Join("; ", Authors.SafeQuery), Title, Journal)
        End Function

        Public Shared Function InternalParser(str As String()) As RefInfo
            Dim ref As String = str.First
            Dim subSections = Internal_subSectionParser(str.Skip(1).ToArray)
            Dim L_ref As RefInfo = New RefInfo
            Dim numbers As New List(Of Long)
            Dim number As Long = 0
            Dim hasNumber As Boolean = False

            For i As Integer = 0 To ref.Length - 1
                Dim c As Char = ref(i)

                If c >= "0"c AndAlso c <= "9"c Then
                    number = number * 10 + (AscW(c) - AscW("0"c))
                    hasNumber = True
                ElseIf hasNumber Then
                    Call numbers.Add(number)
                    number = 0
                    hasNumber = False
                End If
            Next

            If hasNumber Then
                Call numbers.Add(number)
            End If

            If numbers.Count > 0 Then
                L_ref.Id = CInt(numbers(0))
            End If

            If numbers.Count > 2 Then
                L_ref.BaseLocation = New ComponentModel.Loci.Location(numbers(1), numbers(2))
            Else
                L_ref.BaseLocation = New ComponentModel.Loci.Location
            End If

            Dim Internal_getValue = Function(key As String) If(subSections.ContainsKey(key), subSections(key), "")

            L_ref.Authors = Strings.Split(Internal_getValue("AUTHORS"), ", ")
            L_ref.Title = Internal_getValue("TITLE")
            L_ref.Journal = Internal_getValue("JOURNAL")
            L_ref.Consrtm = Internal_getValue("CONSRTM")
            L_ref.Pubmed = Internal_getValue("PUBMED")
            L_ref.Remark = Internal_getValue("REMARK")

            Return L_ref
        End Function

        ''' <summary>
        ''' 解析引用文献之中的子字段(``AUTHORS``/``TITLE``/``JOURNAL``等)
        ''' </summary>
        Private Shared Function Internal_subSectionParser(str As String()) As Dictionary(Of String, String)
            Dim dict As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            If str.IsNullOrEmpty Then
                Return dict
            End If

            Dim head As String = __name(str(Scan0))
            Dim sb As New Text.StringBuilder(__value(str(Scan0)))

            For i As Integer = 1 To str.Length - 1
                Dim line As String = str(i)
                Dim name As String = __name(line)

                If String.IsNullOrEmpty(name) Then
                    ' 当前子字段还没有结束
                    Call sb.Append(" "c).Append(__value(line))
                Else
                    __setValue(dict, head, sb.ToString())
                    Call sb.Clear()
                    Call sb.Append(__value(line))
                    head = name
                End If
            Next

            __setValue(dict, head, sb.ToString())

            Return dict
        End Function

        ''' <summary>
        ''' 向字典之中写入数据，假如同一个名称已经存在，则将新的值追加到旧的值之后
        ''' (旧版本使用<c>Dictionary.Add</c>，在出现重复的子字段的时候会抛出异常)
        ''' </summary>
        Private Shared Sub __setValue(dict As Dictionary(Of String, String), name As String, value As String)
            If String.IsNullOrEmpty(name) Then
                Return
            End If

            Dim exists As String = Nothing

            If dict.TryGetValue(name, exists) Then
                dict(name) = exists & " " & value
            Else
                dict(name) = value
            End If
        End Sub

        ''' <summary>
        ''' 取出子字段的名称(第3列到第12列)
        ''' </summary>
        Private Shared Function __name(line As String) As String
            ' 子字段的名称占用第3列到第12列(下标2到11)
            If String.IsNullOrEmpty(line) OrElse line.Length <= 2 Then
                Return ""
            End If

            Dim endIndex As Integer = Math.Min(12, line.Length)

            If endIndex <= 2 Then
                Return ""
            End If

            Return line.Substring(2, endIndex - 2).Trim()
        End Function

        ''' <summary>
        ''' 取出子字段的值部分(第13列开始)
        ''' </summary>
        Private Shared Function __value(line As String) As String
            If String.IsNullOrEmpty(line) Then
                Return ""
            ElseIf line.Length <= 12 Then
                Return line.Trim()
            Else
                Return line.Substring(12).Trim()
            End If
        End Function
    End Class
End Namespace
