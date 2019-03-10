#Region "Microsoft.VisualBasic::ef62d31bb5c1bd6d8912047ac77d74ea, analysis\ProteinTools\Interactions.BioGRID\TabularFiles\IDENTIFIERS.vb"

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

    ' Class IDENTIFIERS
    ' 
    '     Properties: BIOGRID_ID, IDENTIFIER_TYPE, IDENTIFIER_VALUE, ORGANISM_OFFICIAL_NAME
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

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
