#Region "Microsoft.VisualBasic::153fa94bbee59066d8cd6647262fb090, ..\GCModeller\data\ExternalDBSource\Regtransbase\WebServices\WebServiceHandler.vb"

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

Namespace Regtransbase.WebServices

    Public Class WebServiceHandler

        Public Shared Function TFFamily() As WebServices.RegPreciseTFFamily
            Dim url = "http://regprecise.lbl.gov/RegPrecise/collections_tffam.jsp"
            Return WebServices.RegPreciseTFFamily.Download(url)
        End Function

        Public Shared Function HandleErrorSequence(LogFilePath As String) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 从KEGG数据库中下载TFF数据库中的每一个结合位点的调控因子的序列
        ''' </summary>
        ''' <param name="TFFamily"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 对于每一个结合位点，都有一个唯一的基因号来进行标识，故而可以通过该基因号得到物种编号，之后既可以下载调控因子的蛋白质序列
        ''' 对于
        ''' </remarks>
        Public Shared Function DownloadSequenceData(TFFamily As WebServices.RegPreciseTFFamily, FileSaved As String) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim UniqueIdList As List(Of KeyValuePair(Of String, KeyValuePair(Of String, String())())) = New List(Of KeyValuePair(Of String, KeyValuePair(Of String, String())()))
            For Each TFF In TFFamily.Family
                Dim Item = TFF.Regulogs.GetUniqueIds
                Call UniqueIdList.AddRange(Item)
            Next
            Dim FsaFile As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile = New Assembly.SequenceModel.FASTA.FastaFile
            If FileIO.FileSystem.FileExists(FileSaved) Then Call FileIO.FileSystem.DeleteFile(FileSaved)

            Dim stringLine = Function(valueArray As String()) As String
                                 Dim sBuilder As StringBuilder = New StringBuilder(128)
                                 For Each id In valuearray
                                     Call sBuilder.Append(id & ";")
                                 Next
                                 Call sBuilder.Remove(sBuilder.Length - 1, 1)
                                 Return sbuilder.tostring
                             End Function

            Dim i As Integer
            For Each Item In UniqueIdList
                Dim RegulatorId As String = Split(Item.Key, " - ").First 'RegulatorId可能为一个专有的LocusTag，也可能为通用的基因名
                '首先进行查询，假若仅返回一条记录并且基因名为RegulatorId，那么RegulatorId很可能就为一个专有的LocusTag了
                Dim RegulatorEntries = LANS.SystemsBiology.Assembly.KEGG.DBGET.WebRequest.HandleQuery(keyword:=RegulatorId)
                If RegulatorEntries.IsNullOrEmpty Then 'NO_ENTRY_FOUND
                    Dim Err As String = String.Format("[KEGG_ID_NOT_FOUND] Regulog:={0}" & vbCrLf & vbCrLf, Item.Key)
                    Call FileIO.FileSystem.WriteAllText(My.Application.Info.DirectoryPath & "/Error.log", Err, True)
                    Call Console.WriteLine(Err)
                ElseIf RegulatorEntries.Count = 1 AndAlso String.Equals(RegulatorEntries.First.AccessionId, RegulatorId) Then
                    '则本Regulog列表之下的所有调控位点都被认为可以被该Regulator对象所调控
                    Dim RegulatorEntry = RegulatorEntries.First

                    Try
                        Dim Fsa = LANS.SystemsBiology.Assembly.KEGG.DBGET.WebRequest.FetchSeq(Entry:=RegulatorEntry)
                        Dim TempList = Fsa.Attributes.First

                        TempList = TempList & String.Format(" [Regulog={0}]", Item.Key)
                        Dim sBuilder As StringBuilder = New StringBuilder(128)
                        For Each Id In Item.Value
                            For Each LocusTag In Id.Value
                                Call sBuilder.Append(LocusTag & ";")
                            Next
                        Next
                        Call sBuilder.Remove(sBuilder.Length - 1, 1)
                        TempList = TempList & (String.Format(" [tfbs={0}]", sBuilder.ToString))

                        i += 1
                        Fsa.Attributes = New String() {String.Format("lcl{0}", i), TempList}

                        Call FileIO.FileSystem.WriteAllText(FileSaved, Fsa.Generate & vbCrLf, append:=True)
                        Call FsaFile.Add(item:=Fsa)
                    Catch ex As Exception
                        Dim Err = String.Format("[KEGG_DBGET_QUERY_EXCEPTION] [Regulog={0}] [KEGG_ENTRY={1}:{2}]" & vbCrLf, Item.Key, RegulatorEntry.SpeciesId, RegulatorId)
                        Call Console.WriteLine(Err)
                        Call FileIO.FileSystem.WriteAllText(My.Application.Info.DirectoryPath & "/Error.log", Err, append:=True)
                    End Try
                Else '为通用的基因名称，则按照TFBS的LocusTag信息得到KEGG的物种编号，在进行组合查询
                    Dim TempObject As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject = Nothing
                    Dim TempObject_LocusTagList As List(Of String) = New List(Of String)

                    For Each LocusId In Item.Value  'LocusId的物种是唯一的
                        Dim Entries = LANS.SystemsBiology.Assembly.KEGG.DBGET.WebRequest.HandleQuery(keyword:=LocusId.Key)
                        If Not Entries.IsNullOrEmpty Then '获得目标物种编号
                            Dim Temp As String() = (From Entry In Entries Where String.Equals(Entry.AccessionId, LocusId.Key) Select Entry.SpeciesId).ToArray
                            Dim KEGG_speciesId As String = ""
                            If Temp.IsNullOrEmpty Then
                                Dim Err As String = String.Format("[KEGG_ID_NOT_FOUND] Locus_tag:={0}, Regulog:={1}" & vbCrLf & vbCrLf, LocusId, Item.Key)
                                Call FileIO.FileSystem.WriteAllText(My.Application.Info.DirectoryPath & "/Error.log", Err, True)
                                Call Console.WriteLine(Err)
                                Continue For
                            Else
                                KEGG_speciesId = Temp.First
                            End If

                            Try
                                Dim Fsa = LANS.SystemsBiology.Assembly.KEGG.DBGET.WebRequest.FetchSeq(KEGG_speciesId, accessionId:=RegulatorId)
                                If TempObject Is Nothing Then
                                    If Not Fsa Is Nothing Then TempObject = Fsa.Copy
                                    TempObject_LocusTagList.AddRange(LocusId.Value)
                                Else
                                    If Fsa Is Nothing Then '没有查询到数据记录，则使用前一个数据
                                        Call TempObject_LocusTagList.AddRange(LocusId.Value)
                                        Continue For
                                    Else
                                        Dim TempList = Fsa.Attributes.First

                                        TempList = TempList & String.Format(" [Regulog={0}]", Item.Key)
                                        TempList = TempList & (String.Format(" [tfbs={0}]", stringline(locusid.value)))

                                        i += 1
                                        Fsa.Attributes = New String() {String.Format("lcl{0}", i), TempList}

                                        Call FileIO.FileSystem.WriteAllText(FileSaved, Fsa.Generate & vbCrLf, append:=True)
                                        Call FsaFile.Add(item:=Fsa)
                                    End If
                                End If
                            Catch ex As Exception
                                Dim Err = String.Format("[KEGG_DBGET_QUERY_EXCEPTION] [Regulog={0}] [KEGG_ENTRY={1}:{2}]" & vbCrLf, Item.Key, KEGG_speciesId, RegulatorId)
                                Call Console.WriteLine(Err)
                                Call FileIO.FileSystem.WriteAllText(My.Application.Info.DirectoryPath & "/Error.log", Err, append:=True)
                            End Try
                        Else '搜索不到任何记录
                            Dim Err As String = String.Format("[KEGG_ID_NOT_FOUND] Locus_tag:={0} ---> {1}, Regulog:={2}" & vbCrLf & vbCrLf, LocusId, stringLine(LocusId.Value), Item.Key)
                            Call FileIO.FileSystem.WriteAllText(My.Application.Info.DirectoryPath & "/Error.log", Err, True)
                            Call Console.WriteLine(Err)
                        End If
                    Next

                    '生成TEMPObject的数据记录
                    If TempObject Is Nothing Then '记录下错误
                        Dim Err = String.Format("[KEGG_DBGET_QUERY_EXCEPTION] Could not found any query entry for object [Regulog={0}] !" & vbCrLf, Item.Key)
                        Call Console.WriteLine(Err)
                        Call FileIO.FileSystem.WriteAllText(My.Application.Info.DirectoryPath & "/Error.log", Err, append:=True)
                    Else '将缓存数据写入文件
                        Dim TempList = TempObject.Attributes.First
                        Dim LOCUS_TAG_ENTRY As String = stringLine(TempObject_LocusTagList.ToArray)

                        TempList = TempList & String.Format(" [Regulog={0}]", Item.Key)
                        TempList = TempList & (String.Format(" [tfbs={0}]", LOCUS_TAG_ENTRY))

                        i += 1
                        TempObject.Attributes = New String() {String.Format("lcl{0}", i), TempList}

                        Call FileIO.FileSystem.WriteAllText(FileSaved, TempObject.Generate & vbCrLf, append:=True)
                        Call FsaFile.Add(item:=TempObject)
                    End If
                End If
            Next

            Return FsaFile
        End Function
    End Class
End Namespace
