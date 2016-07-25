Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v10

    ''' <summary>
    ''' ``&lt;string>`` Table type (a controlled vocabulary) acceptable values
    ''' </summary>
    Public Class types

        Public Const OTU_table As String = "OTU table"
        Public Const Pathway_table As String = "Pathway table"
        Public Const Function_table As String = "Function table"
        Public Const Ortholog_table As String = "Ortholog table"
        Public Const Gene_table As String = "Gene table"
        Public Const Metabolite_table As String = "Metabolite table"
        Public Const Taxon_table As String = "Taxon table"
    End Class

    ''' <summary>
    ''' ##### The biom file format: Version 1.0
    ''' 
    ''' The ``biom`` format is based on ``JSON`` to provide the overall structure for the format. 
    ''' JSON is a widely supported format with native parsers available within many programming 
    ''' languages.
    ''' </summary>
    Public Class Json

        ''' <summary>
        ''' ``&lt;string or null>`` a field that can be used to id a table (or null)
        ''' </summary>
        ''' <returns></returns>
        Public Property id As String
        ''' <summary>
        ''' ``&lt;string>`` The name and version of the current biom format
        ''' </summary>
        ''' <returns></returns>
        Public Property format As String
        ''' <summary>
        ''' ``&lt;url>`` A string with a static URL providing format details
        ''' </summary>
        ''' <returns></returns>
        Public Property format_url As String
        ''' <summary>
        ''' ``&lt;string>`` Table type (a controlled vocabulary)
        ''' Acceptable values : 
        ''' 
        ''' + "OTU table"
        ''' + "Pathway table"
        ''' + "Function table"
        ''' + "Ortholog table"
        ''' + "Gene table"
        ''' + "Metabolite table"
        ''' + "Taxon table"
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String
        Public Property generated_by As String = "GCModeller"
        Public Property [date] As Date
        Public Property matrix_type As String
        Public Property matrix_element_type As String
        Public Property shape As Integer()
        Public Property data As Integer()()
        Public Property rows As row()
        Public Property columns As column()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class row
        Public Property id As String
        Public Property metadata As meta
    End Class

    Public Class meta
        Public Property taxonomy As String()

    End Class

    Public Class column
        Public Property id As String
        Public Property metadata As columnMeta
    End Class

    Public Class columnMeta
        Public Property BarcodeSequence As String
        Public Property LinkerPrimerSequence As String
        Public Property BODY_SITE As String
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace