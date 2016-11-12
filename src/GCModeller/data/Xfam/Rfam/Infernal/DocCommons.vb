#Region "Microsoft.VisualBasic::72825b61fe1b8cfd796b0ed33667ab50, ..\GCModeller\analysis\Xfam\Rfam\Infernal\DocCommons.vb"

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

Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.Xfam.Rfam.Infernal.cmsearch
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Infernal

    Public MustInherit Class STDOUT : Inherits ClassObject

        Public Property version As String
        ''' <summary>
        ''' query sequence file
        ''' </summary>
        ''' <returns></returns>
        Public Property query As String
        ''' <summary>
        ''' target CM database
        ''' </summary>
        ''' <returns></returns>
        Public Property CM As String
    End Class

    Public MustInherit Class IHit : Inherits Contig
        <XmlAttribute> Public Property start As Long
        <XmlAttribute> Public Property [end] As Long
        <XmlAttribute> Public Property strand As Char

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(start, [end], strand.GetStrands)
        End Function
    End Class

    ''' <summary>
    ''' 同时兼容cm_scan以及cm_search的数据
    ''' </summary>
    Public Class HitDataRow : Inherits IHit

        Public Property ORF As String

        <Column("ORF.strand")>
        Public Property direction As Char
        Public Property distance As Integer

        <Column("loci.describ")>
        Public Property LociDescrib As String

        ''' <summary>
        ''' 通过这个动态属性来兼容cm_scan以及cm_search的输出结果
        ''' </summary>
        ''' <returns></returns>
        <Meta(GetType(String))> Public Property data As Dictionary(Of String, String)

        ''' <summary>
        ''' Rfam编号
        ''' </summary>
        ''' <returns></returns>
        <Ignored> <ScriptIgnore> Public ReadOnly Property RfamAcc As String
            Get
                Dim s As String = Nothing
                Call data.TryGetValue(NameOf(RfamHit.Accession), s)
                Return s
            End Get
        End Property

        Public Function Copy() As HitDataRow
            Dim value As HitDataRow = DirectCast(Me.MemberwiseClone, HitDataRow)
            value.data =
                value.data.ToDictionary(Function(x) x.Key,
                                        Function(x) x.Value)
            Return value
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
