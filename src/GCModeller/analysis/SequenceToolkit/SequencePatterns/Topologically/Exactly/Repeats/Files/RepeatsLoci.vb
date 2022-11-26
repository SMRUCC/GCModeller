#Region "Microsoft.VisualBasic::69bf8ec4e4fdc0b161a3c09bca960139, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\Repeats\Files\RepeatsLoci.vb"

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

    '   Total Lines: 155
    '    Code Lines: 96
    ' Comment Lines: 43
    '   Blank Lines: 16
    '     File Size: 5.96 KB


    '     Class ReverseRepeats
    ' 
    '         Properties: RepeatLoci, RevSegment
    ' 
    '         Function: CreateDocument, GenerateDocumentSegment, GenerateFromBase
    ' 
    '     Class Repeats
    ' 
    '         Properties: averageIntervals, length, locations, loci, numberOfRepeats
    ' 
    '         Function: CreateDocument, GenerateDocumentSegment, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically

    ''' <summary>
    ''' The reversed repeats.(反向重复序列)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ReverseRepeats : Inherits Repeats

        Public Property RevSegment As String
        ''' <summary>
        ''' The left loci of the repeat sequence.(正向的序列片段的位置集合)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RepeatLoci As Integer()

        Public Shared Function GenerateFromBase(obj As Repeats) As ReverseRepeats
            Dim seq As String = New String(obj.loci.ToArray.Reverse.ToArray)
            Return New ReverseRepeats With {
                .locations = obj.locations,
                .loci = NucleicAcid.Complement(seq),
                .RevSegment = obj.loci
            }
        End Function

        ''' <summary>
        ''' {Repeats, Rev-Repeats, Loci_left}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GenerateDocumentSegment() As Topologically.RepeatsLoci()
            Dim LQuery As RepeatsLoci() = LinqAPI.Exec(Of RepeatsLoci) <=
 _
                From revLoci As Integer
                In Me.locations
                Select From loci As Integer
                       In Me.RepeatLoci
                       Let site = New Topologically.RevRepeatsLoci With {
                           .RepeatLoci = Me.loci,
                           .LociLeft = loci,
                           .RevRepeats = Me.RevSegment,
                           .RevLociLeft = revLoci
                       }
                       Select DirectCast(site, Topologically.RepeatsLoci)

            Return LQuery
        End Function

        Public Overloads Shared Function CreateDocument(RevData As IEnumerable(Of ReverseRepeats)) As Topologically.RevRepeatsLoci()
            Dim LQuery As IEnumerable(Of RepeatsLoci) =
                LinqAPI.Exec(Of RepeatsLoci) <= From line As ReverseRepeats
                                                In RevData.AsParallel
                                                Select line.GenerateDocumentSegment
            Return (From loci As RepeatsLoci
                    In LQuery
                    Where Not loci Is Nothing
                    Select loci
                    Group loci By loci.__hash Into Group).Select(Function(loci) DirectCast(loci.Group.First, RevRepeatsLoci)).ToArray
        End Function
    End Class

    ''' <summary>
    ''' The sequence segment of the nucleotide repeats.(重复的核酸片段)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Repeats : Implements IPolymerSequenceModel

        ''' <summary>
        ''' The Repeats sequence data.(重复序列)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property loci As String Implements IPolymerSequenceModel.SequenceData

        Public ReadOnly Property length As Integer
            Get
                Return Len(loci)
            End Get
        End Property

        ''' <summary>
        ''' 重复序列的左端的位置的集合
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property locations As Integer()

        Public ReadOnly Property numberOfRepeats As Integer
            Get
                If locations.IsNullOrEmpty Then
                    Return 0
                Else
                    Return locations.Count
                End If
            End Get
        End Property

        ''' <summary>
        ''' Average Bytes interval between the loci locations
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 相邻的位点两两相减得到间隔长度后取平均值
        ''' </remarks>
        Public ReadOnly Property averageIntervals As Double
            Get
                Return locations _
                    .OrderBy(Function(b) b) _
                    .SlideWindows(2) _
                    .Select(Function(t) t(1) - t(0)) _
                    .Average
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return loci
        End Function

        ''' <summary>
        ''' 视图2：生成文档片段(Repeats, loci_left)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GenerateDocumentSegment() As Topologically.RepeatsLoci()
            Return Me.locations _
                .Select(Function(loci)
                            Return New Topologically.RepeatsLoci With {
                                .LociLeft = loci,
                                .RepeatLoci = Me.loci
                            }
                        End Function) _
                .ToArray
        End Function

        Public Shared Function CreateDocument(data As IEnumerable(Of Repeats)) As Topologically.RepeatsLoci()
            Dim LQuery = (From line As Repeats
                          In data.AsParallel
                          Select line.GenerateDocumentSegment).ToArray.Unlist
            Return (From loci As RepeatsLoci
                    In LQuery
                    Where Not loci Is Nothing
                    Select loci
                    Group loci By loci.__hash Into Group).Select(Function(loci) loci.Group.First).ToArray
        End Function
    End Class
End Namespace
