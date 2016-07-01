#Region "Microsoft.VisualBasic::56c8727c4a0544a5401edb32a9bff4e0, ..\GCModeller\CLI_tools\RNA-seq\CLI\CLI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.FeatureKeys
Imports SMRUCC.genomics.Toolkits.RNA_Seq
Imports SMRUCC.genomics.Toolkits.RNA_Seq.BOW.DocumentFormat.SAM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Toolkits.RNA_Seq.RTools
Imports SMRUCC.genomics.Assembly.DOOR
Imports LANS.SystemsBiology
Imports SMRUCC.genomics.Toolkits.RNA_Seq.Operon
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Toolkits.RNA_Seq.RTools.DESeq2
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language

<PackageNamespace("RNA-seq.CLI", Category:=APICategories.CLI_MAN, Publisher:="xie.guigang@gcmodeller.org")>
Module CLI

    Sub New()
        Call Settings.Session.Initialize()

        Try
            Dim out As String = Settings.Session.Templates & "/DESeq2.SampleTable.Csv"

            If Not out.FileExists Then
                Dim empty As DESeq2.SampleTable() = New DESeq2.SampleTable() {
                    New DESeq2.SampleTable With {.condition = "MMX", .fileName = "M1.txt", .sampleName = "M1"},
                    New DESeq2.SampleTable With {.condition = "MMX", .fileName = "M2.txt", .sampleName = "M2"},
                    New DESeq2.SampleTable With {.condition = "NY", .fileName = "N1.txt", .sampleName = "N1"},
                    New DESeq2.SampleTable With {.condition = "NY", .fileName = "N2.txt", .sampleName = "N2"}
                }
                Call empty.SaveTo(out)
            End If

            out = Settings.Session.Templates & "/DESeq2.Result.Csv"
            If Not out.FileExists Then
                Dim empty As ResultData() = {
                    New ResultData With {
                        .locus_tag = "locus1",
                        .dataExpr0 = New Dictionary(Of String, Double) From {
                            {"Sample1", 152},
                            {"Sample2", 10}
                        }
                    }
                }
                Call empty.SaveTo(out)
            End If
        Catch ex As Exception

        End Try
    End Sub

    <ExportAPI("/DOOR.Corrects", Usage:="/DOOR.Corrects /DOOR <genome.opr> /pcc <pcc.dat> [/out <out.opr> /pcc-cut <0.45>]")>
    Public Function DOORCorrects(args As CommandLine.CommandLine) As Integer
        Dim doorFile As String = args("/door")
        Dim pccDat As String = args("/pcc")
        Dim pccCut As Double = args.GetValue("/pcc-cut", 0.45)
        Dim out As String = args.GetValue("/out", doorFile.TrimFileExt & $".PCC={pccCut}.opr")
        Dim PCC As PccMatrix = MatrixSerialization.Load(from:=pccDat)
        Dim DOOR = DOOR_API.Load(doorFile)
        Dim corrects As Operon() = CorrectDoorOperon(PCC, DOOR, pccCut)

        Dim MAT As New DocumentStream.File
        Call MAT.Add({"DOOR", "locus_id", "Strand"})

        For Each operon As Operon In corrects
            Dim row As New DocumentStream.RowObject
            Dim initX = operon.InitialX
            Call row.AddRange({operon.OperonID, initX.Synonym, initX.Location.Strand.GetBriefCode})
            Call MAT.Add(row)

            If operon.NumOfGenes = 1 Then
                Continue For
            End If

            Dim pre As New List(Of String)

            For Each gene As GeneBrief In (From x In operon Where Not x.Value Is initX Select x.Value)
                row = New RowObject
                Call row.AddRange({operon.OperonID, gene.Synonym, gene.Location.Strand.GetBriefCode})
                Call row.Add(PCC.GetValue(initX.Synonym, gene.Synonym))

                For Each sId As String In pre
                    Call row.Add(PCC.GetValue(sId, gene.Synonym))
                Next

                Call pre.Add(gene.Synonym)
                Call MAT.Add(row)
            Next
        Next
        Call MAT.Save(out & ".Views.Csv", Encodings.ASCII)
        Return DOOR_API.SaveFile(corrects, out).CLICode
    End Function

    <ExportAPI("/PCC", Usage:="/PCC /expr <expr.matrix.csv> [/out <out.dat>]")>
    Public Function PCC(args As CommandLine.CommandLine) As Integer
        Dim expr As String = args("/expr")
        Dim out As String = args.GetValue("/out", expr.TrimFileExt & ".PCC.dat")
        Dim MAT As PccMatrix = MatrixAPI.CreatePccMAT(DocumentStream.File.Load(expr))
        Return MatrixSerialization.SaveBin(MAT, out).CLICode
    End Function

    <ExportAPI("/SPCC", Usage:="/SPCC /expr <expr.matrix.csv> [/out <out.dat>]")>
    Public Function SPCC(args As CommandLine.CommandLine) As Integer
        Dim expr As String = args("/expr")
        Dim out As String = args.GetValue("/out", expr.TrimFileExt & ".SPCC.dat")
        Dim MAT As PccMatrix = MatrixAPI.CreateSPccMAT(DocumentStream.File.Load(expr))
        Return MatrixSerialization.SaveBin(MAT, out).CLICode
    End Function

    <ExportAPI("/HT-seq", Info:="Count raw reads for DESeq2 analysis.",
               Usage:="/Ht-seq /in <in.sam> /gff <genome.gff> [/out <out.txt> /mode <union, intersection_strict, intersection_nonempty; default:intersection_nonempty> /rpkm /feature <CDS>]")>
    <ParameterInfo("/Mode", True,
                   Description:="The value of this parameter specific the counter of the function will be used, the available counter values are: union, intersection_strict and intersection_nonempty")>
    <ParameterInfo("/feature", True,
                   Description:="[NOTE: value is case sensitive!!!] Value of the gff features can be one of the: tRNA, CDS, exon, gene, tmRNA, rRNA, region")>
    Public Function HTSeqCount(args As CommandLine.CommandLine) As Integer
        Dim inSAM As String = args("/in")
        Dim gffFile As String = args("/gff")
        Dim out As String = args.GetValue("/out", inSAM.TrimFileExt & ".Ht-seq.txt")
        Dim Mode As String = args.GetValue("/mode", "intersection_nonempty")
        Dim RPKM As Boolean = args.GetBoolean("/RPKM")
        Dim sfeature As String = args.GetValue("/feature", "CDS")
        Dim feature As Features = FeaturesHash.TryGetValue(sfeature, Features.CDS)
        Dim outDoc As String = HtseqCountMethod.HtseqCount(inSAM, gffFile, Mode, RPKM, feature)
        Return outDoc.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 如果是CDS feature的话，由于并没有直接的locus_tag，分析起来会不太直观，所以可以使用这个方法将id映射回locus_tag
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/sid.map", Usage:="/sid.map /gff <genome.gff> /raw <htseq-count.txt> [/out <out.txt>]")>
    Public Function sIdMapping(args As CommandLine.CommandLine) As Integer
        Dim gffFile As String = args("/gff")
        Dim raw As String = args("/raw")
        Dim out As String = args.GetValue("/out", raw.TrimFileExt & ".locus_tag.txt")
        Dim counts = CountResult.Load(raw)
        Dim gff As GFF = GFF.LoadDocument(gffFile)
        Dim mappings = gff.ProtId2Locus
        Dim setValue = New SetValue(Of CountResult) <= NameOf(CountResult.Feature)

        counts =
            LinqAPI.Exec(Of CountResult) <= From x As CountResult
                                            In counts
                                            Let id As String =
                                                If(mappings.ContainsKey(x.Feature),
                                                mappings(x.Feature),
                                                x.Feature)
                                            Select setValue(x, id)
        Dim doc As String = counts.ToDoc
        Return doc.SaveTo(out).CLICode
    End Function

    <ExportAPI("/RPKM", Usage:="/RPKM /raw <raw_count.txt> /gff <genome.gff> [/out <expr.out.csv>]")>
    Public Function RPKM(args As CommandLine.CommandLine) As Integer
        Dim inRaw As String = args("/raw")
        Dim gffFile As String = args("/gff")
        Dim out As String = args.GetValue("/out", inRaw.TrimFileExt & ".RPKM.csv")
        Dim dataExpr0 = CountResult.Load(inRaw)
        Dim gff As GFF = GFF.LoadDocument(gffFile)
        dataExpr0 = dataExpr0.RPKM(gff)
        Return dataExpr0.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Data.Frame",
               Info:="Generates the data input for the DESeq2 R package.",
               Usage:="/Data.Frame /in <in.DIR> /ptt <genome.ptt> [/out out.csv]")>
    <ParameterInfo("/in", False,
                   Description:="A directory location which it contains the Ht-Seq raw count text files.")>
    Public Function Df(args As CommandLine.CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & "/DESeq2_dataExpr0.csv")
        Dim ptt As String = args("/ptt")
        Dim raw = FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.txt")
        Dim result = HtseqCountMethod.DataFrame(raw, NCBI.GenBank.TabularFormat.PTT.Load(ptt),)
        Return result.Save(out, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/DESeq2", Usage:="/DESeq2 /sample.table <sampleTable.csv> /raw <raw.txt.DIR> /ptt <genome.ptt> [/design <design, default: ~condition>]")>
    Public Function DESeq2(args As CommandLine.CommandLine) As Integer
        Dim rawDIR As String = args("/raw")
        Dim PTT As String = args("/ptt")
        Dim design As String = args.GetValue("/design", "~condition")
        Dim dtPTT As PTT = NCBI.GenBank.TabularFormat.PTT.Load(PTT)
        Dim sampleTable As DocumentStream.File = DocumentStream.File.Load(args("/sample.table"))
        Return RTools.DESeq2.DESeq2(sampleTable, rawDIR, design, dtPTT).CLICode
    End Function

    <ExportAPI("/DEGs", Usage:="/DEGs /in <diff.csv> [/out <degs.csv> /log_fold 2]")>
    Public Function DEGs(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim logFold As Double = args.GetValue("/log_fold", 2.0R)
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & $".logFold={logFold}.csv")
        Dim diff = inFile.LoadCsv(Of RTools.DESeq2.ResultData)
        diff = (From x In diff Where Math.Abs(x.log2FoldChange) >= logFold Select x).ToList
        Return diff.SaveTo(out).CLICode
    End Function

    <ExportAPI("/DEGs.UpDown", Usage:="/DEGs.UpDown /in <diff.csv> /sample.table <sampleTable.Csv> [/out <outDIR>]")>
    Public Function DEGsUpDown(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim samples As String = args("/sample.table")
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & ".DEGs.UpDowns/")
        Dim inDiff = inFile.LoadCsv(Of DESeq2.ResultData)
        Dim sampleTable = samples.LoadCsv(Of DESeq2.SampleTable)
        Dim conditions = (From x As DESeq2.SampleTable
                          In sampleTable
                          Select x
                          Group x By x.condition Into Group) _
                               .ToDictionary(Function(x) x.condition,
                                             Function(x) x.Group.ToArray(Function(xx) xx.sampleName))
        Dim calb = conditions.First
        Dim cblb = conditions.Last
        Dim LQuery = (From x As DESeq2.ResultData In inDiff
                      Let ca As Double = (From lb In calb.Value Select x.dataExpr0(lb)).Average
                      Let cb As Double = (From lb In cblb.Value Select x.dataExpr0(lb)).Average
                      Select x, upYet = ca > cb).ToArray

        Dim Up As String = out & "/" & calb.Key & "-TO-" & cblb.Key & ".Up.txt"
        Call (From x In LQuery Where x.upYet Select x.x.locus_tag).ToArray.FlushAllLines(Up)
        Dim Down As String = out & "/" & calb.Key & "-TO-" & cblb.Key & ".Down.txt"
        Call (From x In LQuery Where Not x.upYet Select x.x.locus_tag).ToArray.FlushAllLines(Down)

        Return 0
    End Function
End Module

