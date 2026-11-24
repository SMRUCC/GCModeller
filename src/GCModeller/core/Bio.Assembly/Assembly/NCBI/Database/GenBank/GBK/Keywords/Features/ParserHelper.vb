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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' ``FEATURES``段的增量解析器。
    ''' </summary>
    ''' <remarks>
    ''' 旧版本的实现需要先将整个``FEATURES``段展开为一个巨大的字符串数组，
    ''' 然后再逐块进行解析；在人基因组级别的数据库文件之中，单个染色体就会具有
    ''' 90万行以上的位点注释数据，这种两步法的内存开销以及复制开销都是不可接受的。
    ''' 
    ''' 现在改为这个<see cref="FeatureBuilder"/>增量状态机: 逐行喂入文本，
    ''' 内部只会保留当前正在解析的feature的临时缓冲区，解析完成的feature会立即
    ''' 添加到列表之中。
    ''' </remarks>
    Module ParserHelper

        ''' <summary>
        ''' 将``FEATURES``段的文本行解析为位点注释数据的列表
        ''' </summary>
        ''' <param name="data">``FEATURES``段之下的文本行(不包含``FEATURES``标题行)</param>
        ''' <param name="context">用于错误信息定位的上下文(一般是记录的编号)</param>
        <Extension>
        Public Function FeaturesListParser(data As IEnumerable(Of String), context As String) As FEATURES
            Dim builder As New FeatureBuilder(context)

            For Each line As String In data
                Call builder.Feed(line)
            Next

            Return builder.Complete()
        End Function

        ''' <summary>
        ''' 创建一个空的``source``位点，用于降级处理缺失``source``位点的记录
        ''' </summary>
        Friend Function __nullFeature() As Feature
            Return New Feature With {
                .KeyName = "source",
                .Location = New Location With {
                    .Complement = False,
                    .Locations = {New RegionSegment With {.Left = 0, .Right = 0}}
                }
            }
        End Function
    End Module

    ''' <summary>
    ''' ``FEATURES``段的增量状态机解析器
    ''' </summary>
    Public NotInheritable Class FeatureBuilder

        Private ReadOnly list As New List(Of Feature)
        Private ReadOnly qualifiers As New List(Of NamedValue(Of String))
        Private ReadOnly context As String

        Private keyName As String
        Private location As StringBuilder
        Private qualifier As StringBuilder
        Private hasFeature As Boolean

        ''' <summary>
        ''' 解析过程之中所跳过的非法行的数量
        ''' </summary>
        Public Property InvalidLines As Integer
        ''' <summary>
        ''' 当前的feature数量
        ''' </summary>
        Public ReadOnly Property Count As Integer
            Get
                Return list.Count
            End Get
        End Property

        Sub New(Optional context As String = Nothing)
            Me.context = If(context, "unknown")
        End Sub

        ''' <summary>
        ''' 喂入一行``FEATURES``段的文本数据
        ''' </summary>
        Public Sub Feed(line As String)
            If String.IsNullOrEmpty(line) Then
                Return
            End If

            If GbkFormat.IsFeatureHeader(line) Then
                Call FlushQualifier()
                Call FlushFeature()
                Call __startFeature(line)
            ElseIf GbkFormat.IsQualifierStart(line) Then
                Call FlushQualifier()
                qualifier = New StringBuilder(line.Substring(GbkFormat.QUALIFIER_COLUMN))
            ElseIf qualifier IsNot Nothing Then
                ' qualifier的值在换行处使用空格进行连接
                Call qualifier.Append(" "c).Append(line.Trim())
            ElseIf location IsNot Nothing Then
                ' location 的跨行书写在换行处是直接拼接的，不会产生额外的空白字符
                Call location.Append(line.Trim())
            Else
                InvalidLines += 1
            End If
        End Sub

        Private Sub __startFeature(line As String)
            Dim start As Integer = GbkFormat.FEATURE_KEY_COLUMN
            Dim endIndex As Integer = line.IndexOf(" "c, start)

            If endIndex < 0 Then
                keyName = line.Substring(start).Trim()
                location = New StringBuilder()
            Else
                keyName = line.Substring(start, endIndex - start)
                location = New StringBuilder(line.Substring(endIndex).Trim())
            End If

            hasFeature = True
        End Sub

        Private Sub FlushQualifier()
            If qualifier Is Nothing Then
                Return
            End If

            Dim text As String = qualifier.ToString()
            qualifier = Nothing

            If String.IsNullOrEmpty(text) Then
                Return
            End If

            If hasFeature Then
                Call qualifiers.Add(Feature.doParser(text.Substring(1)))
            End If
        End Sub

        Private Sub FlushFeature()
            If Not hasFeature Then
                Return
            End If

            Dim feature As New Feature With {
                .KeyName = keyName,
                .Location = LocationParser.Parse(If(location Is Nothing, "", location.ToString()))
            }

            For Each attr As NamedValue(Of String) In qualifiers
                Call feature.attrSet.Add(attr)
            Next

            Call qualifiers.Clear()
            Call list.Add(feature)

            hasFeature = False
            keyName = Nothing
            location = Nothing
        End Sub

        ''' <summary>
        ''' 完成解析，返回``FEATURES``段的解析结果
        ''' </summary>
        Public Function Complete() As FEATURES
            Call FlushQualifier()
            Call FlushFeature()

            Dim source As Feature = Nothing

            For Each feature As Feature In list
                If String.Equals(feature.KeyName, "source", StringComparison.OrdinalIgnoreCase) Then
                    source = feature
                    Exit For
                End If
            Next

            If source Is Nothing AndAlso InvalidLines > 0 Then
                Call $"genbank record '{context}' has no 'source' feature!".warning
            End If

            Dim features As New FEATURES With {
                ._innerList = list,
                .InvalidLines = InvalidLines
            }

            ' source位点已经存在于列表之中(正常情况下是列表的第一个元素)，
            ' 所以这里的赋值不会产生重复的位点
            If source IsNot Nothing Then
                features.source = source
            End If

            Return features
        End Function
    End Class
End Namespace
