#Region "Microsoft.VisualBasic::847cf657613c5a3ce818fba6cf1cb685, shared\docs\Views.vb"

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

    '     Module TSSsDataViews
    ' 
    '         Function: __5UTRRegion, __TSSsRegion, __upStreamRegion, _5UTR, _5UTRLength
    '                   TSSs, UpStream
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
Imports SMRUCC.genomics.SequenceModel

Namespace DocumentFormat

    Public Module TSSsDataViews

        Public Function _5UTRLength(source As Generic.IEnumerable(Of Transcript)) As File
            Dim lens = (From x In source Where Not x.IsRNA AndAlso Not String.IsNullOrEmpty(x.TSS_ID) Select x._5UTR Group _5UTR By _5UTR Into Count).ToArray
            Dim max = (From x In lens Select x._5UTR).Max
            Dim hash = lens.ToDictionary(Function(x) x._5UTR, Function(x) x.Count)
            Dim LQuery = (From i As Integer In (max + 1).Sequence Select len = i, numOfTSSs = hash.TryGetValue(i, [default]:=0)).ToArray
            Return LQuery.ToCsvDoc(False)
        End Function

        Public Function _5UTR(source As Generic.IEnumerable(Of Transcript), genome As SequenceModel.FASTA.FastaSeq) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            source = (From x In source Where Not x.IsRNA AndAlso Not String.IsNullOrEmpty(x.TSS_ID) AndAlso x._5UTR > 0 Select x).ToArray
            Dim lst5UTR = (From transcript As Transcript In source
                           Let loci = transcript.__5UTRRegion
                           Let seq = genome.CutSequenceLinear(loci)
                           Let fa = New SequenceModel.FASTA.FastaSeq With {
                               .SequenceData = seq.SequenceData,
                               .Headers = {transcript.TSS_ID, transcript.Synonym}
                           }
                           Select fa).ToArray
            Return New SequenceModel.FASTA.FastaFile(lst5UTR)
        End Function

        Public Function TSSs(source As Generic.IEnumerable(Of Transcript),
                             genome As SequenceModel.FASTA.FastaSeq,
                             len As Integer) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Dim offset As Integer = len / 2
            source = (From x In source Where Not String.IsNullOrEmpty(x.TSS_ID) Select x).ToArray
            Dim lstTSSs = (From transcript As Transcript In source
                           Let loci = transcript.__TSSsRegion(offset)
                           Let seq = genome.CutSequenceLinear(loci)
                           Let fa = New SequenceModel.FASTA.FastaSeq With {
                               .SequenceData = seq.SequenceData,
                               .Headers = {transcript.TSS_ID, transcript.Synonym}
                           }
                           Select fa).ToArray
            Return New SequenceModel.FASTA.FastaFile(lstTSSs)
        End Function

        Public Function UpStream(source As Generic.IEnumerable(Of Transcript),
                                 genome As SequenceModel.FASTA.FastaSeq,
                                 len As Integer) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            source = (From x In source Where Not x.IsRNA AndAlso Not String.IsNullOrEmpty(x.TSS_ID) Select x).ToArray
            Dim lstUpStream = (From transcript As Transcript In source
                               Let loci = transcript.__upStreamRegion(offset:=len)
                               Let seq = genome.CutSequenceLinear(loci)
                               Let fa = New SequenceModel.FASTA.FastaSeq With {
                                   .SequenceData = seq.SequenceData,
                                   .Headers = {transcript.TSS_ID, transcript.Synonym}
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
