﻿#Region "Microsoft.VisualBasic::47ec604a5471e8b5785f54bec9be58a9, analysis\SequenceToolkit\SNP\SangerSNPs\DefineConstants.vb"

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
    '    Code Lines: 25 (83.33%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (16.67%)
    '     File Size: 962 B


    '     Module DefineConstants
    ' 
    ' 
    ' 
    '     Structure SNPsAln
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SangerSNPs

    Module DefineConstants

        Public Const KS_SEP_SPACE = 0
        Public Const KS_SEP_TAB = 1
        Public Const KS_SEP_LINE = 2
        Public Const KS_SEP_MAX = 2
        Public Const MAXIMUM_NUMBER_OF_ALT_BASES = 30
        Public Const MAX_SAMPLE_NAME_SIZE = 2048
        Public Const DEFAULT_NUM_SAMPLES = 65536
        Public Const PROGRAM_NAME = "snp-sites"
        Public Const FILENAME_MAX = 256
    End Module

    Public Structure SNPsAln
        Public number_of_samples As Integer
        Public number_of_snps As Integer
        Public sequence_names As String()
        Public snp_locations As Integer()
        Public pseudo_reference_sequence As Char()
        Public length_of_genome As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
