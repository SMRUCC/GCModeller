#Region "Microsoft.VisualBasic::3140f1b868d6d898952d1c220148ef62, GCModeller\visualize\DataVisualizationExtensions\CatalogProfiling\COGCatalogProfiling.vb"

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

    '   Total Lines: 76
    '    Code Lines: 65
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 3.07 KB


    '     Module COGCatalogProfiling
    ' 
    '         Function: COGCatalogProfilingPlot
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace CatalogProfiling

    Public Module COGCatalogProfiling

        <Extension>
        Public Function COGCatalogProfilingPlot(Of T As ICOGCatalog)(
                                           genes As IEnumerable(Of T),
                                   Optional size$ = "2200,2000",
                                   Optional bg$ = "white",
                                   Optional title$ = "COG catalog profiling") As GraphicsData

            Dim COGs As COG.Function = COG.Function.Default
            Dim array As T() = genes.ToArray
            Dim profiling = From c As Char
                            In array _
                                .Select(Function(g) g.Catalog) _
                                .Where(Function(s) Not s.StringEmpty) _
                                .IteratesALL
                            Select c
                            Group c By c Into Count  ' 所有的元素经过分组操作之后都是唯一的
            Dim profiles As New Dictionary(Of String, NamedValue(Of Double)())
            Dim data As New Dictionary(Of String, List(Of NamedValue(Of Double)))
            Dim null% = array.Where(Function(g) g.Catalog.StringEmpty).Count ' 空的分类的基因数目
            Dim profileData = profiling.ToArray
            Dim total% = array.Length / 100
            Dim [class] As NamedValue(Of String)

            For Each catalog In profileData
                [class] = COGs.GetCatalog(CStr(catalog.c))

                If Not data.ContainsKey([class].Name) Then
                    Call data.Add([class].Name, New List(Of NamedValue(Of Double)))
                End If

                Call data([class].Name).Add(
                    New NamedValue(Of Double) With {
                        .Name = $"[{catalog.c}] {[class].Value}",
                        .Value = catalog.Count / total
                    }
                )
            Next

            If Not data.ContainsKey(COG.Function.NotAssigned) Then
                Call data.Add(COG.Function.NotAssigned, New List(Of NamedValue(Of Double)))
            End If

            Call data(COG.Function.NotAssigned).Add(
                New NamedValue(Of Double) With {
                    .Name = "Unknown",
                    .Value = null / total
                }
            )

            For Each catalog In data
                Call profiles.Add(catalog.Key, catalog.Value)
            Next

            Return New CatalogProfiles(profiles).ProfilesPlot(
                axisTitle:="Percentage of catalog genes",
                title:=title,
                bg:=bg,
                size:=size,
                tick:=5,
                removeNotAssign:=False
            )
        End Function
    End Module
End Namespace
