#Region "Microsoft.VisualBasic::9c458b861faa904270596d5cc5e2cb4a, RNA-Seq\Assembler\CLI\GeneMark.vb"

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
    '     Function: GeneMark, ParseGeneMark
    ' 
    '     Sub: __extractDoc, __tryExtract
    ' 
    ' /********************************************************************************/

#End Region

Imports LANS.SystemsBiology.Toolkits.RNA_Seq
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.GenePrediction
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Partial Module CLI

    <ExportAPI("--parse.GeneMark", Usage:="--parse.GeneMark /doc <geneMark.txt> [/nt <nt.fasta> /out <out.dir>]")>
    Public Function ParseGeneMark(args As CommandLine.CommandLine) As Integer
        Dim Doc As String = args("/doc")
        Dim Out As String = args.GetValue("/out", Doc.TrimFileExt)
        Dim Genes = GenePrediction.GeneMark.ParseDoc(Doc)
        Dim Nt = LANS.SystemsBiology.SequenceModel.FASTA.FastaToken.Load(args("/nt"))

        Call __extractDoc(Genes, Nt, Out)

        Return 0
    End Function

    Private Sub __extractDoc(doc As GeneMark, nt As SequenceModel.FASTA.FastaToken, out As String)
        Dim l As Integer

        If Not nt Is Nothing Then
            Try
                Dim parser As New SequenceModel.NucleotideModels.SegmentReader(nt)
                Dim LQuery = (From gene As DocNodes.PredictedGene
                              In doc.PredictedGenes.PredictedGenes
                              Let ORF = New SequenceModel.FASTA.FastaToken With {
                                  .SequenceData = parser.TryParse(gene.Location).SequenceData,
                                  .Attributes = {gene.ToString}
                              }
                              Select ORF).ToArray
                If LQuery.IsNullOrEmpty Then
                    LQuery = {nt}
                End If
                Call New SequenceModel.FASTA.FastaFile(LQuery).Save(out & "/ORF.fasta")
            Catch ex As Exception
                Call New SequenceModel.FASTA.FastaFile(nt).Save(out & "/ORF.fasta")
            End Try

            l = nt.Length
        Else
            l = 0
        End If

        Dim PTT As New Assembly.NCBI.GenBank.TabularFormat.PTT(
            doc.PredictedGenes.PredictedGenes,
            nt.Title.NormalizePathString,
            l)

        Call doc.GetXml.SaveTo(out & "/GeneMark.xml")
        Call PTT.Save(out & "/ORF.PTT")
        Call doc.FramShifts.FrameShifts.SaveTo(out & "/FrameShifts.csv")
        Call doc.InterestRegions.Regions.SaveTo(out & "/InterestRegions.csv")
        Call doc.lstORFs.ORFs.SaveTo(out & "/ORFs.csv")
    End Sub

    <ExportAPI("--Genemark", Usage:="--Genemark /in <anno.fasta> [/not-bacterial /liner]")>
    Public Function GeneMark(args As CommandLine.CommandLine) As Integer
        Dim inFasta = LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.LoadNucleotideData(args("/in"))
        Dim isBacterial As Boolean = Not args.GetBoolean("/not-bacterial")
        Dim isLiner As Boolean = args.GetBoolean("/liner")
        Dim result = GenePrediction.NCBIWebMaster.GetsPredictData(inFasta, isBacterial, isLiner)
        Dim out As String = args("/in").TrimFileExt & "/Genemark/"
        Dim array = inFasta.ToArray

        For i As Integer = 0 To array.Length - 1
            Try
                Call __tryExtract(out, array(i), result(i))
            Catch ex As Exception
                ex = New Exception(result(i), ex)
                Call App.LogException(ex)
            End Try
        Next

        Return 0
    End Function

    Private Sub __tryExtract(out As String, source As SequenceModel.FASTA.FastaToken, genemark As String)
        Dim DIR As String = out & "/" & source.Title.NormalizePathString
        Call genemark.SaveTo(DIR & "/Genmark.txt")
        Dim doc = GenePrediction.GeneMark.Parser(genemark)
        Call __extractDoc(doc, source, DIR)
    End Sub
End Module
