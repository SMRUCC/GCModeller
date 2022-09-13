#Region "Microsoft.VisualBasic::338178523f7f4fce18140982ccc677ee, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\GeneBrief.vb"

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

    '   Total Lines: 188
    '    Code Lines: 112
    ' Comment Lines: 56
    '   Blank Lines: 20
    '     File Size: 7.39 KB


    '     Class GeneBrief
    ' 
    '         Properties: ATG, Code, COG, Gene, IsBlankSegment
    '                     IsORF, Length, Location, PID, Product
    '                     Synonym, TGA
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Clone, (+2 Overloads) CreateObject, (+2 Overloads) DocumentParser, getCOGEntry, Strand
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel

Namespace Assembly.NCBI.GenBank.TabularFormat.ComponentModels

    ''' <summary>
    ''' The gene brief information data in a ncbi PTT document.(PTT文件之中的一行，即一个基因的对象摘要信息)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneBrief : Implements INamedValue
        Implements IGeneBrief

        ''' <summary>
        ''' The location of this ORF gene on the genome sequence.(包含有PTT文件之中的Location, Strand和Length列)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Location As NucleotideLocation Implements IGeneBrief.Location
        <XmlAttribute> Public Property PID As String
        ''' <summary>
        ''' 基因名，在genbank文件里面是/gene=，基因号，这个应该是GI编号，而非平常比较熟悉的字符串编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Gene As String
        <XmlAttribute> Public Property Code As String
        <XmlAttribute> Public Property COG As String Implements IGeneBrief.Feature
        ''' <summary>
        ''' Protein product functional description in the genome.
        ''' (基因的蛋白质产物的功能的描述)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlText> Public Property Product As String Implements IGeneBrief.Product
        ''' <summary>
        ''' The NT length of this ORF.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Length As Integer Implements IGeneBrief.Length
        ''' <summary>
        ''' The gene's locus_tag data.
        ''' (我们所正常熟知的基因编号，<see cref="PTT"/>对象主要是使用这个属性值来生成字典对象的)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Synonym As String Implements INamedValue.Key

        ''' <summary>
        ''' *.ptt => TRUE;  *.rnt => FALSE
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property IsORF As Boolean = True

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Gene, Product)
        End Function

        Public Function Clone() As GeneBrief
            Return DirectCast(MemberwiseClone(), GeneBrief)
        End Function

        Public Function getCOGEntry(Of T_Entry As IGeneBrief)() As T_Entry
            Dim obj As T_Entry = Activator.CreateInstance(Of T_Entry)()
            obj.Feature = COG
            obj.Length = Length
            obj.Product = Product
            obj.Key = Synonym

            Return obj
        End Function

        Public ReadOnly Property ATG As Long
            Get
                If Me.Location.Strand = Strands.Forward Then
                    Return Me.Location.Left
                Else
                    Return Me.Location.Right
                End If
            End Get
        End Property

        Public ReadOnly Property TGA As Long
            Get
                If Me.Location.Strand = Strands.Forward Then
                    Return Me.Location.Right
                Else
                    Return Me.Location.Left
                End If
            End Get
        End Property

        Public Shared Function CreateObject(g As IGeneBrief) As GeneBrief
            Return New GeneBrief With {
                .COG = g.Feature,
                .Length = g.Length,
                .Location = g.Location,
                .Product = g.Product,
                .Synonym = g.Key
            }
        End Function

        Public Shared Function CreateObject(data As IEnumerable(Of IGeneBrief)) As GeneBrief()
            Dim LQuery As GeneBrief() =
                LinqAPI.Exec(Of GeneBrief) <= From gene As IGeneBrief
                                              In data.AsParallel
                                              Select CreateObject(g:=gene)
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strLine">Ptt文档之中的一行数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DocumentParser(strLine As String, fillBlankGeneName As Boolean) As GeneBrief
            Dim tokens As String() = strLine.Split(ASCII.TAB)
            Dim location As Long() = (From str As String
                                      In Strings.Split(tokens(Scan0), "..")
                                      Let n = CType(Val(str), Long)
                                      Select n).ToArray
            Dim p As i32 = 2
            Dim strand As Strands = tokens(1)(0).GetStrands
            Dim loci = New NucleotideLocation(location(0), location(1), strand).Normalization
            Dim gene As New GeneBrief With {
                .Location = loci,
                .Length = tokens(++p),
                .PID = tokens(++p),
                .Gene = tokens(++p),
                .Synonym = tokens(++p),
                .Code = tokens(++p),
                .COG = tokens(++p),
                .Product = tokens(++p)
            }

            If (String.Equals(gene.Gene, "-") OrElse String.IsNullOrEmpty(gene.Gene)) AndAlso fillBlankGeneName Then
                ' 假若基因名称为空值的话，假设填充则使用基因号进行填充
                gene.Gene = gene.Synonym
            End If


            Return gene
        End Function

        ''' <summary>
        ''' 42..1370	+	442	66766353	dnaA	XC_0001	-	COG0593L	chromosome replication initiator DnaA
        ''' </summary>
        ''' <param name="strLine"></param>
        ''' <returns></returns>
        Public Shared Function DocumentParser(strLine As String) As GeneBrief
            Return DocumentParser(strLine, False)
        End Function

        ''' <summary>
        ''' 判断本对象是否是由<see cref=" ContextModel.BlankSegment"></see>方法所生成的空白片段
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsBlankSegment As Boolean
            Get
                Return String.Equals(Gene, BLANK_VALUE) OrElse String.Equals(Synonym, BLANK_VALUE)
            End Get
        End Property

        Public Function Strand() As Char
            If Location.Strand = Strands.Forward Then
                Return "+"c
            ElseIf Location.Strand = Strands.Reverse Then
                Return "-"c
            Else
                Return "?"c
            End If
        End Function
    End Class
End Namespace
