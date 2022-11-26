#Region "Microsoft.VisualBasic::88c7c0bc83c59ffc8d23e73eeedcb397, GCModeller\core\Bio.Assembly\Assembly\DOOR\Models\OperonGene.vb"

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

    '   Total Lines: 72
    '    Code Lines: 47
    ' Comment Lines: 17
    '   Blank Lines: 8
    '     File Size: 2.77 KB


    '     Class OperonGene
    ' 
    '         Properties: COG_number, GI, Length, Location, OperonID
    '                     Product, Synonym
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace Assembly.DOOR

    ''' <summary>
    ''' Door操纵子之中的一个基因对象的数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OperonGene : Implements INamedValue
        Implements IGeneBrief

        ''' <summary>
        ''' DOOR数据库之中的操纵子编号
        ''' </summary>
        ''' <returns></returns>
        Public Property OperonID As String Implements INamedValue.Key
        Public Property GI As String
        Public Property Synonym As String
        Public Property Length As Integer Implements IGeneBrief.Length
        Public Property COG_number As String Implements IFeatureDigest.Feature
        Public Property Product As String Implements IGeneBrief.Product
        Public Property Location As NucleotideLocation Implements IContig.Location

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]{1}: {2}", COG_number, Synonym, Product)
        End Function

        Sub New()
        End Sub

        ''' <summary>
        ''' 将PTT文件之中的一个基因模型转换为DOOR数据库之中的一个基因模型
        ''' </summary>
        ''' <param name="g"></param>
        Sub New(g As NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief)
            GI = g.PID
            Synonym = g.Synonym
            Length = g.Location.FragmentSize
            COG_number = g.COG
            Product = g.Product
            Location = g.Location
        End Sub

        ''' <summary>
        ''' 将``opr``文件之中的一行元素解析为一个操纵子之中的基因的模型
        ''' </summary>
        ''' <param name="strLine">文件之中的文本行</param>
        ''' <returns></returns>
        Public Shared Function TryParse(strLine As String) As OperonGene
            Dim tokens As String() = Strings.Split(strLine, vbTab)
            Dim p As i32 = Scan0

            Return New OperonGene With {
                .OperonID = tokens(++p),
                .GI = tokens(++p),
                .Synonym = tokens(++p),
                .Location = New NucleotideLocation With {
                    .Left = tokens(++p),
                    .Right = tokens(++p),
                    .Strand = GetStrand(tokens(++p))
                },
                .Length = tokens(++p),
                .COG_number = tokens(++p),
                .Product = tokens(++p)
            }
        End Function
    End Class
End Namespace
