#Region "Microsoft.VisualBasic::e928a695d5843c7f0ea10b52bf80b1d2, meme_suite\MEME.DocParser\MAST\HtmlParser.vb"

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

    '     Module HtmlParser
    ' 
    '         Function: __getRegulation, Export, Invoke, Process
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel

#Const DEBUG = 0

Namespace DocumentFormat.MAST.HTML

    <Package("Mast.Html.Parser", Publisher:="xie.guigang@gmail.com")>
    Public Module HtmlParser

        Private Function __getRegulation(Tfbs As FASTA.FastaSeq) As KeyValuePair(Of String, String)
            Dim Title As String = Tfbs.Title
            Dim siteId As String = Title.Split().First
            Dim regulator As String = Regex.Match(Title, "regulator=[^]]+").Value.Split(CChar("=")).Last
            Return New KeyValuePair(Of String, String)(siteId, regulator)
        End Function

        ''' <summary>
        ''' MEME批量输出的文件里面的每一个位点集合文件夹的名称应该都应该在MAST文件夹里面找得到相对应的文件夹进行数据解析
        ''' </summary>
        ''' <param name="MEME_out"></param>
        ''' <param name="MAST_out"></param>
        ''' <param name="FastaDir"></param>
        ''' <param name="RegpreciseTFBS"></param>
        ''' <returns></returns>
        <ExportAPI("Export.Dir.Batch")>
        Public Function Export(<Parameter("Dir.MEME.Out")> MEME_out As String,
                               <Parameter("Dir.Mast.Out")> MAST_out As String,
                               <Parameter("Dir.Fasta")> FastaDir As String,
                               RegpreciseTFBS As FASTA.FastaFile) As String
            Dim BaseDirExported As String = FileIO.FileSystem.GetParentPath(MEME_out)
            Dim TFBSInfos = (From Tfbs As FASTA.FastaSeq
                             In RegpreciseTFBS.AsParallel
                             Select __getRegulation(Tfbs)).ToDictionary(Function(item As KeyValuePair(Of String, String)) item.Key)

            Dim Directories As String() = FileIO.FileSystem.GetDirectories(MEME_out, FileIO.SearchOption.SearchTopLevelOnly).ToArray
            Dim DirName As String = MEME_out.Replace("\", "/").Split(CChar("/")).Last

            Call $"Load data from dir {MEME_out }!".debug

            Dim LQuery = (From i As Integer
                          In Directories.Sequence.AsParallel
                          Let subDir As String = Directories(i)
                          Select Process(subDir, FastaDir, MAST_out, TFBSInfos)).ToArray.Unlist

            Dim Path As String = String.Format("{0}/Exported/{1}.csv", BaseDirExported, DirName)

            Call LQuery.SaveTo(Path, False)
            Call $"Dir {MEME_out} data load completed!".debug

            Return Path
        End Function

        Private Function Process(subDir As String,
                                 FastaDir As String,
                                 Mast_Out As String,
                                 TFBSInfos As Dictionary(Of String, KeyValuePair(Of String, String))) As MEME.HTML.MEMEOutput()
            Dim ObjectId As String = subDir.Replace("\", "/").Split(CChar("/")).Last
            Dim FastaFile = FastaToken.Load(FASTA.FastaFile.Read(String.Format("{0}/{1}", FastaDir, ObjectId)))
            Dim Exported = Invoke(subDir, String.Format("{0}/{1}", Mast_Out, ObjectId), ObjectId)

            If Exported.IsNullOrEmpty Then
                Return Exported
            End If

            For Each FastaObject In FastaFile
                Dim GetIdLQuery = (From item In Exported Where String.Equals(item.Name, FastaObject.UniqueId, StringComparison.OrdinalIgnoreCase) Select item).ToArray
                Dim p As Integer
                If FastaObject.Strand = Strands.Forward Then
                    p = FastaObject.Location.Left - FastaObject.Length
                Else
                    p = FastaObject.Location.Right + FastaObject.Length
                End If

                For i As Integer = 0 To GetIdLQuery.Count - 1
                    Dim ResultItem = GetIdLQuery(i)
                    Dim MatchedMotif = TFBSInfos(ResultItem.MatchedMotif)

                    ResultItem.Name = FastaObject.GeneList
                    ResultItem.DoorOperonId = FastaObject.DOOR
                    ResultItem.MatchedRegulator = MatchedMotif.Value

                    If FastaObject.Strand = Strands.Forward Then
                        ResultItem.Start += p
                        ResultItem.Ends += p
                        ResultItem.Strand = "+"
                    Else
                        ResultItem.Start = p - GetIdLQuery(i).Start
                        ResultItem.Ends = p - GetIdLQuery(i).Ends
                        ResultItem.Start.Swap(GetIdLQuery(i).Ends)
                        ResultItem.Strand = "-"
                    End If
                Next
            Next

            Return Exported
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MEME_Out"></param>
        ''' <param name="MAST_Out"></param>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Invoke.Export")>
        Public Function Invoke(<Parameter("Dir.Meme")> MEME_Out As String,
                               <Parameter("Dir.Mast")> MAST_Out As String,
                               <Parameter("Object.ID")> ObjectId As String) As MEME.HTML.MEMEOutput()

            Dim MEMESites = MEME.HTML.LoadDocument(MEME_Out & "/meme.html").GetMatchedSites
            Dim MASTMatches = LoadDocument(MAST_Out & "/mast.html", False).MatchedSites
            Dim List As New List(Of MEME.HTML.MEMEOutput)

            For Each MAST_Matched In MASTMatches
                If MAST_Matched Is Nothing Then Continue For

                Dim GetSites = (From site In MEMESites Where site.Id = MAST_Matched.MotifId Select site).ToArray
                For Each site In GetSites
                    Dim MatchedSite = MEME.HTML.MEMEOutput.CreateObject(ObjectId, site)
                    MatchedSite.MAST_PValue = MAST_Matched.PValue
                    MatchedSite.MAST_EValue = MAST_Matched.EValue
                    MatchedSite.MatchedMotif = MAST_Matched.SequenceId
                    MatchedSite.Strand = MAST_Matched.Strand

                    Call List.Add(MatchedSite)
                Next
            Next

            Return List.ToArray
        End Function

    End Module
End Namespace
