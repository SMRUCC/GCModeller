#Region "Microsoft.VisualBasic::3f282da18cf389cee530a19aded42bc8, GCModeller\models\Networks\Microbiome\PathwayProfile\EnrichmentProfiles.vb"

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

    '   Total Lines: 14
    '    Code Lines: 11
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 406 B


    '     Class EnrichmentProfiles
    ' 
    '         Properties: pathway, profile, pvalue, RankGroup
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace PathwayProfile

    Public Class EnrichmentProfiles
        Public Property RankGroup As String
        Public Property pathway As String
        Public Property profile As Double
        Public Property pvalue As Double

        Public Overrides Function ToString() As String
            Return $"{pathway}: {pvalue.ToString("G3")}"
        End Function
    End Class

End Namespace
