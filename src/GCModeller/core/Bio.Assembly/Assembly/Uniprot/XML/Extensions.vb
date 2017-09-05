#Region "Microsoft.VisualBasic::5cfbf6955a28d38d49b17d2e0f4688f1, ..\core\Bio.Assembly\Assembly\Uniprot\XML\Extensions.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ProteinModel

Namespace Assembly.Uniprot.XML

    Public Module Extensions

        <Extension>
        Public Function ECNumberList(protein As entry) As String()
            Return protein?. _
                protein?. _
                recommendedName?. _
                ecNumber.
                ToArray(Function(ec) ec.value)
        End Function

        <Extension>
        Public Function OrganismScientificName(protein As entry) As String
            If protein.organism Is Nothing Then
                Return ""
            Else
                Return protein.organism.scientificName
            End If
        End Function

        <Extension> Public Function proteinFullName(protein As entry) As String
            If protein Is Nothing OrElse protein.protein Is Nothing Then
                Return ""
            Else
                Return protein.protein.FullName
            End If
        End Function

        <Extension> Public Function ORF(protein As entry) As String
            If protein?.gene Is Nothing OrElse Not protein.gene.HaveKey("ORF") Then
                Return Nothing
            Else
                Return protein.gene.ORF.First
            End If
        End Function

        ''' <summary>
        ''' 获取蛋白质在细胞内的亚细胞定位结果
        ''' </summary>
        ''' <param name="protein"></param>
        ''' <returns></returns>
        <Extension> Public Function SubCellularLocations(protein As entry) As String()
            Dim cellularComments = protein _
                .CommentList _
                .TryGetValue("subcellular location", [default]:={})
            Return cellularComments _
                .Select(Function(c)
                            Return c.subcellularLocations _
                                .Select(Function(x)
                                            Return x.locations _
                                                .Select(Function(l) l.value)
                                        End Function)
                        End Function) _
                .IteratesALL _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

        ''' <summary>
        ''' 获取蛋白质的功能结构信息
        ''' </summary>
        ''' <param name="prot"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetDomainData(prot As entry) As DomainModel()
            Dim features As feature() = prot.features.Takes("domain")
            Dim out As DomainModel() = features.ToArray(
                Function(f) New DomainModel With {
                    .DomainId = f.description,
                    .Start = f.location.begin.position,
                    .End = f.location.end.position
                })
            Return out
        End Function

        ''' <summary>
        ''' 生成KEGG或者GO注释分类的mapping表
        ''' </summary>
        ''' <param name="uniprotXML"></param>
        ''' <param name="type$"></param>
        ''' <param name="idType"></param>
        ''' <returns>``term --> geneID``</returns>
        <Extension>
        Public Function Term2Gene(uniprotXML As UniProtXML, Optional type$ = "GO", Optional idType As IDTypes = IDTypes.Accession) As IDMap()
            Dim out As New List(Of IDMap)
            Dim getID As Func(Of entry, String) = idType.GetID

            For Each prot As entry In uniprotXML.entries
                Dim ID As String = getID(prot)

                If prot.Xrefs.ContainsKey(type) Then
                    out += From term As dbReference
                           In prot.Xrefs(type)
                           Select New IDMap With {
                               .Key = term.id,
                               .Maps = ID
                           }
                End If
            Next

            Return out
        End Function
    End Module
End Namespace
