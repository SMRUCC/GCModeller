#Region "Microsoft.VisualBasic::c244354d52a35fe90803899c3b1466b6, GCModeller\core\Bio.Assembly\ComponentModel\DBLinkBuilder\IdMapping\Document.vb"

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

    '   Total Lines: 36
    '    Code Lines: 31
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1.37 KB


    '     Module Document
    ' 
    '         Function: LoadMappingText, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace ComponentModel.DBLinkBuilder

    <HideModuleName> Public Module Document

        <Extension>
        Public Function Save(maps As SecondaryIDSolver, path As String) As Boolean
            Using output As New StreamWriter(path.Open(, doClear:=True), Encodings.ASCII.CodePage) With {
                .NewLine = ASCII.LF
            }
                For Each map In maps.idMapping
                    Call output.WriteLine($"{map.Key} {map.Value.JoinBy(",")}")
                Next
            End Using

            Return True
        End Function

        Public Function LoadMappingText(handle As String, Optional skip2ndMaps As Boolean = False) As SecondaryIDSolver
            Return handle _
                .LineIterators _
                .DoCall(Function(mapLines)
                            Return SecondaryIDSolver.Create(
                                source:=mapLines.Select(Function(l) l.Split(" "c)),
                                mainID:=Function(a) a(Scan0),
                                secondaryID:=Function(a) a(1).Split(","c),
                                skip2ndMaps:=skip2ndMaps
                            )
                        End Function)
        End Function
    End Module
End Namespace
