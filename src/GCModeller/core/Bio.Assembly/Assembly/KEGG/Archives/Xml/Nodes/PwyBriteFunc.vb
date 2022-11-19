#Region "Microsoft.VisualBasic::64efb58ee0e3cc98e1770dd922569b3a, GCModeller\core\Bio.Assembly\Assembly\KEGG\Archives\Xml\Nodes\PwyBriteFunc.vb"

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

    '   Total Lines: 29
    '    Code Lines: 13
    ' Comment Lines: 12
    '   Blank Lines: 4
    '     File Size: 917 B


    '     Class PwyBriteFunc
    ' 
    '         Properties: [Class], Category, Pathways
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.DBGET

Namespace Assembly.KEGG.Archives.Xml.Nodes

    <XmlType("KEGG.CellPhenotype_BritText", Namespace:="http://code.google.com/p/genome-in-code/kegg/bio_model/")>
    Public Class PwyBriteFunc

        ''' <summary>
        ''' 大分类
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property [Class] As String
        ''' <summary>
        ''' 小分类
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Category As String
        <XmlArray("KEGG.Pathways")> Public Property Pathways As bGetObject.Pathway()

        Public Overrides Function ToString() As String
            Return [Class]
        End Function
    End Class
End Namespace
