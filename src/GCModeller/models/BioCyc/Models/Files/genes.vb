#Region "Microsoft.VisualBasic::30a7b4cd4d1dfc28058176a1759dd512, models\BioCyc\Models\Files\genes.vb"

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

    '   Total Lines: 48
    '    Code Lines: 33 (68.75%)
    ' Comment Lines: 8 (16.67%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (14.58%)
    '     File Size: 1.71 KB


    ' Class genes
    ' 
    '     Properties: accession1, accession2, db_links, db_xrefs, dnaseq
    '                 product
    ' 
    '     Function: (+2 Overloads) OpenFile, ParseText
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("genes.dat")>
Public Class genes : Inherits Model

    ''' <summary>
    ''' The unique identifier of this gene object in the NCBI genbak database.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' (本基因对象在NCBI Genbak数据库之中的唯一标识符)
    ''' </remarks>
    <AttributeField("ACCESSION-1")>
    Public Property accession1 As String
    <AttributeField("ACCESSION-2")>
    Public Property accession2 As String
    <AttributeField("DBLINKS")>
    Public Property db_xrefs As String()
    <AttributeField("PRODUCT")>
    Public Property product As String

    Public ReadOnly Property db_links As DBLink()
        Get
            Return GetDbLinks(db_xrefs).ToArray
        End Get
    End Property

    Public Property dnaseq As String

    Public Shared Function OpenFile(fullName As String) As AttrDataCollection(Of genes)
        Using file As Stream = fullName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return AttrDataCollection(Of genes).LoadFile(file)
        End Using
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function OpenFile(file As Stream) As AttrDataCollection(Of genes)
        Return AttrDataCollection(Of genes).LoadFile(file)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function ParseText(data As String) As AttrDataCollection(Of genes)
        Return AttrDataCollection(Of genes).LoadFile(New StringReader(data))
    End Function
End Class
