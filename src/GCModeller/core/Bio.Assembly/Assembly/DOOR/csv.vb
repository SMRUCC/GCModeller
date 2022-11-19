#Region "Microsoft.VisualBasic::333d0881525c996f8a8b5dabb997967a, GCModeller\core\Bio.Assembly\Assembly\DOOR\csv.vb"

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

    '   Total Lines: 73
    '    Code Lines: 54
    ' Comment Lines: 6
    '   Blank Lines: 13
    '     File Size: 2.49 KB


    '     Module DOOR_IO
    ' 
    '         Function: __generate, csv
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.DOOR

    Partial Public Module DOOR_IO

        <Extension>
        Private Function __generate(DOOR As DOOR, sId As String, trim As Boolean) As String
            Dim Genes As OperonGene() = DOOR.[Select](operonID:=sId)

            If trim Then
                If Genes.Length < 2 Then
                    Return ""
                End If
            End If

            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim LocationBuilder As StringBuilder = New StringBuilder(128)

            For Each obj In Genes
                Dim strand As String = If(
                    obj.Location.Strand = Strands.Forward,
                    "+; ",
                    "-; ")

                Call sBuilder.Append(obj.Synonym & "; ")
                Call LocationBuilder.Append(strand)
            Next
            Call sBuilder.Remove(sBuilder.Length - 2, 2)
            Call LocationBuilder.Remove(LocationBuilder.Length - 2, 2)

            Dim strData As New StringBuilder(1024)
            Call strData.Append(String.Format("{0},{1},", sId, Genes.Length))
            Call strData.Append(LocationBuilder.ToString & ",")
            Call strData.Append(sBuilder.ToString)

            Return strData.ToString
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DOOR"></param>
        ''' <param name="trim">是否移除仅含有一个基因的操纵子对象？默认不进行移除</param>
        ''' <returns></returns>
        <Extension>
        Public Function csv(DOOR As DOOR, Optional trim As Boolean = False) As String
            Dim operonIDs As String() = LinqAPI.Exec(Of String) <=
 _
                From gene As OperonGene
                In DOOR.Genes
                Select gene.OperonID
                Distinct
                Order By OperonID Ascending

            Dim LQuery = LinqAPI.MakeList(Of String) <=
 _
                From OperonID As String
                In operonIDs
                Let row As String = DOOR.__generate(OperonID, trim)
                Where Not String.IsNullOrEmpty(row)
                Select row

            Call LQuery.Insert(0, "OperonID,Counts,Direction,OperonGenes")

            Return LQuery.JoinBy(ASCII.LF)
        End Function
    End Module
End Namespace
