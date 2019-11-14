#Region "Microsoft.VisualBasic::ad6473c47315e4c877941665a8588c98, CLI_tools\c2\WGCNAWeight.vb"

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

    ' Class WGCNAWeight
    ' 
    '     Function: FindMinWeight, Generate
    ' 
    ' /********************************************************************************/

#End Region

Imports LANS.SystemsBiology.Toolkits.RNASeq.WGCNA
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Public Class WGCNAWeight : Inherits LANS.SystemsBiology.Toolkits.RNASeq.WGCNA.WGCNAWeight

    Public Shared Function Generate(WeightFile As String, RegulationDir As String, ChipData As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, Export As String) As Weight()
        Dim Weights = Microsoft.VisualBasic.DocumentFormat.Csv.DataImports.Imports(WeightFile, " ").AsDataSource(Of Weight)(False)
        Dim Pcc = LANS.SystemsBiology.Toolkits.RNASeq.PccMatrix.CreatePccMAT(ChipData, True)
        Dim LQuery = (From file In (From p In FileIO.FileSystem.GetFiles(RegulationDir, FileIO.SearchOption.SearchTopLevelOnly, "*.csv") Select Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(p)).ToArray.AsParallel
                      Select New KeyValuePair(Of String, Weight)(file.FilePath, FindMinWeight(Weights, file.AsDataSource(Of Regulation)(False), Pcc))).ToArray
        Dim csvFile As New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Call csvFile.Add(New String() {"fromNode", "ToNode", "Weight", "FilePath"})
        For Each item In LQuery
            Call csvFile.AppendLine(New String() {item.Value.FromNode, item.Value.ToNode, item.Value.Weight, item.Key})
        Next

        Call csvFile.Save(Export, False)
        Return (From item In LQuery Select item.Value).ToArray
    End Function

    ''' <summary>
    ''' 先试用PCC筛选出相关系数大于0.6的对象，在再筛选出的对象中计算出最小的WGCNA权重
    ''' </summary>
    ''' <param name="Weights"></param>
    ''' <param name="RegulationCollection"></param>
    ''' <param name="Pcc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Shared Function FindMinWeight(Weights As Weight(), RegulationCollection As Regulation(), Pcc As LANS.SystemsBiology.Toolkits.RNASeq.PccMatrix) As Weight
        RegulationCollection = (From item In RegulationCollection Where System.Math.Abs(Pcc.GetValue(item.MatchedRegulator, item.Name)) > 0.6 Select item).ToArray
        Dim LQuery = (From item In RegulationCollection
                      Let n = Weight.Find(item.MatchedRegulator, item.Name, Weights)
                      Where Not n Is Nothing
                      Select n
                      Order By n.Weight Ascending).ToArray
        If LQuery.IsNullOrEmpty Then
            Return Nothing
        Else
            Return LQuery.First
        End If
    End Function
End Class
