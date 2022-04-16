Imports System.Runtime.CompilerServices

Namespace Data.Repository

    Public Module FNV1a

        ReadOnly hashHandler As New Dictionary(Of Type, Func(Of Object, Integer))

        Sub New()
            hashHandler(GetType(String)) = AddressOf stringCode
        End Sub

        Private Function stringCode(str As String) As Integer

        End Function

        <Extension()>
        Public Function GetHashCode(Of T)(x As T, getFields As Func(Of T, IEnumerable(Of Object))) As Integer
            Return GetHashCode(getFields(x))
        End Function

        Public Function GetHashCode(getValues As Func(Of IEnumerable(Of Object))) As Integer
            Return GetHashCode(getValues())
        End Function

        Public Sub RegisterHashFunction(target As Type, hash As Func(Of Object, Integer))
            hashHandler(target) = hash
        End Sub

        Private Function getHashValue(obj As Object) As Integer
            If TypeOf obj Is Integer Then
                Return DirectCast(obj, Integer)
            ElseIf hashHandler.ContainsKey(obj.GetType) Then
                Return hashHandler(obj.GetType)(obj)
            Else
                Return obj.GetHashCode
            End If
        End Function

        ''' <summary>
        ''' get FNV-1a hascode
        ''' 
        ''' ```
        ''' hash = offset_basis
        ''' 
        ''' for each octet_of_data to be hashed
        '''   hash = hash Xor octet_of_data
        '''   hash = hash * FNV_prime
        '''   
        ''' return hash
        ''' ```
        ''' </summary>
        ''' <param name="targets"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' http://www.isthe.com/chongo/tech/comp/fnv/index.html
        ''' </remarks>
        Public Function GetHashCode(targets As IEnumerable(Of Object)) As Integer
            Const offset As Integer = 2166136261
            Const prime As Integer = 16777619

            Return targets.Aggregate(
                seed:=offset,
                func:=Function(hashCode As Integer, value As Object)
                          If value Is Nothing Then
                              Return (hashCode Xor 0) * prime
                          Else
                              Return (hashCode Xor getHashValue(value)) * prime
                          End If
                      End Function)
        End Function
    End Module
End Namespace
