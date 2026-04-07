#Region "Microsoft.VisualBasic::4bb5917be3987b2f3d5b84ad141c6dc9, modules\ExperimentDesigner\ExpressionModel\ExpressionData.vb"

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

    '   Total Lines: 30
    '    Code Lines: 24 (80.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (20.00%)
    '     File Size: 1.11 KB


    ' Module ExpressionData
    ' 
    '     Function: (+2 Overloads) GroupAverage
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Public Module ExpressionData

    <Extension>
    Public Iterator Function GroupAverage(Of T As {New, INamedValue, DynamicPropertyBase(Of Double)})(genes As IEnumerable(Of T), sampleinfo As IEnumerable(Of SampleInfo)) As IEnumerable(Of T)
        Dim groups As New DataAnalysis(sampleinfo)

        For Each gene As T In genes
            Yield gene.GroupAverage(groups)
        Next
    End Function

    <Extension>
    Public Function GroupAverage(Of T As {New, INamedValue, DynamicPropertyBase(Of Double)})(gene As T, groups As DataAnalysis) As T
        Dim data As New Dictionary(Of String, Double)

        For Each group As DataGroup In groups.AsEnumerable
            Call data.Add(group.sampleGroup, group.GetData(gene).Average)
        Next

        Return New T With {
            .Key = gene.Key,
            .Properties = data
        }
    End Function
End Module

