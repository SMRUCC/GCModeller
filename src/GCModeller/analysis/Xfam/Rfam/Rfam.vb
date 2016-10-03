#Region "Microsoft.VisualBasic::b949996368d23ac6463b7c04c526133a, ..\GCModeller\analysis\Xfam\Rfam\Rfam.vb"

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

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel

''' <summary>
''' 相当于RNA调控因子的数据结构
''' </summary>
Public Class Rfamily : Inherits NucleotideModels.Contig
    Implements I_PolymerSequenceModel

    ''' <summary>
    ''' 数据库之中得到的匹配记录
    ''' </summary>
    ''' <returns></returns>
    Public Property Hit As String
    Public Property Rfam As String
    Public Property Name As String
    ''' <summary>
    ''' 可以在这里给RNA自定义基因的编号
    ''' </summary>
    ''' <returns></returns>
    Public Property LocusId As String
    Public Property Evalue As Double
    Public Property Identities As Double

#Region “Location on the reference genome.”
    Public Property Left As Integer
    Public Property Right As Integer
    Public Property Strand As String
#End Region

    Public Property Location As String
    Public Property Relates As String()
    Public Property rLociStrand As String

    Public ReadOnly Property Length As Integer
        Get
            Return MappingLocation.FragmentSize
        End Get
    End Property

    Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

    Protected Overrides Function __getMappingLoci() As NucleotideLocation
        Return New NucleotideLocation(Left, Right, Strand)
    End Function

    Public Overrides Function ToString() As String
        Return $"{LocusId}   {MyBase.ToString}"
    End Function

    Protected Friend Function __copyTo(Of T As Rfamily)(site As T) As T
        site.Evalue = Evalue
        site.Hit = Hit
        site.Identities = Identities
        site.Left = Left
        site.Location = Location
        site.LocusId = LocusId
        site.Name = Name
        site.Rfam = Rfam
        site.Right = Right
        site.SequenceData = SequenceData
        site.Strand = Strand
        site.Relates = Relates
        site.rLociStrand = rLociStrand
        Return site
    End Function
End Class
