#Region "Microsoft.VisualBasic::3df99bda3492b84a6b500d78cd18b26e, GCModeller\core\Bio.Assembly\ComponentModel\Annotation\PathwayBrief.vb"

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
    '    Code Lines: 24
    ' Comment Lines: 9
    '   Blank Lines: 6
    '     File Size: 1.45 KB


    '     Class PathwayBrief
    ' 
    '         Properties: briteID, description, EntryId
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace ComponentModel.Annotation

    Public MustInherit Class PathwayBrief : Inherits XmlDataModel
        Implements IKeyValuePairObject(Of String, String)
        Implements INamedValue

        <XmlAttribute("id")>
        Public Overridable Property EntryId As String Implements INamedValue.Key, IKeyValuePairObject(Of String, String).Key
        ''' <summary>
        ''' The map title display name
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        Public Property description As String Implements IKeyValuePairObject(Of String, String).Value

        ''' <summary>
        ''' Gets the pathway related genes.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetPathwayGenes() As IEnumerable(Of NamedValue(Of String))
        Public MustOverride Function GetCompoundSet() As IEnumerable(Of NamedValue(Of String))

        ''' <summary>
        ''' 和具体的物种的编号无关的在KEGG数据库之中的参考对象的编号
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <XmlIgnore>
        Public Overridable ReadOnly Property briteID As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return EntryId
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", EntryId, description)
        End Function
    End Class
End Namespace
