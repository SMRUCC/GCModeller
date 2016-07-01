Imports System.Text
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Namespace ProteinDomainArchitecture.MPAlignment

    ''' <summary>
    ''' CSV格式的MPAlignment结果数据记录
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MPCsvArchive : Inherits BBH.I_BlastQueryHit

        ''' <summary>
        ''' Protein sequence length.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property QueryLength As Integer
        ''' <summary>
        ''' Description for the <see cref="MPCsvArchive.QueryName"></see> protein.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description As String
        <Column("query.pfam-string")> Public Property QueryPfamString As String
        <Column("subject.pfam-string")> Public Property SubjectPfamString As String

        ''' <summary>
        ''' MPScore
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Score As Double
        Public Property FullScore As Double

        ''' <summary>
        ''' <see cref="Score"></see>/<see cref="FullScore"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Similarity As Double
            Get
                Return Score / FullScore
            End Get
        End Property

        Public Property Distance As Double
        Public Property LevScore As Double
        Public Property LevMatch As String
        Public Property LengthDelta As Double
        Public ReadOnly Property MatchDomains As Integer
            Get
                If String.IsNullOrEmpty(LevMatch) Then
                    Return 0
                End If
                Dim n As Integer = (From ch As Char In LevMatch Where ch <> "-"c Select ch).Count
                Return n
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' 结构域是否是完全匹配上的
        ''' </summary>
        ''' <returns></returns>
        Public Property StructMatched As Boolean

        Public Shared Function CreateObject(output As LevAlign) As MPCsvArchive
            Return New MPCsvArchive With {
                .HitName = output.SubjectPfam.ProteinId,
                .QueryName = output.QueryPfam.ProteinId,
                .SubjectPfamString = output.SubjectPfam.get__PfamString,
                .QueryPfamString = output.QueryPfam.get__PfamString,
                .Score = output.Score * (1 - output.LengthDelta),
                .Distance = output.Distance,
                .LevMatch = output.Matches,
                .QueryLength = output.QueryPfam.Length,
                .Description = output.QueryPfam.Description,
                .LevScore = output.Score,
                .LengthDelta = output.LengthDelta,
                .FullScore = 1,
                .StructMatched = output.StructMatched
            }
        End Function

        Public Shared Function CreateObject(output As AlignmentOutput) As MPCsvArchive
            Return New MPCsvArchive With {
                .FullScore = output.FullScore,
                .HitName = output.ProteinSbjct.ProteinId,
                .LengthDelta = output.LengthDelta,
                .Description = output.ProteinQuery.Description,
                .QueryLength = output.ProteinQuery.Length,
                .QueryName = output.ProteinQuery.ProteinId,
                .QueryPfamString = output.ProteinQuery.get__PfamString,
                .Score = output.Score,
                .SubjectPfamString = output.ProteinSbjct.get__PfamString
            }
        End Function
    End Class
End Namespace