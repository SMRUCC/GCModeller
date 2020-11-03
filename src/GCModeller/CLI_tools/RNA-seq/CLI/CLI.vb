#Region "Microsoft.VisualBasic::6e51cc16da2a76ceb35f704180edec09, CLI_tools\RNA-seq\CLI\CLI.vb"

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
    '     Constructor: (+1 Overloads) Sub New
    '     Function: DEGs, DEGsUpDown, DESeq2, Df, HTSeqCount
    '               RPKM, SamplingStats, sIdMapping
    '     Class SampleValue
    ' 
    '         Properties: Description, Name, value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.RNA_Seq.BOW

<Package("RNA-seq.CLI", Category:=APICategories.CLI_MAN, Publisher:="xie.guigang@gcmodeller.org")>
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

    '<ExportAPI("/DOOR.Corrects", Usage:="/DOOR.Corrects /DOOR <genome.opr> /pcc <pcc.dat> [/out <out.opr> /pcc-cut <0.45>]")>
    'Public Function DOORCorrects(args As CommandLine) As Integer
    '    Dim doorFile As String = args("/door")
    '    Dim pccDat As String = args("/pcc")
    '    Dim pccCut As Double = args.GetValue("/pcc-cut", 0.45)
    '    Dim out As String = args.GetValue("/out", doorFile.TrimSuffix & $".PCC={pccCut}.opr")
    '    Dim PCC As PccMatrix = MatrixSerialization.Load(from:=pccDat)
    '    Dim DOOR = DOOR_API.Load(doorFile)
    '    Dim corrects As Operon() = CorrectDoorOperon(PCC, DOOR, pccCut)

    '    Dim MAT As New IO.File
    '    Call MAT.Add({"DOOR", "locus_id", "Strand"})

    '    For Each operon As Operon In corrects
    '        Dim row As New RowObject
    '        Dim initX = operon.InitialX
    '        Call row.AddRange({operon.OperonID, initX.Synonym, initX.Location.Strand.GetBriefCode})
    '        Call MAT.Add(row)

    '        If operon.NumOfGenes = 1 Then
    '            Continue For
    '        End If

    '        Dim pre As New List(Of String)

    '        For Each gene As OperonGene In (From x In operon Where Not x.Value Is initX Select x.Value)
    '            row = New RowObject
    '            Call row.AddRange({operon.OperonID, gene.Synonym, gene.Location.Strand.GetBriefCode})
    '            Call row.Add(PCC.GetValue(initX.Synonym, gene.Synonym))

    '            For Each sId As String In pre
    '                Call row.Add(PCC.GetValue(sId, gene.Synonym))
    '            Next

    '            Call pre.Add(gene.Synonym)
    '            Call MAT.Add(row)
    '        Next
    '    Next
    '    Call MAT.Save(out & ".Views.Csv", Encodings.ASCII)
    '    Return DOOR_API.SaveFile(corrects, out).CLICode
    'End Function

    '<ExportAPI("/PCC", Usage:="/PCC /expr <expr.matrix.csv> [/out <out.dat>]")>
    'Public Function PCC(args As CommandLine) As Integer
    '    Dim expr As String = args("/expr")
    '    Dim out As String = args.GetValue("/out", expr.TrimSuffix & ".PCC.dat")
    '    Dim MAT As PccMatrix = MatrixAPI.CreatePccMAT(IO.File.Load(expr))
    '    Return MatrixSerialization.SaveBin(MAT, out).CLICode
    'End Function

    '<ExportAPI("/SPCC", Usage:="/SPCC /expr <expr.matrix.csv> [/out <out.dat>]")>
    'Public Function SPCC(args As CommandLine) As Integer
    '    Dim expr As String = args("/expr")
    '    Dim out As String = args.GetValue("/out", expr.TrimSuffix & ".SPCC.dat")
    '    Dim MAT As PccMatrix = MatrixAPI.CreateSPccMAT(IO.File.Load(expr))
    '    Return MatrixSerialization.SaveBin(MAT, out).CLICode
    'End Function

    <ExportAPI("/HT-seq", Info:="Count raw reads for DESeq2 analysis.",
               Usage:="/Ht-seq /in <in.sam> /gff <genome.gff> [/out <out.txt> /mode <union, intersection_strict, intersection_nonempty; default:intersection_nonempty> /rpkm /feature <CDS>]")>
    <ArgumentAttribute("/Mode", True,
                   Description:="The value of this parameter specific the counter of the function will be used, the available counter values are: union, intersection_strict and intersection_nonempty")>
    <ArgumentAttribute("/feature", True,
                   Description:="[NOTE: value is case sensitive!!!] Value of the gff features can be one of the: tRNA, CDS, exon, gene, tmRNA, rRNA, region")>
    Public Function HTSeqCount(args As CommandLine) As Integer
        Dim inSAM As String = args("/in")
        Dim gffFile As String = args("/gff")
        Dim out As String = args.GetValue("/out", inSAM.TrimSuffix & ".Ht-seq.txt")
        Dim Mode As String = args.GetValue("/mode", "intersection_nonempty")
        Dim RPKM As Boolean = args.GetBoolean("/RPKM")
        Dim sfeature As String = args.GetValue("/feature", "CDS")
        Dim feature As Features = FeatureKeys.featuresIndex.TryGetValue(sfeature, Features.CDS)
        Dim outDoc As String = HtseqCountMethod.HtseqCount(inSAM, gffFile, Mode, RPKM, feature)
        Return outDoc.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 如果是CDS feature的话，由于并没有直接的locus_tag，分析起来会不太直观，所以可以使用这个方法将id映射回locus_tag
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/sid.map", Usage:="/sid.map /gff <genome.gff> /raw <htseq-count.txt> [/out <out.txt>]")>
    Public Function sIdMapping(args As CommandLine) As Integer
        Dim gffFile As String = args("/gff")
        Dim raw As String = args("/raw")
        Dim out As String = args.GetValue("/out", raw.TrimSuffix & ".locus_tag.txt")
        Dim counts = CountResult.Load(raw)
        Dim gff As GFFTable = GFFTable.LoadDocument(gffFile)
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
    Public Function RPKM(args As CommandLine) As Integer
        Dim inRaw As String = args("/raw")
        Dim gffFile As String = args("/gff")
        Dim out As String = args.GetValue("/out", inRaw.TrimSuffix & ".RPKM.csv")
        Dim dataExpr0 = CountResult.Load(inRaw)
        Dim gff As GFFTable = GFFTable.LoadDocument(gffFile)
        dataExpr0 = dataExpr0.RPKM(gff)
        Return dataExpr0.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Data.Frame",
               Info:="Generates the data input for the DESeq2 R package.",
               Usage:="/Data.Frame /in <in.DIR> /ptt <genome.ptt> [/out out.csv]")>
    <ArgumentAttribute("/in", False,
                   Description:="A directory location which it contains the Ht-Seq raw count text files.")>
    Public Function Df(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & "/DESeq2_dataExpr0.csv")
        Dim ptt As String = args("/ptt")
        Dim raw = FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.txt")
        Dim result = HtseqCountMethod.DataFrame(raw, NCBI.GenBank.TabularFormat.PTT.Load(ptt),)
        Return result.Save(out, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/DESeq2", Usage:="/DESeq2 /sample.table <sampleTable.csv> /raw <raw.txt.DIR> /ptt <genome.ptt> [/design <design, default: ~condition>]")>
    Public Function DESeq2(args As CommandLine) As Integer
        Dim rawDIR As String = args("/raw")
        Dim PTT As String = args("/ptt")
        Dim design As String = args.GetValue("/design", "~condition")
        Dim dtPTT As PTT = NCBI.GenBank.TabularFormat.PTT.Load(PTT)
        Dim sampleTable As IO.File = IO.File.Load(args("/sample.table"))
        Return RTools.DESeq2.DESeq2(sampleTable, rawDIR, design, dtPTT).CLICode
    End Function

    <ExportAPI("/DEGs", Usage:="/DEGs /in <diff.csv> [/out <degs.csv> /log_fold 2]")>
    Public Function DEGs(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim logFold As Double = args.GetValue("/log_fold", 2.0R)
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & $".logFold={logFold}.csv")
        Dim diff = inFile.LoadCsv(Of RTools.DESeq2.ResultData)
        diff = (From x In diff Where Math.Abs(x.log2FoldChange) >= logFold Select x).AsList
        Return diff.SaveTo(out).CLICode
    End Function

    <ExportAPI("/DEGs.UpDown", Usage:="/DEGs.UpDown /in <diff.csv> /sample.table <sampleTable.Csv> [/out <outDIR>]")>
    Public Function DEGsUpDown(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim samples As String = args("/sample.table")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".DEGs.UpDowns/")
        Dim inDiff = inFile.LoadCsv(Of DESeq2.ResultData)
        Dim sampleTable = samples.LoadCsv(Of DESeq2.SampleTable)
        Dim conditions = (From x As DESeq2.SampleTable
                          In sampleTable
                          Select x
                          Group x By x.condition Into Group) _
                               .ToDictionary(Function(x) x.condition,
                                             Function(x) x.Group.Select(Function(xx) xx.sampleName))
        Dim calb = conditions.First
        Dim cblb = conditions.Last
        Dim LQuery = (From x As DESeq2.ResultData In inDiff
                      Let ca As Double = (From lb In calb.Value Select x.dataExpr0(lb)).Average
                      Let cb As Double = (From lb In cblb.Value Select x.dataExpr0(lb)).Average
                      Select x, upYet = ca > cb).ToArray

        Dim Up As String = out & "/" & calb.Key & "-TO-" & cblb.Key & ".Up.txt"
        Call (From x In LQuery Where x.upYet Select x.x.locus_tag).ToArray.FlushAllLines(Up, Encodings.ASCII)
        Dim Down As String = out & "/" & calb.Key & "-TO-" & cblb.Key & ".Down.txt"
        Call (From x In LQuery Where Not x.upYet Select x.x.locus_tag).ToArray.FlushAllLines(Down, Encodings.ASCII)

        Return 0
    End Function

    <ExportAPI("/Sampling.stats",
               Usage:="/Sampling.stats /in <expression.csv> /samples <stats.csv.DIR> [/out <out.csv>]")>
    Public Function SamplingStats(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim samples As String = args("/samples")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & samples.BaseName & ".stats.csv")
        Dim exps As QueryArgument() = [in].LoadCsv(Of QueryArgument)
        Dim expression = (From x As QueryArgument
                          In exps
                          Select x,
                              exp = x.Expression.Build).ToArray

        For Each file As String In ls - l - r - wildcards("*.csv") <= samples
            Dim data = file.LoadCsv(Of SampleValue)
            Dim name$ = file.BaseName

            For Each x In data
                x.Name = x.Name & " " & x.Description

                Dim LQuery = From query In expression Where query.exp.Match(x.Name) Select expo = query.x

                For Each expo In LQuery
                    expo.Data(name) = Val(expo.Data.TryGetValue(name)) + x.value
                Next
            Next
        Next

        Return exps.SaveTo(out).CLICode
    End Function

    Public Class SampleValue

        Public Property Name As String
        <Column("x")> Public Property value As Double
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Module
