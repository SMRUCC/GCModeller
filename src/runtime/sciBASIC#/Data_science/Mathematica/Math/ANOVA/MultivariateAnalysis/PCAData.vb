﻿#Region "Microsoft.VisualBasic::94271ae1918ed479406563de0991b91e, Data_science\Mathematica\Math\ANOVA\MultivariateAnalysis\PCAData.vb"

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

    '   Total Lines: 66
    '    Code Lines: 47 (71.21%)
    ' Comment Lines: 8 (12.12%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 11 (16.67%)
    '     File Size: 2.16 KB


    ' Module PCAData
    ' 
    '     Function: GetPCALoading, GetPCAScore
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Language

''' <summary>
''' get pca result: contribution, score, loading
''' </summary>
Public Module PCAData

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mvar"></param>
    ''' <returns>a dataframe object that contains data field column: ``PC1, PC2, PCx``</returns>
    <Extension>
    Public Function GetPCAScore(mvar As MultivariateAnalysisResult) As DataFrame
        Dim score As New DataFrame
        Dim filesize = mvar.StatisticsObject.YIndexes.Count
        Dim compSize = mvar.Contributions.Count
        Dim labels = mvar.StatisticsObject.YLabels

        If labels Is Nothing OrElse labels.Count = 0 Then
            labels = New ObjectModel.ObservableCollection(Of String)

            For Each i As Integer In mvar.StatisticsObject.YIndexes
                labels.Add(i + 1)
            Next
        End If

        For i = 0 To filesize - 1
            Dim tList = New List(Of Double)()
            For j = 0 To compSize - 1
                tList.Add(mvar.TPreds(j)(i))
            Next
            score.add(labels(i), tList.ToArray)
        Next

        score.rownames = mvar.Contributions _
            .Select(Function(c, i) $"PC{i + 1}") _
            .ToArray

        Return score.Transpose
    End Function

    <Extension>
    Public Function GetPCALoading(mvar As MultivariateAnalysisResult) As DataFrame
        Dim loading As New DataFrame
        Dim metsize = mvar.StatisticsObject.XLabels.Count
        Dim compSize = mvar.Contributions.Count
        Dim labels = mvar.StatisticsObject.XLabels

        For i As Integer = 0 To metsize - 1
            Dim pList = New List(Of Double)()
            For j = 0 To compSize - 1
                pList.Add(mvar.PPreds(j)(i))
            Next
            loading.add(labels(i), pList.ToArray)
        Next

        loading.rownames = mvar.Contributions _
            .Select(Function(c, i) $"PC{i + 1}") _
            .ToArray

        Return loading.Transpose
    End Function
End Module
