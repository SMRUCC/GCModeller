#Region "Microsoft.VisualBasic::9f1966fbba1f44cda361c1e0f27730f7, modules\keggReport\ReportRender.vb"

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

    '   Total Lines: 101
    '    Code Lines: 70
    ' Comment Lines: 20
    '   Blank Lines: 11
    '     File Size: 3.50 KB


    ' Class ReportRender
    ' 
    '     Function: (+3 Overloads) CreateMap, Render
    ' 
    ' Class MapShape
    ' 
    '     Properties: entities, isEntity, location, shape, title
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Class ReportRender

    Public Function CreateMap(compound As Compound, location As PointF) As MapShape
        Return New MapShape With {
            .entities = {compound.entry},
            .title = compound.commonNames.FirstOrDefault,
            .location = {location.X, location.Y},
            .shape = "circle"
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gene">
    ''' <see cref="htext.ko00001"/>
    ''' </param>
    ''' <param name="location"></param>
    ''' <returns></returns>
    Public Function CreateMap(gene As BriteHText, location As PointF) As MapShape
        Return New MapShape With {
            .entities = {gene.entryID},
            .location = {location.X, location.Y},
            .shape = "rect",
            .title = gene.description
        }
    End Function

    Public Shared Function CreateMap(area As Area) As MapShape
        Return New MapShape With {
            .shape = area.shape,
            .title = area.title,
            .entities = area.IDVector,
            .location = area.coords _
                .Split(","c) _
                .Select(AddressOf Val) _
                .ToArray,
            .isEntity = Not .entities _
                .All(Function(id)
                         Return id.IsPattern("[KCDGR]\d+")
                     End Function)
        }
    End Function

    Public Shared Function Render(map As Map,
                                  compounds As NamedValue(Of String)(),
                                  genes As NamedValue(Of String)(),
                                  proteins As NamedValue(Of String)(),
                                  Optional text_color As String = "white") As String

        Dim mapjson As MapShape() = map.shapes _
            .Select(AddressOf CreateMap) _
            .ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="map"></param>
    ''' <param name="highlights">
    ''' a collection of ``[kegg_id => color]`` tuples.
    ''' </param>
    ''' <returns></returns>
    Public Shared Function Render(map As Map, highlights As IEnumerable(Of NamedValue(Of String)), Optional text_color As String = "white") As String
        Dim mapjson As MapShape() = map.shapes _
            .Select(AddressOf CreateMap) _
            .ToArray
        Dim objectList As NamedValue(Of String)() = highlights.ToArray
        Dim rendering As Image = LocalRender.Rendering(map, objectList, textColor:=text_color)

        With New ScriptBuilder(My.Resources.map_template)
            !title = map.Name
            !map_json = mapjson.GetJson(indent:=True)
            !map_base64 = New DataURI(rendering).ToString
            !image_width = rendering.Width
            !keggLink = New NamedCollection(Of NamedValue(Of String))() With {
                .name = If(map.EntryId.IsPattern("\d+"), $"map{map.EntryId}", map.EntryId),
                .description = map.name,
                .value = objectList
            }.KEGGURLEncode

            Return .ToString
        End With
    End Function
End Class

Public Class MapShape

    Public Property shape As String
    Public Property location As Double()
    ''' <summary>
    ''' kegg id list
    ''' </summary>
    ''' <returns></returns>
    Public Property entities As String()
    Public Property title As String
    Public Property isEntity As Boolean

    Public Overrides Function ToString() As String
        Return title
    End Function

End Class
