#Region "Microsoft.VisualBasic::8a1222aab5422a7289fd90730da9eb69, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\ReferenceMap\ReferenceMap.vb"

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

    '   Total Lines: 209
    '    Code Lines: 157
    ' Comment Lines: 26
    '   Blank Lines: 26
    '     File Size: 10.27 KB


    '     Class ReferenceMapData
    ' 
    '         Properties: [Class], [Module], Disease, Name, OtherDBs
    '                     Reactions, ReferenceGenes, References
    ' 
    '         Function: __DBLinksParser, __diseaseParser, __downloadRefRxn, __parserLinks, Download
    '                   (+2 Overloads) GetGeneOrthology, GetPathwayGenes, GetReaction
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.KEGG.DBGET.ReferenceMap

    ''' <summary>
    ''' KEGG数据库之中的参考途径
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlType("KEGG-ReferenceMapData", Namespace:="http://code.google.com/p/genome-in-code/kegg/reference_map_data")>
    Public Class ReferenceMapData : Inherits PathwayBrief

        ''' <summary>
        ''' 直系同源的参考基因
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("ReferenceGeneData")>
        Public Property ReferenceGenes As KeyValuePairObject(Of ListEntry, KeyValuePairObject(Of String, FASTA.FastaSeq)())()
            Get
                If m_geneOrthology.IsNullOrEmpty Then
                    Return New KeyValuePairObject(Of ListEntry, KeyValuePairObject(Of String, FASTA.FastaSeq)())() {}
                End If
                Return m_geneOrthology.Values.ToArray
            End Get
            Set(value As KeyValuePairObject(Of ListEntry, KeyValuePairObject(Of String, FASTA.FastaSeq)())())
                If value.IsNullOrEmpty Then
                    m_geneOrthology = New Dictionary(Of String, KeyValuePairObject(Of ListEntry, KeyValuePairObject(Of String, FASTA.FastaSeq)()))
                Else
                    m_geneOrthology = value.ToDictionary(Function(obj) obj.Key.entryId)
                End If
            End Set
        End Property

        Dim m_geneOrthology As Dictionary(Of String, KeyValuePairObject(Of ListEntry, KeyValuePairObject(Of String, FASTA.FastaSeq)()))
        Dim m_reactions As New Dictionary(Of String, ReferenceReaction)

        Public Property [Class] As String
        Public Property Name As String
        Public Property [Module] As KeyValuePair()
        Public Property Disease As KeyValuePair()
        Public Property OtherDBs As KeyValuePair()
        Public Property References As String()
        Public Property Reactions As ReferenceReaction()
            Get
                If m_reactions.IsNullOrEmpty Then
                    Return New ReferenceMap.ReferenceReaction() {}
                End If
                Return m_reactions.Values.ToArray
            End Get
            Set(value As ReferenceReaction())
                If value.IsNullOrEmpty Then
                    m_reactions = New Dictionary(Of String, ReferenceReaction)
                Else
                    m_reactions = value.ToDictionary(Function(ref) ref.ID)
                End If
            End Set
        End Property

        Public Function GetReaction(ID As String) As ReferenceMap.ReferenceReaction
            If m_reactions.ContainsKey(ID) Then
                Return m_reactions(ID)
            Else
                Return Nothing
            End If
        End Function

        Public Overrides Function GetPathwayGenes() As String()
            Dim LQuery = (From g In Me.ReferenceGenes Select (From nn In g.Value Select nn.Key.Split(CChar(":")).Last)).ToVector
            Return LQuery
        End Function

        Const DBGET_URL As String = "http://www.genome.jp/dbget-bin/www_bget?"
        Const MODULE_PATTERN As String = "<a href=""/kegg-bin/show_module\?M\d+.+?\[PATH:.+?</a>\]"

        Public Function GetGeneOrthology(refRxn As ReferenceMap.ReferenceReaction) As KeyValuePairObject(Of KEGG.WebServices.ListEntry, KeyValuePairObject(Of String, SequenceModel.FASTA.FastaSeq)())()
            Dim LQuery = (From ort In refRxn.SSDBs Where m_geneOrthology.ContainsKey(ort.name) Select m_geneOrthology(ort.name)).ToArray
            Return LQuery
        End Function

        Public Function GetGeneOrthology(KO_ID As String) As KeyValuePairObject(Of KEGG.WebServices.ListEntry, KeyValuePairObject(Of String, SequenceModel.FASTA.FastaSeq)())
            If m_geneOrthology.ContainsKey(KO_ID) Then
                Return m_geneOrthology(KO_ID)
            Else
                Return Nothing
            End If
        End Function

        Public Shared Function Download(ID As String) As ReferenceMap.ReferenceMapData
            Dim Form As New WebForm(resource:=DBGET_URL & ID)
            Dim RefMap As New ReferenceMapData With {.EntryId = ID}

            RefMap.Name = Form("Name").FirstOrDefault
            RefMap.description = Form("Description").FirstOrDefault
            RefMap.Class = Form("Class").FirstOrDefault

            Dim sValue As String

            sValue = Form("Module").FirstOrDefault
            If Not String.IsNullOrEmpty(sValue) Then
                RefMap.Module = LinqAPI.Exec(Of KeyValuePair)() <=
 _
                    From m As Match
                    In Regex.Matches(sValue, MODULE_PATTERN)
                    Let str As String = m.Value
                    Let ModID As String = Regex.Match(str, "M\d+").Value
                    Let descr As String = str.Replace(String.Format("<a href=""/kegg-bin/show_module?{0}"">{0}</a>", ModID), "").Trim
                    Select New KeyValuePair With {
                        .Key = ModID,
                        .Value = Regex.Replace(descr, "<a href=""/kegg-bin/show_pathway?map\d+\+M\d+"">map\d+</a>", ModID)
                    }
            End If

            sValue = Form("Disease").FirstOrDefault
            If Not String.IsNullOrEmpty(sValue) Then
                RefMap.Disease = LinqAPI.Exec(Of KeyValuePair) <= From m As Match
                                                                  In Regex.Matches(sValue, "<a href=""/dbget-bin/www_bget\?ds:H.+?"">H.+?</a> [^<]+")
                                                                  Let str As String = m.Value
                                                                  Select __diseaseParser(str)
            End If

            sValue = Form("Other DBs").FirstOrDefault

            If Not String.IsNullOrEmpty(sValue) Then
                RefMap.OtherDBs = __DBLinksParser(sValue)
            End If

            'Dim ReactionEntryList = KEGG.WebServices.LoadList(Form.AllLinksWidget("KEGG REACTION")) '代谢途径之中的代谢反应的集合
            'Dim RefGeneEntryList = KEGG.WebServices.LoadList(Form.AllLinksWidget("Gene")) '当前的这个代谢途径之中的直系同源基因列表
            'RefMap.ReferenceGenes =
            '    LinqAPI.Exec(Of KeyValuePairObject(Of ListEntry, KeyValuePairObject(Of String, FASTA.FastaSeq)())) <=
            '        From item As ListEntry
            '        In RefGeneEntryList
            '        Select New KeyValuePairObject(Of
            '            ListEntry,
            '            KeyValuePairObject(Of String, FASTA.FastaSeq)()) With {
            '                .Key = item,
            '                .Value = Nothing
            '            }
            'RefMap.Reactions = LinqAPI.Exec(Of ReferenceReaction) <= From Entry As ListEntry
            '                                                         In ReactionEntryList
            '                                                         Select __downloadRefRxn(Entry)

            Return RefMap
        End Function

        Private Shared Function __downloadRefRxn(Entry As WebServices.ListEntry) As ReferenceMap.ReferenceReaction
            Dim path As String = "./Downloads/" & Entry.entryId.NormalizePathString & ".xml"

            If FileIO.FileSystem.FileExists(path) Then
                Dim refData = path.LoadXml(Of ReferenceMap.ReferenceReaction)()
                If Not refData Is Nothing AndAlso Not String.IsNullOrEmpty(refData.Equation) Then
                    Return refData
                End If
            End If

            Dim ref = ReferenceReaction.Download(Entry)
            Call ref.GetXml.SaveTo(path)
            Return ref
        End Function

        Const DB_LINK_PATTERN As String = ".+: (<a href="".+?"">.+?</a>\s*)+"

        Private Shared Function __DBLinksParser(str As String) As KeyValuePair()
            Dim LQuery As KeyValuePair() =
                Regex.Matches(str, DB_LINK_PATTERN) _
                    .ToArray(AddressOf __parserLinks) _
                    .ToVector
            Return LQuery
        End Function

        Private Shared Function __parserLinks(str As String) As KeyValuePair()
            Dim DBName As String = Regex.Match(str, ".+?:").Value
            Dim ID As String() =
                LinqAPI.Exec(Of String) <= From m As Match
                                           In Regex.Matches(str.Replace(DBName, ""), "<a href=.+?</a>")
                                           Select Regex.Match(m.Value, ">.+?</a>").Value
            DBName = DBName.Split(CChar(":")).First
            Dim LQuery As KeyValuePair() =
                LinqAPI.Exec(Of KeyValuePair) <= From sid As String
                                                 In ID
                                                 Select New KeyValuePair With {
                                                     .Key = DBName,
                                                     .Value = sid.GetValue
                                                 }
            Return LQuery
        End Function

        Private Shared Function __diseaseParser(str As String) As KeyValuePair
            Dim dsID As String = Regex.Match(str, "H\d+").Value
            Dim Description As String = str.Replace(String.Format("<a href=""/dbget-bin/www_bget?ds:{0}"">{0}</a>", dsID), "")
            Return New KeyValuePair With {
                .Key = dsID,
                .Value = Description
            }
        End Function
    End Class
End Namespace
