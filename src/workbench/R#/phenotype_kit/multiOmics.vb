#Region "Microsoft.VisualBasic::af5845dc5473a3e5f209db3d4ea75a20, R#\phenotype_kit\multiOmics.vb"

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

    '   Total Lines: 55
    '    Code Lines: 49
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 2.32 KB


    ' Module multiOmics
    ' 
    '     Function: getData, omics2DScatterPlot
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("multi_omics")>
Module multiOmics

    <ExportAPI("omics.2D_scatter")>
    Public Function omics2DScatterPlot(x As Object, y As Object,
                                       Optional xlab$ = "X",
                                       Optional ylab$ = "Y",
                                       Optional size As Object = "3000,3000",
                                       Optional padding As Object = "padding: 200px 250px 200px 100px;",
                                       Optional ptSize! = 10,
                                       Optional env As Environment = Nothing) As Object

        If x Is Nothing OrElse y Is Nothing Then
            Return REnv.Internal.debug.stop("data can not be null!", env)
        End If

        Return OmicsScatter2D.Plot(
            omicsX:=getData(x, xlab),
            omicsY:=getData(y, ylab),
            xlab:=xlab,
            ylab:=ylab,
            pointSize:=ptSize,
            size:=InteropArgumentHelper.getSize(size, env, "3000,3000"),
            padding:=InteropArgumentHelper.getPadding(padding, "padding: 200px 250px 200px 100px;")
        )
    End Function

    Private Function getData(x As Object, ByRef label$) As NamedValue(Of Double)()
        Dim type As Type = x.GetType

        If type Is GetType(Dictionary(Of String, Double)) Then
            Return DirectCast(x, Dictionary(Of String, Double)) _
                .Select(Function(gene)
                            Return New NamedValue(Of Double)(gene.Key, gene.Value)
                        End Function) _
                .ToArray
        ElseIf type Is GetType(list) Then
            Return DirectCast(x, list).slots _
                .Select(Function(gene)
                            Return New NamedValue(Of Double)(gene.Key, CDbl(REnv.getFirst(gene.Value)))
                        End Function) _
                .ToArray
        Else
            Return {}
        End If
    End Function
End Module
