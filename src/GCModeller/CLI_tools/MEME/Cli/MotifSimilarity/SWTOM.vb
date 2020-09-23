#Region "Microsoft.VisualBasic::762165afe48202de46214e224023b25a, CLI_tools\MEME\Cli\MotifSimilarity\SWTOM.vb"

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
    '     Function: __SWQueryCommon, BatchCopy, BatchCopyDIR, CompareMotif, ExportMotifDraw
    '               MEME2LDM, SiteScan, SWTomCompares, SWTomComparesBatch, SWTomLDM
    '               SWTomQuery, SWTomQueryBatch
    '     Class __batchTask
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DATA
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.GCModeller.Workbench
Imports SMRUCC.genomics.Interops.NBCR
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Copys.DIR", Usage:="/Copys.DIR /in <inDIR> /out <outDIR>")>
    Public Function BatchCopyDIR(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args("/out")
        Dim inDIRS = FileIO.FileSystem.GetDirectories(inDIR)
        Dim lst = FileIO.FileSystem.GetDirectories(inDIRS.First).Select(Function(x) FileIO.FileSystem.GetDirectoryInfo(x).Name)

        For Each DIR As String In inDIRS
            Dim Name As String = FileIO.FileSystem.GetDirectoryInfo(DIR).Name
            For Each [sub] As String In lst
                Dim source As String = DIR & "/" & [sub]
                Dim copyTo As String = out & "/" & [sub] & "/" & Name
                Call FileIO.FileSystem.CopyDirectory(source, copyTo)
            Next
        Next

        Return 0
    End Function

    <ExportAPI("/Copys", Usage:="/Copys /in <inDIR> [/out <outDIR> /file <meme.txt>]")>
    Public Function BatchCopy(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".MEME/")
        Dim file As String = args.GetValue("/file", "meme.txt")
        Dim files = FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchAllSubDirectories, file)
        Dim ext As String = file.Split("."c).Last

        Call FileIO.FileSystem.CreateDirectory(out)

        For Each path As String In files
            Dim copyTo As String = out & "/" & FileIO.FileSystem.GetDirectoryInfo(FileIO.FileSystem.GetParentPath(path)).Name & "." & ext
            Call FileIO.FileSystem.CopyFile(path, copyTo)
        Next

        Return 0
    End Function

    <ExportAPI("/SWTOM.LDM", Usage:="/SWTOM.LDM /query <ldm.xml> /subject <ldm.xml> [/out <outDIR> /method <pcc/ed/sw; default:=pcc>]")>
    Public Function SWTomLDM(args As CommandLine) As Integer
        Dim inQuery As String = args("/query")
        Dim inSubject As String = args("/subject")
        Dim method As String = args.GetValue("/method", "pcc")
        Dim out As String = args.GetValue("/out", inQuery.TrimSuffix & "-" & BaseName(inSubject) & "." & method)
        Dim query = inQuery.LoadXml(Of AnnotationModel)
        Dim subject = inSubject.LoadXml(Of AnnotationModel)
        Dim result = SWTom.Compare(query, subject, method)
        Return TomReport.WriteHTML(result, out)
    End Function

    <ExportAPI("/SWTOM.Query", Usage:="/SWTOM.Query /query <meme.txt> [/out <outDIR> /method <pcc> /bits.level 1.6 /minW 6 /no-HTML]")>
    <Argument("/no-HTML", True,
                   Description:="If this parameter is true, then only the XML result will be export.")>
    Public Function SWTomQuery(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim out As String = args.GetValue("/out", query.TrimSuffix)
        Dim method As String = args.GetValue("/method", "pcc")
        Dim bitsLevel As Double = args.GetValue("/bits.level", 1.6)
        Dim minW As Integer = args.GetValue("/minW", 6)
        Dim noHTML As Boolean = args.GetBoolean("/no-HTML")
        Dim params As New Parameters

        params.Method = method
        params.BitsLevel = bitsLevel
        params.MinW = minW

        Call __SWQueryCommon(query, params, noHTML, out)

        Return 0
    End Function

    Private Function __SWQueryCommon(query As String,
                                     params As Parameters,
                                     noHTML As Boolean,
                                     out As String, Optional ByRef hits As MotifHit() = Nothing) As CompareResult()
        Dim result = SWTom.CompareBest(query, params)

        Call $"Motifs computing done! {result.Length} hits...".__DEBUG_ECHO

        If noHTML Then
            Call (From hit As MEME_Suite.Analysis.Similarity.TOMQuery.Output
                  In result
                  Let outXml As String = out & $"/{hit.Query.Uid}-{hit.Subject.Uid}.xml"
                  Select hit.SaveAsXml(outXml, throwEx:=False)).ToArray
        Else
            Call (From hit As MEME_Suite.Analysis.Similarity.TOMQuery.Output
           In result
                  Let outDIR As String = out & $"/{hit.Query.Uid}-{hit.Subject.Uid}/"
                  Select TomReport.WriteHTML(hit, outDIR)).ToArray
        End If

        Dim LQuery = (From x As Output
                      In result
                      Select x.HSP.Select(Function(hsp) CreateResult(x.Query, x.Subject, hsp.Alignment))).Unlist
        Call LQuery.SaveTo(out & "/Query.Csv")

        hits = result.Select(Function(x) MotifHit.CreateObject(x))

        Dim index As New StringBuilder
        Call index.AppendLine(LQuery.ToHTMLTable)

        For Each hit As CompareResult In LQuery
            Call index.Replace($"<td>{hit.HitName}</td>", $"<td><a href=""{hit.QueryName}-{hit.HitName}/TomQuery.html"">{hit.HitName}</a></td>")
        Next

        Call ReportBuilder.SaveAsHTML(index, out & "/index.html").CLICode

        Return LQuery.ToArray
    End Function

    <ExportAPI("/SWTOM.Compares.Batch", Usage:="/SWTOM.Compares.Batch /query <query.meme.DIR> /subject <subject.meme.DIR> [/out <outDIR> /no-HTML]")>
    Public Function SWTomComparesBatch(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim outDIR As String = args.GetValue("/out", query.TrimSuffix & "." & BaseName(subject))
        Dim subjects = subject.LoadSourceEntryList({"*.txt"})
        Dim params As New Parameters
        Dim results As New List(Of Output)

        For Each qx As String In FileIO.FileSystem.GetFiles(query, FileIO.SearchOption.SearchTopLevelOnly, "*.txt")
            Dim queryLDM = AnnotationModel.LoadDocument(qx)
            Dim qName As String = BaseName(qx)
            Dim subjectLDM = (From file In subjects Where InStr(file.Key, qName) = 1 Select AnnotationModel.LoadDocument(file.Value)).Unlist

            For Each x In queryLDM
                For Each y In subjectLDM
                    Dim result As Output = SWTom.Compare(x, y, params)
                    Dim out As String = outDIR & $"/{x.Uid}.{y.Uid}.Xml"
                    Call result.SaveAsXml(out)
                    Call results.Add(result)
                Next
            Next
        Next

        Dim LQuery = (From x As Output
                      In results
                      Select x.HSP.Select(Function(hsp) CreateResult(x.Query, x.Subject, hsp.Alignment))).Unlist
        Call LQuery.SaveTo(outDIR & "/Compares.Csv")

        Dim hits = results.Select(Function(x) MotifHit.CreateObject(x))
        Return hits.SaveTo(outDIR & "/MotifHits.Csv")
    End Function

    <ExportAPI("/SWTOM.Compares", Usage:="/SWTOM.Compares /query <query.meme.txt> /subject <subject.meme.txt> [/out <outDIR> /no-HTML]")>
    Public Function SWTomCompares(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim outDIR As String = args.GetValue("/out", query.TrimSuffix & "." & BaseName(subject))
        Dim queryLDM = AnnotationModel.LoadDocument(query)
        Dim subjectLDM = AnnotationModel.LoadDocument(subject)
        Dim params As New Parameters
        Dim results As New List(Of Output)

        For Each x In queryLDM
            For Each y In subjectLDM
                Dim result As Output = SWTom.Compare(x, y, params)
                Dim out As String = outDIR & $"/{x.Uid}.{y.Uid}.Xml"
                Call result.SaveAsXml(out)
                Call results.Add(result)
            Next
        Next

        Dim LQuery = (From x As Output
                      In results
                      Select x.HSP.Select(Function(hsp) CreateResult(x.Query, x.Subject, hsp.Alignment))).Unlist
        Call LQuery.SaveTo(outDIR & "/Compares.Csv")

        Dim hits = results.Select(Function(x) MotifHit.CreateObject(x))

        Return hits.SaveTo(outDIR & "/MotifHits.Csv")
    End Function

    ''' <summary>
    ''' bits.level越低则条件越苛刻
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks>默认参数已经是经过RegulatorsMotif测试过的</remarks>
    <ExportAPI("/SWTOM.Query.Batch",
               Usage:="/SWTOM.Query.Batch /query <meme.txt.DIR> [/out <outDIR> /SW-offset 0.6 /method <pcc> /bits.level 1.5 /minW 4 /SW-threshold 0.75 /tom-threshold 0.75 /no-HTML]")>
    <Argument("/no-HTML", True,
                   Description:="If this parameter is true, then only the XML result will be export.")>
    Public Function SWTomQueryBatch(args As CommandLine) As Integer
        Dim inDIR As String = args("/query")
        Dim out As String = args.GetValue("/out", inDIR & "_SW-TOM.OUT/")
        Dim method As String = args.GetValue("/method", "pcc")
        Dim bitsLevel As Double = args.GetValue("/bits.level", 1.5)
        Dim minW As Integer = args.GetValue("/minW", 4)
        Dim swThres As Double = args.GetValue("/sw-threshold", 0.75)
        Dim tomThres As Double = args.GetValue("/tom-threshold", 0.75)
        Dim noHTML As Boolean = args.GetBoolean("/no-html")
        Dim swOffsets As Double = args.GetValue("/sw-offset", 0.6)
        Dim params As New Parameters With {
            .Method = method,
            .BitsLevel = bitsLevel,
            .MinW = minW,
            .SWThreshold = swThres,
            .TOMThreshold = tomThres,
            .SWOffset = swOffsets
        }

        Dim BatchTask = (From memeText As String
                         In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.txt").AsParallel
                         Select New __batchTask(
                             memeText,
                             params,
                             noHTML,
                             out)).ToArray
        Call BatchTask.Select(Function(x) x.out).Unlist.SaveTo(out & "/SW-TOM.Query.csv")
        Call BatchTask.Select(Function(x) x.hits).Unlist.SaveTo(out & "/SW-TOM.Hits.Csv")

        Return 0
    End Function

    Private Class __batchTask
        Public hits As MotifHit() = Nothing
        Public out As CompareResult() = Nothing

        Sub New(memeText As String, params As Parameters, noHTML As Boolean, out As String)
            Dim sId As String = BaseName(memeText)
            Me.out = __SWQueryCommon(memeText, params, noHTML, out & "/" & sId, Me.hits)
        End Sub
    End Class

    <ExportAPI("/site.scan", Usage:="/site.scan /query <LDM.xml> /subject <subject.fasta> [/out <outDIR>]")>
    Public Function SiteScan(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim out As String = args.GetValue("/out", query.TrimSuffix & "-" & BaseName(subject))
        Dim profiles = Parameters.SiteScanProfile
        Dim reuslt = SiteScanner.Scan(query.LoadXml(Of AnnotationModel), New FastaSeq(subject), profiles)
        Return TomReport.WriteHTML(reuslt, outDIR:=out).CLICode
    End Function

    <ExportAPI("/MEME.LDMs", Usage:="/MEME.LDMs /in <meme.txt> [/out <outDIR>]")>
    Public Function MEME2LDM(args As CommandLine) As Integer
        Dim inMEME As String = args("/in")
        Dim outDIR As String = args.GetValue("/out", inMEME.TrimSuffix & ".LDMs/")
        Dim LDMs = AnnotationModel.LoadDocument(inMEME)

        For Each x As AnnotationModel In LDMs
            Dim out As String = outDIR & x.Uid.NormalizePathString & ".Xml"
            Call x.SaveAsXml(out)
        Next

        Return 0
    End Function

    <ExportAPI("/LDM.Compares",
               Usage:="/LDM.Compares /query <query.LDM.Xml> /sub <subject.LDM.Xml> [/out <outDIR>]")>
    Public Function CompareMotif(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim subj As String = args("/sub")
        Dim out As String = args.GetValue("/out", query.TrimSuffix & "." & subj.BaseName & ".Compares/")
        Dim queryLDM As AnnotationModel = query.LoadXml(Of AnnotationModel)
        Dim subLDM As AnnotationModel = subj.LoadXml(Of AnnotationModel)
        Dim swTOM_OUT As Output = SWTom.Compare(queryLDM, subLDM, cutoff:=0.4, tomThreshold:=0.4, bitsLevel:=2)

        Call TomReport.WriteHTML(swTOM_OUT, out & "/SW-TOM/")

        Return 0
    End Function

    ''' <summary>
    ''' 生成论文表格
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("/EXPORT.MotifDraws",
               Usage:="/EXPORT.MotifDraws /in <virtualFootprints.csv> /MEME <meme.txt.DIR> /KEGG <KEGG_Modules/Pathways.DIR> [/pathway /out <outDIR>]")>
    Public Function ExportMotifDraw(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".MotifDraws/")
        Dim cbUids = (From x As PredictedRegulationFootprint
                      In inFile.LoadCsv(Of PredictedRegulationFootprint)
                      Where Not InStr(x.MotifTrace, "@") > 0
                      Let MotifTrace As String = Val(x.tag) & ".MEME::" & x.MotifTrace.Replace("::", ".")
                      Select MotifTrace,
                              x.MotifFamily,
                              uid = x.MotifFamily & x.MotifTrace
                      Group By uid Into Group)
        Dim pathway As Boolean = args.GetBoolean("/pathway")
        Dim footprints = (From x In cbUids
                          Select x.Group.First
                          Group First By First.MotifTrace Into Group) _
                               .ToDictionary(Function(x) x.MotifTrace,
                                             Function(x) x.Group.Select(Function(xx) xx.MotifFamily).ToArray)
        Dim pheno As ModuleClassAPI =
            If(pathway,
            ModuleClassAPI.FromPathway(args("/KEGG")),
            ModuleClassAPI.FromModules(args("/KEGG")))
        Dim memes As Dictionary(Of AnnotationModel) =
            AnnotationModel.LoadMEMEOUT(args("/meme"), full:=True)
        Return MotifDraws.CreateViews(footprints, memes, EXPORT:=out, pheno:=pheno)
    End Function
End Module
