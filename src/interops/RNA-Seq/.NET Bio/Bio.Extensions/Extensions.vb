Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace(".NET_Bio.Extensions", Description:="Data type convert between GCModeller and Microsoft .NET Bio", Publisher:="amethyst.asuka@gcmodeller.org")>
Public Module Extensions

    <ExportAPI("ToFasta", Info:="Convert the .NET Bio type sequence object as a GCModeller fasta sequence object.")>
    <Extension> Public Function ToFasta(BioSequence As Bio.ISequence) As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
        Dim SequenceData As String = New Bio.Sequence(BioSequence).ToString
        Dim attr = (From dt In BioSequence.Metadata Select $"{dt.Key}:={dt.Value?.ToString}").ToArray
        Dim Fasta = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {.Attributes = attr, .SequenceData = SequenceData}
        Return Fasta
    End Function

End Module
