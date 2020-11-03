#Region "Microsoft.VisualBasic::f6facfe1a497022e4aeb7086a5d01e78, CLI_tools\mpl\CLI\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: __getSBHhash, AlignPfam, DumpPfamString, ExportAppSBH, FamilyAlignmentTest
    '               MPAlignment, MPAlignment2, SelectPfams, SubPfam, ViewAlignment
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Visualize.NCBIBlastResult

<Package("MPAlignment.CLI", Category:=APICategories.CLI_MAN)>
Module CLI

    <ExportAPI("/Pfam.Sub", Usage:="/Pfam.Sub /index <bbh_index.csv> /pfam <pfam-string> [/out <sub-out.csv>]")>
    Public Function SubPfam(args As CommandLine) As Integer
        Dim inFile As String = args("/index")
        Dim pfam As String = args("/pfam")
        Dim out As String = args.GetValue("/out", inFile.Trim & ".Pfam-String.Csv")
        Dim index As String() = (From x In inFile.LoadCsv(Of BBHIndex) Select x.QueryName Distinct).ToArray
        Dim pfamString = pfam.LoadCsv(Of PfamString).ToDictionary(Function(x) x.ProteinId)
        Dim subs As PfamString() = (From x As String In index Where pfamString.ContainsKey(x) Select pfamString(x)).ToArray
        Return subs.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Pfam.Align",
               Info:="Align your proteins with selected protein domain structure database by using blast+ program.",
               Usage:="/Pfam.Align /query <query.fasta> [/db <name/path> /out <blastOut.txt>]")>
    Public Function AlignPfam(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim alignDb As String = args("/db")
        alignDb = GCModeller.FileSystem.GetPfamDb(alignDb)

        If String.IsNullOrEmpty(alignDb) Then
            Return -100
        End If

        Dim out As String = args.GetValue("/out", $"{query.TrimSuffix}.vs__.{basename(alignDb)}.txt")
        Dim localblast As New Programs.BLASTPlus(GCModeller.FileSystem.GetLocalblast)
        Call localblast.FormatDb(alignDb, localblast.MolTypeProtein).Start(waitForExit:=True)
        Call localblast.Blastp(query, alignDb, out, Settings.SettingsFile.GetMplParam.Evalue).Start(waitForExit:=True)
        Return 0
    End Function

    ''' <summary>
    ''' 从blastp结果之中解析出结构域数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Pfam-String.Dump",
               Usage:="/Pfam-String.Dump /In <blastp_out.txt> [/out <pfam-String.csv> /evalue <0.001> /identities <0.2> /coverage <0.85>]",
               Info:="Dump the pfam-String domain Structure composition information from the blastp alignment result.")>
    <ArgumentAttribute("/In", False,
                   Description:="The blastp output For the protein alignment, which the aligned database can be selected from Pfam-A Or NCBI CDD. And blast+ program Is recommended For used For the domain alignment.")>
    <ArgumentAttribute("/out", True,
                   Description:="The output Excel .csv data file path For the dumped pfam-String data Of your annotated protein. If this parameter Is empty, Then the file will saved On the same location With your blastp input file.")>
    Public Function DumpPfamString(args As CommandLine) As Integer
        Dim inFile As String = args("/In")
        Dim outFile As String = args.GetValue("/out", inFile.TrimSuffix & ".Pfam-String.Csv")
        Dim Settings = xMPAlignment.Settings.Session.Initialize.GetMplParam
        Dim BlastOut = BlastPlus.Parser.ParsingSizeAuto(inFile)
        Dim PfamString = Pfam.CreatePfamString(
            BlastOut,
            disableUltralarge:=True,
            num_threads:=Settings.ParserThreads,
            timeOut:=Settings.ParserTimeOut,
            coverage:=Settings.Coverage,
            identities:=Settings.Identities,
            evalue:=Settings.Evalue,
            offset:=Settings.Offset)
        Return PfamString.SaveTo(outFile)
    End Function

    ''' <summary>
    ''' 导出所有的单向比对的结果
    ''' 宽松 0.5 coverage / 0.15 identities
    ''' 严格 0.8 coverage / 0.5 identities
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--blast.allhits", Usage:="--blast.allhits /blast <blastp.txt> [/out <sbh.csv> /coverage <0.5> /identities 0.15]")>
    Public Function ExportAppSBH(args As CommandLine) As Integer
        Dim inFile As String = args("/blast")
        Dim coverage As Double = args.GetValue("/coverage", 0.5)
        Dim identities As Double = args.GetValue("/identities", 0.15)
        Dim blastOut = BlastPlus.Parser.TryParse(inFile)
        Dim allHits = blastOut.ExportAllBestHist(coverage:=coverage, identities_cutoff:=identities)
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".sbh.Csv")
        Return allHits.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 使用自定义数据库来比对
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--align",
               Usage:="--align /query <pfam-string.csv> /subject <pfam-string.csv> [/hits <query_vs_sbj.blastp.csv> /flip-bbh /out <alignment_out.csv> /mp <cutoff:=0.65> /swap /parts]",
               Info:="MPAlignment on your own dataset.")>
    <ArgumentAttribute("/swap", True,
                   Description:="Swap the location of query and subject in the output result set.")>
    <ArgumentAttribute("/parts", True,
                   Description:="Does the domain motif equals function determine the domain positioning equals just if one side in the high scoring then thoese two domain its position is equals? 
Default is not, default checks right side and left side.")>
    <ArgumentAttribute("/flip-bbh", True, Description:="Swap the direction of the query_name/hit_name in the hits?")>
    Public Function MPAlignment(args As CommandLine) As Integer
        Dim queryFile As String = args("/query")
        Dim subjectFile As String = args("/subject")
        Dim query = queryFile.LoadCsv(Of PfamString)
        Dim subject = (From x As PfamString In subjectFile.LoadCsv(Of PfamString)
                       Where Not x.PfamString.IsNullOrEmpty
                       Select x, proteinId = x.ProteinId.Split(":"c).Last
                       Group By proteinId Into Group) _
                            .ToDictionary(Function(x) x.proteinId,
                                          Function(x) x.Group.First.x)
        Dim out As String = args.GetValue("/out", queryFile.TrimSuffix & $"_vs_{subjectFile.BaseName}.Csv")
        Dim mpCutoff As Double = args.GetValue("/mp", 0.65)
        Dim sbh As Dictionary(Of String, String()) = __getSBHhash(args("/hits"), query, subject.Values, args.GetBoolean("/flip-bbh"))
        Dim equals As New DomainEquals(mpCutoff, args.GetBoolean("/parts"))
        Dim LQuery = (From x As PfamString
                      In query.AsParallel
                      Where sbh.ContainsKey(x.ProteinId)
                      Let lstId As String() = sbh(x.ProteinId)
                      Let lstPfam = lstId.Where(Function(sId) subject.ContainsKey(sId)).Select(Function(sId) subject(sId)).ToArray
                      Select (From sbj As PfamString
                              In lstPfam
                              Select PfamStringEquals(x, sbj, equals))).IteratesALL
        Dim swap As Boolean = args.GetBoolean("/swap")
        Dim resultOut As MPCsvArchive() = LQuery.Select(Function(x) x.ToRow)

        If resultOut.IsNullOrEmpty Then  ' 由于没有作任何筛选，但是LQuery是空的，则说明字典不对
            Call VBDebugger.Warning(ResultNullWarning)
        End If

        If swap Then
            For i As Integer = 0 To resultOut.Length - 1
                Dim row = resultOut(i)
                Call row.QueryName.Swap(row.HitName)
            Next
        End If

        Return resultOut.SaveTo(out).CLICode
    End Function

    Const ResultNullWarning As String =
        "Null mpl data, its your hits data correct?? Try swap query and hits in the Excel or enable the /flip-bbh option in the commandline and then try this alignment again!"

    ''' <summary>
    ''' 测试用
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--align.Family_test", Usage:="--align.Family_test /query <pfam-string> /name <dbName/Path> [/threshold <0.65> /mpCut <0.65> /accept <10>]")>
    Public Function FamilyAlignmentTest(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim name As String = args("/name")
        Dim queryPfam = Pfam.PfamString.CLIParser(query)
        Dim Database As New SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem.Database
        Dim FamilyDb = Database.GetDatabase(name)
        Dim threshold As Double = args.GetValue("/threshold", 0.65)
        Dim mpCut As Double = args.GetValue("/mpCut", 0.65)
        Dim accept As Integer = args.GetValue("/accept", 10)
        Dim align = FamilyDb.Classify(queryPfam, threshold, mpCut, accept)
        Call align.JoinBy("; ").__DEBUG_ECHO
        Return 0
    End Function

    ''' <summary>
    ''' 测试用
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--align.String",
               Usage:="--align.String /query <pfam-string> /subject <pfam-string> [/mp <cutoff:=0.65> /out <outDIR> /parts]",
               Info:="MPAlignment test, pfam-string value must be in format as  <locusId>:<length>:<pfam-string>")>
    Public Function MPAlignment2(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim mpCutoff As Double = args.GetValue("/mp", 0.65)
        Dim queryPfam = Pfam.PfamString.CLIParser(query)
        Dim subjectPfam = Pfam.PfamString.CLIParser(subject)
        Dim outAlign = PfamStringEquals(queryPfam, subjectPfam, mpCutoff, args.GetBoolean("/parts"))
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & $"/{queryPfam.ProteinId}_.{subjectPfam.ProteinId}/")
        Call outAlign.HTMLVisualize.SaveTo(out & "/LevAlign.html")
        Call outAlign.SaveAsXml(out & "/mpl.Xml")
        Call {outAlign.ToRow}.SaveTo(out & "/mpl.Csv")
        Return 0
    End Function

    ''' <summary>
    ''' Query, Subject()
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Private Function __getSBHhash(path As String,
                                  query As IEnumerable(Of PfamString),
                                  subject As IEnumerable(Of PfamString),
                                  flip As Boolean) As Dictionary(Of String, String())

        If Not path.FileExists Then
            Call "Hits data is not presents... select all data!".__DEBUG_ECHO
            Dim sbj As String() = subject.Select(Function(x) x.ProteinId)
            Dim dict = query.ToDictionary(Function(x) x.ProteinId, Function(null) sbj) ' 没有sbh数据的筛选，则默认比对全部的数据
            Return dict
        End If

        Dim sbhFile As IEnumerable(Of BBHIndex) = path.LoadCsv(Of BBHIndex)

        If flip Then
            For Each x As BBHIndex In sbhFile
                Call x.QueryName.Swap(x.HitName)
            Next
        End If

        Dim LQuery = (From x As BBHIndex
                      In sbhFile
                      Select x
                      Group x By x.QueryName Into Group) _
                         .ToDictionary(Function(x) x.QueryName.Split(":"c).Last,
                                       Function(x)
                                           Return x.Group.Where(Function(xx) xx.isMatched).Select(Function(xx) xx.HitName.Split(":"c).Last).ToArray
                                       End Function)
        Return LQuery
    End Function

    <ExportAPI("--View.Alignment", Usage:="--View.Alignment /blast <blastp.txt> /name <queryName> [/out <out.png>]")>
    Public Function ViewAlignment(args As CommandLine) As Integer
        Dim inFile As String = args("/blast")
        Dim name As String = args("/name")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & "_" & name.NormalizePathString & ".png")
        Dim blast = BlastPlus.Parser.TryParse(inFile)
        Dim res As Image = BlastVisualize.InvokeDrawing(blast, name)
        If res Is Nothing Then
            Return -1
        Else
            Call res.SaveAs(out, ImageFormats.Png)
            Return 0
        End If
    End Function

    <ExportAPI("/Select.Pfam-String",
               Usage:="/Select.Pfam-String /in <pfam-string.csv> /hits <bbh/sbh.csv> [/hit_name /out <out.csv>]")>
    Public Function SelectPfams(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim hits As String = args - "/hits"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & hits.BaseName & ".csv")
        Dim pfamString As List(Of PfamString) = [in].LoadCsv(Of PfamString)
        Dim hhits As List(Of BBHIndex) = hits.LoadCsv(Of BBHIndex)
        Dim names As String() = If(args.GetBoolean("/hit_name"),
            hhits.Select(Function(x) x.HitName.Split(":"c).Last),
            hhits.Select(Function(x) x.QueryName.Split(":"c).Last))
        Dim LQuery = LinqAPI.MakeList(Of PfamString) <=
            From x As PfamString
            In pfamString
            Where Array.IndexOf(names, x.ProteinId.Split(":"c).Last) > -1
            Select x

        Return LQuery > out
    End Function
End Module
