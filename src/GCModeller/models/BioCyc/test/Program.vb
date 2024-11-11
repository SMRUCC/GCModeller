﻿#Region "Microsoft.VisualBasic::ed5821c970c8da2cb46dae1694732179, models\BioCyc\test\Program.vb"

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

'   Total Lines: 26
'    Code Lines: 19 (73.08%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 7 (26.92%)
'     File Size: 746 B


' Module Program
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports SMRUCC.genomics.Data.BioCyc

Module Program
    Sub Main(args As String())

        Using file As Stream = "F:\ecoli\28.1\data\proteins.dat".Open
            Dim data = AttrDataCollection(Of proteins).LoadFile(file)

            Pause()
        End Using

        Using file As Stream = "F:\ecoli\28.1\data\genes.dat".Open
            Dim data = AttrDataCollection(Of genes).LoadFile(file)

            Pause()
        End Using

        Using file As Stream = "F:\ecoli\28.1\data\reactions.dat".Open
            Dim data = AttrDataCollection(Of reactions).LoadFile(file)

            Pause()
        End Using

        Using file As Stream = "G:\GCModeller\src\GCModeller\models\BioCyc\test\compounds.txt".Open
            Dim data = AttrDataCollection(Of compounds).LoadFile(file)
            Dim xrefs = compounds.GetDbLinks(data.features.First).ToArray

            Pause()
        End Using

        Using file As Stream = "P:\2022_nar\25.5\data\pathways.dat".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim data = AttrDataCollection(Of pathways).LoadFile(file)

            Pause()
        End Using

        Using file As StreamReader = "P:\2022_nar\25.5\data\protein-features.dat".OpenReader
            Dim data = AttrValDatFile.ParseFile(file)

            Pause()
        End Using
    End Sub
End Module
