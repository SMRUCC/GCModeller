#Region "Microsoft.VisualBasic::5c4b4356e13d8f402f30d627b1534290, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Pattern\Extensions.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel

Namespace Pattern

    Public Module Extensions

        <ExportAPI("Loci.Find.Location",
                   Info:="Found out all of the loci site on the target sequence.")>
        <Extension>
        Public Function FindLocation(Sequence As I_PolymerSequenceModel, Loci As String) As Integer()
            Return FindLocation(Sequence.SequenceData, Loci)
        End Function

        ''' <summary>
        ''' Found out all of the loci site on the target sequence.
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Loci"></param>
        ''' <returns></returns>
        ''' <remarks>这个位置查找函数是OK的</remarks>
        <ExportAPI("Loci.Find.Location",
                   Info:="Found out all of the loci site on the target sequence.")>
        <Extension>
        Public Function FindLocation(Sequence As String, Loci As String) As Integer()
            Dim Locis = New List(Of Integer)
            Dim p As Integer = 1

            Do While True
                p = InStr(Start:=p, String1:=Sequence, String2:=Loci)
                If p > 0 Then
                    Call Locis.Add(p)
                    p += 1
                Else
                    Exit Do
                End If
            Loop

            Return Locis.ToArray
        End Function
    End Module
End Namespace
