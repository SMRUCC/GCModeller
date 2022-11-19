#Region "Microsoft.VisualBasic::3fe072b9ab7441801fa787ac5817ef36, GCModeller\annotations\GSEA\GSEA\KOBAS\phenotype.vb"

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

    '   Total Lines: 53
    '    Code Lines: 39
    ' Comment Lines: 4
    '   Blank Lines: 10
    '     File Size: 1.73 KB


    '     Class phenotype
    ' 
    '         Properties: labels, phnameA, phnameB, sample_num, sample0
    '                     sample1
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: FromClsFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace KOBAS

    Public Class phenotype

        Public Property labels As Integer()
        Public Property phnameA As String
        Public Property phnameB As String
        Public Property sample_num As Integer
        Public Property sample0 As Integer
        Public Property sample1 As Integer

        ''' <summary>
        ''' read_cls_file
        ''' </summary>
        ''' <param name="cls"></param>
        Sub New(cls As CLS)
            If cls.numOfClasses <> 2 Then
                Throw New Exception("Sorry, only two categorical phenotype labels are allowed.")
            ElseIf cls.sampleClass.Length <> cls.numOfSamples Then
                Throw New Exception("The total of phenotype labels doesn't match with the total of samples as you described at the first line. Please check this cls file.")
            End If

            Dim labels As New List(Of Integer)

            sample_num = cls.numOfSamples

            With cls.classNameMaps.ToList
                phnameA = .Item(0).Value
                phnameB = .Item(1).Value
            End With

            For Each label As String In cls.sampleClass
                If label = phnameA Then
                    labels += 0
                    sample0 += 1
                Else
                    labels += 1
                    sample1 += 1
                End If
            Next

            Me.labels = labels
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromClsFile(path As String) As phenotype
            Return New phenotype(CLS.ParseFile(path))
        End Function
    End Class
End Namespace
