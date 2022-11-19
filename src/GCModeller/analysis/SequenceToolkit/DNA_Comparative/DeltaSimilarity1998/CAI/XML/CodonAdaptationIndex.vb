#Region "Microsoft.VisualBasic::e42e902967ead72d43dc9fe560732460, GCModeller\analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\CAI\XML\CodonAdaptationIndex.vb"

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

    '   Total Lines: 63
    '    Code Lines: 48
    ' Comment Lines: 8
    '   Blank Lines: 7
    '     File Size: 2.13 KB


    '     Class CodonAdaptationIndex
    ' 
    '         Properties: CAI, Name, Table
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetCodonBiasList
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace DeltaSimilarity1998.CAI.XML

    ''' <summary>
    ''' codon adaptation index profile table
    ''' </summary>
    ''' 
    <XmlRoot("codon-adaptation-index", [Namespace]:="http://gcmodeller.org/analysis/SequenceToolkit/DNA_Comparative/codon-adaptation-index.XML")>
    Public Class CodonAdaptationIndex

        <XmlElement> Public Property Name As String
        <XmlElement> Public Property CAI As Double
        <XmlElement>
        Public Property Table As CodonFrequencyCAI()

        ''' <summary>
        ''' 对<see cref="Table"></see>进行展开
        ''' </summary>
        ''' <remarks></remarks>
        Public Function GetCodonBiasList() As KeyValuePair(Of Char, CodonBias)()
            Return Table _
                .Select(Function(subItem)
                            Return subItem _
                                .BiasFrequency _
                                .Select(Function(bfrq) New KeyValuePair(Of Char, CodonBias)(
                                    subItem.AminoAcid,
                                    bfrq))
                        End Function) _
                .IteratesALL _
                .ToArray
        End Function

        Sub New()
        End Sub

        Sub New(model As RelativeCodonBiases)
            Dim cfrqData = model _
                .CodonFrequencyStatics _
                .Values

            Name = model.ToString
            CAI = model.CAI
            Table = LinqAPI.Exec(Of CodonFrequencyCAI) <=
 _
            From c As CodonFrequency
            In cfrqData
            Select New CodonFrequencyCAI With {
                .AminoAcid = c.AminoAcid,
                .BiasFrequency = c _
                    .BiasFrequency _
                    .Select(Function(cfrq) New CodonBias(cfrq.Key, cfrq.Value)) _
                    .ToArray,
                .BiasFrequencyProfile = c _
                    .BiasFrequencyProfile _
                    .Values _
                    .ToArray
            }
        End Sub
    End Class
End Namespace
