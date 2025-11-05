#Region "Microsoft.VisualBasic::4a949d0599bcaa73eed5209af63d9e66, meme_suite\MEME.DocParser\MEME\HtmlParser\LDM\Site.vb"

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
'         Properties: Ends, Evalue, Id, InformationContent, LogLikelihoodRatio
'                     Name, Pvalue, RegularExpression, RelativeEntropy, Start
'                     Width
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: Copy
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

Namespace DocumentFormat.MEME.HTML

    Public Class Site : Inherits SiteInfo

        <Column("SequenceId")> Public Overrides Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property
        <Column("MEME.P-value")> Public Overrides Property Pvalue As Double
            Get
                Return MyBase.Pvalue
            End Get
            Set(value As Double)
                MyBase.Pvalue = value
            End Set
        End Property
        <Column("Starts")> Public Overrides Property Start As Long
            Get
                Return MyBase.Start
            End Get
            Set(value As Long)
                MyBase.Start = value
            End Set
        End Property
        <Column("Ends")> Public Overrides Property Ends As Long
            Get
                Return MyBase.Ends
            End Get
            Set(value As Long)
                MyBase.Ends = value
            End Set
        End Property

        <Column("MotifId")> Public Property Id As Integer
        <Column("MEME.E-value")> Public Property Evalue As Double
        <Column("Width")> Public Property Width As Integer
        <Column("Log_Likelihood_Ratio")> Public Property LogLikelihoodRatio As Double
        <Column("Information_Content")> Public Property InformationContent As Double
        <Column("Relative_Entropy")> Public Property RelativeEntropy As Double
        <Column("Signature")> Public Property RegularExpression As String

        Protected Friend Function Copy(MotifInfo As Motif) As Site
            Id = MotifInfo.Id
            Evalue = MotifInfo.Evalue
            Width = MotifInfo.Width
            LogLikelihoodRatio = MotifInfo.LogLikelihoodRatio
            InformationContent = MotifInfo.InformationContent
            RelativeEntropy = MotifInfo.RelativeEntropy
            RegularExpression = MotifInfo.RegularExpression
            Return Me
        End Function

        Sub New(info As SiteInfo)
            MyBase.Name = info.Name
            MyBase.Pvalue = info.Pvalue
            MyBase.Start = info.Start
            MyBase.Ends = info.Ends
        End Sub

        Protected Sub New()
        End Sub
    End Class
End Namespace
