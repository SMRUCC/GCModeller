'Namespace MBF

'    Public Class Assembler

'        Public Function Assembling(Fq As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
'            Dim Assembler As New Bio.Algorithms.Assembly.OverlapDeNovoAssembler
'            Dim Fastq = New Bio.IO.FastQ.FastQParser().Parse(New IO.FileStream(Fq, IO.FileMode.Open))
'            Dim resultBuffer = Assembler.Assemble(Fastq)
'            Dim LQuery = (From sequence In resultBuffer.AssembledSequences.AsParallel Select sequence.ToFasta).ToArray
'            Return LQuery
'        End Function



'        Sub map(fq As String)
'            Dim mapper As New Bio.Algorithms.Assembly.MatePairMapper
'            Dim asfds As New Bio.Algorithms.Assembly.Padena.Scaffold.ReadContigMap
'            Dim Fastq = New Bio.IO.FastQ.FastQParser().Parse(New IO.FileStream(fq, IO.FileMode.Open))

'        End Sub

'    End Class
'End Namespace