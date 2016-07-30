#Region "Microsoft.VisualBasic::3b9bc0ecef073aea46ef2477e16ec2dc, ..\GCModeller\analysis\ProteinTools\Interactions.BioGRID\FileSystem.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Public Module FileSystem

    <Extension>
    Public Iterator Function LoadAllmiTab(path As String) As IEnumerable(Of ALLmitab)
        For Each line As String In path.IterateAllLines.Skip(1)
            Dim tokens As String() = line.Split(Text.ASCII.TAB)
            Dim i As Pointer = 0

            Yield New ALLmitab With {
                .A = tokens(0),
                .B = tokens(1),
                .AltA = tokens(2),
                .AltB = tokens(3),
                .AliasA = tokens(4),
                .AliasB = tokens(5),
                .IDM = tokens(6),
                .Author = tokens(7),
                .Publication = tokens(8),
                .TaxidA = tokens(9),
                .TaxidB = tokens(10),
                .InteractType = tokens(11),
                .Database = tokens(12),
                .uid = tokens(13),
                .Confidence = tokens(14)
            }
        Next
    End Function
End Module

