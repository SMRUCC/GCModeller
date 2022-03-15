Namespace Analysis.FastUnfolding

    Friend Class Matrix

        Public Property Q As Double
        Public Property tag_dict As Dictionary(Of String, String)
        Public Property tag_dict2 As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return $"Q={Q.ToString("F4")}"
        End Function
    End Class

    Friend Class Map
        Public Property tag2 As Dictionary(Of String, String)
        Public Property map2 As KeyMaps
    End Class

End Namespace