#Region "Microsoft.VisualBasic::1fc1598a381d7920beea8dfdca32cafa, ..\localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BlastnMapping.vb"

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

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Map(Of String, String)
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace LocalBLAST.Application

    ''' <summary>
    ''' Blastn Mapping for fastaq
    ''' </summary>
    Public Class BlastnMapping : Inherits Contig
        Implements IMap

        ''' <summary>
        ''' The name of the reads query
        ''' </summary>
        ''' <returns></returns>
        <Column("Reads.Query")> Public Property ReadQuery As String Implements IMap.Key
        ''' <summary>
        ''' The name of the reference genome sequence.
        ''' </summary>
        ''' <returns></returns>
        Public Property Reference As String Implements IMap.Maps
        ''' <summary>
        ''' Length of <see cref="ReadQuery"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property QueryLength As Integer
        <Column("Score(bits)")> Public Property Score As Integer
        <Column("Score(Raw)")> Public Property RawScore As Integer
        <Column("E-value")> Public Property Evalue As Double
        ''' <summary>
        ''' Identities(%)
        ''' </summary>
        ''' <returns></returns>
        <Column("Identities(%)")> Public Property Identities As Double
        <Column("Identities")> Public Property IdentitiesFraction As String
            Get
                Return _identitiesFraction
            End Get
            Set(value As String)
                _identitiesFraction = value
                If Not String.IsNullOrEmpty(value) Then
                    Dim Tokens As String() = value.Replace("\", "/").Split("/"c)
                    If Tokens.Count > 1 Then
                        __identitiesFraction = Math.Abs(Val(Tokens(Scan0) - Val(Tokens(1))))
                    Else
                        __identitiesFraction = Integer.MaxValue
                    End If
                Else
                    __identitiesFraction = Integer.MaxValue
                End If
            End Set
        End Property

        Dim _identitiesFraction As String
        Dim __identitiesFraction As Integer

        ''' <summary>
        ''' Gaps(%)
        ''' </summary>
        ''' <returns></returns>
        <Column("Gaps(%)")> Public Property Gaps As String
        <Column("Gaps")> Public Property GapsFraction As String

#Region "Public Property Strand As String"

        <Ignored> Public ReadOnly Property QueryStrand As Strands
        ''' <summary>
        ''' 在进行装配的时候是以基因组上面的链方向以及位置为基准的
        ''' </summary>
        ''' <returns></returns>
        <Ignored> Public ReadOnly Property ReferenceStrand As Strands

        Dim _strand As String

        Public Property Strand As String
            Get
                Return _strand
            End Get
            Set(value As String)
                _strand = value

                If String.IsNullOrEmpty(value) Then
                    Me._QueryStrand = Strands.Unknown
                    Me._ReferenceStrand = Strands.Unknown
                    Return
                End If

                Dim Tokens As String() = value.Split("/"c)
                Me._QueryStrand = GetStrand(Tokens(Scan0))
                Me._ReferenceStrand = GetStrand(Tokens(1))
            End Set
        End Property
#End Region

        <Column("Left(Query)")> Public Property QueryLeft As Integer
        <Column("Right(Query)")> Public Property QueryRight As Integer
        <Column("Left(Reference)")> Public Property ReferenceLeft As Integer
        <Column("Right(Reference)")> Public Property ReferenceRight As Integer

        'Public Property Lambda As Double
        'Public Property K As Double
        'Public Property H As Double

        '<Column("Lambda(Gapped)")> Public Property Lambda_Gapped As Double
        '<Column("K(Gapped)")> Public Property K_Gapped As Double
        '<Column("H(Gapped)")> Public Property H_Gapped As Double

        '<Column("Effective Search Space")> Public Property EffectiveSearchSpaceUsed As String

        ''' <summary>
        ''' Unique?(这个属性值应该从blastn日志之中导出mapping数据的时候就执行了的)
        ''' </summary>
        ''' <returns></returns>
        <Column("Unique?")> Public Property Unique As Boolean
        <Column("FullLength?")> Public ReadOnly Property AlignmentFullLength As Boolean
            Get
                Return QueryLeft = 1 AndAlso QueryLength = QueryRight
            End Get
        End Property

        ''' <summary>
        ''' Perfect?
        ''' </summary>
        ''' <returns></returns>
        <Column("Perfect?")> Public ReadOnly Property PerfectAlignment As Boolean
            Get
                ' Explicit conditions
                Return (Identities = 100.0R AndAlso __identitiesFraction <= 3) AndAlso Val(Gaps) = 0R
            End Get
        End Property

        <Meta(GetType(String))>
        Public Property Extensions As Dictionary(Of String, String)

        ''' <summary>
        ''' 不存在的键名会返回空值
        ''' </summary>
        ''' <param name="key$"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property Data(key$) As String
            Get
                If Extensions.ContainsKey(key) Then
                    Return Extensions(key)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Me.ReadQuery} //{MappingLocation.ToString}"
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(ReferenceLeft, ReferenceRight, ReferenceStrand)
        End Function
    End Class
End Namespace
