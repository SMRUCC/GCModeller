﻿Imports Microsoft.VisualBasic.MachineLearning.XGBoost.util

Namespace gbm

    ''' <summary>
    ''' Interface of gradient boosting model.
    ''' </summary>
    Public Interface GradBooster
        WriteOnly Property numClass As Integer

        ''' <summary>
        ''' Loads model from stream.
        ''' </summary>
        ''' <param name="reader">       input stream </param>
        ''' <param name="with_pbuffer"> whether the incoming data contains pbuffer </param>
        ''' <exceptioncref="IOException"> If an I/O error occurs </exception>
        Sub loadModel(reader As ModelReader, with_pbuffer As Boolean)

        ''' <summary>
        ''' Generates predictions for given feature vector.
        ''' </summary>
        ''' <param name="feat">        feature vector </param>
        ''' <param name="ntree_limit"> limit the number of trees used in prediction </param>
        ''' <returns> prediction result </returns>
        Function predict(feat As FVec, ntree_limit As Integer) As Double()

        ''' <summary>
        ''' Generates a prediction for given feature vector.
        ''' <para>
        ''' This method only works when the model outputs single value.
        ''' </para>
        ''' </summary>
        ''' <param name="feat">        feature vector </param>
        ''' <param name="ntree_limit"> limit the number of trees used in prediction </param>
        ''' <returns> prediction result </returns>
        Function predictSingle(feat As FVec, ntree_limit As Integer) As Double

        ''' <summary>
        ''' Predicts the leaf index of each tree. This is only valid in gbtree predictor.
        ''' </summary>
        ''' <param name="feat">        feature vector </param>
        ''' <param name="ntree_limit"> limit the number of trees used in prediction </param>
        ''' <returns> predicted leaf indexes </returns>
        Function predictLeaf(feat As FVec, ntree_limit As Integer) As Integer()
    End Interface

    Public Class GradBooster_Factory
        ''' <summary>
        ''' Creates a gradient booster from given name.
        ''' </summary>
        ''' <param name="name"> name of gradient booster </param>
        ''' <returns> created gradient booster </returns>
        Public Shared Function createGradBooster(name As String) As GradBooster
            If "gbtree".Equals(name) Then
                Return New GBTree()
            ElseIf "gblinear".Equals(name) Then
                Return New GBLinear()
            ElseIf "dart".Equals(name) Then
                Return New Dart()
            End If

            Throw New ArgumentException(name & " is not supported model.")
        End Function
    End Class

    <Serializable>
    Public MustInherit Class GBBase
        Implements GradBooster

        Public MustOverride Function predictLeaf(feat As FVec, ntree_limit As Integer) As Integer() Implements GradBooster.predictLeaf
        Public MustOverride Function predictSingle(feat As FVec, ntree_limit As Integer) As Double Implements GradBooster.predictSingle
        Public MustOverride Function predict(feat As FVec, ntree_limit As Integer) As Double() Implements GradBooster.predict
        Public MustOverride Sub loadModel(reader As ModelReader, with_pbuffer As Boolean) Implements GradBooster.loadModel
        Protected Friend num_class As Integer

        Public Overridable WriteOnly Property numClass As Integer Implements GradBooster.numClass
            Set(value As Integer)
                num_class = value
            End Set
        End Property
    End Class
End Namespace
