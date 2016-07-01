Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module Utilities

    <ExportAPI("/SNP",
               Usage:="/SNP /in <nt.fasta> [/ref 0 /pure /monomorphic]")>
    <ParameterInfo("/in", False, AcceptTypes:={GetType(FastaFile)}, Description:="")>
    <ParameterInfo("/ref", True, AcceptTypes:={GetType(Integer)})>
    <ParameterInfo("/pure", True, AcceptTypes:={GetType(Boolean)})>
    Public Function SNP(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim pure As Boolean = args.GetBoolean("/pure")
        Dim monomorphic As Boolean = args.GetBoolean("/monomorphic")
        Dim nt As New FastaFile([in])
        Dim ref As Integer = args.GetInt32("/ref")
        Dim json As String = [in].TrimFileExt & ".SNPs.args.json"
        Return nt.ScanSNPs(ref, pure, monomorphic).GetJson.SaveTo(json)
    End Function
End Module
