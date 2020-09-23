#Region "Microsoft.VisualBasic::b2209628e58ee397279e05b2d792d68b, analysis\TRNAlignment\RulerAlignment.vb"

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

    ' Class RulerAlignment
    ' 
    '     Function: Scan, ToString
    ' 
    ' Class ScanRuler
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetEnumerator, IEnumerable_GetEnumerator
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998

Public Class RulerAlignment

    ReadOnly rulers As ScanRuler()
    ReadOnly win_size As Integer
    ReadOnly steps As Integer

    ''' <summary>
    ''' 对标尺进行分段扫描生成机器学习的训练数据集
    ''' </summary>
    ''' <param name="nt"></param>
    ''' <returns></returns>
    Public Function Scan(nt As String) As Double()
        Dim buffer As New List(Of Double)
        Dim target As New NucleicAcid(nt)

        For Each ruler In rulers
            For Each fragment As NucleicAcid In ruler
                Call DifferenceMeasurement.Sigma(fragment, nt).DoCall(AddressOf buffer.Add)
            Next
        Next

        Return buffer.ToArray
    End Function

    Public Overrides Function ToString() As String
        Return $"{rulers.Length} reference rulers with [win_size={win_size}bits and steps={steps}bits]"
    End Function

End Class

Public Class ScanRuler : Implements IEnumerable(Of NucleicAcid)

    ReadOnly framents As NucleicAcid()

    Sub New(ruler As NucleicAcid, win_size As Integer, steps As Integer)
        framents = ruler.CreateFragments(win_size, steps).ToArray
    End Sub

    Public Iterator Function GetEnumerator() As IEnumerator(Of NucleicAcid) Implements IEnumerable(Of NucleicAcid).GetEnumerator
        For Each region In framents
            Yield region
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class
