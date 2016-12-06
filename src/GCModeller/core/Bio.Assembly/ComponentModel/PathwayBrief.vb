#Region "Microsoft.VisualBasic::079f8559a80b7228afe89e6f7d18402a, ..\GCModeller\core\Bio.Assembly\ComponentModel\PathwayBrief.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports System.Xml.Serialization

Namespace ComponentModel

    Public MustInherit Class PathwayBrief
        Implements IKeyValuePairObject(Of String, String)
        Implements INamedValue

        <XmlAttribute>
        Public Overridable Property EntryId As String Implements INamedValue.Key, IKeyValuePairObject(Of String, String).Identifier
        Public Property Description As String Implements IKeyValuePairObject(Of String, String).Value

        ''' <summary>
        ''' Gets the pathway related genes.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetPathwayGenes() As String()

        ''' <summary>
        ''' 和具体的物种的编号无关的在KEGG数据库之中的参考对象的编号
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property BriteId As String
            Get
                Return EntryId
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", EntryId, Description)
        End Function
    End Class
End Namespace
