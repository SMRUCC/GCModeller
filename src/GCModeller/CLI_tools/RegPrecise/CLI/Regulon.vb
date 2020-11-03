#Region "Microsoft.VisualBasic::99151f8d3fd011416a2fa16a79d1a1a6, CLI_tools\RegPrecise\CLI\Regulon.vb"

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
    '     Function: ExportRegpreciseMotifSites, GetSites, RegulatorsBBh, RunMatches
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Gets.Sites.Genes",
               Usage:="/Gets.Sites.Genes /in <tf.bbh.csv> /sites <motiflogs.csv> [/out <out.csv>]")>
    Public Function GetSites(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim sites As String = args("/sites")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"-{sites.BaseName}.csv")
        Dim bbh As IEnumerable(Of BBHIndex) = [in].LoadCsv(Of BBHIndex)
        Dim motifLogs As IEnumerable(Of MotifLog) = sites.LoadCsv(Of MotifLog)
        Dim Xmls As IEnumerable(Of String) = FileIO.FileSystem.GetFiles(GCModeller.FileSystem.GetRepositoryRoot & "/RegpreciseDownloads/", FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
        Dim MotifLDMs = (From log As BacteriaRegulome
                         In Xmls.Select(AddressOf LoadXml(Of BacteriaRegulome))
                         Where Not log.regulome Is Nothing AndAlso
                             Not log.regulome.regulators.IsNullOrEmpty
                         Select log)
        Dim result As New List(Of MotifLog)
        Dim bbhhash = BBHIndex.BuildHitsTable(bbh, True)
        Dim motifsHash = (From x As MotifLog
                          In motifLogs
                          Select x
                          Group x By x.Regulog Into Group) _
                               .ToDictionary(Function(x) x.Regulog,
                                             Function(x) x.Group.ToArray)
        Dim logsHash As New List(Of Regulator)

        For Each regulog As BacteriaRegulome In MotifLDMs
            For Each TF As Regulator In regulog.regulome.regulators
                If TF.type <> Types.TF Then
                    Continue For
                End If
                If Not bbhhash.ContainsKey(TF.regulator.name) Then
                    Continue For
                End If
                If Not motifsHash.ContainsKey(TF.regulog.name) Then
                    Continue For
                End If

#If DEBUG Then
                logsHash += TF
#End If

                Dim maps As String() = bbhhash(TF.regulator.name)
                Dim sitesFound As MotifLog() = motifsHash(TF.regulog.name)

                For Each site In sitesFound
                    site.tag = String.Join("; ", maps)
                Next

                result += sitesFound

                Call Console.Write(".")
            Next
        Next

#If DEBUG Then
        Dim test = (From x As Regulator
                    In logsHash
                    Select x
                    Group x By x.Regulog.name Into Group) _
                         .ToDictionary(Function(x) x.name,
                                       Function(x) x.Group.ToArray)
        result = New List(Of MotifLog)(result.OrderBy(Function(x) x.ID))
#End If
        Return result.SaveTo(out)
    End Function

    <ExportAPI("/Export.Regprecise.motifs")>
    <Usage("/Export.Regprecise.motifs /in <dir=genome_regprecise.xml> [/out <motifs.fasta>]")>
    <Description("Export Regprecise motif sites as a single fasta sequence file.")>
    Public Function ExportRegpreciseMotifSites(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.motifs.fasta"
        Dim fasta$

        Using writer As StreamWriter = out.OpenWriter(Encodings.ASCII)
            For Each genome As String In ls - l - "*.xml" <= [in]
                Dim data As BacteriaRegulome = genome.LoadXml(Of BacteriaRegulome)
                Dim regulators As Regulator() = data.regulome.regulators

                For Each regulator As Regulator In regulators
                    For Each site In regulator.regulatorySites.Where(Function(motif) Not motif.SequenceData.StringEmpty)
                        fasta = New FastaSeq With {
                            .SequenceData = Regtransbase.WebServices.Regulator.SequenceTrimming(site.SequenceData).Replace("-"c, "N"c),
                            .Headers = {
                                site.locus_tag,
                                site.position,
                                regulator.family,
                                site.bacteria
                            }
                        }.GenerateDocument(-1)

                        Call writer.WriteLine(fasta)
                    Next
                Next

                Call genome.BaseName.__INFO_ECHO
            Next
        End Using

        Return 0
    End Function

    ''' <summary>
    ''' 关联需要注释的蛋白质在Regprecise数据库之中的信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/regulators.bbh", Usage:="/regulators.bbh /bbh <bbh.index.Csv> /regprecise <repository.directory> [/sbh /description <KEGG_genomes.fasta> /allow.multiple /out <save.csv>]")>
    <Description("Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.")>
    <Group(CLIGroups.RegulonTools)>
    <ArgumentAttribute("/allow.multiple", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Allow the regulator assign multiple family name? By default is not allow, which means one protein just have one TF family name.")>
    Public Function RegulatorsBBh(args As CommandLine) As Integer
        Dim in$ = args <= "/bbh"
        Dim repo$ = args <= "/regprecise"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.regprecise.regulators.csv"
        Dim isSBH As Boolean = args("/sbh")
        Dim bbh As Dictionary(Of String, BBHIndex()) = [in] _
            .LoadCsv(Of BBHIndex) _
            .Where(Function(map)
                       ' 删除hits not found的情况
                       ' hits not found的时候，identities肯定是0
                       Return Not map.HitName.StringEmpty AndAlso BBHIndex.GetIdentities(map) > 0
                   End Function) _
            .GroupBy(Function(map)
                         If isSBH Then
                             Return map.HitName
                         Else
                             Return map.QueryName
                         End If
                     End Function) _
            .ToDictionary(Function(map) map.Key.Split(":"c).Last,
                          Function(g)
                              Return g.ToArray
                          End Function)
        Dim getDescription As Func(Of String, String)
        Dim allowMultiple As Boolean = args("/allow.multiple")

        With args <= "/description"
            If .FileExists Then
                Dim titles = FastaFile.Read(.ByRef) _
                    .Select(Function(seq) seq.Title.GetTagValue) _
                    .ToDictionary(Function(seq) seq.Name,
                                  Function(seq)
                                      Return seq.Value
                                  End Function)
                getDescription = AddressOf titles.TryGetValue
            Else
                getDescription = Function() ""
            End If
        End With

        Return (ls - l - r - "*.Xml" <= repo) _
            .Select(AddressOf LoadXml(Of BacteriaRegulome)) _
            .RunMatches(bbh, getDescription, distinct:=Not allowMultiple, isSBH:=isSBH) _
            .SaveTo(out) _
            .CLICode
    End Function

    <Extension>
    Public Function RunMatches(genomes As IEnumerable(Of BacteriaRegulome),
                               bbh As Dictionary(Of String, BBHIndex()),
                               getDescription As Func(Of String, String),
                               Optional distinct As Boolean = True,
                               Optional isSBH As Boolean = False) As RegPreciseRegulatorMatch()

        Dim map As RegPreciseRegulatorMatch
        Dim matches As New List(Of RegPreciseRegulatorMatch)
        Dim description$
        Dim getQuery, getRegprecise As Func(Of BBHIndex, String)

        If isSBH Then
            getQuery = Function(hit) hit.QueryName
            getRegprecise = Function(hit) hit.HitName
        Else
            getQuery = Function(hit) hit.HitName
            getRegprecise = Function(hit) hit.QueryName
        End If

        For Each genome As BacteriaRegulome In genomes
            Dim genomeName$ = genome.genome.name

            For Each regulator As Regulator In genome.regulome.regulators
                If Not regulator.type = Types.TF Then
                    Call $"Not working for non-TF type: {regulator.regulog.name}".Warning
                    Continue For
                End If
                If regulator.locus_tag.name.StringEmpty Then
                    Call $"Empty locus_tag: {regulator.regulog.name}?".Warning
                    Continue For
                End If
                If Not bbh.ContainsKey(regulator.locus_tag.name) Then
                    ' no maps
                    Call Console.Write(".")
                    Continue For
                End If

                Dim motifSites$() = regulator.regulatorySites _
                    .Select(Function(site)
                                Return $"{site.locus_tag}:{site.position}"
                            End Function) _
                    .ToArray

                For Each hit As BBHIndex In bbh(regulator.locus_tag.name)
                    If isSBH Then
                        description = getDescription(hit.HitName)
                    Else
                        description = getDescription(hit.QueryName)
                    End If

                    map = New RegPreciseRegulatorMatch With {
                        .biological_process = regulator.biological_process.JoinBy("; "),
                        .effector = regulator.effector,
                        .Family = regulator.family,
                        .Identities = BBHIndex.GetIdentities(hit),
                        .mode = regulator.regulationMode,
                        .Query = getQuery(hit),
                        .Regulator = getRegprecise(hit),
                        .Regulog = regulator.regulog.name,
                        .species = genomeName,
                        .RegulonSites = motifSites,
                        .Description = description
                    }

                    Call matches.Add(map)
                Next
            Next

            Call genomeName.__DEBUG_ECHO
        Next

        If distinct Then
            ' 分组之后取出最多的家族的结果
            Return matches _
                .GroupBy(Function(match) match.Query) _
                .Select(Function(matchGroup)
                            Dim familyGroup = matchGroup _
                                .Select(Function(m)
                                            Return m.Family.Split("/"c).Select(Function(family) (family, m))
                                        End Function) _
                                .IteratesALL _
                                .GroupBy(Function(g) g.Item1) _
                                .OrderByDescending(Function(g) g.Count) _
                                .First

                            Return familyGroup.Select(Function(g) g.Item2)
                        End Function) _
                .IteratesALL _
                .ToArray
        Else
            Return matches
        End If
    End Function
End Module
