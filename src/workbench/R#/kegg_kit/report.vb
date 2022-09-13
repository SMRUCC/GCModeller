#Region "Microsoft.VisualBasic::1c64d4528d6f89e10eadfba83d781f5c, R#\kegg_kit\report.vb"

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

    '   Total Lines: 233
    '    Code Lines: 167
    ' Comment Lines: 43
    '   Blank Lines: 23
    '     File Size: 9.86 KB


    ' Module report
    ' 
    '     Function: checkIntersection, fromTable, getHighlightObjects, loadMap, MapRender
    '               parseUrl, renderMapHighlights, showReportHtml, singleColor, url
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.GCModeller.Workbench.KEGGReport
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports any = Microsoft.VisualBasic.Scripting
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' the kegg pathway map report helper tool
''' </summary>
<Package("report.utils")>
Module report

    ''' <summary>
    ''' load a blank kegg pathway map template object from a given file object.
    ''' </summary>
    ''' <param name="file">a given file object, it can be a file path or a file input stream.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("loadMap")>
    <RApiReturn(GetType(Map))>
    Public Function loadMap(file As Object, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return Internal.debug.stop("file can not be nothing!", env)
        End If

        If TypeOf file Is String Then
            With DirectCast(file, String)
                If .FileExists Then
                    Return .LoadXml(Of Map)
                Else
                    Return .LoadFromXml(Of Map)
                End If
            End With
        ElseIf TypeOf file Is Stream Then
            Return New StreamReader(DirectCast(file, Stream)).ReadToEnd.LoadFromXml(Of Map)
        Else
            Return Internal.debug.stop({
                 "invalid data type!",
                 "required: " & GetType(String).FullName,
                 "but given: " & file.GetType.FullName
            }, env)
        End If
    End Function

    <ExportAPI("map.local_render")>
    Public Function MapRender(maps As Dictionary(Of String, Map)) As LocalRender
        Return New LocalRender(maps)
    End Function

    <ExportAPI("nodes.colorAs")>
    Public Function singleColor(nodes As String(), color$) As NamedValue(Of String)()
        Return nodes.Select(Function(id) New NamedValue(Of String)(id, color)).ToArray
    End Function

    ''' <summary>
    ''' generate the kegg pathway map highlight image render result
    ''' </summary>
    ''' <param name="map">the blank template of the kegg map</param>
    ''' <param name="highlights">a list of object with color highlights, or url</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("keggMap.highlights")>
    Public Function renderMapHighlights(map As Map, <RRawVectorArgument> highlights As Object,
                                        Optional text_color As String = "white",
                                        Optional env As Environment = Nothing) As Object
        Dim highlightObjs = getHighlightObjects(highlights, env)

        If highlightObjs Like GetType(Message) Then
            Return highlightObjs.TryCast(Of Message)
        Else
            Return LocalRender.Rendering(map, highlightObjs.TryCast(Of NamedValue(Of String)()), textColor:=text_color)
        End If
    End Function

    <Extension>
    Private Function fromTable(table As IDictionary) As NamedValue(Of String)()
        Return (From key As Object
                In table.Keys.AsQueryable
                Let value As Object = table(key)
                Let colorVal As String = RColorPalette.getColor(value)
                Select New NamedValue(Of String) With {
                    .Name = any.ToString(key),
                    .Value = colorVal
                }).ToArray
    End Function

    Private Function getHighlightObjects(highlights As Object, env As Environment) As [Variant](Of Message, NamedValue(Of String)())
        If TypeOf highlights Is vector Then
            highlights = DirectCast(highlights, vector).data
        End If

        If TypeOf highlights Is NamedValue(Of String)() Then
            Return DirectCast(highlights, NamedValue(Of String)())
        ElseIf TypeOf highlights Is String()() Then
            Return DirectCast(highlights, String()()) _
                .Select(Function(tuple)
                            Return New NamedValue(Of String) With {
                                .Name = tuple(Scan0),
                                .Value = tuple(1)
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf highlights Is list Then
            Return DirectCast(highlights, list).slots.fromTable
        ElseIf TypeOf highlights Is Dictionary(Of String, String) Then
            Return DirectCast(highlights, Dictionary(Of String, String)).fromTable
        ElseIf TypeOf highlights Is Dictionary(Of String, Object) Then
            Return DirectCast(highlights, Dictionary(Of String, Object)).fromTable
        ElseIf TypeOf highlights Is KeyValuePair(Of String, Object)() Then
            Return DirectCast(highlights, KeyValuePair(Of String, Object)()) _
                .Select(Function(t)
                            Return New NamedValue(Of String) With {
                                .Name = t.Key,
                                .Value = RColorPalette.getColor(t.Value)
                            }
                        End Function) _
                .ToArray

        ElseIf TypeOf highlights Is String() OrElse TypeOf highlights Is String Then
            Return URLEncoder.URLParser(renv.GetFirst(highlights)).value
        Else
            Return Internal.debug.stop(New InvalidCastException(highlights.GetType.FullName), env)
        End If
    End Function

    ''' <summary>
    ''' generate the kegg pathway map highlight html report
    ''' </summary>
    ''' <param name="map">the blank template of the kegg map</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("keggMap.reportHtml")>
    Public Function showReportHtml(map As Map,
                                   <RRawVectorArgument>
                                   highlights As Object,
                                   Optional text_color As String = "white",
                                   Optional env As Environment = Nothing) As Object

        Dim highlightObjs = getHighlightObjects(highlights, env)

        If highlightObjs Like GetType(Message) Then
            Return highlightObjs.TryCast(Of Message)
        Else
            Return ReportRender.Render(map, highlightObjs.TryCast(Of NamedValue(Of String)()), text_color:=text_color)
        End If
    End Function

    ''' <summary>
    ''' check object id that intersect with the current given map object.
    ''' </summary>
    ''' <param name="map">a given kegg pathway map object model</param>
    ''' <param name="list">an object id list</param>
    ''' <returns></returns>
    <ExportAPI("map.intersects")>
    Public Function checkIntersection(map As Map, list As String()) As String()
        Return map _
            .GetMembers _
            .Intersect(list) _
            .ToArray
    End Function

    ''' <summary>
    ''' generate the url that used for view the highlight result on kegg website.
    ''' </summary>
    ''' <param name="mapId">the id of the map object.</param>
    ''' <param name="highlights">a list of object with color highlight specifics</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("keggMap.url")>
    <RApiReturn(GetType(String))>
    Public Function url(mapId As String, highlights As Object, Optional env As Environment = Nothing) As Object
        Dim highlightObjs = getHighlightObjects(highlights, env)

        If highlightObjs Like GetType(Message) Then
            Return highlightObjs.TryCast(Of Message)
        Else
            Return New NamedCollection(Of NamedValue(Of String))() With {
                .name = mapId,
                .description = Nothing,
                .value = highlightObjs.TryCast(Of NamedValue(Of String)())
            }.KEGGURLEncode
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="url"></param>
    ''' <param name="compound">the default color string for kegg compounds</param>
    ''' <param name="gene">the default color string for KO/gene</param>
    ''' <param name="reaction">the default color string for kegg reactions</param>
    ''' <returns></returns>
    <ExportAPI("parseKeggUrl")>
    Public Function parseUrl(url As String,
                             Optional compound$ = "blue",
                             Optional gene$ = "red",
                             Optional reaction$ = "green") As list

        Dim data = URLEncoder.URLParser(url)
        Dim result As New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"map", data.name},
                {"objects", Nothing}
            }
        }
        Dim kegg_objects As New Dictionary(Of String, Object)

        For Each item As NamedValue(Of String) In data
            Select Case item.Value
                Case "Compound" : kegg_objects(item.Name) = compound
                Case "KO" : kegg_objects(item.Name) = gene
                Case "Reaction" : kegg_objects(item.Name) = reaction
                Case Else
                    kegg_objects(item.Name) = item.Value.TrimEnd("#"c, " "c, ASCII.TAB, "/"c)
            End Select
        Next

        result.slots("objects") = kegg_objects.ToArray

        Return result
    End Function

End Module
