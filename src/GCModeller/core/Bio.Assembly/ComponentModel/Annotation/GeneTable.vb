#Region "Microsoft.VisualBasic::ab16ad9d12c1b1aabb60ff690e08eba0, Bio.Assembly\ComponentModel\Annotation\GeneTable.vb"

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

    '     Class GeneTable
    ' 
    '         Properties: [Function], CDS, COG, CommonName, EC_Number
    '                     GC_Content, GeneName, GI, GO, InterPro
    '                     Left, Length, Location, LocusID, ProteinId
    '                     Right, Species, SpeciesAccessionID, Strand, Transl_Table
    '                     Translation, UniprotSwissProt, UniprotTrEMBL
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
    Public Class GeneTable
        Implements INamedValue
        Implements IGeneBrief

        ''' <summary>
        ''' 假若在GBK文件之中没有Locus_tag属性，则导出函数<see cref="DumpEXPORT"></see>会尝试使用<see cref="ProteinId"></see>来替代
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LocusID As String Implements INamedValue.Key
        Public Property GeneName As String
        Public Property CommonName As String Implements IGeneBrief.Product
        Public Property Left As Integer
        Public Property Right As Integer
        Public Property Strand As String
        Public Property [Function] As String
        Public Property UniprotSwissProt As String
        Public Property UniprotTrEMBL As String
        Public Property ProteinId As String
        Public Property GI As String
        Public Property GO As String
        Public Property InterPro As String()
        Public Property Species As String
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
        Public Property Transl_Table As String

        Public Property COG As String Implements IFeatureDigest.Feature
        Public Property Length As Integer Implements IGeneBrief.Length
        Public Property Location As NucleotideLocation Implements IGeneBrief.Location
            Get
                Return New NucleotideLocation(Left, Right, Strand:=Strand)
            End Get
            Set(value As NucleotideLocation)
                Left = value.Left
                Right = value.Right
                Strand = If(value.Strand = Strands.Forward, "+", "-")
                Length = value.FragmentSize
            End Set
        End Property

        Public Property EC_Number As String

        Public Overrides Function ToString() As String
            Return LocusID & ": " & CommonName
        End Function
    End Class
End Namespace
