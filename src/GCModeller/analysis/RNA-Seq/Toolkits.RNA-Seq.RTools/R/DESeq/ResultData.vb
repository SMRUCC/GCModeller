#Region "Microsoft.VisualBasic::fcb78f38ab19a3057598809b754b1204, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\R\DESeq\ResultData.vb"

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

    '     Class ResultData
    ' 
    '         Properties: dataExpr0, locus_tag
    ' 
    '         Function: ToSample, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT

Namespace DESeq2

    ''' <summary>
    ''' 原始输出数据
    ''' </summary>
    Public Class ResultData : Inherits DESeq2Diff
        Implements IExprMAT
        Implements INamedValue

        <Meta(GetType(Double))>
        Public Property dataExpr0 As Dictionary(Of String, Double) Implements IExprMAT.dataExpr0
            Get
                If _dataExpr0 Is Nothing Then
                    _dataExpr0 = New Dictionary(Of String, Double)
                End If
                Return _dataExpr0
            End Get
            Set(value As Dictionary(Of String, Double))
                _dataExpr0 = value
                If _dataExpr0.ContainsKey("") Then  ' 会有一个空格部分，不清楚为什么
                    Call _dataExpr0.Remove("")
                End If
            End Set
        End Property

        Dim _dataExpr0 As Dictionary(Of String, Double)

        Public Overrides Property locus_tag As String Implements IExprMAT.LocusId

        Public Overrides Function ToString() As String
            Return $"{locus_tag} ---> log2FoldChange  {log2FoldChange}"
        End Function

        Public Function ToSample() As ExprMAT
            Return New ExprMAT With {
                .LocusId = Me.locus_tag,
                .dataExpr0 = dataExpr0
            }
        End Function
    End Class
End Namespace
