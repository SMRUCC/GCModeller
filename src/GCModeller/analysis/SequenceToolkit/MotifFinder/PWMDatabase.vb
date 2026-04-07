#Region "Microsoft.VisualBasic::df469a7e949e9f14db2c12e370c6c677, analysis\SequenceToolkit\MotifFinder\PWMDatabase.vb"

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

    '   Total Lines: 23
    '    Code Lines: 16 (69.57%)
    ' Comment Lines: 3 (13.04%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (17.39%)
    '     File Size: 888 B


    ' Class PWMDatabase
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: LoadMotifs, OpenReadOnly
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports MotifSet = SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.PWMDatabase

''' <summary>
''' Save motif dataset inside a HDS package file
''' </summary>
Public Class PWMDatabase : Inherits MotifSet

    Public Sub New(s As Stream, Optional is_readonly As Boolean = False)
        MyBase.New(New StreamPack(s, [readonly]:=is_readonly))
    End Sub

    Public Shared Function OpenReadOnly(s As Stream) As PWMDatabase
        Return New PWMDatabase(s, is_readonly:=True)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Shared Function LoadMotifs(s As Stream) As Dictionary(Of String, Probability())
        Return LoadMotifs(New StreamPack(s, [readonly]:=True))
    End Function
End Class

