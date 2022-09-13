#Region "Microsoft.VisualBasic::ac4ae48f7b8971f09a2fdeecf3473329, GCModeller\analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\SimilarDiscriptions.vb"

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

    '   Total Lines: 53
    '    Code Lines: 17
    ' Comment Lines: 28
    '   Blank Lines: 8
    '     File Size: 2.33 KB


    '     Enum SimilarDiscriptions
    ' 
    '         Close, Distant, DistantlySimilar, ModeratelySimilar, VeryDistant
    '         WeaklySimilar
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' For convenience, we describe levels of sigma-differences for some reference examples (all values mutliplied by 1000)
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SimilarDiscriptions As Integer

        ''' <summary>
        ''' (sigma &lt;= 50; pervasively within species, human vs cow, Lactococcus lactis vs Streptococcus pyogenes).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma <= 50; pervasively within species, human vs cow, Lactococcus lactis vs Streptococcus pyogenes")>
        Close

        ''' <summary>
        ''' (55 &lt;= sigma &lt;= 85; human vs chicken, Escherichia coli vs Haemophilus influenzae, Synechococcus vs Anabaena).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [55, 85]; human vs chicken, Escherichia coli vs Haemophilus influenzae, Synechococcus vs Anabaena")>
        ModeratelySimilar

        ''' <summary>
        ''' (90 &lt;= sigma &lt;= 120; human vs sea urchin, M. genitalium vs M. pneumoniae).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [90, 120]; human vs sea urchin, M. genitalium vs M. pneumoniae")>
        WeaklySimilar

        ''' <summary>
        ''' (125 &lt;= sigma &lt;= 145; human vs Sulfolobus, E. coli vs R. prowazekii, M. jannaschii vs M. thermoautotrophicum).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [125, 145]; human vs Sulfolobus, E. coli vs R. prowazekii, M. jannaschii vs M. thermoautotrophicum")>
        DistantlySimilar

        ''' <summary>
        ''' (150 &lt;= sigma &lt;= 180; human vs Drosophilia, E. coli vs Helicobacter pylori).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [150, 180]; human vs Drosophilia, E. coli vs Helicobacter pylori")>
        Distant

        ''' <summary>
        ''' (sigma >= 190; human vs E. coli, E. coli vs Sulfolobus, M. jannaschii vs Halobacterium).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma >= 190; human vs E. coli, E. coli vs Sulfolobus, M. jannaschii vs Halobacterium")>
        VeryDistant
    End Enum
End Namespace
