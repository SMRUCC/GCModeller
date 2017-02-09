#Region "Microsoft.VisualBasic::e3af7c83a37b6595b71db9c3baaace29, ..\GCModeller\visualize\SyntenyVisual\ModelAPI.vb"

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

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Public Module ModelAPI

    Private Structure __colorHelper

        ReadOnly __reference As String()
        ReadOnly __clProfiles As ColorMgr
        ReadOnly __refMaps As Dictionary(Of String, String())

        Sub New(model As Analysis.BestHit, mgr As ColorMgr)
            __reference = model.hits.ToArray(Function(x) x.QueryName)
            __clProfiles = mgr

            Dim LQuery = (From x As HitCollection
                          In model.hits
                          Select From hit As Hit
                                 In x.Hits
                                 Select hit.HitName,
                                     x.QueryName).IteratesALL
            __refMaps = (From x In LQuery
                         Select x
                         Group x By x.HitName Into Group) _
                              .ToDictionary(Function(x) x.HitName,
                                            Function(x) x.Group.ToArray(Function(o) o.QueryName))
        End Sub

        ''' <summary>
        ''' 找到reference，然后就可以得到颜色了
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function GetColor(a As GeneBrief, b As GeneBrief) As Color
            If Not a Is Nothing AndAlso
                Array.IndexOf(__reference, a.Synonym) > -1 Then
                Return __clProfiles.GetEntityColor(a.Synonym)
            ElseIf Not b Is Nothing AndAlso
                Array.IndexOf(__reference, b.Synonym) > -1 Then
                Return __clProfiles.GetEntityColor(b.Synonym)
            Else
                Dim query As String

                If a Is Nothing AndAlso b Is Nothing Then
                    Return __clProfiles.Default
                End If

                If a Is Nothing Then
                    query = b.Synonym
                Else
                    query = a.Synonym
                End If

                query = __refMaps(query).First

                Return __clProfiles.GetEntityColor(query)
            End If
        End Function
    End Structure

    ''' <summary>
    ''' Convert data model <see cref="DeviceModel"/> to drawing object model <see cref="DrawingModel"/>
    ''' </summary>
    ''' <param name="path">The json file path of the drawing data model <see cref="DeviceModel"/></param>
    ''' <param name="style">The link line style.(假若设置了这个参数的话，就会将模型里面的数据给覆盖掉)</param>
    ''' <returns></returns>
    Public Function GetDrawsModel(path As String, Optional style As LineStyles = LineStyles.NotSpecific) As DrawingModel
        Dim model As DeviceModel = JsonContract.LoadJsonFile(Of DeviceModel)(path)
        Dim DIR As New Directory(path.ParentPath)
        Dim bbhMeta As Analysis.BestHit =
            DIR.GetFullPath(model.Meta).LoadXml(Of Analysis.BestHit)
        Dim PTT As Dictionary(Of String, PTT) =
            model.PTT.ToDictionary(Function(x) x.Key,
                                   Function(x) TabularFormat.PTT.Load(DIR.GetFullPath(x.Value)))
        Dim height As Integer = model.Size.Height - model.Margin.Height * 2
        Dim width As Integer = model.Size.Width - model.Margin.Width * 2

        height /= PTT.Count

        Dim maps As SlideWindowHandle(Of String)() = model.Orders.CreateSlideWindows(2)
        Dim bbhs As BBHIndex() =
            LinqAPI.Exec(Of BBHIndex) <= From hits As HitCollection
                                         In bbhMeta.hits
                                         Select From tag As SlideWindowHandle(Of String)
                                                In maps
                                                Let o As BBHIndex = IsOrtholog(tag.Elements.First, tag.Elements.Last, hits, bbhMeta.sp)
                                                Where Not o Is Nothing
                                                Select o
        Dim spGroups = (From x As BBHIndex
                        In bbhs
                        Let sp As String = x.Properties("query")
                        Select sp,
                            x
                        Group By sp Into Group)
        Dim h1 As Integer = model.Margin.Height
        Dim h2 As Integer = h1 + height
        Dim links As New List(Of Line)
        Dim genomes As New List(Of GenomeBrief)
        Dim i As int = Scan0
        Dim last As PTT = Nothing
        Dim titles As Dictionary(Of Title) = model.GetTitles(DIR).ToDictionary
        Dim lastsp As String = Nothing
        Dim title As String
        Dim colorMaps As New __colorHelper(bbhMeta, model.GetColors(DIR))

        If style = LineStyles.NotSpecific Then
            style = model.style
        End If
        If style = LineStyles.NotSpecific Then  '由于可能模型里面的定义也是空的，所以在这里还需要再判断一次
            style = LineStyles.Straight
        End If

        For Each buf In spGroups
            Dim sp As String = buf.sp
            Dim hit As String = maps(++i).Elements.Last
            Dim query As PTT = PTT(sp)
            Dim hitBrief As PTT = PTT(hit)

            title = query.Title

            If titles.ContainsKey(sp) Then
                title = titles(sp).Title
            End If

            links += OrthologAPI.FromBBH(
                buf.Group.ToArray(Function(x) x.x),
                query,
                hitBrief,
                AddressOf colorMaps.GetColor,
                h1,
                h2,
                width,
                model.Margin.Width,
                style)
            genomes += New GenomeBrief With {
                .Name = title,
                .Size = query.Size,
                .Y = h1
            }
            h1 += height
            h2 += height
            last = hitBrief
            lastsp = buf.sp
        Next

        title = last.Title

        If titles.ContainsKey(lastsp) Then
            title = titles(lastsp).Title
        End If

        genomes += New GenomeBrief With {
            .Name = title,
            .Size = last.Size,
            .Y = h1
        }

        Return New DrawingModel With {
            .Links = links,
            .size = model.Size,
            .penWidth = model.penWidth,
            .briefs = genomes,
            .margin = model.Margin
        }
    End Function

    ''' <summary>
    ''' 空值表示没有同源关系
    ''' </summary>
    ''' <param name="query">基因组标识符</param>
    ''' <param name="hit">基因组标识符</param>
    ''' <param name="hits"></param>
    ''' <param name="hitsTag">基因组标识符</param>
    ''' <returns></returns>
    Public Function IsOrtholog(query As String, hit As String, hits As HitCollection, hitsTag As String) As BBHIndex
        Dim qsp As String = query

        query = hits.__getName(hitsTag, query)
        hit = hits.__getName(hitsTag, hit)

        If String.IsNullOrEmpty(query) OrElse String.IsNullOrEmpty(hit) Then
            Return Nothing
        Else
            Return New BBHIndex With {
                .QueryName = query,
                .HitName = hit,
                .Properties = New Dictionary(Of String, String) From {
                    {NameOf(query), qsp}
                }
            }
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="hits"></param>
    ''' <param name="hitsTag"></param>
    ''' <param name="query">基因组标识符</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Private Function __getName(hits As HitCollection, hitsTag As String, query As String) As String
        If String.Equals(query, hitsTag) Then
            Return hits.QueryName
        Else
            Dim hitX As Hit = hits.GetHitByTagInfo(query)

            If hitX Is Nothing Then
                Return Nothing
            Else
                Return hitX.HitName
            End If
        End If
    End Function
End Module
