Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("FastQ")>
Public Module FastQ

    <ExportAPI("quality_score")>
    <RApiReturn(GetType(Double))>
    Public Function GetQualityScore(q As Object, Optional env As Environment = Nothing) As Object
        If q Is Nothing Then
            Return Nothing
        End If

        If TypeOf q Is String Then
            Return FQ.FastQ _
                .GetQualityOrder(CStr(q)) _
                .Select(Function(d) CDbl(d)) _
                .ToArray
        ElseIf TypeOf q Is FastQFile Then
            Return New list With {
                .slots = DirectCast(q, FastQFile) _
                    .ToDictionary(Function(i) i.Title,
                                  Function(i)
                                      Dim scores = FQ.FastQ _
                                         .GetQualityOrder(i.Quality) _
                                         .Select(Function(d) CDbl(d)) _
                                         .ToArray

                                      Return CObj(scores)
                                  End Function)
            }
        ElseIf TypeOf q Is FQ.FastQ Then
            Return FQ.FastQ _
                .GetQualityOrder(DirectCast(q, FQ.FastQ).Quality) _
                .Select(Function(d) CDbl(d)) _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(FQ.FastQ), q.GetType, env)
        End If
    End Function
End Module
