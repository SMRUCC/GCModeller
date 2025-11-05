#Region "Microsoft.VisualBasic::337f3bd7aa5ba66b4b1a3e56ba6c949c, meme_suite\MEME.DocParser\MEME\HtmlParser\LDM\Motif.vb"

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

    '     Class Motif
    ' 
    '         Properties: Evalue, Id, InformationContent, LogLikelihoodRatio, MatchedSites
    '                     MotifId, RegularExpression, RelativeEntropy, Width
    ' 
    '         Function: ToString, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace DocumentFormat.MEME.HTML

    Public Class Motif

        ''' <summary>
        ''' [Motif].[MotifId]，当前的这个属性值可以唯一的标识一个Motif对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Id As Integer
        <XmlAttribute> Public Property Evalue As Double
        <XmlAttribute> Public Property Width As Integer
        <XmlAttribute> Public Property LogLikelihoodRatio As Double
        <XmlAttribute> Public Property InformationContent As Double
        <XmlAttribute> Public Property RelativeEntropy As Double

        Public ReadOnly Property MotifId(ObjectId As String) As String
            Get
                Return String.Join(".", ObjectId, Id)
            End Get
        End Property

        <Column("Signature")> <XmlElement>
        Public Property RegularExpression As String
        Public Property MatchedSites As SiteInfo()

        Public Overrides Function ToString() As String
            Return String.Format("{0}  -->  {1}", Id, RegularExpression)
        End Function

        Const FLAG_EVALUE As String = "<th class=""minorth"">E-value</th>"
        Const FLAG_WIDTH As String = "<th class=""minorth"">Width</th>"
        Const FLAG_LOG_LIKELIHOOD_RATIO As String = "class=""minorth"">Log Likelihood Ratio"
        Const FLAG_INFORMATION_CONTENT As String = "class=""minorth"">Information Content"
        Const FLAG_RELATIVE_ENTROPY As String = "class=""minorth"">Relative Entropy"
        Const FLAG_REGULAR_EXPRESSION As String = "Regular expression <a href=""#regular_expression_doc"" class=""help"">"

        Protected Friend Shared Function TryParse(strData As String) As Motif
            Dim Motif As Motif = New Motif
            Motif.Id = Val(Mid(Regex.Match(strData, """mainh"">Motif \d+</h").Value.GetValue, 6).Trim)
            Dim p As Integer = InStr(strData, FLAG_EVALUE) + Len(FLAG_EVALUE)
            Dim strTemp As String = Mid(strData, p + 1, InStr(p, strData, "</td>") - p)
            Motif.Evalue = Val(strTemp.GetValue)

            p = InStr(strData, FLAG_WIDTH) + Len(FLAG_WIDTH)
            strTemp = Mid(strData, p + 1, InStr(p, strData, "</td>") - p)
            Motif.Width = Val(strTemp.GetValue)

            p = InStr(strData, FLAG_LOG_LIKELIHOOD_RATIO) + Len(FLAG_LOG_LIKELIHOOD_RATIO)
            p = InStr(p, strData, "</td>") + 6
            strTemp = Mid(strData, p, InStr(p, strData, "</td>") - p + 1)
            Motif.LogLikelihoodRatio = Val(strTemp.GetValue)

            p = InStr(strData, FLAG_INFORMATION_CONTENT) + Len(FLAG_INFORMATION_CONTENT)
            p = InStr(p, strData, "</td>") + 6
            strTemp = Mid(strData, p, InStr(p, strData, "</td>") - p + 1)
            Motif.InformationContent = Val(strTemp.GetValue)

            p = InStr(strData, FLAG_RELATIVE_ENTROPY) + Len(FLAG_RELATIVE_ENTROPY)
            p = InStr(p, strData, "</td>") + 6
            strTemp = Mid(strData, p, InStr(p, strData, "</td>") - p + 1)
            Motif.RelativeEntropy = Val(strTemp.GetValue)

            p = InStr(strData, FLAG_REGULAR_EXPRESSION) + Len(FLAG_REGULAR_EXPRESSION)
            p = InStr(p, strData, "<p class=""pad"">") + 14
            strTemp = Mid(strData, p, InStr(p, strData, "</p>") - p + 1)
            Motif.RegularExpression = strTemp.GetValue.TrimNewLine

            strTemp = Mid(strData, InStr(strData, "<h4>Block Diagrams ") + 50)
            strTemp = Mid(strTemp, InStr(strTemp, "<tbody>") + 7)

            Dim Tokens As String() = (From match As Match In Regex.Matches(strTemp, "<tr>.+?</tr>", RegexOptions.Singleline) Select match.Value).ToArray
            Dim Sites As SiteInfo() = New SiteInfo(Tokens.Count - 2) {}
            For i As Integer = 0 To Sites.Count - 1
                Sites(i) = SiteInfo.TryParse(Tokens(i))
            Next
            Motif.MatchedSites = Sites

            Return Motif
        End Function
    End Class
End Namespace
