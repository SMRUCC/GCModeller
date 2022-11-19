#Region "Microsoft.VisualBasic::951bf8234602a41e813e540ac8ecfdc2, GCModeller\models\BioCyc\test\Program.vb"

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
    '    Code Lines: 19
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 721 B


    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports System.IO
Imports BioCyc

Module Program
    Sub Main(args As String())

        Using file As Stream = "P:\2022_nar\25.5\data\reactions.dat".Open
            Dim data = AttrDataCollection(Of reactions).LoadFile(file)

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
