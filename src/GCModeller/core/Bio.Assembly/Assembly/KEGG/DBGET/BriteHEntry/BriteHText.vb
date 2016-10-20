#Region "Microsoft.VisualBasic::602364573a5e16825f2868cd43773e75, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHText.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' BRITE Functional Hierarchies
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BriteHText

        <XmlAttribute> Public Property ClassLabel As String
        <XmlElement> Public Property CategoryItems As BriteHText()
        <XmlAttribute> Public Property Level As Integer
        <XmlAttribute> Public Property Degree As Char

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
                    End If
                End If

                Return _EntryId
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

        Private Shared ReadOnly ClassLevels As Char() = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

        Public Function GetEntries() As String()
            If Me.CategoryItems.IsNullOrEmpty Then
                Return {EntryId}
            Else
                Return (From htext As BriteHText
                    In Me.CategoryItems
                        Select s_Data = htext.GetEntries).ToArray.ToVector
            End If
        End Function

        Public Shared Function Load(strData As String) As BriteHText
            Dim strLines As String() = (From strLine As String In Strings.Split(strData.Replace("<b>", "").Replace("</b>", ""), vbLf)
                                        Where Not String.IsNullOrEmpty(strLine) AndAlso (Array.IndexOf(ClassLevels, strLine.First) > -1 AndAlso Len(strLine) > 1)
                                        Select strLine).ToArray
            Dim Root As BriteHText = New BriteHText With {.ClassLabel = "/", .Level = -1, .Degree = strData(1)}
            Dim p As Integer = 0

            Dim ClassItems As List(Of BriteHText) = New List(Of BriteHText)

            Do While p < strLines.Count - 1
                Call ClassItems.Add(LoadData(strLines, p, Level:=0))
            Loop

            Root.CategoryItems = ClassItems.ToArray
            Return Root
        End Function

        Public ReadOnly Property CategoryLevel As Char
            Get
                If Level < 0 Then
                    Return "/"
                End If
                Return ClassLevels(Level)
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

        Private Shared Function LoadData(strLines As String(), ByRef p As Integer, Level As Integer) As BriteHText
            Dim Category As BriteHText = New BriteHText With {.Level = Level, .ClassLabel = Mid(strLines(p), 2).Trim}
            Call p.MoveNext()

            If p > strLines.Count - 1 Then
                Return Category
            End If

            If strLines(p).First > Category.CategoryLevel Then
                Dim SubCategories As List(Of BriteHText) = New List(Of BriteHText)
                Do While strLines(p).First > Category.CategoryLevel
                    Call SubCategories.Add(LoadData(strLines, p, Level + 1))
                    If p > strLines.Count - 1 Then
                        Exit Do
                    End If
                Loop

                Category.CategoryItems = SubCategories.ToArray
            End If

            Return Category
        End Function

        Public Shared Function Load_ko00002() As BriteHText
            Return Load(strData:=My.Resources.ko00002_keg)
        End Function

        ''' <summary>
        ''' KEGG Orthology
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Load_ko00001() As BriteHText
            Return Load(strData:=My.Resources.ko00001)
        End Function
    End Class
End Namespace
