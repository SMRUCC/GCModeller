#Region "Microsoft.VisualBasic::4dce1b6911860ea7ac4fe4e501f58689, analysis\SequenceToolkit\SequencePatterns\Pattern\Extensions.vb"

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

'     Module Extensions
' 
'         Function: (+2 Overloads) FindLocation
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.SequenceModel

Namespace Pattern

    <HideModuleName> Public Module Extensions

        <ExportAPI("Loci.Find.Location", Info:="Found out all of the loci site on the target sequence.")>
        <Extension>
        Public Function FindLocation(Sequence As IPolymerSequenceModel, Loci As String) As Integer()
            Return FindLocation(Sequence.SequenceData, Loci)
        End Function

        ''' <summary>
        ''' Found out all of the loci site on the target sequence.
        ''' (使用字符串查找得到目标位点在序列之上的所有的位置集合)
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Loci"></param>
        ''' <returns></returns>
        ''' <remarks>这个位置查找函数是OK的</remarks>
        <ExportAPI("Loci.Find.Location", Info:="Found out all of the loci site on the target sequence.")>
        <Extension>
        Public Function FindLocation(Sequence As String, Loci As String) As Integer()
            Dim locis As New List(Of Integer)
            Dim p As Integer = 1  ' vb6之中下标是从1开始的

            Do While True
                ' 这里需要进行迭代查找，即在上一个位置之后查找，否则会出现无限的重复查找
                p = InStr(Start:=p, String1:=Sequence, String2:=Loci)

                If p > 0 Then
                    Call locis.Add(p)
                    p += 1
                Else
                    Exit Do
                End If
            Loop

            Return locis.ToArray
        End Function
    End Module
End Namespace
