Namespace Core

    Public Class CellDir

        Public DirH As SByte
        Public DirE As SByte
        Public DirF As SByte

    End Class

    Public Class UngappedResult

        Public BestScore As Double
        Public BestFrom As Integer
        Public BestTo As Integer
        Public SeedI As Integer
        Public SeedJ As Integer

    End Class

    Public Class GappedForwardResult

        Public Best As Double
        Public BestU As Integer
        Public BestV As Integer
        Public TotalCells As Long
        Public Traces As Dictionary(Of Integer, Dictionary(Of Integer, CellDir))

    End Class
End Namespace