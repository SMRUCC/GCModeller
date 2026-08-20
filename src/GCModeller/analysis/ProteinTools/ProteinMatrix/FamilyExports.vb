Public Class FamilyExports

    Public Property family_id As String
    Public Property members As Integer
    ''' <summary>
    ''' representative sequence title
    ''' </summary>
    ''' <returns></returns>
    Public Property representative As String
    ''' <summary>
    ''' representative sequence
    ''' </summary>
    ''' <returns></returns>
    Public Property rep_seq As String

End Class

Public Class SequenceCluster

    Public Property seq_title As String
    Public Property family_id As String
    Public Property score As Double
    Public Property seq As String

End Class