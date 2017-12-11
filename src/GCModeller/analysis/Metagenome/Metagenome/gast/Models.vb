Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace gast

    ''' <summary>
    ''' *.names
    ''' </summary>
    Public Class Names : Implements INamedValue

        Public Property Unique As String Implements INamedValue.Key
        <Ignored>
        Public Property members As String()
        Public Property NumOfSeqs As Integer
        Public Property taxonomy As String
        Public Property distance As Double
        Public Property refs As String
        <Meta>
        Public Property Composition As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class gastOUT : Implements INamedValue

        Public Property read_id As String Implements INamedValue.Key
        Public Property taxonomy As String
        Public Property distance As Double
        Public Property rank As String
        Public Property refssu_count As String
        Public Property vote As String
        Public Property minrank As String
        Public Property taxa_counts As String
        Public Property max_pcts As String
        Public Property na_pcts As String
        Public Property refhvr_ids As String
        Public Property counts As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace