Namespace Core

    Public Class DockOptions

        Public Exhaustiveness As Int32 = 8
        Public StepsPerRun As Int32 = 30
        Public NumModes As Int32 = 9
        Public MinRmsd As Double = 1.5
        Public BoxCenter() As Double = Nothing
        Public BoxHalfSize As Double = 12.0
        Public Seed As Int32 = 0
        Public Mmgbsa As Boolean = False
        Public Nwat As Int32 = 0
        Public MmgbsaTop As Int32 = 3

    End Class
End Namespace