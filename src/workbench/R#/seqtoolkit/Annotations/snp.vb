Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP.SangerSNPs
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("snp_toolkit")>
<RTypeExport("snp", GetType(SNP))>
Module snpTools

    <ExportAPI("snp_scan")>
    Public Function snp_scan(nt As FastaFile,
                             ref_index$,
                             Optional pureMode As Boolean = False,
                             Optional monomorphic As Boolean = False,
                             Optional ByRef vcf_output_filename$ = Nothing) As SNPsAln

        Return nt.ScanSNPs(ref_index, pureMode, monomorphic, vcf_output_filename)
    End Function
End Module
