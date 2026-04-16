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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ec_number"></param>
        ''' <param name="fuzzy"></param>
        ''' <param name="fuzzyLevel"> 
        ''' max allow fuzzy level, value should be c(1,2,3,4), which means the class levels of the ec_number(which has 4 ranks).
        ''' 
        ''' example as 4, means only allows fuzzy matches on the last digit
        ''' 3, means allows fuzzy matches on the level 3 and level 4 digits</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Query(ec_number As String, Optional fuzzy As Boolean = False, Optional fuzzyLevel As Integer = 4) As IEnumerable(Of T)
            Dim ec As ECNumber = ECNumber.ValueParser(ec_number)

            If fuzzy Then
                Return FuzzyQuery(ec, fuzzyLevel)
            Else
                ' 精确查询 - 只处理完全指定的EC编号
                If ec.subType <= 0 OrElse ec.subCategory <= 0 OrElse ec.serialNumber <= 0 Then
                    Return Enumerable.Empty(Of T)()
                End If

                Return QueryList(ec)
            End If
        End Function

        Private Iterator Function FuzzyQuery(ec As ECNumber, fuzzyLevel As Integer) As IEnumerable(Of T)
            Dim ec_class = QueryFuzzy(ec_numbers, ec.type, allowFuzzy:=1 >= fuzzyLevel)
            Dim ec_subclass = ec_class.Select(Function(level) QueryFuzzy(level, ec.subType, 2 >= fuzzyLevel)).IteratesALL
            Dim ec_category = ec_subclass.Select(Function(level) QueryFuzzy(level, ec.subCategory, 3 >= fuzzyLevel)).IteratesALL

            For Each enz_list In ec_category
                If ec.serialNumber <= 0 AndAlso fuzzyLevel >= 1 Then
                    For Each list In enz_list.Values
                        For Each enz As T In list
                            Yield enz
                        Next
                    Next
                Else
                    ' 1.1.1.1 matches 1.1.1.1 and 1.1.1.- in fuzzy mode
                    ' 1.1.1.- matches 1.1.1.1, 1.1.1.2 and 1.1.1.- in fuzzy mode

                    If enz_list.ContainsKey(ec.serialNumber) Then
                        For Each enz As T In enz_list.Item(key:=ec.serialNumber)
                            Yield enz
                        Next
                    End If
                    If enz_list.ContainsKey(0) AndAlso fuzzyLevel >= 1 Then
                        For Each enz As T In enz_list.Item(key:=0)
                            Yield enz
                        Next
                    End If
                End If
            Next
        End Function

        Private Shared Iterator Function QueryFuzzy(Of V)(t As Dictionary(Of Integer, Dictionary(Of Integer, V)), key As Integer, allowFuzzy As Boolean) As IEnumerable(Of Dictionary(Of Integer, V))
            If key <= 0 AndAlso allowFuzzy Then
                ' symbol - match all in fuzzy mode
                For Each __ As Dictionary(Of Integer, V) In t.Values
                    Yield __
                Next
            ElseIf t.ContainsKey(key) Then
                ' number for exact match
                Yield t.Item(key:=key)
            End If

            ' 1.1.1.1 matches 1.1.1.1 and 1.1.1.- in fuzzy mode
            ' 1.1.1.- matches 1.1.1.1, 1.1.1.2 and 1.1.1.- in fuzzy mode

            If key > 0 Then
                ' key <= 0 already populate all items
                If allowFuzzy AndAlso t.ContainsKey(0) Then
                    Yield t.Item(key:=0)
                End If
            End If
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