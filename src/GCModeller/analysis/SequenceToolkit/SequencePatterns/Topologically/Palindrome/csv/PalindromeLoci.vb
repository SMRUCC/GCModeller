#Region "Microsoft.VisualBasic::d1d21a336fc673f6ba0b58b487920d76, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Palindrome\PalindromeLoci.vb"

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

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Mathematical

Namespace Topologically

    ''' <summary>
    ''' Mirror 或者 Palindrome
    ''' </summary>
    Public Class PalindromeLoci : Inherits Contig
        Implements I_PolymerSequenceModel
        Implements ILoci
        Implements INumberTag

        ''' <summary>
        ''' 特殊的位点序列
        ''' </summary>
        ''' <returns></returns>
        Public Property Loci As String
        ''' <summary>
        ''' <see cref="NucleotideLocation.Left"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Start As Integer Implements ILoci.Left, INumberTag.Tag
        ''' <summary>
        ''' 回文序列，在生成Mirror镜像位点数据的时候是使用这个和<see cref="Loci"/>组合产生的
        ''' </summary>
        ''' <returns></returns>
        Public Property Palindrome As String
        ''' <summary>
        ''' <see cref="NucleotideLocation.Right"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property PalEnd As Integer
        ''' <summary>
        ''' 和<see cref="loci"/>相对应的反向序列
        ''' </summary>
        ''' <returns></returns>
        Public Property MirrorSite As String

        Public Property Data As Dictionary(Of String, String)

        Public ReadOnly Property Mirror As Integer
            Get
                Return Start + Len(Loci)
            End Get
        End Property

        Public ReadOnly Property Length As Integer
            Get
                Return Len(Loci)
            End Get
        End Property

        ''' <summary>
        ''' sequence data for loci value <see cref="PalindromeLoci.MappingLocation(Boolean)"/>.
        ''' (理论上这个序列属性的值应该是等于<see cref="loci"/> + <see cref="MirrorSite"/>)
        ''' </summary>
        ''' <returns></returns>
        Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

        ''' <summary>
        ''' 选出最长的片段
        ''' </summary>
        ''' <param name="sites"></param>
        ''' <returns></returns>
        Public Shared Function SelectMaxLengthSite(sites As IEnumerable(Of PalindromeLoci)) As PalindromeLoci
            Dim LQuery As PalindromeLoci = LinqAPI.DefaultFirst(Of PalindromeLoci) <=
                From site As PalindromeLoci
                In sites
                Select site
                Order By site.Length Descending

            Return LQuery
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(Start, PalEnd)
        End Function

        Public Overloads Shared Function GetMirror(seq As String) As String
            Return New String(seq.Reverse.ToArray)
        End Function

        Public Shared Function GetPalindrome(seq As String) As String
            seq = GetMirror(seq)
            seq = NucleicAcid.Complement(seq)
            Return seq
        End Function
    End Class
End Namespace
