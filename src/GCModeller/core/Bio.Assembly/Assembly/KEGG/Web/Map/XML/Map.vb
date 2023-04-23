#Region "Microsoft.VisualBasic::59dd70b9b96bddb6ff71c28ee99462f8, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Map\XML\Map.vb"

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

'   Total Lines: 95
'    Code Lines: 61
' Comment Lines: 19
'   Blank Lines: 15
'     File Size: 3.21 KB


'     Class Map
' 
'         Properties: description, id, Name, PathwayImage, shapes
'                     URL
' 
'         Function: GenericEnumerator, GetEnumerator, GetImage, GetMembers, ParseFromUrl
'                   ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' The kegg reference map
    ''' </summary>
    <XmlRoot("Map", [Namespace]:=Map.XmlNamespace)>
    Public Class Map : Inherits PathwayBrief
        Implements INamedValue
        Implements Enumeration(Of Area)

        Public Const XmlNamespace$ = "http://GCModeller.org/core/KEGG/KGML_map.xsd"

        Public Property URL As String

        ''' <summary>
        ''' 节点的位置，在这里面包含有代谢物(小圆圈)以及基因(方块)的位置定义
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("shapes")>
        Public Property shapes As Area()

        ''' <summary>
        ''' base64 image
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("KEGGmap")>
        Public Property PathwayImage As String

        ''' <summary>
        ''' Get all member id list from this pathway map object.
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMembers() As String()
            Return shapes _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

        Public Function GetImage() As Image
            Dim lines$() = PathwayImage _
                .Trim(" "c, ASCII.LF, ASCII.CR) _
                .LineTokens _
                .Select(AddressOf Trim) _
                .ToArray
            Dim base64$ = String.Join("", lines)

            Return base64.GetImage
        End Function

        Public Overrides Function ToString() As String
            Return shapes.GetJson
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseFromUrl(url As String) As Map
            Return ParseHTML(html:=url.GET)
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Area) Implements Enumeration(Of Area).GenericEnumerator
            For Each item As Area In shapes
                Yield item
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Area).GetEnumerator
            Yield GenericEnumerator()
        End Function

        Public Overrides Function GetPathwayGenes() As IEnumerable(Of NamedValue(Of String))
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetCompoundSet() As IEnumerable(Of NamedValue(Of String))
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
