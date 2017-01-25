#Region "Microsoft.VisualBasic::8499b22dcc240bc81686446835e8200c, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\SearchReversedRepeats.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Pattern
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically

    Public Class SearchReversedRepeats : Inherits SearchWorker

        Public ReadOnly Property MinAppeared As Integer
        Public ReadOnly Property ResultSet As New List(Of RevRepeats)

        ''' <summary>
        ''' 片段按照长度的数量上的分布情况
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CountStatics As New IO.File

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
            Dim setValue = New SetValue(Of RevRepeats) <= NameOf(RevRepeats.RepeatLoci)
            Dim ResultExport As RevRepeats() =
                LinqAPI.Exec(Of RevRepeats) <= From NotAppearsSegment As String
                                               In currentRemoves.AsParallel
                                               Let revSegment As String = NucleicAcid.Complement(
                                                   New String(NotAppearsSegment.ToArray.Reverse.ToArray))
                                               Let Repeats = GenerateRepeats(SequenceData, revSegment, MinAppeared)
                                               Where Not Repeats Is Nothing
                                               Let RepeatsLeftLoci = GenerateRepeats(SequenceData, NotAppearsSegment, MinAppeared, Rev:=True)
                                               Let RevRepeats = RevRepeats.GenerateFromBase(Repeats)
                                               Select setValue(RevRepeats, RepeatsLeftLoci.Locations)

            Call ResultSet.AddRange(ResultExport)
            Call CountStatics.AppendLine(New String() {currLen, currentStat.Count, ResultExport.Count})
            Call $"Length={currLen}, Chunk={currentStat.Count}, Removed={currentRemoves.Length}  .......{100 * (currLen - Min) / (Max - Min)}%".__DEBUG_ECHO
        End Sub

        ''' <summary>
        ''' 获取将要进行移除的片段列表
        ''' </summary>
        ''' <param name="currentStat"></param>
        ''' <returns></returns>
        Protected Overrides Function __getRemovedList(ByRef currentStat As List(Of String)) As String()
            Dim RemoveList As String() = LinqAPI.Exec(Of String) <=
 _
                From Segment As String
                In currentStat.AsParallel
                Let revStr As String = New String(Segment.ToArray.Reverse.ToArray)
                Let revSegment As String = NucleicAcid.Complement(revStr)
                Where SequenceData.IndexOf(revSegment) = -1 OrElse
                    FindLocation(SequenceData, revSegment).Length < MinAppeared
                Select Segment '找不到反向序列或者重复的次数少于阈值的，都将会被删除

            Return RemoveList
        End Function
    End Class
End Namespace
