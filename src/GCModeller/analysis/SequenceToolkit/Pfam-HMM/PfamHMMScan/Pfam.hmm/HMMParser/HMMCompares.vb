#Region "Microsoft.VisualBasic::0afff90d2b1c6770c3fc03f5c99a0f8c, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\Pfam.hmm\HMMParser\HMMCompares.vb"

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

    ' Module HMMCompares
    ' 
    '     Function: __char, Equals
    '     Structure __compareHelper
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Equals
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math

Public Module HMMCompares

    ''' <summary>
    ''' 计算HMM模型的相似度
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <param name="cut"></param>
    ''' <returns></returns>
    Public Function Equals(a As HMM, b As HMM, Optional cut As Double = 0.6) As Boolean
        Dim helper As New __compareHelper(cut)
        Dim c As Boolean = helper.Equals(a.COMPO, b.COMPO)
        If Not c Then
            Return False
        End If
        Dim edits As DistResult = LevenshteinDistance.ComputeDistance(a.nodes, b.nodes, AddressOf helper.Equals, AddressOf __char)

        If edits Is Nothing Then
            Return False
        End If

        Return edits.MatchSimilarity >= cut
    End Function

    Private Function __char(n As Node) As Char
        Dim i As Integer = n.Match.MaxIndex
        Return CType(i, AAIndex).ToString.First
    End Function

    Const A As Integer = Asc("A"c)

    Private Structure __compareHelper

        Public threshold As Double

        Sub New(cut As Double)
            threshold = cut
        End Sub

        Public Overloads Function Equals(a As Node, b As Node) As Boolean
            Dim i As Boolean = Correlations.GetPearson(a.Insert, b.Insert) >= threshold
            Dim m As Boolean = Correlations.GetPearson(a.Match, b.Match) >= threshold
            Dim s As Boolean = Correlations.GetPearson(a.StateTransitions, b.StateTransitions) >= threshold

            Return i AndAlso m AndAlso s
        End Function
    End Structure
End Module
