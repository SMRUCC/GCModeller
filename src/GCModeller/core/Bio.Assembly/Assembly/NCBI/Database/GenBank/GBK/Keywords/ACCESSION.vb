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

Imports Microsoft.VisualBasic.Language.[Default]

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    Public Class ACCESSION : Inherits KeyWord

        Public Property AccessionId As String
        Public Property Alternative As String

        Public Overrides Function ToString() As String
            Return AccessionId
        End Function

        Friend Shared Function CreateObject(str As String(), DefaultID As String) As ACCESSION
            If str.IsNullOrEmpty Then '可能是新的数据，还没有提交到数据库之中，所以这里的数据库编号可能是空的
RETURN_EMPTY:   Return New ACCESSION With {
                    .AccessionId = DefaultID,
                    .Alternative = DefaultID
                }
            Else
                Dim value As String = __fieldValue(str(Scan0))

                If String.IsNullOrEmpty(value) Then
                    GoTo RETURN_EMPTY
                End If

                Dim tokens As String() = value.Split({" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)

                If tokens.Length = 0 Then
                    GoTo RETURN_EMPTY
                Else
                    Return New ACCESSION With {
                        .AccessionId = tokens(Scan0),
                        .Alternative = tokens(tokens.Length - 1)
                    }
                End If
            End If
        End Function

        ''' <summary>
        ''' 取出头部字段的值部分(去掉前面的字段名所占用的列)
        ''' </summary>
        Friend Shared Function __fieldValue(line As String) As String
            If String.IsNullOrEmpty(line) Then
                Return ""
            End If

            Dim i As Integer = line.IndexOf(" "c)

            If i < 0 Then
                Return ""
            End If

            Return line.Substring(i).Trim()
        End Function
    End Class

    ''' <summary>
    ''' ``LOCUS``行所描述的记录摘要信息
    ''' </summary>
    ''' <remarks>
    ''' ```
    ''' LOCUS       NC_000001          248956422 bp    DNA     linear   CON 06-AUG-2025
    ''' LOCUS       SCU49845              5028 bp    DNA     circular PLN 21-JUN-1999
    ''' ```
    ''' </remarks>
    Public Class LOCUS : Inherits KeyWord

        ''' <summary>
        ''' 一般可以通过这个属性来作为整个基因组数据的唯一编号
        ''' </summary>
        ''' <returns></returns>
        Public Property AccessionID As String
        ''' <summary>
        ''' 序列的长度(碱基数量)
        ''' </summary>
        Public Property Length As Integer
        ''' <summary>
        ''' 分子的类型: ``DNA``/``RNA``/``mRNA``等
        ''' </summary>
        Public Property Type As String
        ''' <summary>
        ''' 分子的拓扑结构: ``linear``/``circular``/``both``
        ''' </summary>
        Public Property Molecular As String
        ''' <summary>
        ''' 数据库的分类代码: ``CON``/``BCT``/``PLN``/``PRI``/``MAM`` 等
        ''' </summary>
        Public Property Division As String
        ''' <summary>
        ''' 最后更新的日期
        ''' </summary>
        Public Property UpdateTime As String

        ''' <summary>
        ''' 分子的拓扑结构(``linear``/``circular``)
        ''' </summary>
        Public ReadOnly Property Topology As String
            Get
                Return Molecular
            End Get
        End Property

        ''' <summary>
        ''' 是否为环状分子
        ''' </summary>
        Public ReadOnly Property IsCircular As Boolean
            Get
                Return GbkFormat.IsCircular(Molecular)
            End Get
        End Property

        ''' <summary>
        ''' 这个记录是否为``CON``(constructed)类型的记录: 即序列是由多个片段拼接而成的
        ''' </summary>
        Public ReadOnly Property IsCON As Boolean
            Get
                Return String.Equals(Division, "CON", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return AccessionID
        End Function

        Public Shared Function InternalParser(s As String) As LOCUS
            If String.IsNullOrEmpty(s) Then
                Return Nothing
            End If

            Dim tokens As String() = s.Split({" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)
            Dim loc As New LOCUS

            If tokens.Length = 0 Then
                Return Nothing
            End If

            ' tokens(0) 为 "LOCUS" 字段名
            Dim i As Integer = 1

            If i < tokens.Length Then
                loc.AccessionID = tokens(i)
                i += 1
            End If

            Dim length As Long

            If i < tokens.Length AndAlso Long.TryParse(tokens(i), length) Then
                loc.Length = CInt(Math.Min(length, Integer.MaxValue))
                i += 1
            End If

            ' 跳过 "bp"/"aa" 这样的长度单位
            If i < tokens.Length AndAlso (String.Equals(tokens(i), "bp", StringComparison.OrdinalIgnoreCase) OrElse
                                          String.Equals(tokens(i), "aa", StringComparison.OrdinalIgnoreCase)) Then
                i += 1
            End If

            If i < tokens.Length Then
                loc.Type = tokens(i)
                i += 1
            End If

            If i < tokens.Length Then
                loc.Molecular = tokens(i)
                i += 1
            End If

            If i < tokens.Length Then
                ' 分类代码是可选的字段
                If tokens(i).Length <= 4 AndAlso Not __isDate(tokens(i)) Then
                    loc.Division = tokens(i)
                    i += 1
                End If
            End If

            If i < tokens.Length Then
                loc.UpdateTime = tokens(i)
            End If

            Return loc
        End Function

        Private Shared Function __isDate(s As String) As Boolean
            ' 日期格式: 21-JUN-1999
            If s.Length <> 11 Then
                Return False
            End If

            Return s(2) = "-"c AndAlso s(6) = "-"c
        End Function
    End Class

    Public Class DEFINITION : Inherits KeyWord

        Public Property Value As String

        Sub New()
        End Sub

        Sub New(def As String)
            Value = def
        End Sub

        Public Overrides Function ToString() As String
            Return Value
        End Function

        Public Shared Widening Operator CType(str As String()) As DEFINITION
            If str.IsNullOrEmpty Then
                Return New DEFINITION With {.Value = ""}
            End If

            Dim sb As New Text.StringBuilder()
            Dim first As String = str(Scan0)

            ' 去掉 "DEFINITION" 字段名所占用的列
            If first.Length > 10 Then
                Call sb.Append(first.Substring(10).Trim())
            End If

            For i As Integer = 1 To str.Length - 1
                Dim line As String = str(i)

                If Not String.IsNullOrEmpty(line) Then
                    Call sb.Append(" "c).Append(line.Trim())
                End If
            Next

            Return New DEFINITION With {
                .Value = sb.ToString()
            }
        End Operator

        Public Shared Narrowing Operator CType(obj As DEFINITION) As String
            Return obj.Value
        End Operator

        Friend Shared ReadOnly WGSKeywords As String() = New String() {"whole genome shotgun", "WGS"}
    End Class

    Public Class VERSION : Inherits KeyWord
        Implements IsEmpty

        Public Property Ver As String
        Public Property AccessionID As String
        Public Property GI As String

        Public ReadOnly Property IsEmpty As Boolean Implements Language.Default.IsEmpty.IsEmpty
            Get
                Return String.IsNullOrEmpty(Ver) AndAlso
                    String.IsNullOrEmpty(AccessionID) AndAlso
                    String.IsNullOrEmpty(GI)
            End Get
        End Property

        Sub New()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="v">version string</param>
        Sub New(v As String)
            Ver = v
        End Sub

        Public ReadOnly Property VersionString As String
            Get
                Return String.Format("{0}.{1}", AccessionID, Ver)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return VersionString
        End Function

        Public Shared Widening Operator CType(str As String()) As VERSION
            If str.IsNullOrEmpty Then
                Return New VERSION
            End If

            Dim value As String = ACCESSION.__fieldValue(str(Scan0))

            If String.IsNullOrEmpty(value) Then
                Return New VERSION
            End If

            Dim tokens As String() = value.Split({" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)
            Dim ver As New VERSION With {
                .AccessionID = tokens(Scan0),
                .GI = ""
            }

            For i As Integer = 1 To tokens.Length - 1
                If tokens(i).StartsWith("GI:", StringComparison.OrdinalIgnoreCase) Then
                    ver.GI = tokens(i).Substring(3)
                End If
            Next

            Dim parts As String() = ver.AccessionID.Split(CChar("."))

            If parts.Length > 1 Then
                ver.AccessionID = parts(Scan0)
                ver.Ver = parts(parts.Length - 1)
            Else
                ver.Ver = ""
            End If

            Return ver
        End Operator
    End Class
End Namespace
