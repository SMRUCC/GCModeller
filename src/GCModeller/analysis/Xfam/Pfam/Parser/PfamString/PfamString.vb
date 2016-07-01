#Region "Microsoft.VisualBasic::650ffb0d43addfdce37639542604abde, ..\GCModeller\analysis\Xfam\Pfam\Parser\PfamString\PfamString.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ProteinModel

Namespace PfamString

    ''' <summary>
    '''  This data object specific for a protein function protein domain structure.
    ''' </summary>
    ''' <remarks></remarks>
    '''
    <XmlType("PfamString", [Namespace]:="http://gcmodeller.org/tools/sanger-pfam/prot_struct")>
    Public Class PfamString : Implements sIdEnumerable

        <XmlAttribute> <Column("ProteinId")>
        Public Property ProteinId As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' The protein sequence length
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> <Column("Length")> Public Property Length As Integer
        <Column("Description")> Public Property Description As String
        ''' <summary>
        ''' 在Pfam结构域的数据之间可能会有ChouFasman方法所计算出来的二级结构的数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> <CollectionAttribute("Pfam-string", "+")>
        Public Property PfamString As String()
        <XmlAttribute> <CollectionAttribute("Domain.Ids")>
        Public Property Domains As String()

        ''' <summary>
        ''' 是否有<see cref="SMRUCC.genomics.SequenceModel.Polypeptides.SecondaryStructure.ChouFasman"></see>
        ''' 的蛋白质二级结构计算数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Ignored> Public ReadOnly Property HasChouFasmanData As Boolean
            Get
                If PfamString.IsNullOrEmpty Then
                    Return False
                End If

                Dim LQuery = (From strData As String
                              In PfamString
                              Where Regex.Match(strData, "\[.+?\]").Success
                              Select 1).ToArray
                Return Not LQuery.IsNullOrEmpty
            End Get
        End Property

        Private Shared Function IsChouFasmanData(strData As String) As Boolean
            Return Regex.Match(strData, "\[.+?\]").Success
        End Function

        ''' <summary>
        ''' 这个方法仅返回ChouFasman的计算结果，每一个计算结果都会被当作为一个独立的蛋白质对象进行MP的计算
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_ChouFasmanData() As DomainObject()
            Dim ChunkBuffer = (From strData As String
                               In PfamString
                               Let DomainData = __getDomainTrace(strData, 1)
                               Select DomainData
                               Order By DomainData.Position.Left Ascending).ToArray
            Dim ChunkList As List(Of DomainObject) = New List(Of DomainObject)
            Dim i As Integer = 1
            Dim previous = ChunkBuffer(0)

            Do While i <= ChunkBuffer.Count - 1
                If i = ChunkBuffer.Count - 1 Then
                    Exit Do
                End If

                Dim current = ChunkBuffer(i)
                Dim [next] = ChunkBuffer(i + 1)

                If IsChouFasmanData(current.Identifier) Then
                    Dim Data = current.CopyTo(Of DomainObject)()
                    Data.ProteinId = ProteinId
                    Data.Id_Handle = String.Format("{0}_{1}*{2}_{3}", previous.Identifier, i - 1, [next].Identifier, i + 1)

                    Call ChunkList.Add(Data)
                End If

                previous = current
                Call i.MoveNext()
            Loop

            Return ChunkList.ToArray
        End Function

        ''' <summary>
        ''' 从<see cref="PfamString"/>属性之中返回Pfam的比对数据，请注意，这个函数不会返回ChouFasman的计算数据；
        ''' 返回的数据可能是经过排序操作的
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDomainData(ordered As Boolean) As ProteinModel.DomainObject()
            If StringHelpers.IsNullOrEmpty(PfamString) Then
                Return New ProteinModel.DomainObject() {}
            End If

            Dim LQuery = (From strValue As String In PfamString
                          Where Not Regex.Match(strValue, "\[.+?\]").Success
                          Let DomainData As ProteinModel.DomainObject = __getDomainTrace(strValue, Length)
                          Select DomainData).ToArray
            If ordered Then
                LQuery = (From domainData As ProteinModel.DomainObject
                          In LQuery
                          Select domainData
                          Order By domainData.Identifier, domainData.Position.Left Ascending).ToArray
            End If

            Return LQuery
        End Function

        Public Function get__PfamString(Optional nullDescribe As String = "") As String
            If PfamString.IsNullOrEmpty Then
                Return nullDescribe
            Else
                Return String.Join("+", PfamString)
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", ProteinId, get__PfamString)
        End Function

        Public Function get_PlantTextOutput() As String
            Dim sBuilder As StringBuilder = New StringBuilder()
            Call sBuilder.AppendLine("ProteinId:   " & ProteinId)
            Call sBuilder.AppendLine("Length:      " & Length)
            Call sBuilder.AppendLine("Description: " & Description)
            Call sBuilder.AppendLine("Pfam-string: " & get__PfamString(nullDescribe:="NULL"))
            Call sBuilder.AppendLine("Domain.Ids:  ")
            Call sBuilder.Append(If(Domains.IsNullOrEmpty, "", String.Join("; ", Domains)))
            Call sBuilder.AppendLine()

            Return sBuilder.ToString
        End Function
    End Class
End Namespace
