Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' EC number query helper
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class EnzymeQuery(Of T As IEnzymeObject) : Implements Enumeration(Of T)

        ReadOnly ec_numbers As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, List(Of T)))))

        ''' <summary>
        ''' get total count of the enzyme object inside this query pool
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            Get
                Return Me.AsEnumerable.Count
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(pool As IEnumerable(Of T))
            If Not pool Is Nothing Then
                For Each enz As T In pool
                    If Not enz Is Nothing Then
                        Call Add(enz)
                    End If
                Next
            End If
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Add(enz As T)
            Call QueryList(ECNumber.ValueParser(enz.ECNumber)).Add(enz)
        End Sub

        Private Function QueryList(ec As ECNumber) As List(Of T)
            Dim ec_class = QueryLevel(ec_numbers, ec.type)
            Dim ec_subclass = QueryLevel(ec_class, ec.subType)
            Dim ec_category = QueryLevel(ec_subclass, ec.subCategory)

            If Not ec_category.ContainsKey(ec.serialNumber) Then
                ec_category.Add(ec.serialNumber, New List(Of T))
            End If

            Return ec_category.Item(key:=ec.serialNumber)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Query(ec_number As String) As IEnumerable(Of T)
            Return QueryList(ECNumber.ValueParser(ec_number)).AsEnumerable
        End Function

        Private Shared Function QueryLevel(Of V)(ByRef t As Dictionary(Of Integer, Dictionary(Of Integer, V)), key As Integer) As Dictionary(Of Integer, V)
            If Not t.ContainsKey(key) Then
                t.Add(key, New Dictionary(Of Integer, V))
            End If

            Return t.Item(key:=key)
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of T) Implements Enumeration(Of T).GenericEnumerator
            For Each ec_class In ec_numbers.Values
                For Each sub_class In ec_class.Values
                    For Each ec_category In sub_class.Values
                        For Each list In ec_category.Values
                            For Each enz As T In list
                                Yield enz
                            Next
                        Next
                    Next
                Next
            Next
        End Function
    End Class
End Namespace