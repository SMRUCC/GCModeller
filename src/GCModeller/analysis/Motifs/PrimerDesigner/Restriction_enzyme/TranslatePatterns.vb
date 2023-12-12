Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Restriction_enzyme

    Public Module TranslatePatterns

        ''' <summary>
        ''' translate the recongnition site string as regular expression motif pattern data.
        ''' </summary>
        ''' <param name="enzyme"></param>
        ''' <returns></returns>
        <Extension>
        Public Function TranslateRegular(enzyme As Enzyme, Optional direction As Strands = Strands.Forward) As MotifPattern
            Dim site As String = enzyme.Recognition(direction)

        End Function
    End Module
End Namespace