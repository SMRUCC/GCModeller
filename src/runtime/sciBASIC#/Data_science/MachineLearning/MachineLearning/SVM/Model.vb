#Region "Microsoft.VisualBasic::e96ff2f3e3eaaa0de489894a5cc49614, Data_science\MachineLearning\MachineLearning\SVM\Model.vb"

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

    '     Class Model
    ' 
    '         Properties: ClassLabels, NumberOfClasses, NumberOfSVPerClass, PairwiseProbabilityA, PairwiseProbabilityB
    '                     Parameter, Rho, SupportVectorCoefficients, SupportVectorCount, SupportVectorIndices
    '                     SupportVectors
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Equals, GetHashCode, (+2 Overloads) Read
    ' 
    '         Sub: (+2 Overloads) Write
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' 
' * SVM.NET Library
' * Copyright (C) 2008 Matthew Johnson
' * 
' * This program is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * (at your option) any later version.
' * 
' * This program is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' * 
' * You should have received a copy of the GNU General Public License
' * along with this program.  If not, see <http://www.gnu.org/licenses/>.




Imports System.IO
Imports Microsoft.VisualBasic.Text

Namespace SVM
    ''' <summary>
    ''' Encapsulates an SVM Model.
    ''' </summary>
    <Serializable>
    Public Class Model
        Friend Sub New()
        End Sub

        ''' <summary>
        ''' Parameter object.
        ''' </summary>
        Public Property Parameter As Parameter

        ''' <summary>
        ''' Number of classes in the model.
        ''' </summary>
        Public Property NumberOfClasses As Integer

        ''' <summary>
        ''' Total number of support vectors.
        ''' </summary>
        Public Property SupportVectorCount As Integer

        ''' <summary>
        ''' The support vectors.
        ''' </summary>
        Public Property SupportVectors As Node()()

        ''' <summary>
        ''' The coefficients for the support vectors.
        ''' </summary>
        Public Property SupportVectorCoefficients As Double()()

        ''' <summary>
        ''' Values in [1,...,num_training_data] to indicate SVs in the training set
        ''' </summary>
        Public Property SupportVectorIndices As Integer()

        ''' <summary>
        ''' Constants in decision functions
        ''' </summary>
        Public Property Rho As Double()

        ''' <summary>
        ''' First pairwise probability.
        ''' </summary>
        Public Property PairwiseProbabilityA As Double()

        ''' <summary>
        ''' Second pairwise probability.
        ''' </summary>
        Public Property PairwiseProbabilityB As Double()

        ' for classification only

        ''' <summary>
        ''' Class labels.
        ''' </summary>
        Public Property ClassLabels As Integer()

        ''' <summary>
        ''' Number of support vectors per class.
        ''' </summary>
        Public Property NumberOfSVPerClass As Integer()

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim test As Model = TryCast(obj, Model)
            If test Is Nothing Then Return False
            Dim same = ClassLabels.IsEqual(test.ClassLabels)
            same = same AndAlso NumberOfClasses = test.NumberOfClasses
            same = same AndAlso NumberOfSVPerClass.IsEqual(test.NumberOfSVPerClass)
            If PairwiseProbabilityA IsNot Nothing Then same = same AndAlso PairwiseProbabilityA.IsEqual(test.PairwiseProbabilityA)
            If PairwiseProbabilityB IsNot Nothing Then same = same AndAlso PairwiseProbabilityB.IsEqual(test.PairwiseProbabilityB)
            same = same AndAlso Parameter.Equals(test.Parameter)
            same = same AndAlso Rho.IsEqual(test.Rho)
            same = same AndAlso SupportVectorCoefficients.IsEqual(test.SupportVectorCoefficients)
            same = same AndAlso SupportVectorCount = test.SupportVectorCount
            same = same AndAlso SupportVectors.IsEqual(test.SupportVectors)
            Return same
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return ClassLabels.ComputeHashcode() + NumberOfClasses.GetHashCode() + NumberOfSVPerClass.ComputeHashcode() + PairwiseProbabilityA.ComputeHashcode() + PairwiseProbabilityB.ComputeHashcode() + Parameter.GetHashCode() + Rho.ComputeHashcode() + SupportVectorCoefficients.ComputeHashcode2() + SupportVectorCount.GetHashCode() + SupportVectors.ComputeHashcode2()
        End Function

        ''' <summary>
        ''' Reads a Model from the provided file.
        ''' </summary>
        ''' ''' <param name="filename">The name of the file containing the Model</param>
        ''' <returns>the Model</returns>
        Public Shared Function Read(ByVal filename As String) As Model
            Dim input = File.OpenRead(filename)

            Try
                Return Read(input)
            Finally
                input.Close()
            End Try
        End Function

        ''' <summary>
        ''' Reads a Model from the provided stream.
        ''' </summary>
        ''' ''' <param name="stream">The stream from which to read the Model.</param>
        ''' <returns>the Model</returns>
        Public Shared Function Read(ByVal stream As Stream) As Model
            Start()
            Dim input As StreamReader = New StreamReader(stream)

            ' read parameters

            Dim model As Model = New Model()
            Dim param As Parameter = New Parameter()
            model.Parameter = param
            model.Rho = Nothing
            model.PairwiseProbabilityA = Nothing
            model.PairwiseProbabilityB = Nothing
            model.ClassLabels = Nothing
            model.NumberOfSVPerClass = Nothing
            Dim headerFinished = False

            While Not headerFinished
                Dim line As String = input.ReadLine()
                Dim cmd, arg As String
                Dim splitIndex = line.IndexOf(" "c)

                If splitIndex >= 0 Then
                    cmd = line.Substring(0, splitIndex)
                    arg = line.Substring(splitIndex + 1)
                Else
                    cmd = line
                    arg = ""
                End If

                arg = arg.ToLower()
                Dim i, n As Integer

                Select Case cmd
                    Case "svm_type"
                        param.SvmType = CType([Enum].Parse(GetType(SvmType), arg.ToUpper()), SvmType)
                    Case "kernel_type"
                        If Equals(arg, "polynomial") Then arg = "poly"
                        param.KernelType = CType([Enum].Parse(GetType(KernelType), arg.ToUpper()), KernelType)
                    Case "degree"
                        param.Degree = Integer.Parse(arg)
                    Case "gamma"
                        param.Gamma = Double.Parse(arg)
                    Case "coef0"
                        param.Coefficient0 = Double.Parse(arg)
                    Case "nr_class"
                        model.NumberOfClasses = Integer.Parse(arg)
                    Case "total_sv"
                        model.SupportVectorCount = Integer.Parse(arg)
                    Case "rho"
                        n = CInt(model.NumberOfClasses * (model.NumberOfClasses - 1) / 2)
                        model.Rho = New Double(n - 1) {}
                        Dim rhoParts As String() = arg.Split()

                        For i = 0 To n - 1
                            model.Rho(i) = Double.Parse(rhoParts(i))
                        Next

                    Case "label"
                        n = model.NumberOfClasses
                        model.ClassLabels = New Integer(n - 1) {}
                        Dim labelParts As String() = arg.Split()

                        For i = 0 To n - 1
                            model.ClassLabels(i) = Integer.Parse(labelParts(i))
                        Next

                    Case "probA"
                        n = CInt(model.NumberOfClasses * (model.NumberOfClasses - 1) / 2)
                        model.PairwiseProbabilityA = New Double(n - 1) {}
                        Dim probAParts As String() = arg.Split()

                        For i = 0 To n - 1
                            model.PairwiseProbabilityA(i) = Double.Parse(probAParts(i))
                        Next

                    Case "probB"
                        n = CInt(model.NumberOfClasses * (model.NumberOfClasses - 1) / 2)
                        model.PairwiseProbabilityB = New Double(n - 1) {}
                        Dim probBParts As String() = arg.Split()

                        For i = 0 To n - 1
                            model.PairwiseProbabilityB(i) = Double.Parse(probBParts(i))
                        Next

                    Case "nr_sv"
                        n = model.NumberOfClasses
                        model.NumberOfSVPerClass = New Integer(n - 1) {}
                        Dim nrsvParts As String() = arg.Split()

                        For i = 0 To n - 1
                            model.NumberOfSVPerClass(i) = Integer.Parse(nrsvParts(i))
                        Next

                    Case "SV"
                        headerFinished = True
                    Case Else
                        Throw New Exception("Unknown text in model file")
                End Select
            End While

            ' read sv_coef and SV

            Dim m = model.NumberOfClasses - 1
            Dim l = model.SupportVectorCount
            model.SupportVectorCoefficients = New Double(m - 1)() {}

            For i = 0 To m - 1
                model.SupportVectorCoefficients(i) = New Double(l - 1) {}
            Next

            model.SupportVectors = New Node(l - 1)() {}

            For i = 0 To l - 1
                Dim parts As String() = input.ReadLine().Trim().Split()

                For k = 0 To m - 1
                    model.SupportVectorCoefficients(k)(i) = Double.Parse(parts(k))
                Next

                Dim n = parts.Length - m
                model.SupportVectors(i) = New Node(n - 1) {}

                For j = 0 To n - 1
                    Dim nodeParts = parts(m + j).Split(":"c)
                    model.SupportVectors(i)(j) = New Node()
                    model.SupportVectors(i)(j).Index = Integer.Parse(nodeParts(0))
                    model.SupportVectors(i)(j).Value = Double.Parse(nodeParts(1))
                Next
            Next

            [Stop]()
            Return model
        End Function

        ''' <summary>
        ''' Writes a model to the provided filename.  This will overwrite any previous data in the file.
        ''' </summary>
        ''' ''' <param name="filename">The desired file</param>
        ''' ''' <param name="model">The Model to write</param>
        Public Shared Sub Write(ByVal filename As String, ByVal model As Model)
            Dim stream = File.Open(filename, FileMode.Create)

            Try
                Write(stream, model)
            Finally
                stream.Close()
            End Try
        End Sub

        ''' <summary>
        ''' Writes a model to the provided stream.
        ''' </summary>
        ''' ''' <param name="stream">The output stream</param>
        ''' ''' <param name="model">The model to write</param>
        Public Shared Sub Write(ByVal stream As Stream, ByVal model As Model)
            Start()
            Dim output As StreamWriter = New StreamWriter(stream)
            Dim param = model.Parameter
            output.Write("svm_type {0}" & ASCII.LF, param.SvmType)
            output.Write("kernel_type {0}" & ASCII.LF, param.KernelType)
            If param.KernelType = KernelType.POLY Then output.Write("degree {0}" & ASCII.LF, param.Degree)
            If param.KernelType = KernelType.POLY OrElse param.KernelType = KernelType.RBF OrElse param.KernelType = KernelType.SIGMOID Then output.Write("gamma {0:0.000000}" & ASCII.LF, param.Gamma)
            If param.KernelType = KernelType.POLY OrElse param.KernelType = KernelType.SIGMOID Then output.Write("coef0 {0:0.000000}" & ASCII.LF, param.Coefficient0)
            Dim nr_class = model.NumberOfClasses
            Dim l = model.SupportVectorCount
            output.Write("nr_class {0}" & ASCII.LF, nr_class)
            output.Write("total_sv {0}" & ASCII.LF, l)

            If True Then
                output.Write("rho")

                For i As Integer = 0 To CInt(nr_class * (nr_class - 1) / 2) - 1
                    output.Write(" {0:0.000000}", model.Rho(i))
                Next

                output.Write(ASCII.LF)
            End If

            If model.ClassLabels IsNot Nothing Then
                output.Write("label")

                For i = 0 To nr_class - 1
                    output.Write(" {0}", model.ClassLabels(i))
                Next

                output.Write(ASCII.LF)
            End If
            ' regression has probA only
            If model.PairwiseProbabilityA IsNot Nothing Then
                output.Write("probA")

                For i As Integer = 0 To CInt(nr_class * (nr_class - 1) / 2) - 1
                    output.Write(" {0:0.000000}", model.PairwiseProbabilityA(i))
                Next

                output.Write(ASCII.LF)
            End If

            If model.PairwiseProbabilityB IsNot Nothing Then
                output.Write("probB")

                For i As Integer = 0 To CInt(nr_class * (nr_class - 1) / 2) - 1
                    output.Write(" {0:0.000000}", model.PairwiseProbabilityB(i))
                Next

                output.Write(ASCII.LF)
            End If

            If model.NumberOfSVPerClass IsNot Nothing Then
                output.Write("nr_sv")

                For i = 0 To nr_class - 1
                    output.Write(" {0}", model.NumberOfSVPerClass(i))
                Next

                output.Write(ASCII.LF)
            End If

            output.Write("SV" & ASCII.LF)
            Dim sv_coef = model.SupportVectorCoefficients
            Dim SV = model.SupportVectors

            For i = 0 To l - 1

                For j = 0 To nr_class - 1 - 1
                    output.Write("{0:0.000000} ", sv_coef(j)(i))
                Next

                Dim p = SV(i)

                If p.Length = 0 Then
                    output.Write(ASCII.LF)
                    Continue For
                End If

                If param.KernelType = KernelType.PRECOMPUTED Then
                    output.Write("0:{0:0.000000}", CInt(p(0).Value))
                Else
                    output.Write("{0}:{1:0.000000}", p(0).Index, p(0).Value)

                    For j = 1 To p.Length - 1
                        output.Write(" {0}:{1:0.000000}", p(j).Index, p(j).Value)
                    Next
                End If

                output.Write(ASCII.LF)
            Next

            output.Flush()
            [Stop]()
        End Sub
    End Class
End Namespace

