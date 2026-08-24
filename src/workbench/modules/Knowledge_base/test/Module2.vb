#Region "Microsoft.VisualBasic::b6bf750d748a1f3c87e97185b9bc46eb, modules\Knowledge_base\test\Module2.vb"

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

    '   Total Lines: 20
    '    Code Lines: 12 (60.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (40.00%)
    '     File Size: 641 B


    ' Module Module2
    ' 
    '     Sub: Main1
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.BITS

Module Module2

    Sub Main1()
        Dim doc = "C:\Users\Administrator\Downloads\livertox_NBK547852\Pegcetacoplan.nxml".LoadXml(Of BookPartWrapper)

        Call Console.WriteLine(doc.GetXml)

        doc = "C:\Users\Administrator\Downloads\livertox_NBK547852\Acitretin.nxml".LoadXml(Of BookPartWrapper)

        Call Console.WriteLine(doc.GetXml)

        doc = "C:\Users\Administrator\Downloads\livertox_NBK547852\Ampicillin.nxml".LoadXml(Of BookPartWrapper)

        Dim cites = doc.GetCitations.ToArray

        Pause()
    End Sub
End Module

