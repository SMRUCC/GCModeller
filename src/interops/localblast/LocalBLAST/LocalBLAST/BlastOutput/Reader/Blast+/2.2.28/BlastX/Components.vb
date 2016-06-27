Imports LANS.SystemsBiology.ComponentModel.Loci

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX.Components

    Public Class Query

        Dim InternalHitsVector As HitFragment()

        Public Property QueryName As String
        Public Property QueryLength As Integer
        Public Property SubjectName As String
        Public Property SubjectLength As Integer
        Public Property Hits As HitFragment()
            Get
                Return InternalHitsVector
            End Get
            Set(value As HitFragment())
                InternalHitsVector = (From item In value Select item.SetStack(Me)).ToArray
            End Set
        End Property

        Public Overrides Function ToString() As String
            If Hits.IsNullOrEmpty Then
                Return String.Format("{0}  <===>  {1}  (HITS_NOT_FOUND)", QueryName, SubjectName)
            Else
                Return String.Format("{0}  <===>  {1}", QueryName, SubjectName)
            End If
        End Function

        ''' <summary>
        ''' 这个函数过滤掉所有过短的比对片段
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="p">0-1之间的数，用于表示长度的百分比</param>
        ''' <remarks></remarks>
        Public Function FilteringSegments(p As Double) As HitFragment()
            Dim LQuery = (From hsp In Hits Where hsp.SubjectLength / SubjectLength >= p Select hsp).ToArray
            Return LQuery
        End Function
    End Class

    Public Class HitFragment

        Dim QueryStack As Query

        Public Property Score As ComponentModel.BlastXScore
        Public Property Hsp As ComponentModel.HitSegment()
        Public Property HitName As String
        Public Property HitLen As Integer

        Public ReadOnly Property SubjectLength As Integer
            Get
                Return Math.Abs(Hsp.Last.Sbjct.Right - Hsp.First.Sbjct.Left)
            End Get
        End Property

        Public ReadOnly Property Coverage As Double
            Get
                Return SubjectLength / QueryStack.SubjectLength
            End Get
        End Property

        ''' <summary>
        ''' ORF阅读框的偏移碱基数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ReadingFrameOffSet As Integer
            Get
                Return Score.Frame
            End Get
        End Property

        Public ReadOnly Property QueryLoci As Location
            Get
                Return New Location(Hsp.First.Query.Left, Hsp.Last.Query.Right)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("({0}{1})  {2};    {3}%", If(ReadingFrameOffSet > 0, "+", ""), ReadingFrameOffSet, QueryLoci.ToString, Coverage)
        End Function

        Friend Function SetStack(Query As Query) As HitFragment
            QueryStack = Query
            Return Me
        End Function
    End Class
End Namespace