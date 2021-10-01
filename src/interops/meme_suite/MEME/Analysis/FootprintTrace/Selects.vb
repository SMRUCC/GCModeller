#Region "Microsoft.VisualBasic::9d2e09fda1d540c77d3ec030eba67c7e, meme_suite\MEME\Analysis\FootprintTrace\Selects.vb"

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

    '     Module Selects
    ' 
    '         Function: (+2 Overloads) [Select]
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Analysis.FootprintTraceAPI

    ''' <summary>
    ''' 从计算出来的footprint数据之中选出motif用来进行聚类操作
    ''' </summary>
    Public Module Selects

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="trace"></param>
        ''' <param name="MEME">MEME DIR</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("")>
        <Extension>
        Public Function [Select](trace As FootprintTrace, MEME As String) As AnnotationModel()
            Dim memeHash As Dictionary(Of AnnotationModel) = AnnotationModel.LoadMEMEOUT(MEME)
            Dim LQuery = (From x As MatchResult
                          In trace.Footprints
                          Select x.Select(memeHash)).ToVector
            Return LQuery
        End Function

        <Extension>
        Public Function [Select](footprints As MatchResult, models As Dictionary(Of AnnotationModel)) As AnnotationModel()
            If footprints.Matches.IsNullOrEmpty Then
                Return Nothing
            Else
                Dim LQuery = (From x As MotifHits In footprints.Matches
                              Let tks As String() = Strings.Split(x.Trace, "::")
                              Let uid As String = $"{tks(Scan0)}.{tks(1)}"
                              Where models.ContainsKey(uid)
                              Select models(uid)).ToArray
                Return LQuery
            End If
        End Function
    End Module
End Namespace
