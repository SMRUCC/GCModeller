#Region "Microsoft.VisualBasic::7bcabc931c0f852b63a847577bafd85a, GCModeller\analysis\SequenceToolkit\SNP\VCF\SNPVcf.vb"

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

    '   Total Lines: 68
    '    Code Lines: 50
    ' Comment Lines: 7
    '   Blank Lines: 11
    '     File Size: 2.20 KB


    '     Class SNPVcf
    ' 
    '         Properties: ALT, CHROM, FILTER, FORMAT, ID
    '                     INFO, POS, QUAL, REF, Sequences
    ' 
    '         Function: Load, ToString, VcfHighMutateScreens
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace VCF

    ''' <summary>
    ''' VCF文件之中的某一个位点数据行
    ''' </summary>
    Public Class SNPVcf

#Region "Site Information"

        <Column("#CHROM")>
        Public Property CHROM As String
        Public Property POS As Integer
        Public Property ID As String
        Public Property REF As String
        Public Property ALT As String
        Public Property QUAL As String
        Public Property FILTER As String
        Public Property INFO As String
        Public Property FORMAT As String

#End Region

        ''' <summary>
        ''' SNP位点数据
        ''' </summary>
        ''' <returns></returns>
        <Meta>
        Public Property Sequences As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function Load(path As String) As SNPVcf()
            Dim bufs As IEnumerable(Of String) = path.ReadAllLines.Skip(3)
            Dim out As SNPVcf() = DataImports.ImportsData(bufs).AsDataSource(Of SNPVcf)
            Return out
        End Function

        Public Shared Iterator Function VcfHighMutateScreens(path$, Optional cut# = 0.65) As IEnumerable(Of String)
            Dim i As Integer = 0

            For Each line$ In path.IterateAllLines
                If i < 4 Then
                    i += 1
                    Yield line
                Else
                    Dim tokens$() = line.Split(ASCII.TAB)
                    Dim m As Integer = tokens _
                    .Skip(9) _
                    .Select(AddressOf Val) _
                    .Where(Function(x) x > 0) _
                    .Count

                    If cut <= (m / (tokens.Length - 9)) Then
                        Yield line
                    End If
                End If
            Next
        End Function
    End Class
End Namespace
