#Region "Microsoft.VisualBasic::3aa987de88b2dbfb61677c3891f7a683, GCModeller\core\Bio.Assembly\Assembly\KEGG\Archives\Xml\Nodes\EC_Mapping.vb"

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

    '   Total Lines: 126
    '    Code Lines: 93
    ' Comment Lines: 15
    '   Blank Lines: 18
    '     File Size: 5.56 KB


    '     Class ReactionMaps
    ' 
    '         Properties: EC, Reactions
    ' 
    '         Function: ToString
    ' 
    '     Class EC_Mapping
    ' 
    '         Properties: ECMaps, locusId
    ' 
    '         Function: (+2 Overloads) __mapFlux, ContainsRxn, Generate_ECMappings, IsECEquals, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET

Namespace Assembly.KEGG.Archives.Xml.Nodes

    Public Class ReactionMaps : Implements INamedValue

        <XmlAttribute> Public Property EC As String Implements INamedValue.Key

        Public Property Reactions As String()

        Public Overrides Function ToString() As String
            Return String.Format("[EC {0}]  {1}", EC, String.Join(";", Reactions))
        End Function
    End Class

    <XmlType("EC_Mapping", Namespace:="http://code.google.com/p/genome-in-code/component-models/ec_mapping")>
    Public Class EC_Mapping : Implements INamedValue

        <XmlElement("EC_ID", Namespace:="http://code.google.com/p/genome-in-code/component-models/ec_mapping_id-string")>
        Public Property ECMaps As ReactionMaps()

        ''' <summary>
        ''' 酶分子的基因编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("locus_tag")>
        Public Property locusId As String Implements INamedValue.Key

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
            Return locusId & "  --> " & String.Join(Of String)(", ", ECMaps.Select(Function(x) x.EC).ToArray)
        End Function

        Public Shared Function Generate_ECMappings(Model As XmlModel) As EC_Mapping()
            Dim gECs = (From cat As PwyBriteFunc In Model.Pathways
                        Select (From Pathway As bGetObject.Pathway
                                In cat.Pathways
                                Where Not Pathway.genes.IsNullOrEmpty
                                Select (From gene As NamedValue In Pathway.genes
                                        Let EC As String() = gene.text.EcParser
                                        Select locusId = gene.name,
                                            EC).ToArray).ToArray).IteratesALL.IteratesALL
            Dim gLst = (From GG In (From GO In gECs
                                    Select GO
                                    Group GO By GO.locusId Into Group)
                        Let EC As String() = (From s In GG.Group Select s.EC).IteratesALL.Distinct.ToArray
                        Where Not EC.IsNullOrEmpty
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
                                                        Where IsECEquals(rxn.Enzyme, mappedEC)
                                                        Select rxn
            Return New ReactionMaps With {
                .EC = mappedEC,
                .Reactions = LQuery.Select(Function(x) x.ID).ToArray
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
            Return mappedEC _
                .Select(Function(cls) __mapFlux(model, cls)) _
                .ToArray
        End Function
    End Class
End Namespace
