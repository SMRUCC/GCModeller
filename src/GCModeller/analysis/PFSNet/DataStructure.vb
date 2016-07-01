#Region "Microsoft.VisualBasic::789f7803eb705a6e6e9f2cee679bc657, ..\GCModeller\analysis\PFSNet\DataStructure.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DataVisualization.Network.Abstract

Namespace DataStructure

    ''' <summary>
    ''' The gene expression data samples file.(基因的表达数据样本)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataFrameRow

        Public Property Name As String
        ''' <summary>
        ''' This gene's expression value in the different experiment condition.(同一个基因在不同实验之下的表达值)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ExperimentValues As Double()

        ''' <summary>
        ''' Gets the sample counts of current gene expression data.(获取基因表达数据样本数目)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Samples As Integer
            Get
                Return ExperimentValues.GetElementCounts
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}   ==> {1}", Name, String.Join(", ", ExperimentValues))
        End Function

        ''' <summary>
        ''' Load the PfsNET file1 and file2 data into the memory.(加载PfsNET计算数据之中的文件1和文件2至计算机内存之中)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadData(path As String) As DataFrameRow()
            Dim LQuery = (From line As String In IO.File.ReadAllLines(path)
                          Let Tokens As String() = Strings.Split(line, vbTab)
                          Select New DataFrameRow With
                                 {
                                     .Name = Tokens.First, .ExperimentValues = (From s As String In Tokens Select Val(s)).ToArray}).ToArray
            Return LQuery
        End Function

        Public Shared Function TakeSamples(data As DataFrameRow(), sampleVector As Integer(), reversed As Boolean) As DataFrameRow()
            Dim LQuery = (From item In data.AsParallel Select New DataFrameRow With {.Name = item.Name, .ExperimentValues = item.ExperimentValues.Takes(sampleVector, reversedSelect:=reversed)}).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 以列为单位
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateApplyFunctionCache(data As DataFrameRow()) As KeyValuePair(Of String(), Double()())
            Dim LQuery = (From i As Integer In data.First.ExperimentValues.Sequence
                          Let rows = (From row In data Select row.ExperimentValues(i)).ToArray
                          Select rows).ToArray
            Return New KeyValuePair(Of String(), Double()())((From row In data Select row.Name).ToArray, LQuery)
        End Function

        Public Shared Function InternalCreateDataFrameFromCache(names As String(), cols As Double()()) As DataFrameRow()
            Dim LQuery = (From i As Integer In names.Sequence Select New DataFrameRow With {.Name = names(i), .ExperimentValues = (From col In cols Select col(i)).ToArray}).ToArray
            Return LQuery
        End Function
    End Class

    ''' <summary>
    ''' Gene to Gene Interaction.(基因与基因之间的互作关系)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GraphEdge : Implements IInteraction
        <XmlAttribute> Public Property PathwayID As String
        <XmlAttribute("GeneObject1")> Public Property g1 As String Implements IInteraction.source
        <XmlAttribute("GeneObject2")> Public Property g2 As String Implements IInteraction.target

        Public ReadOnly Property SelfLoop As Boolean
            Get
                Return String.Equals(g1, g2)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Join(vbTab, PathwayID, g1, g2)
        End Function

        Public Shared Function LoadData(path As String) As GraphEdge()
            Dim LQuery = (From line As String In IO.File.ReadAllLines(path) Let tokens As String() = Strings.Split(line, vbTab) Select New GraphEdge With {.PathwayID = tokens(0), .g1 = tokens(1), .g2 = tokens(2)}).ToArray
            Return LQuery
        End Function
    End Class

    ''' <summary>
    ''' The xml format pfsnet calculation result data.(PfsNET结果Xml文件)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PFSNetResultOut

        ''' <summary>
        ''' The data tag value for the current PfsNET evaluation.(本次计算结果的数据标签)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DataTag As String

        ''' <summary>
        ''' The mutation phenotype 1 evaluation data for the significant sub network.(Class1)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property Phenotype1 As PFSNetGraph()
        ''' <summary>
        ''' The another mutation phenotype evaluation data for the significant sub network.(Class2)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property Phenotype2 As PFSNetGraph()

        Public Overrides Function ToString() As String
            Return DataTag
        End Function
    End Class

    ''' <summary>
    ''' The gene node in the PfsNET evaluated sub network.(PfsNET所计算出来的子网络结果之中的一个基因节点)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PFSNetGraphNode

        ''' <summary>
        ''' The gene name.(基因名称)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Name As String
        ''' <summary>
        ''' Fuzzy weight of this gene node in current sub network.(这个基因节点在当前的这个子网络之中的模糊权重)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property weight As Double
        ''' <summary>
        ''' Fuzzy weight2 of this gene node in the current sub network.(这个基因节点在当前的这个子网络之中的模糊权重2) 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property weight2 As Double

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    ''' <summary>
    ''' A metabolism pathway network or its calculated sub network.(一个代谢途径或者子网络，或者说是所属出的计算结果之中的一个子网络对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PFSNetGraph

        Dim _InternalNodesDict As Dictionary(Of String, PFSNetGraphNode) = New Dictionary(Of String, PFSNetGraphNode)

        ''' <summary>
        ''' The nodes in the PfsNET sub network.(网络之中的基因节点)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Nodes As PFSNetGraphNode()
            Get
                Return _InternalNodesDict.Values.ToArray
            End Get
            Set(value As PFSNetGraphNode())
                _InternalNodesDict = value.ToDictionary(Function(n) n.Name)
            End Set
        End Property

        ''' <summary>
        ''' Gene to gene interaction, ggi.(基因与基因之间的连接，即ggi，基因对基因的互作)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Edges As GraphEdge()
        <XmlAttribute> Public Property Id As String
        <XmlAttribute> Public Property statistics As Double
        <XmlAttribute> Public Property pvalue As Double
        <XmlAttribute> Public Property masked As Boolean

        ''' <summary>
        ''' The gene counts in the current calculated PfsNET sub network.
        ''' (当前的这个PfsNET子网络之中所计算出来的基因节点的数目)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Length As Integer
            Get
                Return Nodes.GetElementCounts
            End Get
        End Property

        ''' <summary>
        ''' Gets a specific gene node from its name property.(通过基因名来获取本网路对象之中的一个基因节点，当该节点不存在的时候会返回空值)
        ''' </summary>
        ''' <param name="name"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public ReadOnly Property Node(Name As String) As PFSNetGraphNode
            Get
                If _InternalNodesDict.ContainsKey(Name) Then
                    Return _InternalNodesDict(Name)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            If Edges.IsNullOrEmpty Then
                Return String.Format("{0} ({1})", Id, String.Join("; ", (From Node In Me.Nodes Select Node.Name).ToArray))
            End If
            Return String.Format("{0}: {1}", Id, String.Join("; ", (From ed In Edges Select String.Format("{0} <-> {1}", ed.g1, ed.g2))))
        End Function
    End Class
End Namespace
