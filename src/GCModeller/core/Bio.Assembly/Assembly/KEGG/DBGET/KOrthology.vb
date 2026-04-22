#Region "Microsoft.VisualBasic::32ad401bebfa9ee54d04b6227b0f46ac, core\Bio.Assembly\Assembly\KEGG\DBGET\KOrthology.vb"

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

    '   Total Lines: 58
    '    Code Lines: 36 (62.07%)
    ' Comment Lines: 9 (15.52%)
    '    - Xml Docs: 77.78%
    ' 
    '   Blank Lines: 13 (22.41%)
    '     File Size: 2.00 KB


    '     Class KOrthology
    ' 
    '         Properties: [function], EC_number, geneNames, KO_id
    ' 
    '         Function: ParseText, RequestKEGG, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.Text

Namespace Assembly.KEGG.DBGET

    ''' <summary>
    ''' KEGG Orthology
    ''' </summary>
    Public Class KOrthology

        Public Property KO_id As String
        Public Property geneNames As String()
        Public Property [function] As String

        Const EC_pattern As String = "\[EC[:]\d+.+\]"

        ' K00001  E1.1.1.1, adh; alcohol dehydrogenase [EC:1.1.1.1]
        ' K00004  BDH, butB; (R,R)-butanediol dehydrogenase / meso-butanediol dehydrogenase / diacetyl reductase [EC:1.1.1.4 1.1.1.- 1.1.1.303]

        Public ReadOnly Property EC_number As String()
            Get
                Return [function].Match(EC_pattern, RegexICSng) _
                    .GetStackValue("[", "]") _
                    .Split(":"c) _
                    .LastOrDefault _
                    .StringSplit("\s+")
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{KO_id}{vbTab}{geneNames.JoinBy(", ")}; {[function]}"
        End Function

        Public Shared Iterator Function ParseText(list_ko As String) As IEnumerable(Of KOrthology)
            For Each line As String In list_ko.LineTokens
                Yield ParseID(line)
            Next
        End Function

        Public Shared Function ParseID(line As String) As KOrthology
            Dim t As String() = line.Split(ASCII.TAB)
            Dim ko_id As String = t(0)

            t = t(1).Split(";"c)

            Return New KOrthology With {
                .KO_id = ko_id,
                .geneNames = t(0).StringSplit("\s*,\s+"),
                .[function] = t.Skip(1).JoinBy(";").Trim
            }
        End Function

        Public Shared Function ParseTerm(ko_id As String, text As String) As KOrthology
            Dim t As String() = Strings.Trim(text).Split(";"c)

            Return New KOrthology With {
                .KO_id = ko_id,
                .geneNames = t(0).StringSplit("\s*,\s+"),
                .[function] = t.Skip(1).JoinBy(";").Trim
            }
        End Function

        ''' <summary>
        ''' request of the KEGG orthology class data via the kegg rest api
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function RequestKEGG() As IEnumerable(Of KOrthology)
            Return ParseText("https://rest.kegg.jp/list/ko".GET)
        End Function

    End Class
End Namespace
