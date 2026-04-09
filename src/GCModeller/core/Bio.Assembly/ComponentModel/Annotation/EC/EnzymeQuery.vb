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
        Public Function Query(ec_number As String, Optional fuzzy As Boolean = False) As IEnumerable(Of T)
            Dim ec As ECNumber = ECNumber.ValueParser(ec_number)

            If fuzzy Then
                Return FuzzyQuery(ec)
            Else
                ' 精确查询 - 只处理完全指定的EC编号
                If ec.subType <= 0 OrElse ec.subCategory <= 0 OrElse ec.serialNumber <= 0 Then
                    Return Enumerable.Empty(Of T)()
                End If

                Return QueryList(ec)
            End If
        End Function

        Private Iterator Function FuzzyQuery(ec As ECNumber) As IEnumerable(Of T)
            ' 遍历第一级
            For Each kv1 In ec_numbers
                If ec.type <> 0 AndAlso CInt(ec.type) <> kv1.Key Then
                    Continue For
                End If

                ' 遍历第二级
                For Each kv2 In kv1.Value
                    If ec.subType > 0 AndAlso ec.subType <> kv2.Key Then
                        Continue For
                    End If

                    ' 遍历第三级
                    For Each kv3 In kv2.Value
                        If ec.subCategory > 0 AndAlso ec.subCategory <> kv3.Key Then
                            Continue For
                        End If

                        ' 遍历第四级
                        For Each kv4 In kv3.Value
                            If ec.serialNumber > 0 AndAlso ec.serialNumber <> kv4.Key Then
                                Continue For
                            Else
                                For Each enzyme As T In kv4.Value
                                    Yield enzyme
                                Next
                            End If
                        Next
                    Next
                Next
            Next
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