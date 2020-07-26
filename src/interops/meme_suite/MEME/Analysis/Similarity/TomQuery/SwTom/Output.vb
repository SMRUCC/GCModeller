#Region "Microsoft.VisualBasic::feef97402fd9ed7442491e377ef4ffe6, meme_suite\MEME\Analysis\Similarity\TomQuery\SwTom\Output.vb"

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

    '     Class MotifHit
    ' 
    '         Properties: Coverage, Hsp, Lev, Query, QueryMotif
    '                     Similarity, Subject, SubjectMotif
    ' 
    '         Function: CreateObject, ToString
    ' 
    '     Class Output
    ' 
    '         Properties: Coverage, Directions, DP, HSP, lev
    '                     Match, Parameters, Query, QueryMotif, Similarity
    '                     Subject, SubjectMotif
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis
Imports SMRUCC.genomics.Analysis.SequenceTools.SmithWaterman
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace Analysis.Similarity.TOMQuery

    ''' <summary>
    ''' <see cref="Output"/>的Csv文件输出
    ''' </summary>
    Public Class MotifHit : Implements IQueryHits

        Public Property Query As String Implements IBlastHit.queryName
        Public Property Subject As String Implements IBlastHit.hitName
        ''' <summary>
        ''' 高分区的数目
        ''' </summary>
        ''' <returns></returns>
        Public Property Hsp As Integer
        Public Property QueryMotif As String
        Public Property SubjectMotif As String
        Public Property Coverage As Double
        Public Property Lev As Double
        Public Property Similarity As Double Implements IQueryHits.identities

        Public Overrides Function ToString() As String
            Return $"{Query} --> {Subject}"
        End Function

        Public Shared Function CreateObject(x As Output) As MotifHit
            Return New MotifHit With {
                .Coverage = x.Coverage,
                .Hsp = x.HSP.Length,
                .Lev = x.lev,
                .Query = x.Query.Uid,
                .QueryMotif = x.QueryMotif,
                .Similarity = x.Similarity,
                .Subject = x.Subject.Uid,
                .SubjectMotif = x.SubjectMotif
            }
        End Function
    End Class

    Public Class Output
        Public Property Query As MotifScans.AnnotationModel
        Public Property Subject As MotifScans.AnnotationModel
        Public Property Parameters As Parameters
        Public Property HSP As SW_HSP()
        ''' <summary>
        ''' Dynmaic programming matrix.(也可以看作为得分矩阵)
        ''' </summary>
        ''' <returns></returns>
        Public Property DP As ArrayRow()
        ''' <summary>
        ''' The directions pointing to the cells that
        ''' give the maximum score at the current cell.
        ''' The first index is the column index.
        ''' The second index is the row index.
        ''' </summary>
        ''' <returns></returns>
        Public Property Directions As ArrayRow()

        Public Property QueryMotif As String
        Public Property SubjectMotif As String

        Public ReadOnly Property Match As Boolean
            Get
                If HSP.IsNullOrEmpty Then
                    Return False
                End If
                Dim LQuery = (From x As TomOUT In HSP
                              Where Not x.Alignment Is Nothing AndAlso
                                  x.Alignment.MatchSimilarity >= Parameters.TOMThreshold
                              Select 100).FirstOrDefault
                Return LQuery > 50
            End Get
        End Property

        ''' <summary>
        ''' 由于大部分情况下是对数据库的查询，所以coverage是以subject为主的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Coverage As Double
            Get
                Dim array = (From x In HSP Select New Coordinate(x.FromS, x.ToS)).ToArray
                Dim s As Double = SequenceTools.Coverage.Length(array) / Subject.PWM.Length
                Return s
            End Get
        End Property

        ''' <summary>
        ''' lev%
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property lev As Double
            Get
                If HSP.IsNullOrEmpty Then
                    Return 0
                End If
                Dim ls As Double = HSP.Select(Function(x) x.Alignment.MatchSimilarity).Average
                Return ls
            End Get
        End Property

        Public ReadOnly Property Similarity As Double
            Get
                Dim s As Double = lev * Coverage
                Return s
            End Get
        End Property
    End Class
End Namespace
