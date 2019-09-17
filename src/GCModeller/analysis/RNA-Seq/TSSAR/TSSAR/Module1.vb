#Region "Microsoft.VisualBasic::ca9a33a3a41ce2adf805f297e88e871e, analysis\RNA-Seq\TSSAR\TSSAR\Module1.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'Imports SMRUCC.genomics.AnalysisTools.TSSAR.Skellam
'Imports Microsoft.VisualBasic.DocumentFormat.Csv

'Module Module1

'    Public Sub Annotation(s_library_P_1 As String, s_library_P_0 As String, s_library_M_1 As String, s_library_M_0 As String,
'                             genome_size As Integer, win_size As Integer, min_peak_size As Integer,
'                             R_pval_output_plus_fh As String,
'                             R_SeenRegion_fh As String,
'                             R_pval_output_minus_fh As String)

'        '# load skellam and VGAM package
'        '# suppressPackageStartupMessages(library("skellam"))
'        'suppressPackageStartupMessages(library("VGAM"))
'        'options(error=dump.frames)

'        '# load primary 5' coverage for both libraries and both strands
'        Dim library_P_1 As DataFrame = Read.Table(File:=s_library_P_1, Header:=T)
'        Dim library_P_0 As DataFrame = Read.Table(File:=s_library_P_0, Header:=T)
'        Dim library_M_1 As DataFrame = Read.Table(File:=s_library_M_1, Header:=T)
'        Dim library_M_0 As DataFrame = Read.Table(File:=s_library_M_0, Header:=T)

'        '# calculate read count over whole genome for both libraries
'        Dim sum_P = Sum(C(library_P_1("coverage"), library_P_0("coverage")), NaRM:=True)
'        Dim sum_M = Sum(C(library_M_1("coverage"), library_M_0("coverage")), NaRM:=True)

'        '# calculates normalization factors to nomalize the bigger library to the niveau of the smaller one
'        Dim normalize_P = If((sum_P >= sum_M), (sum_M / sum_P), 1)
'        Dim normalize_M = If((sum_M >= sum_P), (sum_P / sum_M), 1)

'        ' # specify parameter in use
'        Dim step_size = ceiling(win_size / 10)

'        '  # Calculation for (+) strand
'        '  # initialize variables
'        Dim PVAL_1 = Array(NA, [dim]:=C(genome_size, ceiling(win_size / step_size) + 1))
'        Dim TSS_1 = Array(NA, [dim]:=C(genome_size, 3))
'        Dim SeenRegion_1 = Vector()

'        ' # sliding window
'        Dim pos = 1
'        Do While (pos < genome_size)

'            Dim lambda_PP = 9999
'            Dim lambda_MM = 9999
'            Dim lrt0_PP = 9999
'            Dim lrt0_MM = 9999

'            Dim fit1P = 9999
'            Dim pfit1P = 9999
'            Dim pstr0P = 9999
'            Dim fit1M = 9999
'            Dim pfit1M = 9999
'            Dim pstr0M = 9999

'            Dim P As Vector = PrimitiveAPI.Vector()
'            Dim PP As Vector = PrimitiveAPI.Vector()
'            Dim PPP As Vector = PrimitiveAPI.Vector()
'            Dim M As Vector = PrimitiveAPI.Vector()
'            Dim MM As Vector = PrimitiveAPI.Vector()
'            Dim MMM As Vector = PrimitiveAPI.Vector()

'            Dim expected_value_P = 9999
'            Dim expected_value_M = 9999

'            '  # extract sub-vector covering the sliding window
'            P = (library_P_1("coverage")(pos, Min(genome_size, (pos + win_size - 1))))
'            M = (library_M_1("coverage")(pos, Min(genome_size, (pos + win_size - 1))))

'            '   # calculate lambda for each library for the current window
'              if (Not (length(P(P<>0))=0 or length(PPmt(=0))=0))  then 
'                PP = P
'                PP(order(PP)(Length(PP))) = sort(PP)(Length(PP) - 1) ' # winsorizer; ev. 2. wert ersetzen??
'                PP(order(PP)(1)) = sort(PP)(2)            '# winsorizer

