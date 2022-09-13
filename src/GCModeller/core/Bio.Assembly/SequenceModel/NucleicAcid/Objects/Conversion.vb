#Region "Microsoft.VisualBasic::38354795bd7755ca1b1188bded2be181, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Objects\Conversion.vb"

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

    '   Total Lines: 133
    '    Code Lines: 83
    ' Comment Lines: 31
    '   Blank Lines: 19
    '     File Size: 5.03 KB


    '     Class Conversion
    ' 
    '         Properties: BaseDegenerateEntries, DegenerateBases, NucleotideAsChar, NucleotideConvert
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CharEnums, Equals, IsAValidDNAChar, ToChar
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' 转换包含有普通类型的基本碱基字符，还包括有简并碱基字符
    ''' </summary>
    Public NotInheritable Class Conversion

        ''' <summary>
        ''' 简并碱基以及与之所对应的DNA碱基列表
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property DegenerateBases As Dictionary(Of DNA, DNA())
        Public Shared ReadOnly Property BaseDegenerateEntries As Dictionary(Of DNA, DNA())

        Shared ReadOnly validsChars As Index(Of Char)

        Public Overloads Shared Function Equals(a As DNA, b As DNA) As Boolean
            If a = b Then
                Return True
            End If

            Dim vx As DNA() = Nothing, vy As DNA() = Nothing

            If DegenerateBases.ContainsKey(a) Then
                vx = DegenerateBases(a)
            End If
            If DegenerateBases.ContainsKey(b) Then
                vy = DegenerateBases(b)
            End If

            If vx Is Nothing Then
                ' a 不是简并碱基
                If vy Is Nothing Then
                    ' b也不是简并碱基
                    Return False
                Else
                    ' b 是简并碱基
                    Return Array.IndexOf(vy, a) > -1
                End If
            Else
                ' a 是简并碱基
                If vy Is Nothing Then
                    ' b不是简并碱基
                    Return Array.IndexOf(vx, b) > -1
                Else
                    ' b也是简并碱基
                    ' 判断二者是不是有交集？
                    For Each base In vx
                        If Array.IndexOf(vy, base) > -1 Then
                            Return True
                        End If
                    Next

                    Return False
                End If
            End If
        End Function

        Private Sub New()
        End Sub

        Shared Sub New()
            NucleotideConvert = Enums(Of DNA).ToDictionary(Function(base) base.Description.First)
            NucleotideAsChar = Enums(Of DNA) _
                .ToDictionary(Function(base) base,
                              Function(c)
                                  Return c.Description.First
                              End Function)

            ' 添加完大写的碱基字母之后再添加小写的碱基字母，这样子就可以大小写不敏感了
            For Each base In NucleotideConvert.ToArray
                If base.Key <> "-" Then
                    Call NucleotideConvert.Add(Char.ToLower(base.Key), base.Value)
                End If
            Next

            validsChars = New Index(Of Char)(NucleotideConvert.Keys)

            With New DegenerateBasesExtensions
                DegenerateBases = .DegenerateBases _
                    .ToDictionary(Function(dgBase) CharEnums(dgBase.Key),
                                  Function(bases)
                                      Return bases _
                                          .Value _
                                          .Select(AddressOf CharEnums) _
                                          .ToArray
                                  End Function)
                BaseDegenerateEntries = .BaseDegenerateEntries _
                    .ToDictionary(Function(base) CharEnums(base.Key),
                                  Function(dgBases)
                                      Return dgBases _
                                          .Value _
                                          .Select(AddressOf CharEnums) _
                                          .ToArray
                                  End Function)
            End With
        End Sub

        Public Shared Function IsAValidDNAChar(c As Char) As Boolean
            Return validsChars(c) > -1
        End Function

        ''' <summary>
        ''' 大小写不敏感
        ''' </summary>
        Public Shared ReadOnly Property NucleotideConvert As Dictionary(Of Char, DNA)

        ''' <summary>
        ''' 大小写不敏感
        ''' </summary>
        ''' <param name="base"></param>
        ''' <returns></returns>
        Public Shared Function CharEnums(base As Char) As DNA
            Return NucleotideConvert(base)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        Public Shared ReadOnly Property NucleotideAsChar As Dictionary(Of DNA, Char)

        ''' <summary>
        ''' ``<see cref="DNA"/> -> char``.(转换出来的全部都是大写字母)
        ''' </summary>
        ''' <param name="base"></param>
        ''' <returns></returns>
        Public Shared Function ToChar(base As DNA) As Char
            Return NucleotideAsChar(base)
        End Function
    End Class
End Namespace
