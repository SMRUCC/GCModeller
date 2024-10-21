Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports Microsoft.VisualBasic.Math

Public Class MSAMotif : Inherits MSAOutput

    Public Property start As Integer()
    Public Property countMatrix As Integer()()
    Public Property rowSum As Integer

    Public Property p As Double()
    Public Property q As Double()

    Public ReadOnly Property score As Double()
        Get
            Return SIMD.Divide.f64_op_divide_f64(q, p)
        End Get
    End Property

End Class
