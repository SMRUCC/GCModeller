#Region "Microsoft.VisualBasic::e08a9b0a3d2dee7c0af38de80786697c, core\Bio.Assembly\ComponentModel\DBLinkBuilder\IdMapping\Synonym.vb"

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

    '   Total Lines: 40
    '    Code Lines: 18
    ' Comment Lines: 16
    '   Blank Lines: 6
    '     File Size: 1.29 KB


    '     Class Synonym
    ' 
    '         Properties: [alias], accessionID
    ' 
    '         Function: GenericEnumerator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.DBLinkBuilder

    ''' <summary>
    ''' data alias model, a mapping of the main accession id to 
    ''' a collection of the secondary accession id.
    ''' </summary>
    <XmlType("synonym")>
    Public Class Synonym : Implements Enumeration(Of String)

        ''' <summary>
        ''' the main accession id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property accessionID As String
        ''' <summary>
        ''' the secondary accession id or the alias id in other database.
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property [alias] As String()

        Public Overrides Function ToString() As String
            Return accessionID
        End Function

        ''' <summary>
        ''' get all accession id union
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GenericEnumerator() As IEnumerator(Of String) Implements Enumeration(Of String).GenericEnumerator
            Yield accessionID

            For Each id As String In [alias]
                Yield id
            Next
        End Function
    End Class
End Namespace
