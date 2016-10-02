Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX.Components

    Public Class HitFragment

        Dim _queryStack As Query

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
                Return SubjectLength / _queryStack.SubjectLength
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
            _queryStack = Query
            Return Me
        End Function
    End Class
End Namespace