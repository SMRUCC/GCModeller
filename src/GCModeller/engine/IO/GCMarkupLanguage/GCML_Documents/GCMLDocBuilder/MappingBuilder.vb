#Region "Microsoft.VisualBasic::64414b65a4463c8b9e4875e9c2dfd95b, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\MappingBuilder.vb"

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

    '   Total Lines: 96
    '    Code Lines: 58
    ' Comment Lines: 27
    '   Blank Lines: 11
    '     File Size: 5.21 KB


    '     Class MappingBuilder
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Invoke
    ' 
    '         Sub: GeneLinkMetabolism, Link, LinkGene
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem

Namespace Builder

    ''' <summary>
    ''' 在模型中的对象间建立连接
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MappingBuilder : Inherits IBuilder
        Implements System.IDisposable

        Sub New(MetaCyc As DatabaseLoadder, Model As BacterialModel)
            MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Call GeneLinkMetabolism()
            Call Link(Proteins:=MetaCyc.GetProteins, Compounds:=MetaCyc.GetCompounds, ProtLigandCplxes:=MetaCyc.GetProtLigandCplx, Regulations:=MetaCyc.GetRegulations)
            Return MyBase.Model
        End Function

        ''' <summary>
        ''' Link the gene object to the specific metabolism reaction using its product property.(将基因对象与相应的代谢反应进行连接)
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub GeneLinkMetabolism()
            Dim EnzymeList As List(Of Slots.Protein.IEnzyme) = New List(Of Slots.Protein.IEnzyme)
            Call EnzymeList.AddRange(MetaCyc.GetProteins)
            Call EnzymeList.AddRange(MetaCyc.GetProtLigandCplx)  '将所有的蛋白质生成一个集合

            For i As Integer = 0 To Model.Metabolism.MetabolismNetwork.Count - 1   '遍历所有的代谢反应
                Dim Enzrxn = Model.Metabolism.MetabolismNetwork(i)
                If Enzrxn.Enzymes.IsNullOrEmpty Then
                    Continue For
                End If
                For j As Integer = 0 To Enzrxn.Enzymes.Count - 1 '遍历该代谢反应中所有的酶催化剂分子
                    Dim EnzymeId As String = Enzrxn.Enzymes(j)
                    Dim Handle As Integer = Model.BacteriaGenome.Genes.IndexOf(EnzymeId)  '先尝试使用其Id编号通过基因产物属性来查找出相应的基因对象

                    If Handle > -1 Then  '找到了相应的基因：这个酶催化剂分子为单体蛋白质
                        '     Model.BacteriaGenome.Genes(Handle)._MetabolismNetwork.Add(i) '变量i指的是生化反应在模型中的对象句柄值
                    Else '没有找到，则这个酶催化剂分子可能为蛋白质复合物，则将其进行拆分为单体蛋白质，再进行查找
                        Dim Enzyme = EnzymeList.Select(EnzymeId)
                        If Not Enzyme Is Nothing Then  '没有找到则跳过当前对象  
                            For Each ID In Enzyme.Components
                                Call LinkGene(ID, EnzymeList, Handle_i:=i)
                            Next
                        End If
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 蛋白质复合物对基因对象的连接的递归算法
        ''' </summary>
        ''' <param name="EnzymeId"></param>
        ''' <param name="EnzymeList"></param>
        ''' <remarks></remarks>
        Private Sub LinkGene(EnzymeId As String, EnzymeList As List(Of Slots.Protein.IEnzyme), Handle_i As Integer)
            Dim Handle As Integer = Model.BacteriaGenome.Genes.IndexOf(EnzymeId)  '先尝试使用其Id编号通过基因产物属性来查找出相应的基因对象

            If Handle > -1 Then  '找到了相应的基因：这个酶催化剂分子为单体蛋白质
                '        Model.BacteriaGenome.Genes(Handle)._MetabolismNetwork.Add(Handle_i)
            Else '没有找到，则这个酶催化剂分子可能为蛋白质复合物，则将其进行拆分为单体蛋白质，再进行查找
                Dim Enzyme = EnzymeList.Select(EnzymeId)
                If Not Enzyme Is Nothing Then  '没有找到则跳过当前对象  
                    For Each ID In Enzyme.Components
                        Call LinkGene(ID, EnzymeList, Handle_i)
                    Next
                End If
            End If
        End Sub

        ''' <summary>
        ''' 分别生成酶促反应对象的催化关系以及调控关系
        ''' </summary>
        ''' <param name="Proteins"></param>
        ''' <param name="ProtLigandCplxes"></param>
        ''' <remarks></remarks>
        Private Sub Link(Proteins As Proteins, ProtLigandCplxes As ProtLigandCplxes, Compounds As Compounds, Regulations As Regulations)
            Dim Proteome As List(Of Slots.Protein.IEnzyme) = New List(Of Slots.Protein.IEnzyme)
            Proteome.AddRange(Proteins)
            Proteome.AddRange(ProtLigandCplxes)

            'Dim EQuery = From e In Proteome.AsParallel Where e.Catalyze.Count > 0 Select New EnzymeCatalystKineticLaw With {.Enzyme = e.locusId} '
            'Model.Metabolism.MetabolismEnzymes = EQuery.ToArray
            'For i As Integer = 0 To Model.Metabolism.MetabolismEnzymes.Length - 1
            '    '      Model.Metabolism.MetabolismEnzymes(i).Catalye = Model.Metabolism.MetabolismEnzymes(i).BaseType.Catalyze.ToArray  '生成催化关系的指针链接
            'Next
        End Sub
    End Class
End Namespace
