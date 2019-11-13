#Region "Microsoft.VisualBasic::6308d36b04ff5ae896b6dccd19a00001, engine\AutoCAD\GeneticComponents\test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: Main, writeBin
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports GeneticComponents
Imports Microsoft.VisualBasic.Data.IO
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Module Module1

    Sub Main()
        Dim database = UniProtXML.EnumerateEntries(path:="K:\uniprot-taxonomy%3A2.xml")

        Using writer As New BinaryDataWriter("X:/test.bin".Open(, True))
            For Each node In database.CreateDump.Where(Function(n) Not n.Accession.StringEmpty)
                Call writer.writeBin(node)
            Next
        End Using
    End Sub

    <Extension>
    Private Sub writeBin(writer As BinaryDataWriter, node As GeneticNode)
        Call writer.Write(node.ID, BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.Accession, BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.KO, BinaryStringFormat.ByteLengthPrefix)
        Call writer.Write(node.GO.JoinBy("|"), BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.Function, BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.Xref, BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.Nt.Length)
        Call writer.Write(node.Nt)
        Call writer.Write(node.Sequence.Length)
        Call writer.Write(node.Sequence)
    End Sub
End Module
