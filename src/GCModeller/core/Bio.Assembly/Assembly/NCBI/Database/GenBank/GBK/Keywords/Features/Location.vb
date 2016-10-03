#Region "Microsoft.VisualBasic::2fa0d0fdd5438daf1e8788c406c6a28d, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\Features\Location.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    Public Class Location : Implements ComponentModel.Loci.Abstract.ILocationSegment

        ''' <summary>
        ''' 对于环状的DNA分子，当某一个特性位点跨越了终点的时候，会有一个这个属性，本属性此时不会为空值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property JoinLocation As RegionSegment

        ''' <summary>
        ''' 这个基因的位置是否在互补链
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Complement As Boolean
        <XmlAttribute> Public Property Locations As RegionSegment()
            Get
                Return lstLocis
            End Get
            Set(value As RegionSegment())
                lstLocis = value

                If Not value.IsNullOrEmpty Then
                    _left = (From loc As RegionSegment
                             In value
                             Select loc.Left).ToArray.Min
                    _right = (From loc As RegionSegment
                              In value
                              Select loc.Right).ToArray.Max
                End If
            End Set
        End Property

        Dim lstLocis As RegionSegment()
        Dim _left, _right As Long

        ''' <summary>
        ''' 假若目标对象是真核生物基因组的话，则可能会因为内含子的原因出现不连续的片段，故而此时的<see cref="Locations"></see>属性会有多个值，这个属性会尝试将连续的区域返回。对于原核生物而言，也可以直接使用这个属性来获取特性位点的在基因组序列之上的位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ContiguousRegion As ComponentModel.Loci.NucleotideLocation
            Get
                Return New ComponentModel.Loci.NucleotideLocation(_left, _right, Complement)
            End Get
        End Property

        Const LOCATION_PAIRED As String = "\d+[.]{2}[>]?\d+"

        Public Shared Widening Operator CType(strData As String) As Location
            Dim LocationComplement As Boolean = InStr(strData, "complement(") > 0
            Dim LQuery As List(Of RegionSegment) =
                (From p As Match In Regex.Matches(strData, LOCATION_PAIRED)
                 Let Tokens = Regex.Split(p.Value, "[.]{2}[>]?")
                 Let Segment As RegionSegment = New RegionSegment With {
                     .Left = Val(Tokens.First),
                     .Right = Val(Tokens.Last)
                 }
                 Select Segment).ToList

            If LQuery.IsNullOrEmpty Then
                Call $"Location is empty!   ""{strData}""".__DEBUG_ECHO
            End If

            Dim JoinLocation As RegionSegment = Nothing

            If InStr(strData, "join(") > 0 Then
                If LocationComplement Then
                    JoinLocation = LQuery.First
                    Call LQuery.RemoveAt(0)
                Else
                    JoinLocation = LQuery.Last
                    Call LQuery.Remove(JoinLocation)
                End If

                Call $"Join location at {JoinLocation.ToString}".__DEBUG_ECHO
            End If

            Dim Location As Location = New Location With {
                .Locations = LQuery.ToArray,
                .Complement = LocationComplement,
                .JoinLocation = JoinLocation
            }

            Return Location
        End Operator

        Public ReadOnly Property HasJoinLocation As Boolean
            Get
                Return Not JoinLocation Is Nothing
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim lst As String() = (From p As RegionSegment
                                   In Locations
                                   Select p.Left & ".." & p.Right).ToArray
            Dim sBuilder = String.Join(", ", lst)

            If Complement Then
                sBuilder = String.Format("complement({0})", sBuilder)
            End If

            If HasJoinLocation Then
                sBuilder &= "  Join with " & JoinLocation.ToString
            End If

            Return sBuilder
        End Function

        Public ReadOnly Property Location As ComponentModel.Loci.Location Implements ComponentModel.Loci.Abstract.ILocationSegment.Location
            Get
                Return ContiguousRegion
            End Get
        End Property

        Public ReadOnly Property UniqueId As String Implements ComponentModel.Loci.Abstract.ILocationSegment.UniqueId
            Get
                Return ToString()
            End Get
        End Property
    End Class

    ''' <summary>
    ''' A site region on the sequence.(序列上面的一个位点)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RegionSegment
        <XmlAttribute> Public Property Left As Long
        <XmlAttribute> Public Property Right As Long

        Public ReadOnly Property RegionLength As Integer
            Get
                Return Math.Abs(Left - Right)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1}", Left, Right)
        End Function
    End Class
End Namespace
