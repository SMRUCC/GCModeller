#Region "Microsoft.VisualBasic::50d663c12d86ac5b75770fea6c5e88c4, GCModeller\engine\Model\Cellular\Molecule\Genotype.vb"

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

    '   Total Lines: 42
    '    Code Lines: 20
    ' Comment Lines: 15
    '   Blank Lines: 7
    '     File Size: 2.06 KB


    '     Structure Genotype
    ' 
    '         Function: GetEnumerator, IEnumerable_GetEnumerator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

Namespace Cellular.Molecule

    ''' <summary>
    ''' 目标细胞模型的基因组模型
    ''' </summary>
    Public Structure Genotype : Implements IEnumerable(Of CentralDogma)

        ''' <summary>
        ''' 假设基因组之中的基因型定义信息全部都是由中心法则来构成的
        ''' </summary>
        ''' <remarks>
        ''' 请注意，当前的模块之中所定义的计算模型和GCMarkup之类的数据模型在看待基因组的构成上面的角度是有一些差异的：
        ''' 
        ''' 例如，对于复制子的描述上面，GCMarkup数据模型之中是更加倾向于将复制子分开进行描述的，这样子
        ''' 会更加的方便人类进行模型文件的阅读
        ''' 而在本计算模型之中，因为计算模型是计算机程序所阅读的，并且在实际的生命活动之中，染色体和质粒是在同一个环境之中
        ''' 工作的，所以在计算模型之中，没有太多的复制子的概念，而是将他们都合并到当前的这个中心法则列表对象之中，作为一个
        ''' 整体来进行看待
        ''' </remarks>
        Dim centralDogmas As CentralDogma()

        Dim RNAMatrix As RNAComposition()
        Dim ProteinMatrix As ProteinComposition()

        Public Overrides Function ToString() As String
            Return $"{centralDogmas.Length} genes"
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of CentralDogma) Implements IEnumerable(Of CentralDogma).GetEnumerator
            For Each cd As CentralDogma In centralDogmas
                Yield cd
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Structure
End Namespace
