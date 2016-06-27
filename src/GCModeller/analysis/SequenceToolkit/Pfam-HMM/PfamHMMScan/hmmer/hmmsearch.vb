Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' # hmmsearch :: search profile(s) against a sequence database
''' </summary>
Public Class hmmsearch

    Public Const Pfam As String = NameOf(Pfam)

    ''' <summary>
    ''' # hmmsearch :: search profile(s) against a sequence database
    ''' # HMMER 3.1b1 (May 2013); http://hmmer.org/
    ''' # Copyright (C) 2013 Howard Hughes Medical Institute.
    ''' # Freely distributed under the GNU General Public License (GPLv3).
    ''' </summary>
    ''' <returns></returns>
    Public Property version As String
    ''' <summary>
    ''' query HMM file
    ''' </summary>
    ''' <returns></returns>
    Public Property HMM As String
    ''' <summary>
    ''' target sequence database
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String
    Public Property Queries As PfamQuery()

    Public Function GetProfiles() As Dictionary(Of String, AlignmentHit())
        Dim LQuery As IEnumerable(Of AlignmentHit) =
            LinqAPI.MakeList(Of AlignmentHit) <= From x As PfamQuery
                                                 In Queries.AsParallel
                                                 Select x.alignments
        Dim Groups = From x As AlignmentHit
                     In LQuery
                     Let tag As String = x.locus.Split.First
                     Select o = x,
                         tag
                     Group By tag Into Group
        Dim hash As Dictionary(Of String, AlignmentHit()) =
            Groups.ToDictionary(Function(x) x.tag,
                                Function(x) x.Group.ToArray(Function(o) o.o))
        Return hash
    End Function

    Public Overrides Function ToString() As String
        Return New With {version, HMM, source}.GetJson
    End Function
End Class

Public Class PfamQuery
    Public Property Query As String
    Public Property MLen As Integer
    Public Property Accession As String
    Public Property describ As String
    Public Property hits As Score()
    Public Property uncertain As Score()
    Public Property alignments As AlignmentHit()

    Public Overrides Function ToString() As String
        Return Query
    End Function
End Class

Public Class Score
    Public Property Full As hmmscan.Score
    Public Property Best As hmmscan.Score
    Public Property exp As Double
    Public Property N As Integer
    Public Property locus As String
    Public Property describ As String

    Public Overrides Function ToString() As String
        Return locus
    End Function
End Class

Public Class AlignmentHit : Implements IMatched

    Public Property locus As String
    Public Property hits As hmmscan.Align()
    ''' <summary>
    ''' Tag data for the Pfam HMM model
    ''' </summary>
    ''' <returns></returns>
    Public Property QueryTag As String

    Public ReadOnly Property IsMatched As Boolean Implements IMatched.IsMatched
        Get
            Return Not (From x As hmmscan.Align
                        In hits.SafeQuery
                        Where DirectCast(x, IMatched).IsMatched
                        Select x).FirstOrDefault Is Nothing
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return locus
    End Function
End Class