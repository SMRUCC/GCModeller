#Region "Microsoft.VisualBasic::4d3aa0d0143e1dada68e2b87c6dc76b0, analysis\ProteinTools\ProteinTools.Interactions\Bayesian\BeliefNetwork.vb"

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

    ' Class BeliefNetwork
    ' 
    '     Function: (+2 Overloads) Convert, GenerateBlankVector, GenerateNetwork, GetBelief
    ' 
    '     Sub: LoadData
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DataMining
Imports Microsoft.VisualBasic.DataMining.ComponentModel
Imports Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout
Imports Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler

Public Class BeliefNetwork

    Dim BeliefNetwork As DataMining.Kernel.BayesianBeliefNetwork.BElim

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <returns>假设其网络结构为线性的</returns>
    ''' <remarks></remarks>
    Public Shared Function GenerateNetwork(Data As AlignmentColumn()) _
        As Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout
        Dim InitializeFirstNode As Func(Of Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode) =
            Function() As Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode
                Dim FirstResidue As Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode =
                    New Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode With {.Name = "FirstNode", .Range = 24} '生成第一个节点，第一个节点的概率表仅为各个残基的概率统计
                Dim CPTable As Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF =
                    New Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF
                Dim AlignmentColumn As SequenceAssembler.AlignmentColumn = Data(0)
                Dim CpChunkBuffer As CPColumn() = (From residue As Char
                                                   In Global.SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.AlignmentColumn.GetResidueCollection
                                                   Select New CPColumn() With {.Data = {AlignmentColumn.GetFrequency(residue)}}).ToArray
                CPTable.CPColumns = CpChunkBuffer
                FirstResidue.CPTable = CPTable
                FirstResidue.Parents = New Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.ParentList With
                                       {
                                           .ParentNodes = New String() {}}
                Return FirstResidue
            End Function

        Dim NodeList As List(Of BeliefNode) =
            New List(Of BeliefNode) From {
                InitializeFirstNode()
            }
        For colIndex As Integer = 1 To Data.Count - 1
            Dim EntityList As IntegerEntity() = Convert(New SequenceAssembler.AlignmentColumn() {Data(colIndex - 1)}, Data(colIndex)).ToArray
            Dim CpTable As List(Of CPColumn) = New List(Of CPColumn)
            Dim Bayesian = DataMining.Kernel.Classifier.Bayesian.Load(EntityList)

            For i As Integer = 0 To 22
                Dim CpList As List(Of Double) = New List(Of Double)

                For j As Integer = 0 To 23
                    Call CpList.Add(Bayesian.P({i}, j))
                Next
                Call CpTable.Add(New CPColumn With {.Data = CpList.ToArray})
            Next

            '生成贝叶斯网络中的一个计算节点
            Dim Node As Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode =
                New Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode With
                {
                    .Name = String.Format("residue_{0}", colIndex),
                    .Range = 24} '生成第一个节点，第一个节点的概率表仅为各个残基的概率统计
            Node.CPTable = New Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF With
                           {
                               .CPColumns = CpTable.ToArray}
            Node.Parents = New Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.ParentList With
                                   {
                                       .ParentNodes = New String() {
                                           String.Format("residue_{0}", colIndex - 1)}}
            Call NodeList.Add(Node)
        Next
        NodeList(1).Parents.ParentNodes = New String() {"FirstNode"}

        Return New Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout With {.Nodes = NodeList.ToArray}
    End Function

    Public Sub LoadData(NetworkLayout As Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout)
        Dim BeliefNetwork = Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(NetworkLayout)
        Me.BeliefNetwork = New Kernel.BayesianBeliefNetwork.BElim(BeliefNetwork)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Proteins">蛋白质序列的排列顺序必须与计算网络时候所使用的比对序列的位置相一致，对于空白的部分请使用一个空字符串</param>
    ''' <param name="ProteinsConditions">蛋白质序列的排列顺序必须与计算网络时候所使用的比对序列的位置相一致，对于空白的部分请使用一个空字符串</param>
    ''' <returns></returns>
    ''' <remarks>假设没有缺失的序列部分，并且在这里将空缺转换为-1</remarks>
    Public Function GetBelief(Proteins As String(), ProteinsConditions As String()) As Double
        Dim Subject As List(Of Integer) = New List(Of Integer)
        Dim Condition As List(Of Integer) = New List(Of Integer)
        Dim BlockCounts As Integer = Proteins.Count - 1

        For idx As Integer = 0 To BlockCounts '假设所有的序列都是没有被空缺掉的
            Dim Seq As String = Proteins(idx)
            Dim SeqCondition As String = ProteinsConditions(idx)
            Dim BlockWidth As Integer = System.Math.Max(Seq.Length, SeqCondition.Length)

            Call Subject.AddRange(Convert(BlockWidth, Seq))
            Call Condition.AddRange(Convert(BlockWidth, SeqCondition))
        Next

        Return Me.BeliefNetwork.GetBelief(Subject.ToArray, Condition.ToArray)
    End Function

    Private Shared Function Convert(BlockWidth As Integer, Seq As String) As Integer()
        If Seq.StringEmpty Then
            Return GenerateBlankVector(BlockWidth)
        Else
            Dim result = (From residue As Char
                          In Seq
                          Select SequenceAssembler.AlignmentColumn.Alphabet.Convert(residue)).ToArray
            Return result
        End If
    End Function

    ''' <summary>
    ''' 使用-1来填充空白的序列区块
    ''' </summary>
    ''' <param name="Width"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GenerateBlankVector(Width As Integer) As Integer()
        Return (From i As Integer In Width.Sequence Select -1).ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SubjectColumns">Entity.Properties</param>
    ''' <param name="TargetColumn">Entity.Class</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Iterator Function Convert(SubjectColumns As AlignmentColumn(), TargetColumn As AlignmentColumn) As IEnumerable(Of IntegerEntity)
        For index As Integer = 0 To TargetColumn.CharArray.Length - 1

            Dim TargetClass As Integer = AlignmentColumn.ProteinAlphabetDictionary(TargetColumn.CharArray(index))
            Dim Hwnd As Integer = index
            Dim EntityProperty As Integer() = (From idx As Integer
                                               In SubjectColumns.Sequence
                                               Let col As SequenceAssembler.AlignmentColumn = SubjectColumns(idx)
                                               Let residue As Char = col.CharArray(Hwnd)
                                               Select SequenceAssembler.AlignmentColumn.ProteinAlphabetDictionary(residue)).ToArray
            Yield New IntegerEntity With {
                .Class = TargetClass,
                .entityVector = EntityProperty
            }
        Next
    End Function
End Class
