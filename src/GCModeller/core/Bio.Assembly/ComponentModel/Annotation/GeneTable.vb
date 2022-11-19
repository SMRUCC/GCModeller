#Region "Microsoft.VisualBasic::69dffeca60acc98ee2483851132959f3, GCModeller\core\Bio.Assembly\ComponentModel\Annotation\GeneTable.vb"

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

    '   Total Lines: 91
    '    Code Lines: 45
    ' Comment Lines: 39
    '   Blank Lines: 7
    '     File Size: 3.45 KB


    '     Class GeneTable
    ' 
    '         Properties: [function], CDS, COG, commonName, EC_Number
    '                     GC_Content, geneName, GI, GO, InterPro
    '                     KO, left, length, Location, locus_id
    '                     ProteinId, right, species, SpeciesAccessionID, strand
    '                     Transl_table, Translation, UniprotSwissProt, UniprotTrEMBL
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' The gene dump information from the NCBI genbank.
    ''' (从GBK文件之中所导出来的一个基因对象的简要信息，尝试使用这个对象以csv表格的格式存储一个基因的所有的注释信息)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneTable : Implements INamedValue
        Implements IGeneBrief

        ''' <summary>
        ''' 假若在GBK文件之中没有Locus_tag属性，则导出函数<see cref="DumpEXPORT"></see>会尝试使用<see cref="ProteinId"></see>来替代
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property locus_id As String Implements INamedValue.Key
        Public Property geneName As String
        Public Property commonName As String Implements IGeneBrief.Product
        Public Property left As Integer
        Public Property right As Integer
        Public Property strand As String
        ''' <summary>
        ''' The gene product function description
        ''' </summary>
        ''' <returns></returns>
        Public Property [function] As String
        Public Property UniprotSwissProt As String
        Public Property UniprotTrEMBL As String
        Public Property ProteinId As String
        Public Property GI As String
        ''' <summary>
        ''' The Go term id list of current protein object
        ''' </summary>
        ''' <returns></returns>
        Public Property GO As String()
        Public Property InterPro As String()
        ''' <summary>
        ''' The KEGG Ortholog feature
        ''' </summary>
        ''' <returns></returns>
        Public Property KO As String
        Public Property species As String
        Public Property GC_Content As Double
        ''' <summary>
        ''' 基因序列
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CDS As String
        ''' <summary>
        ''' 该蛋白质所位于的基因组的在NCBI之中的编号信息
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpeciesAccessionID As String
        Public Property Translation As String
        Public Property Transl_table As String

        ''' <summary>
        ''' The COG feature
        ''' </summary>
        ''' <returns></returns>
        Public Property COG As String Implements IFeatureDigest.Feature
        Public Property length As Integer Implements IGeneBrief.Length

        Public Property Location As NucleotideLocation Implements IGeneBrief.Location
            Get
                Return New NucleotideLocation(left, right, Strand:=strand)
            End Get
            Set(value As NucleotideLocation)
                left = value.left
                right = value.right
                strand = If(value.Strand = Strands.Forward, "+", "-")
                length = value.FragmentSize
            End Set
        End Property

        Public Property EC_Number As String

        Public Overrides Function ToString() As String
            Return locus_id & ": " & commonName
        End Function
    End Class
End Namespace
