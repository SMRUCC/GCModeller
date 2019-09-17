#Region "Microsoft.VisualBasic::bd7c69ee510ab5153457c27b08b17634, CLI_tools\RegPrecise\CLI\TF.vb"

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
    '     Function: Effectors, FamilyHits, ProteinMotifsEXPORT, ProtMotifToPfamString, ToPfamString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Assembly.MetaCyc.File
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.ProteinModel
Imports SMRUCC.genomics.SequenceModel

Partial Module CLI

    <ExportAPI("/Prot_Motifs.EXPORT.pfamString",
               Usage:="/Prot_Motifs.EXPORT.pfamString /in <motifs.json> /PTT <genome.ptt> [/out <pfam-string.csv>]")>
    Public Function ProteinMotifsEXPORT(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim PTT As PTT = TabularFormat.PTT.Load(args - "/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".PfamString.Csv")
        Dim motifs As Protein() = [in].ReadAllText.LoadJSON(Of Protein())
        Dim lenHash As Dictionary(Of String, Integer) = PTT.GeneObjects.ToDictionary(Function(x) x.Synonym, Function(x) x.Length)
        Dim PfamString As PfamString() = motifs.ToPfamString(lenHash, 0.00001).ToArray
        Return PfamString.SaveTo(out)
    End Function

    <ExportAPI("/Prot_Motifs.PfamString",
               Usage:="/Prot_Motifs.PfamString /in <RegPrecise.Download_DIR> [/fasta <RegPrecise.fasta> /out <pfam-string.csv>]")>
    Public Function ProtMotifToPfamString(args As CommandLine) As Integer
        Dim files As String() = LinqAPI.Exec(Of String) <= From dr As String
                                                           In ls - l - lsDIR <= args("/in")
                                                           Select ls - l - wildcards("*.json") <= dr
        Dim RegPrecise = (From fa As FASTA.FastaSeq
                          In New FASTA.StreamIterator(args.GetValue("/fasta", $"{args - "/in"}/RegPrecise.fasta")).ReadStream
                          Select fa
                          Group fa By fa.Headers.First.Split.First.Split(":"c).Last Into Group) _
                               .ToDictionary(Function(x) x.Last,
                                             Function(x) x.Group.First.Length)
        Dim PfamString As PfamString() =
            LinqAPI.Exec(Of PfamString) <= From json As String In files
                                           Let doc As String = json.ReadAllText
                                           Select doc.LoadJSON(Of Protein()).ToPfamString(RegPrecise, 0.00001)
        Dim out As String = args.GetValue("/out", (args - "/in").TrimEnd("/"c).TrimEnd("\"c) & ".PfamString.Csv")
        Return PfamString.SaveTo(out)
    End Function

    <Extension>
    Public Iterator Function ToPfamString(source As IEnumerable(Of Protein), hash As Dictionary(Of String, Integer), cut As Double) As IEnumerable(Of PfamString)
        Dim l As Integer
        Dim motifs As ProteinModel.DomainObject()

        For Each x As Protein In source
            l = If(hash.ContainsKey(x.ID), hash(x.ID), x.Length)
            motifs = (From m As ProteinModel.DomainObject In x.Domains Where Val(m.EValue) <= cut Select m).ToArray

            If motifs.Length <= 1 Then
                motifs =
                    LinqAPI.Exec(Of ProteinModel.DomainObject) <=
                   (LinqAPI.Takes(Of ProteinModel.DomainObject)(4) <=
                        From m As ProteinModel.DomainObject
                        In x.Domains
                        Select m
                        Order By m.EValue Ascending)
            End If

            Yield New PfamString With {
                .ProteinId = x.ID,
                .Length = l,
                .Description = x.Description,
                .PfamString = motifs.Select(Function(d) $"{d.Name.Split(":"c).Last}({d.Position.Left}|{d.Position.Right})"),
                .Domains =
                    LinqAPI.Exec(Of String) <= From d As ProteinModel.DomainObject
                                               In motifs
                                               Let id As String = d.Name.Split(":"c).Last
                                               Select $"{id}:{id}"
                                               Distinct
            }
        Next
    End Function

    ''' <summary>
    ''' 计数调控因子所比对上的家族，这个函数输出两个文件，一个是原始的文件，一个是只取最多的hit的家族的文件
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Family.Hits",
               Usage:="/Family.Hits /bbh <bbh.csv> [/regprecise <RegPrecise.Xml> /pfamKey <query.pfam-string> /out <out.DIR>]")>
    Public Function FamilyHits(args As CommandLine) As Integer
        Dim inBBH As String = args("/bbh")
        Dim inDIR As String = args.GetValue("/regprecise", GCModeller.FileSystem.RegPrecise.Directories.RegPreciseRegulations)
        Dim out As String = args.GetValue("/out", inBBH.TrimSuffix & ".FamilyHits/")
        Dim bbh As IEnumerable(Of BBHIndex) = inBBH.LoadCsv(Of BBHIndex)
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
        Dim genomes As IEnumerable(Of BacteriaRegulome) = inDIR.LoadXml(Of TranscriptionFactors).genomes
        Dim all = (From x As BacteriaRegulome In genomes
                   Where Not x.regulome Is Nothing AndAlso
                       Not x.regulome.regulators.IsNullOrEmpty
                   Select xx = x.regulome.regulators).Unlist
        Dim regulators = (From regulator As Regulator In all
                          Let sid As String = regulator.LocusId
                          Where hitsHash.ContainsKey(sid)
                          Select sid,
                              hits = hitsHash(sid),
                              regulator.family).ToArray
        Dim queryRegulators = (From qx In
                                   (From x In regulators
                                    Select (From hit As BBHIndex In x.hits
                                            Select query = hit, x.sid, x.Family).ToArray).Unlist
                               Select qx
                               Group qx By qx.query.QueryName Into Group).ToArray
        bbh = (From x In queryRegulators Select x.Group.Select(Function(xx) xx.query)).Unlist
        Call bbh.SaveTo(out & "/Regulators.bbh.csv") ' 将Regulators的bbh结果分离出来了

        Dim FamilyBriefs As IEnumerable(Of FamilyHit) =
            LinqAPI.Exec(Of FamilyHit) <= From x In queryRegulators
                                          Select From hit In x.Group
                                                 Select New FamilyHit With {
                                                     .QueryName = x.QueryName,
                                                     .Family = hit.Family,
                                                     .HitName = hit.query.HitName
                                                 }
        Call FamilyBriefs.SaveTo(out & "/Regulators.FamilyHits.Csv")

        hitsHash = (From x As BBHIndex
                    In bbh
                    Where x.isMatched
                    Select uid = x.QueryName,
                        x
                    Group By uid Into Group) _
                            .ToDictionary(Function(x) x.uid,
                                          Function(x)
                                              Return x.Group.Select(Function(o) o.x)
                                          End Function)

        Dim key As String = args.GetValue("/pfamKey", "query.pfam-string")
        Dim LQuery = (From x In queryRegulators
                      Let names As IEnumerable(Of String) =
                          (From hit In x.Group Select name = hit.Family.Split("/"c)).IteratesALL
                      Select x.QueryName,
                          pfam = hitsHash(x.QueryName).First.Property(key),
                          Family = (From s As String
                                    In names
                                    Select s
                                    Group s By s.ToUpper Into Group) _
                                         .OrderByDescending(Function(f) f.Group.Count).First.Group.First).ToArray
        Return LQuery.SaveTo(out & "/Top-Family.Csv")
    End Function

    <ExportAPI("/Maps.Effector",
               Usage:="/Maps.Effector /imports <RegPrecise.DIR> [/out <out.csv>]")>
    Public Function Effectors(args As CommandLine) As Integer
        Dim xmls As IEnumerable(Of String) = ls - l - r - wildcards("*.xml") <= args - "/imports"
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/RegPrecise.Effector.Maps.Csv")
        Dim list As New List(Of Effectors)

        For Each genome As BacteriaRegulome In From xml As String In xmls Select xml.LoadXml(Of BacteriaRegulome)
            list += From x As Regulator
                    In genome.regulome.regulators
                    Where x.type = Types.TF
                    Where Not x.effector.StringEmpty
                    Let tokens As String() = x.effector.Split(";"c)
                    Select From name As String
                           In tokens
                           Select New Effectors With {
                               .Effector = name.Trim,
                               .BiologicalProcess = x.biological_process.JoinBy("; "),
                               .Pathway = x.pathway,
                               .Regulon = x.regulog.name,
                               .TF = x.LocusId
                           }
        Next

        Return list.SaveTo(out).CLICode
    End Function

    '<ExportAPI("/Effector.FillNames",
    '           Usage:="/Effector.FillNames /in <effectors.csv> /compounds <metacyc.compounds> [/out <out.csv>]")>
    'Public Function EffectorFillNames(args As CommandLine) As Integer
    '    Dim [in] As String = args - "/in"
    '    Dim compounds As String = args - "/compounds"
    '    Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & compounds.BaseName & ".Csv")
    '    Dim effectors As IEnumerable(Of Effectors) = [in].LoadCsv(Of Effectors)
    '    Dim maps As New CompoundsMapping(DataFiles.Compounds.LoadCompoundsData(compounds))
    '    Dim LQuery = (From x As Effectors
    '                  In effectors.AsParallel
    '                  Select x.Fill(maps.NameQuery(x.Effector))).ToArray
    '    Return LQuery.SaveTo(out).CLICode
    'End Function
End Module
