#Region "Microsoft.VisualBasic::3eb639c441dad31d903cc091eb762f0b, ..\GCModeller\core\Bio.Assembly\ComponentModel\Loci.Models\LociSites\Extensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace ComponentModel.Loci

    Public Module Extensions

        ''' <summary>
        ''' 这个函数返回来的位点里面的<see cref="MotifSite.Type"/>信息可以使用``+``分割
        ''' </summary>
        ''' <param name="sites"></param>
        ''' <param name="groupByType">是否在合并拼接之前进行按照类型分组？</param>
        ''' <param name="gapOffset%">默认是不允许跳过gap间隙的</param>
        ''' <returns></returns>
        <Extension>
        Public Function Assemble(sites As IEnumerable(Of IMotifSite), Optional groupByType As Boolean = False, Optional gapOffset% = 0) As IEnumerable(Of IMotifSite)
            Dim out As New List(Of IMotifSite)

            If groupByType Then
                Dim gbt = From x As IMotifSite
                          In sites
                          Select x
                          Group x By x.Type Into Group

                For Each g In gbt
                    out += g.Group.Assemble(False, gapOffset:=gapOffset)
                Next

                Return out
            End If

            Dim locations As New List(Of Location)
            Dim sitesData As IMotifSite() = sites.ToArray

            Const motif As String = NameOf(motif)

            For Each x As IMotifSite In sitesData
                x.Site.Extension = New ExtendedProps
                x.Site.Extension.DynamicHashTable(motif) = x
                locations.Add(x.Site)
            Next

            Dim assm As Location() = locations _
                .OrderBy(Function(x) x.Left) _
                .FragmentAssembly(gapOffset)

            For Each x As Location In assm
                Dim o As IMotifSite = DirectCast(x.Extension.DynamicHashTable(motif), IMotifSite)

                Call x.Extension _
                    .DynamicHashTable _
                    .Properties _
                    .Remove(motif)
                out += New MotifSite With {
                    .Name = o.Name,
                    .Site = o.Site,
                    .Type = {
                        o.Type
                    } _
                    .Join(x.Extension _
                           .DynamicHashTable _
                           .Properties _
                           .Values _
                           .Select(Function(s) DirectCast(DirectCast(s, Location) _
                           .Extension _
                           .DynamicHashTable _
                           .Properties(motif), IMotifSite).Type)) _
                    .JoinBy("+")  ' 这里不进行Distinct了，因为这些重复的类型可能还有别的用途，例如数量上面的统计之类的
                }
            Next

            Return out
        End Function
    End Module
End Namespace
