#Region "Microsoft.VisualBasic::4edda3117569b3af7f57ccdd51ef2f3e, modules\keggReport\ReportRender.vb"

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

    '   Total Lines: 149
    '    Code Lines: 112 (75.17%)
    ' Comment Lines: 21 (14.09%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 16 (10.74%)
    '     File Size: 5.59 KB


    ' Class ReportRender
    ' 
    '     Function: (+3 Overloads) CreateMap, (+3 Overloads) Render
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
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

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

        Dim highlights As New MapHighlights With {
            .compounds = compounds,
            .genes = genes,
            .proteins = proteins
        }

        Return Render(map, highlights, text_color)
    End Function

    Public Shared Function Render(map As Map, highlights As MapHighlights, Optional text_color As String = "white") As String
        Dim mapjson As MapShape() = map.shapes.mapdata _
            .Select(AddressOf CreateMap) _
            .ToArray
        Dim rendering As Image = LocalRender.Rendering(map, highlights, textColor:=text_color)

        With New ScriptBuilder(My.Resources.map_template)
            !title = map.name
            !map_json = mapjson.GetJson(indent:=True)
            !map_base64 = New DataURI(rendering).ToString
            !image_width = rendering.Width
            !keggLink = New NamedCollection(Of NamedValue(Of String))() With {
                .name = If(map.EntryId.IsPattern("\d+"), $"map{map.EntryId}", map.EntryId),
                .description = map.name,
                .value = highlights.PopulateAllHighlights.ToArray
            }.KEGGURLEncode

            Return .ToString
        End With
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="map"></param>
    ''' <param name="highlights">
    ''' a collection of ``[kegg_id => color]`` tuples.
    ''' </param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Render(map As Map, highlights As IEnumerable(Of NamedValue(Of String)), Optional text_color As String = "white") As String
        Return Render(map, MapHighlights.CreateAuto(highlights), text_color)
    End Function
End Class


