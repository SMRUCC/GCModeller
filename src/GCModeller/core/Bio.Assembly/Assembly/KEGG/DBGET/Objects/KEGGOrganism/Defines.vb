#Region "Microsoft.VisualBasic::def65d5caddeb0a569f3191071dc1a4a, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\KEGGOrganism\Defines.vb"

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

    '   Total Lines: 184
    '    Code Lines: 119
    ' Comment Lines: 32
    '   Blank Lines: 33
    '     File Size: 6.16 KB


    '     Class Organism
    ' 
    '         Properties: [Class], KEGGId, Kingdom, Phylum, RefSeq
    '                     Species, Tcode
    ' 
    '         Function: parseObjectText, ToString, Trim
    ' 
    '     Class Prokaryote
    ' 
    '         Properties: pubmed, Year
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: GetValue, Trim
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.DBGET.bGetObject.Organism

    Public Class Organism : Implements INamedValue

        <XmlAttribute> Public Property Kingdom As String
        <XmlAttribute> Public Property Phylum As String
        <XmlAttribute> Public Property [Class] As String
        ''' <summary>
        ''' 物种全称
        ''' </summary>
        ''' <returns></returns>
        Public Property Species As String
        ''' <summary>
        ''' T code for KEGG www_bfind
        ''' </summary>
        ''' <returns></returns>
        Public Property Tcode As String

        ''' <summary>
        ''' KEGG里面的物种的简称代码
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property KEGGId As String Implements INamedValue.Key
        ''' <summary>
        ''' FTP url on NCBI
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RefSeq As String

        Public Const Field As String = "<td rowspan=\d+  align=[a-z]+>.+</td>"
        Public Const ClassType As String = "<td rowspan=\d+  align=[a+z]+><a href='.+'>.+</a></td>"
        Public Const CELL_ITEM As String = "<td align=[a-z]+><a href='.+'>[a-z0-9]+</a>"

        Private Shared ReadOnly doSetValueOf As Action(Of Organism, String)() = {
            Sub(org, value) org.Kingdom = value,
            Sub(org, value) org.Phylum = value,
            Sub(org, value) org.Class = value,
            Sub(org, value) org.KEGGId = value,
            Sub(org, value) org.Species = value,
            Sub(org, value) org.RefSeq = value
        }

        Friend Overridable Function Trim() As Organism
            Phylum = Phylum.GetValue()
            [Class] = [Class].GetValue
            Species = Species.GetValue
            KEGGId = KEGGId.GetValue
            Kingdom = Kingdom.GetValue
            RefSeq = r.Match(RefSeq, "("".+"")|('.+')").Value _
                .GetStackValue("""", """") _
                .GetStackValue("'", "'")

            Return Me
        End Function

        Friend Shared Function parseObjectText(text As String) As Organism
            Dim columns As String() = text.GetColumnsHTML

            If columns.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim org As New Organism
            Dim p As Integer = Organism.doSetValueOf.Length - 1

            For i As Integer = columns.Length - 1 To 0 Step -1
                Call Organism.doSetValueOf(p)(org, columns(i))
                p -= 1
            Next

            org.Tcode = org.Species.href.Match("T\d+")

            Return org
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", KEGGId, Species)
        End Function
    End Class

    ''' <summary>
    ''' 原核生物
    ''' </summary>
    ''' <remarks>
    ''' 原核生物相较于真核生物的数据，在KEGG的列表中多了一个pubmed编号数据
    ''' </remarks>
    Public Class Prokaryote : Inherits Organism

        ''' <summary>
        ''' 首次测序发表的年份
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Year As String
        ''' <summary>
        ''' 首次测序发表的论文的在NCBI的pubmed数据库中的文献编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property pubmed As String

        Public Sub New()
        End Sub

        Sub New(org As Organism)
            MyBase.Class = org.Class
            MyBase.KEGGId = org.KEGGId
            MyBase.Kingdom = org.Kingdom
            MyBase.Phylum = org.Phylum
            MyBase.RefSeq = org.RefSeq
            MyBase.Species = org.Species
            MyBase.Tcode = org.Tcode
        End Sub

        Private Shared ReadOnly SetValues As Action(Of Prokaryote, String)() = {
            Sub(org, value) org.Kingdom = value,
            Sub(org, value) org.Phylum = value,
            Sub(org, value) org.Class = value,
            Sub(org, value) org.KEGGId = value,
            Sub(org, value) org.Species = value,
            Sub(org, value) org.Year = value,
            Sub(org, value) org.RefSeq = value
        }

        Protected Friend Sub New(text As String)
            Dim columns As String() = text.GetColumnsHTML

            If columns.IsNullOrEmpty Then
            Else
                Dim p As Integer = Prokaryote.SetValues.Length - 1

                For i As Integer = columns.Length - 1 To 0 Step -1
                    Call Prokaryote.SetValues(p)(Me, columns(i))
                    p -= 1
                Next

                Tcode = Species.href.Match("T\d+")
                pubmed = Year.href.Match("pubmed[/]\d+")

                If Not pubmed.StringEmpty Then
                    pubmed = pubmed.Split("/"c).Last
                End If
            End If
        End Sub

        Friend Overrides Function Trim() As Organism
            Phylum = GetValue(Phylum)
            [Class] = GetValue([Class])
            Species = GetValue(Species)
            KEGGId = GetValue(KEGGId)
            Year = GetValue(Year)

            If Not String.IsNullOrEmpty(RefSeq) Then
                RefSeq = r.Match(RefSeq, "<a\s*href\s*[=]\s*("".+?"")|('.+?')\s*>").Value
            End If

            RefSeq = RefSeq.GetStackValue("""", """").GetStackValue("'", "'")
            Kingdom = GetValue(Kingdom)

            Return Me
        End Function

        Protected Friend Overloads Shared Function GetValue(str As String) As String
            If String.IsNullOrEmpty(str) Then
                Return ""
            End If

            Dim m = r.Match(str, "<a href="".+?"">.+?</a>")

            If m.Success Then
                str = m.Value.GetValue
            Else
                str = str.GetValue
            End If

            Return str
        End Function
    End Class
End Namespace
