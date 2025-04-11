#Region "Microsoft.VisualBasic::b28a425f7f194e6795f4e63f828c5423, data\RCSB PDB\PDB\Keywords\AtomUnit.vb"

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

'   Total Lines: 68
'    Code Lines: 39 (57.35%)
' Comment Lines: 21 (30.88%)
'    - Xml Docs: 95.24%
' 
'   Blank Lines: 8 (11.76%)
'     File Size: 2.35 KB


'     Structure Point3D
' 
'         Function: ToString
' 
'     Class AtomUnit
' 
'         Properties: AA_ID, AA_IDX, Atom, Index, Location
' 
'         Function: InternalParser, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language

Namespace Keywords

    Public Structure Point3D : Implements PointF3D

        Public Property Z As Double Implements PointF3D.Z
        Public Property X As Double Implements Layout2D.X
        Public Property Y As Double Implements Layout2D.Y

        Public Overrides Function ToString() As String
            Return $"[x:{X}. y:{Y}, z:{Z}]"
        End Function
    End Structure

    Public Class AtomUnit

        ''' <summary>
        ''' 氨基酸的名称简写
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AA_ID As String
        ''' <summary>
        ''' 当前的氨基酸分子在Fasta序列之中的残基位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AA_IDX As Integer
        Public Property Index As Integer
        Public Property Atom As String
        Public Property Location As Point3D

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  ---> ({1}) {2}   {3}", Index, AA_ID, Location.ToString, Atom)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s">
        ''' -----------------------------------------------|-----------------
        ''' N   ILE     7      25.289   6.282   7.602  1.00121.47           N
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Shared Function InternalParser(s As String, InternalIndex As Integer) As AtomUnit
            Dim Tokens As String() =
                LinqAPI.Exec(Of String) <= From strToken As String
                                           In s.Split
                                           Where Not String.IsNullOrEmpty(strToken)
                                           Select strToken

            Dim xyz As Point3D

            If Tokens.Length = 2 Then
                ' TER
                Return Nothing
            End If
            If Tokens.Length = 3 Then
                Tokens = {""}.Join(Tokens).ToArray
            Else
                xyz = New Point3D With {
                    .X = Val(Tokens(3)),
                    .Y = Val(Tokens(4)),
                    .Z = Val(Tokens(5))
                }
            End If

            Return New AtomUnit With {
                .Index = InternalIndex,
                .Location = xyz,
                .Atom = Tokens(0),
                .AA_ID = Tokens(1),
                .AA_IDX = Val(Tokens(2))
            }
        End Function
    End Class
End Namespace
