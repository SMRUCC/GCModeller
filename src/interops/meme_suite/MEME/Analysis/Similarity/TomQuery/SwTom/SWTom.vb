Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.AnalysisTools.SequenceTools.SmithWaterman
Imports SMRUCC.genomics.AnalysisTools
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports System.Runtime.CompilerServices

Namespace Analysis.Similarity.TOMQuery

    Public Class SWAlignment : Inherits GSW(Of MotifScans.ResidueSite)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="equals">越相似得分应该越高</param>
        Sub New(query As MotifScans.AnnotationModel,
                subject As MotifScans.AnnotationModel,
                equals As ISimilarity(Of MotifScans.ResidueSite))
            Call MyBase.New(query.PWM, subject.PWM, equals, AddressOf TomTOm.ToChar)
        End Sub
    End Class

    <PackageNamespace("TOMQuery.Smith-Waterman", Category:=APICategories.ResearchTools)>
    Public Module SWTom

        Private Class __similarity

            ReadOnly __compares As TomTOm.ColumnCompare
            ReadOnly __offsets As Double = 0.6

            Sub New(compare As TomTOm.ColumnCompare, offset As Double)
                __compares = compare
                __offsets = offset
            End Sub

            Public Function Similarity(x As MotifScans.ResidueSite, y As MotifScans.ResidueSite) As Integer
                Dim value As Double = __compares(x, y)
                value = 10 * (value - __offsets)
                Return CInt(value)
            End Function
        End Class

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="method"></param>
        ''' <param name="cutoff">0% - 100%</param>
        ''' <param name="minW">
        ''' The  minimal width of the motif hsp fragment size, default is 6 residue, this is recommended by meme program.
        ''' </param>
        ''' <returns></returns>
        <ExportAPI("Compare")>
        Public Function Compare(query As MotifScans.AnnotationModel,
                                subject As MotifScans.AnnotationModel,
                                Optional method As String = "pcc",
                                Optional cutoff As Double = 0.75,
                                Optional minW As Integer = 4,
                                Optional tomThreshold As Double = 0.75,
                                Optional bitsLevel As Double = 1.5) As Output
            Dim param As New Parameters With {
                .Method = method,
                .MinW = minW,
                .SWThreshold = cutoff,
                .TOMThreshold = tomThreshold,
                .BitsLevel = bitsLevel
            }
            Return Compare(query, subject, param)
        End Function

        <ExportAPI("Compare")>
        Public Function Compare(query As MotifScans.AnnotationModel,
                                subject As MotifScans.AnnotationModel,
                                params As Parameters) As Output
            Dim similarity As ISimilarity(Of MotifScans.ResidueSite) =
                AddressOf New __similarity(TomTOm.GetMethod(params.Method), params.SWOffset).Similarity
            Return Compare(query, subject, similarity, params)
        End Function

        <ExportAPI("Compare")>
        Public Function Compare(query As MotifScans.AnnotationModel,
                                subject As MotifScans.AnnotationModel,
                                method As ISimilarity(Of MotifScans.ResidueSite),
                                params As Parameters) As Output
            Dim sw As New SWAlignment(query, subject, method)
            Dim out As SequenceTools.SmithWaterman.Output =
                SequenceTools.SmithWaterman.Output.CreateObject(sw, AddressOf TomTOm.ToChar, params.SWThreshold, params.MinW)
            Dim output As New Output With {
                .Query = query,
                .Subject = subject,
                .HSP = __alignHSP(query, subject, out, params),
                .Directions = out.Directions,
                .DP = out.DP,
                .Parameters = params,
                .QueryMotif = query.Motif,
                .SubjectMotif = subject.Motif
            }
            Return output
        End Function

        Private Function __alignHSP(query As MotifScans.AnnotationModel,
                                    subject As MotifScans.AnnotationModel,
                                    sw As SequenceTools.SmithWaterman.Output,
                                    param As Parameters) As SW_HSP()
            Dim method = TomTOm.GetMethod(param.Method)
            Dim alignment = (From out As SW_HSP
                             In sw.HSP.ToArray(Function(x) x.__alignInvoke(query, subject, method, param), Parallel:=param.Parallel)
                             Where Not out Is Nothing
                             Select out).ToArray
            Return alignment
        End Function

        <Extension> Private Function __alignInvoke(hsp As HSP,
                                                   query As MotifScans.AnnotationModel,
                                                   subject As MotifScans.AnnotationModel,
                                                   method As TomTOm.ColumnCompare,
                                                   param As Parameters) As SW_HSP
            Dim queryp As MotifScans.AnnotationModel = __parts(query, hsp.FromA, hsp.ToA)
            Dim subjectp As MotifScans.AnnotationModel = __parts(subject, hsp.FromB, hsp.ToB)
            Dim out As DistResult = TomTOm.Compare(
                queryp,
                subjectp,
                method,
                param)

            If out Is Nothing OrElse out.MatchSimilarity < param.TOMThreshold Then
                Return Nothing
            End If

            Dim align As New SW_HSP With {
                .Query = queryp,
                .Subject = subjectp,
                .Alignment = out,
                .ToS = hsp.ToB,
                .FromQ = hsp.FromA,
                .FromS = hsp.FromB,
                .Score = hsp.Score,
                .ToQ = hsp.ToA
            }
            Return align
        End Function

        Private Function __parts(LDM As MotifScans.AnnotationModel,
                                 start As Integer,
                                 ends As Integer) As MotifScans.AnnotationModel
            Dim array As MotifScans.ResidueSite() = LDM.PWM.Skip(start).Take(ends - start).ToArray
            Return New MotifScans.AnnotationModel With {
                .PWM = array,
                .Uid = LDM.Uid
            }
        End Function

        <ExportAPI("Compare.Best")>
        Public Function CompareBest(query As MotifScans.AnnotationModel,
                                    method As ISimilarity(Of MotifScans.ResidueSite),
                                    param As Parameters) As Output()
            Dim LQuery = (From x In TomTOm.Motifs Select Compare(query, x.Value, method, param)).ToArray
            Call Console.Write(".")
            Return LQuery
        End Function

        <ExportAPI("Compare.Best")>
        Public Function CompareBest(memeText As String, param As Parameters) As Output()
            Dim similarity As ISimilarity(Of MotifScans.ResidueSite) =
                AddressOf New __similarity(TomTOm.GetMethod(param.Method), param.SWOffset).Similarity
            Dim query = MotifScans.AnnotationModel.LoadDocument(memeText)
            Dim LQuery = (From x As MotifScans.AnnotationModel
                          In query.AsParallel
                          Select CompareBest(x, similarity, param)).ToArray
            Dim resultSet = (From x In LQuery.MatrixToList Where x.Match Select x).ToArray
            Return resultSet
        End Function
    End Module
End Namespace