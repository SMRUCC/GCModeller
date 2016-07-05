#Region "Microsoft.VisualBasic::b848cd8404670ca536e2812792b728a0, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\Archives\Xml\Nodes\EC_Mapping.vb"

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

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml.Nodes
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.Archives.Xml.Nodes

    Public Class ReactionMaps : Implements sIdEnumerable

        <XmlAttribute> Public Property EC As String Implements sIdEnumerable.Identifier

        Public Property Reactions As String()

        Public Overrides Function ToString() As String
            Return String.Format("[EC {0}]  {2}", EC, String.Join(";", Reactions))
        End Function
    End Class

    <XmlType("EC_Mapping", Namespace:="http://code.google.com/p/genome-in-code/component-models/ec_mapping")>
    Public Class EC_Mapping : Implements sIdEnumerable

        <XmlElement("EC_ID", Namespace:="http://code.google.com/p/genome-in-code/component-models/ec_mapping_id-string")>
        Public Property ECMaps As ReactionMaps()

        ''' <summary>
        ''' 酶分子的基因编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("locus_tag")>
        Public Property locusId As String Implements sIdEnumerable.Identifier

        ''' <summary>
        ''' 这个映射之中是否包含有某一个代谢过程
        ''' </summary>
        ''' <param name="rxn"></param>
        ''' <returns></returns>
        Public Function ContainsRxn(rxn As String) As Boolean
            If ECMaps.IsNullOrEmpty Then
                Return False
            End If

            For Each x As ReactionMaps In ECMaps
                If x.Reactions.IsNullOrEmpty Then
                    Continue For
                End If

                If Array.IndexOf(x.Reactions, rxn) > -1 Then
                    Return True
                End If
            Next

            Return False
        End Function

        Public Overrides Function ToString() As String
            Return locusId & "  --> " & String.Join(Of String)(", ", ECMaps.ToArray(Function(x) x.EC))
        End Function

        Public Shared Function Generate_ECMappings(Model As XmlModel) As EC_Mapping()
            Dim gECs = (From cat As PwyBriteFunc In Model.Pathways
                        Select (From Pathway As bGetObject.Pathway
                                In cat.Pathways
                                Where Not Pathway.Genes.IsNullOrEmpty
                                Select (From gene As KeyValuePair In Pathway.Genes
                                        Let EC As String() = gene.Value.EcParser
                                        Select locusId = gene.Key,
                                            EC).ToArray).ToArray).MatrixToVector.MatrixAsIterator
            Dim gLst = (From GG In (From GO In gECs
                                    Select GO
                                    Group GO By GO.locusId Into Group)
                        Let EC As String() = (From s In GG.Group Select s.EC).MatrixAsIterator.Distinct.ToArray
                        Where Not StringHelpers.IsNullOrEmpty(EC)
                        Select GG.locusId,
                            EC).ToArray
            Dim LQuery = (From Gene In gLst
                          Let MappedEC As String() = (From s As String In Gene.EC Select Strings.Split(s, ".-").First).ToArray
                          Let mapRxns As ReactionMaps() = __mapFlux(Model, MappedEC)
                          Select Gene,
                              mapRxns).ToArray
            Dim maps As EC_Mapping() =
                LinqAPI.Exec(Of EC_Mapping) <= From map
                                               In LQuery
                                               Select New EC_Mapping With {
                                                   .locusId = map.Gene.locusId,
                                                   .ECMaps = map.mapRxns
                                               }
            Return maps
        End Function

        Private Shared Function __mapFlux(model As XmlModel, mappedEC As String) As ReactionMaps
            Dim LQuery As bGetObject.Reaction() =
                LinqAPI.Exec(Of bGetObject.Reaction) <= From rxn As DBGET.bGetObject.Reaction
                                                        In model.Metabolome
                                                        Where IsECEquals(rxn.ECNum, mappedEC)
                                                        Select rxn
            Return New ReactionMaps With {
                .EC = mappedEC,
                .Reactions = LQuery.ToArray(Function(x) x.Entry)
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="def">代谢过程之中定义的EC的集合</param>
        ''' <param name="mappedEC">目标比较的EC编号值</param>
        ''' <returns></returns>
        Public Shared Function IsECEquals(def As String(), mappedEC As String) As Boolean
            For Each ECNum As String In def
                If InStr(ECNum, mappedEC) = 1 Then
                    Return True
                End If
            Next

            Return False
        End Function

        Private Shared Function __mapFlux(model As XmlModel, mappedEC As String()) As ReactionMaps()
            Return mappedEC.ToArray(Function(cls) __mapFlux(model, cls))
        End Function
    End Class
End Namespace
