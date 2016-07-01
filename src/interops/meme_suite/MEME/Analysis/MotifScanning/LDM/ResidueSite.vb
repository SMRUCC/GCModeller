Imports System.Xml.Serialization
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.Similarity
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.AnalysisTools.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.DatabaseServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Analysis.MotifScans

    ''' <summary>
    ''' A column in the motif
    ''' </summary>
    Public Class ResidueSite : Inherits Motif.ResidueSite

        Sub New()
            Call MyBase.New
        End Sub

        Sub New(c As Char)
            Call MyBase.New(c)
        End Sub

        Public Function ToNtBase() As MotifPM
            Return New MotifPM(PWM(0), PWM(1), PWM(2), PWM(3)) With {.Bits = Bits}
        End Function
    End Class
End Namespace