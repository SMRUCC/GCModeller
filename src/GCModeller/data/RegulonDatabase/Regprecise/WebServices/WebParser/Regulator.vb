#Region "Microsoft.VisualBasic::0a199b3c028596594133716fb014fed6, ..\GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\Regulator.vb"

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
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.ComponentModel

Namespace Regprecise

    Public Class Regulator : Implements IReadOnlyId

        Public Enum Types
            NotSpecific = -1
            ''' <summary>
            '''
            ''' </summary>
            ''' <remarks></remarks>
            TF
            ''' <summary>
            ''' RNA regulatory element
            ''' </summary>
            ''' <remarks></remarks>
            RNA
        End Enum

        <XmlAttribute> Public Property Type As Types
        <XmlElement> Public Property Regulator As KeyValuePair
        <XmlElement> Public Property Effector As String
        <XmlElement> Public Property Pathway As String
        <XmlElement> Public Property LocusTag As KeyValuePair
        <XmlAttribute> Public Property Family As String
        <XmlAttribute> Public Property RegulationMode As String
        <XmlElement> Public Property BiologicalProcess As String
        <XmlElement> Public Property Regulog As KeyValuePair
        <XmlArray> Public Property RegulatorySites As Regtransbase.WebServices.FastaObject()
        ''' <summary>
        ''' 被这个调控因子所调控的基因，按照操纵子进行分组，这个适用于推断Regulon的
        ''' </summary>
        ''' <returns></returns>
        <XmlArray> Public Property lstOperon As Operon()
        <XmlElement> Public Property Regulates As RegulatedGene()
        Public Property SiteMore As String

        ''' <summary>
        ''' 该Regprecise调控因子的基因号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property LocusId As String Implements IReadOnlyId.Identity
            Get
                Return LocusTag.Key
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", Type.ToString, Regulator.ToString)
        End Function

        Public Function GetMotifSite(GeneId As String, MotifPosition As Integer) As Regtransbase.WebServices.FastaObject
            Dim LQuery = (From fa As Regtransbase.WebServices.FastaObject In RegulatorySites
                          Where String.Equals(GeneId, fa.LocusTag) AndAlso
                              fa.Position = MotifPosition
                          Select fa).FirstOrDefault
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="trace">locus_tag:position</param>
        ''' <returns></returns>
        Public Function GetMotifSite(trace As String) As Regtransbase.WebServices.FastaObject
            Dim Tokens As String() = trace.Split(":"c)
            Return GetMotifSite(Tokens(Scan0), CInt(Val(Tokens(1))))
        End Function

        ''' <summary>
        ''' 这个函数会自动移除一些表示NNNN的特殊符号
        ''' </summary>
        ''' <returns></returns>
        Public Function ExportMotifs() As FASTA.FastaToken()
            Dim LQuery As FASTA.FastaToken() =
                LinqAPI.Exec(Of FASTA.FastaToken) <=
                    From FastaObject As Regtransbase.WebServices.FastaObject
                    In RegulatorySites
                    Let t As String = $"[gene={FastaObject.LocusTag}:{FastaObject.Position}] [family={Family}] [regulog={Regulog.Key}]"
                    Let attrs = New String() {t}
                    Let seq As String = Regtransbase.WebServices.Regulator.SequenceTrimming(FastaObject)
                    Let fa As FASTA.FastaToken =
                        New FASTA.FastaToken With {
                            .SequenceData = seq,
                            .Attributes = attrs
                        }
                    Select fa
            Return LQuery
        End Function

        Public Shared Function CreateObject(strData As String) As Regulator
            Dim Items As String() = Regex.Matches(strData, "<td.+?</td>").ToArray
            Dim Regulator As New Regulator
            Dim p As Integer

            Regulator.Type = If(InStr(Items(p.MoveNext), " RNA "), Regulator.Types.RNA, Regulator.Types.TF)
            Dim EntryData As String = Regex.Match(Items(p.MoveNext), "href="".+?"">.+?</a>").Value
            Dim url As String = "http://regprecise.lbl.gov/RegPrecise/" & EntryData.href
            Regulator.Regulator = KeyValuePair.CreateObject(WebAPI.GetsId(EntryData), url)
            Regulator.Effector = __getTagValue(Items(p.MoveNext))
            Regulator.Pathway = __getTagValue(Items(p.MoveNext))

            Return More(Regulator)
        End Function

        Private Shared Function More(Regulator As Regulator) As Regulator
            Dim html As String = Regulator.Regulator.Value.GET
            html = Regex.Match(html, "<table class=""proptbl"">.+?</table>", RegexOptions.Singleline).Value
            Dim Properties As String() = Regex.Matches(html, "<tr>.+?</tr>", RegexICSng).ToArray
            Dim p As Integer = 1

            Regulator.SiteMore = Regex.Match(html, "\[<a href="".+?"">see more</a>\]", RegexOptions.IgnoreCase).Value
            Regulator.SiteMore = "http://regprecise.lbl.gov/RegPrecise/" & Regulator.SiteMore.href

            If Regulator.Type = Types.TF Then
                Dim LocusTag As String = Regex.Match(Properties(p.MoveNext), "href="".+?"">.+?</a>", RegexOptions.Singleline).Value
                Regulator.LocusTag = KeyValuePair.CreateObject(WebAPI.GetsId(LocusTag), LocusTag.href)
                Regulator.Family = __getTagValue_td(Properties(p.MoveNext).Replace("<td>Regulator family:</td>", ""))
            Else
                Dim Name As String = Regex.Matches(Properties(p.MoveNext), "<td>.+?</td>", RegexICSng).ToArray.Last
                Name = Mid(Name, 5)
                Name = Mid(Name, 1, Len(Name) - 5)
                Regulator.LocusTag = KeyValuePair.CreateObject(Name, "")
                Regulator.Family = Regex.Match(html, "<td class=""[^""]+?"">RFAM:</td>[^<]+?<td>.+?</td>", RegexOptions.Singleline).Value
                Regulator.Family = __getTagValue_td(Regulator.Family)
            End If

            Regulator.RegulationMode = __getTagValue_td(Properties(p.MoveNext))
            Regulator.BiologicalProcess = __getTagValue_td(Properties(p.MoveNext))

            Dim RegulogEntry As String = Regex.Match(Properties(p + 1), "href="".+?"">.+?</a>", RegexOptions.Singleline).Value
            Dim url As String = "http://regprecise.lbl.gov/RegPrecise/" & RegulogEntry.href
            Regulator.Regulog = KeyValuePair.CreateObject(WebAPI.GetsId(RegulogEntry).TrimA("").Replace(vbTab, "").Trim, url)

            Dim exportServletLnks As String() = __exportServlet(html)
            Regulator.lstOperon = Operon.OperonParser(html) 'WebAPI.GetRegulates(url:=exportServletLnks.Get(Scan0))
            Regulator.RegulatorySites = Regtransbase.WebServices.FastaObject.Parse(url:=exportServletLnks.Get(1))

            Return Regulator
        End Function

        Private Shared Function __getTagValue_td(strData As String) As String
            strData = Regex.Match(strData, "<td>.+?</td>", RegexOptions.Singleline).Value
            If String.IsNullOrEmpty(Trim(strData)) Then
                Return ""
            End If
            strData = Mid(strData, 5)
            strData = Mid(strData, 1, Len(strData) - 5)
            Return strData
        End Function

        Private Shared Function __exportServlet(pageContent As String) As String()
            Dim url As String = Regex.Match(pageContent, "<table class=""tblexport"">.+?</table>", RegexOptions.Singleline).Value
            Dim links As String() = (From match As Match In Regex.Matches(url, "<tr>.+?</tr>", RegexOptions.Singleline + RegexOptions.IgnoreCase) Select match.Value).ToArray
            links = links.ToArray(Function(s) Regex.Match(s, "href="".+?""><b>DOWNLOAD</b>").Value)
            links = links.ToArray(Function(s) "http://regprecise.lbl.gov/RegPrecise/" & s.href)
            Return links
        End Function

        Private Shared Function __getTagValue(s As String) As String
            s = Regex.Match(s, """>.+?</td>").Value
            s = Mid(s, 3)
            s = Mid(s, 1, Len(s) - 5)
            Return Trim(s)
        End Function
    End Class
End Namespace
