#Region "Microsoft.VisualBasic::c39e7caf54d9581e7bc18c14516e5f6b, CLI_tools\c2\Workflows\MEME_Analysis_ACtions.vb"

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

    '     Class MatchedResult
    ' 
    '         Properties: Effectors, Family, Mast_evalue, Mast_pvalue, MatchedRegulator
    '                     Meme_evalue, Meme_pvalue, ObjectId, OperonGenes, OperonId
    '                     RegpreciseRegulator, tfbsId
    ' 
    '         Function: ToString
    ' 
    '     Module MEME_Analysis_ACtions
    ' 
    '         Function: Export, FinalRegulationMatch, GetDiReBh, GetKeywords, GetMaxItem
    '                   LocalBlastp, SequenceNotDownload, TrimF
    ' 
    '         Sub: Action, match
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Text

Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace Workflows

    Public Class MatchedResult
        <Column("Object_Id")> Public Property ObjectId As String
        <Column("Operon_Id")> Public Property OperonId As String
        <CollectionAttribute("Operon_Genes")> Public Property OperonGenes As String()
        <Column("tfbs_Id")> Public Property tfbsId As String
        <Column("Regprecise_regulator")> Public Property RegpreciseRegulator As String
        <Column("Matched_regulator")> Public Property MatchedRegulator As String
        <Column("meme_pvalue")> Public Property Meme_pvalue As Double
        <Column("meme_evalue")> Public Property Meme_evalue As Double
        <Column("mast_pvalue")> Public Property Mast_pvalue As Double
        <Column("mast_evalue")> Public Property Mast_evalue As Double
        <CollectionAttribute("Effectors")> Public Property Effectors As String()
        <Column("Family")> Public Property Family As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1}", ObjectId, OperonId)
        End Function
    End Class

    Public Module MEME_Analysis_ACtions

        ''' <summary>
        ''' 返回所导出的结果所在的文件夹
        ''' </summary>
        ''' <param name="predictedRegulation"></param>
        ''' <param name="regpreciseRegulator"></param>
        ''' <param name="regprecisetfbs"></param>
        ''' <param name="reg_vs_query_bh"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FinalRegulationMatch(predictedRegulation As String, regpreciseRegulator As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile,
                                        regprecisetfbs As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile,
                                        reg_vs_query_bh As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As String
            Console.WriteLine("function FinalRegulationMatch()")
            Dim dir = predictedRegulation & "/matched/"
            Console.WriteLine("create dir {0}", dir)

            Dim regulators = reg_vs_query_bh.AsDataSource(Of Global.LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH)(False)

            Console.WriteLine("matching data file")
#If DEBUG Then
            For Each file In (From path In FileIO.FileSystem.GetFiles(predictedRegulation, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                              Select Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(path)).ToArray
#Else
            Parallel.ForEach((From path In FileIO.FileSystem.GetFiles(predictedRegulation, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                                         Select Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(path)).ToArray,
                                     Sub(file As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
#End If
                Dim matchedFile As New List(Of MatchedResult)

                Console.WriteLine("procede file {0}", file.FilePath)

                Dim create = (From row In file.Skip(1)
                              Let create_action = Function() As MatchedResult()
                                                      Dim tfbs = row(2)
                                                      Dim LQuery = (From item As LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH
                                                                     In regulators.AsParallel Where Array.IndexOf(item.RegpreciseTfbsIds, tfbs) > -1
                                                                                             Select item).ToArray

                                                      If Not LQuery.IsNullOrEmpty Then
                                                          Dim chunkbuffer As MatchedResult() = New MatchedResult(LQuery.Count - 1) {}

                                                          For i As Integer = 0 To LQuery.Count - 1
                                                              Dim item = LQuery(i)
                                                              Dim operon = Strings.Split(row(1), " --> ")
                                                              Dim operonId = operon.First
                                                              Dim operonGenes = Strings.Split(operon.Last, "; ")
                                                              Dim newMatch = New MatchedResult With {.ObjectId = row(0),
                                                                                                      .tfbsId = tfbs,
                                                                                                      .Meme_pvalue = row(3),
                                                                                                      .Mast_pvalue = row(4),
                                                                                                      .Mast_evalue = row(5),
                                                                                                     .OperonId = operonId,
                                                                                                     .OperonGenes = operonGenes,
                                                                                                     .RegpreciseRegulator = item.HitName,
                                                                                                     .MatchedRegulator = item.QueryName}
                                                              chunkbuffer(i) = newMatch
                                                          Next
                                                          Return chunkbuffer
                                                      Else
                                                          Return New MatchedResult() {}
                                                      End If
                                                  End Function Select create_action()).ToArray

                Console.WriteLine("end of row creation")

                For Each item In create
                    Call matchedFile.AddRange(item)
                Next

                Call matchedFile.SaveTo(String.Format("{0}/{1}", dir, FileIO.FileSystem.GetName(file.FilePath)), False)
#If DEBUG Then
            Next
#Else
                                     End Sub)
#End If
            Console.WriteLine("/* FinalRegulationMatch */")

            Return dir
        End Function

        ''' <summary>
        ''' 返回解析结果所导出的位置
        ''' </summary>
        ''' <param name="MEME_Out"></param>
        ''' <param name="MAST_OUT"></param>
        ''' <param name="FsaDir"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Export(MEME_Out As String, MAST_OUT As String, FsaDir As String) As String
            Console.WriteLine("Call Global.MEME.MEME_OUT.Action(MEME_Out)")
            Call Global.LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Workflows.MEME_OUT.Action(MEME_Out)
            Console.WriteLine("Call Global.MEME.MAST_OUT.Action(MAST_OUT)")
            Call Global.LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Workflows.MAST_OUT.Action(MAST_OUT)
            Console.WriteLine("Call match(MEME_Out, FsaDir)")
            Call match(MEME_Out, FsaDir)

            Dim dir As String = FileIO.FileSystem.GetParentPath(MEME_Out) & "/predicted_regulation/"
            Call FileIO.FileSystem.CreateDirectory(dir)

            Console.WriteLine("Start to Export data")
#If DEBUG Then
            For Each file As String In FileIO.FileSystem.GetFiles(MEME_Out, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
#Else
            Call Parallel.ForEach(FileIO.FileSystem.GetFiles(MEME_Out, FileIO.SearchOption.SearchTopLevelOnly, "*.csv"),
                                                Sub(file As String)
#End If
                Dim name As String = FileIO.FileSystem.GetName(file)
                Dim mast As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = String.Format("{0}/{1}", MAST_OUT, name)
                Dim motifFile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
                Dim meme = Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(file)

                Call motifFile.Add(New String() {"ObjectId", "Operon", "tfbsId", "meme_pvalue", "mast_pvalue", "mast_evalue"})

                Dim geneLQuery = (From row In meme.Skip(1).AsParallel
                                  Let generate = Function() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject()
                                                     Dim objectId = row(0), motifId = row(2)
                                                     Dim TFBSSites = (From tfbs_row In mast.AsParallel Where String.Equals(tfbs_row(0), objectId) AndAlso String.Equals(tfbs_row(1), motifId) Select tfbs_row).ToArray
                                                     'objectid,operon,tfbs,meme_pvalue,mast_p-value,mast_e-value
                                                     Dim LQuery = (From site In TFBSSites.AsParallel Select New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject From {objectId, row(1), site(3), row(3), site(2), site(4)}).ToArray
                                                     For i As Integer = 0 To LQuery.Count - 1
                                                         Dim newrow = LQuery(i)
                                                         newrow(1) = String.Format("{0} --> {1}", Regex.Match(newrow(1), "AssociatedOperon=\d+").Value.Split(CChar("=")).Last, Regex.Match(newrow(1), "OperonGenes=[^]]+").Value.Split(CChar("=")).Last)
                                                     Next
                                                     Return LQuery
                                                 End Function Select generate()).ToArray
                Console.WriteLine("create rows done! {0}", file)
                For Each item In geneLQuery
                    Call motifFile.AppendRange(item)
                Next

                file = String.Format("{0}/{1}", dir, name)
                Console.WriteLine("writing file {0}", file)
                                                    Call motifFile.Save(file, False)
#If DEBUG Then
            Next
#Else
            End Sub)
#End If
            Console.WriteLine("/* Sub Export(MEME_Out As String, MAST_OUT As String, FsaDir As String) */")

            Return dir
        End Function

        Public Sub match(MEME_OUT As String, FsaDir As String)
            Dim list = (From path In FileIO.FileSystem.GetFiles(MEME_OUT, FileIO.SearchOption.SearchTopLevelOnly, "*.csv") Select Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(path)).ToArray
            Dim DirList = FileIO.FileSystem.GetDirectories(FsaDir, FileIO.SearchOption.SearchTopLevelOnly)

#If DEBUG Then
            For Each csvfile In list
#Else
            Parallel.ForEach(list, Sub(csvFile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
#End If
                Dim Dirname As String = FileIO.FileSystem.GetName(csvfile.FilePath)
                Dim dirs = (From strDir As String In DirList Where InStr(Dirname, FileIO.FileSystem.GetName(strDir)) Select strDir).ToArray
                If dirs.IsNullOrEmpty Then
                    Console.WriteLine("Original data not found for ""{0}""", csvfile.FilePath)
#If DEBUG Then
                    Continue For
#Else
                                           Return
#End If
                End If

                Dim dir As String = dirs.First
                Dim FsaList = (From file In FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly, "*.fsa").AsParallel Select LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.Read(file)).ToArray
                Dim IDLIst = (From fsa In FsaList.AsParallel Select New KeyValuePair(Of String, KeyValuePair(Of String, String)())(FileIO.FileSystem.GetFileInfo(fsa.SourceFile).Name.Replace(".fsa", ""), (From obj In fsa.AsParallel Let s = obj.Title.Split Select New KeyValuePair(Of String, String)(s(0), obj.Title)).ToArray)).ToArray

                For i As Integer = 1 To csvfile.Count - 1
                    Dim row = csvfile(i)

                    If String.IsNullOrEmpty(row(1).Trim) Then
                        Continue For
                    End If

                    Dim Id = row(0)
                    Dim operon____ID As String = row(1)
                    Dim LQuery = (From item In IDLIst.AsParallel Where String.Equals(Id, item.Key) Select item).ToArray
                    If Not LQuery.IsNullOrEmpty Then
                        Dim OperonId = (From item In LQuery.First.Value.AsParallel Where String.Equals(item.Key, operon____ID) Select item).ToArray
                        If Not OperonId.IsNullOrEmpty Then
                            row(1) = OperonId.First.Value
                        End If
                    End If
                Next

                                       Call csvFile.Save(LazySaved:=False)
#If DEBUG Then
            Next
#Else
            End Sub)
#End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="regprecise">数据库 文件路径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetKeywords(regprecise As String) As LANS.SystemsBiology.ComponentModel.KeyValuePair()
            Dim DbRegprecise = regprecise.LoadXml(Of LANS.SystemsBiology.DatabaseServices.Regtransbase.WebServices.RegPreciseTFFamily)()
            Dim List As List(Of String) = New List(Of String)
            For Each family In DbRegprecise.Family
                Dim Collection = ((From item In family.Regulogs.Logs Select item.Regulog.Key).ToArray)
                Call List.AddRange(Collection)
            Next

            Return (From str As String
                    In (From ss As String In List Select ss Distinct).ToArray
                    Let tokens As String() = str.Split(CChar("-"))
                    Select New LANS.SystemsBiology.ComponentModel.KeyValuePair With {
                        .Key = Trim(tokens(0)),
                        .Value = Trim(tokens(1))}).ToArray
        End Function

        Public Function SequenceNotDownload(regprecise As String, DownloadSeq As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile) As LANS.SystemsBiology.ComponentModel.KeyValuePair()
            Dim AllList = GetKeywords(regprecise).ToList
            Dim DonwloadList = (From fsa In DownloadSeq Let id = fsa.Attributes.Last Let tokens = Strings.Split(id, " - ") Select New LANS.SystemsBiology.ComponentModel.KeyValuePair With {.Key = tokens(0), .Value = tokens(1)}).ToArray

            For Each iten In DonwloadList
                Call AllList.RemoveAll(Function(n As LANS.SystemsBiology.ComponentModel.KeyValuePair) iten.Equals(n, strict:=False))
            Next

            Dim result = LANS.SystemsBiology.ComponentModel.KeyValuePair.Distinct(AllList.ToArray)
            Return result
        End Function

        Public Sub Action(list As LANS.SystemsBiology.ComponentModel.KeyValuePair(), savedFile As String)
            Dim NCBIProteins = New LANS.SystemsBiology.Assembly.NCBI.Entrez.Protein()
            Dim FsaList As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile =
                New LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Dim attrList As List(Of String) = New List(Of String)

            For Each item In list
                Dim Entries = NCBIProteins.GetEntry(item.Key, item.Value, MaxLimited:=3)

                If Entries.IsNullOrEmpty Then
                    Continue For
                End If

                Dim Locus_Tag = GetMaxItem((From entry In Entries Let s = Trim(entry.LocusTag) Where Not String.IsNullOrEmpty(s) Select s Distinct).ToArray)
                Dim entryList As LANS.SystemsBiology.Assembly.KEGG.WebServices.QueryEntry()
                If String.IsNullOrEmpty(Locus_Tag) Then
                    entryList = LANS.SystemsBiology.Assembly.KEGG.WebServices.WebRequest.HandleQuery(item.Key) '模糊查找多条记录
                    Dim LQuery = (From entry In entryList Where String.Equals(entry.SpeciesId, "eco") Select entry).ToArray

                    If Not LQuery.IsNullOrEmpty Then
                        Dim fsa = LANS.SystemsBiology.Assembly.KEGG.WebServices.WebRequest.FetchSeq(LQuery.First)
                        attrList = fsa.Attributes.ToList
                        attrList.Add(item.Key & " - " & item.Value)

                        fsa.Attributes = attrList.ToArray
                        Call FsaList.Add(fsa)
                    End If
                Else
                    entryList = LANS.SystemsBiology.Assembly.KEGG.WebServices.WebRequest.HandleQuery(Locus_Tag) '只有一条记录

                    If Not entryList.IsNullOrEmpty Then
                        Dim fsa = LANS.SystemsBiology.Assembly.KEGG.WebServices.WebRequest.FetchSeq(entryList.First)
                        attrList = fsa.Attributes.ToList
                        attrList.Add(item.Key & " - " & item.Value)

                        fsa.Attributes = attrList.ToArray
                        Call FsaList.Add(fsa)
                    End If
                End If
            Next

            Call FsaList.Save(savedFile)
        End Sub

        Private Function GetMaxItem(Collection As String()) As String
            If Collection.IsNullOrEmpty Then
                Return ""
            End If

            Dim List = (From str In Collection Select str Distinct).ToArray
            Dim Vote As Integer() = New Integer(List.Count - 1) {}
            For Each item In Collection
                Dim idx = Array.IndexOf(List, item)
                Vote(idx) += 1
            Next

            Dim max As Integer = Integer.MinValue
            Dim max_p As Integer = -1
            For i As Integer = 0 To Vote.Count - 1
                If Vote(i) > max Then
                    max = Vote(i)
                    max_p = i
                End If
            Next

            Return List(max_p)
        End Function

        Public Function LocalBlastp(LocalBlast As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Programs.LocalBLAST,
                                    WorkDir As String,
                                    Query As String,
                                    Subject As String) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File

            Call LocalBlast.FormatDb(Query, LocalBlast.MolTypeProtein).Start(WaitForExit:=True)
            Call LocalBlast.FormatDb(Subject, LocalBlast.MolTypeProtein).Start(WaitForExit:=True)

            Call LocalBlast.Blastp(Query, Subject, WorkDir & "/blastp_logs_qvss.txt", "1").Start(WaitForExit:=True)
            Dim Log = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput.TryParse(LocalBlast.LastBLASTOutputFilePath)
            Call Log.Grep(AddressOf Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile("'match gene[=][^]]+';'tokens = 1'").Grep,
                                   AddressOf Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile("tokens | 1").Grep)

            Dim File = Log.ExportBestHit
            For i As Integer = 1 To File.Count - 1
                Dim row = File(i)
                row.HitName = TrimF(row.HitName)
            Next

            Dim CsvDoc_1 = File.GenerateCsvDocument
            Call CsvDoc_1.SaveTo("x:\xan8004vregprecise.csv", False)

            Call LocalBlast.Blastp(Subject, Query, WorkDir & "/blastp_logs_svsq.txt", "1").Start(WaitForExit:=True)
            Log = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput.TryParse(LocalBlast.LastBLASTOutputFilePath)
            Call Log.Grep(AddressOf Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile("tokens | 1").Grep,
                          AddressOf Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile("'match gene[=][^]]+';'tokens = 1'").Grep)

            Dim File2 = Log.ExportBestHit
            For i As Integer = 1 To File.Count - 1
                Dim row = File(i)
                row.QueryName = Trim(row.QueryName)
            Next
            Dim CsvDoc_2 = File2.GenerateCsvDocument
            Call CsvDoc_2.SaveTo("x:\regprecisevxan8004.csv", False)

            Dim best = GetDiReBh(CsvDoc_1, CsvDoc_2,
                                 LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.Read(Query),
                                 LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.Read(Subject))

            Return best
        End Function

        ''' <summary>
        ''' 获取双向的最佳匹配结果
        ''' </summary>
        ''' <param name="QvS"></param>
        ''' <param name="SvQ"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDiReBh(SvQ As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
                                  QvS As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
                                  QueryFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile,
                                  SubjectFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile) _
            As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File

            Dim File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
            Call File.AppendLine(New String() {"query_id", "hit_id", "query_description", "hit_description"})

            Dim GetQuery = Function(queryId As String) As String
                               Dim LQuery = (From fsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
                                             In QueryFsa
                                             Let title As String = fsa.Title
                                             Where InStr(title, "gene=" & queryId)
                                             Select Regex.Match(title, "protein=[^]]+", RegexOptions.IgnoreCase).Value.Replace("protein=", "")).ToArray
                               Return LQuery.First
                           End Function
            Dim GetSubject = Function(subjectId As String) As String
                                 Dim LQuery = (From fsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
                                          In SubjectFsa
                                               Let title As String = fsa.Title
                                               Let tokens = title.Split(CChar("|"))
                                               Where String.Equals(tokens(1), subjectId)
                                               Select tokens(0)).ToArray
                                 Return LQuery.First
                             End Function

            For Each row In QvS.Skip(1) '跳过标题行
                Dim subject As String = row(1), ID As String = row(0)
                If String.IsNullOrEmpty(Trim(subject)) Then
                    Call File.AppendLine(New String() {ID, " ", GetQuery(ID)})
                    Continue For
                End If
                Dim sbjRowsFind As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject() = SvQ.FindAtColumn(subject, Column:=0)

                If sbjRowsFind.IsNullOrEmpty Then
                    Call File.AppendLine(New String() {ID, " ", GetQuery(ID)})
                    Continue For
                End If

                Dim s As String = sbjRowsFind.First()(1)

                If String.Equals(ID, s) Then '可以双向匹配
                    Call File.AppendLine(New String() {ID, subject, GetQuery(ID), GetSubject(subject)})
                Else
                    Call File.AppendLine(New String() {ID, " ", GetQuery(ID)})
                End If
            Next

            Return File
        End Function

        Public Function TrimF(strText As String) As String
            If String.IsNullOrEmpty(Trim(strText)) Then
                Return " "
            End If
            Dim Tokens = Strings.Split(strText, " - ")
            Return String.Format("{0} - {1}", Trim(Tokens(0)), Trim(Tokens(1)))
        End Function
    End Module
End Namespace
