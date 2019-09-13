Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Pipeline.LocalBlast

    ''' <summary>
    ''' A hit model of pfam domain annotation on query protein sequence.
    ''' </summary>
    Public Class PfamHit : Inherits BestHit

        ''' <summary>
        ''' The start position of this domain object on target sequence
        ''' </summary>
        ''' <returns></returns>
        Public Property start As Integer
        ''' <summary>
        ''' The end position of this domain object on target sequence.
        ''' </summary>
        ''' <returns></returns>
        Public Property ends As Integer

    End Class
End Namespace