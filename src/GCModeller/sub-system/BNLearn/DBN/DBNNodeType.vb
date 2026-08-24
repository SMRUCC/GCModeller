#Region "Microsoft.VisualBasic::8b9de32e2c5447455d288295b5d7315a, sub-system\BNLearn\DBN\DBNNodeType.vb"

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

    '   Total Lines: 17
    '    Code Lines: 7 (41.18%)
    ' Comment Lines: 8 (47.06%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 2 (11.76%)
    '     File Size: 682 B


    '     Enum DBNNodeType
    ' 
    '         EffectorMetabolite, Gene, TranscriptionFactor
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DBN

    ' ==================== Enums ====================

    ''' <summary>
    ''' Type of node in the Dynamic Bayesian Network.
    ''' Determines how the node participates in inference and coupling.
    ''' </summary>
    Public Enum DBNNodeType
        ''' <summary>Target gene or operon being regulated. Expression predicted by CPT.</summary>
        Gene
        ''' <summary>Transcription factor (protein or RNA). State provided as evidence from ODEs.</summary>
        TranscriptionFactor
        ''' <summary>Effector metabolite that modulates TF activity. Concentration from ODEs.</summary>
        EffectorMetabolite
    End Enum
End Namespace
