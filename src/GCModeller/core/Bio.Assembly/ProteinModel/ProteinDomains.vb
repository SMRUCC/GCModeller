#Region "Microsoft.VisualBasic::7bbbf3b2fe0337d89df1027e2d4bf8ab, GCModeller\core\Bio.Assembly\ProteinModel\ProteinDomains.vb"

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

    '   Total Lines: 115
    '    Code Lines: 66
    ' Comment Lines: 36
    '   Blank Lines: 13
    '     File Size: 4.90 KB


    '     Class Protein
    ' 
    '         Properties: Description, Domains, ID, Length, Organism
    '                     SequenceData
    ' 
    '         Function: __equals, ContainsAnyDomain, ContainsDomain, EXPORT, InteractionWith
    '                   SimilarTo, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.SequenceModel

Namespace ProteinModel

    ''' <summary>
    ''' A type of data structure for descript the protein domain architecture distribution.(一个用于描述蛋白质结构域分布的数据结构)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlType("ProtDomains", [Namespace]:="http://gcmodeller.org/models/protein")>
    Public Class Protein : Implements INamedValue, IPolymerSequenceModel

        ''' <summary>
        ''' 该目标蛋白质的唯一标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("Identifier", [Namespace]:="http://gcmodeller.org/programming/language/visualbasic/Identifier")>
        Public Overridable Property ID As String Implements INamedValue.Key
        Public Property Organism As String

        <XmlElement> Public Property Description As String
        <XmlElement> Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
        ''' <summary>
        ''' 结构域分布
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Domains As DomainObject()

        Public ReadOnly Property Length As Integer
            Get
                Return Len(SequenceData)
            End Get
        End Property

        Default Public ReadOnly Property Domain(index As Integer) As DomainObject
            Get
                Return Domains(index)
            End Get
        End Property

        Public Function EXPORT() As FASTA.FastaSeq
            Return New FASTA.FastaSeq({ID, Description}, SequenceData)
        End Function

        Public Function ContainsDomain(DomainAccession As String) As Boolean
            Dim LQuery = From Domain In Domains
                         Where String.Equals(DomainAccession, Domain.Name)
                         Select 100 '
            Return LQuery.FirstOrDefault > 50
        End Function

        ''' <summary>
        ''' 本蛋白质之中是否包含有目标结构域编号列表中的任何结构域信息，返回所包含的编号列表
        ''' </summary>
        ''' <param name="DomainAccessions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContainsAnyDomain(DomainAccessions As IEnumerable(Of String)) As String()
            Dim LQuery = From Id As String
                         In DomainAccessions
                         Where ContainsDomain(DomainAccession:=Id) = True
                         Select Id '
            Return LQuery.ToArray
        End Function

        ''' <summary>
        ''' 获取与本蛋白质相互作用的结构域列表
        ''' </summary>
        ''' <param name="DOMINE"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InteractionWith(DOMINE As DOMINE.Database) As String()
            If Me.Domains.IsNullOrEmpty Then
                Return New String() {}
            Else
                Dim lstInteracts As IEnumerable(Of String) = (From motif As DomainObject
                                                              In Me.Domains
                                                              Select motif.GetInteractionDomains(DOMINE)).IteratesALL
                Return lstInteracts.Distinct.ToArray
            End If
        End Function

        ''' <summary>
        ''' 简单的根据结构域分布来判断两个蛋白质是否相似，两个蛋白质的结构域分布必须以相似的方式进行排布
        ''' </summary>
        ''' <param name="Protein1"></param>
        ''' <param name="Protein2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SimilarTo(Protein1 As Protein, Protein2 As Protein, Optional threshold As Double = 0.3) As Boolean
            Dim a As String() = Protein1.Domains.Select(Function(x) x.Name).ToArray
            Dim b As String() = Protein2.Domains.Select(Function(x) x.Name).ToArray
            Dim edits As DistResult = LevenshteinDistance.ComputeDistance(a, b, AddressOf __equals, Function(c) "")
            Return edits.MatchSimilarity >= threshold
        End Function

        Private Shared Function __equals(m1 As String, m2 As String) As Boolean
            Return String.Equals(m1, m2, StringComparison.OrdinalIgnoreCase)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
