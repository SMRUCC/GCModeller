#Region "Microsoft.VisualBasic::eb0448a4873832fd1cdeba0e98c1304a, RDotNet.Extensions.Bioinformatics\Declares\genetics\API.vb"

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

    '     Module API
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: dataframe, genotype
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API.base
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.RScripts

Namespace genetics

    Public Module API

        ''' <summary>
        ''' ``genotype`` creates a genotype object.
        ''' </summary>
        ''' <param name="a1">vector(s) or matrix containing two alleles for each individual. See details, below</param>
        ''' <param name="a2">vector(s) or matrix containing two alleles for each individual. See details, below</param>
        ''' <param name="alleles">names (and order if reorder="yes") of possible alleles</param>
        ''' <param name="sep">character separator or column number used to divide alleles when a1 is a vector of strings where each string holds both alleles. See below for details</param>
        ''' <param name="removeSpaces">logical indicating whether spaces and tabs will be removed from a1 and a2 before processing</param>
        ''' <param name="reorder">how should alleles within an individual be reordered. If reorder="no", use the order specified by the alleles parameter. If reorder="freq" or reorder="yes", sort alleles within each individual by observed frequency. If reorder="ascii", reorder alleles in ASCII order (alphabetical, with all upper case before lower case). The default value for genotype is "freq". The default value for haplotype is "no".</param>
        ''' <param name="allowPartialmissing">logical indicating whether one allele is permitted to be missing. When set to FALSE both alleles are set to NA when either is missing.</param>
        ''' <param name="locus">object of class locus, gene, or marker, holding information about the source of this genotype.</param>
        ''' <param name="genotypeOrder">character, vector of genotype/haplotype names so that further functions can sort genotypes/haplotypes in wanted order</param>
        ''' <returns>
        ''' The genotype class extends "factor" and haplotype extends genotype. Both classes have the following attributes:
        ''' 
        ''' + levels	
        '''     character vector Of possible genotype/haplotype values stored coded by paste( allele1, "/", allele2, sep="").
        ''' + allele.names	
        '''     character vector Of possible alleles. For a SNP, these might be c("A","T"). For a variable length dinucleotyde repeat this might be c("136","138","140","148").
        ''' + allele.map	
        '''     matrix encoding how the factor levels correspond To alleles. See the source code To allele.genotype() For how To extract allele values Using this matrix. Better yet, just use allele.genotype().
        ''' + genotypeOrder	
        '''     character, genotype/haplotype names in defined order that can used for sorting in various functions. Note that this slot stores both ordered And unordered genotypes i.e. "A/B" And "B/A".
        ''' </returns>
        ''' <remarks>
        ''' Genotype objects hold information on which gene or marker alleles were observed for different individuals. For each individual, two alleles are recorded.
        ''' The genotype Class considers the stored alleles To be unordered, i.e., "C/T" Is equivalent To "T/C". The haplotype Class considers the order Of the alleles To be significant so that "C/T" Is distinct from "T/C".
        ''' When calling genotype Or haplotype
        ''' If only a1 Is provided And Is a character vector, it Is assumed that Each element encodes both alleles. In this Case, If sep Is a character String, a1 Is assumed To be coded As ``Allele1&lt;sep>Allele2``. 
        ''' If sep Is a numeric value, it Is assumed that character locations 1:sep contain allele 1 And that remaining locations contain allele 2.
        ''' If a1 Is a matrix, it Is assumed that column 1 contains allele 1 And column 2 contains allele 2.
        ''' If a1 And a2 are both provided, Each Is assumed To contain one allele value so that the genotype For an individual Is obtained by paste(a1,a2,sep="/").
        ''' If remove.spaces Is True, (the Default) any whitespace contained In a1 And a2 Is removed When the genotypes are created. If whitespace Is used As the separator, (eg "C C", "C T", ...), be sure To Set remove.spaces To False.
        ''' When the alleles are explicitly specified using the alleles argument, all potential alleles Not present in the list will be converted to NA.
        ''' NOTE: genotype assumes that the order Of the alleles Is Not important (E.G., "A/C" == "C/A"). Use Class haplotype If order Is significant.
        ''' If genotypeOrder = NULL(the Default setting), Then expectedGenotypes Is used To Get standard sorting order. Only unique values In genotypeOrder are used, which In turns means that the first occurrence prevails. When genotypeOrder Is given some genotype names, but Not all that appear In the data, the rest (those In the data And possible combinations based On allele variants) Is automatically added at the End Of genotypeOrder. This puts "missing" genotype names at the End Of sort order. This feature Is especially useful When there are a lot Of allele variants And especially In haplotypes. See examples.
        ''' </remarks>
        Public Function genotype(a1 As String,
                                 Optional a2 As String = NULL,
                                 Optional alleles As String = NULL,
                                 Optional sep As String = "/",
                                 Optional removeSpaces As Boolean = True,
                                 Optional reorder As String = "c(""yes"", ""no"", ""default"", ""ascii"", ""freq"")",
                                 Optional allowPartialmissing As Boolean = False,
                                 Optional locus As String = NULL,
                                 Optional genotypeOrder As String = NULL) As String

            Dim tmp As String = App.NextTempName

            Call $"{tmp} <- genotype({a1}, a2={a2}, alleles={alleles}, sep={Rstring(sep)}, remove.spaces={removeSpaces.λ},
                                     reorder = {reorder},
                                     allow.partial.missing={allowPartialmissing.λ}, locus={locus},
                                     genotypeOrder={genotypeOrder})".__call
            Return tmp
        End Function

        Sub New()
            Call require("genetics")  ' 调用genetics包
        End Sub

        ''' <summary>
        ''' 将``SNP``数据转换为``LDheatmap {LDheatmap}``绘图所需要的genotype数据
        ''' </summary>
        ''' <param name="df"></param>
        ''' <returns></returns>
        <Extension>
        Public Function dataframe(df As File) As String
            Dim locis As New List(Of String)
            Dim tmp As String = App.NextTempName

            For Each col In df.Columns.Where(Function(x) x(Scan0) <> NameOf(EntityObject.ID))
                Dim loci As String = col(Scan0)
                Dim vec As String = RScripts.c(col.Skip(1).ToArray)

                Call $"{loci} <- {genotype(vec, removeSpaces:=False)}".__call
                locis += loci
            Next

            Call $"{tmp} <- data.frame({locis.JoinBy(",")})".__call

            Return tmp
        End Function
    End Module
End Namespace
