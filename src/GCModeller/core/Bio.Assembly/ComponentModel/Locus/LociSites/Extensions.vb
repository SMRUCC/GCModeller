#Region "Microsoft.VisualBasic::f7b39d282515c2674880e7ceabe28f84, GCModeller\core\Bio.Assembly\ComponentModel\Locus\LociSites\Extensions.vb"

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

    '   Total Lines: 67
    '    Code Lines: 47
    ' Comment Lines: 7
    '   Blank Lines: 13
    '     File Size: 2.55 KB


    '     Module Extensions
    ' 
    '         Function: Assemble
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.My.JavaScript

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
                          Group x By x.family Into Group

                For Each g In gbt
                    out += g.Group.Assemble(False, gapOffset:=gapOffset)
                Next

                Return out
            End If

            Dim locations As New List(Of Location)
            Dim sitesData As IMotifSite() = sites.ToArray

            Const motif As String = NameOf(motif)

            For Each x As IMotifSite In sitesData
                x.site.Tag = New JavaScriptObject
                x.site.Tag(motif) = x
                locations.Add(x.site)
            Next

            Dim assm As Location() = locations _
                .OrderBy(Function(x) x.left) _
                .FragmentAssembly(gapOffset)

            For Each x As Location In assm
                Dim o As IMotifSite = DirectCast(x.Tag(motif), IMotifSite)

                Call x.Tag.Delete(motif)
                out += New MotifSite With {
                    .Name = o.family,
                    .Site = o.site,
                    .Type = {
                        o.family
                    } _
                    .Join(From s As NamedValue(Of Object) In x.Tag Select DirectCast(DirectCast(s.Value, Location).Tag(motif), IMotifSite).family) _
                    .JoinBy("+")  ' 这里不进行Distinct了，因为这些重复的类型可能还有别的用途，例如数量上面的统计之类的
                }
            Next

            Return out
        End Function
    End Module
End Namespace
