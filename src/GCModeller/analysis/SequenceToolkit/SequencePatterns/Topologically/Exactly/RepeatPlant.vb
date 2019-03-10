﻿#Region "Microsoft.VisualBasic::32ce267f98a7f0d7efa665005f4720bf, analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\RepeatPlant.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class RepeatsLoci
    ' 
    '         Properties: LociLeft, RepeatLoci
    ' 
    '         Function: __hash
    ' 
    '     Class RevRepeatsLoci
    ' 
    '         Properties: RevLociLeft, RevRepeats
    ' 
    '         Function: __hash
    ' 
    '     Class RepeatsView
    ' 
    '         Properties: Data, Hot, IntervalAverages, Left, Length
    '                     Locis, RepeatsNumber, SequenceData
    ' 
    '         Function: LociProvider, ToLoci, ToString, ToVector, TrimView
    ' 
    '     Class RevRepeatsView
    ' 
    '         Properties: Hot, IntervalAverages, RepeatsNumber, RevLocis, RevSegment
    ' 
    '         Function: LociProvider, TrimView
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically

    Public Class RepeatsLoci : Implements ILoci

        Public Property RepeatLoci As String
        <Column("Loci.Left")> Public Property LociLeft As Integer Implements ILoci.Left

        Friend Overridable Function __hash() As String
            Return Me.RepeatLoci & CStr(LociLeft)
        End Function
    End Class

    Public Class RevRepeatsLoci : Inherits RepeatsLoci
        <Column("rev-Repeats")> Public Property RevRepeats As String
        <Column("rev-Loci.Left")> Public Property RevLociLeft As Integer

        Friend Overrides Function __hash() As String
            Return MyBase.__hash() & Me.RevRepeats & CStr(Me.RevLociLeft)
        End Function
    End Class

    ''' <summary>
    ''' 正向重复位点的序列模型
    ''' </summary>
    Public Class RepeatsView : Implements ILoci
        Implements IPolymerSequenceModel

        Public Property Left As Integer Implements ILoci.Left
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
        Public Property Locis As Integer()
        Public ReadOnly Property Length As Integer
            Get
                Return Len(SequenceData)
            End Get
        End Property

        Public Property Data As Dictionary(Of String, String)

        ''' <summary>
        ''' 每个重复的片段之间平均的间隔长度
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property IntervalAverages As Double
            Get
                Dim orders = Locis.OrderBy(Function(x) x)
                Dim pre = Left
                Dim interval As New List(Of Integer)

                For Each x As Integer In orders
                    interval += (x - pre)
                    pre = x
                Next

                Return interval.Average
            End Get
        End Property

        Public Overridable Function LociProvider() As Integer()
            Return Locis
        End Function

        Public Shared Function TrimView(data As Generic.IEnumerable(Of RepeatsLoci)) As RepeatsView()
            Dim LQuery = From loci As RepeatsLoci
                         In data
                         Select loci
                         Group loci By loci.RepeatLoci Into Group
            Dim views = LinqAPI.Exec(Of RepeatsView) <=
                From loci
                In LQuery
                Let pos As Integer() = loci.Group _
                    .Select(Function(site) CInt(site.LociLeft)) _
                    .ToArray
                Select New RepeatsView With {
                    .SequenceData = loci.RepeatLoci,
                    .Left = pos.Min,
                    .Locis = pos
                }
            Return views
        End Function

        Public Overrides Function ToString() As String
            Return $"{SequenceData}  @{Left}, {Hot}"
        End Function

        ''' <summary>
        ''' 返回的是基因组上面的每一个位点的热度的列表
        ''' </summary>
        ''' <typeparam name="TView"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="size"></param>
        ''' <returns></returns>
        Public Shared Function ToVector(Of TView As RepeatsView)(data As IEnumerable(Of TView), size As Long) As Double()
            Dim Extract = (From obj As TView
                           In data
                           Where Not obj Is Nothing
                           Select (From site As Integer
                                   In obj.LociProvider
                                   Select site,
                                       obj.Hot).ToArray).Unlist
            Dim src = (From t In (From site In Extract
                                  Select site
                                  Group site By site.site Into Group)
                       Select t.site,
                           Group = t.Group.ToArray
                       Order By site Ascending).ToArray
            Dim LQuery As Dictionary(Of Integer, Double) =
                src.ToDictionary(
                Function(site) site.site,
                Function(site) site.Group.Select(Function(loci) loci.Hot).Sum)
            Dim vector As Double() = size.Sequence.Select(Function(idx) LQuery.TryGetValue(idx, [default]:=0))
            Return vector
        End Function

        ''' <summary>
        ''' 平均距离越小，则热度越高
        ''' 位点越多，热度越高
        ''' 片段越长，热度越高
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property Hot As Double
            Get
                Dim ordLocis = (From n As Integer
                                In Me.LociProvider
                                Select n
                                Order By n Ascending).CreateSlideWindows(2)
                Dim avgDist As Double = ordLocis.Select(
                    Function(loci) _
                        If(loci.Items.IsNullOrEmpty OrElse
                        loci.Items.Length = 1, 1,
                        loci.Items.Last - loci.Items.First)).Average
                avgDist = Len(SequenceData) / avgDist  ' 表达式的含义： 片段越长，热度越高，  平均距离越短，热度越高
                Dim lociCounts As Double = LociProvider.Length
                lociCounts = lociCounts / 10
                Return avgDist + lociCounts
            End Get
        End Property

        Public Overridable ReadOnly Property RepeatsNumber As Integer
            Get
                Return Me.LociProvider.Length
            End Get
        End Property

        Public Overridable Function ToLoci() As SimpleSegment
            Dim id$ = ""

            If Not Data Is Nothing Then
                If Data.ContainsKey("seq") Then
                    id = Data("seq") & "-"
                End If
            End If

            id &= $"{Left},{Left + Length}"

            Return New SimpleSegment With {
                .SequenceData = SequenceData,
                .ID = id,
                .Start = Left,
                .Ends = Left + Length,
                .Strand = "+"
            }
        End Function
    End Class

    ''' <summary>
    ''' 反向重复序列的模型，继承于<see cref="RepeatsView"/>模型
    ''' </summary>
    Public Class RevRepeatsView : Inherits RepeatsView
        Implements ILoci
        Implements IPolymerSequenceModel

        Public Property RevLocis As Integer()
        Public Property RevSegment As String

        Public Overrides Function LociProvider() As Integer()
            Return RevLocis
        End Function

        Public Overloads Shared Function TrimView(data As IEnumerable(Of RevRepeats)) As RevRepeatsView()
            Dim LQuery As RevRepeatsView() = data.Select(
                Function(loci) _
                    New RevRepeatsView With {
                        .Left = loci.Locations.Min,
                        .Locis = loci.RepeatLoci,
                        .RevLocis = loci.Locations,
                        .RevSegment = loci.RevSegment,
                        .SequenceData = loci.SequenceData
                    })
            Return LQuery
        End Function

        Public Overrides ReadOnly Property RepeatsNumber As Integer
            Get
                Return RevLocis.Length
            End Get
        End Property

        Public Overrides ReadOnly Property Hot As Double
            Get
                Dim loci = (From n As Integer
                            In RevLocis
                            Select n
                            Order By n Ascending).CreateSlideWindows(2)
                Dim avgDist As Double = loci.Select(
                    Function(lo) _
                        If(lo.Items.IsNullOrEmpty OrElse
                        lo.Items.Length = 1,
                        1, lo.Items.Last - lo.Items.First)).Average
                Return MyBase.Hot + Len(RevSegment) / avgDist
            End Get
        End Property

        Public Overrides ReadOnly Property IntervalAverages As Double
            Get
                Dim orders = RevLocis.OrderBy(Function(x) x).ToArray
                Dim pre As Integer = orders.First
                Dim interval As New List(Of Integer)

                For Each x As Integer In orders.Skip(1)
                    interval += (x - pre)
                    pre = x
                Next

                Return interval.Average
            End Get
        End Property
    End Class
End Namespace
