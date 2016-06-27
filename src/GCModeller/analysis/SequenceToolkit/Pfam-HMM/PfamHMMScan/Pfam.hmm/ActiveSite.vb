Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' active_site.dat
''' </summary>
Public Structure ActiveSite : Implements sIdEnumerable

    Public Property ID As String Implements sIdEnumerable.Identifier
    Public Property RE As Dictionary(Of String, RE)
    Public Property AL As Alignment()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Iterator Function LoadStream(path As String) As IEnumerable(Of ActiveSite)
        Dim lines As String() = path.ReadAllLines
        Dim tokens As IEnumerable(Of String()) = lines.Split("//")

        For Each token As String() In tokens
            Yield StreamParser(token)
        Next
    End Function

    Public Shared Function StreamParser(stream As String()) As ActiveSite
        Dim LQuery = (From s As String
                      In stream
                      Let tag As String = s.Substring(0, 2)
                      Let value As String = s.Substring(4)
                      Select tag,
                          value
                      Group By tag Into Group) _
                              .ToDictionary(Function(x) x.tag,
                                            Function(x) x.Group.ToArray(Function(o) o.value))
        Dim pfam As New ActiveSite With {
            .ID = LQuery.TryGetValue(NameOf(pfam.ID)).DefaultFirst,
            .RE = __RLhash(LQuery.TryGetValue(NameOf(pfam.RE))),
            .AL = (From s As String
                   In LQuery.TryGetValue(NameOf(pfam.AL)).AsParallel
                   Select New Alignment(s)).ToArray
        }

        Return pfam
    End Function

    Private Shared Function __RLhash(value As String()) As Dictionary(Of String, RE)
        Dim LQuery = (From x As String In value
                      Let tokens As String() = Strings.Split(x, "  ")
                      Let id As String = tokens(Scan0)
                      Let int As Integer = Scripting.CTypeDynamic(Of Integer)(tokens(1))
                      Select id,
                          int
                      Group By id Into Group) _
                              .ToDictionary(Function(x) x.id,
                                            Function(x) New RE With {
                                                .ID = x.id,
                                                .Value = x.Group.ToArray(Function(t) t.int)})
        Return LQuery
    End Function
End Structure

Public Structure RE : Implements sIdEnumerable
    Public Property ID As String Implements sIdEnumerable.Identifier
    Public Property Value As Integer()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure

Public Structure Alignment : Implements sIdEnumerable
    Public Property ID As String Implements sIdEnumerable.Identifier
    Public Property MAL As String

    Sub New(s As String)
        Dim tokens As String() = Regex.Split(s, "\s+")
        ID = tokens(Scan0)
        MAL = tokens(1)
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure