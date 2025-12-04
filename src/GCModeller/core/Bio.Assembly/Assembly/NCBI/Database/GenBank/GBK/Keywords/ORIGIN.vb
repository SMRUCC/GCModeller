#Region "Microsoft.VisualBasic::d3a6924f35844e4ce1f9738179dd6e4b, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\ORIGIN.vb"

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

'   Total Lines: 141
'    Code Lines: 78 (55.32%)
' Comment Lines: 42 (29.79%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 21 (14.89%)
'     File Size: 5.27 KB


'     Class ORIGIN
' 
'         Properties: GCSkew, Headers, SequenceData, Size, Title
' 
'         Function: GetEnumerator, GetEnumerator1, GetFeatureSegment, ToFasta, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' This GenBank keyword section stores the sequence data for this database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ORIGIN : Inherits KeyWord
        Implements IAbstractFastaToken
        Implements IEnumerable(Of Char)

        ''' <summary>
        ''' The sequence data that stores in this GenBank database, which can be a genomics DNA sequence, protein sequence or RNA sequence.
        ''' (序列数据，类型可以包括基因组DNA序列，蛋白质序列或者RNA序列)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        ''' <summary>
        ''' The origin nucleic acid sequence contains illegal character in the nt sequence, ignored as character N... 
        ''' for <see cref="SequenceData"/>
        ''' </summary>
        Const InvalidWarns As String = "The origin nucleic acid sequence contains illegal character in the nt sequence, ignored as character N..."

        ''' <summary>
        ''' ``<see cref="SequenceData"/> -> index``
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property [Char](index As Long) As Char
            Get
                Return SequenceData(index)
            End Get
        End Property

        ''' <summary>
        ''' 基因组的大小
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            Get
                Return SequenceData.Length
            End Get
        End Property

        ''' <summary> 
        ''' 获取该Feature位点的序列数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFeatureSegment(feature As Feature) As String
            Dim loci As NucleotideLocation = feature.Location.ContiguousRegion
            Return Me.CutSequenceLinear(loci).SequenceData
        End Function

        Public Overrides Function ToString() As String
            Return SequenceData
        End Function

        ''' <summary>
        ''' 是整条序列的GC偏移
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GCSkew As Double
            Get
                Dim G As Integer = (From ch In Me.SequenceData Where ch = "G"c OrElse ch = "g"c Select 1).Count
                Dim C As Integer = (From ch In Me.SequenceData Where ch = "C"c OrElse ch = "c"c Select 1).Count
                Return (G + C) / (G - C)
            End Get
        End Property

        Public Shared Widening Operator CType(section As String()) As ORIGIN
            Dim sb As New StringBuilder(2048)

            For Each line As String In section
                sb.Append(Mid$(line, 10))
            Next

            Dim trimChars As Char() = LinqAPI.Exec(Of Char) _
                                                            _
                () <= From b As Char In sb.ToString
                      Where b <> " "c
                      Select b

            Return New ORIGIN With {
                .SequenceData = New String(trimChars)
            }
        End Operator

        Public Shared Narrowing Operator CType(ori As ORIGIN) As String
            Return ori.SequenceData
        End Operator

        Public Shared Narrowing Operator CType(obj As ORIGIN) As FastaSeq
            Return obj.ToFasta
        End Operator

        ''' <summary>
        ''' Returns the whole genome sequence which was records in this GenBank database file. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' (返回记录在本Genbank数据库文件之中的全基因组核酸序列)
        ''' </remarks>
        Public Function ToFasta() As FastaSeq
            Dim id As String = gb.Accession.AccessionId
            Dim size As Integer = Strings.Len(SequenceData)
            ' ncbi sequence standard headers
            ' accession_id title
            Dim attrs As String() = {id & " " & title & " " & If(size < 1024, size & "bp", StringFormats.Lanudry(size))}
            Dim seq$ = SequenceData.ToUpper

            Return New FastaSeq(attrs, seq)
        End Function

        Public ReadOnly Property title As String Implements IAbstractFastaToken.title
            Get
                Return gb.Definition.Value
            End Get
        End Property

        Public Property Headers As String() Implements IAbstractFastaToken.headers

        Public Iterator Function GetEnumerator() As IEnumerator(Of Char) Implements IEnumerable(Of Char).GetEnumerator
            For Each ch As Char In SequenceData
                Yield ch
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
