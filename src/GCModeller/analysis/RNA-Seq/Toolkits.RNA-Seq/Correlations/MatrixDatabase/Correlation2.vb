#Region "Microsoft.VisualBasic::d8ccf9f0713b566b029ea36d424b1ff9, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\MatrixDatabase\Correlation2.vb"

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

    ' Class Correlation2
    ' 
    '     Properties: Pcc, SPcc, WGCNA
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CreateFromName, GetPcc, GetPccGreaterThan, GetPccSignificantThan, GetSPcc
    '               GetWGCNAWeight, LoadAuto
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.WGCNA

''' <summary>
''' 基因表达相关性的本地文件数据库服务
''' </summary>
Public Class Correlation2 : Implements ICorrelations

    Public ReadOnly Property Pcc As PccMatrix
    Public ReadOnly Property WGCNA As WGCNAWeight
    Public ReadOnly Property SPcc As PccMatrix

    Sub New(Pcc As String, SPcc As String, WGCNA As String)
        '    Call $"Load WGCNA data from {WGCNA.ToFileURL}".__DEBUG_ECHO
        '    Dim WGCNAar = Function() FastImports(WGCNA)
        '    Dim loadWGCNA = WGCNAar.BeginInvoke(Nothing, Nothing)

        '    Call $"Load pcc data from {Pcc.ToFileURL}".__DEBUG_ECHO
        '    Me.Pcc = MatrixSerialization.Load(from:=Pcc)
        '    Call $"Load spcc data from {SPcc.ToFileURL}".__DEBUG_ECHO
        '    Me.SPcc = MatrixSerialization.Load(from:=SPcc)
        '    Me.WGCNA = WGCNAar.EndInvoke(loadWGCNA)

        '    Call "OK!".__DEBUG_ECHO
        Throw New NotImplementedException
    End Sub

    Sub New(dumpDir As String)
        Call Me.New(Pcc:=dumpDir & "/Pcc.db", SPcc:=dumpDir & "/sPcc.db", WGCNA:=dumpDir & "/CytoscapeEdges.txt")
    End Sub

    Public Function GetPcc(id1 As String, id2 As String) As Double Implements ICorrelations.GetPcc
        Return Pcc.GetValue(id1, id2)
    End Function

    Public Function GetSPcc(id1 As String, id2 As String) As Double Implements ICorrelations.GetSPcc
        Return SPcc.GetValue(id1, id2)
    End Function

    Public Function GetWGCNAWeight(id1 As String, id2 As String) As Double Implements ICorrelations.GetWGCNAWeight
        Return WGCNA.GetValue(id1, id2)
    End Function

    Public Function GetPccGreaterThan(id As String, cutoff As Double) As Dictionary(Of String, Double) Implements ICorrelations.GetPccGreaterThan
        Dim sample = Pcc(id)
        Dim array = Pcc.lstGenes
        Dim dict As New Dictionary(Of String, Double)

        For i As Integer = 0 To array.Length - 1
            If sample(i) >= cutoff Then
                Call dict.Add(array(i), sample(i))
            End If
        Next

        Return dict
    End Function

    Public Function GetPccSignificantThan(id As String, cutoff As Double) As Dictionary(Of String, Double) Implements ICorrelations.GetPccSignificantThan
        Dim sample = Pcc(id)
        Dim array = Pcc.lstGenes
        Dim dict As New Dictionary(Of String, Double)

        For i As Integer = 0 To array.Length - 1
            If Math.Abs(sample(i)) >= cutoff Then
                Call dict.Add(array(i), sample(i))
            End If
        Next

        Return dict
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="codeName">这个一般是KEGG之中的物种编号</param>
    ''' <returns></returns>
    Public Shared Function CreateFromName(codeName As String) As Correlation2
        'Call Settings.Session.Initialize()

        'Dim DIR As String = GCModeller.FileSystem.Correlations & $"/{codeName}/"
        'If Not DIR.DirectoryExists OrElse String.IsNullOrEmpty(codeName) Then
        '    Call $"{codeName} is not exists in the repository...".__DEBUG_ECHO
        '    Return Nothing
        'End If

        'Dim corr As New Correlation2(DIR)
        'Return corr
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 自动识别加载，加载失败的话会返回空值
    ''' </summary>
    ''' <param name="SpNameOrDIR"></param>
    ''' <returns></returns>
    Public Shared Function LoadAuto(SpNameOrDIR As String) As Correlation2
        'If SpNameOrDIR.DirectoryExists Then
        '    Return New Correlation2(SpNameOrDIR)
        'Else
        '    Return CreateFromName(SpNameOrDIR)
        'End If
        Throw New NotImplementedException
    End Function
End Class
