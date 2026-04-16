#Region "Microsoft.VisualBasic::b3c5d597583c683ee92fe1540a41b745, engine\IO\GCMarkupLanguage\v2\Xml\Metabolism\Compound.vb"

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

    '   Total Lines: 44
    '    Code Lines: 25 (56.82%)
    ' Comment Lines: 8 (18.18%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (25.00%)
    '     File Size: 1.31 KB


    '     Class Compound
    ' 
    '         Properties: db_xrefs, formula, ID, name, referenceIds
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace v2

    <XmlType("compound", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Compound : Implements INamedValue

        <XmlAttribute>
        Public Property ID As String Implements IKeyedEntity(Of String).Key

        ''' <summary>
        ''' the reference id of current metabolite
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property referenceIds As String()

        <XmlText> Public Property name As String

        <XmlAttribute>
        Public Property formula As String

        ''' <summary>
        ''' the cross reference id of this compound, used for search on the workbench ui
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property db_xrefs As String()

        Sub New()
        End Sub

        Sub New(id As String, Optional name As String = Nothing)
            Me.ID = id
            Me.name = If(name, id)
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{ID}] {name}"
        End Function

    End Class
End Namespace
