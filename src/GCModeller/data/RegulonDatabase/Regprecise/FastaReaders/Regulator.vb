#Region "Microsoft.VisualBasic::c224e9af439c8ee15f6822be664f0708, GCModeller\data\RegulonDatabase\Regprecise\FastaReaders\Regulator.vb"

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

    '   Total Lines: 103
    '    Code Lines: 69
    ' Comment Lines: 21
    '   Blank Lines: 13
    '     File Size: 3.83 KB


    '     Class Regulator
    ' 
    '         Properties: Definition, Family, KEGG, KEGGFamily, LocusTag
    '                     Regulog, Sites, SpeciesCode
    ' 
    '         Function: __KEGGFamily, (+3 Overloads) LoadDocument, NullDictionary, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.KEGG.Archives
Imports SMRUCC.genomics.SequenceModel

Namespace Regprecise.FastaReaders

    ''' <summary>
    ''' 调控因子的蛋白质序列
    ''' > xcb:XC_1184|Family|Regulates|Regulog|Definition
    ''' </summary>
    Public Class Regulator : Inherits FASTA.FastaSeq
        Implements INamedValue

        ''' <summary>
        ''' &lt;(KEGG)species_code>:&lt;locusTag>
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGG As String Implements INamedValue.Key
            Get
                Return _kegg
            End Get
            Set(value As String)
                _kegg = value
                Dim Tokens As String() = _kegg.Split(":"c)
                _SpeciesCode = Tokens(Scan0)
                _LocusTag = Tokens(1)
            End Set
        End Property
        Public Property Sites As String()
        Public Property Family As String
        Public Property Regulog As String
        Public Property Definition As String

        Public ReadOnly Property SpeciesCode As String
        Public ReadOnly Property LocusTag As String

        Dim _kegg As String

        ''' <summary>
        ''' $"{<see cref="KEGG"/>}|{<see cref="Family"/>}|{<see cref="String.Join"/>(";", <see cref="Sites"/>)}|{<see cref="Regulog"/>}|{<see cref="Definition"/>}"
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"{KEGG}|{Family}|{String.Join(";", Sites)}|{Regulog}|{Definition}"
        End Function

        Public Shared Function LoadDocument(FastaObject As FASTA.FastaSeq) As Regulator
            Dim attributes As String() = FastaObject.Headers
            Dim RegpreciseRegulator As Regulator =
                New Regulator With {
                    .KEGG = attributes(Scan0),
                    .Headers = attributes,
                    .Family = attributes(1),
                    .Sites = Strings.Split(attributes(2), ";"),
                    .Regulog = attributes(3),
                    .SequenceData = FastaObject.SequenceData.ToUpper,
                    .Definition = attributes(4)
            }

            Return RegpreciseRegulator
        End Function

        Public Shared Function LoadDocument(FastaFile As FASTA.FastaFile) As Regulator()
            Dim LQuery As Regulator() = (From FastaObject As FASTA.FastaSeq
                                         In FastaFile.AsParallel
                                         Select Regulator.LoadDocument(FastaObject)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="fasta">文件的路径</param>
        ''' <returns></returns>
        Public Shared Function LoadDocument(fasta As String) As Regulator()
            Dim File As FASTA.FastaFile = SequenceModel.FASTA.FastaFile.Read(fasta)
            Dim regulators As Regulator() = LoadDocument(File)
            Return regulators
        End Function

        Protected Friend Shared Function NullDictionary(uniqueId As String) As Regprecise.FastaReaders.Regulator
            Return Nothing
        End Function

        ''' <summary>
        ''' 在KEGG数据库之中所注释的家族分类
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property KEGGFamily As String
            Get
                Try
                    Return __KEGGFamily()
                Catch ex As Exception
                    Return Family
                End Try
            End Get
        End Property

        Private Function __KEGGFamily() As String
            Return SequenceDump.KEGGFamily(Definition, [default]:=Me.Family)
        End Function
    End Class
End Namespace
