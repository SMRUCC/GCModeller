#Region "Microsoft.VisualBasic::ed1a8bde2a21611d15c904ccae5ab8bc, meme_suite\MEME\Workflows\MEME_Analysis_Actions.vb"

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

    '     Module MEME_Analysis_ACtions
    ' 
    '         Function: GetKeywords, GetMaxItem, SequenceNotDownload
    ' 
    '         Sub: Action, Export, FinalRegulationMatch, match, TokenInvoke
    '         Class __Parallelinvoker
    ' 
    '             Function: __createAction
    ' 
    '             Sub: Invoke
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Threading.Tasks.Parallel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel

Namespace Workflows

    'Public Class MatchedResult
    '    <Column("Object_Id")> Public Property ObjectId As String
    '    <Column("Operon_Id")> Public Property OperonId As String
    '    <CollectionColumn("Operon_Genes")> Public Property OperonGenes As String()
    '    <Column("tfbs_Id")> Public Property tfbsId As String
    '    <Column("Regprecise_regulator")> Public Property RegpreciseRegulator As String
    '    <Column("Matched_regulator")> Public Property MatchedRegulator As String
    '    <Column("meme_pvalue")> Public Property Meme_pvalue As Double
    '    <Column("meme_evalue")> Public Property Meme_evalue As Double
    '    <Column("mast_pvalue")> Public Property Mast_pvalue As Double
    '    <Column("mast_evalue")> Public Property Mast_evalue As Double
    '    <CollectionColumn("Effectors")> Public Property Effectors As String()
    '    <Column("Family")> Public Property Family As String

    '    Public Overrides Function ToString() As String
    '        Return String.Format("{0} -> {1}", ObjectId, OperonId)
    '    End Function
    'End Class

    Public Module MEME_Analysis_ACtions

        Public Sub FinalRegulationMatch(predictedRegulation As String,
                                        regpreciseRegulator As FASTA.FastaFile,
                                        regprecisetfbs As FASTA.FastaFile,
                                        reg_vs_query_bh As IO.File)

            Console.WriteLine("function FinalRegulationMatch()")
            Dim dir = predictedRegulation & "/matched/"
            Console.WriteLine("create dir {0}", dir)
            Console.WriteLine("createing tfbs informations...")
            Dim tfbsInfo As KeyValuePair(Of String, String)() = (From fsa In regprecisetfbs.AsParallel
                                                                 Let attr = fsa.Headers.First
                                                                 Let tokens = attr.Split
                                                                 Let id = tokens.First
                                                                 Let gene = Regex.Match(tokens(1), "[a-z0-9_]+[:][-\d+]+").Value
                                                                 Select New KeyValuePair(Of String, String)(id, gene)).ToArray
            Console.WriteLine("creating regulators informations...")
            Dim regulators As KeyValuePair(Of String, String())() = (From fsa In regpreciseRegulator.AsParallel
                                                                     Let list = Regex.Match(fsa.Headers.Last, "tfbs=[^]]+").Value.Split(CChar("=")).Last.Split(CChar(";"))
                                                                     Let id = String.Format("{0}|{1}", fsa.Headers.First, fsa.Headers(1).Split.First)
                                                                     Select New KeyValuePair(Of String, String())(id, list)).ToArray

            Console.WriteLine("matching data file")
            Call ForEach((From path
                          In FileIO.FileSystem.GetFiles(predictedRegulation, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                          Select IO.File.Load(path)).ToArray,
                                  AddressOf New __Parallelinvoker With {.regulators = regulators, .reg_vs_query_bh = reg_vs_query_bh, .tfbsInfo = tfbsInfo}.Invoke)
            Console.WriteLine("/* FinalRegulationMatch */")
        End Sub

        Private Class __Parallelinvoker

            Public tfbsInfo As KeyValuePair(Of String, String)()
            Public regulators As KeyValuePair(Of String, String())()
            Public reg_vs_query_bh As IO.File

            Public Sub Invoke(File As IO.File)
                Dim matchedFile As New IO.File

                Dim create = (From row In File.Skip(1).AsParallel Select __createAction(row)).ToArray

                Console.WriteLine("end of row creation")
                Dim firstrow = File.First
                Call firstrow.AddRange({"regprecise_regulator", "matched_regulator"})
                Call matchedFile.Add(firstrow)
                Call matchedFile.AppendRange(create)

                '  Call matchedFile.Save(String.Format("{0}/{1}", Dir, FileIO.FileSystem.GetName(File.FilePath)), False)
            End Sub

            Private Function __createAction(row As RowObject) As RowObject
                Dim tfbs = row(2)
                Dim tfbsNameList = (From obj As KeyValuePair(Of String, String) In tfbsInfo.AsParallel Where String.Equals(tfbs, obj.Key) Select obj.Value).ToArray
                If tfbsNameList.IsNullOrEmpty Then
                    Return row
                End If
                Dim tfbsName = tfbsNameList.First
                Dim LQuery = (From obj As KeyValuePair(Of String, String()) In regulators.AsParallel Where Array.IndexOf(obj.Value, tfbsName) > -1 Select obj.Key).ToArray
                Dim newrow As New RowObject
                Call newrow.AddRange(row.ToArray)

                If Not LQuery.IsNullOrEmpty Then
                    Dim regulator = LQuery.First
                    Dim matched As String = "", matchList = reg_vs_query_bh.FindAtColumn(regulator, 0)
                    If Not matchList.IsNullOrEmpty Then
                        matched = matchList.First()(1)
                    End If

                    Call newrow.Add(regulator)
                    Call newrow.Add(matched)
                End If

                Return newrow
            End Function
        End Class

        Public Sub Export(MEME_Out As String, MAST_OUT As String, FsaDir As String)
            Console.WriteLine("Call Global.MEME.MEME_OUT.Action(MEME_Out)")
            Call Workflows.MEME_OUT.Action(MEME_Out)
            Console.WriteLine("Call Global.MEME.MAST_OUT.Action(MAST_OUT)")
            Call Workflows.MAST_OUT.Action(MAST_OUT)
            Console.WriteLine("Call match(MEME_Out, FsaDir)")
            Call match(MEME_Out, FsaDir)

            Dim dir As String = FileIO.FileSystem.GetParentPath(MEME_Out) & "/predicted_regulation/"
            Call FileIO.FileSystem.CreateDirectory(dir)

            Console.WriteLine("Start to Export data")
            Call ForEach((From path As String
                                   In FileIO.FileSystem.GetFiles(MEME_Out, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                          Select New KeyValuePair(Of String, String)(path, MAST_OUT)),
                                  AddressOf TokenInvoke)
            Console.WriteLine("/* Sub Export(MEME_Out As String, MAST_OUT As String, FsaDir As String) */")
        End Sub

        Private Sub TokenInvoke(File_MAST_OUT As KeyValuePair(Of String, String))
            Dim File As String = File_MAST_OUT.Key
            Dim MAST_OUT As String = File_MAST_OUT.Value
            Dim name As String = FileIO.FileSystem.GetName(File)
            Dim mast As IO.File = String.Format("{0}/{1}", MAST_OUT, name)
            Dim motifFile As New IO.File
            Dim meme As IO.File = IO.File.Load(File)

            Call motifFile.Add(New String() {"ObjectId", "Operon", "tfbsId", "meme_pvalue", "mast_pvalue", "mast_evalue"})

            Dim geneLQuery = (From Row In meme.Skip(1).AsParallel
                              Let generate = Function() As RowObject()
                                                 Dim objectId = Row(0), motifId = Row(2)
                                                 Dim TFBSSites = (From tfbs_row In mast.AsParallel Where String.Equals(tfbs_row(0), objectId) AndAlso String.Equals(tfbs_row(1), motifId) Select tfbs_row).ToArray
                                                 'objectid,operon,tfbs,meme_pvalue,mast_p-value,mast_e-value
                                                 Dim LQuery = (From site In TFBSSites.AsParallel
                                                               Select New RowObject From {objectId, Row(1), site(3), Row(3), site(2), site(4)}).ToArray
                                                 For i As Integer = 0 To LQuery.Count - 1
                                                     Dim newrow = LQuery(i)
                                                     newrow(1) = String.Format("{0} --> {1}",
                                                                               Regex.Match(newrow(1), "AssociatedOperon=\d+").Value.Split("="c).Last,
                                                                               Regex.Match(newrow(1), "OperonGenes=[^]]+").Value.Split("="c).Last)
                                                 Next
                                                 Return LQuery
                                             End Function Select generate()).ToArray
            Console.WriteLine("create rows done! {0}", File.ToFileURL)
            For Each item In geneLQuery
                Call motifFile.AppendRange(item)
            Next

            '   File = String.Format("{0}/{1}", Dir, name)
            Console.WriteLine("writing file {0}", File.ToFileURL)
            Call motifFile.Save(File, False)
        End Sub

        Public Sub match(MEME_OUT As String, FsaDir As String)
            Dim list = ls - l - "*.csv" <= MEME_OUT
            Dim DirList = FileIO.FileSystem.GetDirectories(FsaDir, FileIO.SearchOption.SearchTopLevelOnly)

            ForEach(list, Sub(path As String)
                              Dim csvFile = IO.File.Load(path)
                              Dim Dirname As String = FileIO.FileSystem.GetName(path)
                              Dim Dir = (From strDir As String In DirList Where InStr(Dirname, FileIO.FileSystem.GetName(strDir)) Select strDir).First

                              Dim FsaList = (From file In FileIO.FileSystem.GetFiles(Dir, FileIO.SearchOption.SearchTopLevelOnly, "*.fsa").AsParallel Select FASTA.FastaFile.Read(file)).ToArray
                              Dim IDLIst = (From fsa In FsaList.AsParallel Select New KeyValuePair(Of String, KeyValuePair(Of String, String)())(FileIO.FileSystem.GetFileInfo(fsa.FilePath).Name.Replace(".fsa", ""), (From obj In fsa.AsParallel Let s = obj.Title.Split Select New KeyValuePair(Of String, String)(s(1), obj.Title)).ToArray)).ToArray

                              For i As Integer = 1 To csvFile.Count - 1
                                  Dim row = csvFile(i)

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

                              Call csvFile.Save(path)
                          End Sub)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="regprecise">数据库 文件路径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetKeywords(regprecise As String) As KeyValuePair()
            Dim DbRegprecise = regprecise.LoadXml(Of Regtransbase.WebServices.RegPreciseTFFamily)()
            Dim List As List(Of String) = New List(Of String)
            For Each family In DbRegprecise.Family
                Dim Collection = ((From item In family.Regulogs.Logs Select item.Regulog.Key).ToArray)
                Call List.AddRange(Collection)
            Next

            Return (From str As String
                    In (From ss As String In List Select ss Distinct).ToArray
                    Let tokens As String() = str.Split(CChar("-"))
                    Select New KeyValuePair With {
                        .Key = Trim(tokens(0)),
                        .Value = Trim(tokens(1))}).ToArray
        End Function

        Public Function SequenceNotDownload(regprecise As String, DownloadSeq As FASTA.FastaFile) As KeyValuePair()
            Dim AllList = GetKeywords(regprecise).AsList
            Dim DonwloadList = (From fsa In DownloadSeq Let id = fsa.Headers.Last Let tokens = Strings.Split(id, " - ") Select New KeyValuePair With {.Key = tokens(0), .Value = tokens(1)}).ToArray

            For Each iten In DonwloadList
                Call AllList.RemoveAll(Function(n As KeyValuePair) iten.Equals(n, strict:=False))
            Next

            Dim result = KeyValuePair.Distinct(AllList.ToArray)
            Return result
        End Function

        Public Sub Action(list As KeyValuePair(), savedFile As String)
            Dim NCBIProteins = New SMRUCC.genomics.Assembly.NCBI.Entrez.Protein()
            Dim FsaList As SMRUCC.genomics.SequenceModel.FASTA.FastaFile = New SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Dim attrList As List(Of String) = New List(Of String)

            For Each item In list
                Dim Entries = NCBIProteins.GetEntry(item.Key, item.Value, MaxLimited:=3)

                If Entries.IsNullOrEmpty Then
                    Continue For
                End If

                Dim Locus_Tag = GetMaxItem((From entry In Entries Let s = Trim(entry.LocusTag) Where Not String.IsNullOrEmpty(s) Select s Distinct).ToArray)
                Dim entryList As SMRUCC.genomics.Assembly.KEGG.WebServices.QueryEntry()
                If String.IsNullOrEmpty(Locus_Tag) Then
                    entryList = SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.HandleQuery(item.Key) '模糊查找多条记录
                    Dim LQuery = (From entry In entryList Where String.Equals(entry.speciesID, "eco") Select entry).ToArray

                    If Not LQuery.IsNullOrEmpty Then
                        Dim fsa = SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.FetchSeq(LQuery.First)
                        attrList = fsa.Headers.AsList
                        attrList.Add(item.Key & " - " & item.Value)

                        fsa.Headers = attrList.ToArray
                        Call FsaList.Add(fsa)
                    End If
                Else
                    entryList = SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.HandleQuery(Locus_Tag) '只有一条记录

                    If Not entryList.IsNullOrEmpty Then
                        Dim fsa = SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.FetchSeq(entryList.First)
                        attrList = fsa.Headers.AsList
                        attrList.Add(item.Key & " - " & item.Value)

                        fsa.Headers = attrList.ToArray
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

        ' ''' <summary>
        ' ''' 对目标基因组做
        ' ''' </summary>
        ' ''' <param name="LocalBlast"></param>
        ' ''' <param name="WorkDir"></param>
        ' ''' <param name="Query"></param>
        ' ''' <param name="Subject"></param>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Function LocalBlastp(LocalBlast As SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Programs.LocalBLAST, WorkDir As String, Query As String, Subject As String) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        '    Call LocalBlast.FormatDb(Query, LocalBlast.MolTypeProtein).Start(WaitForExit:=True)
        '    Call LocalBlast.FormatDb(Subject, LocalBlast.MolTypeProtein).Start(WaitForExit:=True)

        '    Call LocalBlast.Blastp(Query, Subject, WorkDir & "/blastp_logs_qvss.txt", "1").Start(WaitForExit:=True)
        '    Dim Log = SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput.TryParse(LocalBlast.LastBLASTOutputFilePath)
        '    Call Log.Grep(AddressOf SMRUCC.genomics.NCBI.Extensions.LocalBLAST.GrepScript.Compile("'match gene[=][^]]+';'tokens = 1'").Grep,
        '                           AddressOf SMRUCC.genomics.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens | 1").Grep)

        '    Dim File = Log.ExportBestHit
        '    For i As Integer = 1 To File.Count - 1
        '        Dim row = File(i)
        '        row(1) = TrimF(row(1))
        '    Next

        '    Call File.Save("x:\xan8004vregprecise.csv")

        '    Call LocalBlast.Blastp(Subject, Query, WorkDir & "/blastp_logs_svsq.txt", "1").Start(WaitForExit:=True)
        '    Log = SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput.TryParse(LocalBlast.LastBLASTOutputFilePath)
        '    Call Log.Grep(AddressOf SMRUCC.genomics.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens | 1").Grep,
        '                  AddressOf SMRUCC.genomics.NCBI.Extensions.LocalBLAST.GrepScript.Compile("'match gene[=][^]]+';'tokens = 1'").Grep)

        '    Dim File2 = Log.ExportBestHit
        '    For i As Integer = 1 To File.Count - 1
        '        Dim row = File(i)
        '        row(0) = Trim(row(0))
        '    Next

        '    Call File2.Save("x:\regprecisevxan8004.csv")

        '    Dim best = GetDiReBh(File2, File, SMRUCC.genomics.SequenceModel.FASTA.File.Read(Query), SMRUCC.genomics.SequenceModel.FASTA.File.Read(Subject))

        '    Return best
        'End Function

        'Public Function TrimF(strText As String) As String
        '    If String.IsNullOrEmpty(Trim(strText)) Then
        '        Return " "
        '    End If
        '    Dim Tokens = Split(strText, " - ")
        '    Return String.Format("{0} - {1}", Trim(Tokens(0)), Trim(Tokens(1)))
        'End Function
    End Module
End Namespace
