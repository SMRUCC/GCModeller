#Region "Microsoft.VisualBasic::753cebaf81d8d53d1dc9e26e08eedc27, CLI_tools\MEME\Cli\RegulatorMotifs.vb"

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
    '     Function: __hitTest, HitTest, RegulatorMotifs, TestRegulatorMotifs
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA

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
    Public Function TestRegulatorMotifs(args As CommandLine) As Integer
        Dim inFamilyHits = args("/hits")
        Dim inMotifs As String = args("/motifs")
        Dim out As String = args.GetValue("/out", inMotifs.TrimSuffix & ".Test.Csv")
        Dim FamilyHits = inFamilyHits.LoadCsv(Of FamilyHit)
        Dim Motifs As IEnumerable(Of FamilyHit) = inMotifs.LoadCsv(Of MotifHit)
        Motifs = HitTest(Motifs, FamilyHits)
        Return Motifs.ToArray.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 导出由bbh结果所得到的motif信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Regulator.Motifs", Usage:="/Regulator.Motifs /bbh <bbh.csv> /regprecise <genome.DIR> [/out <outDIR>]")>
    Public Function RegulatorMotifs(args As CommandLine) As Integer
        Dim inBBH As String = args("/bbh")
        Dim inDIR As String = args("/regprecise")
        Dim out As String = args.GetValue("/out", inBBH.TrimSuffix & ".Motifs.fa")
        Dim bbh = inBBH.LoadCsv(Of BBHIndex)
        Dim hitsHash = (From x As BBHIndex
                        In bbh
                        Where x.isMatched
                        Select uid = x.HitName.Split(":"c).Last,
                            x
                        Group By uid Into Group) _
                            .ToDictionary(Function(x) x.uid,
                                          Function(x)
                                              Return x.Group.Select(Function(xx) xx.x)
                                          End Function)
        Dim genomes As BacteriaRegulome() = (ls - l - wildcards("*.Xml") <= inDIR) _
            .Select(AddressOf SafeLoadXml(Of BacteriaRegulome))
        Dim all As String() = genomes.Select(Function(x) x.ListRegulators).Unlist.Distinct.ToArray
        Dim regulators = (From sid As String In all Where hitsHash.ContainsKey(sid) Select sid, hits = hitsHash(sid)).ToArray
        Dim queryRegulators = (From qx In
                                   (From x In regulators
                                    Select (From hit As BBHIndex
                                            In x.hits
                                            Select query = hit,
                                                x.sid)).IteratesALL
                               Select qx
                               Group qx By qx.query.QueryName Into Group).ToArray
        bbh = (From x In queryRegulators Select x.Group.Select(Function(xx) xx.query)).Unlist
        ' 将Regulators的bbh结果分离出来了
        Call bbh.SaveTo(out & "/Regulators.bbh.csv")

        Dim regs = (From reg As Regulator
                    In genomes.Select(Function(x) x.regulome.regulators).IteratesALL
                    Select reg
                    Group reg By reg.LocusId Into Group) _
                         .ToDictionary(Function(x) x.LocusId,
                                       Function(x) x.Group.ToArray)
        Dim motifs = (From query In queryRegulators
                      Let qName As String = query.QueryName
                      Let sites = (From reg In query.Group Let hit = reg.sid Let ss = regs(hit) Select ss).IteratesALL.Select(Function(x) x.regulatorySites).Unlist
                      Select qName, sites).ToArray
        For Each query In motifs
            Dim path As String = $"{out}/{query.qName}.fasta"
            Dim duplicates = (From x In query.sites Select x Group x By x.UniqueId Into Group).ToArray
            Dim fa As New FastaFile(duplicates.Select(Function(x) x.Group.First))
            Dim setSeq = New SetValue(Of FastaSeq) <= NameOf(FastaSeq.SequenceData)
            fa = New FastaFile(fa.Select(Function(x) setSeq(x, Regtransbase.WebServices.Regulator.SequenceTrimming(x.SequenceData))))
            Call fa.Save(-1, path, Encodings.ASCII)
        Next

        Return 0
    End Function
End Module
