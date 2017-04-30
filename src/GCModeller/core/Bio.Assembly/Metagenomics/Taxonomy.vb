Namespace Metagenomics

    Public Structure Taxonomy

        Dim scientificName As String
        ''' <summary>
        ''' 1. 域
        ''' </summary>
        Dim domain As String
        ''' <summary>
        ''' 2. 界
        ''' </summary>
        Dim kingdom As String
        ''' <summary>
        ''' 3. 门
        ''' </summary>
        Dim phylum As String
        ''' <summary>
        ''' 4A. 纲
        ''' </summary>
        Dim [class] As String
        ''' <summary>
        ''' 5B. 目
        ''' </summary>
        Dim order As String
        ''' <summary>
        ''' 6C. 科
        ''' </summary>
        Dim family As String
        ''' <summary>
        ''' 7D. 属
        ''' </summary>
        Dim genus As String
        ''' <summary>
        ''' 8E. 种
        ''' </summary>
        Dim species As String

        Public Overrides Function ToString() As String
            Return scientificName
        End Function
    End Structure
End Namespace