#Region "Microsoft.VisualBasic::6ef2c13dc912dccd59fd5cbf08361edf, visualize\Phylip\MatrixFile\MatrixFile.vb"

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

    '     Class MatrixFile
    ' 
    '         Properties: lstID, MATRaw
    ' 
    '         Function: __trimData, CreateObject, GenerateDocument, MAT_ID, RoundNumber
    ' 
    '         Sub: __createMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace MatrixFile

    ''' <summary>
    ''' When Contml is used in the gene-frequency mode (its usual, default mode), or when Gendist is used, 
    ''' the first line contains the number of species (or populations) and the number of loci and the options information. 
    ''' 
    ''' 第一行包含有基因组的数目 和 所比较的位点的数目
    ''' 
    ''' There then follows a line which gives the numbers of alleles at each locus, in order. This must be the full number of alleles, 
    ''' not the number of alleles which will be input: i. e. for a two-allele locus the number should be 2, not 1. 
    ''' 
    ''' 第二行包含有每一个位点之上的等位基因的数目，并且必须是完整的等位基因的数目
    ''' 
    ''' 
    ''' There then follow the species (population) data, each species beginning on a new line. 
    ''' The first 10 characters are taken as the name, and thereafter the values of the individual characters are read free-format, preceded and separated by blanks. 
    ''' They can go to a new line if desired, though of course not in the middle of a number. Missing data is not allowed - an important limitation.
    ''' 
    ''' 
    ''' In the default configuration, for each locus, the numbers should be the frequencies of all but one allele. 
    ''' The menu option A (All) signals that the frequencies of all alleles are provided in the input data -- the program will then automatically ignore the last of them. 
    ''' So without the A option, for a three-allele locus there should be two numbers, the frequencies of two of the alleles (and of course it must always be the same two!). 
    ''' Here is a typical data set without the A option:(当没有设置A选项的时候， 2 + 3 + 2 (第二行) = 4 (矩阵之中有4列) 的原因)
    ''' 
    ''' 
    ''' 在默认的配置数据之中，对于每一个位点，其数字应该为所有等位基因的频率
    ''' A选项表示在所输入的数据之中已经提供了所有的等位基因的频率 ---  程序将会忽略掉（每一个等位基因的数据之中的）最后一列
    ''' 所以当没有为程序设置A选项的时候，对于一个有三个等位基因的位点，其数据只需要两列
    ''' 
    ''' 故而在下列的示例数据之中，会出现 2+3+2 = 4(列的情况)   2-1 + 3-1 + 2-1  
    ''' 
    ''' 
    ''' 之后则是每一个基因组的数据了
    ''' 前10个字符为基因组的名称，后面的数值使用空格进行分割
    ''' 
    ''' 
    '''      5    3
    ''' 2 3 2
    ''' Alpha      0.90 0.80 0.10 0.56
    ''' Beta       0.72 0.54 0.30 0.20
    ''' Gamma      0.38 0.10 0.05  0.98
    ''' Delta      0.42 0.40 0.43 0.97
    ''' Epsilon    0.10 0.30 0.70 0.62
    ''' whereas here is what it would have to look like if the A option were invoked:
    ''' 
    '''      5    3
    ''' 2 3 2
    ''' Alpha      0.90 0.10 0.80 0.10 0.10 0.56 0.44
    ''' Beta       0.72 0.28 0.54 0.30 0.16 0.20 0.80
    ''' Gamma      0.38 0.62 0.10 0.05 0.85  0.98 0.02
    ''' Delta      0.42 0.58 0.40 0.43 0.17 0.97 0.03
    ''' Epsilon    0.10 0.90 0.30 0.70 0.00 0.62 0.38
    ''' The first line has the number of species (or populations) and the number of loci. The second line has the number of alleles for each of the 3 loci. The species lines have names (filled out to 10 characters with blanks) followed by the gene frequencies of the 2 alleles for the first locus, the 3 alleles for the second locus, and the 2 alleles for the third locus. You can start a new line after any of these allele frequencies, and continue to give the frequencies on that line (without repeating the species name).
    ''' 
    ''' If all alleles of a locus are given, it is important to have them add up to 1. Roundoff of the frequencies may cause the program to conclude that the numbers do not sum to 1, and stop with an error message.
    ''' 
    ''' While many compilers may be more tolerant, it is probably wise to make sure that each number, including the first, is preceded by a blank, and that there are digits both preceding and following any decimal points.
    ''' 
    ''' Contml and Contrast also treat quantitative characters (the continuous-characters mode in Contml, which is option C). It is assumed that each character is evolving according to a Brownian motion model, at the same rate, and independently. In reality it is almost always impossible to guarantee this. The issue is discussed at length in my review article in Annual Review of Ecology and Systematics (Felsenstein, 1988a), where I point out the difficulty of transforming the characters so that they are not only genetically independent but have independent selection acting on them. If you are going to use Contml to model evolution of continuous characters, then you should at least make some attempt to remove genetic correlations between the characters (usually all one can do is remove phenotypic correlations by transforming the characters so that there is no within-population covariance and so that the within-population variances of the characters are equal -- this is equivalent to using Canonical Variates). However, this will only guarantee that one has removed phenotypic covariances between characters. Genetic covariances could only be removed by knowing the coheritabilities of the characters, which would require genetic experiments, and selective covariances (covariances due to covariation of selection pressures) would require knowledge of the sources and extent of selection pressure in all variables.
    ''' 
    ''' Contrast is a program designed to infer, for a given phylogeny that is provided to the program, the covariation between characters in a data set. Thus we have a program in this set that allows us to take information about the covariation and rates of evolution of characters and make an estimate of the phylogeny (Contml), and a program that takes an estimate of the phylogeny and infers the variances and covariances of the character changes. But we have no program that infers both the phylogenies and the character covariation from the same data set.
    ''' 
    ''' In the quantitative characters mode, a typical small data set would be:
    ''' 
    '''      5   6
    ''' Alpha      0.345 0.467 1.213  2.2  -1.2 1.0
    ''' Beta       0.457 0.444 1.1    1.987 -0.2 2.678
    ''' Gamma      0.6 0.12 0.97 2.3  -0.11 1.54
    ''' Delta      0.68  0.203 0.888 2.0  1.67
    ''' Epsilon    0.297  0.22 0.90 1.9 1.74
    ''' Note that in the latter case, there is no line giving the numbers of alleles at each locus. In this latter case no square-root transformation of the coordinates is done: each is assumed to give directly the position on the Brownian motion scale.
    ''' </summary>
    ''' <remarks>
    ''' 使用phylip软件构建进化树的步骤:
    ''' 
    ''' 首先使用本类型的对象创建一个矩阵文件
    ''' 然后使用Phylip软件之中的gendist程序生成一个邻接矩阵
    ''' 之后使用neighbor/kitsch/fitch程序生成进化树
    ''' 使用DrawGram.jar/DrawTree.jar生成进化树的图像文件
    ''' </remarks>
    Public Class MatrixFile

        ''' <summary>
        ''' 请注意，第一行为标题行，第一列为基因组的编号列
        ''' </summary>
        ''' <remarks></remarks>
        Protected _innerMATRaw As IO.File

        Public ReadOnly Property MATRaw As IO.File
            Get
                Return _innerMATRaw
            End Get
        End Property

        ''' <summary>
        ''' 从模型之中生成矩阵文件
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GenerateDocument() As String
            Throw New NotImplementedException
        End Function

        Public Property lstID As KeyValuePair()

        ''' <summary>
        ''' 请注意，第一行为标题行，第一列为基因组的编号列
        ''' </summary>
        ''' <param name="docBuilder"></param>
        ''' <remarks></remarks>
        Protected Sub __createMatrix(ByRef docBuilder As StringBuilder)
            Dim lstID As New List(Of KeyValuePair)

            For Each Genome In _innerMATRaw.Skip(1) '第一行为标题行
                Dim row As String

                If Len(Genome.First) > 10 Then
                    row = Mid(Genome.First, 1, 10)
                ElseIf Len(Genome.First) = 10 Then
                    row = Genome.First
                Else
                    Dim blank As String = New String(" "c, 10 - Len(Genome.First))
                    row = Genome.First & blank
                End If

                row = row.NormalizePathString(True)
                lstID.Add({row, Genome.First})
                row &= " "
                row &= String.Join(" ", (From n As String In Genome.Skip(1)
                                         Let trimmed As String = __trimData(n)
                                         Select trimmed).ToArray)

                Call docBuilder.AppendLine(row)
            Next

            Me.lstID = lstID.ToArray
        End Sub

        Private Shared Function __trimData(n As String) As String
            Dim p = InStr(n, ".")

            If p > 0 Then
                Return Mid(n, 1, InStr(n, ".") + 2)
            Else
                Return n & ".00"
            End If
        End Function

        ''' <summary>
        ''' 这个构造函数只是对<see cref="_innerMATRaw"/>域进行赋值，适合于所有phylip矩阵文件对象的构建
        ''' </summary>
        ''' <typeparam name="MAT">
        ''' 矩阵文件的格式要求为：
        ''' 行的标题（每一行中的第一个元素）为基因组的名称
        ''' 每一列为某一个基因的频率或者其他数值
        ''' 例如：
        '''         基因1，基因2，基因3， ...
        ''' 基因组1   1     1      0
        ''' 基因组2   2     1      4
        ''' ....
        ''' </typeparam>
        ''' <param name="dat"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Shared Function CreateObject(Of MAT As MatrixFile)(dat As IO.File) As MAT
            Dim MATModel As MAT = Activator.CreateInstance(Of MAT)()
            MATModel._innerMATRaw = dat
            Return MATModel
        End Function

        Public Shared Function MAT_ID(ID As String) As String
            If Len(ID) > 10 Then
                ID = Mid(ID, 1, 10)
            Else
                Dim d = 10 - Len(ID)

                If d > 0 Then
                    ID = ID & New String(" "c, d)
                End If
            End If

            Return ID
        End Function

        Public Shared Function RoundNumber(n As String, Length As Integer) As String
            If String.Equals(n, "0") OrElse String.IsNullOrEmpty(n) Then
                n = "0.0000"
            Else
                n = Mid(n, 1, Length)
            End If

            Dim d = Length - Len(n)
            If d > 0 Then
                n = n & New String("0"c, d)
            End If

            Return n
        End Function
    End Class
End Namespace
