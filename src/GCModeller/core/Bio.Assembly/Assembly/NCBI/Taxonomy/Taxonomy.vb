Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI

    Public Class TaxonValue : Inherits ClassObject

        ''' <summary>
        ''' Class level
        ''' </summary>
        ''' <returns></returns>
        Public Property Rank As String
        Public Property nodes As TaxonValue()
        ''' <summary>
        ''' ``gi -> gbff.head``
        ''' </summary>
        ''' <returns></returns>
        Public Property sp As NamedValue(Of BriefInfo)
    End Class

    Public Class BriefInfo : Inherits ClassObject

    End Class

    ''' <summary>
    ''' Build Taxonomy tree from NCBI genbank data.
    ''' </summary>
    Public Module Taxonomy

    End Module
End Namespace