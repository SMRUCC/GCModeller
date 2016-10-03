#Region "Microsoft.VisualBasic::3112154aba80b258ce589daa6537ba14, ..\GCModeller\shared\docs\ExprConsistency.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

'Public Module ExprConsistency

'    Public Function ApplyProperty(data As Generic.IEnumerable(Of DocumentFormat.Transcript), rawExpr As Dictionary(Of String, Integer)) As DocumentFormat.Transcript()
'        Dim LQuery = (From Transcript In data.AsParallel Select __apply(Transcript, rawExpr)).ToArray
'        Return LQuery
'    End Function

'    Private Function __apply(Transcript As DocumentFormat.Transcript, rawExpr As Dictionary(Of String, Integer)) As DocumentFormat.Transcript
'        If rawExpr.ContainsKey(Transcript.Synonym) Then
'            Transcript.Raw = rawExpr(Transcript.Synonym)

'            Dim a = Math.Abs(Transcript.Raw - Transcript.TSSsShared) / (Transcript.Raw + 1)  '可能不表达，计数的时候没有所以为0，会出错，所以+1
'            Dim b = Math.Abs(Transcript.Raw - Transcript.TSSsShared) / (Transcript.TSSsShared + 1)

'            If a <= 0.1 OrElse b <= 0.1 Then  '是不是这样，由于转录起始位点是转录最开始的位置，后面由于调控的原因会有时不会转录下去，所以这个可能会产生转录起始位点的计数要大于基因的单位长度的raw计数
'                Transcript.ExprConsi = 1 - (a + b) / 2
'                Call Console.Write(".")
'            Else
'                Transcript.Raw = 0
'            End If
'        Else
'            If String.IsNullOrEmpty(Transcript.Synonym) Then  '基因号为空，则可能是小RNA或者新基因，不会做进一步处理
'                Transcript.Raw = Transcript.TSSsShared
'            Else
'                Transcript.Raw = 0
'            End If
'        End If

'        If Transcript.Raw = 0 Then
'            Transcript.ExprConsi = 0
'        End If

'        Return Transcript
'    End Function

'    ''' <summary>
'    ''' Ht-seq Count raw计数输出文件
'    ''' </summary>
'    ''' <param name="htseqCount"></param>
'    ''' <param name="readsAvgLen">一般的RNA-seq的样本所得到的长度大约为90bp</param>
'    ''' <returns></returns>
'    Public Function Normalize(htseqCount As String, ptt As String, readsAvgLen As Integer) As Dictionary(Of String, Integer)
'        Dim Lines = IO.File.ReadAllLines(htseqCount)
'        Dim Raw = (From line As String In Lines.AsParallel
'                   Let Tokens As String() = Strings.Split(line, vbTab)
'                   Where Not Tokens.IsNullOrEmpty
'                   Select GeneID = Tokens(Scan0), Expr = CInt(Val(Tokens(1)))).ToArray.ToDictionary(Function(obj) obj.GeneID.ToUpper, elementSelector:=Function(obj) obj.Expr)
'        Return Normalize(Raw, ptt, readsAvgLen)
'    End Function

'    Public Function Normalize(raw As Dictionary(Of String, Integer), ptt As String, readsAvgLen As Integer) As Dictionary(Of String, Integer)
'        Dim __getValue = Function(GeneID As String) If(raw.ContainsKey(GeneID), raw(GeneID), 0)
'        Dim pttFeatures = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(ptt)

'        Call $"{NameOf(Normalize)}::  {NameOf(raw)}:={raw.Count} genes, {NameOf(pttFeatures)}:={pttFeatures.NumOfProducts} features site,  {NameOf(readsAvgLen)}:={readsAvgLen} bp.".__DEBUG_ECHO

'        Dim LQuery = (From GeneObject In pttFeatures.GeneObjects.AsParallel
'                      Let Expr As Integer = CInt(__getValue(GeneObject.Synonym) / (GeneObject.Length / readsAvgLen))
'                      Select GeneObject.Synonym, Expr).ToArray
'        Dim ExprNormalized = LQuery.ToDictionary(Function(obj) obj.Synonym, elementSelector:=Function(obj) obj.Expr)

'        Return ExprNormalized
'    End Function
'End Module