'                ' #set.seed(123) 
'                Dim zdata = Data.frame(x2 = runif(nn = Length(PP)))
'                zdata = transform(zdata,
'                                   pstr01 = logit(-0.5 + 1 * x2, inverse:=True),
'                                   Ps01 = logit(-0.5, inverse:=True),
'                                   lambda1 = loge(-0.5 + 2 * x2, inverse:=True))
'                zdata = transform(zdata, y1 = PP)

'            [try]( Sub () fit1P      = vglm(y1 ~ x2, zapoisson(zero = 1), zdata, crit = "coef"), silent:=TRUE)
'            [try](Sub () predict(fit1P, zdata(1, )) -> pfit1P, silent:=TRUE)
'                [Try](Sub() pstr0P = logit(pfit1P(1), inverse = True), silent:=True)
'                [Try](Sub() lambda_PP = loge(pfit1P(2), inverse = True), silent:=True)
'                [Try](Sub() lrt0_PP = pstr0P / dzipois(x = 0, lambda = lambda_PP, pstr0 = pstr0P), silent:=True)

'                [Try](Sub() expected_value_P = Length(PP(PP = 0)) * lrt0_PP, silent:=True)
'            ElseIf (Length(P(P = 0)) = 0) Then
'                PP = P
'                PP(order(PP)(Length(PP))) = sort(PP)(Length(PP) - 1) '# winsorizer; ev. 2. wert ersetzen??
'                PP(order(PP)(1)) = sort(PP)(2)            '# winsorizer

'                lambda_PP = mean(PP, NArm:=True)

'                expected_value_P = 0
'            ElseIf (Length(P(P <> 0)) = 0) Then
'                PP = P

'                lambda_PP = 0

'                expected_value_P = Length(PP)
'            End If

'            If (Not (Length(M(M <> 0)) = 0 Or Length(M(M = 0)) = 0)) Then
'                MM = M
'                MM(order(MM)(Length(MM))) = sort(MM)(Length(MM) - 1) ' # winsorizer; ev. 2. wert ersetzen??
'                MM(order(MM)(1)) = sort(MM)(2)                     '# winsorizer

'                ' #set.seed(123)
'                Dim zdata = Data.frame(x2 = runif(nn = Length(MM)))
'                zdata = transform(zdata,
'                                   pstr01 = logit(-0.5 + 1 * x2, inverse = True),
'                                   Ps01 = logit(-0.5, inverse = True),
'                                   lambda1 = loge(-0.5 + 2 * x2, inverse = True))
'                zdata = transform(zdata, y1 = MM)

'             [try]( Sub ()  fit1M      = vglm(y1 ~ x2, zapoisson(zero = 1), zdata, crit = "coef"), silent:=TRUE)
'            [try]( Sub ()  predict(fit1M, zdata(1, )) -> pfit1M, silent:=TRUE)
'                [Try](Sub() pstr0M = logit(pfit1M(1), inverse = True), silent:=True)
'                [Try](Sub() lambda_MM = loge(pfit1M(2), inverse = True), silent:=True)
'                [Try](Sub() lrt0_MM = pstr0M / dzipois(x = 0, lambda = lambda_MM, pstr0 = pstr0M), silent:=True)

'             [try]( Sub () expected_value_M = length(MM(MM==0))*lrt0_MM, silent:=TRUE)
'            ElseIf (Length(M(M = 0)) = 0) Then
'                MM = M
'                MM(order(MM)(Length(MM))) = sort(MM)(Length(MM) - 1) ' # winsorizer; ev. 2. wert ersetzen??
'                MM(order(MM)(1)) = sort(MM)(2)                    ' # winsorizer

'                lambda_MM = mean(MM, NArm:=True)

'                expected_value_M = 0
'            ElseIf (Length(M(M <> 0)) = 0) Then
'                MM = M

'                lambda_MM = 0

'                expected_value_M = Length(MM)
'            End If
'            If (Not ([Is].na(expected_value_P) Or [Is].na(expected_value_M))) Then
'                If (expected_value_P <> 9999 & expected_value_M <> 9999) Then
'                    '    # store which region on the plus strand could be modeled by zapoisson
'                    SeenRegion_1(pos, Min(genome_size, (pos + win_size - 1))) = 1

