#Region "Microsoft.VisualBasic::d9e4adb84d67c711af2cc64df0c30556, analysis\ProteinTools\Interactions.BioGRID\FileSystem.vb"

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

    ' Module FileSystem
    ' 
    '     Function: LoadAllmiTab, LoadIdentifiers, LoadOspreyDataset, SkipHeader
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
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

    <Extension>
    Public Iterator Function LoadIdentifiers(path As String) As IEnumerable(Of IDENTIFIERS)
        For Each line As String In path.IterateAllLines.SkipHeader
            Dim tokens As String() = line.Split(Text.ASCII.TAB)

            Yield New IDENTIFIERS With {
                .BIOGRID_ID = tokens(0),
                .IDENTIFIER_VALUE = tokens(1),
                .IDENTIFIER_TYPE = tokens(2),
                .ORGANISM_OFFICIAL_NAME = tokens(3)
            }
        Next
    End Function

    <Extension>
    Private Function SkipHeader(source As IEnumerable(Of String)) As IEnumerable(Of String)
        Dim n As Integer, lines As Integer

        For Each line As String In source
            If Not String.IsNullOrEmpty(line) AndAlso
                Regex.Match(line, "-+").Value = line Then
                n += 1
            End If

            If n = 3 Then
                Exit For
            End If

            lines += 1
        Next

        Return source.Skip(lines + 3)
    End Function

    <Extension>
    Public Iterator Function LoadOspreyDataset(path As String) As IEnumerable(Of OSPREY_DATASET)
        For Each line As String In path.IterateAllLines.Skip(1)
            Dim tokens As String() = line.Split(Text.ASCII.TAB)

            Yield New OSPREY_DATASET With {
                .GeneA = tokens(0),
                .GeneB = tokens(1),
                .ScreenNameA = tokens(2),
                .ScreenNameB = tokens(3),
                .ExperimentalSystem = tokens(4),
                .Source = tokens(5),
                .PubmedID = tokens(6)
            }
        Next
    End Function
End Module
