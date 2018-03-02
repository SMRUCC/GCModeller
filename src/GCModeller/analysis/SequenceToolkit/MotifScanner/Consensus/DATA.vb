Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class PopulatorParameter

    ''' <summary>
    ''' <see cref="Protocol.pairwiseSeeding(FastaSeq, FastaSeq, PopulatorParameter)"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property minW As Integer
    ''' <summary>
    ''' <see cref="Protocol.pairwiseSeeding(FastaSeq, FastaSeq, PopulatorParameter)"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property maxW As Integer
    ''' <summary>
    ''' <see cref="Protocol.pairwiseSeeding(FastaSeq, FastaSeq, PopulatorParameter)"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property seedingCutoff As Double
    Public Property ScanMinW As Integer
    Public Property ScanCutoff As Double

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function DefaultParameter() As DefaultValue(Of PopulatorParameter)
        Return New PopulatorParameter With {
            .minW = 6,
            .maxW = 20,
            .seedingCutoff = 0.9,
            .ScanCutoff = 0.6,
            .ScanMinW = 6
        }
    End Function

End Class

Public Class Motif : Inherits Probability
    Public Property seeds As MSAOutput
    Public Property length As Integer
End Class