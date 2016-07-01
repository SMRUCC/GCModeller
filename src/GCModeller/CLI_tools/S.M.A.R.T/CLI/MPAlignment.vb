Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Partial Module CLI

    ''' <summary>
    ''' SBH -> MPAlignment，这个方法特别适用于调控因子的同源比对操作
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--MPAlignment", Usage:="--MPAlignment /sbh <sbh.csv> /query <pfam-string.csv> /subject <pfam-string.csv> [/mp <0.65> /out <out.csv>]")>
    Public Function SBHAlignment(args As CommandLine.CommandLine) As Integer
        Dim sbh = args("/sbh").LoadCsv(Of SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)
        Dim query = args("/query").LoadCsv(Of Sanger.Pfam.PfamString.PfamString).ToDictionary(Function(x) x.ProteinId)
        Dim subject = args("/subject").LoadCsv(Of Sanger.Pfam.PfamString.PfamString).ToDictionary(Function(x) x.ProteinId)
        Dim MP As Double = args.GetValue("/mp", 0.65)
        Dim LQuery = (From hit As SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit
                      In sbh
                      Where query.ContainsKey(hit.QueryName) AndAlso
                          subject.ContainsKey(hit.HitName)
                      Select Sanger.Pfam.ProteinDomainArchitecture.MPAlignment.PfamStringEquals(query(hit.QueryName), subject(hit.HitName), MP)).ToArray
        Dim out As String = args.GetValue("/out", args("/sbh").TrimFileExt & ".MPAlignment.csv")
        Return AlignmentOutput2Csv(LQuery).SaveTo(out).CLICode
    End Function

    <ExportAPI("--align", Usage:="--align /query <query.csv> /subject <subject.csv> [/out <out.DIR> /inst]")>
    Public Function Align(args As CommandLine.CommandLine) As Integer
        If args.GetBoolean("/inst") Then
            Return __alignInst(args)
        End If

        Dim query = args("/query").LoadCsv(Of Sanger.Pfam.PfamString.PfamString)
        Dim subject = args("/subject").LoadCsv(Of Sanger.Pfam.PfamString.PfamString)
        Dim out As String = args.GetValue("/out", App.CurrentDirectory)
        Dim resultSet As New List(Of LevAlign)

        out = $"{out}/{IO.Path.GetFileNameWithoutExtension(args("/query"))}_vs.{IO.Path.GetFileNameWithoutExtension(args("/subject"))}"

        Dim outHTML As String = out & "/HTML/"

        For Each prot As PfamString.PfamString In query
            Dim alignInvoke As LevAlign() = (From b As PfamString.PfamString
                                             In subject.AsParallel
                                             Select Sanger.Pfam.ProteinDomainArchitecture.MPAlignment.PfamStringEquals(prot, b, 0.8)).ToArray

            resultSet += alignInvoke

            For Each paired In alignInvoke
                Dim html As String = paired.Visualize
                Dim path As String = outHTML & $"/{paired.QueryPfam.ProteinId.NormalizePathString}_vs.{paired.SubjectPfam.ProteinId.NormalizePathString}.html"
                Call html.SaveTo(path)
            Next

            Call prot.__DEBUG_ECHO
            Call FlushMemory()
        Next

        Call $"Alignment Done! Export to {out}".__DEBUG_ECHO
        Call resultSet.GetXml.SaveTo(out & "/MPAlignment.xml")
        Call resultSet.ToArray(Function(x) x.ToRow).SaveTo($"{out}/MPAlignment.csv")

        Return 0
    End Function

    Private Function __alignInst(args As CommandLine.CommandLine) As Integer
        Dim query = Sanger.Pfam.PfamString.CLIParser(args("/query"))
        Dim subject = Sanger.Pfam.PfamString.CLIParser(args("/subject"))
        Dim out As String = args.GetValue("/out", App.CurrentDirectory)
        Dim result = Algorithm.PfamStringEquals(query, subject, 0.8)

        out = $"{out}/{query.ProteinId.NormalizePathString}_vs.{subject.ProteinId.NormalizePathString}"

        Call result.GetXml.SaveTo($"{out}/MPAlignment.xml")
        Call result.Visualize.SaveTo(out & "/MPAlignment.html")

        Dim outTxt As String = result.ToString
        Call Console.WriteLine(outTxt)
        Call outTxt.SaveTo(out & "/MPAlignment.txt")

        Return 0
    End Function

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    ''' <summary>
    ''' 1. 完全比对得上
    ''' 2. 部分比对得上，但是必须要有一个结构域是和家族名称一致的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--align.family", Usage:="--align.family /In <In.bbh.csv> /query <query-pfam.csv> /subject <subject-pfam.csv> [/out <out.DIR> /mp <mp-align:0.65> /lev <lev-align:0.65>]")>
    Public Function FamilyAlign(args As CommandLine.CommandLine) As Integer
        Dim input As String = args("/in")
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim out As String = args.GetValue("/out", input.ParentDirName & "/MPAlignment/")
        Dim inBBH = input.LoadCsv(Of NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit)
        Dim align = MotifParallelAlignment.AlignProteins(inBBH, query.LoadCsv(Of Sanger.Pfam.PfamString.PfamString), subject.LoadCsv(Of Sanger.Pfam.PfamString.PfamString))
        Call align.ToArray.GetXml.SaveTo($"{out}/{IO.Path.GetFileNameWithoutExtension(input)}.xml")
        Call align.ToArray(Function(x) x.ToRow).SaveTo($"{out}/{IO.Path.GetFileNameWithoutExtension(input)}.csv")

        Dim Regprecise = GCModeller.FileSystem.KEGGFamilies.LoadCsv(Of SMRUCC.genomics.DatabaseServices.Regprecise.FastaReaders.Regulator) _
                .ToDictionary(Function(prot) prot.LocusTag) '.LoadXml(Of SMRUCC.genomics.DatabaseServices.Regprecise.WebServices.Regulations)
        Dim mp As Double = args.GetValue("/mp", 0.65)
        ' Dim lev As Double = args.GetValue("/lev", 0.65)
        Dim havMatches = (From mm In (From match In align.AsParallel
                                      Where match.NumMatches > 0 AndAlso
                                          match.Score > mp AndAlso
                                          (match.Score = 1.0R OrElse ' 完全比对的上的
                                          match.QueryPfam.__contains(Regprecise(match.SubjectPfam.ProteinId.Split(":"c).Last).KEGGFamily)) ' 或者具备有目标结构域的
                                      Select match).ToArray
                          Select mm.QueryPfam.ProteinId,
                              subjectId = mm.SubjectPfam.ProteinId
                          Group By ProteinId Into Group) _
                             .ToDictionary(Function(match) match.ProteinId,
                                           Function(match) match.Group.ToArray(Function(prot) prot.subjectId))

        Dim matchesBBH = (From match In inBBH.AsParallel
                          Where havMatches.ContainsKey(match.QueryName) AndAlso
                              Array.IndexOf(havMatches(match.QueryName), match.HitName) > -1
                          Select match).ToArray

        Call matchesBBH.SaveTo($"{out}/{IO.Path.GetFileNameWithoutExtension(input)}.bbh_matches.csv")

        Return 0
    End Function

    <Extension> Private Function __contains(prot As Sanger.Pfam.PfamString.PfamString, Family As String) As Boolean
        Dim Tokens As String() = Common.Extensions.FamilyTokens(Family)
        Dim LQuery = (From domain
                      In prot.GetDomainData(True)
                      Where Not (From fam As String In Tokens Where InStr(domain.Identifier, fam, CompareMethod.Text) > 0 Select 1).ToArray.IsNullOrEmpty
                      Select 1).ToArray
        Return Not LQuery.IsNullOrEmpty
    End Function

    <ExportAPI("--SelfAlign", Usage:="--SelfAlign /query <pfam-string.csv> /subject <subject.csv> /aln <mpalignment.csv> [/lstID <lstID.txt> /mp <0.65> /id <id>]")>
    <ParameterInfo("/lstID", True, Description:="If this parameter is not empty, then the /aln parameter will be disable")>
    <ParameterInfo("/id", True,
                          Description:="If this parameter is not null, then the record of this query or hits will be used to subset the alignment set.")>
    Public Function SelfAlign(args As CommandLine.CommandLine) As Integer
        Dim id As String = args("/id")
        Dim queryFile As String = args("/query")
        Dim subject = args("/subject").LoadCsv(Of Sanger.Pfam.PfamString.PfamString)
        Dim query = queryFile.LoadCsv(Of Sanger.Pfam.PfamString.PfamString)
        Dim cut As Double = args.GetValue("/mp", 0.65)
        Dim lstId As String()
        Dim idFile As String = args("/lstID")
        Dim path As String

        If Not String.IsNullOrEmpty(idFile) Then
            lstId = IO.File.ReadAllLines(idFile)
            path = idFile.TrimFileExt & ".SelfAlign.csv"
            GoTo GET_ID
        Else
            path = args("/aln").TrimFileExt & ".SelfAlign.csv"
        End If

        Dim aln = args("/aln").LoadCsv(Of SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment.MPCsvArchive)

        If String.IsNullOrEmpty(id) Then
            lstId = aln.ToArray(Function(x) x.QueryName).Join(aln.ToArray(Function(x) x.HitName)).Distinct.ToArray
        Else
            Dim ql = aln.ToArray(Function(x) x.QueryName)
            Dim subSet As Sanger.Pfam.ProteinDomainArchitecture.MPAlignment.MPCsvArchive()

            If Array.IndexOf(ql, id) > -1 Then
                subSet = (From x In aln Where String.Equals(id, x.QueryName, StringComparison.OrdinalIgnoreCase) Select x).ToArray
            Else
                subSet = (From x In aln Where String.Equals(id, x.HitName, StringComparison.OrdinalIgnoreCase) Select x).ToArray
            End If

            lstId = subSet.ToArray(Function(x) x.QueryName).Join(subSet.ToArray(Function(x) x.HitName)).Distinct.ToArray
        End If

GET_ID:
        Dim pfSubSet As Sanger.Pfam.PfamString.PfamString() =
            (From x In query Where Array.IndexOf(lstId, x.ProteinId) > -1 Select x).ToArray
        Call pfSubSet.Add((From x In subject Where Array.IndexOf(lstId, x.ProteinId) > -1 Select x).ToArray)
        Dim alnResult = (From x In pfSubSet Select (From y In pfSubSet Select Sanger.Pfam.ProteinDomainArchitecture.MPAlignment.PfamStringEquals(x, y, cut)).ToArray).ToArray.MatrixToList
        Return Sanger.Pfam.ProteinDomainArchitecture.MPAlignment.AlignmentOutput2Csv(alnResult).SaveTo(path)
    End Function
End Module