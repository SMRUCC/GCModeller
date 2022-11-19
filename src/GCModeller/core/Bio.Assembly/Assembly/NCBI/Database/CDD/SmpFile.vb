#Region "Microsoft.VisualBasic::54f646efedf00e04aa093c9c5d05fca2, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\CDD\SmpFile.vb"

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

    '   Total Lines: 134
    '    Code Lines: 91
    ' Comment Lines: 21
    '   Blank Lines: 22
    '     File Size: 4.56 KB


    '     Class SmpFile
    ' 
    '         Properties: CommonName, Describes, ID, Length, Name
    '                     SequenceData, Title
    ' 
    '         Function: __contactLines, EXPORT, Load, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.CDD

    ''' <summary>
    ''' The file data structrue description of each domain smp description data before the CDD database compilation operation.
    ''' (在对CDD数据库编译之前，每一个结构域对象的单独的数据文件格式)
    ''' </summary>
    ''' <remarks></remarks>
    <XmlType("cdd.smp", Namespace:="http://GCModeller.org/Data/CDD/smp")>
    Public Class SmpFile

#Region "Public Property & Constants"

        Implements INamedValue
        Implements IPolymerSequenceModel

        <XmlAttribute>
        Public Property ID As String
        <XmlElement>
        Public Property Title As String
        <XmlElement>
        Public Property Describes As String

        ''' <summary>
        ''' The sequence data of this conserved structure domain.
        ''' (这个保守的结构域的氨基酸分子序列数据)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlText>
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
        ''' <summary>
        ''' UniqueId
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute>
        Public Overridable Property Name As String Implements INamedValue.Key
        <XmlAttribute("name")>
        Public Property CommonName As String

        Public ReadOnly Property Length As Integer
            Get
                Return Len(SequenceData)
            End Get
        End Property

        Const TAGID_REGX As String = "tag id \d+"
        Const DESCR_REGX As String = "title "".+?""},"
        Const SEQDT_REGX As String = "seq-data .+? ""[A-Z]+"""

#End Region

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", ID, Title)
        End Function

        ''' <summary>
        ''' 从一个smp文件之中导出一个FASTA序列数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EXPORT() As FastaSeq
            Dim Title As String = $"{ID}/1-2 {ID}.1 {Name}.1;{CommonName};"
            Dim fasta As New FastaSeq With {
                .SequenceData = SequenceData,
                .Headers = New String() {Title}
            }

            Return fasta
        End Function

        Public Shared Widening Operator CType(path As String) As SmpFile
            Dim text As String = __contactLines(IO.File.ReadAllLines(path))
            Dim SmpFile As New SmpFile With {
                .Id = CInt(Val(Regex.Match(text, TAGID_REGX).Value.Split.Last))
            }
            Dim sTemp As String = Regex.Match(text, DESCR_REGX).Value
            sTemp = Mid(sTemp, 8, Len(sTemp) - 10)

            Dim p As Integer = InStr(sTemp, ". ")
            Dim tokens As String()
            If p > 0 Then
                tokens = Strings.Split(Mid(sTemp, 1, Length:=p - 1), ",")
            Else
                tokens = Strings.Split(sTemp, ",")
            End If

            SmpFile.Name = tokens(0)
            SmpFile.CommonName = tokens(1).Trim
            SmpFile.Title = tokens(2).Trim

            If p > 0 Then
                SmpFile.Describes = Mid(sTemp, p + 2).Trim
                SmpFile.Describes = If(
                    String.IsNullOrEmpty(SmpFile.Describes),
                    SmpFile.Title,
                    SmpFile.Describes)
            Else
                SmpFile.Describes = SmpFile.Title
            End If

            SmpFile.SequenceData = Regex.Match(text, SEQDT_REGX).Value
            SmpFile.SequenceData = Mid(SmpFile.SequenceData, 18) _
                .Replace("""", String.Empty)

            Return SmpFile
        End Operator

        Public Shared Function Load(path As String) As SmpFile
            Return CType(path, SmpFile)
        End Function

        Private Shared Function __contactLines(Data$()) As String
            Dim sb As New StringBuilder(512 * 1024)

            For Each line As String In Data.Select(AddressOf Trim)
                If String.Equals(line, "intermediateData {") Then
                    Exit For
                Else
                    Call sb.Append(line)
                End If
            Next

            Return sb.ToString
        End Function
    End Class
End Namespace
