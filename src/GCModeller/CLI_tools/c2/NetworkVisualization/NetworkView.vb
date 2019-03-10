#Region "Microsoft.VisualBasic::87e9ca28e3f5e50078a915930720c73d, CLI_tools\c2\NetworkVisualization\NetworkView.vb"

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

    ' Module NetworkView
    ' 
    '     Function: CreateOperonNetwork
    ' 
    '     Sub: Action
    ' 
    ' /********************************************************************************/

#End Region

Public Module NetworkView

    Public Function CreateOperonNetwork(File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Dim NetworkFile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        Call NetworkFile.AppendLine(New String() {"Regulator", "Regulation", "Operon"})
        For Each row In File.Skip(1)
            Dim regulatorId As String = Trim(row(3))
            If Not String.IsNullOrEmpty(regulatorId) Then
                Dim effect As String = Trim(row(6))
                If Not String.IsNullOrEmpty(effect) Then
                    Call NetworkFile.AppendLine(New String() {regulatorId, effect, "OperonId:" & row(0)})
                Else
                    Call NetworkFile.AppendLine(New String() {regulatorId, "regulate", "OperonId:" & row(0)})
                End If
            End If
        Next

        Return NetworkFile
    End Function

    Public Sub Action()
        For Each file In {"E:\meme_analysis_logs_result\finalView\operon_150bp.csv",
                          "E:\meme_analysis_logs_result\finalView\operon_200bp.csv",
                          "E:\meme_analysis_logs_result\finalView\operon_250bp.csv"}
            Dim fileName As String = FileIO.FileSystem.GetFileInfo(file).Name
            Call CreateOperonNetwork(file).Save("E:\meme_analysis_logs_result\network\" & fileName, False)
        Next
    End Sub
End Module
