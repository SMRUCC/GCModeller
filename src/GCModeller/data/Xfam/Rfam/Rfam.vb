#Region "Microsoft.VisualBasic::9d565d2b73ae5267a625f7f3156a23ef, data\Xfam\Rfam\Rfam.vb"

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

    ' Class Rfamily
    ' 
    '     Properties: Evalue, Hit, Identities, Left, Length
    '                 Location, LocusId, Name, Relates, Rfam
    '                 Right, rLociStrand, SequenceData, Strand
    ' 
    '     Function: __copyTo, __getMappingLoci, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel

''' <summary>
''' 相当于RNA调控因子的数据结构
''' </summary>
Public Class Rfamily : Inherits NucleotideModels.Contig
    Implements IPolymerSequenceModel

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

    Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

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
