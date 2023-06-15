#Region "Microsoft.VisualBasic::cb7f2c62bbb4ffa55b729a114e7ef664, modules\keggReport\PathwayMapRender.vb"

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

'   Total Lines: 112
'    Code Lines: 79
' Comment Lines: 19
'   Blank Lines: 14
'     File Size: 4.79 KB


' Module PathwayMapRender
' 
'     Function: (+4 Overloads) QueryMaps, RenderMaps
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Zip
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

''' <summary>
''' 在空白的pathway map的基础上利用map中预先定义的位置进行区域颜色渲染 
''' </summary>
Public Module PathwayMapRender

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="keggList"></param>
    ''' <param name="internalResource">zip package data</param>
    ''' <returns></returns>
    Public Function QueryMaps(keggList As IEnumerable(Of String), internalResource As IEnumerable(Of Byte)) As IEnumerable(Of NamedValue(Of Image))
        Dim zip$ = TempFileSystem.GetAppSysTempFile(".zip", App.PID)
        Dim repo$ = TempFileSystem.GetAppSysTempFile(".zip", App.PID)

        Call internalResource.FlushStream(zip)
        Call UnZip.ImprovedExtractToDirectory(zip, repo, Overwrite.Always)

        Return keggList.QueryMaps(repo)
    End Function

    <Extension>
    Public Function QueryMaps(keggList As IEnumerable(Of String), repo$, Optional color$ = "blue", Optional scale$ = "1,1") As IEnumerable(Of NamedValue(Of Image))
        Return LocalRender.FromRepository(repo).QueryMaps(keggList, 1, color, scale:=scale)
    End Function

    <Extension>
    Public Function QueryMaps(render As LocalRender, keggList As IEnumerable(Of String),
                              Optional threshold% = 3,
                              Optional color$ = "red",
                              Optional scale$ = "1,1",
                              Optional throwException As Boolean = True) As IEnumerable(Of NamedValue(Of Image))
        Return render.QueryMaps(
            keggList:=keggList.Select(Function(id) New NamedValue(Of String)(id, color)),
            threshold:=threshold,
            scale:=scale,
            throwException:=throwException
        )
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="render"></param>
    ''' <param name="keggList">``{id => color}``</param>
    ''' <param name="threshold%"></param>
    ''' <param name="scale$"></param>
    ''' <param name="throwException"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function QueryMaps(render As LocalRender,
                                       keggList As IEnumerable(Of NamedValue(Of String)),
                                       Optional threshold% = 3,
                                       Optional scale$ = "1,1",
                                       Optional throwException As Boolean = True) As IEnumerable(Of NamedValue(Of Image))

        Dim idTable As Dictionary(Of NamedValue(Of String)) = keggList.ToDictionary

        ' 首先查找出化合物在哪些map之中出现，然后生成绘图查询数据
        For Each foundResult As NamedValue(Of String()) In render.IteratesMapNames(keggList.Keys, 3)
            Dim nodes As NamedValue(Of String)() = foundResult.Value _
                .Select(Function(x) idTable(x)) _
                .ToArray
            Dim highlights As MapHighlights = MapHighlights.CreateAuto(nodes)

            Try
                Yield New NamedValue(Of Image) With {
                    .Name = foundResult.Name,
                    .Value = render _
                        .Rendering(.Name, highlights,, scale:=scale),
                    .Description = foundResult _
                        .Value _
                        .JoinBy("|")
                }
            Catch ex As Exception When throwException
                Throw New Exception(foundResult.GetJson, ex)
            Catch ex As Exception
                Call ex.PrintException
                Call App.LogException(ex)
            End Try
        Next
    End Function

    Public Function RenderMaps(repo$, idlist$(), out$, Optional scale$ = "1.5,1.5") As NamedValue(Of String)()
        Dim render As LocalRender = LocalRender.FromRepository(repo)
        Dim maplist As New List(Of NamedValue(Of String))

        For Each map As NamedValue(Of Image) In render.QueryMaps(idlist,, scale:=scale, throwException:=False)
            Dim save$ = $"{out}/{map.Name}.png"

            map.Value.SaveAs(save, ImageFormats.Png)
            maplist += New NamedValue(Of String) With {
                .Name = map.Name,
                .Value = render.GetTitle(map.Name),
                .Description = map.Description
            }
        Next

        Return maplist
    End Function
End Module
