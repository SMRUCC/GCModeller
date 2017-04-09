Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' 转换包含有普通类型的基本碱基字符，还包括有简并碱基字符
    ''' </summary>
    Public NotInheritable Class Conversion

        Public Shared ReadOnly Property DegenerateBases As Dictionary(Of DNA, DNA())
        Public Shared ReadOnly Property BaseDegenerateEntries As Dictionary(Of DNA, DNA())

        Shared ReadOnly validsChars As IndexOf(Of Char)

        Private Sub New()
        End Sub

        Shared Sub New()
            NucleotideConvert = Enums(Of DNA).ToDictionary(Function(base) base.Description.First)
            NucleotideAsChar = Enums(Of DNA) _
                .ToDictionary(Function(base) base,
                              Function(c) c.Description.First)

            ' 添加完大写的碱基字母之后再添加小写的碱基字母，这样子就可以大小写不敏感了
            For Each base In NucleotideConvert.ToArray
                If base.Key <> "-" Then
                    Call NucleotideConvert.Add(Char.ToLower(base.Key), base.Value)
                End If
            Next

            validsChars = New IndexOf(Of Char)(NucleotideConvert.Keys)

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