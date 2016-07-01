Imports SMRUCC.genomics.AnalysisTools.SequenceTools.SmithWaterman
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Analysis.Similarity.TOMQuery

    ''' <summary>
    ''' <see cref="Output"/>的Csv文件输出
    ''' </summary>
    Public Class MotifHit : Implements IQueryHits

        Public Property Query As String Implements IBlastHit.locusId
        Public Property Subject As String Implements IBlastHit.Address
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
        Public Property DP As Streams.Array.Double()
        ''' <summary>
        ''' The directions pointing to the cells that
        ''' give the maximum score at the current cell.
        ''' The first index is the column index.
        ''' The second index is the row index.
        ''' </summary>
        ''' <returns></returns>
        Public Property Directions As Streams.Array.Integer()

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
                Dim array = (From x In HSP Select New Coords(x.FromS, x.ToS)).ToArray
                Dim s As Double = Length(array) / Subject.PWM.Length
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
                Dim ls As Double = HSP.ToArray(Function(x) x.Alignment.MatchSimilarity).Average
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