#Region "Microsoft.VisualBasic::9191db37a2b39ced24d474e20d2bef23, data\RCSB PDB\PDB\Keywords\AtomUnit.vb"

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

'   Total Lines: 89
'    Code Lines: 54 (60.67%)
' Comment Lines: 25 (28.09%)
'    - Xml Docs: 92.00%
' 
'   Blank Lines: 10 (11.24%)
'     File Size: 2.96 KB


'     Structure Point3D
' 
'         Properties: X, Y, Z
' 
'         Constructor: (+1 Overloads) Sub New
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
        Public Property ChianID As String
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
            Dim xyz As Point3D
            Dim t As String() =
                LinqAPI.Exec(Of String) <= From strToken As String
                                           In s.Split
                                           Where Not String.IsNullOrEmpty(strToken)
                                           Select strToken
            If t.Length = 2 Then
                ' TER
                Return Nothing
            End If
            If t.Length = 3 Then
                t = {""}.Join(t).ToArray
            Else
                xyz = New Point3D With {
                    .X = Val(t(4)),
                    .Y = Val(t(5)),
                    .Z = Val(t(6))
                }
            End If

            Return New AtomUnit With {
                .Index = InternalIndex,
                .Location = xyz,
                .Atom = t(0),
                .AA_ID = t(1),
                .AA_IDX = Val(t(3)),
                .ChianID = t(2)
            }
        End Function
    End Class
End Namespace
