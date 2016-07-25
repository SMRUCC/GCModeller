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

        Public Const MatrixTypeSparse As String = "sparse"
        Public Const MatrixTypeDense As String = "dense"

        Public Const MatrixElTypeInt As String = "int"
        Public Const MatrixElTypeFloat As String = "float"
        Public Const MatrixElTypeUnicode As String = "unicode"
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
        ''' ``&lt;string>`` A free text field containing any information that you
        ''' feel Is relevant (Or just feel Like sharing)
        ''' </summary>
        ''' <returns></returns>
        Public Property comment As String
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
        ''' <summary>
        ''' ``&lt;string>`` Package and revision that built the table
        ''' </summary>
        ''' <returns></returns>
        Public Property generated_by As String = "GCModeller"
        ''' <summary>
        ''' ``&lt;datetime>`` Date the table was built (ISO 8601 format)
        ''' </summary>
        ''' <returns></returns>
        Public Property [date] As Date
        ''' <summary>
        ''' ``&lt;string>`` Type of matrix data representation (a controlled vocabulary)
        ''' Acceptable values : 
        ''' 
        ''' + "sparse" : only non-zero values are specified
        ''' + "dense" : every element must be specified
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property matrix_type As String
        ''' <summary>
        ''' Value type in matrix (a controlled vocabulary)
        ''' Acceptable values : 
        ''' 
        ''' + "int"  Integer
        ''' + "float" : floating point
        ''' + "unicode" : unicode string
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property matrix_element_type As String
        ''' <summary>
        ''' ``&lt;list of ints>``, the number of rows and number of columns in data
        ''' </summary>
        ''' <returns></returns>
        Public Property shape As Integer()
        ''' <summary>
        ''' ``&lt;list of lists>``, counts of observations by sample
        ''' 
        ''' If matrix_type Is "sparse", 
        ''' 
        ''' ```
        ''' [[row, column, value],
        '''  [row, column, value],
        '''  ...]
        ''' ```
        '''  
        ''' If matrix_type Is "dense",  
        ''' 
        ''' ```
        ''' [[value, value, value, ...],
        '''  [value, value, value, ...],
        '''  ...]
        ''' ```
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property data As Integer()()
        ''' <summary>
        ''' ``&lt;list of objects>`` An ORDERED list of obj describing the rows
        ''' </summary>
        ''' <returns></returns>
        Public Property rows As row()
        ''' <summary>
        ''' ``&lt;list of objects>`` An ORDERED list of obj  describing the columns
        ''' </summary>
        ''' <returns></returns>
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