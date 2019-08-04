#Region "Microsoft.VisualBasic::65cd90249c20e45acc8d7ac4fed5270e, RNA-Seq\Assembler\API\Assembler.vb"

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

    ' Class Assembler
    ' 
    '     Properties: DataOverviews, ForwardsReadsBuffer, ReversedReadsBuffer, Transcripts
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __Assembler, Assembling
    ' 
    '     Sub: __Assembler
    '     Class DirectionalAssembler
    ' 
    '         Properties: readsBufferChunk, resultBuffer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Assemble, ToString
    '         Class ForwardsAssembler
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: __initReadsBufferChunk
    ' 
    '         Class ReversedAssembler
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: __initReadsBufferChunk
    ' 
    ' 
    ' 
    '     Delegate Function
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Function
    ' 
    '         Function: ForwardsDeltaPrediction, getForwordsLeft, getReversedRight, ReversedDeltaPrediction
    ' 
    '         Sub: setForwardsRight, setReversedLeft
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Terminal.Utility

''' <summary>
''' 在进行装配的时候应该尽量的在两个reads的左端错开一个距离，否则会装配完没有剩余（非线程安全）
''' </summary>
''' <remarks>
''' 两部分的reads的重叠的原则：
''' 丰度必须要接近，对于丰度不同的两个reads则可能是两个transcript的了
''' </remarks>
Public Class Assembler

    ''' <summary>
    ''' 装配得到的转录对象的在基因组上面的转录的内容
    ''' </summary>
    ''' <returns></returns>
    Public Property Transcripts As NucleotideLocation()
    Public Property DataOverviews As DocumentStream.File

    ''' <summary>
    ''' 反向的数据已经从大到小排过序了的
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ReversedReadsBuffer As GroupListNode(Of Contig, Long)()
    ''' <summary>
    ''' 正向的数据已经从小到大排过序了的，由于在装配的时候在第一个循环的过程之中会遍历所有的元素，所以有些会在装配之后还有剩余，
    ''' 这个是正常现象，不影响结果，因为在装配的函数之中由于性能优化的需要忽略掉了对遍历的元素的删除的过程
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ForwardsReadsBuffer As GroupListNode(Of Contig, Long)()
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Delta">下一个reads的左端与当前的contig的右端的最长的距离，建议为reads长度的平均值的一半</param>
    Sub New(Delta As Integer)
        _delta = Delta
        Call $"{NameOf(Delta)}:= {Delta}".__DEBUG_ECHO
    End Sub

    ReadOnly _delta As Integer

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data">假设这里都是已经处理好的数据，则在这个装配函数之中就只会根据重叠的情况来进行装配</param>
    ''' <returns></returns>
    Public Function Assembling(data As IEnumerable(Of Contig)) As NucleotideLocation()

        Dim TotalReads As Integer = data.Count

        Me.DataOverviews = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Call Me.DataOverviews.Add({"GCModeller Assembler Data Overview", Now.ToString})

        Call $"{NameOf(TotalReads)}:={TotalReads}".__DEBUG_ECHO
        Call Me.DataOverviews.Add({NameOf(TotalReads), TotalReads})
        Call "Begin invoke reversed partitioning...".__DEBUG_ECHO

        Dim sw As Stopwatch = Stopwatch.StartNew
#If Not DEBUG Then
        Dim getReversedReads As Func(Of List(Of LANS.SystemsBiology.SequenceModel.NucleotideModels.Contig)) =
            Function() (From reads In data.AsParallel
                        Where reads.MappingLocation.Strand = LANS.SystemsBiology.ComponentModel.Loci.Strands.Reverse
                        Select reads
                        Order By reads.MappingLocation.Right Descending).ToList
        Dim ayHandle As System.IAsyncResult = getReversedReads.BeginInvoke(Nothing, Nothing)
#End If
        Call "Begin invoke forwards partitioning...".__DEBUG_ECHO
        '首先按照链进行分组
        Dim Forwards = (From reads In data.AsParallel
                        Where reads.MappingLocation.Strand = LANS.SystemsBiology.ComponentModel.Loci.Strands.Forward
                        Select reads
                        Order By reads.MappingLocation.Left Ascending).ToList
        Call $"{NameOf(Forwards)}:={Forwards.Count} reads in this partition...".__DEBUG_ECHO
        Call Me.DataOverviews.Add({NameOf(Forwards), Forwards.Count})
#If Not DEBUG Then
        Call $"Waiting for the end invoke of {NameOf(getReversedReads)}...".__DEBUG_ECHO
        Dim Reversed = getReversedReads.EndInvoke(ayHandle)

        Call $"{NameOf(Reversed)}:={Reversed.Count} reads in this partition...".__DEBUG_ECHO
        Call Me.DataOverviews.Add({NameOf(Reversed), Reversed.Count})

        Call "Begin start reversed assembler...".__DEBUG_ECHO

        Dim ReversedAssembler As New Assembler.DirectionalAssembler.ReversedAssembler(Me)
        Dim assembleReversedInvoke = Function() ReversedAssembler.Assemble(Reversed)
        ayHandle = assembleReversedInvoke.BeginInvoke(Nothing, Nothing)
#End If
        Call "Begin start forwards assembler...".__DEBUG_ECHO

        Dim ForwardsAssembler As New Assembler.DirectionalAssembler.ForwardsAssembler(Me)
        Dim Result = ForwardsAssembler.Assemble(Forwards).ToList
        Call $"{NameOf(ForwardsAssembler)}:={Result.Count} contigs was assembled...".__DEBUG_ECHO
        Call Me.DataOverviews.Add({NameOf(ForwardsAssembler), Result.Count, NameOf(LANS.SystemsBiology.SequenceModel.NucleotideModels.Contig)})
#If Not DEBUG Then
        Dim reversedTemp = assembleReversedInvoke.EndInvoke(ayHandle)
        Call Result.AddRange(reversedTemp)
        Call $"{NameOf(ReversedAssembler)}:={reversedTemp.Count} contigs was assembled....".__DEBUG_ECHO
        Call Me.DataOverviews.Add({NameOf(ReversedAssembler), reversedTemp.Count, NameOf(LANS.SystemsBiology.SequenceModel.NucleotideModels.Contig)})
#End If
        Call $"[Assembler Job Done!] assembling {data.Count} reads in {sw.ElapsedMilliseconds / 1000}s...".__DEBUG_ECHO
        Call Me.DataOverviews.Add({NameOf(sw.Elapsed) & "(s)", sw.ElapsedMilliseconds / 1000})

        Me._ForwardsReadsBuffer = ForwardsAssembler.readsBufferChunk
#If Not DEBUG Then
        Me._ReversedReadsBuffer = ReversedAssembler.readsBufferChunk
#Else
        Me._ReversedReadsBuffer = New GroupListNode(Of Contig, Long)() {}
#End If

        Transcripts = Result.ToArray

        Return Transcripts
    End Function

#Region "DirectionalAssembler"

    Private MustInherit Class DirectionalAssembler

        Dim Parameter As Assembler

        Public Overrides Function ToString() As String
            Return Parameter.ToString & $" // {NameOf(setMethod)}:={setMethod.ToString}; {NameOf(getMethod)}:={getMethod.ToString}; {NameOf(relationPrediction)}:={relationPrediction.ToString}; {NameOf(DeltaPrediction)}:={DeltaPrediction.ToString }"
        End Function

        ''' <summary>
        ''' 在这个构造函数之中初始化下面的所有的只读域
        ''' </summary>
        ''' <param name="Assembler"></param>
        Sub New(Assembler As Assembler)
            Me.Parameter = Assembler
        End Sub

        ''' <summary>
        ''' 返回得到的结果
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property resultBuffer As New List(Of LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation)
        ''' <summary>
        ''' 初始的聚合结果
        ''' </summary>
        ''' <returns></returns>
        Public Property readsBufferChunk As GroupListNode(Of Contig, Long)()

        Protected getMethod As getMethod, setMethod As setMethod, relationPrediction As SegmentRelationships, DeltaPrediction As DeltaPrediction

        ''' <summary>
        ''' 对某一个指定方向的reads短序列进行装配
        ''' </summary>
        ''' <param name="dataBuffer"></param>
        ''' <returns></returns>
        Public Overridable Function Assemble(dataBuffer As List(Of Contig)) As NucleotideLocation()
            readsBufferChunk = Me.__initReadsBufferChunk(dataBuffer)

            Dim readsBuffer = readsBufferChunk.ToList
            Dim p As New EventProc(readsBuffer.Count, TAG:=Me.GetType.Name)
            Dim n As Integer = readsBuffer.Count

            Do While n > 0

                Dim currentGroup = readsBuffer(Scan0)

                Call readsBuffer.RemoveAt(Scan0)
                Call __Assembler(readsBuffer,
                                 currentGroup,
                                  Me.getMethod,
                                 Me.setMethod,
                                 Me.relationPrediction,
                               Parameter._delta,
                                 Me.DeltaPrediction,
                                 resultBuffer)
                p.Capacity = n
                n = readsBuffer.Count
                Call p.Tick()
            Loop

            Return resultBuffer.ToArray
        End Function

        Protected MustOverride Function __initReadsBufferChunk(dataBuffer As List(Of Contig)) As GroupListNode(Of Contig, Long)()

        Public Class ForwardsAssembler : Inherits DirectionalAssembler

            Sub New(Assembler As Assembler)
                Call MyBase.New(Assembler)

                Me.getMethod = AddressOf getForwordsLeft
                Me.setMethod = AddressOf setForwardsRight
                Me.relationPrediction = SegmentRelationships.DownStreamOverlap
                Me.DeltaPrediction = AddressOf ForwardsDeltaPrediction
            End Sub

            Protected Overrides Function __initReadsBufferChunk(dataBuffer As List(Of Contig)) As GroupListNode(Of Contig, Long)()

                Dim readsBufferChunk = (From obj In (From reads In dataBuffer
                                                     Select reads
                                                     Group reads By reads.MappingLocation.Left Into Group).ToArray.AsParallel
                                        Select data = New GroupListNode(Of Contig, Long) With
                             {
                                .Tag = obj.Left, .Group = obj.Group.ToList
                             }
                                        Order By data.Tag Ascending).ToArray  '分组然后得到原始的丰度
                Return readsBufferChunk
            End Function
        End Class

        Public Class ReversedAssembler : Inherits DirectionalAssembler

            Sub New(Assembler As Assembler)
                Call MyBase.New(Assembler)

                Me.getMethod = AddressOf getReversedRight
                Me.setMethod = AddressOf setReversedLeft
                Me.relationPrediction = SegmentRelationships.UpStreamOverlap
                Me.DeltaPrediction = AddressOf ReversedDeltaPrediction
            End Sub

            Protected Overrides Function __initReadsBufferChunk(dataBuffer As List(Of Contig)) As GroupListNode(Of Contig, Long)()
                Dim readsBuffer = (From obj In
                                       (From reads In dataBuffer
                                        Select reads
                                        Group reads By reads.MappingLocation.Right Into Group).ToArray.AsParallel
                                   Select data = New GroupListNode(Of Contig, Long) With
                             {
                                .Tag = obj.Right, .Group = obj.Group.ToList
                             }
                                   Order By data.Tag Descending).ToArray  '分组然后得到原始的丰度
                Return readsBuffer
            End Function
        End Class
    End Class
#End Region

    ''' <summary>
    ''' 序列装配对象的核心算法
    ''' </summary>
    ''' <param name="readsBuffer"></param>
    ''' <param name="getMethod"></param>
    ''' <param name="setMethod"></param>
    ''' <param name="relationPrediction"></param>
    Private Shared Sub __Assembler(ByRef readsBuffer As List(Of GroupListNode(Of Contig, Long)),
                                        currentGroup As GroupListNode(Of Contig, Long),
                                        getMethod As getMethod,
                                        setMethod As setMethod,
                                        relationPrediction As SegmentRelationships,
                                        Delta As Integer,
                                        DeltaPrediction As DeltaPrediction,
                                        ByRef resultBuffer As List(Of NucleotideLocation))

        For Each currentReads In currentGroup.Group '遍历当前的最左端的分组之中的所有reads
            Dim currentContig As NucleotideLocation = currentReads.MappingLocation
            currentContig = __Assembler(readsBuffer,
                                        currentContig,
                                        getMethod,
                                        setMethod,
                                        relationPrediction,
                                        Delta,
                                        DeltaPrediction,
                                        initReads:=currentGroup.InitReads)
            Call resultBuffer.Add(currentContig)
        Next

    End Sub

    ''' <summary>
    ''' 序列装配对象的核心算法
    ''' </summary>
    ''' <param name="readsBuffer"></param>
    ''' <param name="currentContig"></param>
    ''' <param name="getMethod"></param>
    ''' <param name="setMethod"></param>
    ''' <param name="relationPrediction"></param>
    ''' <param name="initReads">起点的初始reads</param>
    ''' <returns>
    ''' 对于转录起始位点的预测，则跟从文献之中的方法，当凭借完了之后，就会进行富集，假若左端的reads数多于30条则认为是TSSs
    ''' </returns>
    Private Shared Function __Assembler(ByRef readsBuffer As List(Of GroupListNode(Of Contig, Long)),
                                        currentContig As NucleotideLocation,
                                        getMethod As getMethod,
                                        setMethod As setMethod,
                                        relationPrediction As SegmentRelationships,
                                        Delta As Integer,
                                        DeltaPrediction As DeltaPrediction,
                                        initReads As Integer) As NucleotideLocation
        Dim i As Integer = 0
        Dim preReads As Integer = initReads

        Do While i <= readsBuffer.Count - 1

            Dim nextGroup = readsBuffer(i)
            Dim nextReads = nextGroup.Group(Scan0).MappingLocation  '由于reads的位置已经按照左端进行从小到大进行排序了，假若当前的这个reads不能落在当前的contig的区间之中，则后面的reads肯定不会再重叠了

            If Not currentContig.ContainSite(getMethod(nextReads)) Then
                Exit Do 'contig的延伸到此为止
            End If

            '包含有这个reads的左端，即重叠在一起，则判断是否为下游重叠
            If Not currentContig.GetRelationship(nextReads) = relationPrediction OrElse
                Not DeltaPrediction(currentContig:=currentContig, nextReads:=nextReads, Delta:=Delta) Then 'OrElse
                '  Not Math.Abs(nextGroup.InitReads - preReads) / preReads <= 0.2 Then  '两个片段的端点之间太近了，则下一个reads可能会远一点符合条件，假若reads的丰度也不相同，则也不可能是

                i += 1  '当前的不符合条件，则移动到下一个reads
                Continue Do '不是下游重叠，则当前的contig可能包含了这个reads，则这个reads可能是其他的转录本的组成，则跳过这个reads，计算下一个reads
            End If

            '这个reads和当前的contig是下游重叠的了，则延伸当前的contig然后删除这个reads
            Call nextGroup.Group.RemoveAt(Scan0)

            If nextGroup.Group.Count = 0 Then
                Call readsBuffer.RemoveAt(i)
            Else
                i += 1  '当前的分组还有剩余，则跳转到下一个分组
            End If

            preReads = nextGroup.InitReads

            '延伸contig
            Call setMethod(currentContig, nextReads)

#If DEBUG Then
            Call Console.WriteLine($"{NameOf(currentContig)}:  {currentContig.ToString} << {nextReads.ToString}")
#End If

            '由于删除了当前的i位置，后面的元素都前移了一个元素，所以这里不需要再为i增加1了

        Loop '当前的这个Reads装配完了

        Return currentContig
    End Function

    Delegate Function getMethod(loci As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation) As Long
    Delegate Sub setMethod(ByRef loci As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation, pos As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation)
    Delegate Function DeltaPrediction(currentContig As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation,
                                      nextReads As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation,
                                      Delta As Integer) As Boolean

    ''' <summary>
    ''' 当前的contig的右端和当前的下一个reads的左端的距离是否满足条件
    ''' </summary>
    ''' <param name="currentContig"></param>
    ''' <param name="nextReads"></param>
    ''' <param name="Delta"></param>
    ''' <returns></returns>
    Private Shared Function ForwardsDeltaPrediction(currentContig As NucleotideLocation, nextReads As NucleotideLocation, Delta As Integer) As Boolean
        Dim value As Integer = currentContig.Right - nextReads.Left
        Return value <= Delta AndAlso value > 0
    End Function

    ''' <summary>
    ''' 当前contig的左端和下一个reads的右端的距离是否满足条件
    ''' </summary>
    ''' <param name="currentContig"></param>
    ''' <param name="nextReads"></param>
    ''' <param name="Delta"></param>
    ''' <returns></returns>
    Private Shared Function ReversedDeltaPrediction(currentContig As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation,
                                      nextReads As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation,
                                      Delta As Integer) As Boolean
        Dim value As Integer = nextReads.Right - currentContig.Left
        Return value <= Delta AndAlso value > 0
    End Function

    Private Shared Sub setForwardsRight(ByRef loci As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation,
                                        right As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation)
        loci.Right = right.Right
    End Sub

    Private Shared Sub setReversedLeft(ByRef loci As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation,
                                       left As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation)
        loci.Left = left.Left
    End Sub

    Private Shared Function getForwordsLeft(loci As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation) As Long
        Return loci.Left
    End Function

    Private Shared Function getReversedRight(loci As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation) As Long
        Return loci.Right
    End Function
End Class
