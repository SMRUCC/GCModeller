Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Pattern
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic

Namespace Topologically

    Public Class SearchReversedRepeats : Inherits SearchWorker

        Public ReadOnly Property MinAppeared As Integer
        Public ReadOnly Property ResultSet As New List(Of RevRepeats)

        ''' <summary>
        ''' 片段按照长度的数量上的分布情况
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CountStatics As New DocumentStream.File

        Sub New(Sequence As I_PolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer,
                MinAppeared As Integer)
            Call MyBase.New(Sequence, Min, Max)
            Call CountStatics.AppendLine(New String() {"Length", "Matches", "Export"})

            Me.MinAppeared = MinAppeared
        End Sub

        Protected Overrides Sub __beginInit(ByRef seeds As List(Of String))
            seeds = (From Segment As String
                     In seeds
                     Where SequenceData.IndexOf(Segment) > -1
                     Select Segment).ToList
        End Sub

        ''' <summary>
        ''' 虽然当前的片段没有在序列上面出现，但是上一次迭代的片段却是出现的，假若序列片段没有匹配上，则上一次迭代的序列则可能为重复序列
        ''' </summary>
        ''' <param name="currentRemoves"></param>
        ''' <param name="currentStat"></param>
        ''' <param name="currLen"></param>
        Protected Overrides Sub __postResult(currentRemoves() As String, currentStat As List(Of String), currLen As Integer)
            Dim ResultExport = (From NotAppearsSegment As String
                                In currentRemoves.AsParallel
                                Let revSegment As String = NucleicAcid.Complement(
                                    New String(NotAppearsSegment.ToArray.Reverse.ToArray))
                                Let Repeats = GenerateRepeats(SequenceData, revSegment, MinAppeared)
                                Where Not Repeats Is Nothing
                                Let RepeatsLeftLoci = GenerateRepeats(SequenceData, NotAppearsSegment, MinAppeared, Rev:=True)
                                Let RevRepeats = RevRepeats.GenerateFromBase(Repeats)
                                Select RevRepeats.InvokeSet(NameOf(RevRepeats.RepeatLoci), RepeatsLeftLoci.Locations)).ToArray

            Call ResultSet.AddRange(ResultExport)
            Call CountStatics.AppendLine(New String() {currLen, currentStat.Count, ResultExport.Count})
            Call $"Length={currLen}, Chunk={currentStat.Count}, Removed={currentRemoves.Length}  .......{100 * (currLen - Min) / (Max - Min)}%".__DEBUG_ECHO
        End Sub

        Protected Overrides Function __getRemovedList(ByRef currentStat As List(Of String)) As String()
            Dim RemoveList = (From Segment As String
                              In currentStat.AsParallel
                              Let revSegment As String = NucleicAcid.Complement(New String(Segment.ToArray.Reverse.ToArray))
                              Where SequenceData.IndexOf(revSegment) = -1 OrElse
                                  FindLocation(SequenceData, revSegment).Length < MinAppeared
                              Select Segment).ToArray '找不到反向序列或者重复的次数少于阈值的，都将会被删除
            Return RemoveList
        End Function
    End Class
End Namespace