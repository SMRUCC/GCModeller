#Region "Microsoft.VisualBasic::7c9498c98d35bddebf6ca20324681cbd, engine\IO\GCMarkupLanguage\v2\Xml\Metabolism\Pathway.vb"

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

    '   Total Lines: 39
    '    Code Lines: 27 (69.23%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (30.77%)
    '     File Size: 1.20 KB


    '     Class FunctionalCategory
    ' 
    '         Properties: category, pathways
    ' 
    '         Function: ToString
    ' 
    '     Class Pathway
    ' 
    '         Properties: ID, name, note, reactions
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace v2

    Public Class FunctionalCategory

        <XmlAttribute>
        Public Property category As String
        <XmlElement("pathway")>
        Public Property pathways As Pathway()

        Public Overrides Function ToString() As String
            Return category
        End Function

    End Class

    <XmlType("pathway", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Pathway : Implements INamedValue

        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String

        Public Property reactions As String()

        Public Property note As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{ID}] {name} with {reactions.Length} reactions"
        End Function

    End Class

End Namespace
