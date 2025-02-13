Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel

    Public Class HashList(Of T As IAddressOf) : Implements Enumeration(Of T)


        Public Sub New()
        End Sub

        Public Iterator Function GenericEnumerator() As IEnumerator(Of T) Implements Enumeration(Of T).GenericEnumerator

        End Function
    End Class
End Namespace