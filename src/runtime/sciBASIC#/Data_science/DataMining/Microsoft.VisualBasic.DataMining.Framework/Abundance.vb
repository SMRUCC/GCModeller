﻿#Region "Microsoft.VisualBasic::850f05baadcea20dacbadf73ddf1f79b, Data_science\DataMining\Microsoft.VisualBasic.DataMining.Framework\Abundance.vb"

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

    ' Module Abundance
    ' 
    '     Function: RelativeAbundances
    ' 
    ' Interface ISample
    ' 
    '     Properties: Samples
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Public Module Abundance

    ''' <summary>
    ''' x除以最大的值就是相对丰度
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function RelativeAbundances(Of T As ISample)(source As IEnumerable(Of T)) As IEnumerable(Of T)
        Dim array As T() = source.ToArray
        Dim allTags As String() = array _
            .Select(Function(x) x.Samples.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim max As Dictionary(Of String, Double) = (
            From tag As String
            In allTags
            Select mxVal = array _
                .Where(Function(x) x.Samples.ContainsKey(tag)) _
                .Select(Function(x) x.Samples(tag)).Max,
                tag).ToDictionary(Function(x) x.tag,
                                  Function(x) x.mxVal)
        For Each x As T In array
            x.Samples = (From tag As String
                         In allTags
                         Select tag,
                             value = x.Samples.TryGetValue(tag) / max(tag)) _
                             .ToDictionary(Function(xx) xx.tag,
                                           Function(xx) xx.value)
            Yield x
        Next
    End Function
End Module

Public Interface ISample : Inherits INamedValue

    Property Samples As Dictionary(Of String, Double)
End Interface
