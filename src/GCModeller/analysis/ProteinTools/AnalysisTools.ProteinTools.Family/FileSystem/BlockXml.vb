Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Namespace FileSystem

    ''' <summary>
    ''' 数据库文件
    ''' </summary>
    Public Class FamilyPfam

        <XmlAttribute> Public Property Build As String
        Public Property Guid As String
        Public Property Title As String
        Public Property Description As String
        Public Property Author As String

        <XmlElement> Public Property Family As Family()
            Get
                Return _families
            End Get
            Set(value As Family())
                _families = value
                If value.IsNullOrEmpty Then
                    _familys = New Dictionary(Of String, Family)
                Else
                    _familys = value.ToDictionary(Function(x) x.Family.ToLower)
                End If
            End Set
        End Property

        Dim _families As Family()
        ''' <summary>
        ''' 小写的键名
        ''' </summary>
        Dim _familys As Dictionary(Of String, Family)

        Public Function Classify(query As Sanger.Pfam.PfamString.PfamString,
                                 Optional threshold As Double = 0.65,
                                 Optional highlyThreshold As Double = 0.65,
                                 Optional accept As Integer = 10,
                                 Optional parallel As Boolean = True) As String()
            Dim Families As String()

            If parallel Then
                Families = (From fam As Family In Family.AsParallel
                            Let result = fam.IsThisFamily(query, threshold, highlyThreshold, accept)
                            Where result <> FileSystem.Family.MatchStates.NotMatch
                            Select fam.GetName(result)).ToArray
            Else
                Families = (From fam As Family In Family
                            Let result = fam.IsThisFamily(query, threshold, highlyThreshold, accept)
                            Where result <> FileSystem.Family.MatchStates.NotMatch
                            Select fam.GetName(result)).ToArray
            End If

            Dim likes As List(Of String) = (From fm As String
                                            In Families
                                            Where InStr(fm, "-like") > 0
                                            Select fm
                                            Distinct).ToList
            Dim isFamily As String() = (From fm As String
                                        In Families
                                        Where InStr(fm, "-like") = 0
                                        Select fm).ToArray

            For Each name As String In isFamily
                For Each [like] As String In likes.ToArray
                    If InStr([like], name) > 0 Then
                        Call likes.Remove([like])
                    End If
                Next
            Next

            Families = isFamily.Join(likes).ToArray

            Return Families
        End Function

        Public Function IsFamily(PfamString As Sanger.Pfam.PfamString.PfamString,
                                 Name As String,
                                 Optional threshold As Double = 0.65,
                                 Optional highlyThreshold As Double = 0.65,
                                 Optional accept As Integer = 10) As Family.MatchStates
            Dim Family As Family

            If _familys.ContainsKey(Name.ToLower.ShadowCopy(Name)) Then
                Family = _familys(Name)
            Else
                If _familys.ContainsKey(("*" & Name).ShadowCopy(Name)) Then
                    Family = _familys(Name)
                Else
                    Return False
                End If
            End If

            Return Family.IsThisFamily(PfamString, threshold, highlyThreshold, accept)
        End Function
    End Class
End Namespace