#Region "Microsoft.VisualBasic::0f0996ce454d870fac7a2e2d0fd455f0, Data_science\Mathematica\Math\Math\NamespaceDoc.vb"

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

    '   Total Lines: 5
    '    Code Lines: 2 (40.00%)
    ' Comment Lines: 3 (60.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 0 (0.00%)
    '     File Size: 148 B


    ' Class NamespaceDoc
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' The math library root: the shared numeric types and mathematical utilities.
''' </summary>
''' <remarks>
''' <para>矩阵分解（<c>LinearAlgebra.Matrix</c>）这一层提供三种粒度的能力：</para>
''' <list type="bullet">
''' <item><description>
''' <see cref="LinearAlgebra.Matrix.SingularValueDecomposition"/> 等经典分解：适合中小规模稠密矩阵的<b>全量</b>分解；
''' </description></item>
''' <item><description>
''' <see cref="LinearAlgebra.Matrix.RandomizedSVD"/>：随机化截断 SVD（幂迭代 + QR 重正交 + 容差秩），
''' 只依赖"矩阵乘向量"与"转置矩阵乘向量"，对稠密数组、<c>SparseMatrix</c> 与 <c>CsrMatrix</c> 共用同一套流程；
''' </description></item>
''' <item><description>
''' <see cref="LinearAlgebra.Matrix.TruncatedPCA"/>：在随机化 SVD 之上补上"列中心化/标准化"的统计语义，
''' 输出特征空间载荷、得分与解释方差；
''' <see cref="LinearAlgebra.Matrix.TruncatedSVD"/> 则是同一引擎的稀疏兼容入口。
''' </description></item>
''' </list>
''' </remarks>
Friend Class NamespaceDoc
End Class
