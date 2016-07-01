Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans
Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace Analysis.FootprintTraceAPI

    ''' <summary>
    ''' 从计算出来的footprint数据之中选出motif用来进行聚类操作
    ''' </summary>
    Public Module Selects

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="trace"></param>
        ''' <param name="MEME">MEME DIR</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("")>
        <Extension>
        Public Function [Select](trace As FootprintTrace, MEME As String) As AnnotationModel()
            Dim memeHash As Dictionary(Of AnnotationModel) = AnnotationModel.LoadMEMEOUT(MEME)
            Dim LQuery = (From x As MatchResult
                          In trace.Footprints
                          Select x.Select(memeHash)).MatrixToVector
            Return LQuery
        End Function

        <Extension>
        Public Function [Select](footprints As MatchResult, models As Dictionary(Of AnnotationModel)) As AnnotationModel()
            If footprints.Matches.IsNullOrEmpty Then
                Return Nothing
            Else
                Dim LQuery = (From x As MotifHits In footprints.Matches
                              Let tks As String() = Strings.Split(x.Trace, "::")
                              Let uid As String = $"{tks(Scan0)}.{tks(1)}"
                              Where models.ContainsKey(uid)
                              Select models(uid)).ToArray
                Return LQuery
            End If
        End Function
    End Module
End Namespace