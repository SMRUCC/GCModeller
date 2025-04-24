Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text

''' <summary>
''' Make text matches of the ocr text data
''' </summary>
Public Module OcrTextMatches

    <Extension>
    Public Iterator Function MatchGroup(group As Trajectory, target As String) As IEnumerable(Of Double)
        For Each frame As Detection In group.objectSet
            Yield Similarity.LevenshteinEvaluate(target, frame.ObjectID)
        Next
    End Function

End Module
