#Region "Microsoft.VisualBasic::643c424fb766c5acf693b4ad636f863f, ..\GCModeller\analysis\RNA-Seq\TSSsTools\Transcriptome.UTRs\Views.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.COG

''' <summary>
''' TSSs位点的性质的统计函数
''' </summary>
<PackageNamespace("TSSs.Analysis.Views", Description:="The TSS property statics functions.", Publisher:="amethyst.asuka@gcmodeller.org")>
Public Module Views

    <ExportAPI("TSSs.Numbers")>
    Public Function TSSsNumberDistributes(data As Generic.IEnumerable(Of DocumentFormat.Transcript), Optional Max As Integer = 15) As File

        Dim LQuery = (From obj In data.AsParallel
                      Where Not String.IsNullOrEmpty(obj.Synonym)
                      Select obj
                      Group obj By obj.Synonym Into Group).ToArray
        Dim Numbers = New Integer(Max) {}

        For Each g In LQuery
            Dim n As Integer = g.Group.Count
            If n > Max Then
                n = Max
            End If
            n -= 1
            Numbers(n) = Numbers(n) + 1
        Next

        Dim Csv As New File
        Call Csv.Add({"Numbers Of TSSs", "Numbers Of Genes"})
        For i As Integer = 0 To Max - 2
            Call Csv.Add(({i + 1, Numbers(i)}).ToArray(Of String)(Function(n) CStr(n)))
        Next
        Call Csv.Add({">=" & Max, CStr(Numbers(Numbers.Length - 2))})

        Return Csv
    End Function

    <ExportAPI("5UTR.Length")>
    Public Function TSSs5UTRLenDistributes(data As Generic.IEnumerable(Of DocumentFormat.Transcript), Optional Max As Integer = 1200) As File
        Dim LQuery = (From site In data Select d = Math.Abs(site.ATG - site.TSSs) Group d By d Into Group).ToArray
        Dim CSV As New File

        Dim Numbers = New Integer(Max + 1) {}

        For Each Len5UTR In LQuery
            Dim n As Integer = Len5UTR.d
            If n > Max Then
                n = Max
            End If

            Numbers(n) = Len5UTR.Group.Count
        Next

        Call CSV.Add({"5'UTR Length", "Numbers Of Genes"})
        For i As Integer = 0 To Max - 2
            Call CSV.Add(({i + 1, Numbers(i)}).ToArray(Of String)(Function(n) CStr(n)))
        Next
        Call CSV.Add({">=" & Max, CStr(Numbers(Numbers.Length - 2))})

        Return CSV
    End Function

    <ExportAPI("TSSs.Numbers")>
    Public Function TSSsNumberDistributes(data As IEnumerable(Of DocumentFormat.Transcript),
                                          <Parameter("PTT", "The ptt file should contains the COG information.")>
                                          PTT As GenBank.TabularFormat.PTT,
                                          Optional Max As Integer = 15) As DocumentStream.File

        Dim LQuery = (From obj In data.AsParallel
                      Where Not String.IsNullOrEmpty(obj.Synonym)
                      Select obj
                      Group obj By obj.Synonym Into Group).ToArray
        Dim COGCategories = SMRUCC.genomics.Assembly.NCBI.COG.Function.Default
        Dim Numbers As Dictionary(Of SMRUCC.genomics.Assembly.NCBI.COG.COGCategories, Value(Of Integer))() =
            (From i As Integer
             In Max.Sequence
             Select New Dictionary(Of SMRUCC.genomics.Assembly.NCBI.COG.COGCategories, Value(Of Integer)) From {
                 {SMRUCC.genomics.Assembly.NCBI.COG.COGCategories.Genetics, New Value(Of Integer)(0)},
                 {SMRUCC.genomics.Assembly.NCBI.COG.COGCategories.Metabolism, New Value(Of Integer)(0)},
                 {SMRUCC.genomics.Assembly.NCBI.COG.COGCategories.NotAssigned, New Value(Of Integer)(0)},
                 {SMRUCC.genomics.Assembly.NCBI.COG.COGCategories.Signaling, New Value(Of Integer)(0)},
                 {SMRUCC.genomics.Assembly.NCBI.COG.COGCategories.Unclassified, New Value(Of Integer)(0)}}).ToArray

        For Each g In LQuery
            Dim n As Integer = g.Group.Count
            If n > Max Then
                n = Max
            End If
            n -= 1
            Dim Category = COGCategories.GetCategory(PTT.GeneObject(g.Synonym).COG)
            Dim Number = Numbers(n)(Category)
            Number.value += 1
        Next

        Dim Csv As New File
        Dim Categories = COGCategories.Catalogs.ToArray(Of COGCategories)(Function(obj) obj.Class).ToList
        Call Categories.Add(Assembly.NCBI.COG.COGCategories.NotAssigned)

        Call Csv.Add(({"Numbers Of TSSs"}).Join(Categories.ToArray(Of String)(Function(cat) cat.Description).Join({"", "Numbers Of Genes"})))
        For i As Integer = 0 To Max - 2
            Dim NumberValue = Numbers(i)
            Call Csv.Add((New String() {i + 1}).Join((From cat
                                                          In Categories
                                                      Select CStr(NumberValue(cat).value)).ToArray).Join({"", CStr(NumberValue.Values.ToArray(Of Integer)(Function(num) num.value).Sum)}))
        Next
        Call Csv.Add((New String() {">=" & Max}).Join((From cat
                                                           In Categories
                                                       Select CStr(Numbers(Numbers.Length - 2)(cat).value)).ToArray.Join({"", Numbers(Numbers.Length - 2).Values.ToArray(Of Integer)(Function(num) num.value).Sum})))

        Return Csv
    End Function

    <ExportAPI("5UTR.Length")>
    Public Function TSSs5UTRLenDistributes(data As Generic.IEnumerable(Of DocumentFormat.Transcript),
                                           <Parameter("PTT", "The ptt file should contains the COG information.")> PTT As PTT,
                                           Optional Max As Integer = 1200) As File

        Dim LQuery = (From site As DocumentFormat.Transcript
                          In data
                      Select d = Math.Abs(site.ATG - site.TSSs)
                      Group d By d Into Group).ToArray
        Dim CSV As New File
        Dim Numbers = New Integer(Max + 1) {}

        For Each Len5UTR In LQuery
            Dim n As Integer = Len5UTR.d
            If n > Max Then
                n = Max
            End If

            Numbers(n) = Len5UTR.Group.Count
        Next

        Call CSV.Add({"5'UTR Length", "Numbers Of Genes"})
        For i As Integer = 0 To Max - 2
            Call CSV.Add(({i + 1, Numbers(i)}).ToArray(Of String)(Function(n) CStr(n)))
        Next
        Call CSV.Add({">=" & Max, CStr(Numbers(Numbers.Length - 2))})

        Return CSV
    End Function

    <ExportAPI("TSSs.Export")>
    Public Function ExportTSSs(data As Generic.IEnumerable(Of DocumentFormat.Transcript),
                               NT As SMRUCC.genomics.SequenceModel.FASTA.FastaToken,
                               Optional offset As Integer = 3, <Parameter("Just.ORF", "Only the TSSs site of ORF will be export.")>
                               Optional ORF As Boolean = True) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
        Dim Reader = New SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader(NT)
        Dim Source = If(ORF, (From site In data.AsParallel Where Not String.IsNullOrEmpty(site.Synonym) Select site).ToArray, data.ToArray)
        Source = (From site In Source Select site Group site By site.TSSs Into Group).ToArray.ToArray(Function(obj) obj.Group.First)
        Dim LQuery = (From i In Source.Sequence.AsParallel
                      Let site = Source(i)
                      Where Not site.TSSs - offset < 0
                      Let ID As String() = {"Loci_" & i, site.Synonym, site.TSSs}
                      Let Sequence As String = Reader.TryParse(CLng(site.TSSs - offset), Right:=site.TSSs + offset + 1, Strand:=site.MappingLocation.Strand, WARN:=False)
                      Select New SMRUCC.genomics.SequenceModel.FASTA.FastaToken With {
                          .Attributes = ID,
                          .SequenceData = Sequence.ToUpper}).ToArray
        Return CType(LQuery, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
    End Function

    <ExportAPI("TSSs.NT.Frequency")>
    Public Function TSSsNTFrequency(data As IEnumerable(Of DocumentFormat.Transcript),
                                    NT As FASTA.FastaToken,
                                    Optional offset As Integer = 3,
                                    <Parameter("Just.ORF", "Only the TSSs site of ORF will be export.")>
                                    Optional ORF As Boolean = True) As DocumentStream.File
        Dim Fasta = ExportTSSs(data, NT, offset, ORF)
        Dim pStart = -offset
        Dim FrequencyData As Patterns.PatternModel = Patterns.Frequency(Fasta)
        Dim df As DocumentStream.File = New DocumentStream.File + {"prob", "A", "T", "G", "C"}
        df += LinqAPI.Exec(Of Patterns.SimpleSite, DocumentStream.RowObject)(FrequencyData.Residues) _
           <= Function(rsd As Patterns.SimpleSite) New DocumentStream.RowObject({CStr(pStart + rsd.Address)}.Join(ATGC.ToArray(Function(c) rsd.Probability(c).ToString)))

        Return df
    End Function

    ''' <summary>
    ''' 导出所有的
    ''' </summary>
    ''' <param name="sites"></param>
    ''' <param name="NT"></param>
    ''' <param name="ORF"></param>
    ''' <param name="Length"></param>
    ''' <returns></returns>
    <ExportAPI("Upstream.Promoter")>
    Public Function UpstreamPromoter(sites As IEnumerable(Of DocumentFormat.Transcript),
                                     NT As FASTA.FastaToken,
                                     <Parameter("Just.ORF", "Only the TSSs site of ORF will be export.")>
                                     Optional ORF As Boolean = True,
                                     <Parameter("Length.UpStream")>
                                     Optional Length As Integer = 50) As FASTA.FastaFile

        sites = If(ORF, (From site In sites.AsParallel Where Not String.IsNullOrEmpty(site.Synonym) Select site).ToArray, sites)

        Dim sitesGroup = (From site In sites Select site Group site By site.TSSs Into Group).ToArray
        Dim Reader = New NucleotideModels.SegmentReader(NT)
        Dim LQuery = (From site In sitesGroup
                      Let site_loci = site.Group.First
                      Let loci = If(site_loci.MappingLocation.Strand = Strands.Forward,
                          New NucleotideLocation(site_loci.TSSs - Length, site_loci.TSSs, Strands.Forward),
                          New NucleotideLocation(site_loci.TSSs, site_loci.TSSs + Length, Strands.Reverse))
                      Where loci.Normalization.Left > 0
                      Let Sequence = Reader.TryParse(loci)
                      Select New FASTA.FastaToken With {
                          .Attributes = New String() {"lcl_" & site.TSSs, String.Join(",", site.Group.ToArray(Of String)(Function(obj) obj.Synonym))},
                          .SequenceData = Sequence.SequenceData}).ToArray
        Return New FASTA.FastaFile(LQuery)
    End Function

    Const ATGC As String = "ATGC"

    ''' <summary>
    ''' 按照表达水平的变化模式来导出启动子序列
    ''' </summary>
    ''' <param name="sites"></param>
    ''' <param name="DESeqCOGs"></param>
    ''' <param name="NT"></param>
    ''' <param name="Length"></param>
    ''' <returns></returns>
    <ExportAPI("Upstream.Promoter")>
    Public Function UpStreamPromoter(<Parameter("Sites", "Please notice that there is only ORF gene its promoter sequence will be export.")>
                                     sites As Generic.IEnumerable(Of DocumentFormat.Transcript),
                                     <Parameter("DESeq.COGs")> DESeqCOGs As Generic.IEnumerable(Of DESeq2.DESeqCOGs),
                                     NT As SMRUCC.genomics.SequenceModel.FASTA.FastaToken,
                                     <Parameter("Length.UpStream")> Optional Length As Integer = 50,
                                     <Parameter("Dir.Export", "If this directory location is not specified, then the current directory will be used.")>
                                     Optional Export As String = "") As Boolean
        If String.IsNullOrEmpty(Export) Then
            Export = FileIO.FileSystem.CurrentDirectory
        End If

        sites = (From site In sites Where Not String.IsNullOrEmpty(site.Synonym) Select site).ToArray
        Dim SitesGroup = (From site In sites Select site Group site By site.Synonym Into Group).ToArray.ToDictionary(Function(obj) obj.Synonym, elementSelector:=Function(obj) obj.Group.ToArray)
        Dim Reader = New SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader(NT)

        Call __Export(SitesGroup, Reader, Length:=Length, ID:=(From COG In DESeqCOGs Select COG.DiffDown).ToArray.Unlist.Distinct.ToArray) _
                .Save($"{Export}/TSSs+Promoters{Length}/{NameOf(DESeq2.DESeqCOGs.DiffDown)}.fasta")
        Call __Export(SitesGroup, Reader, Length:=Length, ID:=(From COG In DESeqCOGs Select COG.DiffUp).ToArray.Unlist.Distinct.ToArray) _
                .Save($"{Export}/TSSs+Promoters{Length}/{NameOf(DESeq2.DESeqCOGs.DiffUp)}.fasta")
        Call __Export(SitesGroup, Reader, Length:=Length, ID:=(From COG In DESeqCOGs Select COG.IdenticalHigh).ToArray.Unlist.Distinct.ToArray) _
                .Save($"{Export}/TSSs+Promoters{Length}/{NameOf(DESeq2.DESeqCOGs.IdenticalHigh)}.fasta")
        Call __Export(SitesGroup, Reader, Length:=Length, ID:=(From COG In DESeqCOGs Select COG.IdenticalLow).ToArray.Unlist.Distinct.ToArray) _
                .Save($"{Export}/TSSs+Promoters{Length}/{NameOf(DESeq2.DESeqCOGs.IdenticalLow)}.fasta")

        Return True
    End Function

    Private Function __Export(TSSs As Dictionary(Of String, DocumentFormat.Transcript()),
                              Reader As SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader,
                              ID As String(),
                              Length As Integer) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
        Dim LQuery = (From site In ID.AsParallel
                      Where TSSs.ContainsKey(site)
                      Let sites = TSSs(site)
                      Select (From site_loci As DocumentFormat.Transcript
                                  In sites
                              Let sequence As SMRUCC.genomics.SequenceModel.FASTA.FastaToken = __Export(site_loci, Length, Reader)
                              Where Not sequence Is Nothing
                              Select sequence).ToArray).ToArray.Unlist
        Return CType(LQuery, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
    End Function

    Private Function __Export(site_loci As DocumentFormat.Transcript, Length As Integer, reader As SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader) As SMRUCC.genomics.SequenceModel.FASTA.FastaToken
        Dim loci = If(site_loci.MappingLocation.Strand = Strands.Forward,
                          New SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation(site_loci.TSSs - Length, site_loci.TSSs, SMRUCC.genomics.ComponentModel.Loci.Strands.Forward),
                          New SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation(site_loci.TSSs, site_loci.TSSs + Length, SMRUCC.genomics.ComponentModel.Loci.Strands.Reverse))
        If Not loci.Normalization.Left > 0 Then
            Return Nothing
        End If
        Dim Sequence = reader.TryParse(loci)
        Return New SMRUCC.genomics.SequenceModel.FASTA.FastaToken With {
            .Attributes = New String() {$"lcl_{site_loci.TSSs}_{site_loci.Synonym}", site_loci.Synonym},
            .SequenceData = Sequence.SequenceData
        }
    End Function
End Module
