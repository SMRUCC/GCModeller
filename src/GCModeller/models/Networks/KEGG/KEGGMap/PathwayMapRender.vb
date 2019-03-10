﻿#Region "Microsoft.VisualBasic::fd866e55fda5a8309ed0bfa903277167, models\Networks\KEGG\KEGGMap\PathwayMapRender.vb"

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

' Module PathwayMapRender
' 
'     Function: (+3 Overloads) QueryMaps, RenderMaps
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module PathwayMapRender

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="keggList"></param>
    ''' <param name="internalResource">zip package data</param>
    ''' <returns></returns>
    Public Function QueryMaps(keggList As IEnumerable(Of String), internalResource As IEnumerable(Of Byte)) As IEnumerable(Of NamedValue(Of Image))
        Dim zip$ = App.GetAppSysTempFile(".zip", App.PID)
        Dim repo$ = App.GetAppSysTempFile(".zip", App.PID)

        Call internalResource.FlushStream(zip)
        Call ZipLib.ImprovedExtractToDirectory(zip, repo, Overwrite.Always)

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
            Dim nodes = foundResult _
                .Value _
                .Select(Function(x) idTable(x)) _
                .ToArray

            Try
                Yield New NamedValue(Of Image) With {
                    .Name = foundResult.Name,
                    .Value = render _
                        .Rendering(.Name, nodes,, scale:=scale),
                    .Description = foundResult _
                        .Value _
                        .JoinBy("|")
                }
            Catch ex As Exception
                ex = New Exception(foundResult.GetJson, ex)

                If throwException Then
                    Throw ex
                Else
                    Call ex.PrintException
                    Call App.LogException(ex)
                End If
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
