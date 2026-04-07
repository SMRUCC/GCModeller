#Region "Microsoft.VisualBasic::7c4e0f0473a9cb23a05a7e1feafd2b25, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\test\interproReader.vb"

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

    '   Total Lines: 10
    '    Code Lines: 7 (70.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (30.00%)
    '     File Size: 221 B


    ' Module interproReader
    ' 
    '     Sub: Main1
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER.InterPro.Xml

Module interproReader

    Sub Main1()
        Dim data = interprodb.ReadTerms("F:\interpro.xml").ToArray

        Pause()
    End Sub
End Module
