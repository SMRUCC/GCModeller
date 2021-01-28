﻿#Region "Microsoft.VisualBasic::e43e5bca44d7b1f149869d65890636ac, Bio.Assembly\Assembly\ELIXIR\UniProt\XML\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: ECNumberList, EnumerateAllIDs, GetDomainData, GO, KO
    '                   NCBITaxonomyId, ORF, OrganismScientificName, proteinFullName, ProteinSequence
    '                   SubCellularLocations, Summary, Term2Gene
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ProteinModel

Namespace Assembly.Uniprot.XML

    <HideModuleName> Public Module Extensions

        ''' <summary>
        ''' Get protein sequence
        ''' </summary>
        ''' <param name="prot"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ProteinSequence(prot As entry) As String
            Return prot _
                .sequence _
                .sequence _
                .LineTokens _
                .JoinBy("") _
                .Replace(" ", "")
        End Function

        ''' <summary>
        ''' Get KO number of this protein
        ''' </summary>
        ''' <param name="protein"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function KO(protein As entry) As dbReference
            Return protein.xrefs.TryGetValue("KO", [default]:=Nothing).ElementAtOrDefault(0)
        End Function

        ''' <summary>
        ''' Get KO number of this protein
        ''' </summary>
        ''' <param name="protein"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GO(protein As entry) As IEnumerable(Of dbReference)
            Return protein.xrefs.TryGetValue("GO", [default]:=Nothing)
        End Function

        <Extension>
        Public Iterator Function EnumerateAllIDs(entry As entry) As IEnumerable(Of (Database$, xrefID$))
            For Each accession As String In entry.accessions.SafeQuery
                Yield (entry.dataset, accession)
            Next

            Yield ("geneName", entry.name)

            For Each reference As dbReference In entry.dbReferences.SafeQuery
                Yield (reference.type, reference.id)

                For Each prop As [property] In reference.properties.SafeQuery
                    Yield (reference.type & ":" & prop.type.Replace(" ", "_"), prop.value)
                Next
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ECNumberList(protein As entry) As String()
            Return protein _
                ?.protein _
                ?.recommendedName _
                ?.ecNumber _
                 .Select(Function(ec) ec.value) _
                 .ToArray
        End Function

        <Extension>
        Public Function OrganismScientificName(protein As entry) As String
            If protein.organism Is Nothing Then
                Return ""
            Else
                Return protein.organism.scientificName
            End If
        End Function

        <Extension>
        Public Function NCBITaxonomyId(protein As entry) As String
            If protein.organism Is Nothing Then
                Return Nothing
            Else
                Dim ncbi = protein.organism?.dbReference _
                   .SafeQuery _
                   .Where(Function(ref) ref.type = "NCBI Taxonomy") _
                   .FirstOrDefault

                If Not ncbi Is Nothing Then
                    Return ncbi.id
                Else
                    Return Nothing
                End If
            End If
        End Function

        <Extension>
        Public Function proteinFullName(protein As entry) As String
            If protein Is Nothing OrElse protein.protein Is Nothing Then
                Return ""
            Else
                Return protein.protein.fullName
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
        <Extension>
        Public Function SubCellularLocations(protein As entry) As String()
            Dim cellularComments = protein _
                .CommentList _
                .TryGetValue("subcellular location", [default]:={})

            Return cellularComments _
                .Select(Function(c)
                            Return c _
                                .subcellularLocations _
                                .SafeQuery _
                                .Select(Function(loc)
                                            Return loc.locations _
                                                .SafeQuery _
                                                .Select(Function(l)
                                                            Return l.value
                                                        End Function)
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
            Dim out As DomainModel() = features _
                .Select(Function(f)
                            Return New DomainModel With {
                                .DomainId = f.description,
                                .start = f.location.begin.position,
                                .ends = f.location.end.position
                            }
                        End Function) _
                .ToArray

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

                If prot.xrefs.ContainsKey(type) Then
                    out += From term As dbReference
                           In prot.xrefs(type)
                           Select New IDMap With {
                               .Key = term.id,
                               .Maps = ID
                           }
                End If
            Next

            Return out
        End Function

        <Extension>
        Public Iterator Function Summary(Of T)(proteins As IEnumerable(Of entry), asObj As Func(Of entry, T)) As IEnumerable(Of NamedValue(Of T))
            For Each protein As entry In proteins
                Yield New NamedValue(Of T) With {
                    .Name = protein.accessions(Scan0),
                    .Value = asObj(protein)
                }
            Next
        End Function
    End Module
End Namespace
