#Region "Microsoft.VisualBasic::96ac4ae0a5388ad1ce377b2e47b85c80, GCModeller\models\BIOM\BIOM\v1.0\v1.0.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 178
    '    Code Lines: 64
    ' Comment Lines: 102
    '   Blank Lines: 12
    '     File Size: 6.38 KB


    '     Class BIOMDataSet
    ' 
    '         Properties: [date], columns, comment, data, format
    '                     format_url, generated_by, id, matrix_element_type, matrix_type
    '                     rows, shape, type
    ' 
    '         Function: LoadFile, PopulateRows, ToJSON, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.foundation.BIOM.v10.components

Namespace v10

    ''' <summary>
    ''' ##### The biom file format: Version 1.0
    ''' 
    ''' The ``biom`` format is based on ``JSON`` to provide the overall structure for the format. 
    ''' JSON is a widely supported format with native parsers available within many programming 
    ''' languages.
    ''' </summary>
    Public Class BIOMDataSet(Of T As {IComparable(Of T), IEquatable(Of T), IComparable})

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
        ''' <remarks>
        ''' + 行表示一个OTU，对应的物种信息可以在row属性之中查找到
        ''' + 列表示一个sample，对应的sample编号，类型等信息可以在column属性之中查找到
        ''' </remarks>
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
            Return ToJSON()
        End Function

        Public Iterator Function PopulateRows(Optional metaInfoAsRowID As Boolean = True) As IEnumerable(Of NamedValue(Of [Property](Of T)))
            Dim rowID As String
            Dim rowValue As T()
            Dim dataSet As [Property](Of T)

            For Each row As SeqValue(Of row) In rows.SeqIterator
                rowValue = data(row)
                dataSet = New [Property](Of T)

                For i As Integer = 0 To columns.Length - 1
                    Call dataSet.Add(columns(i).id, rowValue(i))
                Next

                If metaInfoAsRowID AndAlso row.value.hasMetaInfo Then
                    rowID = row.value.metadata.lineage.ToString(BIOMstyle:=True)
                Else
                    rowID = row.value.id
                End If

                Yield New NamedValue(Of [Property](Of T)) With {
                    .Name = rowID,
                    .Value = dataSet
                }
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToJSON() As String
            Return JsonContract.GetObjectJson(
                type:=GetType(BIOMDataSet(Of T)),
                obj:=Me,
                indent:=True,
                knownTypes:={
                    GetType(IntegerMatrix),
                    GetType(FloatMatrix),
                    GetType(StringMatrix)
                }
            )
        End Function

        Public Shared Function LoadFile(path As String) As BIOMDataSet(Of T)
            Dim json$ = path.ReadAllText
            Dim biom As BIOMDataSet(Of T) = JsonContract.EnsureDate(json, "date").LoadJSON(Of BIOMDataSet(Of T))
            Return biom
        End Function
    End Class

End Namespace
