#Region "Microsoft.VisualBasic::a59472baa8ab619633393ad4cc8c3503, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\R\Wavelets.vb"

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

    ' Module Wavelets
    ' 
    '     Function: AnalysisSignals, DWT, DWT_RInvoke, Initialize, MODWT
    '               Shift
    '     Class Filter
    ' 
    '         Properties: [class], g, h, L, level
    '                     transform, wtclass, wtname
    ' 
    '     Class Waveletmodwt
    ' 
    '         Properties: aligned, attrX, boundary, classX, coe
    '                     filter, Level, nboundary, series, V
    '                     W
    ' 
    '         Function: Load, ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API.base
Imports RDotNET.SymbolicExpressionExtension

''' <summary>
''' wavelets: A package of functions for computing wavelet filters, wavelet transforms and multiresolution analyses.
''' This package contains functions for computing and plotting discrete wavelet transforms (DWT) and maximal overlap 
''' discrete wavelet transforms (MODWT), as well as their inverses. Additionally, it contains functionality for 
''' computing and plotting wavelet transform filters that are used in the above decompositions as well as 
''' multiresolution analyses.
''' </summary>
''' <remarks>
''' 信号的奇异性检测         
''' 信号的突变点和奇异点等不规则部分通常包含重要信息。一般信号的奇异性分为两种情况：
''' （1）信号在某一时刻其幅值发生突变，引起信号的非连续，这种类型的突变称为第一类型的间断点；   
''' （2）信号在外观上很光滑，幅值没有发生突变，但是信号的一阶微分有突变发生且一阶微分不连续，这种类型的突变称为第二类型的间断点。
''' </remarks>
<[Namespace]("wavelets", Description:="Wavelets tools for analysis the gene expression signal.")> Public Module Wavelets

    <ExportAPI("init()", Info:="Initis the R session for the wavelets analysis.")>
    Public Function Initialize(<Parameter("R_HOME", "The R program install location on your computer.")> Optional R_HOME As String = "") As Boolean

        Try
            If Not String.IsNullOrEmpty(R_HOME) Then
                Call TryInit(R_HOME)
            End If

            Call library("wavelets")

            Return True
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try

    End Function

    Public Class Filter
        <DataFrameColumn> Public Property L As Integer
        <DataFrameColumn("level")> Public Property level As Integer
        <DataFrameColumn("h")> Public Property h As Double()
        <DataFrameColumn("g")> Public Property g As Double()
        <DataFrameColumn("wt.class")> Public Property wtclass As String
        <DataFrameColumn("wt.name")> Public Property wtname As String
        <DataFrameColumn("transform")> Public Property transform As String
        <DataFrameColumn("class")> Public Property [class] As String
    End Class

    ''' <summary>
    ''' 小波变换的输出结果
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Waveletmodwt

        ''' <summary>
        ''' A list with element i comprised of a matrix containing the ith level wavelet coefficients.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>小波变换后的系数比较大，就表明了小波和信号的波形相似程度较大；反之则比较小</remarks>
        <DataFrameColumn> <XmlElement> Public Property W As Double()()
        ''' <summary>
        ''' A list with element i comprised of a matrix containing the ith level scaling coefficients.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn> <XmlElement> Public Property V As Double()()
        ''' <summary>
        ''' A wt.filter object containing information for the filter used in the decomposition. See help(wt.filter) for details.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn("filter")> Public Property filter As Filter
        ''' <summary>
        ''' An integer value representing the level of wavelet decomposition.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn("level")> <XmlAttribute> Public Property Level As Integer
        ''' <summary>
        ''' A numeric vector indicating the number of boundary coefficients at each level of the decomposition.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn("n.boundary")> <XmlAttribute>
        Public Property nboundary As Double()
        ''' <summary>
        ''' A character string indicating the boundary method used in the decomposition. Valid values are "periodic" or "reflection".
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn("boundary")> <XmlAttribute> Public Property boundary As String
        ''' <summary>
        ''' The original time series, X, in matrix format.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn("series")> Public Property series As Double()
        ''' <summary>
        ''' A character string indicating the class of the input series. Possible values are "ts", "mts", "numeric", "matrix", or "data.frame".
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn("class.X")> <XmlAttribute> Public Property classX As String
        ''' <summary>
        ''' A list containing the attributes information of the original time series, X. This is useful if X is an object of 
        ''' class ts or mts and it is desired to retain relevant time information. If the original time series, X, is a 
        ''' matrix or has no attributes, then attr.X is an empty list.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn("attr.X")> Public Property attrX As Object
        ''' <summary>
        ''' A logical value indicating whether the wavelet and scaling coefficients have been phase shifted so as to be aligned with relevant time information from the original series. The value of this slot is initially FALSE and can only be changed to TRUE via the align function, with the modwt object as input.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn("aligned")> <XmlAttribute> Public Property aligned As Boolean
        ''' <summary>
        ''' A logical value indicating whether the center of energy method was used in phase alignement of the wavelet and scaling coefficients. By default, this value is FALSE (and will always be FALSE when aligned is FALSE) and will be set to true if the modwt object is phase shifted via the align function and center of energy method.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataFrameColumn("coe")> <XmlAttribute> Public Property coe As Boolean

        Public Overrides Function ToString() As String
            Return series.ToString
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Expr">R的输出</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Shared Function Load(Expr As RDotNET.SymbolicExpression) As Wavelets.Waveletmodwt

            Try
                Dim attrExpr As RDotNET.SymbolicExpression
                Dim Output As Wavelets.Waveletmodwt =
                    New Waveletmodwt With {
                        .coe = Expr.GetAttribute("coe").AsLogical.First,
                        .aligned = Expr.GetAttribute("aligned").AsLogical.First,
                        .classX = Expr.GetAttribute("class.X").AsCharacter.First,
                        .Level = Expr.GetAttribute("level").AsNumeric.First
                }
                attrExpr = Expr.GetAttribute("W")
                Dim WMatrix = (From vec In attrExpr.AsList.ToArray Select (From row In vec.AsNumericMatrix.ToArray Select DirectCast(row, Double)).ToArray).ToArray
                Output.W = WMatrix

                attrExpr = Expr.GetAttribute("V")
                Dim VMatrix = (From vec In attrExpr.AsList.ToArray Select (From row In vec.AsNumericMatrix.ToArray Select DirectCast(row, Double)).ToArray).ToArray
                Output.V = VMatrix

                attrExpr = Expr.GetAttribute("n.boundary")
                Output.nboundary = (From n In attrExpr.AsList.ToArray Select n.AsNumeric.First).ToArray

                'attrExpr = Expr.GetAttribute("attr.X")
                'Output.attrX = (From n In attrExpr.AsList.ToArray Select n.AsCharacter.First).ToArray

                Return Output
            Catch ex As Exception
                Return New Waveletmodwt With {.W = New Double()() {}}
            End Try

        End Function
    End Class

    ''' <summary>
    ''' Maximal Overlap Discrete Wavelet Transform.
    ''' Computes the maximal overlap discrete wavelet transform coefficients for a univariate or multivariate time series.
    ''' 
    ''' The maximal overlap discrete wavelet transform is computed via the pyramid algorithm, using pseudocode written 
    ''' by Percival and Walden (2000), p. 178. When boundary="periodic" the resulting wavelet and scaling coefficients 
    ''' are computed without making changes to the original series - the pyramid algorithm treats X as if it is circular. 
    ''' However, when boundary="reflection" a call is made to extend.series, resulting in a new series which is reflected 
    ''' to twice the length of the original series. The wavelet and scaling coefficients are then computed by using a 
    ''' periodic boundary condition on the reflected sereis, resulting in twice as many wavelet and scaling coefficients 
    ''' at each level.
    ''' </summary>
    ''' <returns></returns>
    ''' <param name="X">A univariate or multivariate time series. Numeric vectors, matrices and data frames are also accepted.</param>
    ''' <param name="boundary">A character string indicating which boundary method to use. boundary = "periodic" and boundary = "reflection" are the only supported methods at this time.</param>
    ''' <param name="fast">A logical flag which, if true, indicates that the pyramid algorithm is computed with an internal C function. Otherwise, only R code is used in all computations.</param>
    ''' <param name="filter">Either a wt.filter object, a character string indicating which wavelet filter to use in the decomposition, or a numeric vector of wavelet coefficients (not scaling coefficients). See help(wt.filter) for acceptable filter names.</param>
    ''' <param name="nlevels">An integer specifying the level of the decomposition. By default this is the value J such that the length of X is at least as great as the length of the level J wavelet filter, but less than the length of the level J+1 wavelet filter. Thus, j &lt;= log((N-1)/(L-1)+1), where N is the length of X.</param>
    ''' <remarks>Returns an object of class modwt, which is an S4 object with slots</remarks>
    <ExportAPI("modwt", Info:="Maximal Overlap Discrete Wavelet Transform. Computes the maximal overlap discrete wavelet transform coefficients for a univariate or multivariate time series.")>
    Public Function MODWT(<Parameter("X", "A univariate or multivariate time series. Numeric vectors, matrices and data frames are also accepted.")> X As Double(),
                      <Parameter("filter", "Either a wt.filter object, a character string indicating which wavelet filter to use in the " &
                                                "decomposition, or a numeric vector of wavelet coefficients (not scaling coefficients). See help(wt.filter)" &
                                                " for acceptable filter names.")> Optional filter As String = "la8",
                      <Parameter("n.levels", "An integer specifying the level of the decomposition. By default this is the value J such that the length " &
                                                  "of X is at least as great as the length of the level J wavelet filter, but less than the length of the level " &
                                                  "J+1 wavelet filter. Thus, j &lt;= log((N-1)/(L-1)+1), where N is the length of X.")> Optional nlevels As Integer = 3,
                      <Parameter("boundary", "A character string indicating which boundary method to use. boundary = ""periodic"" and boundary = ""reflection"" " &
                                                  "are the only supported methods at this time.")> Optional boundary As String = "periodic",
                      <Parameter("fast", "A logical flag which, if true, indicates that the pyramid algorithm is computed with an internal C function. " &
                                              "Otherwise, only R code is used in all computations.")> Optional fast As Boolean = False) As Waveletmodwt

        Dim s_X As String = String.Format("X <- c({0})", String.Join(",", X))
        Dim s_nLevel As String = String.Format("n.levels <- {0}", nlevels)
        Dim s_Invoke As String = String.Format("modwt(X, filter=""{0}"", n.levels, boundary=""{1}"", fast={2})", filter, boundary, fast.ToString.ToUpper)

        SyncLock R
            With R
                .call = s_X
                .call = s_nLevel
            End With
        End SyncLock

        Dim Expr As RDotNET.SymbolicExpression = s_Invoke.__call
        Return Waveletmodwt.Load(Expr)
    End Function

    ''' <summary>
    ''' Discrete Wavelet Transform
    ''' Computes the discrete wavelet transform coefficients for a univariate or multivariate time series.
    ''' 
    ''' The discrete wavelet transform is computed via the pyramid algorithm, using pseudocode written by Percival and Walden (2000), pp. 100-101. 
    ''' When boundary="periodic" the resulting wavelet and scaling coefficients are computed without making changes to the original series - 
    ''' the pyramid algorithm treats X as if it is circular. However, when boundary="reflection" a call is made to extend.series, resulting in 
    ''' a new series which is reflected to twice the length of the original series. The wavelet and scaling coefficients are then computed by using 
    ''' a periodic boundary condition on the reflected sereis, resulting in twice as many wavelet and scaling coefficients at each level.
    ''' </summary>
    ''' <param name="X">A univariate or multivariate time series. Numeric vectors, matrices and data frames are also accepted.</param>
    ''' <param name="filter">Either a wt.filter object, a character string indicating which wavelet filter to use in the 
    ''' decomposition, or a numeric vector of wavelet coefficients (not scaling coefficients). See help(wt.filter) for acceptable filter names.</param>
    ''' <param name="nlevels">An integer specifying the level of the decomposition. By default this is the value J such that the length of X 
    ''' is at least as great as the length of the level J wavelet filter, but less than the length of the level J+1 wavelet filter. 
    ''' Thus, j &lt;= log((N-1)/(L-1)+1), where N is the length of X.</param>
    ''' <param name="boundary">A character string indicating which boundary method to use. boundary = "periodic" and boundary = "reflection" 
    ''' are the only supported methods at this time.</param>
    ''' <param name="fast">A logical flag which, if true, indicates that the pyramid algorithm is computed with an internal C function. 
    ''' Otherwise, only R code is used in all computations.</param>
    ''' <returns>Returns an object of class dwt, which is an S4 object with slots</returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("dwt", Info:="Discrete Wavelet Transform. Computes the discrete wavelet transform coefficients for a univariate or multivariate time series.")>
    Public Function DWT(X As Double(), Optional filter As String = "la8", <Parameter("n.levels")> Optional nlevels As Integer = 3, Optional boundary As String = "periodic", Optional fast As Boolean = True) As Waveletmodwt
        Dim Expr = DWT_RInvoke(X, filter, nlevels, boundary, fast)
        Return Waveletmodwt.Load(Expr)
    End Function

    Public Function DWT_RInvoke(X As Double(), Optional filter As String = "la8", Optional nlevels As Integer = 3, Optional boundary As String = "periodic", Optional fast As Boolean = True) As RDotNET.SymbolicExpression
        Dim s_X As String = String.Format("X <- c({0})", String.Join(",", X))
        Dim s_nLevel As String = String.Format("n.levels <- {0}", nlevels)
        Dim s_Invoke As String = String.Format("dwt(X, filter=""{0}"", n.levels, boundary=""{1}"", fast={2})", filter, boundary, fast.ToString.ToUpper)

        SyncLock R
            With R
                .call = s_X
                .call = s_nLevel
            End With
        End SyncLock

        Dim Expr As RDotNET.SymbolicExpression = R.Evaluate(s_Invoke)
        Return Expr
    End Function

    <ExportAPI("gene.expression.regulation.analysis")>
    Public Function AnalysisSignals(data As IO.File, Optional Sampling As Integer = 80, Optional filter As String = "haar") As IO.File
        Dim LoadData = (From row As IO.RowObject
                        In data.Skip(1).AsParallel
                        Let ID As String = row.First
                        Let ExpressionData As Double() = (From s As String In row.Skip(1) Select Val(s)).ToArray
                        Select ID, expr = BezierExtensions.BezierSmoothInterpolation(ExpressionData, Sampling)).ToArray '加载数据并进行降噪处理

        ' RDOTNET不能够使用并行？？？
        Dim wavletProcessLQuery = (From signal In LoadData Let dwt = Function() As Wavelets.Waveletmodwt
                                                                         Call Console.Write("--")
                                                                         Dim Output = Wavelets.DWT(signal.expr, filter, fast:=False)
                                                                         'Call Console.Write("[JOB DONE] " & signal.ID)
                                                                         Return Output
                                                                     End Function() Select ID = signal.ID, dwt).ToArray   '小波变换提取信号
        Dim MATLQuery = (From signal In wavletProcessLQuery.AsParallel
                         Let IDCol As String() = New String() {signal.ID}
                         Let expr As String() = (From n In signal.dwt.W.Last Select s = n.ToString).ToArray
                         Let row = New String()() {IDCol, expr}
                         Select CType(Microsoft.VisualBasic.Unlist(row), IO.RowObject)).ToArray   '重新生成数据文件
        Dim MAT = CType(MATLQuery, IO.File)
        Return MAT
    End Function

    <ExportAPI("f.diff")>
    Public Function Shift(data1 As IO.File, data2 As IO.File) As IO.File
        Dim L1 = (From row In data1.AsParallel Let ID As String = row.First Let dat = (From s As String In row.Skip(1) Select Val(s)).ToArray Select ID, dat).ToArray
        Dim L2 = (From row In data2.AsParallel Let ID As String = row.First Let dat = (From s As String In row.Skip(1) Select Val(s)).ToArray Select ID, dat).ToArray.ToDictionary(Function(obj) obj.ID, Function(obj) obj.dat)
        Dim Diff = (From row In L1.AsParallel Let row2 = L2(row.ID) Let InternalDiff = Function() As Double()
                                                                                           Dim Chunkbuffer As Double() = New Double(Math.Min(row.dat.Count, row2.Count) - 1) {}
                                                                                           For i As Integer = 0 To Chunkbuffer.Count - 1
                                                                                               Chunkbuffer(i) = row.dat(i) - row2(i)
                                                                                           Next

                                                                                           Return Chunkbuffer
                                                                                       End Function() Select row.ID, InternalDiff).ToArray
        Dim MAT = (From row In Diff Let colID = New String() {row.ID} Let data As String() = (From n In row.InternalDiff Select s = n.ToString).ToArray Let datRow As String()() = {colID, data} Select CType(datRow.Unlist, IO.RowObject)).ToArray
        Dim CSV = CType(MAT, IO.File)
        Return CSV
    End Function

End Module
