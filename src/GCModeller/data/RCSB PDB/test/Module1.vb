#Region "Microsoft.VisualBasic::d8bfb39725e8327977aa618d63ab65ad, data\RCSB PDB\test\Module1.vb"

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

    '   Total Lines: 11
    '    Code Lines: 7 (63.64%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (36.36%)
    '     File Size: 176 B


    ' Module Module1
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Data.RCSB.PDB

Module Module1

    Sub Main()
        Dim pdb As PDB = PDB.Load("E:\GCModeller\src\GCModeller\data\RCSB PDB\test\pdb1l3q.ent.txt")

        Pause()
    End Sub

End Module

