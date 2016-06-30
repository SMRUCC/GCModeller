Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.CommandLine.Reflection

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