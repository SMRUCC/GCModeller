#Region "Microsoft.VisualBasic::683a3e05a0b2ec70b71fe463b8011e6c, RDotNet.Extensions.Bioinformatics\Declares\adegenet\API.vb"

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
    '         Function: df2genind
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports RDotNet.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace adegenet

    Public Module API

        ''' <summary>
        ''' Convert a ``data.frame`` of allele data to a <see cref="genind"/> object.
        ''' 
        ''' The function ``df2genind`` converts a ``data.frame`` (or a matrix) into a ``genind`` object. The ``data.frame`` must meet the following requirements:
        ''' 
        ''' - genotypes are in row (one row per genotype)
        ''' - markers/loci are in columns
        ''' - each element Is a string of characters coding alleles, ideally separated by a character string (argument sep); if no separator Is used, the number of characters coding alleles must be indicated (argument ncode).
        ''' 
        ''' (这个函数在这里所返回来的是<see cref="genind"/>对象在R之中的变量的名称)
        ''' </summary>
        ''' <param name="X">a matrix or a data.frame containing allelle data only (see decription)</param>
        ''' <param name="sep">a character string separating alleles. See details.</param>
        ''' <param name="ncode">an optional integer giving the number of characters used for coding one genotype at one locus. If not provided, this is determined from data.</param>
        ''' <param name="indNames">an optional character vector giving the individuals names; if NULL, taken from rownames of X.</param>
        ''' <param name="locNames">an optional character vector giving the markers names; if NULL, taken from colnames of X.</param>
        ''' <param name="pop">an optional factor giving the population of each individual.</param>
        ''' <param name="NAchar">a vector of character strings which are to be treated as NA</param>
        ''' <param name="ploidy">an integer indicating the degree of ploidy of the genotypes.</param>
        ''' <param name="type">a character string indicating the type of marker: 'codom' stands for 'codominant' (e.g. microstallites, allozymes); 'PA' stands for 'presence/absence' markers (e.g. AFLP, RAPD).</param>
        ''' <param name="strata">an optional data frame that defines population stratifications for your samples. This is especially useful if you have a hierarchical or factorial sampling design.</param>
        ''' <param name="hierarchy">a hierarchical formula that explicitely defines hierarchical levels in your strata. see hierarchy for details.</param>
        ''' <returns>an object of the class genind for df2genind; a matrix of biallelic genotypes for genind2df</returns>
        ''' <remarks>
        ''' See genind2df to convert genind objects back to such a data.frame.
        '''
        ''' === Details for the sep argument ===
        ''' this character Is directly used In reguar expressions Like gsub, And thus require some characters To be 
        ''' preceeded by Double backslashes. For instance, "/" works but "|" must be coded As "\|".
        ''' 
        ''' #### Examples
        '''
        ''' ```R
        ''' ## simple example
        ''' df &lt;- data.frame(locusA=c("11","11","12","32"),
        ''' locusB=c(NA,"34","55","15"),locusC=c("22","22","21","22"))
        ''' row.names(df) &lt;- .genlab("genotype",4)
        ''' df
        '''
        ''' obj &lt;- df2genind(df, ploidy=2, ncode=1)
        ''' obj
        ''' obj@tab
        '''
        ''' ## converting a genind as data.frame
        ''' genind2df(obj)
        ''' genind2df(obj, sep="/")
        ''' ```
        ''' </remarks>
        Public Function df2genind(X As IO.File,
                                  Optional sep As String = NULL,
                                  Optional ncode As String = NULL,
                                  Optional indNames As String() = Nothing,
                                  Optional locNames As String() = Nothing,
                                  Optional pop As String = NULL,
                                  Optional NAchar As String = "",
                                  Optional ploidy As Integer = 2,
                                  Optional type As String = "c(""codom"", ""PA"")",
                                  Optional strata As String = NULL,
                                  Optional hierarchy As String = NULL) As String

            Dim tmp As String = FileIO.FileSystem.GetTempFileName.BaseName

            Call X.PushAsDataFrame(tmp, typeParsing:=True)

            Dim rowNames As String = If(indNames Is Nothing, NULL, c(indNames))
            Dim colNames As String = If(locNames Is Nothing, NULL, c(locNames))

            Call $"{tmp} <- df2genind({tmp}, sep = {sep}, ncode = {ncode}, ind.names = {rowNames},
  loc.names = {colNames}, pop = {pop}, NA.char = {Rstring(NAchar)}, ploidy = {ploidy},
  type = {type}, strata = {strata}, hierarchy = {hierarchy})".__call

            Return tmp
        End Function
    End Module
End Namespace
