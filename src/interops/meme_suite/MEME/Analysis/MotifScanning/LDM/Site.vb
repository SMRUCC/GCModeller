#Region "Microsoft.VisualBasic::b0bfb4faf2c13d848feca5c21d7d911d, meme_suite\MEME\Analysis\MotifScanning\LDM\Site.vb"

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

    '     Class Site
    ' 
    '         Properties: Regulators
    ' 
    '         Function: CreateObject, GetDist, GetLoci
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat

Namespace Analysis.MotifScans

    <XmlType("regulon_site",
                               [Namespace]:="http://gcmodeller.org/annotationTools/meme/pwm/regprecise_sites")>
    Public Class Site : Inherits MEME.LDM.Site

        ''' <summary>
        ''' <see cref="JSON.regulator.vimssId"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("regulators")> Public Property Regulators As Integer()

        Public Shared Function CreateObject(site As MEME.LDM.Site) As Site
            Return New Site With {
                .Name = site.Name.Split("|"c).First,
                .Pvalue = site.Pvalue,
                .Site = site.Site,
                .Start = site.Start,
                .Right = site.Right
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strand">这个位点所处在的基因<see cref="Name"/>的链的方向</param>
        ''' <returns></returns>
        Public Function GetDist(strand As Strands) As Integer
            If strand = Strands.Forward Then
                Return Right
            Else
                Return Start
            End If
        End Function

        ''' <summary>
        ''' 假若这个位点的名称是基因号的话，可以使用这个方法得到基因组上面的位置，这个函数只适用于ATG上游的情况
        ''' </summary>
        ''' <returns></returns>
        Public Function GetLoci(PTT As PTT) As NucleotideLocation
            Dim g As GeneBrief = PTT(Name)
            If g Is Nothing Then
                Return Nothing
            End If
            Dim loci As NucleotideLocation = g.Location.Normalization           ' ATG上游

            If loci.Strand = Strands.Forward Then
                Dim left As Long = loci.Left - Size + Me.Start
                Dim right As Long = left + Me.Length
                Return New NucleotideLocation(left, right, Strands.Forward)
            Else
                Dim left As Long = loci.Right + Me.Right
                Dim right As Long = left + Me.Length
                Return New NucleotideLocation(left, right, Strands.Reverse)
            End If
        End Function
    End Class
End Namespace
