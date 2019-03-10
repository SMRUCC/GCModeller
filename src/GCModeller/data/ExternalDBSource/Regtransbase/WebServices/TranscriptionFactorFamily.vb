﻿#Region "Microsoft.VisualBasic::c95092ab7875d692d69a6d657ad08213, data\ExternalDBSource\Regtransbase\WebServices\TranscriptionFactorFamily.vb"

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

    '     Class RegPreciseTFFamily
    ' 
    '         Properties: Family
    ' 
    '         Function: Download, Export, FindAtRegulog, ToString
    ' 
    '         Sub: Export
    ' 
    '     Class TranscriptionFactorFamily
    ' 
    '         Properties: Family, Genomes, Regulogs, TFBindingSites, TFRegulons
    '                     Url
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class Regulogs
    ' 
    '         Properties: Counts, Description, Logs
    ' 
    '         Function: Export, GetDescription, GetUniqueIds, TrimText
    ' 
    '         Sub: Parse
    '         Class Item
    ' 
    '             Properties: Phylum, Regulog, TFBSs, TFRegulons
    ' 
    '             Function: Parse, ParseLog, ToString
    ' 
    ' 
    ' 
    '     Class Regulator
    ' 
    '         Properties: BiologicalProcess, Effector, Family, RegulationMode, Regulog
    '                     TFBSs
    ' 
    '         Function: [Select], ExportMotifs, GetUniqueId, Parse, ParseLog
    '                   SequenceTrimming
    ' 
    '     Class FastaObject
    ' 
    '         Properties: Bacteria, LocusTag, Name, Position, Score
    '                     SequenceData
    ' 
    '         Function: [New], Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Text
Imports LANS.SystemsBiology.Assembly.SequenceModel

