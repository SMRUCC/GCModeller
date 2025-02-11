#Region "Microsoft.VisualBasic::34470f95eb540b63dd918a7e68b6e081, models\BioCyc\Models\proteins.vb"

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

    '   Total Lines: 44
    '    Code Lines: 31 (70.45%)
    ' Comment Lines: 4 (9.09%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (20.45%)
    '     File Size: 1.51 KB


    ' Class proteins
    ' 
    '     Properties: db_links, db_xrefs, gene, locations, protseq
    ' 
    '     Function: (+2 Overloads) OpenFile, ParseText
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("proteins.dat")>
Public Class proteins : Inherits Model

    <AttributeField("DBLINKS")>
    Public Property db_xrefs As String()

    ''' <summary>
    ''' the source gene id that make translation to this protein
    ''' </summary>
    ''' <returns></returns>
    <AttributeField("GENE")>
    Public Property gene As String

    <AttributeField("LOCATIONS")>
    Public Property locations As String()

    Public ReadOnly Property db_links As DBLink()
        Get
            Return GetDbLinks(db_xrefs).ToArray
        End Get
    End Property

    Public Property protseq As String

    Public Shared Function OpenFile(fullName As String) As AttrDataCollection(Of proteins)
        Using file As Stream = fullName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return AttrDataCollection(Of proteins).LoadFile(file)
        End Using
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function OpenFile(file As Stream) As AttrDataCollection(Of proteins)
        Return AttrDataCollection(Of proteins).LoadFile(file)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function ParseText(data As String) As AttrDataCollection(Of proteins)
        Return AttrDataCollection(Of proteins).LoadFile(New StringReader(data))
    End Function
End Class