'                    '  # remove inflated zeros from PP and MM and calculate lambdas
'                    expected_value_T = mean(C(expected_value_P, expected_value_M))

'                    lrt0_P = expected_value_T / Length(PP(PP = 0))
'                    zeros = which(PP = 0)
'                    ' #set.seed(123)
'                    rand = runif(Length(zeros), 0, 1)
'                    exclude = which(rand < lrt0_P)
'                    PPP = C(PP(zeros(-exclude)), PP(PP > 0))
'                    lambda_PPP = mean(PPP, narm:=True) * normalize_P

'                    lrt0_M = expected_value_T / Length(MM(MM = 0))
'                    zeros = which(MM = 0)
'                    '  #set.seed(123)
'                    rand = runif(Length(zeros), 0, 1)
'                    exclude = which(rand < lrt0_M)
'                    MMM = C(MM(zeros(-exclude)), MM(MM > 0))
'                    lambda_MMM = mean(MMM, narm:=True) * normalize_M

'                    '    # calculate the difference between the two libraries
'                    D = (P * normalize_P) - (M * normalize_M)

'                    ' # calculate the probability that each position follows a skellam distribution
'                    If (Not ([Is].na(lambda_PPP) Or [Is].na(lambda_MMM))) Then
'                        If (lambda_PPP <> 0 Or lambda_MMM <> 0) Then
'                            pval = 1 - pskellam(D - Machine.Double.xmin, lambda_PPP, lambda_MMM)
'                        Else
'                            pval = Rep(1, Length(D))
'                        End If
'                    Else
'                        pval = Rep(1, Length(D))
'                    End If

'                    ' # keep all probabilities if the corresponding position is covered
'                    '   # by more then $min_peak_size read starts
'                    For P = 1 To Length(P)
'                        If (Not ([Is].na(library_P_1("coverage")((pos + P - 1)))) & library_P_1("coverage")((pos + P - 1)) >= min_peak_size) Then
'                            PVAL_1(pos + P - 1, Match(NA, PVAL_1(pos + P - 1, ))) = pval(P)
'                        End If
'                    Next
'                    ' #PVAL_1(cbind(pos:(pos+length(pval)-1),apply(PVAL_1, 1, function(x) match(NA,x))(pos:(pos+length(pval)-1))))<-pval
'                End If
'                For P = 1 To Length(P)
'                    PVAL_1(pos + P - 1, Match(NA, PVAL_1(pos + P - 1, ))) = 1
'                Next

'            Else
'                For P = 1 To Length(P)
'                    PVAL_1(pos + P - 1, Match(NA, PVAL_1(pos + P - 1, ))) = 1
'                Next
'            End If


'            '  # move sliding window by sprintf("%d", $win_size/10)
'            pos = pos + step_size
'        Loop

'        '  # calculate the geometric mean for all the values for each position
'        For end_pos = 1 To Length(PVAL_1(, 1))
'            TSS_1(end_pos) = Exp(mean(Log(PVAL_1(end_pos, )), NA.rm = True))
'            TSS_1(end_pos, 2) = library_P_1("coverage")((end_pos))
'            TSS_1(end_pos, 3) = library_M_1("coverage")((end_pos))
'        Next

'        '   # write output for (+) strand
'        Write.table(TSS_1, File = R_pval_output_plus_fh, sep = "\t", append = False, col.names = False)


'        ' ### Calculation for (-) strand
'        ' # initialize variables
'        PVAL_0 = Array(NA, [dim]:=C(genome_size, ceiling(win_size / step_size) + 1))
'        TSS_0 = Array(NA, [dim]:=C(genome_size, 3))
'        SeenRegion_0 = Vector()

'        '  # sliding window
'        pos = 1
'        Do While (pos < genome_size)

'            lambda_PP = 9999
'            lambda_MM = 9999
'            lrt0_PP = 9999
'            lrt0_MM = 9999

'            fit1P = 9999
'            pfit1P = 9999
'            pstr0P = 9999
'            fit1M = 9999
'            pfit1M = 9999
'            pstr0M = 9999

'            p = Vector()
'            PP = Vector()
'            PPP = Vector()
'            M = Vector()
'            MM = Vector()
'            MMM = Vector()

'            expected_value_P = 9999
'            expected_value_M = 9999

