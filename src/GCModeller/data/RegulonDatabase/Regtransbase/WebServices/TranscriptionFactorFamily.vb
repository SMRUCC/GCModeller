#Region "Microsoft.VisualBasic::faa7cb819cd9f23daa856f9a1dc56148, data\RegulonDatabase\Regtransbase\WebServices\TranscriptionFactorFamily.vb"

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
    '         Function: [Select], ExportMotifs, GetUniqueId, GetValue, Parse
    '                   ParseLog, (+2 Overloads) SequenceTrimming
    ' 
    '     Class MotifFasta
    ' 
    '         Properties: bacteria, Headers, locus_tag, name, position
    '                     score, SequenceData, Title, UniqueId
    ' 
    '         Function: [New], GetSequenceData, Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Regtransbase.WebServices

    Public Class RegPreciseTFFamily
        <XmlElement> Public Property Family As TranscriptionFactorFamily()

        Const ITEM As String = "<tr .+?</tr>"

        Public Shared Function Download(url As String) As RegPreciseTFFamily
            Dim pageContent As String = url.GET()
            Dim Items = (From m As Match In Regex.Matches(pageContent, ITEM, RegexOptions.Singleline + RegexOptions.IgnoreCase) Select m.Value).ToArray
            Dim Families = (From item As String In Items Select WebServices.TranscriptionFactorFamily.Parse(item)).ToArray

            Return New RegPreciseTFFamily With {.Family = Families}
        End Function

        Public Sub Export(ExportDir As String)
            Call FileIO.FileSystem.CreateDirectory(ExportDir)

            For Each TFF In Family
                Dim file = String.Format("{0}/{1}.fsa", ExportDir, TFF.Family)
                Call CType(TFF.Regulogs.Export, SequenceModel.FASTA.FastaFile).Save(file)
            Next
        End Sub

        Public Shared Function Export(RegPreciseTFFamily As RegPreciseTFFamily) As SequenceModel.FASTA.FastaFile
            Dim Fsa As SMRUCC.genomics.SequenceModel.FASTA.FastaFile = New SequenceModel.FASTA.FastaFile
            For Each TFF In RegPreciseTFFamily.Family
                Dim List = TFF.Regulogs.Export
                Call Fsa.AddRange(List)
            Next

            For i As Integer = 0 To Fsa.Count - 1
                Fsa(i).Headers(0) = String.Format("tfbs_{0} {1}", i, Fsa(i).Headers(0))
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
        <XmlAttribute> Public Property Family As String
        Public Property Url As String
        Public Property Regulogs As Regulogs
        <XmlAttribute> Public Property TFRegulons As String
        <XmlAttribute> Public Property TFBindingSites As String
        <XmlAttribute> Public Property Genomes As String

        Public Overrides Function ToString() As String
            Return Family
        End Function

        Const COLUMN_ITEM As String = "<td.+?</td>"
        Const HREF As String = "href="".+?"""

        Protected Friend Shared Function Parse(strText As String) As TranscriptionFactorFamily
            Dim Items = (From m As Match In Regex.Matches(strText, COLUMN_ITEM, RegexOptions.Singleline + RegexOptions.IgnoreCase) Select m.Value).ToArray
            Dim TFF As TranscriptionFactorFamily = New TranscriptionFactorFamily With {.Regulogs = New Regulogs}
            Dim p As i32 = Scan0
            Dim Head As String = Items(++p)
            TFF.Regulogs.Counts = Regex.Match(Items(++p), "[^>]+</").Value
            TFF.TFRegulons = Regex.Match(Items(++p), "[^>]+</").Value
            TFF.TFBindingSites = Regex.Match(Items(++p), "[^>]+</").Value
            TFF.Genomes = Regex.Match(Items(++p), "[^>]+</").Value
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

        <XmlElement> Public Property Logs As Item()
        <XmlAttribute> Public Property Counts As String
        Public Property Description As String

        Public Function Export() As FASTA.FastaSeq()
            Return Logs.Select(Function(x) x.TFBSs.ExportMotifs).ToVector
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

            <XmlAttribute> Public Property Phylum As String
            Public Property Regulog As KeyValuePair
            <XmlAttribute> Public Property TFRegulons As String
            Public Property TFBSs As Regulator

            Public Overrides Function ToString() As String
                Return Regulog.ToString
            End Function

            Const COLUMNS As String = "<td.+?</td>"

            Protected Friend Shared Function Parse(strText As String) As Item
                Dim Columns = (From m As Match In Regex.Matches(strText, Regulogs.Item.COLUMNS, RegexOptions.Singleline + RegexOptions.IgnoreCase) Select m.Value).ToArray.Skip(1).ToArray
                Dim p As i32 = Scan0
                Dim item As Item = New Item

                item.Phylum = Regex.Match(Columns(++p), ">[^>]+?</").Value
                item.Regulog = ParseLog(Columns(++p))
                item.TFRegulons = Regex.Match(Columns(++p), ">[^>]+?</").Value

                item.Phylum = TrimText(Mid(item.Phylum, 2, Len(item.Phylum) - 3))
                item.TFRegulons = Mid(item.TFRegulons, 2, Len(item.TFRegulons) - 3)

                Dim TFBSsUrl As String = Columns(++p)
                TFBSsUrl = Mid(Regex.Match(TFBSsUrl, "href="".+?""").Value, 7)
                TFBSsUrl = "http://regprecise.lbl.gov/RegPrecise/" & Mid(TFBSsUrl, 1, Len(TFBSsUrl) - 1)

                item.TFBSs = Regulator.Parse(TFBSsUrl)

                Return item
            End Function

            Private Shared Function ParseLog(strText As String) As KeyValuePair
                Dim Pair As New KeyValuePair
                Pair.Key = Regex.Match(strText, ">[^>]+?</a").Value
                Pair.Value = Regex.Match(strText, "href="".+?""").Value
                Pair.Key = Mid(Pair.Key, 2, Len(Pair.Key) - 4)
                Pair.Value = "http://regprecise.lbl.gov/RegPrecise/" & Mid(Pair.Value, 7, Len(Pair.Value) - 7)

                Return Pair
            End Function
        End Class

        Const ROW_ITEM As String = "<tr>.+?</tr>"

        Protected Friend Shared Sub Parse(url As String, Regulogs As Regulogs)
            Dim pageContent As String = url.GET

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
            strText = Strings.Trim(strText.Replace(vbTab, " "))
            Return strText
        End Function
    End Class

    Public Class Regulator
        <XmlAttribute> Public Property Family As String
        <XmlAttribute> Public Property RegulationMode As String
        Public Property BiologicalProcess As String
        Public Property Effector As String
        Public Property Regulog As KeyValuePair

        Public Property TFBSs As MotifFasta()

        ''' <summary>
        ''' 获取一个唯一的物种的编号 {Regulog, {Locus_tag, Locus_tag()}}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUniqueId() As KeyValuePair(Of String, KeyValuePair(Of String, String())())
            Dim Species = (From obj As MotifFasta In TFBSs Select obj.bacteria Distinct).ToArray
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
            Dim LQuery = (From rec In TFBSs Where String.Equals(rec.bacteria, SpeciesId) Select rec.locus_tag).ToArray
            Return LQuery
        End Function

        Public Function ExportMotifs() As FastaSeq()
            Dim LQuery = (From fa As MotifFasta
                          In TFBSs
                          Let header = String.Format("[gene={0}] [family={1}] [regulog={2}]", fa.locus_tag, Family, Regulog.Key)
                          Select New FastaSeq With {
                              .SequenceData = SequenceTrimming(fa),
                              .Headers = {header}
                          }).ToArray
            Return LQuery
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function SequenceTrimming(FastaObject As WebServices.MotifFasta) As String
            Return SequenceTrimming(FastaObject.SequenceData.Replace("-", "N"))
        End Function

        ''' <summary>
        ''' 将序列之中的长度缩写进行拓展为实际的序列
        ''' </summary>
        ''' <param name="sequence"></param>
        ''' <returns></returns>
        Public Shared Function SequenceTrimming(sequence As String) As String
            Dim tokens$() = Regex.Matches(sequence, ".\(\d+\)", RegexOptions.Singleline).ToArray
            Dim sBuilder As New StringBuilder(sequence)

            For Each token As String In tokens
                Dim extends As New String(token.First, Convert.ToInt32(Regex.Match(token, "\d+").Value))

                Call $"  {token}  --> {extends}".Warning
                Call sBuilder.Replace(token, extends)
            Next

            Return sBuilder.ToString
        End Function

        Public Shared Function Parse(url As String) As Regulator
            Dim pageContent As String = url.GET
            Dim ExportDownloadUrl As String = Regex.Match(pageContent, "<a href=""[^>]+?""><b>DOWNLOAD</b></a>", RegexOptions.Singleline).Value
            ExportDownloadUrl = Regex.Match(ExportDownloadUrl, "href="".+?""", RegexOptions.IgnoreCase).Value
            ExportDownloadUrl = "http://regprecise.lbl.gov/RegPrecise/" & Mid(ExportDownloadUrl, 7, Len(ExportDownloadUrl) - 7)

            Dim Regulator As Regulator = New Regulator
            Regulator.TFBSs = MotifFasta.Parse(ExportDownloadUrl)
            pageContent = Mid(pageContent, InStr(pageContent, "<caption class=""tbl_caption"">Properties</caption>", CompareMethod.Text) + 50)
            pageContent = Regex.Matches(pageContent, "<tbody>.+?</tbody>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(0).Value
            Dim Tokens = (From m As Match In Regex.Matches(pageContent, "<tr>.+?</tr>", RegexOptions.Singleline + RegexOptions.IgnoreCase) Select m.Value).ToArray.Skip(1).ToArray
            Dim p As i32 = Scan0
            Regulator.Family = Regex.Matches(Tokens(++p), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value
            Regulator.RegulationMode = Regex.Matches(Tokens(++p), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value
            Regulator.BiologicalProcess = Regex.Matches(Tokens(++p), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value
            Regulator.Effector = Regex.Matches(Tokens(++p), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value
            Regulator.Regulog = ParseLog(Regex.Matches(Tokens(++p), "<td.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase).Item(1).Value)

            Regulator.Family = GetValue(Regulator.Family)
            Regulator.RegulationMode = GetValue(Regulator.RegulationMode)
            Regulator.BiologicalProcess = GetValue(Regulator.BiologicalProcess)
            Regulator.Effector = GetValue(Regulator.Effector)

            Return Regulator
        End Function

        Private Shared Function GetValue(str As String) As String
            str = Mid(str, 5, Len(str) - 9)
            Return str
        End Function

        Private Shared Function ParseLog(str As String) As KeyValuePair
            Dim pair As New KeyValuePair
            pair.Key = Regex.Match(str, ">[^>]+?</a>", RegexOptions.Singleline).Value
            pair.Value = Regex.Match(str, "href="".+?""", RegexOptions.Singleline).Value
            pair.Key = Regulogs.TrimText(Mid(pair.Key, 2, Len(pair.Key) - 5))
            pair.Value = "http://regprecise.lbl.gov/RegPrecise/" & Mid(pair.Value, 7, Len(pair.Value) - 7)

            Return pair
        End Function
    End Class

    ''' <summary>
    ''' The fasta sequence model of the regulated motif site in Regtransbase/Regprecise
    ''' </summary>
    <XmlType("motifsite")> Public Class MotifFasta
        Implements IReadOnlyId
        Implements IPolymerSequenceModel
        Implements IFastaProvider

        <XmlAttribute> Public Property locus_tag As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property position As Integer
        <XmlAttribute> Public Property score As Double
        <XmlAttribute> Public Property bacteria As String

        Public Const xmlns$ = "http://xsd.gcmodeller.org/models/motifsite/"

        <XmlText>
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        <XmlIgnore>
        Public ReadOnly Property UniqueId As String Implements IReadOnlyId.Identity
            Get
                Return String.Format("{0}:{1}", locus_tag, position)
            End Get
        End Property

        <XmlIgnore>
        Public ReadOnly Property Title As String Implements IFastaProvider.Title
            Get
                Return $"{UniqueId} {bacteria}"
            End Get
        End Property

        <XmlIgnore>
        Protected ReadOnly Property Headers As String()
            Get
                Return {UniqueId, bacteria}
            End Get
        End Property

        Public Shared Function Parse(url As String) As MotifFasta()
            Dim text As String = url.GET
            Dim FASTA As FastaFile = FastaFile.ParseDocument(doc:=text)
            Dim LQuery = (From fsa As FastaSeq In FASTA Select MotifFasta.[New](fsa)).ToArray
            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Return String.Format(">{0} : {1}", locus_tag, SequenceData)
        End Function

        Const REAL As String = "-?\d+(\.\d+)?"

        Protected Friend Shared Function [New](DownloadedFastaObject As FastaSeq) As MotifFasta
            Dim Title As String = DownloadedFastaObject.Title
            Dim FastaObject As MotifFasta = New MotifFasta
            Dim Score As String = Regex.Match(Title, "Score=" & REAL, RegexOptions.IgnoreCase).Value
            Dim Position As String = Regex.Match(Title, "Pos=" & REAL, RegexOptions.IgnoreCase).Value
            Dim Bacateria As String = Regex.Match(Title, "\[.+\]").Value

            FastaObject.SequenceData = DownloadedFastaObject.SequenceData
            FastaObject.bacteria = Bacateria
            FastaObject.bacteria = Mid(FastaObject.bacteria, 2, Len(FastaObject.bacteria) - 2)
            FastaObject.position = Val(Position.Split(CChar("=")).Last)
            FastaObject.score = Val(Score.Split(CChar("=")).Last)

            Dim LocusTag As String = Title.Replace(Score, "").Replace(Position, "").Replace(Bacateria, "").Trim
            FastaObject.name = Regex.Match(LocusTag, "\(.+?\)").Value
            FastaObject.name = If(Not String.IsNullOrEmpty(FastaObject.name), Mid(FastaObject.name, 2, Len(FastaObject.name) - 2).Trim, "")
            LocusTag = Regex.Replace(LocusTag, "\(.+?\)", "")
            FastaObject.locus_tag = LocusTag.Replace(">", "").Trim

            Return FastaObject
        End Function

        Private Function GetSequenceData() As String Implements ISequenceProvider.GetSequenceData
            Return SequenceData
        End Function
    End Class
End Namespace
