#Region "Microsoft.VisualBasic::ac0a62baa1756b2fb944b819c95c1d88, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\SearchWorker.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace Topologically

    Public MustInherit Class SearchWorker

        Public ReadOnly Property ResidueBase As Char()
        Public ReadOnly Property SequenceData As String

        Dim _segmentLocis As List(Of String)

        Public ReadOnly Property Min As Integer
            Get
                Return _min
            End Get
        End Property

        Public ReadOnly Property Max As Integer
            Get
                Return _max
            End Get
        End Property

        Protected _min, _max As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(Sequence As I_PolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer)

            SequenceData = Sequence.SequenceData.ToUpper
            ResidueBase =
                LinqAPI.Exec(Of Char) <= From ch As Char
                                         In SequenceData
                                         Where ch <> "-"c AndAlso ch <> "*"c ' 这些缺口的符号是需要被过滤掉的
                                         Select ch
                                         Distinct ' 获取所有的残基的符号
            _segmentLocis = Seeds.InitializeSeeds(ResidueBase, Min)
            _min = Min
            _max = Max

            If Min > Max Then
                Min = Max
            End If
            If Max > Len(Sequence.SequenceData) Then
                Max = Len(Sequence.SequenceData)
            End If

            Call __beginInit(_segmentLocis)
        End Sub

        Public Sub InvokeSearch()
            For i As Integer = Min To Max
                Dim removes As String() = __getRemovedList(_segmentLocis)
                _segmentLocis = (From Segment As String In _segmentLocis.AsParallel
                                 Where Array.IndexOf(removes, Segment) = -1
                                 Select Segment).ToList ' 将没有出现的序列进行删除，或者重复的次数较少的片段
                Call __postResult(removes, _segmentLocis, currLen:=i)
                _segmentLocis = Topologically.Seeds.ExtendSequence(_segmentLocis, ResidueBase)

                Call $"{i} ({_segmentLocis.Count})  ===> {Mid((100 * (i - Min) / (Max - Min)), 1, 7)}%".__DEBUG_ECHO
            Next
        End Sub

        ''' <summary>
        ''' 必须在还较短的时候讲不符合条件的片段删除，否则后面的序列片段会非常多
        ''' </summary>
        ''' <param name="currentStat"></param>
        ''' <returns></returns>
        Protected MustOverride Function __getRemovedList(ByRef currentStat As List(Of String)) As String()
        Protected MustOverride Sub __postResult(currentRemoves As String(), currentStat As List(Of String), currLen As Integer)
        Protected MustOverride Sub __beginInit(ByRef seeds As List(Of String))

    End Class
End Namespace