'            '  # extract sub-vector covering the sliding window
'            P = (library_P_0("coverage")(pos, Min(genome_size, (pos + win_size - 1))))
'            M = (library_M_0("coverage")(pos, Min(genome_size, (pos + win_size - 1))))

'            '  # calculate lambda for each library for the current window
'            If (Not (Length(P(P <> 0)) = 0 Or Length(P(P = 0)) = 0)) Then
'                PP = p
'                PP(order(PP)(Length(PP))) = sort(PP)(Length(PP) - 1) '# winsorizer; ev. 2. wert ersetzen??
'                PP(order(PP)(1)) = sort(PP)(2)           ' # winsorizer

'                '   #set.seed(123)
'                zdata = Data.frame(x2 = runif(nn = Length(PP)))
'                zdata = transform(zdata,
'                                   pstr01 = logit(-0.5 + 1 * x2, inverse = True),
'                                   Ps01 = logit(-0.5, inverse = True),
'                                   lambda1 = loge(-0.5 + 2 * x2, inverse = True))
'                zdata = transform(zdata, y1 = PP)

'         [try]( Sub () fit1P      = vglm(y1 ~ x2, zapoisson(zero = 1), zdata, crit = "coef"), silent:=TRUE)
'         [try]( Sub ()  predict(fit1P, zdata(1, )) -> pfit1P, silent:=TRUE)
'                [Try](Sub() pstr0P = logit(pfit1P(1), inverse = True), silent:=True)
'                [Try](Sub() lambda_PP = loge(pfit1P(2), inverse = True), silent:=True)
'                [Try](Sub() lrt0_PP = pstr0P / dzipois(x = 0, lambda = lambda_PP, pstr0 = pstr0P), silent:=True)

'                [Try](Sub() expected_value_P = Length(PP(PP = 0)) * lrt0_PP, silent:=True)
'            ElseIf (Length(P(P = 0)) = 0) Then
'                PP = p
'                PP(order(PP)(Length(PP))) = sort(PP)(Length(PP) - 1) ' # winsorizer
'                PP(order(PP)(1)) = sort(PP)(2)           ' # winsorizer

'                lambda_PP = mean(PP, NA.rm = True)

'                expected_value_P = 0
'            ElseIf (Length(P(P <> 0)) = 0) Then
'                PP = p

'                lambda_PP = 0

'                expected_value_P = Length(PP)
'            End If

'            If (Not (Length(M(M <> 0)) = 0 Or Length(M(M = 0)) = 0)) Then
'                MM = M
'                MM(order(MM)(Length(MM))) = sort(MM)(Length(MM) - 1) '# winsorizer; ev. 2. wert ersetzen??
'                MM(order(MM)(1)) = sort(MM)(2)                    ' # winsorizer

'                '  #set.seed(123)
'                zdata = Data.frame(x2 = runif(nn = Length(MM)))
'                zdata = transform(zdata,
'                                   pstr01 = logit(-0.5 + 1 * x2, inverse = True),
'                                   Ps01 = logit(-0.5, inverse = True),
'                                   lambda1 = loge(-0.5 + 2 * x2, inverse = True))
'                zdata = transform(zdata, y1 = MM)

'         [try]( Sub ()  fit1M      = vglm(y1 ~ x2, zapoisson(zero = 1), zdata, crit = "coef"), silent:=TRUE)
'         [try]( Sub ()  predict(fit1M, zdata(1, )) -> pfit1M, silent:=TRUE)
'                [Try](Sub() pstr0M = logit(pfit1M(1), inverse = True), silent:=True)
'                [Try](Sub() lambda_MM = loge(pfit1M(2), inverse = True), silent:=True)
'                [Try](Sub() lrt0_MM = pstr0M / dzipois(x = 0, lambda = lambda_MM, pstr0 = pstr0M), silent:=True)

'                [Try](Sub() expected_value_M = Length(MM(MM = 0)) * lrt0_MM, silent:=True)
'            ElseIf (Length(M(M = 0)) = 0) Then
'                MM = M
'                MM(order(MM)(Length(MM))) = sort(MM)(Length(MM) - 1) '# winsorizer; ev. 2. wert ersetzen??
'                MM(order(MM)(1)) = sort(MM)(2)                     '# winsorizer

