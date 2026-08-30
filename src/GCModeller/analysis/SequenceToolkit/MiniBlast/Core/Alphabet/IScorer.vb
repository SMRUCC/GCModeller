Namespace Core

    ''' <summary>打分器统一接口</summary>
    Public Interface IScorer

        Function Score(a As Int32, b As Int32) As Double

    End Interface
End Namespace