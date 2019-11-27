Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Fasta sequence toolkit
''' </summary>
''' 
<Package("bioseq.fasta")>
Module Fasta

    Sub New()
        Call printer.AttachConsoleFormatter(Of FastaSeq)(Function(seq)
                                                             With DirectCast(seq, FastaSeq)
                                                                 Return "> " & .Title & ASCII.LF & .SequenceData
                                                             End With
                                                         End Function)
    End Sub

    ''' <summary>
    ''' Read a single fasta sequence file
    ''' </summary>
    ''' <param name="file">
    ''' Just contains one sequence
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.seq")>
    Public Function readSeq(file As String) As FastaSeq
        Return FastaSeq.Load(file)
    End Function
End Module
