#Region "Microsoft.VisualBasic::9c498c64f6f550df04dbf209ece6e9b3, R#\seqtoolkit\models\Enums.vb"

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

    '   Total Lines: 23
    '    Code Lines: 15 (65.22%)
    ' Comment Lines: 6 (26.09%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (8.70%)
    '     File Size: 447 B


    ' Enum TableTypes
    ' 
    '     BBH, Mapping, SBH, Terms
    ' 
    '  
    ' 
    ' 
    ' 
    ' Enum BBHAlgorithm
    ' 
    '     BHR, HybridBHR, Naive, TaxonomySupports
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline

Public Enum TableTypes
    SBH
    BBH
    ''' <summary>
    ''' blastn mapping of the short reads
    ''' </summary>
    Mapping
    ''' <summary>
    ''' <see cref="RankTerm"/>
    ''' </summary>
    Terms
End Enum

Public Enum BBHAlgorithm
    Naive
    BHR
    <Description("Hybrid-BHR")>
    HybridBHR
    TaxonomySupports
End Enum
