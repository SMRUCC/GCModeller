Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace DocumentFormat

    Public Module TSSsDataViews

        Public Function _5UTRLength(source As Generic.IEnumerable(Of Transcript)) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim lens = (From x In source Where Not x.IsRNA AndAlso Not String.IsNullOrEmpty(x.TSS_ID) Select x._5UTR Group _5UTR By _5UTR Into Count).ToArray
            Dim max = (From x In lens Select x._5UTR).Max
            Dim hash = lens.ToDictionary(Function(x) x._5UTR, Function(x) x.Count)
            Dim LQuery = (From i As Integer In (max + 1).Sequence Select len = i, numOfTSSs = hash.TryGetValue(i, [default]:=0)).ToArray
            Return LQuery.ToCsvDoc(False)
        End Function

        Public Function _5UTR(source As Generic.IEnumerable(Of Transcript), genome As SequenceModel.FASTA.FastaToken) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Dim reader As New SequenceModel.NucleotideModels.SegmentReader(genome)
            source = (From x In source Where Not x.IsRNA AndAlso Not String.IsNullOrEmpty(x.TSS_ID) AndAlso x._5UTR > 0 Select x).ToArray
            Dim lst5UTR = (From transcript As Transcript In source
                           Let loci = transcript.__5UTRRegion
                           Let seq = reader.TryParse(loci)
                           Let fa = New SequenceModel.FASTA.FastaToken With {
                               .SequenceData = seq.SequenceData,
                               .Attributes = {transcript.TSS_ID, transcript.Synonym}
                           }
                           Select fa).ToArray
            Return New SequenceModel.FASTA.FastaFile(lst5UTR)
        End Function

        Public Function TSSs(source As Generic.IEnumerable(Of Transcript),
                             genome As SequenceModel.FASTA.FastaToken,
                             len As Integer) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Dim offset As Integer = len / 2
            Dim reader As New SequenceModel.NucleotideModels.SegmentReader(genome)
            source = (From x In source Where Not String.IsNullOrEmpty(x.TSS_ID) Select x).ToArray
            Dim lstTSSs = (From transcript As Transcript In source
                           Let loci = transcript.__TSSsRegion(offset)
                           Let seq = reader.TryParse(loci)
                           Let fa = New SequenceModel.FASTA.FastaToken With {
                               .SequenceData = seq.SequenceData,
                               .Attributes = {transcript.TSS_ID, transcript.Synonym}
                           }
                           Select fa).ToArray
            Return New SequenceModel.FASTA.FastaFile(lstTSSs)
        End Function

        Public Function UpStream(source As Generic.IEnumerable(Of Transcript),
                                 genome As SequenceModel.FASTA.FastaToken,
                                 len As Integer) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Dim reader As New SequenceModel.NucleotideModels.SegmentReader(genome)
            source = (From x In source Where Not x.IsRNA AndAlso Not String.IsNullOrEmpty(x.TSS_ID) Select x).ToArray
            Dim lstUpStream = (From transcript As Transcript In source
                               Let loci = transcript.__upStreamRegion(offset:=len)
                               Let seq = reader.TryParse(loci)
                               Let fa = New SequenceModel.FASTA.FastaToken With {
                                   .SequenceData = seq.SequenceData,
                                   .Attributes = {transcript.TSS_ID, transcript.Synonym}
                               }
                               Select fa).ToArray
            Return New SequenceModel.FASTA.FastaFile(lstUpStream)
        End Function

        <Extension> Private Function __upStreamRegion(loci As Transcript, offset As Integer) As NucleotideLocation
            Dim site As NucleotideLocation

            If loci.MappingLocation.Strand = Strands.Forward Then
                site = New NucleotideLocation(loci.TSSs, loci.TSSs - offset, Strands.Forward)
            Else
                site = New NucleotideLocation(loci.TSSs, loci.TSSs + offset, Strands.Reverse)
            End If

            Return site.Normalization
        End Function

        <Extension> Private Function __TSSsRegion(loci As Transcript, offset As Integer) As NucleotideLocation
            Dim site As New NucleotideLocation(loci.TSSs - offset, loci.TSSs + offset, loci.MappingLocation.Strand)
            Return site.Normalization
        End Function

        <Extension> Private Function __5UTRRegion(loci As Transcript) As SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
            Dim site As New NucleotideLocation(loci.ATG, loci.TSSs, loci.MappingLocation.Strand)
            Return site.Normalization
        End Function
    End Module
End Namespace