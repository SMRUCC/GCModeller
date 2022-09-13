#Region "Microsoft.VisualBasic::65d335106637a85a46132d6c4cd7241e, GCModeller\core\Bio.Assembly\Assembly\Expasy\NomenclatureDB\NomenclatureDB.vb"

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

    '   Total Lines: 113
    '    Code Lines: 77
    ' Comment Lines: 24
    '   Blank Lines: 12
    '     File Size: 5.79 KB


    '     Class NomenclatureDB
    ' 
    '         Properties: Copyright, Enzymes, Release
    ' 
    '         Function: CreateObject, GetSwissProtEntries, TryExportUniprotFasta
    ' 
    '         Sub: Export
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.Expasy.Database

    ''' <summary>
    ''' ENZYME nomenclature database.(Expasy数据库之中的enzyme.dat注释文件)
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot("ENZYME nomenclature database", Namespace:="http://enzyme.expasy.org/")>
    Public Class NomenclatureDB : Inherits XmlDataModel

        Dim __defHash As Dictionary(Of String, Enzyme)

        <XmlElement("Enzyme", Namespace:="http://code.google.com/p/genome-in-code/expasy")>
        Public Property Enzymes As Enzyme()

        <XmlAttribute("Release", Namespace:="EXPASY/RELEASE_VERSION")>
        Public Property Release As Date

        <XmlText>
        Public Property Copyright As String

        ''' <summary>
        ''' 当目标编号不存在于Expasy数据库之中的时候会返回空值
        ''' </summary>
        ''' <param name="EC"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public ReadOnly Property DataItem(EC As String) As Enzyme
            Get
                If Not __defHash.ContainsKey(EC) Then
                    Return Nothing
                End If
                Return __defHash(EC)
            End Get
        End Property

        ''' <summary>
        ''' 从Expasy数据库之中导出某一种酶分类编号的所有的Uniprot数据库之中的蛋白质编号
        ''' </summary>
        ''' <param name="ECNumber"></param>
        ''' <param name="Strict">是否严格匹配酶分类编号，假若严格匹配的话，则必须要字符串完全相等才会有输出结果，假若不严格匹配，理论上假若所输入的酶分类标号有通配符，即相连着的两个".."符号存在的话，则所有该大分类之下的所有的蛋白酶分子的编号都会被输出</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSwissProtEntries(ECNumber As String, Optional Strict As Boolean = False) As String()
            If (From ch As Char In ECNumber Where ch = "."c Select 1).Count = 2 Then
                If Strict Then
                    Return New String() {}
                End If

                Dim LQuery = (From enz As Enzyme
                              In Enzymes
                              Where InStr(enz.Identification, ECNumber) = 1
                              Select enz.SwissProt).ToArray
                Return LQuery.IteratesALL.Distinct.ToArray
            Else
                Return (From id As String
                        In Enzymes.Take(ECNumber).SwissProt
                        Where Not String.IsNullOrEmpty(id)
                        Select id
                        Distinct).ToArray
            End If
        End Function

        Public Function TryExportUniprotFasta(data As IEnumerable(Of Uniprot.UniprotFasta)) As FASTA.FastaFile
            Dim UniprotIDs As String() = (From enz As Enzyme
                                          In Me.Enzymes
                                          Select enz.SwissProt).IteratesALL.Distinct.ToArray
            Dim LQuery As IEnumerable(Of FASTA.FastaSeq) = From fa As Uniprot.UniprotFasta
                                                             In data.AsParallel
                                                           Where Array.IndexOf(UniprotIDs, fa.UniprotID)
                                                           Select fa
            Return New FASTA.FastaFile(LQuery)
        End Function

        ''' <summary>
        ''' Load the expasy database from the text file.(从文本文件之中加载expasy数据库)
        ''' </summary>
        ''' <param name="File">File path of the expasy database file.(exapsy数据库文件的文件路径)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function CreateObject(File As String) As NomenclatureDB
            Dim strData As String() = Regex.Split(FileIO.FileSystem.ReadAllText(File).Replace("-!-", ""), "^//$", RegexOptions.Multiline)
            Dim DBDescription As String = strData.First
            Dim Enzymes As Enzyme() = (From strLine As String
                                       In strData.Skip(1).AsParallel
                                       Let strLines As String() = (From strItem As String
                                                                   In Strings.Split(strLine, vbLf)
                                                                   Let strL As String = Trim(strItem)
                                                                   Where Not String.IsNullOrEmpty(strL)
                                                                   Select strL).ToArray
                                       Where Not strLines.IsNullOrEmpty
                                       Let Enzyme = Enzyme.__createObjectFromText(strData:=strLines)
                                       Select Enzyme
                                       Order By Enzyme.Identification Ascending).ToArray
            Return New NomenclatureDB With {
                .Enzymes = Enzymes
            }
        End Function

        Public Sub Export(ByRef Classes As csv.Enzyme(), ByRef SwissProt As csv.SwissProt())
            Classes = Enzymes.Select(AddressOf csv.Enzyme.CreateObject).ToArray
            SwissProt = Enzymes _
                .Select(AddressOf csv.SwissProt.CreateObjects) _
                .ToVector
        End Sub
    End Class
End Namespace
