#Region "Microsoft.VisualBasic::3434d7d1fbcc2afc3535111f0657027b, data\RCSB PDB\PDB\PDB.vb"

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

    '   Total Lines: 149
    '    Code Lines: 98 (65.77%)
    ' Comment Lines: 29 (19.46%)
    '    - Xml Docs: 93.10%
    ' 
    '   Blank Lines: 22 (14.77%)
    '     File Size: 5.04 KB


    ' Class PDB
    ' 
    '     Properties: AtomStructures, Author, Compound, crystal1, DbRef
    '                 Experiment, Header, Helix, Het, Journal
    '                 Keywords, Master, MaxSpace, MinSpace, NUMMDL
    '                 Origin1, Origin2, Origin3, Remark, Revisions
    '                 Scale1, Scale2, Scale3, seqadv, Sequence
    '                 Sheet, Source, Title
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GenericEnumerator, (+2 Overloads) Load, Parse
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords

Public Class PDB : Implements Enumeration(Of Atom)

    Public Const REGEX_HEAD As String = "[A-Z]+\s+(\d+)?\s"

    Public Property Header As Header
    Public Property Title As Title
    Public Property Compound As Compound
    Public Property Source As Source
    Public Property Keywords As Keywords.Keywords
    Public Property Experiment As ExperimentData
    Public Property Author As Author
    Public Property Journal As Journal
    Public Property Remark As Remark
    Public Property Sequence As Sequence
    Public Property Revisions As Revision
    Public Property DbRef As DbReference
    Public Property crystal1 As CRYST1
    Public Property Origin1 As ORIGX123
    Public Property Origin2 As ORIGX123
    Public Property Origin3 As ORIGX123
    Public Property Scale1 As SCALE123
    Public Property Scale2 As SCALE123
    Public Property Scale3 As SCALE123

    ''' <summary>
    ''' number of models inside current pdb file
    ''' </summary>
    ''' <returns></returns>
    Public Property NUMMDL As NUMMDL

    Public Property Het As Het
    Public Property HetName As HetName
    Public Property Formula As Formula

    Public Property Helix As Helix
    Public Property Sheet As Sheet
    Public Property Links As Link
    Public Property seqadv As SEQADV

    Public Property Master As Master

    ''' <summary>
    ''' Populate out the multiple structure models inside current pdb data file
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AtomStructures As IEnumerable(Of Atom)
        Get
            Return _atomStructuresData.Values
        End Get
    End Property

    Default Public ReadOnly Property Model(id As String) As Atom
        Get
            Return _atomStructuresData.TryGetValue(id)
        End Get
    End Property

    Default Public ReadOnly Property Model(i As Integer) As Atom
        Get
            Return _atomStructuresData.Values(i)
        End Get
    End Property

    ''' <summary>
    ''' There are multiple model inside a pdb file, start with ``MODEL`` and end with ``ENDMDL``.
    ''' </summary>
    Friend _atomStructuresData As New Dictionary(Of String, Atom)

    Public ReadOnly Property MaxSpace As Keywords.Point3D
        Get
            Dim all = AtomStructures.Select(Function(m) m.Atoms).IteratesALL.ToArray
            Dim xmax = (From atom As AtomUnit In all Select atom.Location.X).Max
            Dim ymax = (From atom As AtomUnit In all Select atom.Location.Y).Max
            Dim zmax = (From atom As AtomUnit In all Select atom.Location.Z).Max

            Return New Point3D With {
                .X = xmax,
                .Y = ymax,
                .Z = zmax
            }
        End Get
    End Property

    Public ReadOnly Property MinSpace As Keywords.Point3D
        Get
            Dim all = AtomStructures.Select(Function(m) m.Atoms).IteratesALL.ToArray
            Dim xmin = (From atom As AtomUnit In all Select atom.Location.X).Min
            Dim ymin = (From atom As AtomUnit In all Select atom.Location.Y).Min
            Dim zmin = (From atom As AtomUnit In all Select atom.Location.Z).Min

            Return New Point3D With {
                .X = xmin,
                .Y = ymin,
                .Z = zmin
            }
        End Get
    End Property

    Protected Friend Sub New()
    End Sub

    ''' <summary>
    ''' 加载一个蛋白质的三维空间结构的数据文件
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Load(path As String) As PDB
        Return Parser.Load(path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)).FirstOrDefault
    End Function

    ''' <summary>
    ''' Parse the given text content as pdb data
    ''' </summary>
    ''' <param name="text"></param>
    ''' <returns></returns>
    Public Shared Function Parse(text As String) As PDB
        Dim str As New MemoryStream(Encoding.UTF8.GetBytes(text))
        Dim pdb As PDB = Parser.Load(str).FirstOrDefault
        Return pdb
    End Function

    ''' <summary>
    ''' Load multiple pdb molecules from a given text stream data
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Load(s As Stream) As IEnumerable(Of PDB)
        Return Parser.Load(s)
    End Function

    Public Overloads Shared Widening Operator CType(path As String) As PDB
        Return PDB.Load(path)
    End Operator

    Public Iterator Function GenericEnumerator() As IEnumerator(Of Atom) Implements Enumeration(Of Atom).GenericEnumerator
        For Each model As Atom In _atomStructuresData.Values
            Yield model
        Next
    End Function
End Class
