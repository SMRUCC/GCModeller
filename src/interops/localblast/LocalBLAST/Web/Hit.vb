Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language

Namespace NCBIBlastResult

    ''' <summary>
    ''' 在目标基因组之上的blast hit的结果
    ''' </summary>
    ''' <remarks>
    ''' Fields: query id, subject ids, % identity, alignment length, mismatches, gap opens, q. start, q. end, s. start, s. end, evalue, bit score
    ''' </remarks>
    <XmlType("hit", [Namespace]:="http://gcmodeller.org/visual/circos/blast_hit")>
    Public Class HitRecord : Inherits ClassObject

        <XmlAttribute("query_name")>
        <Column("query id")>
        Public Property QueryID As String
        ''' <summary>
        ''' 不同的编号，但是都是代表着同一个对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("hits")>
        <Column("subject ids")>
        Public Property SubjectIDs As String
        <XmlAttribute>
        <Column("% identity")>
        Public Property Identity As Double
        <XmlAttribute>
        <Column("alignment length")>
        Public Property AlignmentLength As Integer
        <XmlAttribute("mismatches")>
        <Column("mismatches")>
        Public Property MisMatches As Integer
        <XmlAttribute("gaps")>
        <Column("gap opens")>
        Public Property GapOpens As Integer
        <XmlAttribute("query.start")>
        <Column("q. start")>
        Public Property QueryStart As Integer
        <XmlAttribute("query.ends")>
        <Column("q. end")>
        Public Property QueryEnd As Integer
        <XmlAttribute("hit.start")>
        <Column("s. start")>
        Public Property SubjectStart As Integer
        <XmlAttribute("hit.ends")>
        <Column("s. end")>
        Public Property SubjectEnd As Integer
        <XmlAttribute("E-value")>
        <Column("evalue")>
        Public Property EValue As Double
        <XmlAttribute("bits")>
        <Column("bit score")>
        Public Property BitScore As Integer

        Friend DebugTag As String

        Public ReadOnly Property GI As String()
            Get
                Dim GIList As String() = Regex.Matches(Me.SubjectIDs, "gi\|\d+").ToArray(Function(s) s.Split("|"c).Last)
                Return GIList
            End Get
        End Property

        Public Overrides Function ToString() As String
            If Not String.IsNullOrEmpty(DebugTag) Then
                Return String.Format("[{0}, {1}]   ===> {2}   //{3}", QueryStart, QueryEnd, SubjectIDs, DebugTag)
            Else
                Return String.Format("[{0}, {1}]   ===> {2}", QueryStart, QueryEnd, SubjectIDs)
            End If
        End Function

        ''' <summary>
        ''' Document line parser
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Shared Function Mapper(s As String) As HitRecord
            Dim tokens As String() = Strings.Split(s, vbTab)
            Dim Hit As HitRecord = New HitRecord
            Dim p As New Pointer(Scan0)

            Hit.QueryID = tokens(++p)
            Hit.SubjectIDs = tokens(++p)
            Hit.Identity = Val(tokens(++p))
            Hit.AlignmentLength = Val(tokens(++p))
            Hit.MisMatches = Val(tokens(++p))
            Hit.GapOpens = Val(tokens(++p))
            Hit.QueryStart = Val(tokens(++p))
            Hit.QueryEnd = Val(tokens(++p))
            Hit.SubjectStart = Val(tokens(++p))
            Hit.SubjectEnd = Val(tokens(++p))
            Hit.EValue = Val(tokens(++p))
            Hit.BitScore = Val(tokens(++p))

            Return Hit
        End Function
    End Class
End Namespace