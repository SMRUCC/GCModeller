#Region "Microsoft.VisualBasic::0e61f9fb6b5a167cf51d0ed62452923c, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\RepeatsSearchs.vb"

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
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically

    Public Class RepeatsSearchs : Inherits SearchWorker

        Public ReadOnly Property MinAppeared As Integer
        ''' <summary>
        ''' 片段按照长度的数量上的分布情况
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CountStatics As New IO.File
        Public ReadOnly Property ResultSet As New List(Of Repeats)

        Sub New(Sequence As I_PolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer,
                MinAppeared As Integer)
            Call MyBase.New(Sequence, Min, Max)
            Call CountStatics.AppendLine(New String() {"Length", "Matches", "Export"})

            Me.MinAppeared = MinAppeared
        End Sub

        Protected Overrides Function __getRemovedList(ByRef currentStat As List(Of String)) As String()
            Dim RemoveList = (From Segment As String
                              In currentStat.AsParallel
                              Where SequenceData.IndexOf(Segment) = -1 OrElse
                                  Pattern.FindLocation(SequenceData, Segment).Length < MinAppeared
                              Select Segment).ToArray
            Return RemoveList
        End Function

        ''' <summary>
        ''' 只搜索出现的序列片段，假若短片段就已经不存在于序列之上，则后面的延伸序列肯定也不存在，则删除这部分的序列
        ''' </summary>
        ''' <param name="seeds"></param>
        Protected Overrides Sub __beginInit(ByRef seeds As List(Of String))
            seeds = (From Segment As String
                     In seeds
                     Where SequenceData.IndexOf(Segment) > -1
                     Select Segment).ToList
        End Sub

        ''' <summary>
        ''' 虽然当前的片段没有在序列上面出现，但是上一次迭代的片段却是出现的，假若序列片段没有匹配上，则上一次迭代的序列则可能为重复序列
        ''' </summary>
        ''' <param name="currentStat"></param>
        Protected Overrides Sub __postResult(currentRemoves() As String, currentStat As List(Of String), currLen As Integer)
            Dim ResultExport As Repeats() = LinqAPI.Exec(Of Repeats) <=
 _
                From NotAppearsSegment As String
                In currentRemoves.AsParallel
                Let Repeats As Repeats = GenerateRepeats(
                    SequenceData,
                    NotAppearsSegment,
                    MinAppeared)
                Where Not Repeats Is Nothing
                Select Repeats

            Call ResultSet.AddRange(ResultExport)
            Call CountStatics.AppendLine(New String() {currLen, currentStat.Count, ResultExport.Count})
            Call $"Length={currLen}, Chunk={currentStat.Count}, Removed={currentRemoves.Length}  .......{100 * (currLen - Min) / (Max - Min)}%".__DEBUG_ECHO
        End Sub
    End Class
End Namespace
