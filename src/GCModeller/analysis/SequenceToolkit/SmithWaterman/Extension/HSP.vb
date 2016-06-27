Imports System.Linq
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting

Public Class HSP : Inherits Match

    Public Property Query As String
    Public Property Subject As String
    <XmlAttribute> Public Property QueryLength As Integer
    <XmlAttribute> Public Property SubjectLength As Integer

    ''' <summary>
    ''' length of the query fragment size
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LengthQuery As Integer
        Get
            Return Math.Abs(ToA - FromA)
        End Get
    End Property

    ''' <summary>
    ''' length of the hit fragment size
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LengthHit As Integer
        Get
            Return Math.Abs(ToB - FromB)
        End Get
    End Property

    Public Shared Function CreateObject(match As Match, query As String, subject As String) As HSP
        Dim queryp = Mid(query, match.FromA, match.ToA - match.FromA)
        Dim subjectp = Mid(subject, match.FromB, match.ToB - match.FromB)

        Return New HSP With {
            .FromA = match.FromA,
            .FromB = match.FromB,
            .ToA = match.ToA,
            .ToB = match.ToB,
            .QueryLength = query.Length,
            .SubjectLength = subject.Length,
            .Score = match.Score,
            .Query = queryp,
            .Subject = subjectp
        }
    End Function

    Public Shared Function CreateHSP(Of T)(sw As GSW(Of T), asChar As ToChar(Of T), ByRef best As HSP, cutoff As Double) As HSP()
        Dim query As String = New String(sw.query.ToArray(Function(x) asChar(x)))
        Dim subject As String = New String(sw.subject.ToArray(Function(x) asChar(x)))
        Dim matches = sw.Matches(cutoff).ToList
        Dim hsp = matches.ToArray(Function(x) CreateObject(x, query, subject))

        Try
            Dim lstb = SimpleChaining.chaining(hsp.ToArray(Function(x) x.As(Of Match)).ToList, False)
            lstb = (From x In lstb Select x Order By x.Score Descending).ToList
            If Not lstb.IsNullOrEmpty Then
                best = CreateObject(lstb.FirstOrDefault, query, subject)
            End If
        Catch ex As Exception

        End Try

        Return hsp
    End Function

End Class
