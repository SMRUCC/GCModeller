#Region "Microsoft.VisualBasic::58b9d80d5df440263c24ea4f07d3bae7, models\BioCyc\Models\Files\proteins.vb"

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

    '   Total Lines: 58
    '    Code Lines: 31 (53.45%)
    ' Comment Lines: 18 (31.03%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (15.52%)
    '     File Size: 2.47 KB


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

''' <summary>
''' The class of all proteins is divided into two subclasses: protein complexes and polypeptides.
''' A polypeptide is a single amino acid chain produced from a single gene. A protein
''' complex is a multimeric aggregation of more than one polypeptide subunit. A protein
''' complex may in some cases have another protein complex as a component. Many of the
''' slots that are applicable to Proteins are also applicable to members of the RNA class.
''' (本类型的对象会枚举所有的Component对象的UniqueID)
''' </summary>
''' <remarks>
''' Protein表对象和ProtLigandCplxe表对象相比较：
''' Protein表中包含有所有类型的蛋白质对象，而ProtLigandCplxe则仅包含有蛋白质和小分子化合物配合的之后所形成的复合物，
''' 所以基因的产物在ProtLigandCplxe表中是无法找到的
''' </remarks>
<Xref("proteins.dat")>
Public Class proteins : Inherits Model

    <AttributeField("DBLINKS")>
    Public Property db_xrefs As String()

    ''' <summary>
    ''' the source gene id that make translation to this protein. 
    ''' The gene's UniqueId that indicated that which gene codes this polypeptide.
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

    <AttributeField("COMPONENTS")>
    Public Property components As String()
    Public Property protseq As String

    <AttributeField("UNMODIFIED-FORM")>
    Public Property unmodified_form As String

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
