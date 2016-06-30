Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.Similarity.TOMQuery
Imports LANS.SystemsBiology.DatabaseServices
Imports LANS.SystemsBiology.DatabaseServices.Regprecise
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq

Partial Module CLI

    Public Function HitTest(source As IEnumerable(Of MotifHit), family As IEnumerable(Of FamilyHit)) As MotifHit()
        Dim FamilyHash = (From x As FamilyHit
                          In family
                          Select x
                          Group By x.QueryName Into Group) _
                                    .ToDictionary(Function(x) x.QueryName,
                                                  Function(x) (From xx As FamilyHit
                                                               In x.Group
                                                               Select s = xx.Family
                                                               Distinct).ToArray)
        Dim LQuery = (From x As MotifHit In source
                      Where __hitTest(x, FamilyHash)
                      Select x).ToArray
        Return LQuery
    End Function

    Private Function __hitTest(motifHit As MotifHit, hash As Dictionary(Of String, String())) As Boolean
        Dim qName As String = motifHit.Query.Split("."c).First

        If Not hash.ContainsKey(qName) Then
            Return False
        End If

        Dim hits As String() = hash(qName)
        Dim motif As String = motifHit.Subject.Split("."c).First
        Return Array.IndexOf(hits, motif) > -1
    End Function

    <ExportAPI("/Regulator.Motifs.Test", Usage:="/Regulator.Motifs.Test /hits <familyHits.Csv> /motifs <motifHits.Csv> [/out <out.csv>]")>
    Public Function TestRegulatorMotifs(args As CommandLine.CommandLine) As Integer
        Dim inFamilyHits = args("/hits")
        Dim inMotifs As String = args("/motifs")
        Dim out As String = args.GetValue("/out", inMotifs.TrimFileExt & ".Test.Csv")
        Dim FamilyHits = inFamilyHits.LoadCsv(Of FamilyHit)
        Dim Motifs = inMotifs.LoadCsv(Of MotifHit)
        Motifs = HitTest(Motifs, FamilyHits).ToList
        Return Motifs.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 导出由bbh结果所得到的motif信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Regulator.Motifs", Usage:="/Regulator.Motifs /bbh <bbh.csv> /regprecise <genome.DIR> [/out <outDIR>]")>
    Public Function RegulatorMotifs(args As CommandLine.CommandLine) As Integer
        Dim inBBH As String = args("/bbh")
        Dim inDIR As String = args("/regprecise")
        Dim out As String = args.GetValue("/out", inBBH.TrimFileExt & ".Motifs.fa")
        Dim bbh = inBBH.LoadCsv(Of BBH.BBHIndex)
        Dim hitsHash = (From x As BBH.BBHIndex
                        In bbh
                        Where x.Matched
                        Select uid = x.HitName.Split(":"c).Last,
                            x
                        Group By uid Into Group) _
                            .ToDictionary(Function(x) x.uid,
                                          Function(x) x.Group.ToArray(Function(xx) xx.x))
        Dim genomes As BacteriaGenome() = (ls - l - wildcards("*.Xml") <= inDIR) _
            .ToArray(AddressOf SafeLoadXml(Of BacteriaGenome))
        Dim all As String() = genomes.ToArray(Function(x) x.ListRegulators).MatrixToList.Distinct.ToArray
        Dim regulators = (From sid As String In all Where hitsHash.ContainsKey(sid) Select sid, hits = hitsHash(sid)).ToArray
        Dim queryRegulators = (From qx In
                                   (From x In regulators
                                    Select (From hit As BBH.BBHIndex
                                            In x.hits
                                            Select query = hit,
                                                x.sid)).MatrixAsIterator
                               Select qx
                               Group qx By qx.query.QueryName Into Group).ToArray
        bbh = (From x In queryRegulators Select x.Group.ToArray(Function(xx) xx.query)).MatrixToList
        ' 将Regulators的bbh结果分离出来了
        Call bbh.SaveTo(out & "/Regulators.bbh.csv")

        Dim regs = (From reg As Regulator
                    In genomes.ToArray(Function(x) x.Regulons.Regulators).MatrixAsIterator
                    Select reg
                    Group reg By reg.LocusId Into Group) _
                         .ToDictionary(Function(x) x.LocusId,
                                       Function(x) x.Group.ToArray)
        Dim motifs = (From query In queryRegulators
                      Let qName As String = query.QueryName
                      Let sites = (From reg In query.Group Let hit = reg.sid Let ss = regs(hit) Select ss).MatrixAsIterator.ToArray(Function(x) x.RegulatorySites).MatrixToList
                      Select qName, sites).ToArray
        For Each query In motifs
            Dim path As String = $"{out}/{query.qName}.fasta"
            Dim duplicates = (From x In query.sites Select x Group x By x.UniqueId Into Group).ToArray
            Dim fa As New FastaFile(duplicates.ToArray(Function(x) x.Group.First))
            Dim setSeq = New SetValue(Of FastaToken) <= NameOf(FastaToken.SequenceData)
            fa = New FastaFile(fa.ToArray(Function(x) setSeq(x, Regtransbase.WebServices.Regulator.SequenceTrimming(x.SequenceData))))
            Call fa.Save(-1, path, Encodings.ASCII)
        Next

        Return 0
    End Function
End Module