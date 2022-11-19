#Region "Microsoft.VisualBasic::4c58a9d5f7b6bf48039bdd532700b5e5, GCModeller\annotations\GSEA\GSEA\KOBAS\Formats\CLS.vb"

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

    '   Total Lines: 79
    '    Code Lines: 28
    ' Comment Lines: 45
    '   Blank Lines: 6
    '     File Size: 3.02 KB


    '     Class CLS
    ' 
    '         Properties: classNameMaps, numOfClasses, numOfSamples, sampleClass
    ' 
    '         Function: ParseFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Namespace KOBAS

    ''' <summary>
    ''' The CLS file format defines phenotype (class or template) labels and 
    ''' associates each sample in the expression data with a label. 
    ''' The CLS file format uses spaces or tabs to separate the fields.
    '''
    ''' The CLS file format differs somewhat depending On whether you are 
    ''' defining categorical Or continuous phenotypes. Categorical labels 
    ''' define discrete phenotypes; For example, normal vs tumor.
    ''' </summary>
    ''' <remarks>
    ''' http://software.broadinstitute.org/cancer/software/gsea/wiki/index.php/Data_formats#Phenotype_Data_Formats
    ''' </remarks>
    Public Class CLS

        ''' <summary>
        ''' number of samples
        ''' </summary>
        ''' <returns></returns>
        Public Property numOfSamples As Integer
        ''' <summary>
        ''' number of classes
        ''' </summary>
        ''' <returns></returns>
        Public Property numOfClasses As Integer
        ''' <summary>
        ''' 应用于报告输出的显示名称
        ''' ``[label => name]``
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' The count of this table should be EQUALS to <see cref="numOfClasses"/>
        ''' </remarks>
        Public Property classNameMaps As Dictionary(Of String, String)
        ''' <summary>
        ''' class label of each sample
        ''' </summary>
        ''' <returns></returns>
        Public Property sampleClass As String()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 1: (number of samples) (space) (number of classes) (space) 1
        ''' 2: # (space) (class 0 name) (space) (class 1 name)
        ''' 3: (sample 1 class) (space) (sample 2 class) (space) ... (sample N class)
        ''' </remarks>
        Public Shared Function ParseFile(path As String) As CLS
            Dim lines$() = path.ReadAllLines
            Dim headers = lines(Scan0).StringSplit("\s+")
            Dim classList = lines(2).StringSplit("(\t|\s)+")
            Dim titles = lines(1).Trim("#"c, " "c).StringSplit("\s+")
            ' Note: The order of the labels determines the association of class names 
            ' and class labels, even if the class labels are the same as the class 
            ' names.
            Dim nameMaps As Dictionary(Of String, String) = classList _
                .Distinct _
                .SeqIterator _
                .ToDictionary(Function(lb) lb.value,
                              Function(i)
                                  Return titles(i)
                              End Function)

            Return New CLS With {
                .numOfSamples = headers(Scan0),
                .numOfClasses = headers(1),
                .classNameMaps = nameMaps,
                .sampleClass = classList
            }
        End Function
    End Class

End Namespace
