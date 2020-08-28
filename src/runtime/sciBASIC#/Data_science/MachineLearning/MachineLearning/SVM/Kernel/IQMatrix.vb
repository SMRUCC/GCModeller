
Namespace SVM

    Friend Interface IQMatrix
        Function GetQ(ByVal column As Integer, ByVal len As Integer) As Single()
        Function GetQD() As Double()
        Sub SwapIndex(ByVal i As Integer, ByVal j As Integer)
    End Interface
End Namespace