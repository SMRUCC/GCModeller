#Region "Microsoft.VisualBasic::fffe9a0cfb156f7e9811b69edb282111, data\Xfam\Pfam\Pipeline\PfamString\PfamString.vb"

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

    '   Total Lines: 150
    '    Code Lines: 97
    ' Comment Lines: 33
    '   Blank Lines: 20
    '     File Size: 6.27 KB


    '     Class PfamString
    ' 
    '         Properties: Description, Domains, HasChouFasmanData, Length, PfamString
    '                     ProteinId
    ' 
    '         Function: get__PfamString, get_PlantTextOutput, GetChouFasmanData, GetDomainData, IsChouFasmanData
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.ProteinModel.ChouFasmanRules

Namespace PfamString

    ''' <summary>
    '''  This data object specific for a protein function protein domain structure.
    ''' </summary>
    ''' <remarks></remarks>
    '''
    <XmlType("PfamString", [Namespace]:="http://gcmodeller.org/tools/sanger-pfam/prot_struct")>
    Public Class PfamString : Implements INamedValue

        <XmlAttribute> <Column("ProteinId")>
        Public Property ProteinId As String Implements INamedValue.Key
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
        <XmlAttribute> <Collection("Pfam-string", "+")>
        Public Property PfamString As String()
        <XmlAttribute> <Collection("Domain.Ids")>
        Public Property Domains As String()

        ''' <summary>
        ''' 是否有<see cref="ChouFasman"></see>
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
        Public Function GetChouFasmanData() As DomainObject()
            Dim ChunkBuffer = (From strData As String
                               In PfamString
                               Let DomainData = getDomainTrace(strData, 1)
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

                If IsChouFasmanData(current.Name) Then
                    Dim Data = current.CopyTo(Of DomainObject)()
                    Data.ProteinId = ProteinId
                    Data.Id_Handle = String.Format("{0}_{1}*{2}_{3}", previous.Name, i - 1, [next].Name, i + 1)

                    Call ChunkList.Add(Data)
                End If

                previous = current
                i += 1
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
            If PfamString.IsNullOrEmpty Then
                Return New ProteinModel.DomainObject() {}
            End If

            Dim LQuery = (From strValue As String In PfamString
                          Where Not Regex.Match(strValue, "\[.+?\]").Success
                          Let DomainData As ProteinModel.DomainObject = getDomainTrace(strValue, Length)
                          Select DomainData).ToArray
            If ordered Then
                LQuery = (From domainData As ProteinModel.DomainObject
                          In LQuery
                          Select domainData
                          Order By domainData.Name, domainData.Position.Left Ascending).ToArray
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
