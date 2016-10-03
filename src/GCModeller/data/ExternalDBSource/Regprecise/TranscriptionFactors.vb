#Region "Microsoft.VisualBasic::6a488f97fa8a489a9f3999f3b466543d, ..\GCModeller\data\ExternalDBSource\Regprecise\TranscriptionFactors.vb"

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
Imports System.Text

Imports Microsoft.VisualBasic.DataVisualization.DocumentFormat.Csv.Reflection

Namespace Regprecise

    ''' <summary>
    ''' [Regprecise database] [Collections of regulogs classified by transcription factors]
    ''' Each transcription factor collection organizes all reconstructed regulogs for a given set of orthologous 
    ''' regulators across different taxonomic groups of microorganisms. These collections represent regulons for 
    ''' a selected subset of transcription factors. The collections include both widespread transcription factors 
    ''' such as NrdR, LexA, and Zur, that are present in more than 25 diverse taxonomic groups of Bacteria, and 
    ''' narrowly distributed transcription factors such as Irr and PurR. The TF regulon collections are valuable 
    ''' for comparative and evolutionary analysis of TF binding site motifs and regulon content for orthologous 
    ''' transcription factors.
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Xml.Serialization.XmlRoot("Regprecise.TranscriptionFactors", namespace:="http://regprecise.lbl.gov/RegPrecise/")>
    Public Class TranscriptionFactors : Inherits Global.Microsoft.VisualBasic.ComponentModel.ITextFile

        Public Const WEB_REQUEST_ENTRY_URL As String = "http://regprecise.lbl.gov/RegPrecise/browse_genomes.jsp"

        <Xml.Serialization.XmlElement> Public Property BacteriaGenomes As BacteriaGenome()
        <Xml.Serialization.XmlAttribute("Database.UpdateTime")>
        Public Property DownloadTime As String

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            If String.IsNullOrEmpty(FilePath) Then
                FilePath = MyBase.FilePath
            End If

            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function

        Public Const TABLE_REGEX As String = "<table class=""stattbl"">.+</table>"

        Public Function GetRegulatorId(GeneId As String, MotifPosition As Integer) As String
            For Each Genome In BacteriaGenomes
                Dim LQuery = (From Regulator In Genome.Regulons.Regulators Where Not Regulator.GetMotifSite(GeneId, MotifPosition) Is Nothing Select Regulator).ToArray
                If Not LQuery.IsNullOrEmpty Then
                    Return LQuery.First.LocusTag.Key
                End If
            Next

            Return ""
        End Function

        Public Shared Function Download(Optional Export As String = "") As TranscriptionFactors
            Call Console.WriteLine("Start to fetch regprecise genome information....")

            Dim PageContent As String = Regex.Match(TranscriptionFactors.WEB_REQUEST_ENTRY_URL.Get_PageContent, TABLE_REGEX, RegexOptions.Singleline).Value
            Dim Items As String() = (From matched As Match In Regex.Matches(PageContent, "<tr .+?</tr>", RegexOptions.Singleline + RegexOptions.IgnoreCase) Select matched.Value).ToArray
            Dim BacteriaGenomes As BacteriaGenome() = New BacteriaGenome(Items.Count - 1) {}

            If String.IsNullOrEmpty(Export) Then
                Export = My.Computer.FileSystem.SpecialDirectories.Temp
            End If

            Call Console.WriteLine("{0} bacteria genome are ready to download!", BacteriaGenomes.Count)

            For i As Integer = 0 To BacteriaGenomes.Count - 1
                Dim strData As String = Regex.Match(Items(i), "href="".+?"">.+?</a>").Value
                Dim Entry As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair = LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair.CreateObject(GetId(strData), "http://regprecise.lbl.gov/RegPrecise/" & strData.Get_href)
                Dim SavePath As String = String.Format("{0}/{1}.xml", Export, Entry.Key.NormalizePathString)

                If FileIO.FileSystem.FileExists(SavePath) AndAlso FileIO.FileSystem.GetFileInfo(SavePath).Length > 1024 Then
                    Try
                        Dim ExistsData = SavePath.LoadXml(Of BacteriaGenome)()
                        '有些数据由于解析的缘故，是错误的，故而在这里需要进行校验：后续的过程之中可以将这部分的校验代码进行删除
                        Dim LQuery = (From item In ExistsData.Regulons.Regulators Where (From site In item.RegulatorySites Where site.Position = 0 Select 1).ToArray.IsNullOrEmpty = False Select 1).ToArray
                        If Not LQuery.IsNullOrEmpty Then
                            GoTo RE_DOWNLOAD       '这里的数据是错误的，需要进行重新下载
                        End If

                        BacteriaGenomes(i) = ExistsData
                        Call Console.WriteLine("Downloads process ............................................................. {0}% ({1}/{2})", 100 * i / BacteriaGenomes.Count, i, BacteriaGenomes.Count)
                        Continue For
                    Catch ex As Exception
                        GoTo RE_DOWNLOAD
                    End Try
                End If

RE_DOWNLOAD:    Dim BacteriaGenome As BacteriaGenome = New BacteriaGenome With {.BacteriaGenome = Entry}
                BacteriaGenome.Regulons = BacteriaGenome.Download(Entry.Value)
                BacteriaGenomes(i) = BacteriaGenome

                Call BacteriaGenome.GetXml.SaveTo(SavePath)
                Call Console.WriteLine("Downloads process ............................................................. {0}% ({1}/{2})", 100 * i / BacteriaGenomes.Count, i, BacteriaGenomes.Count)
            Next

            Return New TranscriptionFactors With {.BacteriaGenomes = BacteriaGenomes, .DownloadTime = Now.ToShortDateString}
        End Function

        Public Shared Function Load(File As String) As TranscriptionFactors
            Dim XmlFile = File.LoadXml(Of TranscriptionFactors)()
            XmlFile.FilePath = File
            Return XmlFile
        End Function

        Public Shared Widening Operator CType(FilePath As String) As TranscriptionFactors
            Return TranscriptionFactors.Load(FilePath)
        End Operator

        Protected Friend Shared Function GetId(strData As String) As String
            Dim Id As String = Mid(strData, InStr(strData, """>") + 2)
            If String.IsNullOrEmpty(Trim(Id)) Then
                Return ""
            Else
                Id = Mid(Id, 1, Len(Id) - 4)
                Return Id
            End If
        End Function

        Public Shared Function ExportTFBSInfo(RegpreciseDatabase As TranscriptionFactors) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim FileData As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile = New LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim tfbs As Long = 0

            For Each Bacteria In RegpreciseDatabase.BacteriaGenomes
                For Each Regulator In Bacteria.Regulons.Regulators
                    If Regulator.Type = BacteriaGenome.Regulon.Regulator.Types.RNA Then
                        Continue For
                    End If

                    For Each siteInfo In Regulator.RegulatorySites
                        tfbs += 1
                        Dim strTitle As String = String.Format("{0}:{1}_lcl.{2} [regulator={3}] [regulog={4}]", siteInfo.LocusTag, siteInfo.Position, tfbs, Regulator.LocusTag.Key, Regulator.Regulog.Key)
                        Dim FastaObject As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject = New LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject
                        FastaObject.Attributes = New String() {strTitle}
                        FastaObject.SequenceData = ExternalDBSource.Regtransbase.WebServices.Regulator.SequenceTrimming(siteInfo)

                        Call FileData.Add(FastaObject)
                    Next
                Next
            Next

            Return FileData
        End Function

        'Private Shared Sub SeqTrim(Fasta As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject)
        '    Dim Tokens = (From mh As Match In Regex.Matches(Fasta.SequenceData, ".\(\d+\)", RegexOptions.Singleline) Select mh.Value).ToArray
        '    Dim sBuilder As StringBuilder = New StringBuilder(Fasta.SequenceData)
        '    For Each Token As String In Tokens
        '        Dim NewString As String = New String(Token.First, Convert.ToInt32(Regex.Match(Token, "\d+").Value))
        '        Call sBuilder.Replace(Token, NewString)
        '    Next

        '    Fasta.SequenceData = sBuilder.ToString
        'End Sub

        ''' <summary>
        ''' 从KEGG数据库中下载调控因子的蛋白质序列
        ''' </summary>
        ''' <param name="Regprecise"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DownloadRegulatorSequence(Regprecise As TranscriptionFactors, TempWork As String) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim FileData As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile = New LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim lcl As Long = 0
            Dim ErrLog As Logging.LogFile = New Logging.LogFile(String.Format("{0}/DownloadError_{1}.log", TempWork, Logging.LogFile.NowTime))
            Dim TempFile As String = String.Format("{0}/TempWork.fasta", TempWork)

            For Each Bacteria In Regprecise.BacteriaGenomes
                For Each Regulator In Bacteria.Regulons.Regulators

                    If Regulator.Type = BacteriaGenome.Regulon.Regulator.Types.RNA Then
                        Continue For
                    End If

                    If String.IsNullOrEmpty(Regulator.LocusTag.Key) Then
                        Dim Msg As String = String.Format("[NULL_LOCUS_TAG_DATA] [Regulog={0}] [Bacteria={1}]", Regulator.Regulog.Key, Bacteria.BacteriaGenome.Key) & vbCrLf
                        Call ErrLog.WriteLine(Msg, "", Logging.LogFile.MsgTypes.INF)
                        Continue For
                    End If

                    Dim FastaSaved As String = String.Format("{0}/{1}.fasta", TempWork, Regulator.LocusTag.Key)

                    If FileIO.FileSystem.FileExists(FastaSaved) AndAlso FileIO.FileSystem.GetFileInfo(FastaSaved).Length > 0 Then
                        Call FileData.Add(LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject.Load(FastaSaved))
                        Continue For
                    End If

                    Dim EntryList = LANS.SystemsBiology.Assembly.KEGG.DBGET.WebRequest.HandleQuery(Regulator.LocusTag.Key)
                    If EntryList.IsNullOrEmpty Then
                        Dim Msg = String.Format("[KEGG_ENTRY_NOT_FOUND] [Query_LocusTAG={0}] [Bacteria={1}]", Regulator.LocusTag.Key, Bacteria.BacteriaGenome.Key) & vbCrLf
                        Call ErrLog.WriteLine(Msg, "", Logging.LogFile.MsgTypes.INF)
                        Continue For
                    End If
                    EntryList = (From item In EntryList Where String.Equals(Regulator.LocusTag.Key, item.AccessionId, StringComparison.OrdinalIgnoreCase) Select item).ToArray
                    If EntryList.IsNullOrEmpty Then
                        Dim Msg = String.Format("[KEGG_ENTRY_NOT_FOUND] [Query_LocusTAG={0}] [Bacteria={1}]", Regulator.LocusTag.Key, Bacteria.BacteriaGenome.Key) & vbCrLf
                        Call ErrLog.WriteLine(Msg, "", Logging.LogFile.MsgTypes.INF)
                        Continue For
                    End If
                    Dim Entry = EntryList.First
                    Dim FastaSequence = LANS.SystemsBiology.Assembly.KEGG.DBGET.WebRequest.FetchSeq(Entry)

                    If FastaSequence Is Nothing Then
                        Dim Msg = String.Format("[KEGG_DATA_NOT_FOUND] [Query_LocusTAG={0}] [Bacteria={1}] KEGG not sure the object is a protein.", Regulator.LocusTag.Key, Bacteria.BacteriaGenome.Key) & vbCrLf
                        Call ErrLog.WriteLine(Msg, "", Logging.LogFile.MsgTypes.INF)
                        Continue For
                    End If

                    Dim strTitle As StringBuilder = New StringBuilder(2048)
                    Call strTitle.AppendFormat("[Regulog={0}] ", Regulator.Regulog.Key)
                    Dim tfbs As StringBuilder = New StringBuilder(128)
                    For Each siteInfo In Regulator.RegulatorySites
                        Call tfbs.AppendFormat("{0}:{1};", siteInfo.LocusTag, siteInfo.Position)
                    Next
                    Call tfbs.Remove(tfbs.Length - 1, 1)
                    Call strTitle.AppendFormat("[tfbs={0}]", tfbs.ToString)

                    lcl += 1
                    FastaSequence.Attributes = New String() {String.Format("lcl{0}", lcl), FastaSequence.Attributes.First, strTitle.ToString}
                    Call FileData.Add(FastaSequence)
                    Call FastaSequence.SaveTo(FastaSaved)
                Next
            Next

            Call ErrLog.Save()

            Return FileData
        End Function

        Public Function Regulators() As BacteriaGenome.Regulon.Regulator()
            Dim List As List(Of BacteriaGenome.Regulon.Regulator) = New List(Of BacteriaGenome.Regulon.Regulator)
            For Each BacteriaGenome In Me.BacteriaGenomes
                Call List.AddRange(BacteriaGenome.Regulons.Regulators)
            Next

            Return List.ToArray
        End Function
    End Class

    Public Class BacteriaGenome

        ''' <summary>
        ''' {GenomeName, Url}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Xml.Serialization.XmlElement> Public Property BacteriaGenome As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair
        <Xml.Serialization.XmlElement> Public Property Regulons As Regulon

        Public Overrides Function ToString() As String
            Return BacteriaGenome.Key
        End Function

        Public Shared Function Download(url As String) As Regulon
            Dim pageContent As String = Regex.Match(url.Get_PageContent, "<table class=""stattbl"".+?</table>", RegexOptions.Singleline).Value
            pageContent = Regex.Match(pageContent, "<tbody>.+</tbody>", RegexOptions.Singleline).Value

            Dim Items As String() = (From match As Match In Regex.Matches(pageContent, "<tr .+?</tr>", RegexOptions.Singleline + RegexOptions.IgnoreCase) Select match.Value).ToArray
            Dim Regulators As Regulon.Regulator() = New Regulon.Regulator(Items.Count - 1) {}

            For i As Integer = 0 To Regulators.Count - 1
                Dim strData As String = Items(i)
                Regulators(i) = Regulon.Regulator.CreateObject(strData)
            Next

            Return New Regulon With {.Regulators = Regulators}
        End Function

        Public Class Regulon

            <Xml.Serialization.XmlElement> Public Property Regulators As Regulator()

            Public Class Regulator
                Public Enum Types
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

                <Xml.Serialization.XmlAttribute> Public Property Type As Types
                <Xml.Serialization.XmlElement> Public Property Regulator As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair
                <Xml.Serialization.XmlElement> Public Property Effector As String
                <Xml.Serialization.XmlElement> Public Property Pathway As String
                <Xml.Serialization.XmlElement> Public Property LocusTag As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair
                <Xml.Serialization.XmlAttribute> Public Property Family As String
                <Xml.Serialization.XmlAttribute> Public Property RegulationMode As String
                <Xml.Serialization.XmlElement> Public Property BiologicalProcess As String
                <Xml.Serialization.XmlElement> Public Property Regulog As LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair
                <Xml.Serialization.XmlArray> Public Property RegulatorySites As LANS.SystemsBiology.ExternalDBSource.Regtransbase.WebServices.FastaObject()

                Public Overrides Function ToString() As String
                    Return String.Format("[{0}] {1}", Type.ToString, Regulator)
                End Function

                Public Function GetMotifSite(GeneId As String, MotifPosition As Integer) As Regtransbase.WebServices.FastaObject
                    Dim LQuery = (From item In RegulatorySites Where String.Equals(GeneId, item.LocusTag) AndAlso item.Position = MotifPosition Select item).ToArray
                    If LQuery.IsNullOrEmpty Then
                        Return Nothing
                    Else
                        Return LQuery.First
                    End If
                End Function

                Public Function ExportMotifs() As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject()
                    Dim LQuery = (From FastaObject As Regtransbase.WebServices.FastaObject
                                  In RegulatorySites
                                  Select New LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject With {
                                      .SequenceData = Regtransbase.WebServices.Regulator.SequenceTrimming(FastaObject),
                                      .Attributes = New String() {String.Format("[gene={0}:{1}] [family={2}] [regulog={3}]", FastaObject.LocusTag, FastaObject.Position, Family, Regulog.Key)}}).ToArray
                    Return LQuery
                End Function

                Public Shared Function CreateObject(strData As String) As Regulon.Regulator
                    Dim Items As String() = (From match As Match In Regex.Matches(strData, "<td.+?</td>") Select match.Value).ToArray
                    Dim Regulator As Regulator = New Regulator
                    Dim p As Integer

                    Regulator.Type = If(InStr(Items(p.MoveNext), " RNA "), Regulator.Types.RNA, Regulator.Types.TF)
                    Dim EntryData As String = Regex.Match(Items(p.MoveNext), "href="".+?"">.+?</a>").Value
                    Regulator.Regulator = LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair.CreateObject(TranscriptionFactors.GetId(EntryData), "http://regprecise.lbl.gov/RegPrecise/" & EntryData.Get_href)
                    Regulator.Effector = GetTagValue(Items(p.MoveNext))
                    Regulator.Pathway = GetTagValue(Items(p.MoveNext))

                    Return More(Regulator)
                End Function

                Private Shared Function More(Regulator As Regulator) As Regulator
                    Dim pageContent As String = Regulator.Regulator.Value.Get_PageContent
                    Dim Properties As String() = (From match As Match
                                                  In Regex.Matches(Regex.Match(pageContent, "<table class=""proptbl"">.+?</table>", RegexOptions.Singleline).Value, "<tr>.+?</tr>", RegexOptions.Singleline + RegexOptions.IgnoreCase)
                                                  Select match.Value).ToArray
                    Dim p As Integer = 1
                    If Regulator.Type = Types.TF Then
                        Dim LocusTag As String = Regex.Match(Properties(p.MoveNext), "href="".+?"">.+?</a>", RegexOptions.Singleline).Value
                        Regulator.LocusTag = LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair.CreateObject(TranscriptionFactors.GetId(LocusTag), "http://regprecise.lbl.gov/RegPrecise/" & LocusTag.Get_href)
                        Regulator.Family = GetTagValue_td(Properties(p.MoveNext).Replace("<td>Regulator family:</td>", ""))
                    Else
                        Dim Name As String = (From match As Match In Regex.Matches(Properties(p.MoveNext), "<td>.+?</td>", RegexOptions.Singleline + RegexOptions.IgnoreCase) Select match.Value).ToArray.Last
                        Name = Mid(Name, 5)
                        Name = Mid(Name, 1, Len(Name) - 5)
                        Regulator.LocusTag = LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair.CreateObject(Name, "")
                        Regulator.Family = Regex.Match(pageContent, "<td class=""[^""]+?"">RFAM:</td>[^<]+?<td>.+?</td>", RegexOptions.Singleline).Value
                        Regulator.Family = GetTagValue_td(Regulator.Family)
                    End If

                    Regulator.RegulationMode = GetTagValue_td(Properties(p.MoveNext))
                    Regulator.BiologicalProcess = GetTagValue_td(Properties(p.MoveNext))

                    Dim RegulogEntry As String = Regex.Match(Properties(p + 1), "href="".+?"">.+?</a>", RegexOptions.Singleline).Value
                    Regulator.Regulog = LANS.SystemsBiology.Assembly.ComponentModel.KeyValuePair.CreateObject(TranscriptionFactors.GetId(RegulogEntry).TrimA("").Replace(vbTab, "").Trim, "http://regprecise.lbl.gov/RegPrecise/" & RegulogEntry.Get_href)
                    Regulator.RegulatorySites = GetExportData(pageContent)

                    Return Regulator
                End Function

                Private Shared Function GetTagValue_td(strData As String) As String
                    strData = Regex.Match(strData, "<td>.+?</td>", RegexOptions.Singleline).Value
                    If String.IsNullOrEmpty(Trim(strData)) Then
                        Return ""
                    End If
                    strData = Mid(strData, 5)
                    strData = Mid(strData, 1, Len(strData) - 5)
                    Return strData
                End Function

                Private Shared Function GetExportData(pageContent As String) As ExternalDBSource.Regtransbase.WebServices.FastaObject()
                    Dim url As String = Regex.Match(pageContent, "<table class=""tblexport"">.+?</table>", RegexOptions.Singleline).Value
                    url = (From match As Match In Regex.Matches(url, "<tr>.+?</tr>", RegexOptions.Singleline + RegexOptions.IgnoreCase) Select match.Value).ToArray.Last
                    url = Regex.Match(url, "href="".+?""><b>DOWNLOAD</b>").Value
                    url = "http://regprecise.lbl.gov/RegPrecise/" & url.Get_href

                    Return ExternalDBSource.Regtransbase.WebServices.FastaObject.Parse(url)
                End Function

                Private Shared Function GetTagValue(strData As String) As String
                    strData = Regex.Match(strData, """>.+?</td>").Value
                    strData = Mid(strData, 3)
                    strData = Mid(strData, 1, Len(strData) - 5)
                    Return Trim(strData)
                End Function
            End Class
        End Class
    End Class
End Namespace
