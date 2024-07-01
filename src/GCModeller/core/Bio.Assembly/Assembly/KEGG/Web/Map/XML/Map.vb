#Region "Microsoft.VisualBasic::2f4415825de6f23262fddb5f9b35ab74, core\Bio.Assembly\Assembly\KEGG\Web\Map\XML\Map.vb"

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

    '   Total Lines: 97
    '    Code Lines: 67 (69.07%)
    ' Comment Lines: 15 (15.46%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (15.46%)
    '     File Size: 3.38 KB


    '     Class Map
    ' 
    '         Properties: PathwayImage, shapes, URL
    ' 
    '         Function: GenericEnumerator, GetCompoundSet, GetImage, GetMembers, GetPathwayGenes
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Assembly.KEGG.WebServices.XML

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
        Public Property shapes As MapData

        ''' <summary>
        ''' base64 image
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("image")>
        Public Property PathwayImage As String

        ''' <summary>
        ''' Get all member id list from this pathway map object.
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMembers() As String()
            Return shapes.mapdata _
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

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Area) Implements Enumeration(Of Area).GenericEnumerator
            For Each item As Area In shapes.mapdata
                Yield item
            Next
        End Function

        Public Overrides Iterator Function GetPathwayGenes() As IEnumerable(Of NamedValue(Of String))
            For Each shape As Area In shapes.mapdata
                Dim list = shape.Names.ToArray

                For Each id As NamedValue(Of String) In list
                    If Not id.Name.IsPattern("[CDGRM]\d+") Then
                        Yield id
                    End If
                Next
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>
        ''' a unique set of the kegg compound id
        ''' </returns>
        Public Overrides Iterator Function GetCompoundSet() As IEnumerable(Of NamedValue(Of String))
            Dim unique As New Index(Of String)

            For Each shape As Area In shapes.mapdata
                Dim list = shape.Names.ToArray

                For Each id As NamedValue(Of String) In list
                    If id.Name.IsPattern("C\d+") AndAlso Not id.Name Like unique Then
                        unique.Add(id)
                        Yield id
                    End If
                Next
            Next
        End Function
    End Class
End Namespace
