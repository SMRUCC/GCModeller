Namespace Metagenomics

    Public Structure Taxonomy

        Public Property scientificName As String
        ''' <summary>
        ''' 1. 域
        ''' </summary>
        Public Property domain As String
        ''' <summary>
        ''' 2. 界
        ''' </summary>
        Public Property kingdom As String
        ''' <summary>
        ''' 3. 门
        ''' </summary>
        Public Property phylum As String
        ''' <summary>
        ''' 4A. 纲
        ''' </summary>
        Public Property [class] As String
        ''' <summary>
        ''' 5B. 目
        ''' </summary>
        Public Property order As String
        ''' <summary>
        ''' 6C. 科
        ''' </summary>
        Public Property family As String
        ''' <summary>
        ''' 7D. 属
        ''' </summary>
        Public Property genus As String
        ''' <summary>
        ''' 8E. 种
        ''' </summary>
        Public Property species As String

        Public Overrides Function ToString() As String
            Return scientificName
        End Function
    End Structure
End Namespace