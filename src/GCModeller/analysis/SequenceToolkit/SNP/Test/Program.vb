#Region "Microsoft.VisualBasic::ed56d12e10bdd3121cd87377426b31f1, GCModeller\analysis\SequenceToolkit\SNP\Test\Program.vb"

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

    '   Total Lines: 21
    '    Code Lines: 11
    ' Comment Lines: 2
    '   Blank Lines: 8
    '     File Size: 883 B


    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program

    Sub Main()

        ' Call LANS.SystemsBiology.AnalysisTools.ComparativeGenomics.gwANI.gwANI.calculate_and_output_gwani("F:\Sequence-Patterns-Toolkit\data\gwANI\test.txt")
        'Call LANS.SystemsBiology.AnalysisTools.ComparativeGenomics.gwANI.gwANI.fast_calculate_gwani("F:\Sequence-Patterns-Toolkit\data\gwANI\test.txt")


        Dim ss = "%s+%s+%s".xFormat <= {"sd", "98", "00"}

        Call New FastaFile("F:\Sequence-Patterns-Toolkit\data\SNP\test.txt").ScanSNPs(0).GetJson.SaveTo("F:\Sequence-Patterns-Toolkit\data\SNP\test.args.json")


        Dim result = SNPScan.ScanRaw("F:\Sequence-Patterns-Toolkit\data\SNP\LexA.fasta")
    End Sub
End Module
