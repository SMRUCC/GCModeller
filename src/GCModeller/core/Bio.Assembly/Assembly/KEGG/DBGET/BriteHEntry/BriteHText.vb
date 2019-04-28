#Region "Microsoft.VisualBasic::a5c88dbcdb495b1b416ddf33d70cd77a, Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHText.vb"

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

    '     Class BriteHText
    ' 
    '         Properties: [Class], CategoryItems, CategoryLevel, ClassLabel, Degree
    '                     Description, EntryId, Level, Parent
    ' 
    '         Function: BuildPath, EnumerateEntries, GetEntries, GetHPath, GetRoot
    '                   (+2 Overloads) Load, Load_ko00001, Load_ko00002, LoadData, NormalizePath
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' BRITE Functional Hierarchies
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BriteHText : Implements INamedValue

        ''' <summary>
        ''' 大分类的标签
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ClassLabel As String Implements INamedValue.Key
        <XmlAttribute> Public Property Level As Integer
        ''' <summary>
        ''' ``ABCDEFG``, etc...
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Degree As Char

#Region "Tree"

        <XmlIgnore>
        Public Property Parent As BriteHText
        ''' <summary>
        ''' 假若这个层次还可以进行细分的话，则这个属性就是当前的小分类的子分类列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property CategoryItems As BriteHText()
#End Region

        Dim _EntryId As String

        ''' <summary>
        ''' KEGG db-get对象名词
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property EntryId As String
            Get
                If String.IsNullOrEmpty(_EntryId) Then
                    Dim Tokens As String = ClassLabel.Split.First

                    If Regex.Match(Tokens, "[a-z]\d{5}", RegexOptions.IgnoreCase).Success Then
                        _EntryId = Tokens
                    ElseIf Tokens.IsPattern("\d+") Then
                        _EntryId = Tokens
                    End If
                End If

                Return _EntryId
            End Get
        End Property

        ''' <summary>
        ''' Root class label
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property [Class] As String
            Get
                Dim o As BriteHText = Me.Parent
                Dim label$ = Nothing

                Do While Not o.Parent Is Nothing
                    label = o.ClassLabel
                    o = o.Parent
                Loop

                Return label
            End Get
        End Property

        Public ReadOnly Property Description As String
            Get
                If String.IsNullOrEmpty(EntryId) Then
                    Return ClassLabel
                Else
                    Return ClassLabel.Replace(EntryId, "").Trim
                End If
            End Get
        End Property

        Public Function GetRoot() As BriteHText
            Dim parent As BriteHText = Me.Parent

            Do While Not parent.Parent Is Nothing
                parent = parent.Parent
            Loop

            Return parent
        End Function

        ''' <summary>
        ''' 查找不到会返回空值
        ''' </summary>
        ''' <param name="Key"><see cref="EntryID"/> or <see cref="EntryID"/> in <see cref="CategoryItems"/></param>
        ''' <returns></returns>
        Public Function GetHPath(Key As String) As BriteHText()
            If String.Equals(Key, EntryId, StringComparison.OrdinalIgnoreCase) Then
                Return {Me}
            End If

            If CategoryItems.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim LQuery As BriteHText() = (From value As BriteHText
                                          In Me.CategoryItems
                                          Let path As BriteHText() = value.GetHPath(Key)
                                          Where Not path.IsNullOrEmpty
                                          Select path).FirstOrDefault
            If LQuery.IsNullOrEmpty Then
                Return Nothing
            Else
                Dim path As BriteHText() = Me.Join(LQuery).ToArray
                Return path
            End If
        End Function

        ''' <summary>
        ''' 获取得到当前的分类之下的所有的<see cref="EntryId"/>列表
        ''' </summary>
        ''' <returns></returns>
        Public Function GetEntries() As String()
            If Me.CategoryItems.IsNullOrEmpty Then
                Return {
                    EntryId
                }
            Else
                Return Me.CategoryItems _
                    .Select(Function(htext) htext.GetEntries) _
                    .ToVector
            End If
        End Function

        ''' <summary>
        ''' 递归枚举当前的分类对象之下的所有的Entry列表
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function EnumerateEntries() As IEnumerable(Of BriteHText)
            If CategoryItems.IsNullOrEmpty Then
                Yield Me
            Else
                For Each htext As BriteHText In CategoryItems _
                    .Select(Function(x) x.EnumerateEntries) _
                    .IteratesALL

                    Yield htext
                Next
            End If
        End Function

        Public ReadOnly Property CategoryLevel As Char
            Get
                If Level < 0 Then
                    Return "/"
                End If
                Return BriteHTextParser.ClassLevels(Level)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}", CategoryLevel, ClassLabel)
        End Function

        Public Shared Function NormalizePath(strValue As String) As String
            If String.IsNullOrEmpty(strValue) Then
                Return ""
            End If
            Return Regex.Replace(strValue, "(\\|/|:|\*|\?|""|<|>|\|)", "_")
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data$">文本内容或者文件的路径</param>
        ''' <returns></returns>
        Public Function Load(data$) As BriteHText
            Return BriteHTextParser.Load(data)
        End Function

        Public Shared Function Load_ko00002() As BriteHText
            Return BriteHTextParser.Load(data:=My.Resources.ko00002_keg)
        End Function

        ''' <summary>
        ''' KEGG Orthology
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Load_ko00001() As BriteHText
            Return BriteHTextParser.Load(data:=My.Resources.ko00001)
        End Function

        ''' <summary>
        ''' 创建分层次的文件保存路径
        ''' </summary>
        ''' <param name="EXPORT$">数据文件所导出的文件夹</param>
        ''' <param name="ext$">文件拓展名，KEGG数据库文件默认为``xml``格式</param>
        ''' <returns></returns>
        Public Function BuildPath(EXPORT$, Optional ext$ = ".xml") As String
            Dim levels As New List(Of String)
            Dim o As BriteHText = Me.Parent

            Do While Not o.Parent Is Nothing
                levels += o.ClassLabel
                o = o.Parent
            Loop

            Call levels.Reverse()

            Dim sub$ = levels _
                .Select(Function(s) s.NormalizePathString(False)) _
                .JoinBy("/")

            Return EXPORT & "/" & [sub] & EntryId & ext
        End Function
    End Class
End Namespace
