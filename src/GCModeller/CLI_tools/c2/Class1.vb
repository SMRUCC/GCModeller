#Region "Microsoft.VisualBasic::4c8d8eb5d6175280008eb6fd1709f827, CLI_tools\c2\Class1.vb"

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

    ' Class Class1
    ' 
    '     Function: Invoke, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Public Class Class1
    Public Shared Function Invoke(ChipData As Global.LANS.SystemsBiology.Toolkits.RNASeq.ChipData, Door As LANS.SystemsBiology.Assembly.Door.OperonView)
        Call ChipData.SetColumn(1)

        Dim file As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File

        For Each Operon In Door.Operons
            Dim OperonGenes = (From GeneBrief As LANS.SystemsBiology.Assembly.Door.GeneBrief
                               In Operon.Value
                               Select GeneBrief
                               Order By GeneBrief.Location.Left Ascending).ToArray
            Dim FirstGeneRPKM As Double

            If Operon.FirstGene.Location.Strand = LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.Strands.Forward Then
                FirstGeneRPKM = ChipData.GetValue(OperonGenes.First.Synonym)
                Dim ValueList As List(Of Double) = New List(Of Double) From {0}
                Dim GeneList As List(Of String) = New List(Of String) From {OperonGenes.First.Synonym}

                For Each Gene In OperonGenes.Skip(1)
                    Dim RPKM = ChipData.GetValue(Gene.Synonym)
                    Call ValueList.Add(RPKM - FirstGeneRPKM)
                    Call GeneList.Add(Gene.Synonym)
                Next

                Call file.Add(New String() {Operon.Key, OperonGenes.Count})
                Call file.Last.AddRange(New String() {ToString(Of String)(GeneList)})
                Call file.Last.AddRange((From n In ValueList Select CStr(n)).ToArray)
            Else
                FirstGeneRPKM = ChipData.GetValue(OperonGenes.Last.Synonym)
                Dim ValueList As List(Of Double) = New List(Of Double) From {0}
                Dim GeneList As List(Of String) = New List(Of String) From {OperonGenes.Last.Synonym}

                For i As Integer = OperonGenes.Count - 2 To 0 Step -1
                    Dim Gene = OperonGenes(i)
                    Dim RPKM = ChipData.GetValue(Gene.Synonym)
                    Call ValueList.Add(RPKM - FirstGeneRPKM)
                    Call GeneList.Add(Gene.Synonym)
                Next

                Call file.Add(New String() {Operon.Key, OperonGenes.Count})
                Call file.Last.AddRange(New String() {ToString(Of String)(GeneList)})
                Call file.Last.AddRange((From n In ValueList Select CStr(n)).ToArray)
            End If
        Next

        Call file.Save("x:\dddddd.csv", False)
        Return file
    End Function

    Private Overloads Shared Function ToString(Of T)(ArrayData As Generic.IEnumerable(Of T)) As String
        Dim sBuilder As StringBuilder = New StringBuilder
        For Each item In ArrayData
            Call sBuilder.Append(item.ToString & "; ")
        Next
        Call sBuilder.Remove(sBuilder.Length - 2, 2)

        Return sBuilder.ToString
    End Function
End Class
