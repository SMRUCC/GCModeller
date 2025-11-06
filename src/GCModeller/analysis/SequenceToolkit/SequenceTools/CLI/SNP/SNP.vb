#Region "Microsoft.VisualBasic::964c7d0e2e8a1e13c30c04e6ba6353ae, analysis\SequenceToolkit\SequenceTools\CLI\SNP\SNP.vb"

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


    ' Code Statistics:

    '   Total Lines: 102
    '    Code Lines: 88 (86.27%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (13.73%)
    '     File Size: 4.45 KB


    ' Module Utilities
    ' 
    '     Function: Genotype, GenotypeStatics, SNP, TimeDiffs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP.VCF
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module Utilities

    <ExportAPI("/SNP",
               Usage:="/SNP /in <nt.fasta> [/ref <int_index/title, default:0> /pure /monomorphic /high <0.65>]")>
    <ArgumentAttribute("/in", False, AcceptTypes:={GetType(FastaFile)}, Description:="")>
    <ArgumentAttribute("/ref", True, AcceptTypes:={GetType(Integer)})>
    <ArgumentAttribute("/pure", True, AcceptTypes:={GetType(Boolean)})>
    <Group(CLIGrouping.SNPTools)>
    Public Function SNP(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim pure As Boolean = args("/pure")
        Dim monomorphic As Boolean = args("/monomorphic")
        Dim nt As New FastaFile([in])
        Dim ref$ = args.GetValue("/ref", "0")
        Dim high# = args.GetValue("/high", 0.65)
        Dim json As String = [in].TrimSuffix & ".SNPs.args.json"
        Dim vcf$ = Nothing

        Call nt _
            .ScanSNPs(ref, pure, monomorphic, vcf_output_filename:=vcf) _
            .GetJson _
            .SaveTo(json)

        Return SNPVcf.VcfHighMutateScreens(vcf, cut:=high#) _
            .SaveTo(vcf.TrimSuffix & $"-hight_{high}.vcf")
    End Function

    <ExportAPI("/Time.Mutation",
               Info:="The ongoing time mutation of the genome sequence.",
               Usage:="/Time.Mutation /in <aln.fasta> [/ref <default:first,other:title/index> /cumulative /out <out.csv>]")>
    <Group(CLIGrouping.SNPTools)>
    Public Function TimeDiffs(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".time_variation.csv")
        Dim isCumulative As Boolean = args("/cumulative")

        If isCumulative Then
            out = out.TrimSuffix & "-cumulative.csv"
        End If

        Dim source As New FastaFile([in])
        Dim raw As List(Of EntityObject) = Nothing
        Dim result As DataSet() = source _
            .GetSeqs(args("/ref")) _
            .GroupByDate(isCumulative, raw)
        Dim T As IO.File = result.ToCsvDoc.Transpose
        Dim rawData As File = raw.ToCsvDoc.Transpose
        Call rawData.Save(out.TrimSuffix & "-raw.csv", Encodings.ASCII)
        Return T.Save(out, Encoding.ASCII)
    End Function

    <ExportAPI("/Genotype", Usage:="/Genotype /in <raw.csv> [/out <out.Csv>]")>
    Public Function Genotype(args As CommandLine) As Integer
        Dim [in] As String = args("/in")

        If [in].FileExists Then
            Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".genotype.Csv")
            Dim data = [in].LoadCsv(Of GenotypeDetails)
            Dim result = data.TransViews
            Return result.Save(out, Encodings.ASCII)
        Else
            Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".genotype/")

            For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
                Dim data = file.LoadCsv(Of GenotypeDetails)
                Dim result = data.TransViews
                Dim out As String = EXPORT & file.BaseName & ".Csv"
                Call result.Save(out, Encodings.ASCII)
            Next

            Return 0
        End If
    End Function

    <ExportAPI("/Genotype.Statics", Usage:="/Genotype.Statics /in <in.DIR> [/out <EXPORT>]")>
    Public Function GenotypeStatics(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXOIRT As String = args.GetValue("/out", [in].TrimDIR & ".Statics.EXPORT/")

        For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
            Dim data As IO.File = IO.File.Load(file)
            Dim out As String = EXOIRT & "/" & file.BaseName & ".Csv"
            Dim result = data.Statics
            Call result.Save(out, Encodings.ASCII)
        Next

        Return 0
    End Function
End Module
