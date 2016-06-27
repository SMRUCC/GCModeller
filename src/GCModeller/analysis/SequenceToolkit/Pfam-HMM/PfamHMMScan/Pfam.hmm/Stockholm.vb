Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Pfam-A.hmm.dat
''' </summary>
Public Class Stockholm : Implements sIdEnumerable

    ''' <summary>
    ''' Identifier
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String
    ''' <summary>
    ''' Pfam accession ID
    ''' </summary>
    ''' <returns></returns>
    Public Property AC As String Implements sIdEnumerable.Identifier
    ''' <summary>
    ''' Definition
    ''' </summary>
    ''' <returns></returns>
    Public Property DE As String
    Public Property GA As Double()
    ''' <summary>
    ''' Type
    ''' </summary>
    ''' <returns></returns>
    Public Property TP As String
    Public Property ML As Integer
    Public Property CL As String
    Public Property NE As String()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Iterator Function DocParser(path As String) As IEnumerable(Of Stockholm)
        Dim lines As String() = path.ReadAllLines
        Dim tokens As IEnumerable(Of String()) = lines.Split("//")

        VBDebugger.Mute = True

        For Each token As String() In tokens
            Dim hash As Dictionary(Of String, String()) = __hash(token.Skip(1))
            Dim x As New Stockholm

            x.AC = hash.TryGetValue(NameOf(x.AC)).DefaultFirst
            x.CL = hash.TryGetValue(NameOf(x.CL)).DefaultFirst
            x.DE = hash.TryGetValue(NameOf(x.DE)).DefaultFirst

            Dim tmp As String = hash.TryGetValue(NameOf(x.GA)).DefaultFirst
            If Not String.IsNullOrEmpty(tmp) Then
                x.GA = Strings.Split(tmp, ";").ToArray(Function(s) Val(s), where:=Function(s) Not s.IsBlank)
            End If

            x.ID = hash.TryGetValue(NameOf(x.ID)).DefaultFirst
            x.ML = Scripting.CTypeDynamic(Of Integer)(hash.TryGetValue(NameOf(x.ML)).DefaultFirst)
            x.NE = hash.TryGetValue(NameOf(x.NE))
            x.TP = hash.TryGetValue(NameOf(x.TP)).DefaultFirst

            Yield x
        Next

        VBDebugger.Mute = False
    End Function

    Private Shared Function __hash(token As IEnumerable(Of String)) As Dictionary(Of String, String())
        Dim LQuery = (From s As String
                      In token
                      Let ss As String = s.Replace("#=GF ", "")
                      Let ts As String() = Strings.Split(ss, "   ")
                      Select ts.First, ts.Last
                      Group By First Into Group)

        Dim hash As Dictionary(Of String, String()) =
            LQuery.ToDictionary(Function(x) x.First, Function(x) x.Group.ToArray(Function(ts) ts.Last))
        Return hash
    End Function
End Class
