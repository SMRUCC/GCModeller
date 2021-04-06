Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Vector

    Public Class ChromosomeComposition : Implements IEnumerable(Of NamedValue(Of Double))

        Public Property repliconId As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property A As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property T As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property G As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property C As Integer

        Public Overrides Function ToString() As String
            Return repliconId
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of NamedValue(Of Double)) Implements IEnumerable(Of NamedValue(Of Double)).GetEnumerator
            Yield New NamedValue(Of Double)("A", A)
            Yield New NamedValue(Of Double)("T", T)
            Yield New NamedValue(Of Double)("G", G)
            Yield New NamedValue(Of Double)("C", C)
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace