#Region "Microsoft.VisualBasic::951fc3ce18a51b1262e7e555bd7809e6, analysis\RNA-Seq\Toolkits.RNA-Seq\GenePrediction\DocNodes.vb"

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

    '     Class InterestRegions
    ' 
    '         Properties: Regions
    ' 
    '         Function: __lineParser, Parser
    ' 
    '     Class lstORF
    ' 
    '         Properties: ORFs
    ' 
    '         Function: __lineParser, Parser
    ' 
    '     Class Region
    ' 
    '         Properties: Frame, LEnd, REnd, Strand
    ' 
    '     Class ORF
    ' 
    '         Properties: AvgProb, CodingFrame, Left, Right, StartProb
    '                     Strand
    ' 
    '     Class FrameShift
    ' 
    '         Properties: [To], base, From, OffSet, ShiftDirect
    '                     Strand
    ' 
    '     Class FrameShifts
    ' 
    '         Properties: FrameShifts
    ' 
    '         Function: __lineParser, Parser
    ' 
    '     Class lstGenes
    ' 
    '         Properties: PredictedGenes
    ' 
    '         Function: __getLoci, __lineParser, Parser
    ' 
    '     Class PredictedGene
    ' 
    '         Properties: [Class]
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.Loci.LocusExtensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Namespace GenePrediction.DocNodes

    ''' <summary>
    ''' List of Regions of interest
    ''' (regions from stop to stop codon w/ a signal in between)
    ''' </summary>
    Public Class InterestRegions
        <XmlElement> Public Property Regions As Region()

        Public Shared Function Parser(Tokens As String()) As InterestRegions
            Dim i As Integer = Tokens.Lookup("List of Regions of interest") + 5
            Dim line As String = ""
            Dim lstRegion As New List(Of Region)

            Do While Not String.IsNullOrEmpty(Tokens.Read(i, line))
                Dim region As Region = __lineParser(line)

                If Not region Is Nothing Then
                    Call lstRegion.Add(region)
                End If
            Loop

            Return New InterestRegions With {
                .Regions = lstRegion.ToArray
            }
        End Function

        Private Shared Function __lineParser(line As String) As Region
            If String.IsNullOrEmpty(line) Then
                Return Nothing
            End If

            Dim Tokens As String() = Regex.Split(line, "\s+")
            If Tokens.Length <= 4 Then
                Return Nothing
            End If

            Dim region As New Region With {
                .LEnd = CInt(Val(Tokens(1))),
                .REnd = CInt(Val(Tokens(2))),
                .Strand = Tokens(3),
                .Frame = CInt(Val(Tokens(5)))
            }
            Return region
        End Function
    End Class

    ''' <summary>
    ''' List of Open reading frames predicted as CDSs, shown with alternate starts
    ''' (regions from start to stop codon w/ coding function >0.50)
    ''' </summary>
    Public Class lstORF
        <XmlElement> Public Property ORFs As ORF()

        Public Shared Function Parser(Tokens As String()) As lstORF
            Dim i As Integer = Tokens.Lookup("List of Open reading frames predicted as CDSs") + 6
            Dim line As String = ""
            Dim lstORF As New List(Of ORF)

            Do While Not String.Equals(Tokens.Read(i, line), "List of Regions of interest")
                Dim ORF As ORF = __lineParser(line.Trim)

                If Not ORF Is Nothing Then
                    Call lstORF.Add(ORF)
                End If
            Loop

            Return New lstORF With {
                .ORFs = lstORF.ToArray
            }
        End Function

        Private Shared Function __lineParser(line As String) As ORF
            If String.IsNullOrEmpty(line) Then
                Return Nothing
            End If

            Dim Tokens As String() = Regex.Split(line, "\s+")
            If Tokens.Length <= 5 Then
                Return Nothing
            End If

            Return New ORF With {
                .Left = CInt(Val(Tokens(0))),
                .Right = CInt(Val(Tokens(1))),
                .Strand = Tokens(2),
                .CodingFrame = CInt(Val(Tokens(4))),
                .AvgProb = Val(Tokens(5)),
                .StartProb = Val(Tokens(6))
            }
        End Function
    End Class

    Public Class Region
        <XmlAttribute> Public Property LEnd As Integer
        <XmlAttribute> Public Property REnd As Integer
        <XmlAttribute> Public Property Strand As String
        <XmlAttribute> Public Property Frame As Integer
    End Class

    Public Class ORF
        ''' <summary>
        ''' Left End
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Left As Integer
        ''' <summary>
        ''' Right End
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Right As Integer
        ''' <summary>
        ''' DNA Strand
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Strand As String
        ''' <summary>
        ''' Coding Frame
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property CodingFrame As Integer
        ''' <summary>
        ''' Avg Prob
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property AvgProb As Double
        ''' <summary>
        ''' Start Prob
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property StartProb As Double
    End Class

    Public Class FrameShift
        ''' <summary>
        ''' From Frame
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property From As Integer
        ''' <summary>
        ''' To Frame
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property [To] As Integer
        ''' <summary>
        ''' At base...
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property base As String
        <XmlAttribute> Public Property ShiftDirect As String
        <XmlAttribute> Public Property OffSet As Integer
        <XmlAttribute> Public Property Strand As String
    End Class

    ''' <summary>
    ''' POSSIBLE SEQUENCE FRAMESHIFTS DETECTED
    ''' </summary>
    Public Class FrameShifts
        <XmlElement> Public Property FrameShifts As FrameShift()

        Const FLAG As String = "POSSIBLE SEQUENCE FRAMESHIFTS DETECTED"

        Public Shared Function Parser(Tokens As String()) As FrameShifts
            Dim i As Integer = Tokens.Lookup(FLAG) + 4
            Dim line As String = ""
            Dim lines As New List(Of FrameShift)

            Do While Not String.Equals(Tokens.Read(i, line), "ABOUT THE MATRIX USED:")
                Dim value As FrameShift = __lineParser(line.Trim)
                If Not value Is Nothing Then
                    Call lines.Add(value)
                End If
            Loop

            Return New FrameShifts With {
                .FrameShifts = lines.ToArray
            }
        End Function

        Private Shared Function __lineParser(line As String) As FrameShift
            If String.IsNullOrEmpty(line) Then
                Return Nothing
            End If

            Dim Tokens As String() = Regex.Split(line, "\s+")
            If Tokens.Length <= 4 Then
                Return Nothing
            End If

            Return New FrameShift With {
                .From = CInt(Val(Tokens(Scan0))),
                .To = CInt(Val(Tokens(1))),
                .base = CInt(Val(Tokens(2))),
                .ShiftDirect = Tokens(3),
                .OffSet = CInt(Val(Tokens(4))),
                .Strand = Tokens.Last
            }
        End Function
    End Class

    ''' <summary>
    ''' Predicted genes
    ''' </summary>
    Public Class lstGenes

        <XmlElement> Public Property PredictedGenes As PredictedGene()

        Public Shared Function Parser(Tokens As String()) As lstGenes
            Dim p As Integer = Tokens.Located("Predicted genes", False) + 2
            Dim line As String = ""
            Dim lstGenes As New List(Of PredictedGene)

            Do While Not Tokens.Read(p, line).Count("="c) > 10
                Dim gene As PredictedGene = __lineParser(line)

                If Not gene Is Nothing Then
                    Call lstGenes.Add(gene)
                End If
            Loop

            Return New lstGenes With {
                .PredictedGenes = lstGenes.ToArray
            }
        End Function

        Private Shared Function __lineParser(line As String) As PredictedGene
            If String.IsNullOrEmpty(line) Then
                Return Nothing
            End If

            Dim Tokens As String() = Regex.Split(line, "\s+")
            If Tokens.Length < 6 Then
                Return Nothing
            Else
                Tokens = Tokens.Skip(1).ToArray
            End If

            Dim loci As ComponentModel.Loci.NucleotideLocation = __getLoci(Tokens(2), Tokens(3), Tokens(1))
            Dim Id As String = Tokens(Scan0)
            Dim Gene As New PredictedGene With {
                .Gene = Id,
                .Location = loci,
                .Class = Tokens.Last,
                .Length = CInt(Val(Tokens(4))),
                .Code = Tokens.Last,
                .IsORF = True,
                .PID = Id,
                .Product = Id,
                .Synonym = Id
            }

            Return Gene
        End Function

        Private Shared Function __getLoci(Left As String,
                                          Right As String,
                                          Strand As String) As ComponentModel.Loci.NucleotideLocation

            Left = Regex.Match(Left, "\d+").Value
            Right = Regex.Match(Right, "\d+").Value

            Return New ComponentModel.Loci.NucleotideLocation(CInt(Val(Left)), CInt(Val(Right)), GetStrand(Strand))
        End Function
    End Class

    Public Class PredictedGene : Inherits SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief

        <XmlAttribute> Public Property [Class] As String
            Get
                Return COG
            End Get
            Set(value As String)
                COG = value
            End Set
        End Property
    End Class
End Namespace
