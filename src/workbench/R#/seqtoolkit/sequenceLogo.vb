Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.SequenceModel.FASTA

<Package("bioseq.sequenceLogo")>
Module sequenceLogo

    <ExportAPI("plot.seqLogo")>
    Public Function DrawLogo(MSA As FastaFile, Optional title$ = "") As Image
        Return DrawingDevice.DrawFrequency(MSA, title)
    End Function
End Module
