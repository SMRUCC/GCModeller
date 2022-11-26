#Region "Microsoft.VisualBasic::14c18efdb0ab6d76190f02f060bec265, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHText\BriteHText.vb"

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


    ' Code Statistics:

    '   Total Lines: 249
    '    Code Lines: 157
    ' Comment Lines: 54
    '   Blank Lines: 38
    '     File Size: 8.57 KB


    '     Class BriteHText
    ' 
    '         Properties: [class], categoryItems, CategoryLevel, classLabel, degree
    '                     description, entryID, level, parent
    ' 
    '         Function: BuildPath, EnumerateEntries, GetEntries, GetHPath, GetLevelList
    '                   GetRoot, Load, Load_ko00001, Load_ko00002, NormalizePath
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports r = System.Text.RegularExpressions.Regex

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
        <XmlAttribute> Public Property classLabel As String Implements INamedValue.Key
        <XmlAttribute> Public Property level As Integer
        ''' <summary>
        ''' ``ABCDEFG``, etc...
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property degree As Char

#Region "Tree"

        <XmlIgnore>
        Public Property parent As BriteHText
        ''' <summary>
        ''' 假若这个层次还可以进行细分的话，则这个属性就是当前的小分类的子分类列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property categoryItems As BriteHText()
#End Region

        Public ReadOnly Property CategoryLevel As Char
            Get
                If level < 0 Then
                    Return "/"
                End If
                Return BriteHTextParser.classLevels(level)
            End Get
        End Property

        Dim entry As String

        ''' <summary>
        ''' KEGG db-get对象名词
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property entryID As String
            Get
                If String.IsNullOrEmpty(entry) Then
                    Dim tokens As String = classLabel.Split.First

                    If r.Match(tokens, "[a-z]\d{3,}", RegexOptions.IgnoreCase).Success Then
                        entry = tokens
                    ElseIf tokens.IsPattern("\d+") Then
                        entry = tokens
                    End If
                End If

                Return entry
            End Get
        End Property

        ''' <summary>
        ''' Root class label
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property [class] As String
            Get
                Dim o As BriteHText = Me.parent
                Dim label$ = Nothing

                Do While Not o.parent Is Nothing
                    label = o.classLabel
                    o = o.parent
                Loop

                Return label
            End Get
        End Property

        Public ReadOnly Property description As String
            Get
                If String.IsNullOrEmpty(entryID) Then
                    Return classLabel
                Else
                    Return classLabel.Replace(entryID, "").Trim
                End If
            End Get
        End Property

        Public Function GetRoot() As BriteHText
            Dim parent As BriteHText = Me.parent

            Do While Not parent.parent Is Nothing
                parent = parent.parent
            Loop

            Return parent
        End Function

        Public Function GetLevelList() As Dictionary(Of String, String)
            Dim parents As New List(Of BriteHText) From {Me}

            Do While Not parent.parent Is Nothing
                parents += parent.parent
                parent = parent.parent
            Loop

            Return parents _
                .ToDictionary(Function(level) level.degree.ToString,
                              Function(level)
                                  Return level.classLabel
                              End Function)
        End Function

        ''' <summary>
        ''' 查找不到会返回空值
        ''' </summary>
        ''' <param name="key"><see cref="entryID"/> or <see cref="entryID"/> in <see cref="CategoryItems"/></param>
        ''' <returns></returns>
        Public Function GetHPath(key As String) As BriteHText()
            If String.Equals(key, entryID, StringComparison.OrdinalIgnoreCase) Then
                Return {Me}
            End If

            If categoryItems.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim LQuery As BriteHText() = (From value As BriteHText
                                          In Me.categoryItems
                                          Let path As BriteHText() = value.GetHPath(key)
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
        ''' 获取得到当前的分类之下的所有的<see cref="entryID"/>列表
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GetEntries() As IEnumerable(Of String)
            If Me.categoryItems.IsNullOrEmpty Then
                Yield entryID
            Else
                For Each id As String In Me.categoryItems _
                    .Select(Function(htext) htext.GetEntries) _
                    .IteratesALL

                    Yield id
                Next
            End If
        End Function

        ''' <summary>
        ''' 递归枚举当前的分类对象之下的所有的Entry列表
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function EnumerateEntries() As IEnumerable(Of BriteHText)
            If categoryItems.IsNullOrEmpty Then
                Yield Me
            Else
                For Each htext As BriteHText In categoryItems _
                    .Select(Function(x) x.EnumerateEntries) _
                    .IteratesALL

                    Yield htext
                Next
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}", CategoryLevel, classLabel)
        End Function

        Public Shared Function NormalizePath(strValue As String) As String
            If String.IsNullOrEmpty(strValue) Then
                Return ""
            End If

            Return r.Replace(strValue, "(\\|/|:|\*|\?|""|<|>|\|)", "_")
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data$">文本内容或者文件的路径</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Load(data As String) As BriteHText
            Return BriteHTextParser.Load(data)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Load_ko00002() As BriteHText
            Return BriteHTextParser.Load(text:=My.Resources.ko00002_keg)
        End Function

        ''' <summary>
        ''' KEGG Orthology
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Load_ko00001() As BriteHText
            Return BriteHTextParser.Load(text:=My.Resources.ko00001)
        End Function

        ''' <summary>
        ''' 创建分层次的文件保存路径
        ''' </summary>
        ''' <param name="EXPORT$">数据文件所导出的文件夹</param>
        ''' <param name="ext$">文件拓展名，KEGG数据库文件默认为``xml``格式</param>
        ''' <returns></returns>
        Public Function BuildPath(EXPORT$, Optional ext$ = ".xml") As String
            Dim levels As New List(Of String)
            Dim b As BriteHText = Me.parent

            Do While Not b.parent Is Nothing
                levels += b.classLabel
                b = b.parent
            Loop

            Call levels.Reverse()

            Dim sub$ = levels _
                .Select(Function(s) s.NormalizePathString(False)) _
                .JoinBy("/")

            Return EXPORT & "/" & [sub] & entryID & ext
        End Function
    End Class
End Namespace