'                lambda_MM = mean(MM, NArm:=True)

'                expected_value_M = 0
'            Else
'                MM = M

'                lambda_MM = 0

'                expected_value_M = Length(MM)
'            End If

'            If (Not ([Is].na(expected_value_P) Or [Is].na(expected_value_M))) Then
'                If (expected_value_P <> 9999 & expected_value_M <> 9999) Then
'                    ' # store which region on the minus strand could be modeled my zapoisson
'                    SeenRegion_0(pos, Min(genome_size, (pos + win_size - 1))) = 1

'                    '  # remove inflated zeros from PP and MM and calculate lambdas
'                    expected_value_T = mean(C(expected_value_P, expected_value_M))

'                    lrt0_P = expected_value_T / Length(PP(PP = 0))
'                    zeros = which(PP = 0)
'                    '   #set.seed(123)
'                    rand = runif(Length(zeros), 0, 1)
'                    exclude = which(rand < lrt0_P)
'                    PPP = C(PP(zeros(-exclude)), PP(PP > 0))
'                    lambda_PPP = mean(PPP, narm:=True) * normalize_P

'                    lrt0_M = expected_value_T / Length(MM(MM = 0))
'                    zeros = which(MM = 0)
'                    '   #set.seed(123)
'                    rand = runif(Length(zeros), 0, 1)
'                    exclude = which(rand < lrt0_M)
'                    MMM = C(MM(zeros(-exclude)), MM(MM > 0))
'                    lambda_MMM = mean(MMM, NA.rm = True) * normalize_M

'                    ' # calculate the difference between the two libraries
'                    D = (p * normalize_P) - (M * normalize_M)

'                    ' # calculate the probability that each position follows a skellam distribution
'                    If (Not ([Is].na(lambda_PPP) Or [Is].na(lambda_MMM))) Then
'                        If (lambda_PPP <> 0 Or lambda_MMM <> 0) Then
'                            pval = 1 - pskellam(D - Machine.Double.xmin, lambda_PPP, lambda_MMM)
'                        Else
'                            pval = Rep(1, Length(D))
'                        End If
'                    Else
'                        pval = Rep(1, Length(D))
'                    End If

'                    ' # keep all probabilities if the corresponding position is covered
'                    '  # by more then $min_peak_size read starts
'                    For p = 1 To Length(p)
'                        If (Not ([Is].na(library_P_0("coverage")((pos + p - 1)))) & library_P_0("coverage")((pos + p - 1)) >= min_peak_size) Then
'                            PVAL_0(pos + p - 1, Match(NA, PVAL_0(pos + p - 1, ))) = pval(p)
'                        End If
'                    Next
'                    '  #PVAL_0(cbind(pos:(pos+length(pval)-1),apply(PVAL_0, 1, function(x) match(NA,x))(pos:(pos+length(pval)-1))))<-pval
'                Else
'                    For p = 1 To Length(p)
'                        PVAL_0(pos + p - 1, Match(NA, PVAL_0(pos + p - 1, ))) = 1
'                    Next
'                End If
'            Else
'                For p = 1 To Length(p)
'                    PVAL_0(pos + p - 1, Match(NA, PVAL_0(pos + p - 1, ))) = 1
'                Next
'            End If

'            '  # move sliding window by sprintf("%d", $win_size/10)
'            pos = pos + step_size
'        Loop

'        ' # Calculation for (-) strand        
'        For end_pos = 1 To Length(PVAL_0(, 1))
'            TSS_0(end_pos) = Exp(mean(Log(PVAL_0(end_pos, )), NArm:=True))
'            TSS_0(end_pos, 2) = library_P_0("coverage")((end_pos))
'            TSS_0(end_pos, 3) = library_M_0("coverage")((end_pos))
'        Next

'        ' # write output for (-) strand
'        Write.table(TSS_0, File = R_pval_output_minus_fh, sep = "\t", append = False, col.names = False)

'        '  # write output which regions could be analysed
'        SeenRegion_1(genome_size) = 1
'        SeenRegion_0(genome_size) = 1

'        Write.table(Data.frame(PLUS = SeenRegion_1, MINUS = SeenRegion_0), File = R_SeenRegion_fh)
'    End Sub
'End Module