Namespace Regtransbase.WebServices

    Public Class RegPreciseTFFamily
        <Xml.Serialization.XmlElement> Public Property Family As TranscriptionFactorFamily()

        Const ITEM As String = "<tr .+?</tr>"

        Public Shared Function Download(url As String) As RegPreciseTFFamily
            Dim pageContent As String = url.Get_PageContent()
            Dim Items = (From m As Match In Regex.Matches(pageContent, ITEM, RegexOptions.Singleline + RegexOptions.IgnoreCase) Select m.Value).ToArray
            Dim Families = (From item As String In Items Select WebServices.TranscriptionFactorFamily.Parse(item)).ToArray

            Return New RegPreciseTFFamily With {.Family = Families}
        End Function

        Public Sub Export(ExportDir As String)
            Call FileIO.FileSystem.CreateDirectory(ExportDir)

            For Each TFF In Family
                Dim file = String.Format("{0}/{1}.fsa", ExportDir, TFF.Family)
                Call CType(TFF.Regulogs.Export, Assembly.SequenceModel.FASTA.FastaFile).Save(file)
            Next
        End Sub

        Public Shared Function Export(RegPreciseTFFamily As RegPreciseTFFamily) As Assembly.SequenceModel.FASTA.FastaFile
            Dim Fsa As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile = New Assembly.SequenceModel.FASTA.FastaFile
            For Each TFF In RegPreciseTFFamily.Family
                Dim List = TFF.Regulogs.Export
                Call Fsa.AddRange(List)
            Next

            For i As Integer = 0 To Fsa.Count - 1
                Fsa(i).Attributes(0) = String.Format("tfbs_{0} {1}", i, Fsa(i).Attributes(0))
            Next

            Return Fsa
        End Function

        Public Function FindAtRegulog(Keyword As String) As Regulator
            For Each tff In Family
                Dim LQuery = (From item In tff.Regulogs.Logs Where String.Equals(item.Regulog.Key, Keyword) Select item.TFBSs).ToArray
                If Not LQuery.IsNullOrEmpty Then
                    Return LQuery.First
                End If
            Next
            Return Nothing
        End Function

        Public Overrides Function ToString() As String
            Return "http://regprecise.lbl.gov/RegPrecise/collections_tffam.jsp"
        End Function

        Public Shared Widening Operator CType(FilePath As String) As RegPreciseTFFamily
            Return FilePath.LoadXml(Of RegPreciseTFFamily)()
        End Operator
    End Class

    ''' <summary>
    ''' http://regprecise.lbl.gov/RegPrecise/collections_tffam.jsp
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TranscriptionFactorFamily
        <Xml.Serialization.XmlAttribute> Public Property Family As String
        Public Property Url As String
        Public Property Regulogs As Regulogs
        <Xml.Serialization.XmlAttribute> Public Property TFRegulons As String
        <Xml.Serialization.XmlAttribute> Public Property TFBindingSites As String
        <Xml.Serialization.XmlAttribute> Public Property Genomes As String

        Public Overrides Function ToString() As String
            Return Family
        End Function

        Const COLUMN_ITEM As String = "<td.+?</td>"
        Const HREF As String = "href="".+?"""

        Protected Friend Shared Function Parse(strText As String) As TranscriptionFactorFamily
            Dim Items = (From m As Match In Regex.Matches(strText, COLUMN_ITEM, RegexOptions.Singleline + RegexOptions.IgnoreCase) Select m.Value).ToArray
            Dim TFF As TranscriptionFactorFamily = New TranscriptionFactorFamily With {.Regulogs = New Regulogs}
            Dim p As Integer
            Dim Head As String = Items(p.MoveNext)
            TFF.Regulogs.Counts = Regex.Match(Items(p.MoveNext), "[^>]+</").Value
            TFF.TFRegulons = Regex.Match(Items(p.MoveNext), "[^>]+</").Value
            TFF.TFBindingSites = Regex.Match(Items(p.MoveNext), "[^>]+</").Value
            TFF.Genomes = Regex.Match(Items(p.MoveNext), "[^>]+</").Value
            TFF.Url = Regex.Match(Head, HREF, RegexOptions.IgnoreCase).Value
            TFF.Family = Regex.Match(Mid(Head, InStr(Head, "href", CompareMethod.Text)), "[^>]+<a", RegexOptions.Singleline).Value

            TFF.Url = Mid(TFF.Url, 6)
            TFF.Url = "http://regprecise.lbl.gov/RegPrecise/" & Mid(TFF.Url, 2, Len(TFF.Url) - 2)
            TFF.Regulogs.Counts = Mid(TFF.Regulogs.Counts, 1, Len(TFF.Regulogs.Counts) - 2)
            TFF.TFRegulons = Mid(TFF.TFRegulons, 1, Len(TFF.TFRegulons) - 2)
            TFF.TFBindingSites = Mid(TFF.TFBindingSites, 1, Len(TFF.TFBindingSites) - 2)
            TFF.Genomes = Mid(TFF.Genomes, 1, Len(TFF.Genomes) - 2)
            TFF.Family = Mid(TFF.Family, 1, Len(TFF.Family) - 2)
            Call Regulogs.Parse(TFF.Url, TFF.Regulogs)

            Call Threading.Thread.Sleep(1000) '防止服务器的访问压力过大被封IP

            Return TFF
        End Function
    End Class

    Public Class Regulogs

        <Xml.Serialization.XmlElement> Public Property Logs As Item()
        <Xml.Serialization.XmlAttribute> Public Property Counts As String
        Public Property Description As String

        Public Function Export() As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject()
            Dim List As List(Of LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject) =
                New List(Of Assembly.SequenceModel.FASTA.FastaObject)
            For Each logItem In Logs
                Call List.AddRange(logItem.TFBSs.ExportMotifs)
            Next

            Return List.ToArray
        End Function

        ''' <summary>
        ''' {Regulog, {Locus_tag, Locus_tag()}}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUniqueIds() As KeyValuePair(Of String, KeyValuePair(Of String, String())())()
            Dim LQuery = (From item In Logs Select item.TFBSs.GetUniqueId).ToArray
            Return LQuery
        End Function

        Public Class Item
            <Xml.Serialization.XmlAttribute> Public Property Phylum As String
            Public Property Regulog As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair
            <Xml.Serialization.XmlAttribute> Public Property TFRegulons As String
            Public Property TFBSs As Regulator

            Public Overrides Function ToString() As String
                Return Regulog.ToString
            End Function

            Const COLUMNS As String = "<td.+?</td>"

            Protected Friend Shared Function Parse(strText As String) As Item
                Dim Columns = (From m As Match In Regex.Matches(strText, Regulogs.Item.COLUMNS, RegexOptions.Singleline + RegexOptions.IgnoreCase) Select m.Value).ToArray.Skip(1).ToArray
                Dim p As Integer
                Dim item As Item = New Item

                item.Phylum = Regex.Match(Columns(p.MoveNext), ">[^>]+?</").Value
                item.Regulog = ParseLog(Columns(p.MoveNext))
                item.TFRegulons = Regex.Match(Columns(p.MoveNext), ">[^>]+?</").Value

                item.Phylum = TrimText(Mid(item.Phylum, 2, Len(item.Phylum) - 3))
                item.TFRegulons = Mid(item.TFRegulons, 2, Len(item.TFRegulons) - 3)

                Dim TFBSsUrl As String = Columns(p.MoveNext)
                TFBSsUrl = Mid(Regex.Match(TFBSsUrl, "href="".+?""").Value, 7)
                TFBSsUrl = "http://regprecise.lbl.gov/RegPrecise/" & Mid(TFBSsUrl, 1, Len(TFBSsUrl) - 1)

                item.TFBSs = Regulator.Parse(TFBSsUrl)

                Return item
            End Function

            Private Shared Function ParseLog(strText As String) As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair
                Dim Pair As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair = New Assembly.ComponentModel.KeyValuePair
                Pair.Key = Regex.Match(strText, ">[^>]+?</a").Value
                Pair.Value = Regex.Match(strText, "href="".+?""").Value
                Pair.Key = Mid(Pair.Key, 2, Len(Pair.Key) - 4)
                Pair.Value = "http://regprecise.lbl.gov/RegPrecise/" & Mid(Pair.Value, 7, Len(Pair.Value) - 7)

                Return Pair
            End Function
        End Class

        Const ROW_ITEM As String = "<tr>.+?</tr>"

        Protected Friend Shared Sub Parse(url As String, Regulogs As Regulogs)
            Dim pageContent As String = url.Get_PageContent

            Regulogs.Description = GetDescription(pageContent)
            pageContent = Regex.Matches(pageContent, "<tbody>.+?</tbody>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value

            Dim Rows = (From m As Match In Regex.Matches(pageContent, ROW_ITEM, RegexOptions.Singleline + RegexOptions.IgnoreCase) Select m.Value).ToArray

            Regulogs.Logs = (From strText As String In Rows Select Item.Parse(strText)).ToArray
        End Sub

        Const TEXT_DESCRIPTION As String = "<h2 class=""text_description"".+?</h2>"

        Private Shared Function GetDescription(pageContent As String) As String
            Dim strText As String = Regex.Match(pageContent, TEXT_DESCRIPTION, RegexOptions.Singleline).Value

            Const SPAN As String = "<span .+</span>"
            Dim Tokens = Regex.Split(strText, SPAN, RegexOptions.Singleline)
            Tokens(0) = TrimText(Mid(Tokens(0), Len(Regex.Match(Tokens(0), "<h2.+?"">", RegexOptions.Singleline).Value) + 1))
            If Tokens.Count > 1 Then
                Tokens(1) = Regex.Match(Tokens(1), ">.+?</div", RegexOptions.Singleline).Value
                Tokens(1) = TrimText(Mid(Tokens(1), 2, Len(Tokens(1)) - 6))

                Return Tokens(0) & " " & Tokens(1)
            Else
                Return Tokens(0)
            End If
        End Function

        Protected Friend Shared Function TrimText(strText As String) As String
            strText = strText.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")
            strText = Trim(strText.Replace(vbTab, " "))
            Return strText
        End Function
    End Class

    Public Class Regulator
        <Xml.Serialization.XmlAttribute> Public Property Family As String
        <Xml.Serialization.XmlAttribute> Public Property RegulationMode As String
        Public Property BiologicalProcess As String
        Public Property Effector As String
        Public Property Regulog As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair

        Public Property TFBSs As FastaObject()

        ''' <summary>
        ''' 获取一个唯一的物种的编号 {Regulog, {Locus_tag, Locus_tag()}}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUniqueId() As KeyValuePair(Of String, KeyValuePair(Of String, String())())
            Dim Species = (From obj As FastaObject In TFBSs Select obj.Bacteria Distinct).ToArray
            Dim IdList As KeyValuePair(Of String, String())() = (From sp In Species Let Collection As String() = [Select](sp) Select New KeyValuePair(Of String, String())(Collection.First, Collection)).ToArray

            Return New KeyValuePair(Of String, KeyValuePair(Of String, String())())(Regulog.Key, IdList)
        End Function

        ''' <summary>
        ''' 根据物种编号筛选出基因号
        ''' </summary>
        ''' <param name="SpeciesId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function [Select](SpeciesId As String) As String()
            Dim LQuery = (From rec In TFBSs Where String.Equals(rec.Bacteria, SpeciesId) Select rec.LocusTag).ToArray
            Return LQuery
        End Function

        Public Function ExportMotifs() As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject()
            Dim LQuery = (From FastaObject As WebServices.FastaObject
                          In TFBSs
                          Select New LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject With {
                              .SequenceData = SequenceTrimming(FastaObject),
                              .Attributes = New String() {String.Format("[gene={0}] [family={1}] [regulog={2}]", FastaObject.LocusTag, Family, Regulog.Key)}}).ToArray
            Return LQuery
        End Function

        Public Shared Function SequenceTrimming(FastaObject As WebServices.FastaObject) As String
            Dim Tokens = (From mh As Match In Regex.Matches(FastaObject.SequenceData, ".\(\d+\)", RegexOptions.Singleline) Select mh.Value).ToArray
            Dim sBuilder As StringBuilder = New StringBuilder(FastaObject.SequenceData)
            For Each Token As String In Tokens
                Dim strTemp As String = New String(Token.First, Convert.ToInt32(Regex.Match(Token, "\d+").Value))
                Call sBuilder.Replace(Token, strTemp)
            Next

            Return sBuilder.ToString
        End Function

        Public Shared Function Parse(url As String) As Regulator
            Dim pageContent As String = url.Get_PageContent
            Dim ExportDownloadUrl As String = Regex.Match(pageContent, "<a href=""[^>]+?""><b>DOWNLOAD</b></a>", RegexOptions.Singleline).Value
            ExportDownloadUrl = Regex.Match(ExportDownloadUrl, "href="".+?""", RegexOptions.IgnoreCase).Value
            ExportDownloadUrl = "http://regprecise.lbl.gov/RegPrecise/" & Mid(ExportDownloadUrl, 7, Len(ExportDownloadUrl) - 7)

            Dim Regulator As Regulator = New Regulator
            Regulator.TFBSs = FastaObject.Parse(ExportDownloadUrl)
            pageContent = Mid(pageContent, InStr(pageContent, "<caption class=""tbl_caption"">Properties</caption>", CompareMethod.Text) + 50)
            pageContent = Regex.Matches(pageContent, "<tbody>.+?</tbody>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(0).Value
            Dim Tokens = (From m As Match In Regex.Matches(pageContent, "<tr>.+?</tr>", RegexOptions.Singleline + RegexOptions.IgnoreCase) Select m.Value).ToArray.Skip(1).ToArray
            Dim p As Integer
            Regulator.Family = Regex.Matches(Tokens(p.MoveNext), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value
            Regulator.RegulationMode = Regex.Matches(Tokens(p.MoveNext), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value
            Regulator.BiologicalProcess = Regex.Matches(Tokens(p.MoveNext), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value
            Regulator.Effector = Regex.Matches(Tokens(p.MoveNext), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value
            Regulator.Regulog = ParseLog(Regex.Matches(Tokens(p.MoveNext), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value)

            Dim GetValue = Function(str As String) As String
                               str = Mid(str, 5, Len(str) - 9)
                               Return str
                           End Function

            Regulator.Family = GetValue(Regulator.Family)
            Regulator.RegulationMode = GetValue(Regulator.RegulationMode)
            Regulator.BiologicalProcess = GetValue(Regulator.BiologicalProcess)
            Regulator.Effector = GetValue(Regulator.Effector)

            Return Regulator
        End Function

        Private Shared Function ParseLog(str As String) As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair
            Dim pair As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair = New Assembly.ComponentModel.KeyValuePair
            pair.Key = Regex.Match(str, ">[^>]+?</a>", RegexOptions.Singleline).Value
            pair.Value = Regex.Match(str, "href="".+?""", RegexOptions.Singleline).Value
            pair.Key = Regulogs.TrimText(Mid(pair.Key, 2, Len(pair.Key) - 5))
            pair.Value = "http://regprecise.lbl.gov/RegPrecise/" & Mid(pair.Value, 7, Len(pair.Value) - 7)

            Return pair
        End Function
    End Class

    Public Class FastaObject
        Implements LANS.SystemsBiology.Assembly.SequenceModel.ISequenceModel.ISequence

        <Xml.Serialization.XmlAttribute> Public Property LocusTag As String
        <Xml.Serialization.XmlAttribute> Public Property Name As String
        <Xml.Serialization.XmlAttribute> Public Property Position As Integer
        <Xml.Serialization.XmlAttribute> Public Property Score As Double

        <Xml.Serialization.XmlElement("Sequence")>
        Public Property SequenceData As String Implements ISequenceModel.ISequence.SequenceData
        Public Property Bacteria As String

        Public Shared Function Parse(url As String) As FastaObject()
            Dim Text As String = url.Get_PageContent
            Dim FASTA As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile = LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile.Parse(strText:=Text)
            Dim LQuery = (From fsa In FASTA Select FastaObject.[New](fsa)).ToArray
            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Return String.Format(">{0} : {1}", LocusTag, SequenceData)
        End Function

        Const REAL As String = "-?\d+(\.\d+)?"

        Protected Friend Shared Function [New](DownloadedFastaObject As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject) As FastaObject
            Dim Title As String = DownloadedFastaObject.Title
            Dim FastaObject As FastaObject = New FastaObject
            Dim Score As String = Regex.Match(Title, "Score=" & REAL, RegexOptions.IgnoreCase).Value
            Dim Position As String = Regex.Match(Title, "Pos=" & REAL, RegexOptions.IgnoreCase).Value
            Dim Bacateria As String = Regex.Match(Title, "\[.+\]").Value
           
            FastaObject.SequenceData = DownloadedFastaObject.SequenceData
            FastaObject.Bacteria = Bacateria
            FastaObject.Bacteria = Mid(FastaObject.Bacteria, 2, Len(FastaObject.Bacteria) - 2)
            FastaObject.Position = Val(Position.Split("=").Last)
            FastaObject.Score = Val(Score.Split("=").Last)

            Dim LocusTag As String = Title.Replace(Score, "").Replace(Position, "").Replace(Bacateria, "").Trim
            FastaObject.Name = Regex.Match(LocusTag, "\(.+?\)").Value
            FastaObject.Name = If(Not String.IsNullOrEmpty(FastaObject.Name), Mid(FastaObject.Name, 2, Len(FastaObject.Name) - 2).Trim, "")
            LocusTag = Regex.Replace(LocusTag, "\(.+?\)", "")
            FastaObject.LocusTag = LocusTag.Replace(">", "").Trim

            Return FastaObject
        End Function
    End Class
End Namespace
