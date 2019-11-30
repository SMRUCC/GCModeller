Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.SequenceModel.FASTA

<Package("bioseq.sequenceLogo")>
Module sequenceLogo

    ''' <summary>
    ''' Drawing the sequence logo just simply modelling this motif site from the clustal multiple sequence alignment.
    ''' </summary>
    ''' <param name="MSA"></param>
    ''' <param name="title"></param>
    ''' <returns></returns>
    <ExportAPI("plot.seqLogo")>
    Public Function DrawLogo(MSA As FastaFile, Optional title$ = "") As Image
        Return DrawingDevice.DrawFrequency(MSA, title)
    End Function
End Module
