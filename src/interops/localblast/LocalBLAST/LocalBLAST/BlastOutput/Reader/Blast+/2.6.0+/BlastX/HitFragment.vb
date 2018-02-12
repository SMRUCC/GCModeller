#Region "Microsoft.VisualBasic::e5358c8da8d4ce5d7a321607ffa1c631, localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.6.0+\BlastX\HitFragment.vb"

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

    '     Class HitFragment
    ' 
    '         Properties: Coverage, HitLen, HitName, Hsp, QueryLoci
    '                     ReadingFrameOffSet, Score, SubjectLength
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX.Components

    Public Class HitFragment

        ''' <summary>
        ''' The score of this fragment
        ''' </summary>
        ''' <returns></returns>
        Public Property Score As ComponentModel.BlastXScore

        ''' <summary>
        ''' The alignment high score region.
        ''' </summary>
        ''' <returns></returns>
        Public Property Hsp As ComponentModel.HitSegment()
        Public Property HitName As String
        Public Property HitLen As Integer

        Public ReadOnly Property SubjectLength As Integer
            Get
                Return Math.Abs(Hsp.Last.Subject.Right - Hsp.First.Subject.Left)
            End Get
        End Property

        Public ReadOnly Property Coverage(subject As Subject) As Double
            Get
                Return SubjectLength / subject.SubjectLength
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
            Return String.Format("({0}{1})  {2};", If(ReadingFrameOffSet > 0, "+", ""), ReadingFrameOffSet, QueryLoci.ToString)
        End Function
    End Class
End Namespace
