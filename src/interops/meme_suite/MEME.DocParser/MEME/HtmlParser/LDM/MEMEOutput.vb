#Region "Microsoft.VisualBasic::9fd80002ae4d29f75c2f82615b200dca, ..\interops\meme_suite\MEME.DocParser\MEME\HtmlParser\LDM\MEMEOutput.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace DocumentFormat.MEME.HTML

    ''' <summary>
    ''' 经过匹配MEME结果和MAST结果之后得到的一条Motif记录
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MEMEOutput : Inherits Site

        <Column("ObjectId")> Public Property ObjectId As String
        <Column("Strand")> Public Property Strand As String
        ''' <summary>
        ''' [<see cref="ObjectId"></see>][<see cref="Site.Id"></see>]
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("MatchedMotif")> Public Property MatchedMotif As String
        Public Property MatchedRegulator As String
        <Column("DoorId")> Public Property DoorOperonId As String
        <Column("MAST.E-value")> Public Property MAST_EValue As Double
        <Column("MAST.P-value")> Public Property MAST_PValue As Double

        Public Shared Function CreateObject(ObjectId As String, site As Site) As MEMEOutput
            Dim MEMEOutput As MEMEOutput = New MEMEOutput With {
                .ObjectId = ObjectId,
                .Ends = site.Ends,
                .Evalue = site.Evalue,
                .Id = site.Id,
                .InformationContent = site.InformationContent,
                .LogLikelihoodRatio = site.LogLikelihoodRatio,
                .Name = site.Name,
                .Pvalue = site.Pvalue,
                .RegularExpression = site.RegularExpression,
                .RelativeEntropy = site.RelativeEntropy,
                .Start = site.Start,
                .Width = site.Width,
                .MatchedMotif = String.Format("{0}.{1}", ObjectId, site.Id)
            }
            Return MEMEOutput
        End Function
    End Class
End Namespace
