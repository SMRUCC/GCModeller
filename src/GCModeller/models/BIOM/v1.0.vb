#Region "Microsoft.VisualBasic::2d9dfe6095f59656252e32d638527f68, ..\GCModeller\models\BIOM\v1.0.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

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
    ''' BIOM json with integer matrix data
    ''' </summary>
    Public Class IntegerMatrix : Inherits Json(Of Integer)

        Public Overloads Shared Function LoadFile(path$) As IntegerMatrix
            Dim json$ = path.ReadAllText
            Dim biom As IntegerMatrix = JsonContract.EnsureDate(json, "date").LoadObject(Of IntegerMatrix)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' BIOM json with double matrix data
    ''' </summary>
    Public Class FloatMatrix : Inherits Json(Of Double)

        Public Overloads Shared Function LoadFile(path$) As FloatMatrix
            Dim json$ = path.ReadAllText
            Dim biom As FloatMatrix = JsonContract.EnsureDate(json, "date").LoadObject(Of FloatMatrix)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' BIOM json with string matrix data
    ''' </summary>
    Public Class StringMatrix : Inherits Json(Of String)

        Public Overloads Shared Function LoadFile(path$) As StringMatrix
            Dim json$ = path.ReadAllText
            Dim biom As StringMatrix = JsonContract.EnsureDate(json, "date").LoadObject(Of StringMatrix)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' ##### The biom file format: Version 1.0
    ''' 
    ''' The ``biom`` format is based on ``JSON`` to provide the overall structure for the format. 
    ''' JSON is a widely supported format with native parsers available within many programming 
    ''' languages.
    ''' </summary>
    Public Class Json(Of T As {IComparable(Of T), IEquatable(Of T), IComparable})

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
        Public Property data As T()()
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

        Public Shared Function LoadFile(path$) As Json(Of T)
            Dim json$ = path.ReadAllText
            Dim biom As Json(Of T) = JsonContract.EnsureDate(json, "date").LoadObject(Of Json(Of T))
            Return biom
        End Function
    End Class

    Public Class row
        Public Property id As String
        Public Property metadata As meta
    End Class

    Public Class meta
        Public Property taxonomy As String()
        Public Property KEGG_Pathways As String()
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
