#Region "Microsoft.VisualBasic::0cfe43b185837ecd6cf350535c0ee400, data\RCSB PDB\PDB\Keywords\AtomUnit.vb"

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

    '   Total Lines: 100
    '    Code Lines: 63 (63.00%)
    ' Comment Lines: 25 (25.00%)
    '    - Xml Docs: 92.00%
    ' 
    '   Blank Lines: 12 (12.00%)
    '     File Size: 3.37 KB


    '     Structure Point3D
    ' 
    '         Properties: X, Y, Z
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: DistanceTo, ToString
    ' 
    '     Class AtomUnit
    ' 
    '         Properties: AA_ID, AA_IDX, Atom, ChianID, Index
    '                     Location
    ' 
    '         Function: InternalParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Correlations

Namespace Keywords

    Public Structure Point3D : Implements PointF3D

        Public Property Z As Double Implements PointF3D.Z
        Public Property X As Double Implements Layout2D.X
        Public Property Y As Double Implements Layout2D.Y

        Sub New(x As Double, y As Double, z As Double)
            _X = x
            _Y = y
            _Z = z
        End Sub

        Sub New(pt As PointF3D)
            Call Me.New(pt.X, pt.Y, pt.Z)
        End Sub

        Public Function DistanceTo(x As Double, y As Double, z As Double) As Double
            Return New Double() {Me.X, Me.Y, Me.Z}.EuclideanDistance({x, y, z})
        End Function

        Public Overrides Function ToString() As String
            Return $"[x:{X}. y:{Y}, z:{Z}]"
        End Function
    End Structure

    ''' <summary>
    ''' the amino acid residue/atom model
    ''' </summary>
    ''' <remarks>
    ''' This type is the keyword level view of the unified atom model: all of the field
    ''' storages are moved into the base class <see cref="Structures.Atom"/>, and the legacy
    ''' property names are kept here as the compatibility aliases, so that the existing
    ''' consumers (``AminoAcid.SequenceGenerator``, ``PDB.MaxSpace``/``MinSpace``, ...) do not
    ''' need any change.
    ''' </remarks>
    Public Class AtomUnit : Inherits Structures.Atom

        ''' <summary>
        ''' 氨基酸的名称简写（残基名）
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' 兼容别名，实际存储于 <see cref="Structures.Atom.ResName"/>。
        ''' </remarks>
        Public Property AA_ID As String
            Get
                Return ResName
            End Get
            Set(value As String)
                ResName = value
            End Set
        End Property

        ''' <summary>
        ''' 当前的氨基酸分子在Fasta序列之中的残基位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' 兼容别名，实际存储于 <see cref="Structures.Atom.ResSeq"/>。
        ''' </remarks>
        Public Property AA_IDX As Integer
            Get
                Return ResSeq
            End Get
            Set(value As Integer)
                ResSeq = value
            End Set
        End Property

        ''' <summary>
        ''' 原子序号（PDB 列 7-11）
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 兼容别名，实际存储于 <see cref="Structures.Atom.Serial"/>。
        ''' </remarks>
        Public Property Index As Integer
            Get
                Return Serial
            End Get
            Set(value As Integer)
                Serial = value
            End Set
        End Property

        ''' <summary>
        ''' 原子名称（PDB 列 13-16）
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 兼容别名，实际存储于 <see cref="Structures.Atom.AtomName"/>。
        ''' </remarks>
        Public Property Atom As String
            Get
                Return AtomName
            End Get
            Set(value As String)
                AtomName = value
            End Set
        End Property

        ''' <summary>
        ''' 链标识符（PDB 列 22）
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 兼容别名，实际存储于 <see cref="Structures.Atom.ChainID"/>。
        ''' 注：该名称存在拼写错误（ChianID），保留仅为向后兼容，新代码请使用
        ''' <see cref="Structures.Atom.ChainID"/>。
        ''' </remarks>
        Public Property ChianID As String
            Get
                Return ChainID
            End Get
            Set(value As String)
                ChainID = value
            End Set
        End Property

        ''' <summary>
        ''' Parse one raw ``ATOM`` record line.
        ''' </summary>
        ''' <param name="s">
        ''' The **raw** record line with the record name prefix, e.g.
        ''' ```
        ''' ATOM      1  N   SER A   1      25.289   6.282   7.602  1.00121.47           N
        ''' ```
        ''' the fixed-column offsets are resolved by <see cref="Structures.PdbLineParser"/>.
        ''' </param>
        ''' <returns>
        ''' Returns Nothing when the coordinate columns can not be parsed, the caller should
        ''' skip such a record line.
        ''' </returns>
        ''' <remarks>
        ''' The legacy implementation split the line by the whitespace characters and then took
        ''' the fields by the token index; that is broken for the fixed-column PDB format: when
        ''' the chain identifier column is empty, the tokens shift left by one and the
        ''' coordinates are read as ``(Y, Z, occupancy)``.
        ''' </remarks>
        Friend Shared Function InternalParser(s As String) As AtomUnit
            Dim atom As New AtomUnit

            If Not Structures.PdbLineParser.ParseLine(s, atom, isHet:=False) Then
                Return Nothing
            End If

            Return atom
        End Function
    End Class
End Namespace
