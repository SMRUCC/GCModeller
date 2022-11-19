#Region "Microsoft.VisualBasic::646cf97e0ca0e79e1cdebcb80059d6ab, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\ReferenceMap\Reaction.vb"

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

    '   Total Lines: 19
    '    Code Lines: 9
    ' Comment Lines: 6
    '   Blank Lines: 4
    '     File Size: 667 B


    '     Class ReferenceReaction
    ' 
    '         Properties: SSDBs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace Assembly.KEGG.DBGET.ReferenceMap

    <XmlType("KEGG-RefRxnDef", Namespace:="http://code.google.com/p/genome-in-code/kegg/reference_reaction")>
    Public Class ReferenceReaction : Inherits bGetObject.Reaction

        ''' <summary>
        ''' 酶分子的直系同源的参考序列
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SSDBs As NamedCollection(Of QueryEntry)()

    End Class
End Namespace
