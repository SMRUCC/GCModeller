#Region "Microsoft.VisualBasic::60244c7f424f01d96c1debed69d2d90b, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\test\Program.vb"

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

    '   Total Lines: 15
    '    Code Lines: 12 (80.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (20.00%)
    '     File Size: 436 B


    ' Module Program
    ' 
    '     Sub: Main, parserTest
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER

Module Program
    Sub Main(args As String())
        Call interproReader.Main1()
        Call parserTest()
        HMMER3.Examples.Example1_BasicUsage()
    End Sub

    Sub parserTest()
        Dim list = KOFamScan.ParseTable("G:\GCModeller\src\GCModeller\analysis\SequenceToolkit\Pfam-HMM\kofamscan.txt".OpenReadonly).ToArray

        Pause()
    End Sub
End Module
