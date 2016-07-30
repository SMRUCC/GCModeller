Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' ``BIOGRID-IDENTIFIERS-3.4.138.tab.zip``
''' 
''' This zip archive contains a Single file formatted In Microsoft Excel tab delimited 
''' text file format containing a list Of mappings between different identifiers stored 
''' In our database And the identifiers used In our downloads. You can use this file 
''' To map from one identifier type To another.
''' </summary>
''' <remarks>
''' Brief Description of the Columns: 
'''
''' + ``BIOGRID_ID``              Identifier for this ID within the BioGRID Database
''' + ``IDENTIFIER_VALUE``        Identifier Value Mapped
''' + ``IDENTIFIER_TYPE``         A Brief Text Description of the Identifier
''' + ``ORGANISM_OFFICIAL_NAME``  Official name of the organism
''' </remarks>
Public Class IDENTIFIERS

    ''' <summary>
    ''' Identifier for this ID within the BioGRID Database
    ''' </summary>
    ''' <returns></returns>
    Public Property BIOGRID_ID As String
    ''' <summary>
    ''' Identifier Value Mapped
    ''' </summary>
    ''' <returns></returns>
    Public Property IDENTIFIER_VALUE As String
    ''' <summary>
    ''' A Brief Text Description of the Identifier
    ''' </summary>
    ''' <returns></returns>
    Public Property IDENTIFIER_TYPE As String
    ''' <summary>
    ''' Official name of the organism
    ''' </summary>
    ''' <returns></returns>
    Public Property ORGANISM_OFFICIAL_NAME As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
