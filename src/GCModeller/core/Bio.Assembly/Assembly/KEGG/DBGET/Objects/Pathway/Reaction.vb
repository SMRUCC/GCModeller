#Region "Microsoft.VisualBasic::7967d3d25141d1a36315c2d1135927dc, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Reaction.vb"

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

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' KEGG reaction annotation data.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Reaction : Implements INamedValue

        <XmlAttribute>
        Public Property Entry As String Implements INamedValue.Key
        Public Property CommonNames As String()
        Public Property Definition As String
        Public Property Equation As String

        ''' <summary>
        ''' 标号： <see cref="Expasy.Database.Enzyme.Identification"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ECNum As String()
        Public Property Comments As String
        Public Property Pathway As KeyValuePair()
        Public Property [Module] As KeyValuePair()
        Public Property Orthology As TripleKeyValuesPair()
        Public Property ReactionClass As KeyValuePair()

        Const URL As String = "http://www.kegg.jp/dbget-bin/www_bget?rn:{0}"

        Public ReadOnly Property ReactionModel As DefaultTypes.Equation
            Get
                Try
                    Return EquationBuilder.CreateObject(Me.Equation)
                Catch ex As Exception
                    ex = New Exception(Me.ToString, ex)
                    Throw ex
                End Try
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}:  {2}", ECNum, Entry, Definition)
        End Function

        Public Shared Function Download(Id As String) As Reaction
            Return DownloadFrom(String.Format(URL, Id))
        End Function

        Public Shared Function DownloadFrom(url As String) As Reaction
            Dim WebForm As New InternalWebFormParsers.WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            Else
                Return __webFormParser(Of Reaction)(WebForm)
            End If
        End Function

        Friend Shared Function __webFormParser(Of ReactionType As Reaction)(WebForm As InternalWebFormParsers.WebForm) As ReactionType
            Dim rn As ReactionType = Activator.CreateInstance(Of ReactionType)()

            On Error Resume Next

            rn.Entry = WebForm.StripName(WebForm.GetValue("Entry").FirstOrDefault).StripHTMLTags.StripBlank.Split.First
            rn.Comments = __trimComments(WebForm.StripName(WebForm.GetValue("Comment").FirstOrDefault)).StripBlank.TrimNewLine
            rn.Definition = WebForm.StripName(WebForm.GetValue("Definition").FirstOrDefault).StripHTMLTags.StripBlank
            rn.Pathway = WebForm.parseList(WebForm.GetValue("Pathway").FirstOrDefault, "<a href="".+?"">.+?</a>")
            rn.Module = WebForm.parseList(WebForm.GetValue("Module").FirstOrDefault, "<a href="".+?"">.+?</a>")
            rn.CommonNames = __getCommonNames(WebForm.GetValue("Name").FirstOrDefault)
            rn.Equation = __parsingEquation(WebForm.GetValue("Equation").FirstOrDefault)
            rn.Orthology = __orthologyParser(WebForm.GetValue("Orthology").FirstOrDefault)
            rn.ReactionClass = WebForm.parseList(WebForm.GetValue("Reaction class").FirstOrDefault, "<a href="".+?"">.+?</a>")

            Dim ecTemp As String = WebForm.GetValue("Enzyme").FirstOrDefault
            rn.ECNum = Regex.Matches(ecTemp, "\d+(\.\d+)+").ToArray.Distinct.ToArray

            Return rn
        End Function

        Private Shared Function __orthologyParser(s As String) As TripleKeyValuesPair()
            Dim ms As String() = Regex.Matches(s, "K\d+<.+?\[EC.+?\]", RegexOptions.IgnoreCase).ToArray
            Dim result As TripleKeyValuesPair() = ms.ToArray(AddressOf __innerOrthParser)
            Return result
        End Function

        ''' <summary>
        ''' K01509&lt;/a> adenosinetriphosphatase [EC:&lt;a href="/dbget-bin/www_bget?ec:3.6.1.3">3.6.1.3&lt;/a>]
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Private Shared Function __innerOrthParser(s As String) As TripleKeyValuesPair
            Dim t As String() = Regex.Split(s, "<[/]?a>", RegexOptions.IgnoreCase)
            Dim KO As String = t.Get(Scan0)
            Dim def As String = t.Get(1).Split("["c).First.Trim
            Dim EC As String = Regex.Match(s, "\d+(\.\d+)+").Value

            Return New TripleKeyValuesPair With {
                .Key = KO,
                .Value1 = EC,
                .Value2 = def.StripHTMLTags
            }
        End Function

        Public ReadOnly Property Reversible As Boolean
            Get
                Return InStr(Equation, " <=> ") > 0
            End Get
        End Property

        Const KEGG_COMPOUND_ID As String = "[A-Z]+\d+"

        ''' <summary>
        ''' 得到本反应过程对象中的所有的代谢底物的KEGG编号，以便于查询和下载
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSubstrateCompounds() As String()
            Dim FluxModel = EquationBuilder.CreateObject(Of
                DefaultTypes.CompoundSpecieReference,
                DefaultTypes.Equation)(Regex.Replace(Equation, "(\s*\(.+?\))|(n )", ""))
            Dim Compounds$() = LinqAPI.Exec(Of String) <=
 _
                From csr As DefaultTypes.CompoundSpecieReference
                In {
                    FluxModel.Reactants,
                    FluxModel.Products
                }.IteratesALL
                Select csr.Identifier
                Distinct

            Return Compounds
        End Function

        Public Function IsConnectWith([next] As Reaction) As Boolean
            Dim a = GetSubstrateCompounds(),
                b = [next].GetSubstrateCompounds

            For Each s In a
                If Array.IndexOf(b, s) > -1 Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="lstId"></param>
        ''' <param name="outDIR"></param>
        ''' <returns>返回成功下载的对象的数目</returns>
        ''' <remarks></remarks>
        Public Shared Function FetchTo(lstId As String(), outDIR As String) As Integer
            Dim i As Integer = 0

            For Each Id As String In lstId
                Dim ReactionData As Reaction = Download(Id)
                If ReactionData Is Nothing Then
                    Continue For
                End If

                Dim Path As String = String.Format("{0}/{1}.xml", outDIR, Id)
                Call ReactionData.GetXml.SaveTo(Path)
            Next

            Return i
        End Function

        Private Shared Function __trimComments(strData As String) As String
            If String.IsNullOrEmpty(strData) Then
                Return ""
            End If

            Dim Links As KeyValuePair(Of String, String)() =
                Regex.Matches(strData, "<a href="".+?"">.+?</a>").ToArray(
                Function(m) New KeyValuePair(Of String, String)(m, m.GetValue))
            Dim sBuilder As StringBuilder = New StringBuilder(strData)
            For Each item As KeyValuePair(Of String, String) In Links
                Call sBuilder.Replace(item.Key, item.Value)
            Next
            Call sBuilder.Replace("<br>", "")

            Return sBuilder.ToString.StripHTMLTags
        End Function

        Private Shared Function __parsingEquation(strData As String) As String
            Dim sb As New StringBuilder(strData)

            For Each m As Match In Regex.Matches(strData, "<a href="".+?"">.+?</a>")
                Dim link As New KeyValuePair(Of String, String)(m.Value, m.Value.GetValue)
                Call sb.Replace(link.Key, link.Value)
            Next

            Dim s$ = sb.ToString

            s = s.Replace(Regex.Match(s, "<nobr>.+</nobr>").Value, "")
            s = s.Replace("<=", "&lt;=")
            s = s.Replace("<-", "&lt;-")
            s = s.StripHTMLTags
            s = s.Replace("&lt;", "<")
            s = s.Replace("&gt;", ">")
            s = s.StripBlank

            Return s
        End Function

        Private Shared Function __getCommonNames(str As String) As String()
            Return LinqAPI.Exec(Of String) <=
 _
                From line As String
                In Strings.Split(WebForm.StripName(str), "<br>")
                Let s = line.StripHTMLTags.StripBlank
                Where Not String.IsNullOrEmpty(s)
                Select s

        End Function
    End Class
End Namespace
